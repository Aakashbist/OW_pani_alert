namespace HT.WaterAlerts.Domain
{
    public class DataTableResponseDTO
    {
        public int TotalCount { get; set; }
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public object[] Data { get; set; }
    }
}
