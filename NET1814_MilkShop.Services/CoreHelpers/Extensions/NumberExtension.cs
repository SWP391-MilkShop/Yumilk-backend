namespace NET1814_MilkShop.Services.CoreHelpers.Extensions;

public static class NumberExtension
{
    public static int ToInt(this decimal number)
    {
        return Convert.ToInt32(number);
    }

    public static int ToInt(this double number)
    {
        return Convert.ToInt32(number);
    }
}