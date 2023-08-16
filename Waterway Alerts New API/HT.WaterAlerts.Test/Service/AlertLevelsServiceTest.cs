using AutoFixture.Xunit2;
using HT.WaterAlerts.Core;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Data.SqlClient;
using System.Linq.Expressions;

namespace HT.WaterAlerts.Test.Service
{
    public class AlertLevelsServiceTest
    {
        [Theory, AutoMoqData]
        public void GetAlertLevels_GivenValidRequest_ShouldReturnDataTableResponseDTO(DataTableRequestDTO dataTablerequestDTO,
                                                                    [CollectionSize(20)] List<AlertLevel> levels,
                                                                    [Frozen] Mock<IUnitOfWork> mockUow,
                                                                    [Greedy] AlertLevelService sut)
        {
            dataTablerequestDTO.SearchColumn = String.Empty;
            dataTablerequestDTO.OrderColumn = String.Empty;
            dataTablerequestDTO.PageNumber = 1;
            dataTablerequestDTO.PageSize = 10;
            var param = ServiceHelper.GetDataTableParams(dataTablerequestDTO);
            var criteria = ServiceHelper.GetFilterPredicate<AlertLevel>(param.Filter);
            var orderBy = ServiceHelper.GetOrderPredicate<AlertLevel>(param.OrderColumn);

            mockUow.Setup(X => X.GetRepository<AlertLevel>().Get(It.IsAny<Expression<Func<AlertLevel, bool>>>(),
                                                                    It.IsAny<Expression<Func<AlertLevel, object>>>(),
                                                                    It.IsAny<SortOrder>(),
                                                                    It.IsAny<int>(),
                                                                    It.IsAny<int>()))
                                                                    .Returns(levels.AsQueryable());
            mockUow.Setup(X => X.GetRepository<AlertLevel>().Count(It.IsAny<Expression<Func<AlertLevel, bool>>>())).Returns(20);
            
            var result = sut.GetAlertLevels(dataTablerequestDTO);

            result.Should().NotBeNull();
            result.TotalPage.Should().Be(2);
            result.CurrentPage.Should().Be(1);
            result.TotalCount.Should().Be(20);
            result.Data.Should().NotBeNullOrEmpty();
        }

        [Theory, AutoMoqData]
        public void UpdateAlertLevelsPartial_GivenValidData_ShouldCallServiceOneTimes([Frozen] Mock<IUnitOfWork> mockUow,
                                                                              JsonPatchDocument patchLevels,
                                                                              AlertLevel level,
                                                                              [Greedy] AlertLevelService sut)
        {
            mockUow.Setup(X => X.GetRepository<AlertLevel>().GetById(It.IsAny<Guid>())).Returns(level);
            sut.UpdateAlertLevelsPartial(Guid.NewGuid(), patchLevels);

            mockUow.Verify(X => X.GetRepository<AlertLevel>().GetById(It.IsAny<Guid>()), Times.AtMost(1));

        }
    }
}
