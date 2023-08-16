namespace HT.WaterAlerts.Domain
{
    public class AlertLevelsDTO
    {
        public Guid? Id { get; set; }
        public Guid MeasurementSiteId { get; set; }
        public Guid MeasurementTypeId { get; set; }
        public Guid TemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Level { get; set; }
        public string MeasurementType { get; set; }

        public AlertLevelsDTO MapToAlertLevelsDTO(AlertLevel alertLevel)
        {
            return new AlertLevelsDTO()
            {
                Id = alertLevel.Id,
                Name = alertLevel.Name,
                Description = alertLevel.Description,
                Level = alertLevel.Level,
                MeasurementType = alertLevel.MeasurementType.Name
            };
        }
    }
}
