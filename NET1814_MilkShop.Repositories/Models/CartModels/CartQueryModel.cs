namespace NET1814_MilkShop.Repositories.Models.CartModels
{
    public class CartQueryModel : QueryModel
    {
        /// <summary>
        /// Sort by product name, price (default is name)
        /// </summary>
        public new string? SortColumn { get; set; }
    }
}
