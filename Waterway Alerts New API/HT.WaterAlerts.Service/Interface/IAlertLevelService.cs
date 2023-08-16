using HT.WaterAlerts.Domain;
using Microsoft.AspNetCore.JsonPatch;

namespace HT.WaterAlerts.Service
{
    public interface IAlertLevelService
    {
        DataTableResponseDTO GetAlertLevels(DataTableRequestDTO request);
        void UpdateAlertLevelsPartial(Guid id, JsonPatchDocument patchLevels);
    }
}
