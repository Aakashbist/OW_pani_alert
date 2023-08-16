using System.ComponentModel.DataAnnotations;

namespace HT.WaterAlerts.Domain
{
    public class Template : CommonModel
    {
        [StringLength(100)]
        public string Name { get; set; }
        [MaxLength]
        public string? SMS { get; set; }
        [MaxLength]
        public string? Email { get; set; }
        [MaxLength]
        public string? TextToVoice { get; set; }
        public static Template CreateTemplate(string name)
        {
            return new Template()
            {
                Name = name,
                Email = string.Empty,
                CreatedDate = DateTime.Now
            };
        }
    }
}

