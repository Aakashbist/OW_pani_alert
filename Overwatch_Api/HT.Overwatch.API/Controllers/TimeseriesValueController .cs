using HT.Overwatch.API.Common;
using HT.Overwatch.Application.Common;
using HT.Overwatch.Application.Features.TimeseriesValues;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HT.Overwatch.API.Controllers
{

    public class TimeseriesValueController : CommonController
    {
        private readonly ISender _mediator;

        public TimeseriesValueController(ISender mediator)
        {
            _mediator = mediator;
        }


        [MultiplePolicysAuthorize(new string[] { Policies.AdminUser, Policies.NormalUser }, false)]
        [HttpGet("GetTimeseries")]
        public async Task<IActionResult> GetTimeseries([FromQuery] int currentPage = Constants.CURRENT_PAGE, int pageSize = Constants.PAGE_LIMIT)
        {

            var response = await _mediator.Send(new GetTimeseriesValues.Query() { currentPage = currentPage, PageSize = pageSize });
            return Ok(response);

        }
    }
}
