namespace NET1814_MilkShop.Repositories.CoreHelpers.Enum
{
    public enum OrderStatusId
    {
        PENDING = 1,
        PROCESSING = 2,
        SHIPPING = 3,
        DELIVERED = 4,
        CANCELLED = 5,
        PREORDER = 6,
    }
    public enum RoleId
    {
        ADMIN = 1,
        STAFF = 2,
        CUSTOMER = 3,
    }
    public enum ProductStatusId
    {
        SELLING = 1,
        PREORDER = 2, //move out of product
        OUT_OF_STOCK = 3
    }
}
