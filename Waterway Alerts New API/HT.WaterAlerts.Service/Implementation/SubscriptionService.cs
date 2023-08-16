using HT.WaterAlerts.Core;
using HT.WaterAlerts.Domain;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace HT.WaterAlerts.Service
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(IUnitOfWork uow, ILogger<SubscriptionService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public void SaveSubscription(SubscriptionDTO subscription)
        {
            try
            {
                List<Subscription> subscriptionList = new List<Subscription>();

                foreach (SubscriptionResponseDTO subResponse in subscription.Subscriptions)
                {
                    foreach (string type in subResponse.SubscriptionTypes)
                    {
                        if (type.ToLower() == "email" || type.ToLower() == "voice" || type.ToLower() == "sms")
                        {
                            Subscription sub = new Subscription()
                            {
                                CreatedDate = DateTime.Now,
                                AlertLevelId = subResponse.AlertlevelId,
                                UserId = subscription.UserId,
                                Status = true,
                                Type = type
                            };
                            subscriptionList.Add(sub);
                        }
                    }
                    var currentSubcriptionList = _uow.GetRepository<Subscription>().Get(s => s.AlertLevelId == subResponse.AlertlevelId && s.UserId == subscription.UserId).ToList();

                    if (currentSubcriptionList.Count > 0)
                    {
                        _uow.GetRepository<Subscription>().Delete(currentSubcriptionList);
                        _uow.SaveChanges();
                    }
                }

                if (subscriptionList.Count > 0)
                {
                    _uow.GetRepository<Subscription>().Add(subscriptionList);
                    _uow.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured on SaveSubscription for user " + subscription.UserId, ex.Message);
                throw;
            }
        }

        public IEnumerable<SubscriptionResponseDTO> GetSubscriptions(Guid userId)
        {
            try
            {
                var items = _uow.GetRepository<Subscription>().Get(d => d.UserId == userId && d.Status).ToList().GroupBy(a => a.AlertLevelId);
                return items.Select(x => new SubscriptionResponseDTO()
                {
                    AlertlevelId = x.Key,
                    SubscriptionTypes = x.Select(y => y.Type).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured on GetSubscriptions", ex.Message);
                throw;
            }
        }
    }
}
