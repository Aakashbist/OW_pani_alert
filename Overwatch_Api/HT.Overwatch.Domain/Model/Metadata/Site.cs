using HT.Overwatch.Domain.Entity;

namespace HT.Overwatch.Domain.Model.Metadata
{
    public class Site : Entity<int>
    {
        private Site(int id, int regionId, string name, string? description)
            :base(id)
        {
            RegionId = regionId;
            Name = name;
            Description = description;
        }   

        public int RegionId { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }

        public virtual Region Region { get; private set; }
        public virtual ICollection<Location> Locations { get; private set; }
        public virtual ICollection<QuickLink> QuickLinks { get; private set; }

        public static Site CreateSite(int id, int regionId, string name, string? description)
        {
            return new(id,regionId, name, description);
        }
    }
}