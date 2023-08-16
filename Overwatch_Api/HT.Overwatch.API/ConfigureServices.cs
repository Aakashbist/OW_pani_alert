using FluentValidation.AspNetCore;
using HT.Overwatch.API.Common;
using Microsoft.AspNetCore.Mvc;

namespace HT.Overwatch.API
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {

            services.AddControllersWithViews(options => options.Filters.Add<ApiExceptionFilterAttribute>());
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
            return services;
        }
    }
}
