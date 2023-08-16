using HT.WaterAlerts.Domain;

namespace HT.WaterAlerts.Service
{
    public interface IMeasurementSiteService
    {
        IEnumerable<MeasurementSitesDTO> GetSites();
        DataTableResponseDTO GetSites(DataTableRequestDTO request);
        void UpdateSite(MeasurementSitesDTO siteDto);
        void CreateSite(MeasurementSitesDTO siteDto);
    }
}
