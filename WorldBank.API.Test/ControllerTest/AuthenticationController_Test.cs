using Microsoft.AspNetCore.Mvc;
using WorldBank.API.Business;
using WorldBank.API.Controllers;
using WorldBank.API.Helper;
using WorldBank.API.Test.Fixture;
using WorldBank.Entities;
using WorldBank.Shared.RequestModel;
using WorldBank.Shared.ResponseModel;
using WorldBank.Shared.ResponseModel.CommonResponse;
using WorldBank.UnitOfWork;

namespace WorldBank.API.Test.ControllerTest
{
    [Collection("ControllerIntegrationTest")]
    public class AuthenticationController_Test : IClassFixture<TestDatabaseFixture>
    {
        public AuthenticationController controller;
        public WorldBankDBContext Context { get; }
        MockData mockData;

        public AuthenticationController_Test(TestDatabaseFixture dBFixture)
        {
            Context = dBFixture.CreateContext();

            var unitOfWork = new UnitOfWork<WorldBankDBContext>(Context);
            var tokenHelper = new JWTTokenHelper(new JWTTokenHelperParameters
            {
                JwtAudience = "http://worldbank.net",
                JwtIssuer = "http://worldbank.net",
                JwtKey = "W0rlDB@9K7Wtk89*",
                TokenExpireInMinute = 10
            });

            var authenticationBL = new AuthenticationBL(unitOfWork, tokenHelper);

            controller = new AuthenticationController(authenticationBL);
        }

        public void InsertOrUpdateData()
        {
            mockData = new MockData();

           
            if (!Context.Staff.Any(x => x.StaffId == mockData.Staff[0].StaffId))
            {
                Context.Add(mockData.Staff[0]);
            }

            try
            {
                Context.SaveChanges();
            }
            catch (Exception ex)
            {

            }

        }


        [Fact]
        public async Task Test_PostLogin()
        {
            #region Arrange
            InsertOrUpdateData();
            var request = new PostLoginRequest
            {
                LoginId = "Admin",
                Password = "password",
            };

            #endregion

            #region Act
            var response = await controller.Login(request);
            var okResponse = response as OkObjectResult;
            var data = okResponse.Value as BaseResponse<PostLoginResponse>;

            #endregion

            #region Assert
            Assert.Equal(200, okResponse.StatusCode);
            Assert.NotNull(data.Responsedata);
            Assert.Null(data.Error);
            Assert.Equal(mockData.Staff[0].FullName, data.Responsedata.StaffName);
            #endregion
        }
    }
}
