using AutoMapper;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WorldBank.Shared.ResponseModel.CommonResponse;
using WorldBank.UnitOfWork;
using static WorldBank.Shared.Constant.Constant;

namespace WorldBank.API.Test.UnitTest
{
    public class CustomerBL_Test
    {
        UnitOfWork<WorldBankDBContext> unitOfWork;
        CustomerBL customerBL;
        Guid? customer1Id=null,customer2Id;

        public CustomerBL_Test()
        {
            var customerList = MockData.GetCustomerData();
            customer1Id = customerList[0].CustomerId;
            customer2Id = customerList[1].CustomerId;

            var mockDataSet = new MockDbSet<Customer>(customerList);
            var mockContext = new Mock<WorldBankDBContext>();
            mockContext.Setup(c => c.Set<Customer>()).Returns(mockDataSet.Object);
            unitOfWork = new UnitOfWork<WorldBankDBContext>(mockContext.Object);
            customerBL = new CustomerBL(unitOfWork);
        }

        [Theory]
        [InlineData("c1@gmail.com", true)]
        [InlineData("c99@gmail.com", false)]
        public void Test_CheckEmail(string email, bool isSuccessResult)
        {
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
        public async Task Test_PostCustomer_ShouldReturn_Failed()
        {
            var bankAccountTypes = MockData.GetBankAccountTypes();
            var currencies  = MockData.GetCurrencies();
            var customerList = MockData.GetCustomerData();
            var customerCountBeforeAdd = customerList.Count;
            var exceptedErrorList = new List<Error>
            {
                new Error
                {
                    ErrorCode = ErrorCode.EmailAlreadyUsed,
                    FieldName = "Email"
                },
                new Error
                {
                    ErrorCode = ErrorCode.MobileAlreadyUsed,
                    FieldName = "Mobile"
                },
                new Error
                {
                    ErrorCode = ErrorCode.IdentityAlreadyUsed,
                    FieldName = "IdentityCardNo"
                },
            };
            var request = new PostCustomerRequest
            {
                BankAccountType = bankAccountTypes[0].BankAccountType,
                CurrencyId = currencies[0].CurrencyId,
                Email = customerList[0].Email,
                FullName = "NewCustomer",
                IdentityCardNo = customerList[0].IdentityCardNo,
                MobileCode = customerList[0].MobileCode,
                Mobile = customerList[0].Mobile,
                Password = "password",
                Status = (int)CommonStatus.Active
            };
            var response = await customerBL.PostCustomer(request);
            Assert.NotNull(response);
            Assert.NotNull(response.Error);
            Assert.Null(response.Responsedata);
            Assert.Equal(3, response.Error.Count);
            Assert.Equal(customerCountBeforeAdd,customerList.Count);
            response.Error.ShouldBeEquivalentTo(exceptedErrorList);
        }

        [Fact]
        public async Task Test_PostCustomer_ShouldReturn_Success()
        {
            var bankAccountTypes = MockData.GetBankAccountTypes();
            var currencies = MockData.GetCurrencies();
            var customerList = MockData.GetCustomerData();
            var customerCountBeforeAdd = customerList.Count;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            var mapper = config.CreateMapper();

            var request = new PostCustomerRequest
            {
                BankAccountType = bankAccountTypes[0].BankAccountType,
                CurrencyId = currencies[0].CurrencyId,
                Email = customerList[0].Email,
                FullName = "NewCustomer",
                IdentityCardNo = customerList[0].IdentityCardNo,
                MobileCode = customerList[0].MobileCode,
                Mobile = customerList[0].Mobile,
                Password = "password",
                Status = (int)CommonStatus.Active
            };

            var exceptedResponse = mapper.Map<PostCustomerResponse>(request);

            var response = await customerBL.PostCustomer(request);
            Assert.NotNull(response);
            Assert.Null(response.Error);
            Assert.NotNull(response.Responsedata);
            Assert.Equal(customerCountBeforeAdd+1, customerList.Count);
            response.Responsedata.ShouldBeEquivalentTo(exceptedResponse);
        }

        [Fact]
        public async Task Test_PutCustomer_ShouldReturn_Failed()
        {
            var bankAccountTypes = MockData.GetBankAccountTypes();
            var currencies = MockData.GetCurrencies();
            var customerList = MockData.GetCustomerData();
            var exceptedErrorList = new List<Error>
            {
                new Error
                {
                    ErrorCode = ErrorCode.EmailAlreadyUsed,
                    FieldName = "Email"
                },
                new Error
                {
                    ErrorCode = ErrorCode.MobileAlreadyUsed,
                    FieldName = "Mobile"
                },
                new Error
                {
                    ErrorCode = ErrorCode.IdentityAlreadyUsed,
                    FieldName = "IdentityCardNo"
                },
            };
            var request = new PutCustomerRequest
            {
                CustomerId = customerList[0].CustomerId,
                Email = customerList[1].Email,
                FullName = "EditCustomer",
                IdentityCardNo = customerList[1].IdentityCardNo,
                MobileCode = customerList[1].MobileCode,
                Mobile = customerList[1].Mobile,
                Status = (int)CommonStatus.Inactive
            };
            var response = await customerBL.PutCustomer(request);
            Assert.NotNull(response);
            Assert.NotNull(response.Error);
            Assert.Null(response.Responsedata);
            Assert.Equal(3, response.Error.Count);
            response.Error.ShouldBeEquivalentTo(exceptedErrorList);
        }

        [Fact]
        public async Task Test_PutCustomer_ShouldReturn_Success()
        {
            var bankAccountTypes = MockData.GetBankAccountTypes();
            var currencies = MockData.GetCurrencies();
            var customerList = MockData.GetCustomerData();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            var mapper = config.CreateMapper();

            var request = new PutCustomerRequest
            {
                CustomerId = customerList[0].CustomerId,
                Email = "edit@gmail.com",
                FullName = "EditCustomer",
                IdentityCardNo = "EditIC0001",
                MobileCode = "+88",
                Mobile = "11223344",
                Status = (int)CommonStatus.Inactive
            };
            var exceptedResponse = mapper.Map<PutCustomerResponse>(request);

            var response = await customerBL.PutCustomer(request);
            Assert.NotNull(response);
            Assert.Null(response.Error);
            Assert.NotNull(response.Responsedata);
            response.Responsedata.ShouldBeEquivalentTo(exceptedResponse);
        }
    }
}
