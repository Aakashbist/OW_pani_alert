using AutoFixture.Xunit2;
using HT.WaterAlerts.Core;
using Microsoft.Data.SqlClient;
using System.Linq.Expressions;

namespace HT.WaterAlerts.Test.Service
{
    public class MeasurementSiteServiceTest
    {
        [Theory, AutoMoqData]
        public void GetSites_ShouldReturnListOfMeasurementSitesDTO([CollectionSize(6)] List<MeasurementSite> sites,
                                                                           [Frozen] Mock<IUnitOfWork> mockUow,
                                                                           [Greedy] MeasurementSiteService sut)
        {
            mockUow.Setup(X => X.GetRepository<MeasurementSite>().Get(It.IsAny<Expression<Func<MeasurementSite, bool>>>(),
                                                                    It.IsAny<Expression<Func<MeasurementSite, object>>>(),
                                                                    It.IsAny<SortOrder>())).Returns(sites.AsQueryable());


            var result = sut.GetSites().ToList();

            result.Should().NotBeNull();
            result.Count.Should().Be(6);

        }

        [Theory, AutoMoqData]
        public void GetSites_GivenIfNoSitesPresent_ShouldReturnCountZero([CollectionSize(0)] List<MeasurementSite> sites,
                                                                           [Frozen] Mock<IUnitOfWork> mockUow,
                                                                           [Greedy] MeasurementSiteService sut)
        {
            mockUow.Setup(X => X.GetRepository<MeasurementSite>().Get(It.IsAny<Expression<Func<MeasurementSite, bool>>>(),
                                                                    It.IsAny<Expression<Func<MeasurementSite, object>>>(),
                                                                    It.IsAny<SortOrder>())).Returns(sites.AsQueryable());

            var result = sut.GetSites().ToList();

            result.Should().NotBeNull();
            result.Count.Should().Be(0);

        }
    }
}
