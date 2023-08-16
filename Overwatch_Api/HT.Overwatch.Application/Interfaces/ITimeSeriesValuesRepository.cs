using HT.Overwatch.Application.ResponseModels.SiteTimeSeries;
using HT.Overwatch.Contract.DTO;

namespace HT.Overwatch.Application.Interfaces
{
    public interface ITimeSeriesValuesRepository
    {
        List<TimeSeriesResult> GetTimeSeriesForFilters(DateTime fromDate, DateTime toDate, int point, List<RowFilterOptions> rowFilterOptions);
    }
}
