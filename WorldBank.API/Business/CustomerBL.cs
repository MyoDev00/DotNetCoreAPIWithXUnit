using WorldBank.Entities.DataModel;
using WorldBank.Shared.Helper;
using WorldBank.Shared.RequestModel;
using WorldBank.Shared.ResponseModel;
using WorldBank.Shared.ResponseModel.CommonResponse;
using WorldBank.UnitOfWork;
using static WorldBank.Shared.Constant.Constant;
using AutoMapper;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.EntityFrameworkCore;

namespace WorldBank.API.Business
{
    public interface ICustomerBL
    {
        public Task<BaseResponse<GetCustomersResponse>> GetCustomers(GetCustomersRequest request);
        public Task<BaseResponse<GetCustomerDetailResponse>> GetCustomerDetail(Guid customerId);
        public Task<BaseResponse<PostCustomerResponse>> PostCustomer(PostCustomerRequest request);
        public Task<BaseResponse<PutCustomerResponse>> PutCustomer(PutCustomerRequest request);

    }
    public class CustomerBL : ICustomerBL
    {
        #region Constructor
        
        private readonly IUnitOfWork unitOfWork;
        private IMapper mapper;
        private readonly string encryptionKey;
        private string ibanGeneratorURL { get; set; }
        public CustomerBL(IUnitOfWork unitOfWork,IConfiguration config,IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            encryptionKey = config["EncryptionKey"];
            ibanGeneratorURL = config["IBANGeneratorURL"];
        }
        public CustomerBL(IUnitOfWork unitOfWork,string encryptionKey)
        {
            this.unitOfWork = unitOfWork;
            this.encryptionKey = encryptionKey;
        }
        public CustomerBL()
        {

        }
        public void InjectIBANURL(string url)
        {
            ibanGeneratorURL = url;
        }
        public void InjectMapper(IMapper mapper)
        {
            this.mapper = mapper;
        }
        #endregion

        #region Helper

        public bool CheckEmailExist(string email)
        {
            var encryptedEmail = StringHelper.Encrypt(email, encryptionKey);

            var isAny = unitOfWork.GetRepository<Customer>().GetByCondition(c => c.Email == encryptedEmail).Any();
            return isAny;
        }

        public bool CheckIdentityNumberExist(string identityNo)
        {
            var encryptedNo = StringHelper.Encrypt(identityNo, encryptionKey);
            var isAny = unitOfWork.GetRepository<Customer>().GetByCondition(c => c.IdentityCardNo == encryptedNo).Any();
            return isAny;
        }

        public bool CheckMobileExist(string mobileCode, string mobile)
        {
            var encryptedMobile = StringHelper.Encrypt(mobile, encryptionKey);
            var isAny = unitOfWork.GetRepository<Customer>().GetByCondition(c => c.Mobile == encryptedMobile && c.MobileCode==mobileCode).Any();
            return isAny;
        }

        public bool CheckIBANExist(string IBANNumber)
        {
            if (string.IsNullOrEmpty(IBANNumber))
                return true;

            var isAny = unitOfWork.GetRepository<BankAccount>().GetByCondition(b => b.IbanNumber == IBANNumber).Any();
            return isAny;
        }

        public string GenerateIBAN()
        {
            try
            {

                ChromeOptions options = new ChromeOptions();
                options.AddArgument("headless");
                WebDriver driver = new ChromeDriver(options);
                driver.Url = ibanGeneratorURL;

                IWebElement demo = driver.FindElement(By.Id("demo"));
                return demo.Text;

            }
            catch { return ""; }
        }
        #endregion

