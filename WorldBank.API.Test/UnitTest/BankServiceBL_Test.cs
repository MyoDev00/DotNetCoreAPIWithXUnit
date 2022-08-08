using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldBank.API.Business;
using WorldBank.API.Helper;
using WorldBank.API.Test.Mock;
using WorldBank.Entities;
using WorldBank.Entities.DataModel;
using WorldBank.Shared.Constant;
using WorldBank.Shared.RequestModel;
using WorldBank.Shared.ResponseModel;
using WorldBank.UnitOfWork;
using static WorldBank.Shared.Constant.Constant;

namespace WorldBank.API.Test.UnitTest
{
    public class BankServiceBL_Test
    {
        UnitOfWork<WorldBankDBContext> unitOfWork;
        BankServiceBL bankServiceBL;
        MockData mockData;
        IMapper mapper;

        public void CommonArrange()
        {
            mockData = new MockData();
            var mockContext = new Mock<WorldBankDBContext>();
            var mockBankAccount = new MockDbSet<BankAccount>(mockData.BankAccounts);
            var mockCustomer = new MockDbSet<Customer>(mockData.Customers);
            var mockTransactionCharges = new MockDbSet<TransactionCharges>(mockData.TransactionCharges);
            var mockAuditType = new MockDbSet<AuditTypes>(mockData.AuditTypes);
            var mockTransactionType = new MockDbSet<TransactionTypes>(mockData.TransactionTypes);
            var mockGeneratedNo = new MockDbSet<GeneratedTransactionNumber>(mockData.GeneratedTransactionNumbers);
            var mockTransaction = new MockDbSet<Transaction>(mockData.Transactions);
            var mockBankAccountLedgere = new MockDbSet<BankAccountLedger>(mockData.BankAccountLedgers);
            var mockStaffAuditLog = new MockDbSet<StaffAuditLog>(mockData.StaffAuditLogs);

            mockContext.Setup(c => c.Set<BankAccount>()).Returns(mockBankAccount.Object);
            mockContext.Setup(c => c.Set<Customer>()).Returns(mockCustomer.Object);
            mockContext.Setup(c => c.Set<TransactionCharges>()).Returns(mockTransactionCharges.Object);
            mockContext.Setup(c => c.Set<AuditTypes>()).Returns(mockAuditType.Object);
            mockContext.Setup(c => c.Set<TransactionTypes>()).Returns(mockTransactionType.Object);
            mockContext.Setup(c => c.Set<GeneratedTransactionNumber>()).Returns(mockGeneratedNo.Object);
            mockContext.Setup(c => c.Set<Transaction>()).Returns(mockTransaction.Object);
            mockContext.Setup(c => c.Set<BankAccountLedger>()).Returns(mockBankAccountLedgere.Object);
            mockContext.Setup(c => c.Set<StaffAuditLog>()).Returns(mockStaffAuditLog.Object);

            unitOfWork = new UnitOfWork<WorldBankDBContext>(mockContext.Object);
            bankServiceBL = new BankServiceBL(unitOfWork);


            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.AddProfile(new AutoMapperProfile());
            //});
            //mapper = config.CreateMapper();
            //customerBL.InjectMapper(mapper);
        }


        //#region Arrange
        //#endregion

        //#region Act
        //#endregion

        //#region Assert
        //#endregion

        [Theory]
        [InlineData("CA111111-1111-1111-1111-111111111111", "BA111111-1111-1111-1111-111111111111",true)]
        [InlineData("CA333333-1111-1111-1111-111111111111", "BA333333-1111-1111-1111-111111111111",false)]
        [InlineData("CA444444-1111-1111-1111-111111111111", "BA444444-1111-1111-1111-111111111111",false)]
        [InlineData("CA555555-1111-1111-1111-111111111111", "BA555555-1111-1111-1111-111111111111", false)]
        public void Test_CheckAccountStatus(string strCustomerId,string strBankAccountId,bool isSuccessResult)
        {
            #region Arrange
            CommonArrange();

            var customerId = Guid.Parse(strCustomerId);
            var bankAccountId = Guid.Parse(strBankAccountId);
            #endregion

            #region Act
            var response = bankServiceBL.CheckAccountStatusActive(customerId, bankAccountId);
            #endregion

            #region Assert

            if (isSuccessResult)
            {
                Assert.True(response);    
            }
            else
            {
                Assert.False(response);
            }
            #endregion

        }


