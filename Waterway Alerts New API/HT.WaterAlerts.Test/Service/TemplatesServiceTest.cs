using AutoFixture.Xunit2;
using HT.WaterAlerts.Core;
using Microsoft.Data.SqlClient;
using System.Linq.Expressions;

namespace HT.WaterAlerts.Test.Service
{
    public class TemplatesServiceTest
    {
        [Theory, AutoMoqData]
        public void CreateTemplate_GivenValidTemplate_ShouldExcuteOnce([Frozen] Mock<IUnitOfWork> mockUow,
                                                                              TemplateDTO templateDTO,
                                                                              [Greedy] TemplateService sut)
        {
            sut.CreateTemplate(templateDTO);
            mockUow.Verify(X => X.GetRepository<Template>().Add(It.IsAny<Template>()), Times.AtMost(1));
        }

        [Theory, AutoMoqData]
        public void UpdateTemplate_GivenValidTemplate_ShouldExcuteOnce([Frozen] Mock<IUnitOfWork> mockUow,
                                                                              TemplateDTO templateDTO,
                                                                              [Greedy] TemplateService sut)
        {
            sut.UpdateTemplate(templateDTO);
            mockUow.Verify(X => X.GetRepository<Template>().Update(It.IsAny<Template>()), Times.AtMost(1));
        }
    }
}
