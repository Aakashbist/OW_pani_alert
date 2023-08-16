using HT.Overwatch.Domain.Entity;
using HT.Overwatch.Domain.Model.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HT.Overwatch.Domain.Model.TimeSeriesStorage
{
    public class TimeSeriesComment
    {
        private TimeSeriesComment(DateTime timeStart, DateTime? timeEnd, int timeSeriesId, string comment)
        {
            TimeStart = timeStart;
            TimeEnd = timeEnd;
            TimeSeriesId = timeSeriesId;
            Comment = comment;
        }

        [Key, Column(Order = 0)]
        public DateTime TimeStart { get; private set; }
        public DateTime? TimeEnd { get; private set; } = null!;
        [Key, Column(Order = 1)]
        public int TimeSeriesId { get; private set; }
        public string Comment { get; private set; } = null!;

        public virtual TimeSeries TimeSeries { get; private set; }

        public static TimeSeriesComment CreateTimeSeriesComment(DateTime timeStart, DateTime? timeEnd, int timeSeriesId, string comment)
        {
            return new (timeStart, timeEnd, timeSeriesId, comment);
        }
    }
}
