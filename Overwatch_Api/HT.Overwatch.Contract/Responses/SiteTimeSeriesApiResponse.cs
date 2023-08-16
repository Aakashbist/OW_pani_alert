namespace HT.Overwatch.Contract.Responses
{
    public class SiteTimeSeriesApiResponse
    {
        public string Key { get; set; }
        public List<SiteTimeSeriesApiResponseItem> SiteTimeSeriesApiResponsesItems { get; set; }
    }
    
        public class SiteTimeSeriesApiResponseItem
    {
        public string Metric { get; set; }
        public double Value { get; set; }
        public DateTime Time { get; set; }
        public string LocationName { get; set; }
        public string ParameterName { get; set; }
        public string MeasurementUnit { get; set; }
    }
}
