using HT.Overwatch.Domain.Entity;
using HT.Overwatch.Domain.Model.Metadata;

namespace HT.Overwatch.Domain.Model
{
    public class QuickLink : Entity<int>
    {
        private QuickLink(int id, string name, string url, int? siteId): base(id)
        {
            Name = name;
            Url = url;
            SiteId = siteId;
        }

        public string Name { get; set; }
        public string Url { get; set; }
        public int? SiteId { get; set; }

        public virtual Site Site { get; private set; }

        public static QuickLink CreateQuickLink(int id, int? siteId, string url, string name)
        {
            return new (id, name, url, siteId);
        }
    }
}
