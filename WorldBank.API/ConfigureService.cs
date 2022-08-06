
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WorldBank.API.Business;
using WorldBank.API.Helper;
using WorldBank.Entities;
using WorldBank.UnitOfWork;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigureService
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, ConfigurationManager configuration)
        {

            #region DB
            services.AddDbContext<WorldBankDBContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);

            });
            services.AddTransient<IUnitOfWork, UnitOfWork<WorldBankDBContext>>();
            #endregion

            #region Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = configuration["JWT:Issuer"],
                            ValidAudience = configuration["JWT:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecurityKey"])),
                        };
                    });

            services.AddTransient<JWTTokenHelper>(jwt =>
                new JWTTokenHelper(new JWTTokenHelperParameters
                {
                    JwtKey = configuration["JWT:SecurityKey"],
                    JwtAudience = configuration["JWT:Audience"],
                    JwtIssuer = configuration["JWT:Issuer"],
                    TokenExpireInMinute = int.Parse(configuration["JWT:TokenExpireInMinute"])
                }));
            #endregion

            #region BusinessLogic
            services.AddTransient<IAuthenticationBL, AuthenticationBL>();
            #endregion
            return services;
        }
    }
}
