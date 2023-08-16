namespace HT.Overwatch.Application.ResponseModels.TimeseriesValue
{
    public class TimeseriesValueResponse
    {
        public DateTime Time { get; set; }
        public int TimeSeriesId { get; set; }
        public double Value { get; set; }
        public short Quality { get; set; }
    }
}
