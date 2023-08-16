using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HT.WaterAlerts.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CommonController : ControllerBase
    {
        protected bool IsUserValid(Guid userId)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (!IsUserAdmin())
            {
                string? tokenUserId = identity?.Claims?.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name, StringComparison.OrdinalIgnoreCase))?.Value;
                return tokenUserId.Equals(userId.ToString()) ? true : false;
            }
            else
            {
                return true;
            }
        }

        protected bool IsUserAdmin()
        {
            var identity = User.Identity as ClaimsIdentity;
            var roles = identity?.Claims?.Where(c => c.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase)).ToArray();
            return Array.Exists(roles, x => x.Value.ToLower().Equals("admin"));
        }
    }
}