        #region Task
        public async Task<BaseResponse<PostCustomerResponse>> PostCustomer(PostCustomerRequest request)
        {
            var response = new BaseResponse<PostCustomerResponse>();

            var isEmailUsed = CheckEmailExist(request.Email);
            var isIdentityNumberUsed = CheckIdentityNumberExist(request.IdentityCardNo);
            var isMobileUsed = CheckMobileExist(request.MobileCode,request.Mobile);

            if(!isEmailUsed && !isIdentityNumberUsed && !isMobileUsed)
            {

                var hashPassword = PBKDF2HashHelper.HashPassword(request.Password);

                var customer = new Customer
                {
                    CustomerId = Guid.NewGuid(),
                    FullName = request.FullName,
                    Email = StringHelper.Encrypt(request.Email, encryptionKey),
                    IdentityCardNo = StringHelper.Encrypt(request.IdentityCardNo, encryptionKey),
                    MobileCode = request.MobileCode,
                    Mobile = StringHelper.Encrypt(request.Mobile, encryptionKey),
                    CreatedOn = DateTime.Now,
                    Password = hashPassword.Hash,
                    SaltPassword = hashPassword.Salt,
                    Status = request.Status,
                    UpdatedOn = DateTime.Now,
                };

                var ibanNumber = "";
                int tryCount = 0;
                bool isIBANNumberGenerateSuccess = false;

                do
                {
                    ibanNumber = GenerateIBAN();
                    isIBANNumberGenerateSuccess = !CheckIBANExist(ibanNumber);
                } while (tryCount++ < 3 && !isIBANNumberGenerateSuccess);

                if (!isIBANNumberGenerateSuccess)
                {
                    return new BaseResponse<PostCustomerResponse>(ErrorCode.IBANNumberGenerationFailed,ErrorMessage.IBANNumberGenerationFailed);
                }

                var bankAccount = new BankAccount
                {
                    BankAccountId = Guid.NewGuid(),
                    CustomerId = customer.CustomerId,
                    IbanNumber = ibanNumber,
                    BankAccountTypeId = request.BankAccountTypeId,
                    CurrencyId = request.CurrencyId,
                    ClosingBalance = 0,
                    TotalCredit = 0,
                    TotalDebit  =0,
                    Status = (int)CommonStatus.Active,
                    UpdatedOn= DateTime.Now,
                };

                customer.BankAccount = new List<BankAccount>() { bankAccount };

                unitOfWork.GetRepository<Customer>().Add(customer);
                unitOfWork.GetRepository<BankAccount>().Add(bankAccount);
                unitOfWork.SaveChanges();

                var bankAccountType = unitOfWork.GetRepository<BankAccountTypes>()
                                      .GetByCondition(x => x.BankAccountTypeId == request.BankAccountTypeId)
                                      .FirstOrDefault();

                var currency = unitOfWork.GetRepository<Currency>()
                              .GetByCondition(x => x.CurrencyId == request.CurrencyId).FirstOrDefault();
                
                response.Responsedata = mapper.Map<PostCustomerResponse>(customer);
                response.Responsedata.Email = request.Email;
                response.Responsedata.Mobile = request.Mobile;
                response.Responsedata.IdentityCardNo = request.IdentityCardNo;

                response.Responsedata.BankAccounts = mapper.Map<List<BankAccountResponse>>(customer.BankAccount);
                response.Responsedata.BankAccounts[0].BankAccountType = bankAccountType.BankAccountType;
                response.Responsedata.BankAccounts[0].BankAccountTypeDescription = bankAccountType.Description;
                response.Responsedata.BankAccounts[0].CurrencySymbol = currency.CurrencySymbol;
                response.Responsedata.BankAccounts[0].CurrencyDescryption = currency.Description;

                return response;
            }
            else
            {
                List<Error> errors = new List<Error>();
                if (isEmailUsed)
                    errors.Add(new Error()
                    {
                        ErrorCode = ErrorCode.EmailAlreadyUsed,
                        ErrorMessage = ErrorMessage.EmailAlreadyUsed,
                        FieldName = ErrorFieldName.Email
                    });
                
                if (isIdentityNumberUsed)
                    errors.Add(new Error()
                    {
                        ErrorCode = ErrorCode.IdentityAlreadyUsed,
                        ErrorMessage = ErrorMessage.IdentityAlreadyUsed,
                        FieldName = ErrorFieldName.IdentityCardNo
                    });

                if(isMobileUsed)
                    errors.Add(new Error()
                    {
                        ErrorCode = ErrorCode.MobileAlreadyUsed,
                        ErrorMessage = ErrorMessage.MobileAlreadyUsed,
                        FieldName = ErrorFieldName.Mobile
                    });

                response.Error = errors;
                return response;
            }
        }

