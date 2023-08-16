using AutoFixture.Xunit2;

namespace HT.WaterAlerts.Test.Controller
{
    public class MeasurementSitesControllerTest
    {
        [Theory, AutoMoqData]
        public void Get_ShouldReturnHttpStatusOk(
          Guid Id,
          [CollectionSize(3)] IEnumerable<MeasurementSitesDTO> sites,
          [Frozen] Mock<IMeasurementSiteService> mockService,
          [Greedy] MeasurementSitesController sut)
        {
            mockService.Setup(x => x.GetSites()).Returns(sites);
            var actionResult = sut.Get();
            var response = actionResult as OkObjectResult;
            var result = response?.Value as IEnumerable<MeasurementSitesDTO>;


            response?.StatusCode.Should().Be(200);
            response.Should().NotBeNull();
            result?.Count().Should().Be(3);
        }

        [Theory, AutoMoqData]
        public void Get_GivenNoSitesAvailableInDB_ShouldReturnNotFoundRequestWithCountZero([CollectionSize(0)] IEnumerable<MeasurementSitesDTO> sites,
                                                                                           [Frozen] Mock<IMeasurementSiteService> mockService,
                                                                                           [Greedy] MeasurementSitesController sut)
        {
            mockService.Setup(x => x.GetSites()).Returns(sites);
            var actionResult = sut.Get();
            var response = actionResult as NotFoundObjectResult;
            var result = response?.Value as IEnumerable<MeasurementSitesDTO>;


            response?.StatusCode.Should().Be(404);
            response.Should().NotBeNull();
            result?.Count().Should().Be(0);
        }

        [Theory, AutoMoqData]
        public void Get_GivenIfSitesIsNull_ShouldReturnHttpBadRequest( [Frozen] Mock<IMeasurementSiteService> mockService,
                                                                        [Greedy] MeasurementSitesController sut)
        {
            mockService.Setup(x => x.GetSites()).Returns(() => null);
            var actionResult = sut.Get();
            var response = actionResult as BadRequestObjectResult;


            response?.StatusCode.Should().Be(400);
            response.Should().NotBeNull();
        }
    }
}
