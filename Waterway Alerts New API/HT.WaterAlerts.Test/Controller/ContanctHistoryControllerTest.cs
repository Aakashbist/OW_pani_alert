using AutoFixture.Xunit2;

namespace HT.WaterAlerts.Test.Controller
{
    public class ContanctHistoryControllerTest
    {
        [Theory, AutoMoqData]
        public void Get_GivenValidUserId_ShouldReturnHttpStatusOk(
           Guid Id,
           [CollectionSize(3)]IEnumerable<ContactHistoryDTO> history,
           [Frozen] Mock<IContactHistoryService> mockService,
           [Greedy] ContactHistoryController sut)
        {
            mockService.Setup(x => x.GetContactHistories(It.IsAny<Guid>())).Returns(history);

            var actionResult =  sut.Get(Id);
            var response = actionResult as OkObjectResult;
            var result = response?.Value as IEnumerable<ContactHistoryDTO>;


            response?.StatusCode.Should().Be(200);
            result?.Count().Should().Be(3);
        }

        [Theory, AutoMoqData]
        public void Get_GivenInvalidUserId_ShouldReturnHttpNotFoundWithCountZero(
           Guid Id,
           [CollectionSize(0)] IEnumerable<ContactHistoryDTO> history,
           [Frozen] Mock<IContactHistoryService> mockService,
           [Greedy] ContactHistoryController sut)
        {
            mockService.Setup(x => x.GetContactHistories(It.IsAny<Guid>())).Returns(history);

            var actionResult = sut.Get(Id);
            var response = actionResult as NotFoundObjectResult;
            var result = response?.Value as IEnumerable<ContactHistoryDTO>;


            response?.StatusCode.Should().Be(404);
            result?.Count().Should().Be(0);
        }

        [Theory, AutoMoqData]
        public void Get_GivenNotExistingUserId_ShouldReturnHttpBadRequest(
          Guid Id,
         
          [Frozen] Mock<IContactHistoryService> mockService,
          [Greedy] ContactHistoryController sut)
        {
            mockService.Setup(x => x.GetContactHistories(It.IsAny<Guid>())).Returns(()=>null);

            var actionResult = sut.Get(Id);
            var response = actionResult as BadRequestObjectResult;

            response?.StatusCode.Should().Be(400);
        }

        [Theory,AutoMoqData]
        public void Post_ActionExecutes_ShouldExecuteOnce(ContactHistoryPostDTO contactHistory, [Greedy] ContactHistoryController sut)
        {
            var response = sut.Post(contactHistory) as OkObjectResult;

            response?.StatusCode.Should().Be(200);
        }

        [Theory, AutoMoqData]
        public void Post_GivenInvalidDTOActionExecute_ShouldExecuteOnce(ContactHistoryPostDTO contactHistory, [Frozen] Mock<IContactHistoryService> mockService, [Greedy] ContactHistoryController sut)
        {
            mockService.Setup(a=>a.SaveContactHistories(It.IsAny<ContactHistoryPostDTO>())).Throws<Exception>();    

            var response = sut.Post(contactHistory) as BadRequestObjectResult;

            response?.StatusCode.Should().Be(400);
        }
    }
}
