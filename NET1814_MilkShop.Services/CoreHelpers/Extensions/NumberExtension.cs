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
    /// <summary>
    /// Dùng để áp dụng phần trăm cho một số nguyên (làm tròn)
    /// </summary>
    /// <param name="number"></param>
    /// <param name="percentage"></param>
    /// <returns></returns>
    public static int ApplyPercentage(this int number, int percentage)
    {
        var result = number * (percentage / 100.0);
        var roundedResult = Math.Round(result);
        return roundedResult.ToInt();
    }
}