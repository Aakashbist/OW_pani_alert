namespace HT.Overwatch.Application.ResponseModels.Common
{
    public class PaginationResult<T>
    {
        public int TotalPages { get; set; }
        public int PreviousPage { get; set; }
        public int NextPage { get; set; }
        public int TotalItems { get; set; }
        public IList<T> Items { get; set; }
    }
}
