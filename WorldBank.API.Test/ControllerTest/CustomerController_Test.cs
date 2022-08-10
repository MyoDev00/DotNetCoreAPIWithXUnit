using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldBank.API.Business;
using WorldBank.API.Controllers;
using WorldBank.API.Helper;
using WorldBank.API.Test.Fixture;
using WorldBank.Entities;
using WorldBank.Entities.DataModel;
using WorldBank.Shared.Helper;
using WorldBank.Shared.RequestModel;
using WorldBank.Shared.ResponseModel;
using WorldBank.Shared.ResponseModel.CommonResponse;
using WorldBank.UnitOfWork;
using static WorldBank.Shared.Constant.Constant;

namespace WorldBank.API.Test.ControllerTest
{
    [TestCaseOrderer("WorldBank.API.Test.Helper.PriorityOrderer", "WorldBank.API.Test")]
    public class CustomerController_Test : IClassFixture<TestDatabaseFixture>
    {

        CustomerController controller;
        IMapper mapper;
        public WorldBankDBContext Context { get; }
        string encryptionKey = "E@8s8Jdi*#sdIU0";

        public CustomerController_Test(TestDatabaseFixture dBFixture)
        {
            Context = dBFixture.CreateContext();

            var unitOfWork = new UnitOfWork<WorldBankDBContext>(Context);

            var config = new Mock<IConfiguration>();
            config.Setup(x => x["EncryptionKey"]).Returns("E@8s8Jdi*#sdIU0");
            config.Setup(x => x["IBANGeneratorURL"]).Returns("http://randomiban.com/?country=Netherlands#");

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            mapper = mapperConfig.CreateMapper();
            var customerBL = new CustomerBL(unitOfWork, config.Object, mapper);

            controller = new CustomerController(customerBL);

        }


        [Fact, TestPriority(0)]
        public async Task Test_PostCustomer_ShouldSuccess()
        {
            #region Arrange
            var bankAccountTypes = Context.BankAccountTypes.FirstOrDefault();
            var currencies = Context.Currency.FirstOrDefault();

            var request = new PostCustomerRequest
            {
                BankAccountTypeId = bankAccountTypes.BankAccountTypeId,
                CurrencyId = currencies.CurrencyId,
                Email = "newcustomer@newmail.com",
                FullName = "NewCustomer",
                IdentityCardNo = "NIC00N",
                MobileCode = "+95",
                Mobile = "999888777",
                Password = "password",
                Status = (int)CommonStatus.Active
            };
            #endregion

            #region Act
            var response = await controller.PostCustomer(request);
            var okResponse = response as OkObjectResult;
            var data = okResponse.Value as BaseResponse<PostCustomerResponse>;

            #endregion

            #region Assert
            Assert.NotNull(okResponse);
            Assert.Equal(200, okResponse.StatusCode);
            Assert.NotNull(data.Responsedata);
            Assert.Null(data.Error);

            //check db record
            var customer = Context.Customer.Where(x => x.CustomerId == data.Responsedata.CustomerId).FirstOrDefault();

            Assert.NotNull(customer);

            var bankAccount = Context.BankAccount.Where(x=>x.CustomerId==data.Responsedata.CustomerId).ToList();

            Assert.Single(bankAccount);
            Assert.Equal(request.BankAccountTypeId,bankAccount[0].BankAccountTypeId);
            Assert.Equal(request.CurrencyId,bankAccount[0].CurrencyId);
            Assert.Equal(0,bankAccount[0].ClosingBalance);
            Assert.False(string.IsNullOrEmpty(bankAccount[0].IbanNumber));
            #endregion

        }

        [Fact,TestPriority(1)]
        public async Task Test_PutCustomer_ShouldSuccess()
        {
            #region Arrange
            var customer = Context.Customer.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
            var oldPassword = customer.Password;
            var oldSalt = customer.SaltPassword;
            var oldCreatedOn = customer.CreatedOn;
            var oldUpdatedOn = customer.UpdatedOn;

            var request = new PutCustomerRequest
            {
                CustomerId = customer.CustomerId,
                Email = "edit@gmail.com",
                FullName = "EditCustomer",
                IdentityCardNo = "EditIC0001",
                MobileCode = "+88",
                Mobile = "11223344",
                Status = (int)CommonStatus.Inactive
            };
            #endregion

            #region Act
            var response = await controller.PutCustomer(request);
            var okResponse = response as OkObjectResult;
            var data = okResponse.Value as BaseResponse<PutCustomerResponse>;
            #endregion

            #region Assert
            Assert.NotNull(okResponse);
            Assert.Equal(200, okResponse.StatusCode);
            Assert.NotNull(data.Responsedata);
            Assert.Null(data.Error);

            Assert.Equal(customer.FullName, request.FullName);
            Assert.Equal(customer.MobileCode , request.MobileCode);
            Assert.Equal(StringHelper.Decrypt(customer.Mobile, encryptionKey), request.Mobile);
            Assert.Equal(StringHelper.Decrypt(customer.Email, encryptionKey), request.Email);
            Assert.Equal(StringHelper.Decrypt(customer.IdentityCardNo, encryptionKey), request.IdentityCardNo);
            Assert.Equal(oldPassword, customer.Password);
            Assert.Equal(oldSalt, customer.SaltPassword);
            Assert.Equal(oldCreatedOn, customer.CreatedOn);
            Assert.Equal(request.Status, customer.Status);
            Assert.True(oldUpdatedOn < customer.UpdatedOn);
            
            #endregion
        }

        [Fact, TestPriority(2)]
        public async Task Test_GetCustomers_ShouldSuccess()
        {
            #region Arrange

            var request = new GetCustomersRequest()
            {
                PageIndex = 1,
                PageSize = 10,

            };
            #endregion

            #region Act
            var response = await controller.GetCustomer(request);
            var okResponse = response as OkObjectResult;
            var data = okResponse.Value as BaseResponse<GetCustomersResponse>;

            #endregion

            #region Assert
            Assert.NotNull(okResponse);
            Assert.Equal(200, okResponse.StatusCode);
            Assert.NotNull(data.Responsedata);
            Assert.Null(data.Error);
            Assert.Single(data.Responsedata.Customers);
            #endregion 
        }

        [Fact,TestPriority(3)]
        public async Task Test_GetCustomerDetail_ShouldSuccess()
        {
            #region Arrange
            var customer = Context.Customer.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
            #endregion

            #region Act

            var response = await controller.GetCustomerDetail(customer.CustomerId);
            var okResponse = response as OkObjectResult;
            var data = okResponse.Value as BaseResponse<GetCustomerDetailResponse>;

            #endregion

            #region Assert
            Assert.NotNull(okResponse);
            Assert.Equal(200, okResponse.StatusCode);
            Assert.NotNull(data.Responsedata);
            Assert.Null(data.Error);

            Assert.Equal(customer.FullName, data.Responsedata.FullName);
            Assert.Equal(customer.MobileCode, data.Responsedata.MobileCode);
            Assert.Equal(StringHelper.Decrypt(customer.Mobile, encryptionKey), data.Responsedata.Mobile);
            Assert.Equal(StringHelper.Decrypt(customer.Email, encryptionKey), data.Responsedata.Email);
            Assert.Equal(StringHelper.Decrypt(customer.IdentityCardNo, encryptionKey), data.Responsedata.IdentityCardNo);
            Assert.Equal(customer.Status, data.Responsedata.Status);
            Assert.Single(data.Responsedata.BankAccounts);
            #endregion 
        }
    }
}
