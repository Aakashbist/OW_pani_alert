
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HT.Overwatch.API.Common
{
    public class MultiplePolicysAuthorizeAttribute : TypeFilterAttribute
    {
        public MultiplePolicysAuthorizeAttribute(string[] policies, bool isAnd = false) : base(typeof(MultiplePolicysAuthorizeFilter))
        {
            Arguments = new object[] { policies, isAnd };
        }
    }
    public class MultiplePolicysAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly IAuthorizationService _authorization;
        public string[] Policies { get; private set; }
        public bool IsAnd { get; private set; }

        public MultiplePolicysAuthorizeFilter(string[] policies, bool isAnd, IAuthorizationService authorization)
        {
            Policies = policies;
            IsAnd = isAnd;
            _authorization = authorization;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (IsAnd)
            {
                foreach (var policy in Policies)
                {
                    var authorized = await _authorization.AuthorizeAsync(context.HttpContext.User, policy);
                    if (!authorized.Succeeded)
                    {
                        context.Result = new ForbidResult();
                        return;
                    }

                }
            }
            else
            {
                foreach (var policy in Policies)
                {
                    var authorized = await _authorization.AuthorizeAsync(context.HttpContext.User, policy);
                    if (authorized.Succeeded)
                    {
                        return;
                    }

                }
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
