using System.Text.RegularExpressions;

namespace NET1814_MilkShop.Repositories.CoreHelpers.Regex;

public static partial class PhoneNumberRegex
{
    public const string Pattern = @"(84|0[3|5|7|8|9])+([0-9]{8})\b";

    [GeneratedRegex(Pattern)]
    public static partial System.Text.RegularExpressions.Regex PhoneRegex();
}