        [Theory]
        [InlineData(100,true)]
        [InlineData(0, false)]
        [InlineData(0.001, false)]
        [InlineData(-1, false)]
        [InlineData(-1111.99, false)]
        public async Task Test_PostDeposit_AmountCheck(decimal depositAmount,bool isCorrectAmount)
        {
            #region Arrange
            CommonArrange();

            Guid customerId = mockData.Customers[0].CustomerId;

            var request = new PostDepositRequest
            {
                CustomerId = customerId,
                BankAccountId = mockData.Customers[0].BankAccount[0].BankAccountId,
                Amount = depositAmount,
                Note = "note"
            };
            #endregion

            #region Act
            var response = await bankServiceBL.PostDeposit(request,Guid.NewGuid());
            #endregion

            #region Assert

            if (isCorrectAmount)
            {
                Assert.Null(response.Error);
                Assert.NotNull(response.Responsedata);
            }
            else
            {
                Assert.Null(response.Responsedata);
                Assert.NotNull(response.Error);
                Assert.Equal(ErrorCode.InvalidAmount, response.Error[0].ErrorCode);
                Assert.Equal(ErrorMessage.InvalidAmount, response.Error[0].ErrorMessage);
                Assert.Equal(ErrorFieldName.InvalidAmount, response.Error[0].FieldName);
            }


            #endregion
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Test_PostDeposit_StatusCheck(bool isCorrectCustomerId)
        {
            #region Arrange
            CommonArrange();

            Guid customerId = isCorrectCustomerId ? mockData.Customers[0].CustomerId : Guid.NewGuid();

            var request = new PostDepositRequest
            {
                CustomerId = customerId,
                BankAccountId = mockData.Customers[0].BankAccount[0].BankAccountId,
                Amount = 100,
                Note = "note"
            };
            #endregion

            #region Act
            var response = await bankServiceBL.PostDeposit(request, Guid.NewGuid());
            #endregion

            #region Assert
            
            if (isCorrectCustomerId)
            {
                Assert.Null(response.Error);
                Assert.NotNull(response.Responsedata);
            }
            else
            {
                Assert.Null(response.Responsedata);
                Assert.NotNull(response.Error);
                Assert.Equal(ErrorCode.AcoountIsInactive, response.Error[0].ErrorCode);
                Assert.Equal(ErrorMessage.AcoountIsInactive, response.Error[0].ErrorMessage);
                
            }


            #endregion
        }
        [Fact]
        public async Task Test_PostDeposit_ShouldReturn_Success()
        {

            #region Arrange
            CommonArrange();

            var amountdeposit = 100m;
            var netAmount = 99.9m;
            var charges = 0.1m;
            var chargesPercentage = 0.1m;
            var transactionType = mockData.TransactionTypes.Where(x => x.TransactionType == TransactionType.BankDeposit).FirstOrDefault();
            var staffId = Guid.NewGuid();
            
            var request = new PostDepositRequest
            {
                CustomerId = mockData.Customers[0].CustomerId,
                BankAccountId = mockData.Customers[0].BankAccount[0].BankAccountId,
                Amount = amountdeposit,
                Note = "note"
            };

            var exceptedResponse = new PostDepositResponse 
            {
                CustomerId = mockData.Customers[0].CustomerId,
                BankAccountId = mockData.Customers[0].BankAccount[0].BankAccountId,
                ClosingBalance = mockData.Customers[0].BankAccount[0].ClosingBalance+netAmount,
                TotalDebit = mockData.Customers[0].BankAccount[0].TotalDebit + netAmount,
                TotalCredit = mockData.Customers[0].BankAccount[0].TotalCredit,
                Charges = charges,
            };

            var exceptedTransaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                BankAccountId = exceptedResponse.BankAccountId,
                CustomerId = exceptedResponse.CustomerId,
                Amount = request.Amount,
                NetAmount = netAmount,
                Charges = charges,
                ChargesPercentage = chargesPercentage,
                Notes = request.Note,
                TransactionTypeId = transactionType.TransactionTypeId
            };

            var exceptedLedger = new BankAccountLedger
            {
                BankAccountId = request.BankAccountId,
                TransactionTypeId = exceptedTransaction.TransactionTypeId,
                Credit = 0,
                Debit = netAmount,
            };
            #endregion

            #region Act
            var response = await bankServiceBL.PostDeposit(request,staffId);
            #endregion

            #region Assert
            Assert.NotNull(response);
            Assert.Null(response.Error);
            Assert.StrictEqual(exceptedResponse, response.Responsedata);
            Assert.False(string.IsNullOrEmpty(response.Responsedata.TransactionNo));
            Assert.NotNull(response.Responsedata.TransactionId);

            //check transactions data
            var transaction = unitOfWork.GetRepository<Transaction>()
                            .GetByCondition(x => x.TransactionId == response.Responsedata.TransactionId).ToList();

            Assert.Single(transaction);
            Assert.StrictEqual(exceptedTransaction, transaction[0]);

            var ledger = unitOfWork.GetRepository<BankAccountLedger>()
                        .GetByCondition(x=>x.TransactionId == response.Responsedata.TransactionId).ToList();
            
            Assert.Single(ledger);
            Assert.Equal(exceptedLedger.Credit, ledger[0].Credit);
            Assert.Equal(exceptedLedger.Debit, ledger[0].Debit);
            Assert.Equal(exceptedLedger.BankAccountId, ledger[0].BankAccountId);

            var staffAudit = unitOfWork.GetRepository<StaffAuditLog>()
                            .GetByCondition(x => x.RecordId == response.Responsedata.TransactionId).ToList();
            
            Assert.NotNull(ledger);
            Assert.Single(ledger);
            Assert.Equal(staffId, staffAudit[0].StaffId);
            #endregion
        }

