using HT.Overwatch.Domain.Entity;
using HT.Overwatch.Domain.Model.TimeSeriesStorage;

namespace HT.Overwatch.Domain.Model.Metadata
{
    public class TimeSeries : Entity<int>
    {
        private TimeSeries(int id, string name, string? description, int locationId, int parameterId)
            : base(id)
        {
            Name = name;
            Description = description;
            LocationId = locationId;
            ParameterId = parameterId;
        }

        public string Name { get; private set; }
        public string? Description { get; private set; }
        public int LocationId { get; private set; }
        public int ParameterId { get; private set; }

        public virtual Location Location { get; private set; }
        public virtual Parameter Parameter { get; private set; }
        public virtual ICollection<TimeSeriesValue> TimeSeriesValues { get; private set; }
        public virtual ICollection<TimeSeriesComment> TimeSeriesComments { get; private set; }

        public static TimeSeries CreateTimeSeries(int id, string name, string? description, int locationId, int parameterId)
        {
            return new(id, name, description, locationId, parameterId);
        }
    }
}
