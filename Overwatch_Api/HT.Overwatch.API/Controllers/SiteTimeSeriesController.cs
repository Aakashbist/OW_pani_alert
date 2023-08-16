using HT.Overwatch.API.Common;
using HT.Overwatch.Application.Features.SiteTimeSeries;
using HT.Overwatch.Contract.Requests;
using HT.Overwatch.Contract.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HT.Overwatch.API.Controllers
{

    public class SiteTimeSeriesController : CommonController
    {
        private readonly ISender _mediator;

        public SiteTimeSeriesController(ISender mediator)
        {
            _mediator = mediator;
        }

        [MultiplePolicysAuthorize(new[] { Policies.AdminUser, Policies.NormalUser }, false)]
        [HttpPost("GetTimeSeries")]
        public async Task<IActionResult> GetTimeSeries([FromBody] FilterTimeseriesOptionsRequest request)
        {
            var response = await _mediator.Send(new GetFilterTimeseries.Query()
            {
                FilterTimeseriesOptions = request.FilterTimeseriesOptions
            });
            var apiResponse = response.Select(y => new SiteTimeSeriesApiResponse
            {
                Key = y.key,
                SiteTimeSeriesApiResponsesItems = y.SiteTimeseriesItems.Select(x => new SiteTimeSeriesApiResponseItem
                {
                    Metric = x.Metric,
                    Time = x.Time,
                    Value = Math.Round(x.Value, 3),
                    MeasurementUnit = x.MeasurementUnit,
                    LocationName = x.LocationName,
                    ParameterName = x.ParameterName
                }).ToList()

            });
            return Ok(apiResponse);
        }
    }
}
