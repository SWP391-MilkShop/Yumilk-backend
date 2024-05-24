namespace NET1814_MilkShop.Repositories.Models
{
    public abstract class QueryModel //Neu chi dung de ke thua thi nen dung abstract
    {
        public string? SearchTerm { get; set; }
        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
