using HT.Overwatch.Domain.Entity;

namespace HT.Overwatch.Domain.Model.Metadata
{
    public class Parameter : Entity<int>
    {
        private Parameter(int id, string name, string? shortName, string unitOfMeasure, string? description)
            : base(id)
        {
            Name = name;
            ShortName = shortName;
            UnitOfMeasure = unitOfMeasure;
            Description = description;
        }

        public string Name { get; private set; }
        public string? ShortName { get; private set; }
        public string UnitOfMeasure { get; private set; }
        public string? Description { get; private set; } = null!;

        public static Parameter CreateParameter(int id, string name, string? shortName, string unitOfMeasure, string? description)
        {
            return new(id, name, shortName, unitOfMeasure, description);
        }

    }
}