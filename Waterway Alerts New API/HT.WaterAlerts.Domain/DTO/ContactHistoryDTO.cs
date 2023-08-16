namespace HT.WaterAlerts.Domain
{
    public class ContactHistoryDTO
    {
        public string Content { get; set; }
        public string DeliveredAt { get; set; }
        public string Site { get; set; }
        public string AlertLevel { get; set; }
        public string CreatedDate { get; set; }

        public ContactHistoryDTO MapToContactHistoryDTO(ContactHistory history)
        {
            return new ContactHistoryDTO()
            {
                Content = history.Content,
                DeliveredAt = history.DeliveredAt?.ToString("dd/MM/yyyy HH:mm") ?? "N/A",
                CreatedDate = history.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                Site = history.AlertLevel != null ? history.AlertLevel.MeasurementSite.Name : string.Empty,
                AlertLevel = history.AlertLevel != null ? history.AlertLevel.Name : string.Empty,
            };
        }
    }
}

