using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HT.WaterAlerts.Domain
{
    public class ContactHistory : CommonModel
    {
        [MaxLength]
        public string Content { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public Guid? MessageId { get; set; }
        [StringLength(100)]
        public string AlertType { get; set; }
        [ForeignKey("AlertLevel")]
        public Guid? AlertLevelId { get; set; }
        [ForeignKey("User")]
        public Guid? UserId { get; set; }
        public virtual AlertLevel AlertLevel { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string BatchNumber { get; set; }
        public Guid? CreatedUserId { get; set; }
        public virtual ApplicationUser CreatedUser { get; set; }
    }
}

