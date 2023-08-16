using System.ComponentModel.DataAnnotations;

namespace HT.WaterAlerts.Domain
{
    public class MeasurementUnit : CommonModel
    {
        [StringLength(100)]
        public string Name { get; set; }
    }
}

