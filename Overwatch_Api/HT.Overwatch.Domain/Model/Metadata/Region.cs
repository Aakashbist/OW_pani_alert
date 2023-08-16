using HT.Overwatch.Domain.Entity;

namespace HT.Overwatch.Domain.Model.Metadata
{
    public class Region : Entity<int>
    {
        private Region(int id, string name, string? description)
            : base(id)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }
        public string? Description { get; private set; } = null!;

        public virtual ICollection<Site> Sites { get; private set; }
   
        public static Region CreateRegion(int id,string name, string? description)
        {
            return new(id,name, description);
        }
    }
}
