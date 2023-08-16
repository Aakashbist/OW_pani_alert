namespace HT.Overwatch.Contract.Requests
{
    public class AddQuickLinkRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Url { get; set; } = default!;
        public int SiteId { get; set; }
    }
}
