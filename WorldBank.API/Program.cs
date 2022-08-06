using WorldBank.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Logging.ClearProviders();
builder.Services.AddLogging();

ConfigurationManager configuration = builder.Configuration;
builder.Services.AddAppServices(configuration);

var app = builder.Build();
app.ConfigureExceptionHandler();
// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

