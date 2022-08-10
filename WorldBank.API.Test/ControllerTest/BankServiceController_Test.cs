using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;
using WorldBank.API.Business;
using WorldBank.API.Controllers;
using WorldBank.API.Test.Fixture;
using WorldBank.Entities;
using WorldBank.Shared.Helper;
using WorldBank.Shared.RequestModel;
using WorldBank.Shared.ResponseModel;
using WorldBank.Shared.ResponseModel.CommonResponse;
using WorldBank.UnitOfWork;
using static WorldBank.Shared.Constant.Constant;

namespace WorldBank.API.Test.ControllerTest
{
    [Collection("ControllerIntegrationTest")]
    [TestCaseOrderer("WorldBank.API.Test.Helper.PriorityOrderer", "WorldBank.API.Test")]
    public class BankServiceController_Test : IClassFixture<TestDatabaseFixture>
    {
        BankServiceController controller;
        public WorldBankDBContext Context { get; }
        MockData mockData;

        public BankServiceController_Test(TestDatabaseFixture dBFixture)
        {
            Context = dBFixture.CreateContext();

            var unitOfWork = new UnitOfWork<WorldBankDBContext>(Context);
            var bankServiceBL = new BankServiceBL(unitOfWork);

            controller = new BankServiceController(bankServiceBL);
        }

        public void InsertOrUpdateData()
        {
            mockData = new MockData();
            
            if(!Context.Customer.Any(x=>x.CustomerId==mockData.Customers[0].CustomerId))
            {
                Context.Add(mockData.Customers[0]);
                Context.Add(mockData.BankAccounts[0]);
            }
            else
            {
                Context.Update(mockData.BankAccounts[0]);
            }
            if (!Context.Customer.Any(x => x.CustomerId == mockData.Customers[1].CustomerId))
            {
                Context.Add(mockData.Customers[1]);
                Context.Add(mockData.BankAccounts[1]);
            }
            else
            {
                Context.Update(mockData.BankAccounts[1]);
            }

            if (!Context.Staff.Any(x => x.StaffId == mockData.Staff[0].StaffId))
            {
                Context.Add(mockData.Staff[0]);
            }

            try
            {
                Context.SaveChanges();
            }
            catch(Exception ex)
            {

            }
            
        }

        [Fact, TestPriority(0)]
        public async Task PostDeposit_ShouldSuccess()
        {
            #region Arrange
            InsertOrUpdateData();

            var claims = new[]
                      {
                        new Claim(CustomClaims.AccountId,mockData.Staff[0].StaffId.ToString()),
                    };
            var claimprincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.User = new System.Security.Claims.ClaimsPrincipal(
                claimprincipal
            ) ;

            var amountBeforeDeposit = mockData.Customers[0].BankAccount[0].ClosingBalance;
            var percentage = Context.TransactionCharges.Where(x => x.ChargesType == TransactionChargeTypes.BankDeposit)
                           .Select(x => x.Percentage).FirstOrDefault();

            var request = new PostDepositRequest
            {
                CustomerId = mockData.Customers[0].CustomerId,
                BankAccountId = mockData.Customers[0].BankAccount[0].BankAccountId,
                Amount = 1000,
                Note = "deposit by test"
            };

            var exceptedDepositAmount = amountBeforeDeposit+(request.Amount-(request.Amount * (percentage / 100)));

            #endregion

            #region Act
            var response = await controller.PostDeposit(request);
            var okResponse = response as OkObjectResult;
            var data = okResponse.Value as BaseResponse<PostDepositResponse>;

            #endregion

            #region Assert
            Assert.NotNull(okResponse);
            Assert.Equal(200, okResponse.StatusCode);
            Assert.NotNull(data.Responsedata);
            Assert.Null(data.Error);

            //check db record
            var test = Context.BankAccount.Where(x => true).ToList();

            var balanceAfterDeposit = Context.BankAccount.Where(x => x.CustomerId == mockData.Customers[0].CustomerId)
                                    .FirstOrDefault().ClosingBalance;

            
            Assert.Equal(exceptedDepositAmount, balanceAfterDeposit);
            

            #endregion

        }

        [Fact, TestPriority(1)]
        public async Task FundTransfer_ShouldSuccess()
        {
            #region Arrange
            InsertOrUpdateData();

            var oldSenderBalance = mockData.Customers[0].BankAccount[0].ClosingBalance;
            var oldReceiverBalance = mockData.Customers[1].BankAccount[0].ClosingBalance;

            var request = new PostFundTransferRequest
            {
                CustomerId = mockData.Customers[0].CustomerId,
                BankAccountId = mockData.Customers[0].BankAccount[0].BankAccountId,
                ReceiverCustomerId = mockData.Customers[1].CustomerId,
                ReceiverBankAccountId = mockData.Customers[1].BankAccount[0].BankAccountId,
                Amount = 1000,
                Note = "transfer for test"
            };
            #endregion

            #region Act
            var response = await controller.FundTransfer(request);
            var okResponse = response as OkObjectResult;
            var data = okResponse.Value as BaseResponse<PostFundTransferResponse>;

            #endregion

            #region Assert
            Assert.NotNull(okResponse);
            Assert.Equal(200, okResponse.StatusCode);
            Assert.NotNull(data.Responsedata);
            Assert.Null(data.Error);

            //check db record
           
            var senderBankAccount = Context.BankAccount.Where(x => x.CustomerId == mockData.Customers[0].CustomerId).FirstOrDefault();
            var receiverBankAccount = Context.BankAccount.Where(x => x.CustomerId == mockData.Customers[1].CustomerId).FirstOrDefault();

            Assert.NotNull(senderBankAccount);
            Assert.NotNull(receiverBankAccount);
            Assert.Equal(oldSenderBalance - request.Amount, senderBankAccount.ClosingBalance);
            Assert.Equal(oldReceiverBalance + request.Amount, receiverBankAccount.ClosingBalance);

            #endregion

        }
    }
}

