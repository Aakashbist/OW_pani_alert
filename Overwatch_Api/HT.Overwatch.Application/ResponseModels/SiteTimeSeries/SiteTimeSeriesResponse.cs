
namespace HT.Overwatch.Application.ResponseModels.SiteTimeSeries
{
    public class SiteTimeSeriesResponse
    {
        public string key { get; set; }
        public List<SiteTimeseriesItem> SiteTimeseriesItems { get; set; }
    }
    public class SiteTimeseriesItem 
    {
        public string Metric { get; set; }
        public double Value { get; set; }
        public DateTime Time { get; set; }
        public string LocationName { get; set; }
        public string ParameterName { get; set; }
        public string MeasurementUnit { get; set; }
    }
}
