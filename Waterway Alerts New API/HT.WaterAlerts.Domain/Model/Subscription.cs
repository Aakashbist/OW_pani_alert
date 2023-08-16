using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HT.WaterAlerts.Domain
{
    public class Subscription : CommonModel
    {
        [StringLength(100)]
        public string Type { get; set; }
        public bool Status { get; set; }
        [ForeignKey("AlertLevel")]
        public Guid AlertLevelId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public virtual AlertLevel AlertLevel { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}

