using HT.Overwatch.Application.Common;
using HT.Overwatch.Application.Interfaces;
using HT.Overwatch.Application.ResponseModels.SiteTimeSeries;
using HT.Overwatch.Contract.DTO;
using HT.Overwatch.Domain.Model.TimeSeriesStorage;
using MediatR;

namespace HT.Overwatch.Application.Features.SiteTimeSeries
{
    public class GetFilterTimeseries
    {
        public class Query : IRequest<IEnumerable<SiteTimeSeriesResponse>>
        {
            public FilterTimeseriesOptions FilterTimeseriesOptions { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<SiteTimeSeriesResponse>>
        {
            private readonly ITimeSeriesValuesRepository _timeSeriesValuesRepository;
            private readonly IUnitOfWork _uow;

            public Handler(ITimeSeriesValuesRepository timeSeriesValuesRepository, IUnitOfWork uow)
            {
                _timeSeriesValuesRepository = timeSeriesValuesRepository;
                _uow = uow;
            }

            public Task<IEnumerable<SiteTimeSeriesResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                int maxCount = 0;
                int days = (int)Math.Round((request.FilterTimeseriesOptions.ToDate - request.FilterTimeseriesOptions.FromDate).Duration().TotalDays);
                if (days <= Constants.DAYS_LIMIT && request.FilterTimeseriesOptions.Point == Constants.LTTB_RESOLUTION)
                {
                    foreach (var rowFilterOption in request.FilterTimeseriesOptions.RowFilterOptions)
                    {
                        int count = _uow.GetRepository<TimeSeriesValue>()
                                        .Get(x => x.Time >= request.FilterTimeseriesOptions.FromDate &&
                                                        x.Time <= request.FilterTimeseriesOptions.ToDate &&
                                                        x.TimeSeries.Location.Id == rowFilterOption.Location.Id &&
                                                        x.TimeSeries.Location.Site.Id == rowFilterOption.Site.Id &&
                                                        x.TimeSeries.Parameter.Id == rowFilterOption.Parameter.Id &&
                                                        x.TimeSeries.Id == rowFilterOption.Variable.Id)
                                        .Count();

                        if (count > maxCount)
                        {
                            maxCount = count;
                            request.FilterTimeseriesOptions.Point = maxCount;
                        }
                    }
                }

                var result = _timeSeriesValuesRepository.GetTimeSeriesForFilters(request.FilterTimeseriesOptions.FromDate,
                                                                                                    request.FilterTimeseriesOptions.ToDate,
                                                                                                    request.FilterTimeseriesOptions.Point,
                                                                                                    request.FilterTimeseriesOptions.RowFilterOptions);

                return Task.FromResult(result.GroupBy(grp => new { Metric = grp.Metric }).Select(y => new SiteTimeSeriesResponse
                {
                    key = y.Key.Metric.ToString(),
                    SiteTimeseriesItems = y.Select(x => new SiteTimeseriesItem
                    {
                        Metric = x.Metric,
                        Time = x.Time,
                        Value = x.Value,
                        LocationName = x.Location,
                        ParameterName = x.Parameter,
                        MeasurementUnit = x.MeasurementUnit,

                    }).ToList()
                }));
            }
        }
    }
}
