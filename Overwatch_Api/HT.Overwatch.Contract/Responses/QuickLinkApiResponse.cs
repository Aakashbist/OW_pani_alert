namespace HT.Overwatch.Contract.Responses
{
    public class QuickLinkApiResponse
    {
        public int Id { get; set; }
        public int? SiteId { get; set; }
        public string Name { get; set; } = default!;
        public string Url { get; set; } = default!;
    }
}
