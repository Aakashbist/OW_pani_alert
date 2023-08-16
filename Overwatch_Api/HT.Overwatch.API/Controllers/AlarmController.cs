using HT.Overwatch.API.Common;
using HT.Overwatch.Application.Features.Alarms;
using HT.Overwatch.Contract.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HT.Overwatch.API.Controllers
{
    [Authorize(Policy = Policies.NormalUser)]
    public class AlarmController : CommonController
    {
        private readonly ISender _mediator;

        public AlarmController(ISender mediator)
        {
            _mediator = mediator;
        }

        //[HttpGet("GetAllAlarms")]
        //public async Task<IActionResult> Alarms()
        //{
        //    var response = await _mediator.Send(new GetAlarms.Query());
        //    var apiResponse = response.Select(x => new AlarmApiResponse()
        //    {
        //        SiteName = x.SiteName,
        //        Value = x.Value,
        //        DateSent = x.DateSent
        //    });
        //    return Ok(apiResponse);
        //}
    }
}
