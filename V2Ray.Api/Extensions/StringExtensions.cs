using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace V2Ray.Api.Extensions
{
    public static class StringExtensions
    {

        public static bool ValidPhone(this string phone)
        {
            var phones = phone.TrimStart(new[] { '0' });

            return Regex.IsMatch(phones, @"^(09|9)+([0-9]){9}$");
        }
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input[1..]
            };

        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value[..maxChars] + "...";
        }

        public static bool IsNullOrEmpty(this string input)
        {
            return string.IsNullOrEmpty(input);
        }
    }
}