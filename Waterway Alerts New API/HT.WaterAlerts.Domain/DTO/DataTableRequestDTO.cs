namespace HT.WaterAlerts.Domain
{
    public class DataTableRequestDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string OrderColumn { get; set; }
        public string OrderDirection { get; set; }
        public string SearchColumn { get; set; }
        public string SearchValue { get; set; }
    }
}
