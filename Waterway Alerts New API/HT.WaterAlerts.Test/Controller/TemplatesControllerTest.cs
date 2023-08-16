using AutoFixture.Xunit2;

namespace HT.WaterAlerts.Test.Controller
{
    public class TemplatesControllerTest
    {
        [Theory,AutoMoqData]
        public void Post_ActionExecutes_ShouldReturnGUID(TemplateDTO templateDTO, Guid id, [Frozen] Mock<ITemplateService> mockService, [Greedy] TemplatesController sut)
        {
            mockService.Setup(a => a.CreateTemplate(It.IsAny<TemplateDTO>())).Returns(id);
            var actionResult = sut.Post(templateDTO);
            var response = actionResult as OkObjectResult;
            var result = response?.Value;

            response?.StatusCode.Should().Be(200);
            result.Should().Be(id);
        }

        [Theory, AutoMoqData]
        public void Post_GivenInvalidDTOActionExecute_ShouldThrowException(TemplateDTO templateDTO, [Frozen] Mock<ITemplateService> mockService, [Greedy] TemplatesController sut)
        {
            mockService.Setup(a=>a.CreateTemplate(It.IsAny<TemplateDTO>())).Throws<Exception>();    

            var response = sut.Post(templateDTO) as BadRequestObjectResult;

            response?.StatusCode.Should().Be(400);
        }

        [Theory, AutoMoqData]
        public void Put_GivenValidDTOActionExecute_ShouldReturnOk(TemplateDTO templateDTO, [Frozen] Mock<ITemplateService> mockService, [Greedy] TemplatesController sut)
        {
            var response = sut.Put(templateDTO) as OkObjectResult;
            response?.StatusCode.Should().Be(200);
        }
    }
}
