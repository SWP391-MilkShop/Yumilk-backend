namespace NET1814_MilkShop.Repositories.Models.ImageModels
{
    public class ImgurData
    {
        public string Id { get; set; } = null!;
        public string Deletehash { get; set; } = null!;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Size { get; set; }
        public string Link { get; set; } = null!;
        // Add other properties as needed
    }

    public class ImgurResponse
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public ImgurData? Data { get; set; }
    }
}
