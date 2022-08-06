using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WorldBank.API.Helper
{
    public class JWTTokenHelperParameters
    {
        public string JwtKey { get; set; }
        public string? JwtAudience { get; set; }
        public string? JwtIssuer { get; set; }
        public int TokenExpireInMinute { get; set; }
        public int? RefreshTokenExpireInDay { get; set; }

    }
    public class JWTTokenHelper
    {

        JWTTokenHelperParameters parameters;
        string JwtKey { get => parameters.JwtKey; }
        public string? JwtAudience { get => parameters.JwtAudience; }
        public string? JwtIssuer { get => parameters.JwtIssuer; }
        public int TokenExpireInMinute { get => parameters.TokenExpireInMinute; }
        public int? RefreshTokenExpireInDay { get => parameters.RefreshTokenExpireInDay; }

        public JWTTokenHelper(JWTTokenHelperParameters parameters)
        {
            this.parameters = parameters;
        }
       
        public string GenerateToken(Claim[] claims,out DateTime expireDateTime)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var Vobj_SecurityCertificate = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            expireDateTime = DateTime.UtcNow.AddMinutes(TokenExpireInMinute);

            var Vobj_SecurityToken = new JwtSecurityToken(
                     audience: JwtAudience,
                     issuer: JwtIssuer,
                     claims: claims,
                     notBefore: DateTime.UtcNow,
                     expires: expireDateTime,
                     signingCredentials: Vobj_SecurityCertificate
                     );
            var token = new JwtSecurityTokenHandler().WriteToken(Vobj_SecurityToken);

            return token;
        }

        public bool ValidateToken(string token)
        {
            TokenValidationParameters validationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidIssuer = JwtIssuer,
                ValidAudience = JwtAudience
            };
            SecurityToken validatedToken;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            var payload = handler.ValidateToken(token, validationParameters, out validatedToken);

            if (payload != null)
                return false;
            else
                return true;
        }
    }

    
}
