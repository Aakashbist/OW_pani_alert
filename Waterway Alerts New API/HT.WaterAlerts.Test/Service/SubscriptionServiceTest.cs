using AutoFixture.Xunit2;
using HT.WaterAlerts.Core;
using System.Linq.Expressions;

namespace HT.WaterAlerts.Test.Service
{
    public class SubscriptionServiceTest
    {
        [Theory, AutoMoqData]
        public void GetSubscriptions_GivenValidUserId_ShouldReturnListOfMeasurementSitesDTO([CollectionSize(6)] List<Subscription> subscriptions,
                                                                           [Frozen] Mock<IUnitOfWork> mockUow,
                                                                           [Greedy] SubscriptionService sut)
        {
            var Id = Guid.NewGuid();
            mockUow.Setup(X => X.GetRepository<Subscription>().Get(It.IsAny<Expression<Func<Subscription, bool>>>())).Returns(subscriptions.AsQueryable());

            var result = sut.GetSubscriptions(Id).ToList();

            result.Should().NotBeNull();
            result.Count.Should().Be(6);

        }

        [Theory, AutoMoqData]
        public void GetSubscriptions_GivenInvalidUserId_ShouldReturnCountZero([CollectionSize(0)] List<Subscription> subscriptions,
                                                                           [Frozen] Mock<IUnitOfWork> mockUow,
                                                                           [Greedy] SubscriptionService sut)
        {
            var Id = Guid.NewGuid();
            
            mockUow.Setup(X => X.GetRepository<Subscription>().Get(It.IsAny<Expression<Func<Subscription, bool>>>())).Returns(subscriptions.AsQueryable());


            var result = sut.GetSubscriptions(Id).ToList();

            result.Should().NotBeNull();
            result.Count.Should().Be(0);

        }
    }
}
