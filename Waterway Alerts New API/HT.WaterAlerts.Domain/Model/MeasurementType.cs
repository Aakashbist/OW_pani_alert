using System.ComponentModel.DataAnnotations;

namespace HT.WaterAlerts.Domain
{
    public class MeasurementType : CommonModel
    {
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string Type { get; set; }
    }
}

