using HT.Overwatch.Domain.Entity;

namespace HT.Overwatch.Domain.Model.Metadata
{
    public class Location : Entity<int>
    {
        private Location(int id, string name, int siteId, string? description, string datum, double latitude, double longitude, double elevation)
            : base(id)
        {

            Name = name;
            SiteId = siteId;
            Description = description;
            Datum = datum;
            Latitude = latitude;
            Longitude = longitude;
            Elevation = elevation;
        }

        public string Name { get; private set; }
        public int SiteId { get; private set; }
        public string? Description { get; private set; }
        public string Datum { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public double Elevation { get; private set; }

        public virtual Site Site{ get; private set; }

        public static Location CreateLocation(int id, string name, int siteId, string? description, string datum, double latitude, double longitude, double elevation)
        {
            return new(id, name, siteId, description,datum,latitude,longitude,elevation);
        }


    }
}