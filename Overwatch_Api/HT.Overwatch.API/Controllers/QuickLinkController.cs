using HT.Overwatch.API.Common;
using HT.Overwatch.Application.Features.QuickLinks;
using HT.Overwatch.Contract.Requests;
using HT.Overwatch.Contract.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HT.Overwatch.API.Controllers
{
    public class QuickLinkController : CommonController
    {
        private readonly ISender _mediator;

        public QuickLinkController(ISender mediator)
        {
            _mediator = mediator;
        }


        [MultiplePolicysAuthorize(new string[] { Policies.AdminUser, Policies.NormalUser }, false)]
        [HttpGet("GetQuickLinksBySite")]
        public async Task<IActionResult> GetQuickLinksBySite([FromQuery] int siteId = 0)
        {
            var response = await _mediator.Send(new GetQuickLinksBySite.Query() { SiteId = siteId });

            var apiResponse = response.Select(x => new QuickLinkApiResponse()
            {
                Id = x.Id,
                Name = x.Name,
                SiteId = x.SiteId,
                Url = x.Url,
            });

            return Ok(apiResponse);
        }

        [Authorize(Policy = Policies.AdminUser)]
        [HttpPost("AddQuickLink")]
        public async Task<IActionResult> AddQuickLink([FromBody] AddQuickLinkRequest request)
        {
            var response = await _mediator.Send(new AddQuickLink.Command() { Request = request });

            return Ok(response);
        }

        [Authorize(Policy = Policies.AdminUser)]
        [HttpPut("UpdateQuickLink")]
        public async Task<IActionResult> UpdateQuickLink([FromBody] UpdateQuickLinkRequest request)
        {
            var response = await _mediator.Send(new UpdateQuickLink.Command() { Request = request });

            return Ok(response);
        }

        [Authorize(Policy = Policies.AdminUser)]
        [HttpDelete("DeleteQuickLink")]
        public async Task<IActionResult> DeleteQuickLink([FromQuery] int id)
        {
            var response = await _mediator.Send(new DeleteQuickLink.Command() { Id = id });

            return Ok(response);
        }
    }
}
