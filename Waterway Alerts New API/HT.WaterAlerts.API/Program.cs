using HT.WaterAlerts.Common.Email;
using HT.WaterAlerts.Core;
using HT.WaterAlerts.DatabaseMigration;
using HT.WaterAlerts.Domain;
using HT.WaterAlerts.Service;
using HT.WaterAlerts.Service.Implementation;
using HT.WaterAlerts.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDatabaseContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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


builder.Services.AddControllers().AddNewtonsoftJson();


builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IMeasurementSiteService, MeasurementSiteService>();
builder.Services.AddTransient<ISubscriptionService, SubscriptionService>();
builder.Services.AddTransient<IContactHistoryService, ContactHistoryService>();
builder.Services.AddTransient<IAlertLevelService, AlertLevelService>();
builder.Services.AddTransient<ITemplateService, TemplateService>();
builder.Services.AddTransient<IMeasurementTypeService, MeasurementTypeService>();
builder.Services.AddTransient<IMeasurementUnitService, MeasurementUnitService>();

// configure identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
{
    o.Password.RequireDigit = true;
    o.Password.RequireLowercase = false;
    o.Password.RequireUppercase = false;
    o.Password.RequireNonAlphanumeric = true;
    o.Password.RequiredLength = 8;
    o.Stores.MaxLengthForKeys = 128;
    o.SignIn.RequireConfirmedEmail = true;
    o.User.RequireUniqueEmail = true;
}).AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AppDatabaseContext>();

// configure smtp
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddTransient<ISmtpClient, SmtpClient>();


var appSettingsKey = builder.Configuration.GetSection("SecuredKey").Value.ToString();
var key = Encoding.ASCII.GetBytes(appSettingsKey);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(y =>
{
    y.RequireHttpsMetadata = false;
    y.SaveToken = true;
    y.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "WaterAlerts API" });
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Format: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new string[] { }
                    }
                });
});


var app = builder.Build();
var env = builder.Configuration.GetSection("Environment").Value.ToString().ToLower();
if (env == "development")
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HT.WaterAlerts API");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

