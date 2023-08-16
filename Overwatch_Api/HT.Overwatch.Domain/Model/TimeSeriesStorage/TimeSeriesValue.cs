using HT.Overwatch.Domain.Model.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HT.Overwatch.Domain.Model.TimeSeriesStorage
{ 
    public class TimeSeriesValue
    {
        private TimeSeriesValue(DateTime time, int timeSeriesId, double value, short quality, int? interpolation)
        {
            Time = time;
            TimeSeriesId = timeSeriesId;
            Value = value;
            Quality = quality;
            Interpolation = interpolation;
        }

        [Key, Column(Order = 0)]
        public DateTime Time { get; private set; }
        [Key, Column(Order = 1)]
        public int TimeSeriesId { get; private set; }

        public double Value { get; private set; }
        public short Quality { get; private set; }
        public int? Interpolation { get; private set; }

        public virtual TimeSeries TimeSeries { get; private set; }

        public static TimeSeriesValue CreateTimeSeriesValue(DateTime time, int timeSeriesId, double value, short quality,int? interpolation)
        {
            return new(time, timeSeriesId, value, quality, interpolation);
        }


    }
}
