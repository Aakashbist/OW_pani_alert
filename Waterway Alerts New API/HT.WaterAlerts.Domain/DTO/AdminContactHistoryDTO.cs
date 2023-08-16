
namespace HT.WaterAlerts.Domain
{
    public class AdminContactHistoryDTO
    {
        public string AlertLevel { get; set; }
        public string CreatedByUser { get; set; }
        public List<string> Recipent { get; set; }
        public string Content { get; set; }
        public string Site { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Id { get; set; }
    }
}