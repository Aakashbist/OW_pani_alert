namespace HT.WaterAlerts.Domain
{
    public class MeasurementSitesDTO
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Coordinates { get; set; }
        public string Type { get; set; }
        public string MeasurementType { get; set; }
        public string MeasurementUnit { get; set; }
        public string Description { get; set; }
        public bool IsAutomated { get; set; }
        public bool Status { get; set; }
        public List<AlertLevelsDTO>? AlertLevels { get; set; }

        public MeasurementSitesDTO MapToMeasurementSitesDTO(MeasurementSite site)
        {
            return new MeasurementSitesDTO()
            {
                Id = site.Id,
                Name = site.Name,
                Coordinates = site.Coordinates,
                Type = site.Type,
                Description = site.Description,
                MeasurementType = site.MeasurementType.Name,
                MeasurementUnit = site.MeasurementUnit.Name,
                IsAutomated = site.IsAutomated,
                AlertLevels = site.AlertLevels != null ? site.AlertLevels
                    .Select(al => new AlertLevelsDTO().MapToAlertLevelsDTO(al)).OrderBy(o => o.Name).ToList() : null
            };
        }

        public MeasurementSitesDTO MapToOnlyManualMeasurementSitesDTO(MeasurementSite site)
        {
            return new MeasurementSitesDTO()
            {
                Id = site.Id,
                Name = site.Name,
                Coordinates = site.Coordinates,
                Type = site.Type,
                Description = site.Description,
                MeasurementType = site.MeasurementType.Name,
                MeasurementUnit = site.MeasurementUnit.Name,
                IsAutomated = site.IsAutomated,
                Status = site.Status
            };
        }
    }
}
