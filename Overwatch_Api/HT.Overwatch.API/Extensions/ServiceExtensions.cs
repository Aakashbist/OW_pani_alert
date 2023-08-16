namespace HT.Overwatch.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddCorsOriginsFromConfiguration(this IServiceCollection services, IConfiguration configuration, string policyName)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.AddCors(options =>
            {
                options.AddPolicy(name: policyName,
                                  policy =>
                                  {
                                      policy.WithOrigins(configuration.GetSection(policyName).Get<string[]>())
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowCredentials();
                                  });
            });
        }
    }
}
