using HT.Overwatch.API.Common;
using HT.Overwatch.Application.Features.Sites;
using HT.Overwatch.Contract.Requests;
using HT.Overwatch.Contract.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HT.Overwatch.API.Controllers
{

    public class SiteController : CommonController
    {
        private readonly ISender _mediator;

        public SiteController(ISender mediator)
        {
            _mediator = mediator;
        }


        [MultiplePolicysAuthorize(new string[] {Policies.AdminUser,Policies.NormalUser},false)]
        [HttpGet("GetAllSites")]
        public async Task<IActionResult> Sites([FromQuery] SiteApiRequest request)
        {
            var response = await _mediator.Send(new GetSites.Query() { Name = request.Name });
            var apiResponse = response.Select(x => new KeyValueApiResponse()
            {
                Id = x.Id,
                Name = x.Name,
            });
            return Ok(apiResponse);

        }
    }
}
