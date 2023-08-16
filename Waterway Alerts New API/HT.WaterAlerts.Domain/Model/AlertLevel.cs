using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HT.WaterAlerts.Domain
{
    public class AlertLevel : CommonModel
    {
        [StringLength(100)]
        public string Name { get; set; }
        [MaxLength]
        public string? Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Level { get; set; }
        [ForeignKey("MeasurementSite")]
        public Guid MeasurementSiteId { get; set; }
        [ForeignKey("MeasurementType")]
        public Guid MeasurementTypeId { get; set; }
        [ForeignKey("Template")]
        public Guid? TemplateId { get; set; }
        public virtual MeasurementSite MeasurementSite { get; set; }
        public virtual MeasurementType MeasurementType { get; set; }
        public virtual Template Template { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; }

        public static AlertLevel CreateAlertLevel(MeasurementSite measurementSite, Template template, Guid typeId)
        {
            return new AlertLevel()
            {
                MeasurementSiteId = measurementSite.Id,
                Description = string.Empty,
                TemplateId = template.Id,
                MeasurementTypeId = typeId,
                CreatedDate = DateTime.Now,
                Level = 0,
            };
        }
    }
}

