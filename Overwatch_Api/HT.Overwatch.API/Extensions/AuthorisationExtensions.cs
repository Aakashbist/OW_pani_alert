using Microsoft.AspNetCore.Authorization;

namespace HT.Overwatch.API.Extensions
{
    public static class AuthorisationExtensions
    {

        public static void AddPolicyAuthorisationWithRolesFromConfiguration(
            this IServiceCollection services,
            IConfiguration configuration,
            params string[] policies)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.AddAuthorizationCore(o =>
            {
                foreach (var policy in policies)
                {
                    AddPolicyWithRolesFromConfiguration(o, configuration, policy);
                }
            });
        }

        private static void AddPolicyWithRolesFromConfiguration(
            AuthorizationOptions authorizationOptions,
            IConfiguration configuration,
            string policyName)
        {
            var rolesForPolicy = configuration[$"{policyName}Roles"];

            if (!string.IsNullOrWhiteSpace(rolesForPolicy))
            {
                authorizationOptions.AddPolicy(
                    policyName,
                    policy =>
                    {
                        var roles = rolesForPolicy
                            .Split(',')
                            .Select(p => p.Trim());

                        policy.RequireRole(roles);
                    });
            }
        }
    }
}
