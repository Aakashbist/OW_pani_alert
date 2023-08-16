using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HT.Overwatch.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/")]
    public class CommonController : ControllerBase
    {
    }
}