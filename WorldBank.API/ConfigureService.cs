
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.Text;
using WorldBank.API.Business;
using WorldBank.API.Helper;
using WorldBank.Entities;
using WorldBank.Shared.ResponseModel.CommonResponse;
using WorldBank.UnitOfWork;
using static WorldBank.Shared.Constant.Constant;

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
                        options.Events = new JwtBearerEvents
                        {
                            OnChallenge = async context =>
                            {
                                var tmp = configuration["JWT:Issuer"];
                                tmp = configuration["JWT:Audience"];
                                tmp = configuration["JWT:SecurityKey"];
                                var t = context.Request.Headers["Authorization"];
                                // Override the response status code.
                                context.Response.StatusCode = 401;
                                // Emit the WWW-Authenticate header.
                                context.Response.Headers.Add(
                                     HeaderNames.WWWAuthenticate,
                                     context.Options.Challenge);
                                string responseBody = Newtonsoft.Json.JsonConvert.SerializeObject(new ErrorResponse(ErrorCode.UnAuthorize,ErrorMessage.UnAuthorize));
                                byte[] finalBytes = Encoding.UTF8.GetBytes(responseBody);
                                await context.Response.Body.WriteAsync(finalBytes);
                                context.HandleResponse();
                            }

                        };
                    });

            services.AddTransient(jwt =>
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
            services.AddTransient<ICustomerBL, CustomerBL>();
            services.AddTransient<IBankServiceBL, BankServiceBL>();
            services.AddTransient<IMasterDataBL, MasterDataBL>();
            #endregion
            return services;
        }
    }
}