        public async Task<BaseResponse<PutCustomerResponse>> PutCustomer(PutCustomerRequest request)
        {
            var response = new BaseResponse<PutCustomerResponse>();

            var customer = unitOfWork.GetRepository<Customer>()
                              .GetByCondition(x => x.CustomerId == request.CustomerId).FirstOrDefault();

            if (customer != null)
            {
                bool isEmailUsed=false, isIdentityNumberUsed=false, isMobileUsed=false;
                var encryptedEmail = StringHelper.Encrypt(request.Email, encryptionKey);
                var encryptedMobile = StringHelper.Encrypt(request.Mobile, encryptionKey);
                var encryptedIdentityCardNo = StringHelper.Encrypt(request.IdentityCardNo, encryptionKey);

                if (customer.Email!=encryptedEmail)
                    isEmailUsed= CheckEmailExist(request.Email);

                if (customer.Mobile != encryptedMobile)
                    isMobileUsed = CheckMobileExist(request.MobileCode,request.Mobile);

                if(customer.IdentityCardNo!=encryptedIdentityCardNo)
                    isIdentityNumberUsed = CheckIdentityNumberExist(request.IdentityCardNo);

                if (!isEmailUsed && !isIdentityNumberUsed && !isMobileUsed)
                {
                    customer.Email = encryptedEmail;
                    customer.MobileCode = request.MobileCode;
                    customer.Mobile = encryptedMobile;
                    customer.IdentityCardNo = encryptedIdentityCardNo;
                    customer.FullName = request.FullName;
                    customer.Status = request.Status;
                    customer.UpdatedOn = DateTime.Now;

                    unitOfWork.GetRepository<Customer>().Update(customer);
                    unitOfWork.SaveChanges();

                    response.Responsedata= mapper.Map<PutCustomerResponse>(customer);
                    response.Responsedata.Email = request.Email;
                    response.Responsedata.Mobile = request.Mobile;
                    response.Responsedata.IdentityCardNo = request.IdentityCardNo;

                    return response;
                }
                else
                {
                    List<Error> errors = new List<Error>();
                    if (isEmailUsed)
                        errors.Add(new Error()
                        {
                            ErrorCode = ErrorCode.EmailAlreadyUsed,
                            ErrorMessage = ErrorMessage.EmailAlreadyUsed,
                            FieldName = ErrorFieldName.Email
                        });

                    if (isIdentityNumberUsed)
                        errors.Add(new Error()
                        {
                            ErrorCode = ErrorCode.IdentityAlreadyUsed,
                            ErrorMessage = ErrorMessage.IdentityAlreadyUsed,
                            FieldName = ErrorFieldName.IdentityCardNo
                        });

                    if (isMobileUsed)
                        errors.Add(new Error()
                        {
                            ErrorCode = ErrorCode.MobileAlreadyUsed,
                            ErrorMessage = ErrorMessage.MobileAlreadyUsed,
                            FieldName = ErrorFieldName.Mobile
                        });

                    response.Error = errors;
                    return response;
                }
            }
            else
            {
                return new BaseResponse<PutCustomerResponse>(ErrorCode.CustomerNotFound, ErrorMessage.CustomerNotFound);
            }

           
        }

