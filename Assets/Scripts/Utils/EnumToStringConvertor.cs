using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FlavorfulStory.Utils
{
    /// <summary> Универсальный конвертер enum-ов в строку с автоматическим форматированием CamelCase. </summary>
    public static class EnumToStringConvertor
    {
        /// <summary> Преобразует значение enum в строку, разделяя слова в CamelCase стиле. </summary>
        public static string ToDisplayName<TEnum>(this TEnum value) where TEnum : Enum
        {
            string name = value.ToString();

            string spaced = Regex.Replace(name, "(?<!^)([A-Z])", " $1");

            string lower = spaced.ToLower(CultureInfo.InvariantCulture);
            string result = char.ToUpper(lower[0]) + lower[1..];

            return result;
        }
    }
}