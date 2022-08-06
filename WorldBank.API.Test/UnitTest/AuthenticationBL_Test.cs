using Moq;
using WorldBank.API.Business;
using WorldBank.API.Helper;
using WorldBank.API.Test.Mock;
using WorldBank.Entities;
using WorldBank.Entities.DataModel;
using WorldBank.Shared.Constant;
using WorldBank.Shared.Helper;
using WorldBank.Shared.RequestModel;
using WorldBank.UnitOfWork;
using static WorldBank.Shared.Constant.Constant;

namespace WorldBank.API.Test
{
    public class AuthenticationBL_Test
    {
        UnitOfWork<WorldBankDBContext> unitOfWork;
        JWTTokenHelper tokenHelper;

        public AuthenticationBL_Test()
        {
            var AdminhashPassword = PBKDF2HashHelper.HashPassword("password");
            var ManagerhashPassword = PBKDF2HashHelper.HashPassword("long9@ssword112233445566");
            var staffData = new List<Staff>
            {
                new Staff{ StaffId=Guid.NewGuid(),LoginId="admin",FullName="Admin",Password= AdminhashPassword.Hash,SaltPassword = AdminhashPassword.Salt,Status=(int)CommonStatus.Active},
                new Staff{ StaffId=Guid.NewGuid(),LoginId="manager",FullName="Manager",Password= ManagerhashPassword.Hash,SaltPassword = ManagerhashPassword.Salt,Status=(int)CommonStatus.Inactive}
            };

            var staffMockSet = new MockDbSet<Staff>(staffData);
            var mockContext = new Mock<WorldBankDBContext>();
            //mockContext.Setup(c => c.Accounts).Returns(mockSet.Object);
            mockContext.Setup(c => c.Set<Staff>()).Returns(staffMockSet.Object);
            unitOfWork = new UnitOfWork<WorldBankDBContext>(mockContext.Object);
            tokenHelper = new JWTTokenHelper(new JWTTokenHelperParameters
            {
                JwtAudience = "http://worldbank.net",
                JwtIssuer = "http://worldbank.net",
                JwtKey = "W0rlDB@9K7Wtk89*",
                TokenExpireInMinute = 10
            });

        }
        [Theory]
        [InlineData("admin","password",true,"Admin")]
        [InlineData("manager", "long9@ssword112233445566", true,"Manager")]
        [InlineData("admin", "wrongpassword", false)]
        public async Task Test_Login(string loginId,string password, bool isSuccessResult, string? acctualAccountName=null)
        {
            
            AuthenticationBL authnBL = new AuthenticationBL(unitOfWork,tokenHelper);
            
            var response = await authnBL.PostLogin(new PostLoginRequest
            {
                LoginId = loginId,
                Password = password,
            });


            Assert.NotNull(response);
            
            if (isSuccessResult)
            {
                Assert.Null(response.Error);
                Assert.Equal(acctualAccountName, response.Responsedata.StaffName);
                Assert.False(string.IsNullOrEmpty(response.Responsedata.AuthorizeToken));
                Assert.True(tokenHelper.ValidateToken(response.Responsedata.AuthorizeToken));
            }
            else
            {
                Assert.NotNull(response.Error);
                Assert.Equal(ErrorCode.LoginFailed, response.Error[0].ErrorCode);
                Assert.Null(response.Responsedata);
            }
        }
    }
}