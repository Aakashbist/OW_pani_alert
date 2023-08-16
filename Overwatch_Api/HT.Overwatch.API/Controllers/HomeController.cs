using HT.Overwatch.API.Common;
using HT.Overwatch.API.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;

namespace HT.Overwatch.API.Controllers
{
    [MultiplePolicysAuthorize(new[] { Policies.AdminUser, Policies.NormalUser },false)]
    public class HomeController : CommonController
    {

        private readonly IAuthorizationPolicyProvider _authorisationService;
        private readonly IPolicyEvaluator _policyService;

        public HomeController(IAuthorizationPolicyProvider authorisationService, IPolicyEvaluator policyService)
        {
            _authorisationService = authorisationService;
            _policyService = policyService;
        }

        [HttpGet("userpolicies")]
        public async Task<UserPolicyAuthorisationListDto> GetUserPolicies()
        {
            try
            {
                var userPolicies = new List<UserPolicyAuthorisationDto>();

                foreach (var policyName in Policies.All)
                {
                    var policy = await _authorisationService.GetPolicyAsync(policyName).ConfigureAwait(true);
                    var authorisationResult = await _policyService.AuthorizeAsync(policy, AuthenticateResult.NoResult(), HttpContext, null).ConfigureAwait(true);
                    userPolicies.Add(
                        new UserPolicyAuthorisationDto()
                        {
                            PolicyName = policyName,
                            Authorised = authorisationResult.Succeeded
                        });
                }

                return new UserPolicyAuthorisationListDto(userPolicies);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
              
    }
}
