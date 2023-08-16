using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HT.WaterAlerts.Domain
{
    public class MeasurementSite : CommonModel
    {
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string TimeSeriesId { get; set; }
        public bool Status { get; set; }
        public string Type { get; set; }
        public bool IsAutomated { get; set; }
        [StringLength(100)]
        public string Coordinates { get; set; }
        public string Description { get; set; }
        [ForeignKey("MeasurementType")]
        public Guid MeasurementTypeId { get; set; }
        [ForeignKey("MeasurementUnit")]
        public Guid MeasurementUnitId { get; set; }
        [ForeignKey("OutageStartedTemplate")]
        public Guid OutageStartedTemplateId { get; set; }
        [ForeignKey("OutageStoppedTemplate")]
        public Guid OutageStoppedTemplateId { get; set; }
        public virtual MeasurementType MeasurementType { get; set; }
        public virtual MeasurementUnit MeasurementUnit { get; set; }
        public virtual Template OutageStartedTemplate { get; set; }
        public virtual Template OutageStoppedTemplate { get; set; }
        public ICollection<AlertLevel> AlertLevels { get; set; }


        public static MeasurementSite CreateMeasurementSite(string name, string description, bool status, string coordinates, Guid typeId, Guid unitId, Guid outageStartedTemplate, Guid outageStoppedTemplate)
        {
            return new MeasurementSite
            {
                Name = name,
                Description = description,
                MeasurementTypeId = typeId,
                MeasurementUnitId = unitId,
                OutageStartedTemplateId = outageStartedTemplate,
                OutageStoppedTemplateId = outageStoppedTemplate,
                Status = status,
                IsAutomated = false,
                Coordinates = coordinates,
                CreatedDate = DateTime.Now,
                Type = "public"
            };
        }

    }
}

