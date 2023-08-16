using HT.Overwatch.Application.Interfaces;
using HT.Overwatch.Infrastructure.Common;
using HT.Overwatch.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HT.Overwatch.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, string connectionString)
        {
            services.AddDbContext<OverwatchDbContext>(options => options.UseNpgsql(configuration.GetConnectionString(connectionString)));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDateTime, DateTimeService>();
            services.AddScoped<ITimeSeriesValuesRepository, TimeSeriesValuesRepository>();

            return services;
        }
    }
}
