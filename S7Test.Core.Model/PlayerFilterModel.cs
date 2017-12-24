namespace S7Test.Core.Model
{
    public class PlayerFilterModel
    {
        public string Keyword { get; set; }
        public int? TeamId { get; set; }
        public int Page { get; set; }
        public int pageSize { get; set; }
        public OrderBy OrderBy { get; set; }
    }
}
