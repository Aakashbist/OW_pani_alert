namespace HT.WaterAlerts.Domain
{
    public class SubscriptionResponseDTO
    {
        public Guid AlertlevelId { get; set; }
        public List<string> SubscriptionTypes { get; set; }
    }
}

