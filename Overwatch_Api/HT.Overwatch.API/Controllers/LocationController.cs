using HT.Overwatch.API.Common;
using HT.Overwatch.Application.Features.Locations;
using HT.Overwatch.Contract.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HT.Overwatch.API.Controllers
{
    public class LocationController : CommonController
    {
        private readonly ISender _mediator;

        public LocationController(ISender mediator)
        {
            _mediator = mediator;
        }

        [MultiplePolicysAuthorize(new[] { Policies.AdminUser, Policies.NormalUser }, false)]
        [HttpGet("GetLocations")]
        public async Task<IActionResult> GetLocationNameBySite([FromQuery] int siteId)
        {
            var response = await _mediator.Send(new GetLocations.Query() { SiteId = siteId });
            var apiResponse = response.Select(y => new KeyValueApiResponse
            {
                Id = y.Id,
                Name = y.Name
            });
            return Ok(apiResponse);

        }
    }
}
