namespace HT.WaterAlerts.Domain
{
    public class ContactHistoryPostDTO
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public Guid CreatedUserId { get; set; }
        public Guid AlertLevelId { get; set; }
    }
}

