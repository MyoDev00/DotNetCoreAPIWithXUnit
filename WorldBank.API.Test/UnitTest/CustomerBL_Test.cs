using AutoMapper;
using Moq;
using Shouldly;
using WorldBank.API.Business;
using WorldBank.API.Helper;
using WorldBank.API.Test.Mock;
using WorldBank.Entities;
using WorldBank.Entities.DataModel;
using WorldBank.Shared.Helper;
using WorldBank.Shared.RequestModel;
using WorldBank.Shared.ResponseModel;
using WorldBank.Shared.ResponseModel.CommonResponse;
using WorldBank.UnitOfWork;
using static WorldBank.Shared.Constant.Constant;

namespace WorldBank.API.Test.UnitTest
{
    public class CustomerBL_Test
    {
        UnitOfWork<WorldBankDBContext> unitOfWork;
        CustomerBL customerBL;
        string encryptionKey = "E@8s8Jdi*#sdIU0";
        string iBANURL = @"http://randomiban.com/?country=Netherlands#";
        MockData mockData;
        IMapper mapper;

        private void CommonArrange()
        {
            mockData = new MockData();

            mockData.Customers.ForEach(x => { 
                x.Email = StringHelper.Encrypt(x.Email, encryptionKey);
                x.Mobile = StringHelper.Encrypt(x.Mobile, encryptionKey);
                x.IdentityCardNo = StringHelper.Encrypt(x.IdentityCardNo, encryptionKey);
            });

            var mockDataSet = new MockDbSet<Customer>(mockData.Customers);
            var mockContext = new Mock<WorldBankDBContext>();
            mockContext.Setup(c => c.Set<Customer>()).Returns(mockDataSet.Object);
            unitOfWork = new UnitOfWork<WorldBankDBContext>(mockContext.Object);
            customerBL = new CustomerBL(unitOfWork, encryptionKey);
        }

        public void PutAndPostArrange()
        {
            mockData = new MockData();

           
            var mockContext = new Mock<WorldBankDBContext>();
            var mockCustomer = new MockDbSet<Customer>(mockData.Customers);
            var mockBankAccountType = new MockDbSet<BankAccountTypes>(mockData.BankAccountTypes);
            var mockBankAccount = new MockDbSet<BankAccount>(mockData.BankAccounts);
            var mockCurrencies = new MockDbSet<Currency>(mockData.Currencies);
            
            mockContext.Setup(c => c.Set<Customer>()).Returns(mockCustomer.Object);
            mockContext.Setup(c => c.Set<BankAccountTypes>()).Returns(mockBankAccountType.Object);
            mockContext.Setup(c => c.Set<Currency>()).Returns(mockCurrencies.Object);
            mockContext.Setup(c => c.Set<BankAccount>()).Returns(mockBankAccount.Object);

            unitOfWork = new UnitOfWork<WorldBankDBContext>(mockContext.Object);
            customerBL = new CustomerBL(unitOfWork, encryptionKey);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            mapper = config.CreateMapper();
            customerBL.InjectMapper(mapper);
        }

        [Theory]
        [InlineData("c1@gmail.com", true)]
        [InlineData("c99@gmail.com", false)]
        public void Test_CheckEmail(string email, bool isSuccessResult)
        {
            CommonArrange();

            var response = customerBL.CheckEmailExist(email);
            
            if (isSuccessResult)
            {
                Assert.True(response);
            }
            else
            {
                Assert.False(response);
            }
        }

        [Theory]
        [InlineData("+95","0111", true)]
        [InlineData("+69","0111", false)]
        [InlineData("+95", "0999", false)]
        public void Test_CheckMobile(string mobileCode,string mobile, bool isSuccessResult)
        {
            CommonArrange();

            var response = customerBL.CheckMobileExist(mobileCode,mobile);

            if (isSuccessResult)
            {
                Assert.True(response);
            }
            else
            {
                Assert.False(response);
            }
        }

        [Theory]
        [InlineData("IC0001", true)]
        [InlineData("IC999", false)]
        public void Test_CheckIdentityNumber(string identityNo, bool isSuccessResult)
        {
            CommonArrange();

            var response = customerBL.CheckIdentityNumberExist(identityNo);

            if (isSuccessResult)
            {
                Assert.True(response);
            }
            else
            {
                Assert.False(response);
            }
        }

        [Fact]
        public void Text_GenerateIBAN_ShouldReturn_Success()
        {
            var customerBL = new CustomerBL();
            customerBL.InjectIBANURL(iBANURL);

            var response = customerBL.GenerateIBAN();

            Assert.False(string.IsNullOrEmpty(response));
        }

