namespace HT.WaterAlerts.Domain
{
    public class SubscriptionDTO
    {
        public Guid UserId { get; set; }
        public List<SubscriptionResponseDTO> Subscriptions { get; set; }
    }
}

