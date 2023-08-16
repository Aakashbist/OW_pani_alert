
using HT.Overwatch.API;
using HT.Overwatch.API.Common;
using HT.Overwatch.API.Extensions;
using HT.Overwatch.Application;
using HT.Overwatch.Infrastructure;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Server.IISIntegration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var deploymentModel = builder.Configuration["DeploymentModel"];
var allowSpecificOrigins = "CorsOrigins";
var connectionString = "DefaultConnection";

if (deploymentModel.Equals("IIS", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
}
else
{
    builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
}

builder.Services.AddPolicyAuthorisationWithRolesFromConfiguration(builder.Configuration, Policies.All);

//Logging Configuration
var filename = $"Log_{DateTime.Now.ToString("yyyyMMdd")}.txt";
string fullpath = Path.Combine(builder.Configuration.GetSection("LogFilePath").Value.ToString(), filename);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(fullpath)
    .WriteTo.Console()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

// Add services to the container.
builder.Services.AddInfrastructureServices(builder.Configuration, connectionString);
builder.Services.AddApplicationServices();
builder.Services.AddApiServices();

builder.Services.AddControllers();
builder.Services.AddCorsOriginsFromConfiguration(builder.Configuration, allowSpecificOrigins);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseCors(allowSpecificOrigins);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