        [Fact]
        public async Task Test_PostCustomer_ShouldReturn_Failed()
        {
            #region Arrange
            PutAndPostArrange();
            customerBL.InjectIBANURL(iBANURL);

            //var bankAccountTypes = mockData.BankAccountTypes;
            //var currencies  = mockData.Currencies;
            //var customerList = mockData.Customers;
           
            var exceptedErrorList = new List<Error>
            {
                new Error
                {
                    ErrorCode = ErrorCode.EmailAlreadyUsed,
                    ErrorMessage = ErrorMessage.EmailAlreadyUsed,
                    FieldName = ErrorFieldName.Email
                },
                new Error
                {
                    ErrorCode = ErrorCode.MobileAlreadyUsed,
                    ErrorMessage = ErrorMessage.MobileAlreadyUsed,
                    FieldName = ErrorFieldName.Mobile
                },
                new Error
                {
                    ErrorCode = ErrorCode.IdentityAlreadyUsed,
                    ErrorMessage = ErrorMessage.IdentityAlreadyUsed,
                    FieldName = ErrorFieldName.IdentityCardNo
                },
            };
            exceptedErrorList = exceptedErrorList.OrderBy(x => x.ErrorCode).ToList();

            var request = new PostCustomerRequest
            {
                BankAccountTypeId = mockData.BankAccountTypes[0].BankAccountTypeId,
                CurrencyId = mockData.Currencies[0].CurrencyId,
                Email = mockData.Customers[0].Email,
                FullName = "NewCustomer",
                IdentityCardNo = mockData.Customers[0].IdentityCardNo,
                MobileCode = mockData.Customers[0].MobileCode,
                Mobile = mockData.Customers[0].Mobile,
                Password = "password",
                Status = (int)CommonStatus.Active
            };

            mockData.Customers.ForEach(x => {
                x.Email = StringHelper.Encrypt(x.Email, encryptionKey);
                x.Mobile = StringHelper.Encrypt(x.Mobile, encryptionKey);
                x.IdentityCardNo = StringHelper.Encrypt(x.IdentityCardNo, encryptionKey);
            });

            #endregion

            #region Act
            var response = await customerBL.PostCustomer(request);
            #endregion

            #region Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Error);
            Assert.Null(response.Responsedata);
            Assert.Equal(3, response.Error.Count);

            response.Error = response.Error.OrderBy(x => x.ErrorCode).ToList();

            response.Error.ShouldBeEquivalentTo(exceptedErrorList);

            #endregion
        }

        [Fact]
        public async Task Test_PostCustomer_ShouldReturn_Success()
        {
            #region Arrange
            PutAndPostArrange();

            customerBL.InjectIBANURL(iBANURL);

            var request = new PostCustomerRequest
            {
                BankAccountTypeId = mockData.BankAccountTypes[0].BankAccountTypeId,
                CurrencyId = mockData.Currencies[0].CurrencyId,
                Email = "newcustomer@newmail.com",
                FullName = "NewCustomer",
                IdentityCardNo = "NIC00N",
                MobileCode = "+95",
                Mobile = "999888777",
                Password = "password",
                Status = (int)CommonStatus.Active
            };
            mockData.Customers.ForEach(x => {
                x.Email = StringHelper.Encrypt(x.Email, encryptionKey);
                x.Mobile = StringHelper.Encrypt(x.Mobile, encryptionKey);
                x.IdentityCardNo = StringHelper.Encrypt(x.IdentityCardNo, encryptionKey);
            });


            var exceptedResponse = mapper.Map<PostCustomerResponse>(request);
            //exceptedResponse.Email = StringHelper.Encrypt(exceptedResponse.Email, encryptionKey);
            //exceptedResponse.Mobile = StringHelper.Encrypt(exceptedResponse.Mobile, encryptionKey);
            //exceptedResponse.IdentityCardNo = StringHelper.Encrypt(exceptedResponse.IdentityCardNo, encryptionKey);

            exceptedResponse.BankAccounts = new List<BankAccountResponse>()
            {
                new BankAccountResponse()
                {
                    BankAccountId = Guid.NewGuid(),
                    BankAccountTypeId = request.BankAccountTypeId,
                    BankAccountType = mockData.BankAccountTypes[0].BankAccountType,
                    BankAccountTypeDescription = mockData.BankAccountTypes[0].Description,
                    CurrencyId = mockData.Currencies[0].CurrencyId,
                    CurrencySymbol = mockData.Currencies[0].CurrencySymbol,
                    CurrencyDescryption = mockData.Currencies[0].Description,
                    ClosingBalance = 0,
                    IBANNumber = "",
                    Status = (int)CommonStatus.Active
                }
            };
            #endregion

            #region Act
            var response = await customerBL.PostCustomer(request);
            #endregion

            #region Assert
            Assert.NotNull(response);
            Assert.Null(response.Error);
            Assert.NotNull(response.Responsedata);
            Assert.StrictEqual(exceptedResponse, response.Responsedata);
            Assert.StrictEqual(exceptedResponse.BankAccounts[0], response.Responsedata.BankAccounts[0]);
            #endregion
        }

