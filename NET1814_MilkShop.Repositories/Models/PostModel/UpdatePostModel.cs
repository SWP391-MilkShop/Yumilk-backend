namespace NET1814_MilkShop.Repositories.Models.PostModel
{
    public class UpdatePostModel
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
