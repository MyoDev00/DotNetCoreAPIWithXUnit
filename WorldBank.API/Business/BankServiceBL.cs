using Microsoft.EntityFrameworkCore;
using WorldBank.Entities.DataModel;
using WorldBank.Shared.RequestModel;
using WorldBank.Shared.ResponseModel;
using WorldBank.Shared.ResponseModel.CommonResponse;
using WorldBank.UnitOfWork;
using static WorldBank.Shared.Constant.Constant;

namespace WorldBank.API.Business
{
    public interface IBankServiceBL
    {
        public Task<BaseResponse<PostDepositResponse>> PostDeposit(PostDepositRequest request,Guid staffId);
        public Task<BaseResponse<PostFundTransferResponse>> PostFundTransfer(PostFundTransferRequest request);

    }
    public class BankServiceBL : IBankServiceBL
    {
        #region Constructor
        private readonly IUnitOfWork unitOfWork;
        private decimal depositChargePercentage;
        public BankServiceBL(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        #endregion

        #region Helper
        public bool CheckAccountStatusActive(Guid customerId,Guid bankAccountId)
        {
            var customer = unitOfWork.GetRepository<Customer>()
                        .Includes(c => c.CustomerId == customerId && c.Status == (int)CommonStatus.Active
                        , ci => ci.Include(c => c.BankAccount))
                        .FirstOrDefault();
            if (customer == null)
                return false;

            var isAny = customer.BankAccount.Any(x => x.BankAccountId == bankAccountId && x.Status == (int)CommonStatus.Active);
            return isAny;
        }

        public bool CheckAccountBalance(Guid bankAccountId,decimal amount)
        {
            var isAny = unitOfWork.GetRepository<BankAccount>()
                        .GetByCondition(x=>x.BankAccountId==bankAccountId && x.ClosingBalance>=amount)
                        .Any();
            return isAny;
        }

        public decimal CalculateTransactionCharges(decimal amount,string chargeType,out decimal chargesPercentage)
        {
            chargesPercentage = unitOfWork.GetRepository<TransactionCharges>()
                                            .GetByCondition(tc => tc.ChargesType == chargeType)
                                            .Select(tc => tc.Percentage).FirstOrDefault();
            return amount*(chargesPercentage / 100);
        }

        public string GenerateTransactionNo()
        {
            var maxno = unitOfWork.GetRepository<GeneratedTransactionNumber>().GetAll().OrderByDescending(x => x.TransactionNo).FirstOrDefault();
            var newGenerateNo = new GeneratedTransactionNumber
            {
                GeneratedNo = ""
            };
            unitOfWork.GetRepository<GeneratedTransactionNumber>().Add(newGenerateNo);
            unitOfWork.SaveChanges();

            var maxNoString = (newGenerateNo.TransactionNo+1).ToString();
            var generatedNo = maxNoString.PadLeft(30, '0');
            newGenerateNo.GeneratedNo = generatedNo;

            unitOfWork.GetRepository<GeneratedTransactionNumber>().Update(newGenerateNo);
            unitOfWork.SaveChanges();
            return generatedNo;
        }
        #endregion

        #region Task
        public async Task<BaseResponse<PostDepositResponse>> PostDeposit(PostDepositRequest request,Guid staffId)
        {
            if (request.Amount < 1)
                return new BaseResponse<PostDepositResponse>(ErrorCode.InvalidAmount, ErrorMessage.InvalidAmount, ErrorFieldName.InvalidAmount);

            if (CheckAccountStatusActive(request.CustomerId, request.BankAccountId))
            {
                decimal chargesPercentage;
                var charges = CalculateTransactionCharges(request.Amount, TransactionChargeTypes.BankDeposit, out chargesPercentage);

                if(request.Amount<=chargesPercentage)
                    return new BaseResponse<PostDepositResponse>(ErrorCode.InvalidAmount, ErrorMessage.InvalidAmount, ErrorFieldName.InvalidAmount);

                var audit = unitOfWork.GetRepository<AuditTypes>()
                            .GetByCondition(x => x.AuditType == AuditType.BankDeposit).FirstOrDefault();
                var transactionType = unitOfWork.GetRepository<TransactionTypes>()
                                      .GetByCondition(x => x.TransactionType == TransactionType.BankDeposit).FirstOrDefault();
                var netAmount = request.Amount - charges;
                var tranNo = GenerateTransactionNo();

                var transaction = new Transaction
                {
                    TransactionId = Guid.NewGuid(),
                    TransactionNo = tranNo,
                    BankAccountId = request.BankAccountId,
                    CustomerId = request.CustomerId,
                    TransactionTypeId = transactionType.TransactionTypeId,
                    Amount = request.Amount,
                    NetAmount = netAmount,
                    ChargesPercentage = chargesPercentage,
                    Charges = charges,
                    Notes = request.Note,
                    CreatedOn = DateTime.Now,
                };

                var ledger = new BankAccountLedger
                {
                    LedgerId = Guid.NewGuid(),
                    BankAccountId = request.BankAccountId,
                    TransactionId = transaction.TransactionId,
                    TransactionTypeId = transaction.TransactionTypeId,
                    Credit = 0,
                    Debit = netAmount
                };

                var bankAccount = unitOfWork.GetRepository<BankAccount>()
                                  .GetByCondition(b => b.BankAccountId == request.BankAccountId)
                                  .FirstOrDefault();
                bankAccount.ClosingBalance += netAmount;
                bankAccount.TotalDebit += netAmount;
                bankAccount.UpdatedOn = DateTime.Now;

                var staffAudit = new StaffAuditLog()
                {
                    StaffAuditId = Guid.NewGuid(),
                    AuditTypeId = audit.AuditTypeId,
                    RecordId = transaction.TransactionId,
                    StaffId = staffId,
                    Note = $"{audit.AuditType},TransactionNo:{transaction.TransactionNo},Amount:{request.Amount},Charges:{charges}",
                    CreatedOn = DateTime.Now
                };

                unitOfWork.GetRepository<BankAccount>().Update(bankAccount);
                unitOfWork.GetRepository<Transaction>().Add(transaction);
                unitOfWork.GetRepository<BankAccountLedger>().Add(ledger);
                unitOfWork.GetRepository<StaffAuditLog>().Add(staffAudit);
                unitOfWork.SaveChanges();

            
                var response = new BaseResponse<PostDepositResponse>();
                response.Responsedata = new PostDepositResponse
                {
                    BankAccountId = request.BankAccountId,
                    CustomerId = request.CustomerId,
                    TransactionId = transaction.TransactionId,
                    TransactionNo = transaction.TransactionNo,
                    Charges = transaction.Charges,
                    ClosingBalance = bankAccount.ClosingBalance,
                    TotalCredit = bankAccount.TotalCredit,
                    TotalDebit = bankAccount.TotalDebit,
                    UpdatedOn = bankAccount.UpdatedOn.Value
                };

                return response;
            }
            else
            {
                return new BaseResponse<PostDepositResponse>(ErrorCode.AcoountIsInactive, ErrorMessage.AcoountIsInactive);
            }
        }

        public async Task<BaseResponse<PostFundTransferResponse>> PostFundTransfer(PostFundTransferRequest request)
        {
            if (request.Amount < 1)
                return new BaseResponse<PostFundTransferResponse>(ErrorCode.InvalidAmount, ErrorMessage.InvalidAmount, ErrorFieldName.InvalidAmount);
            
            if (CheckAccountStatusActive(request.CustomerId, request.BankAccountId) && CheckAccountStatusActive(request.ReceiverCustomerId, request.ReceiverBankAccountId))
            {
                if (CheckAccountBalance(request.BankAccountId, request.Amount))
                {

                    var transactionType = unitOfWork.GetRepository<TransactionTypes>()
                                        .GetByCondition(x => x.TransactionType == TransactionType.FundTransfer).FirstOrDefault();
                    var tranNo = GenerateTransactionNo();

                    var transaction = new Transaction
                    {
                        TransactionId = Guid.NewGuid(),
                        TransactionNo = tranNo,
                        BankAccountId = request.BankAccountId,
                        CustomerId = request.CustomerId,
                        ReceiverCustomerId = request.ReceiverCustomerId,
                        ReceiverBankAccountId = request.ReceiverBankAccountId,
                        TransactionTypeId = transactionType.TransactionTypeId,
                        Amount = request.Amount,
                        NetAmount = request.Amount,
                        ChargesPercentage = 0,
                        Charges = 0,
                        Notes = request.Note,
                        CreatedOn = DateTime.Now,
                    };

                    var senderLedger = new BankAccountLedger
                    {
                        LedgerId = Guid.NewGuid(),
                        BankAccountId = request.BankAccountId,
                        TransactionId = transaction.TransactionId,
                        TransactionTypeId = transaction.TransactionTypeId,
                        Credit = request.Amount,
                        Debit = 0
                    };

                    var receiverLedger = new BankAccountLedger
                    {
                        LedgerId = Guid.NewGuid(),
                        BankAccountId = request.ReceiverBankAccountId,
                        TransactionId = transaction.TransactionId,
                        TransactionTypeId = transaction.TransactionTypeId,
                        Credit = 0,
                        Debit = request.Amount
                    };

                    var senderBankAccount = unitOfWork.GetRepository<BankAccount>()
                                      .GetByCondition(b => b.BankAccountId == request.BankAccountId)
                                      .FirstOrDefault();
                    senderBankAccount.ClosingBalance -= request.Amount;
                    senderBankAccount.TotalCredit += request.Amount;
                    senderBankAccount.UpdatedOn = DateTime.Now;


                    var receiverBankAccount = unitOfWork.GetRepository<BankAccount>()
                                      .GetByCondition(b => b.BankAccountId == request.ReceiverBankAccountId)
                                      .FirstOrDefault();
                    receiverBankAccount.ClosingBalance += request.Amount;
                    receiverBankAccount.TotalDebit += request.Amount;
                    receiverBankAccount.UpdatedOn = DateTime.Now;

                    unitOfWork.GetRepository<BankAccount>().Update(senderBankAccount);
                    unitOfWork.GetRepository<BankAccount>().Update(receiverBankAccount);
                    unitOfWork.GetRepository<Transaction>().Add(transaction);
                    unitOfWork.GetRepository<BankAccountLedger>().Add(senderLedger);
                    unitOfWork.GetRepository<BankAccountLedger>().Add(receiverLedger);
                    unitOfWork.SaveChanges();

                    var response = new BaseResponse<PostFundTransferResponse>();
                    response.Responsedata = new PostFundTransferResponse
                    {
                        TransactionId = transaction.TransactionId,
                        TransactionNo = transaction.TransactionNo,
                        Amount = request.Amount,
                        Charges = 0
                    };
                    return response;
                }
                else
                {
                    return new BaseResponse<PostFundTransferResponse>(ErrorCode.NotEnoughBalance, ErrorMessage.NotEnoughBalance,ErrorFieldName.NotEnoughBalance);
                }

            }
            else
            {
                return new BaseResponse<PostFundTransferResponse>(ErrorCode.AcoountIsInactive, ErrorMessage.AcoountIsInactive);
            }

        }

        #endregion
    }
}
