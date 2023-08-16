using HT.Overwatch.API.Common;
using HT.Overwatch.Application.Features.Variables;
using HT.Overwatch.Contract.Requests;
using HT.Overwatch.Contract.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HT.Overwatch.API.Controllers
{
    public class VariableController : CommonController
    {
        private readonly ISender _mediator;

        public VariableController(ISender mediator)
        {
            _mediator = mediator;
        }

        [MultiplePolicysAuthorize(new[] { Policies.AdminUser, Policies.NormalUser }, false)]
        [HttpGet("GetVariables")]
        public async Task<IActionResult> GetParametersName([FromQuery] VariableRequest request)
        {
            var response = await _mediator.Send(new GetVariables.Query() { SiteId = request.SiteId, LocationId = request.LocationId, ParameterId = request.ParameterId });
            var apiResponse = response.Select(y => new KeyValueApiResponse
            {
                Id = y.Id,
                Name = y.Name

            });

            return Ok(apiResponse);

        }
    }
}
