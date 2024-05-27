using System.Text;

namespace NET1814_MilkShop.Services.CoreHelpers.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Normalize string for case-insensitive and Unicode normalization
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Return a normalized string or null if input is null</returns>
        public static string? Normalize(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            //Trim for trimming spaces
            //ToLowerInvariant for case-insensitive
            //Normalize for Unicode normalization
            return input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormC);
        }
    }
}
