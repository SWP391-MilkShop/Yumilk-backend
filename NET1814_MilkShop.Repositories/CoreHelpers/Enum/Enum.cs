namespace NET1814_MilkShop.Repositories.CoreHelpers.Enum;

public enum OrderStatusId
{
    Pending = 1,
    Processing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5,
    Preorder = 6,
}

public enum RoleId
{
    Admin = 1,
    Staff = 2,
    Customer = 3,
}

public enum ProductStatusId
{
    Selling = 1,
    Preordered = 2, //move out of product
    OutOfStock = 3
}