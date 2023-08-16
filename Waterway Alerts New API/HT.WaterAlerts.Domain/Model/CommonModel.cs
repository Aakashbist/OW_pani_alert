using System.ComponentModel.DataAnnotations;

namespace HT.WaterAlerts.Domain
{
    public class CommonModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}