namespace HT.Overwatch.Contract.DTO
{
    public class FilterTimeseriesOptions
    {
        public List<RowFilterOptions> RowFilterOptions { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Point { get; set; }
    }
}
