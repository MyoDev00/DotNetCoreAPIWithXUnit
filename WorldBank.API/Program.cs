using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WorldBank.API.Helper;
using WorldBank.API.Middleware;
using WorldBank.Entities;
using WorldBank.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Logging.ClearProviders();
builder.Services.AddLogging();

ConfigurationManager configuration = builder.Configuration;


builder.Services.AddDbContext<WorldBankDBContext>(options =>
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);

});
builder.Services.AddTransient<IUnitOfWork<WorldBankDBContext>, UnitOfWork<WorldBankDBContext>>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

builder.Services.AddSingleton<JWTTokenHelper>(jwt =>
    new JWTTokenHelper(new JWTTokenHelperParameters
    {
        JwtKey = configuration["JWT:SecurityKey"],
        JwtAudience = configuration["JWT:Audience"],
        JwtIssuer = configuration["JWT:Issuer"],
        TokenExpireInMinute = int.Parse(configuration["JWT:TokenExpireInMinute"])
    }));


var app = builder.Build();
app.ConfigureExceptionHandler();
// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
