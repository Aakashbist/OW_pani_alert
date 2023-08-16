using HT.WaterAlerts.Domain;
using Microsoft.AspNetCore.JsonPatch;

namespace HT.WaterAlerts.Service
{
    public interface ITemplateService
    {
        Guid CreateTemplate(TemplateDTO template);
        void UpdateTemplate(TemplateDTO template);
    }
}
