namespace HT.WaterAlerts.Domain
{
    public class AlertLevelSubscriptionsDTO : AlertLevelsDTO
    {
        public Guid? TemplateId { get; set; }
        public string MeasurementSite { get; set; }
        public string MeasurementSiteType { get; set; }
        public string MeasurementUnit { get; set; }
        public string TemplateName { get; set; }
        public string EmailTemplate { get; set; }
        public List<string>? SubscribedUsers { get; set; }

        public AlertLevelSubscriptionsDTO MapToAlertLevelSubscriptionsDTO(AlertLevel alertLevel)
        {
            return new AlertLevelSubscriptionsDTO()
            {
                Id = alertLevel.Id,
                Name = alertLevel.Name,
                Description = alertLevel.Description,
                Level = alertLevel.Level,
                MeasurementType = alertLevel.MeasurementType.Name,
                MeasurementSite = alertLevel.MeasurementSite.Name,
                MeasurementSiteType = alertLevel.MeasurementSite.MeasurementType.Name,
                MeasurementUnit = alertLevel.MeasurementSite.MeasurementUnit.Name,
                TemplateName = alertLevel.Template != null ? alertLevel.Template.Name : null,
                TemplateId = alertLevel.Template != null ? alertLevel.Template.Id : null,
                EmailTemplate = alertLevel.Template != null ? alertLevel.Template.Email : null,
                SubscribedUsers = alertLevel.Subscriptions != null ? alertLevel.Subscriptions.Where(x=>x.User.Status).Select(s => s.User.FirstName + " " + s.User.LastName).OrderBy(o => o).ToList() : null
            };
        }
    }
}
