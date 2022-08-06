using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WorldBank.API.Helper;
using WorldBank.Entities.DataModel;
using WorldBank.Shared.Constant;
using WorldBank.Shared.Helper;
using WorldBank.Shared.RequestModel;
using WorldBank.Shared.ResponseModel;
using WorldBank.Shared.ResponseModel.CommonResponse;
using WorldBank.UnitOfWork;
using static WorldBank.Shared.Constant.Constant;

namespace WorldBank.API.Business
{
    public interface IAuthenticationBL
    {
        public Task<BaseResponse<PostLoginResponse>> PostLogin(PostLoginRequest request);
    }
    public class AuthenticationBL : IAuthenticationBL
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly JWTTokenHelper tokenHelper;

        public AuthenticationBL(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public AuthenticationBL(IUnitOfWork unitOfWork, JWTTokenHelper tokenHelper)
        {
            this.unitOfWork = unitOfWork;
            this.tokenHelper = tokenHelper;
        }

        public async Task<BaseResponse<PostLoginResponse>> PostLogin(PostLoginRequest request)
        {
            var response = new BaseResponse<PostLoginResponse>();

            //var hashResult = PBKDF2HashHelper.HashPassword(request.Password);
            var staff = unitOfWork.GetRepository<Staff>()
                        .GetByCondition(x=>
                           x.LoginId==request.LoginId
                        )
                        .Select(x=> new Staff
                        {
                            StaffId = x.StaffId,
                            FullName = x.FullName,
                            Password=x.Password,
                            SaltPassword=x.SaltPassword,
                        })
                        .FirstOrDefault();

            if (staff != null && PBKDF2HashHelper.ValidatePassword(request.Password,new PBKDF2HashHelper.PBKDF2Result
            {
                Hash = staff.Password,
                Salt = staff.SaltPassword
            }))
            {

                DateTime expiryDate;

                var claims = new[]
                       {
                        new Claim(CustomClaims.AccountId,staff.StaffId.ToString()),
                        new Claim(CustomClaims.Name,staff.FullName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                var token = tokenHelper.GenerateToken(claims,out expiryDate);
                
                response.Responsedata = new PostLoginResponse
                {
                    AuthorizeToken = token,
                    AuthorizeTokenExpireDate = expiryDate,
                    StaffName = staff.FullName
                };
                return response;
            }
            else
            {
                response.Error = new Error
                {
                    ErrorCode = ErrorCode.LoginFailed,
                    ErrorMessage = ErrorMessage.LoginFailed
                };
                return response;
            }
        }

    }
}
