using HT.WaterAlerts.Domain;

namespace HT.WaterAlerts.Service
{
    public interface IContactHistoryService
    {
        IEnumerable<ContactHistoryDTO> GetContactHistories(Guid userId);
        void SaveContactHistories(ContactHistoryPostDTO contactHistory);
        DataTableResponseDTO GetContactHistoriesForManualSites(DataTableRequestDTO request);
    }
}
