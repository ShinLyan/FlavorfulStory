using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FlavorfulStory.Utils
{
    /// <summary> Конвертер enum-значений в строки. </summary>
    public static class EnumToStringConvertor
    {
        /// <summary> Преобразует значение enum в строку по определённым правилам. </summary>
        /// <typeparam name="TEnum"> Тип перечисления. </typeparam>
        /// <param name="value"> Значение перечисления. </param>
        /// <returns> Отформатированная строка с пробелами между словами, приведёнными к нижнему регистру,
        /// с заглавной первой буквой. </returns>
        /// <remarks> Enum value "Get money", после преобразования "Get money". </remarks>
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