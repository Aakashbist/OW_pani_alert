using AutoFixture.Xunit2;
using HT.WaterAlerts.Common.Email;
using HT.WaterAlerts.Core;
using Microsoft.Data.SqlClient;
using System.Linq.Expressions;

namespace HT.WaterAlerts.Test.Service
{
    public class ContactHistoryServiceTest
    {
        [Theory, AutoMoqData]
        public void GetContactHistories_GivenValidUserId_ShouldReturnListOfContactHistoryDTO(Guid Id,
                                                                                            [Frozen] Mock<IUnitOfWork> mockUow,
                                                                                            [CollectionSize(6)] List<ContactHistory> contactHistories,
                                                                                            [Greedy] ContactHistoryService sut)
        {
            mockUow.Setup(X => X.GetRepository<ContactHistory>().Get(It.IsAny<Expression<Func<ContactHistory, bool>>>(),
                                                                    It.IsAny<Expression<Func<ContactHistory, object>>>(),
                                                                    It.IsAny<SortOrder>())).Returns(contactHistories.AsQueryable());


            var result = sut.GetContactHistories(Id).ToList();

            result.Should().NotBeNull();
            result.Count.Should().Be(6);
        }

        [Theory, AutoMoqData]
        public void GetContactHistories_GivenInvalidUserId_ShouldReturnCountZero(Guid Id,
                                                                                [Frozen] Mock<IUnitOfWork> mockUow,
                                                                                [CollectionSize(0)] List<ContactHistory> contactHistories,
                                                                                [Greedy] ContactHistoryService sut)
        {
            mockUow.Setup(X => X.GetRepository<ContactHistory>().Get(It.IsAny<Expression<Func<ContactHistory, bool>>>(),
                                                                    It.IsAny<Expression<Func<ContactHistory, object>>>(),
                                                                    It.IsAny<SortOrder>())).Returns(contactHistories.AsQueryable());

            var result = sut.GetContactHistories(Id).ToList();

            result.Should().NotBeNull();
            result.Count.Should().Be(0);
        }

        [Theory, AutoMoqData]
        public void SaveContactHistories_GivenValidContactHistory_ShouldCallServiceThreeTimes([Frozen] Mock<IUnitOfWork> mockUow,
                                                                              ContactHistoryPostDTO contactHistoryPostDTO,
                                                                              [CollectionSize(2)] List<AlertLevel> alertLevels,
                                                                              [Greedy] ContactHistoryService sut)
        {
            mockUow.Setup(X => X.GetRepository<AlertLevel>().Get(It.IsAny<Expression<Func<AlertLevel, bool>>>())).Returns(alertLevels.AsQueryable());
            
            sut.SaveContactHistories(contactHistoryPostDTO);

            mockUow.Verify(X => X.GetRepository<ContactHistory>().Add(It.IsAny<ContactHistory>()), Times.AtMost(3));

        }
    }
}
