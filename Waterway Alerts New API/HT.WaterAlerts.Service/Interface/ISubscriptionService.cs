using HT.WaterAlerts.Domain;

namespace HT.WaterAlerts.Service
{
    public interface ISubscriptionService
    {
        void SaveSubscription(SubscriptionDTO subscription);
        IEnumerable<SubscriptionResponseDTO> GetSubscriptions(Guid userId);
    }
}