        [Fact]
        public async Task Test_PutCustomer_ShouldReturn_Failed()
        {
            #region Arrange

            PutAndPostArrange();

            var exceptedErrorList = new List<Error>
            {
                new Error
                {
                    ErrorCode = ErrorCode.EmailAlreadyUsed,
                    ErrorMessage = ErrorMessage.EmailAlreadyUsed,
                    FieldName = ErrorFieldName.Email
                },
                new Error
                {
                    ErrorCode = ErrorCode.MobileAlreadyUsed,
                    ErrorMessage = ErrorMessage.MobileAlreadyUsed,
                    FieldName = ErrorFieldName.Mobile
                },
                new Error
                {
                    ErrorCode = ErrorCode.IdentityAlreadyUsed,
                    ErrorMessage = ErrorMessage.IdentityAlreadyUsed,
                    FieldName = ErrorFieldName.IdentityCardNo
                },
            };
            exceptedErrorList = exceptedErrorList.OrderBy(x => x.ErrorCode).ToList();

            var request = new PutCustomerRequest
            {
                CustomerId = mockData.Customers[0].CustomerId,
                Email = mockData.Customers[1].Email,
                FullName = "EditCustomer",
                IdentityCardNo = mockData.Customers[1].IdentityCardNo,
                MobileCode = mockData.Customers[1].MobileCode,
                Mobile = mockData.Customers[1].Mobile,
                Status = (int)CommonStatus.Inactive
            };
            mockData.Customers.ForEach(x => {
                x.Email = StringHelper.Encrypt(x.Email, encryptionKey);
                x.Mobile = StringHelper.Encrypt(x.Mobile, encryptionKey);
                x.IdentityCardNo = StringHelper.Encrypt(x.IdentityCardNo, encryptionKey);
            });
            #endregion

            #region Act

            var response = await customerBL.PutCustomer(request);

            #endregion

            #region Assert

            Assert.NotNull(response);
            Assert.NotNull(response.Error);
            Assert.Null(response.Responsedata);
            Assert.Equal(3, response.Error.Count);
            response.Error = response.Error.OrderBy(x => x.ErrorCode).ToList();

            response.Error.ShouldBeEquivalentTo(exceptedErrorList);

            #endregion
        }

        [Fact]
        public async Task Test_PutCustomer_ShouldReturn_Success()
        {
            #region Arrange
            PutAndPostArrange();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            var mapper = config.CreateMapper();

            var request = new PutCustomerRequest
            {
                CustomerId = mockData.Customers[0].CustomerId,
                Email = "edit@gmail.com",
                FullName = "EditCustomer",
                IdentityCardNo = "EditIC0001",
                MobileCode = "+88",
                Mobile = "11223344",
                Status = (int)CommonStatus.Inactive
            };
            var exceptedResponse = mapper.Map<PutCustomerResponse>(request);
            
            mockData.Customers.ForEach(x => {
                x.Email = StringHelper.Encrypt(x.Email, encryptionKey);
                x.Mobile = StringHelper.Encrypt(x.Mobile, encryptionKey);
                x.IdentityCardNo = StringHelper.Encrypt(x.IdentityCardNo, encryptionKey);
            });
            #endregion

            #region Act
            var response = await customerBL.PutCustomer(request);
            var customerAfterEdit = unitOfWork.GetRepository<Customer>()
                .GetByCondition(x => x.CustomerId == request.CustomerId).FirstOrDefault();

            #endregion

            #region Assert

            Assert.NotNull(response);
            Assert.Null(response.Error);
            Assert.NotNull(response.Responsedata);
            response.Responsedata.ShouldBeEquivalentTo(exceptedResponse);
            response.Responsedata.ShouldBeEquivalentTo(exceptedResponse);

            #endregion
        }
    }
}