        public async Task<BaseResponse<GetCustomersResponse>> GetCustomers(GetCustomersRequest request)
        {
            var encryptedSearch = StringHelper.Encrypt(request.SearchString??"",encryptionKey);

            var query = unitOfWork.GetRepository<Customer>()
                        .Includes(x =>
                        (string.IsNullOrEmpty(request.SearchString) ||
                        (x.FullName == request.SearchString || x.Email == encryptedSearch || x.Mobile == encryptedSearch || x.IdentityCardNo == encryptedSearch)
                        ), x => x.Include(c => c.BankAccount))
                        .Select(x => new GetCustomersResponse.CustomerModel
                        {
                            CustomerId = x.CustomerId,
                            FullName = x.FullName,
                            CreatedOn = x.CreatedOn,
                            UpdatedOn = x.UpdatedOn,
                        });
            var totalCount = query.Count();
            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                if (request.Order == "asc")
                {
                    switch (request.OrderBy.ToLower())
                    {
                        case "fullname":
                            query.OrderBy(x => x.FullName);
                            break;
                        case "createdon":
                            query.OrderBy(x => x.CreatedOn);
                            break;
                        case "updatedon":
                        default:
                            query.OrderBy(x => x.UpdatedOn);
                            break;
                    }
                }
                else
                {
                    switch (request.OrderBy.ToLower())
                    {
                        case "fullname":
                            query.OrderByDescending(x => x.FullName);
                            break;
                        case "createdon":
                            query.OrderByDescending(x => x.CreatedOn);
                            break;
                        case "updatedon":
                        default:
                            query.OrderByDescending(x => x.UpdatedOn);
                            break;
                    }
                }
            }

            var paging = query.Skip(request.PageIndex - 1).Take(request.PageSize).ToList();
            if (paging != null)
            {
                return new BaseResponse<GetCustomersResponse>()
                {
                    Responsedata = new GetCustomersResponse()
                    {
                        TotalRecord = totalCount,
                        PageIndex = request.PageIndex,
                        PageSize = request.PageSize,
                        Customers = paging
                    }
                };
            }
            else
            {
                return new BaseResponse<GetCustomersResponse>(ErrorCode.NoContent, ErrorMessage.NoContent);
            }

        }

        public async Task<BaseResponse<GetCustomerDetailResponse>> GetCustomerDetail(Guid customerId)
        {
            var bankAccountTypes = unitOfWork.GetRepository<BankAccountTypes>().GetAll();
            var currencies = unitOfWork.GetRepository<Currency>().GetAll();

            var customer = unitOfWork.GetRepository<Customer>()
                .Includes(c => c.CustomerId == customerId, c => c.Include(b => b.BankAccount))
                .FirstOrDefault();

            if (customer != null)
            {
                var responseDetail = new GetCustomerDetailResponse
                {
                    CustomerId = customer.CustomerId,
                    FullName = customer.FullName,
                    Email = StringHelper.Decrypt(customer.Email, encryptionKey),
                    IdentityCardNo = StringHelper.Decrypt(customer.IdentityCardNo, encryptionKey),
                    MobileCode = customer.MobileCode,
                    Mobile = StringHelper.Decrypt(customer.Mobile, encryptionKey),
                    Status = customer.Status,
                    BankAccounts = customer.BankAccount.Select(b => new BankAccountResponse
                    {
                        BankAccountId = b.BankAccountId,
                        BankAccountTypeId = b.BankAccountTypeId,
                        BankAccountType = bankAccountTypes.Where(t => t.BankAccountTypeId == b.BankAccountTypeId).Select(t => t.BankAccountType).FirstOrDefault(),
                        BankAccountTypeDescription = bankAccountTypes.Where(t => t.BankAccountTypeId == b.BankAccountTypeId).Select(t => t.Description).FirstOrDefault(),
                        CurrencyId = b.CurrencyId,
                        CurrencyDescryption = currencies.Where(cr => cr.CurrencyId == b.CurrencyId).Select(cr => cr.Description).FirstOrDefault(),
                        CurrencySymbol = currencies.Where(cr => cr.CurrencyId == b.CurrencyId).Select(cr => cr.CurrencySymbol).FirstOrDefault(),
                        ClosingBalance = b.ClosingBalance,
                        IBANNumber = b.IbanNumber,
                        Status = b.Status

                    }).ToList()

                };
                return new BaseResponse<GetCustomerDetailResponse>()
                {
                    Responsedata = responseDetail
                };
            }
            else
            {
                return new BaseResponse<GetCustomerDetailResponse>(ErrorCode.NoContent, ErrorMessage.NoContent);
            }
            

        }

        #endregion
    }
}
