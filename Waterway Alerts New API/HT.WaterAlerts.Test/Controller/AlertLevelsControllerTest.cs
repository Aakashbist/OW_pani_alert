using AutoFixture.Xunit2;
using Microsoft.AspNetCore.JsonPatch;

namespace HT.WaterAlerts.Test.Controller
{
    public class AlertLevelsControllerTest
    {
        [Theory, AutoMoqData]
        public void Get_ShouldReturnHttpStatusOk(
          DataTableRequestDTO dataTableRequest,
          [Greedy] AlertLevelsController sut)
        {
            var actionResult = sut.Get(dataTableRequest);
            var response = actionResult as OkObjectResult;
            var result = response?.Value as DataTableResponseDTO;

            response.Should().NotBeNull();
            response?.StatusCode.Should().Be(200);
            result?.Data.Should().NotBeNull();
        }

        [Theory, AutoMoqData]
        public void Get_GivenNoAlertLevelsAvailable_ShouldReturnNotFoundRequest(DataTableRequestDTO dataTableRequest,
            DataTableResponseDTO dataTableResponse,
            [Frozen] Mock<IAlertLevelService> mockService,
          [Greedy] AlertLevelsController sut)
        {
            dataTableResponse.Data = new object[0];
            mockService.Setup(x => x.GetAlertLevels(It.IsAny<DataTableRequestDTO>())).Returns(dataTableResponse);
            
            var actionResult = sut.Get(dataTableRequest);
            var response = actionResult as NotFoundObjectResult;
            
            response.Should().NotBeNull();
            response?.StatusCode.Should().Be(404);
        }

        [Theory, AutoMoqData]
        public void Get_GivenIfAlertLevelsIsNull_ShouldReturnHttpBadRequest(DataTableRequestDTO dataTableRequest,
            [Frozen] Mock<IAlertLevelService> mockService,
            [Greedy] AlertLevelsController sut)
        {
            mockService.Setup(x => x.GetAlertLevels(It.IsAny<DataTableRequestDTO>())).Returns(() => null);
            var actionResult = sut.Get(dataTableRequest);
            var response = actionResult as BadRequestObjectResult;

            response.Should().NotBeNull();
            response?.StatusCode.Should().Be(400);
        }

        [Theory, AutoMoqData]
        public void Patch_GivenValidAlertLevelId_ShouldReturnHttpOk(Guid Id,
                                                    JsonPatchDocument patchLevels,
                                                    [Greedy] AlertLevelsController sut)
        {

            var response = sut.Patch(Id, patchLevels) as OkObjectResult;
            response?.StatusCode.Should().Be(200);
        }
    }
}
