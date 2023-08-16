using HT.Overwatch.API.Common;
using HT.Overwatch.Application.Features.Parameters;
using HT.Overwatch.Contract.Requests;
using HT.Overwatch.Contract.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HT.Overwatch.API.Controllers
{
    public class ParametersController : CommonController
    {
        private readonly ISender _mediator;

        public ParametersController(ISender mediator)
        {
            _mediator = mediator;
        }

        [MultiplePolicysAuthorize(new[] { Policies.AdminUser, Policies.NormalUser }, false)]
        [HttpGet("GetParameters")]
        public async Task<IActionResult> GetParametersName([FromQuery] ParameterRequest request)
        {
            var response = await _mediator.Send(new GetParameters.Query() { SiteId = request.SiteId, LocationId = request.LocationId });
            var apiResponse = response.Select(y => new KeyValueApiResponse
            {
                Id = y.Id,
                Name = y.Name

            });

            return Ok(apiResponse);

        }
    }
}
