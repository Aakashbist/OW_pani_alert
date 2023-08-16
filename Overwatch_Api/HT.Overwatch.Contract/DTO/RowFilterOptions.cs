namespace HT.Overwatch.Contract.DTO
{
    public class RowFilterOptions
    {
        public int Id { get; set; }
        public Location Location { get; set; }
        public Site Site { get; set; }
        public Parameter Parameter { get; set; }
        public Variable Variable { get; set; }
    }
}