        [Theory]
        [InlineData(-1,false)]
        [InlineData(0,false)]
        [InlineData(0.000000001,false)]
        [InlineData(100000, false)]
        [InlineData(1000,true)]
        public async Task Test_PostFundTransfer_AmountCheck(decimal amount,bool isSuccessResult)
        {
            #region Arrange
            CommonArrange();

            var request = new PostFundTransferRequest
            {
                CustomerId = mockData.Customers[0].CustomerId,
                BankAccountId = mockData.Customers[0].BankAccount[0].BankAccountId,

                ReceiverCustomerId = mockData.Customers[1].CustomerId,
                ReceiverBankAccountId = mockData.Customers[1].BankAccount[0].BankAccountId,

                Amount = amount
            };

            #endregion

            #region Act
            var response = await bankServiceBL.PostFundTransfer(request);
            #endregion

            #region Assert
            if (isSuccessResult)
            {
                Assert.Null(response.Error);
                Assert.NotNull(response.Responsedata);
                Assert.False(string.IsNullOrEmpty(response.Responsedata.TransactionNo));
            }
            else
            {

                Assert.NotNull(response.Error);
                Assert.Null(response.Responsedata);
                if (amount < 1)
                    Assert.Equal(ErrorCode.InvalidAmount, response.Error[0].ErrorCode);
                else
                    Assert.Equal(ErrorCode.NotEnoughBalance, response.Error[0].ErrorCode);
            }
            #endregion
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public async Task Test_PostFundTransfer_CheckAccountStatus(bool isSenderActive,bool isReceiverActive)
        {
            #region Arrange
            CommonArrange();

            var sender = mockData.Customers.Where(x => x.Status == (isSenderActive ? (int)CommonStatus.Active : (int)CommonStatus.Inactive) )
                        .FirstOrDefault();
            var receiver = mockData.Customers.Where(x => x.Status == (isReceiverActive ? (int)CommonStatus.Active : (int)CommonStatus.Inactive)
                         && x.CustomerId!=sender.CustomerId).FirstOrDefault();

            var request = new PostFundTransferRequest
            {
                CustomerId = sender.CustomerId,
                BankAccountId = sender.BankAccount[0].BankAccountId,

                ReceiverCustomerId = receiver.CustomerId,
                ReceiverBankAccountId = receiver.BankAccount[0].BankAccountId,

                Amount = 100
            };

            #endregion

            #region Act
            var response = await bankServiceBL.PostFundTransfer(request);
            #endregion

            #region Assert
            if (isSenderActive&&isReceiverActive)
            {
                Assert.Null(response.Error);
                Assert.NotNull(response.Responsedata);
                Assert.False(string.IsNullOrEmpty(response.Responsedata.TransactionNo));
            }
            else
            {
                Assert.NotNull(response.Error);
                Assert.Null(response.Responsedata);
                Assert.Equal(ErrorCode.AcoountIsInactive, response.Error[0].ErrorCode);
            }
            #endregion
        }

        [Fact]
        public async Task Test_PostFundTransfer_ShouldSuccess()
        {
            #region Arrange
            CommonArrange();

            var transactionType = mockData.TransactionTypes.Where(x => x.TransactionType == TransactionType.FundTransfer)
                                .FirstOrDefault();

            var sender = mockData.Customers.Where(x => x.Status == (int)CommonStatus.Active
                         && x.BankAccount[0].Status == (int)CommonStatus.Active).FirstOrDefault();
            var receiver = mockData.Customers.Where(x => x.Status == (int)CommonStatus.Active
                         && x.BankAccount[0].Status == (int)CommonStatus.Active
                         && x.CustomerId != sender.CustomerId).FirstOrDefault();

            var amountTransfer = 100;

            var exceptedSenderAccountBalance = sender.BankAccount[0].ClosingBalance - amountTransfer;
            var exceptedSenderAccountCredit = sender.BankAccount[0].TotalCredit + amountTransfer;
            var exceptedSenderAccountDebit = sender.BankAccount[0].TotalDebit;

            var exceptedReceiverAccountBalance = receiver.BankAccount[0].ClosingBalance + amountTransfer;
            var exceptedReceiverAccountCredit = sender.BankAccount[0].TotalCredit;
            var exceptedReceiverAccountDebit = receiver.BankAccount[0].TotalDebit + amountTransfer;


            var request = new PostFundTransferRequest
            {
                CustomerId = sender.CustomerId,
                BankAccountId = sender.BankAccount[0].BankAccountId,

                ReceiverCustomerId = receiver.CustomerId,
                ReceiverBankAccountId = receiver.BankAccount[0].BankAccountId,

                Amount = amountTransfer,

                Note = "note"
            };

            var exceptedResponse = new PostFundTransferResponse
            {
                Amount = amountTransfer,
                Charges = 0
            };

            var exceptedTransaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                BankAccountId = sender.BankAccount[0].BankAccountId,
                CustomerId = sender.CustomerId,
                ReceiverBankAccountId = receiver.BankAccount[0].BankAccountId,
                ReceiverCustomerId = receiver.CustomerId,
                Amount = request.Amount,
                NetAmount = request.Amount,
                Charges = 0,
                ChargesPercentage = 0,
                Notes = request.Note,
                TransactionTypeId = transactionType.TransactionTypeId
            };

            var exceptedSenderLedger = new BankAccountLedger
            {
                BankAccountId = sender.BankAccount[0].BankAccountId,
                TransactionTypeId = exceptedTransaction.TransactionTypeId,
                Credit = request.Amount,
                Debit = 0,
            };
            var exceptedReceiverLedger = new BankAccountLedger
            {
                BankAccountId = receiver.BankAccount[0].BankAccountId,
                TransactionTypeId = exceptedTransaction.TransactionTypeId,
                Credit = 0,
                Debit = request.Amount
            };
            #endregion

            #region Act
            var response = await bankServiceBL.PostFundTransfer(request);
            #endregion

            #region Assert
            Assert.Null(response.Error);
            Assert.NotNull(response.Responsedata);
            Assert.False(string.IsNullOrEmpty(response.Responsedata.TransactionNo));

            var senderAfter = unitOfWork.GetRepository<BankAccount>()
                .GetByCondition(x => x.BankAccountId == sender.BankAccount[0].BankAccountId).FirstOrDefault();
            var receiverAfter = unitOfWork.GetRepository<BankAccount>()
                .GetByCondition(x => x.BankAccountId == receiver.BankAccount[0].BankAccountId).FirstOrDefault();

            Assert.Equal(exceptedSenderAccountBalance,senderAfter.ClosingBalance);
            Assert.Equal(exceptedSenderAccountCredit, senderAfter.TotalCredit);
            Assert.Equal(exceptedReceiverAccountBalance, receiverAfter.ClosingBalance);
            Assert.Equal(exceptedReceiverAccountDebit, receiverAfter.TotalDebit);

            //check transactions data
            var transaction = unitOfWork.GetRepository<Transaction>()
                            .GetByCondition(x => x.TransactionId == response.Responsedata.TransactionId).ToList();

            Assert.Single(transaction);
            Assert.False(string.IsNullOrEmpty(transaction[0].TransactionNo));

            var senderLedger = unitOfWork.GetRepository<BankAccountLedger>()
                        .GetByCondition(x => x.TransactionId == response.Responsedata.TransactionId 
                        && x.BankAccountId==sender.BankAccount[0].BankAccountId).ToList();

            var receiverLedger = unitOfWork.GetRepository<BankAccountLedger>()
                        .GetByCondition(x => x.TransactionId == response.Responsedata.TransactionId
                        && x.BankAccountId == receiver.BankAccount[0].BankAccountId).ToList();

            Assert.Single(senderLedger);
            Assert.Single(receiverLedger);

            Assert.Equal(exceptedSenderLedger.Credit,senderLedger[0].Credit);
            Assert.Equal(exceptedSenderLedger.Debit,senderLedger[0].Debit);

            Assert.Equal(exceptedReceiverLedger.Credit, receiverLedger[0].Credit);
            Assert.Equal(exceptedReceiverLedger.Debit, receiverLedger[0].Debit);

    
            var senderBankAccount = unitOfWork.GetRepository<BankAccount>()
                .GetByCondition(x=>x.BankAccountId==sender.BankAccount[0].BankAccountId).FirstOrDefault();
            var receiverBankAccount = unitOfWork.GetRepository<BankAccount>()
                .GetByCondition(x => x.BankAccountId == receiver.BankAccount[0].BankAccountId).FirstOrDefault();

            Assert.Equal(exceptedSenderAccountBalance, senderBankAccount.ClosingBalance);
            Assert.Equal(exceptedSenderAccountCredit,senderBankAccount.TotalCredit);
            Assert.Equal(exceptedSenderAccountDebit, senderBankAccount.TotalDebit);

            Assert.Equal(exceptedReceiverAccountBalance, receiverBankAccount.ClosingBalance);
            Assert.Equal(exceptedReceiverAccountCredit,receiverBankAccount.TotalCredit);
            Assert.Equal(exceptedReceiverAccountDebit,receiverBankAccount.TotalDebit);
            #endregion
        }
    }
}
