using HT.Overwatch.Application.Interfaces;
using HT.Overwatch.Application.ResponseModels.SiteTimeSeries;
using HT.Overwatch.Contract.DTO;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace HT.Overwatch.Infrastructure.Persistence
{
    public class TimeSeriesValuesRepository : ITimeSeriesValuesRepository
    {
        private readonly OverwatchDbContext? _context;

        public TimeSeriesValuesRepository(OverwatchDbContext? context)
        {
            _context = context;
        }


        public List<TimeSeriesResult> GetTimeSeriesForFilters(DateTime fromDate, DateTime toDate, int point, List<RowFilterOptions> rowFilterOptions)
        {
            List<TimeSeriesResult> results = new List<TimeSeriesResult>();
            foreach (var filter in rowFilterOptions)
            {
                string sql = @"SELECT CONCAT(ids.""name"", '/', idl.""name"", '/', idp.""name"", ' (', idp.unit_of_measure , ')') as metric, 
                        val.time as time, 
                        val.value as value,
                        idp.unit_of_measure as unit, idl.""name"" as location, idp.""name"" as parameter
                        FROM (
                        SELECT lttb(tsv.""time"", tsv.value, @points) as time_value, tsv.time_series_id as id
                        FROM time_series_values tsv
                        INNER JOIN time_series ts ON ts.id = tsv.time_series_id
                        INNER JOIN locations idl ON idl.id = ts.location_id
                        INNER JOIN sites ids ON ids.id = idl.site_id
                        INNER JOIN parameters idp ON idp.id = ts.parameter_id
                        WHERE tsv.""time"" BETWEEN @startDate AND @endDate
                        AND idp.id = @parameterId
                        AND idl.id = @locationId
                        AND ids.id = @siteId
                        AND ts.id = @variableId
                        AND tsv.quality > 3
                        GROUP BY tsv.time_series_id
                        ) AS tsv
                        CROSS JOIN unnest(time_value) AS val(time, value)
                        INNER JOIN time_series ts ON ts.id = tsv.id
                        INNER JOIN locations idl ON idl.id = ts.location_id
                        INNER JOIN sites ids ON ids.id = idl.site_id
                        INNER JOIN regions rgn ON rgn.id = ids.region_id
                        INNER JOIN parameters idp ON idp.id = ts.parameter_id
                        ORDER BY time ASC";

                var parameterIdParam = new NpgsqlParameter("@parameterId", NpgsqlDbType.Integer) { Value = filter.Parameter.Id };
                var locationIdParam = new NpgsqlParameter("@locationId", NpgsqlDbType.Integer) { Value = filter.Location.Id };
                var siteIdParam = new NpgsqlParameter("@siteId", NpgsqlDbType.Integer) { Value = filter.Site.Id };
                var variableIdParam = new NpgsqlParameter("@variableId", NpgsqlDbType.Integer) { Value = filter.Variable.Id };

                results.AddRange(_context.TimeseriesDataRetrivals.FromSqlRaw(sql,
                          new NpgsqlParameter("@startDate", NpgsqlDbType.TimestampTz) { Value = fromDate },
                          new NpgsqlParameter("@endDate", NpgsqlDbType.TimestampTz) { Value = toDate },
                          new NpgsqlParameter("@points", NpgsqlDbType.Integer) { Value = point },
                          parameterIdParam,
                          locationIdParam,
                          siteIdParam,
                          variableIdParam
                      ).Select(x => new TimeSeriesResult
                      {
                          Metric = x.Metric,
                          Time = x.Time,
                          Location = x.Location,
                          Parameter = x.Parameter,
                          MeasurementUnit = x.Unit,
                          Value = x.Value,
                      }).ToList());
            }
            return results;
        }
    }
}
