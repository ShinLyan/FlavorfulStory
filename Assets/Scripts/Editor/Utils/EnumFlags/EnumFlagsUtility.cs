using System;
using System.Collections.Generic;
using System.Linq;

namespace FlavorfulStory.Editor.Utils.EnumFlags
{
    /// <summary> Утилита для работы с enum, имеющими атрибут <c>[Flags]</c>. </summary>
    /// <remarks> Позволяет проверять наличие флагов, получать список активных значений
    /// и проверять состояние <c>Everything</c> или <c>None</c>. </remarks>
    public static class EnumFlagsUtility
    {
        /// <summary> Установлен ли указанный флаг в значении перечисления? </summary>
        /// <param name="value"> Текущее значение перечисления. </param>
        /// <param name="flag"> Флаг, который нужно проверить. </param>
        /// <typeparam name="T"> Перечисление с атрибутом [Flags]. </typeparam>
        /// <returns> Возвращает true, если флаг установлен, иначе false. </returns>
        public static bool HasFlag<T>(T value, T flag) where T : Enum =>
            (Convert.ToInt32(value) & Convert.ToInt32(flag)) != 0;

        /// <summary> Получить список всех установленных флагов в значении перечисления. </summary>
        /// <param name="value"> Текущее значение перечисления. </param>
        /// <typeparam name="T"> Перечисление с атрибутом [Flags]. </typeparam>
        /// <returns> Список активных флагов. </returns>
        public static List<T> GetSelectedFlags<T>(T value) where T : Enum =>
            Enum.GetValues(typeof(T)).Cast<T>()
                .Where(enumValue => Convert.ToInt32(enumValue) != 0 && HasFlag(value, enumValue))
                .ToList();

        /// <summary> Проверить, выбраны ли все возможные флаги (кроме None). </summary>
        /// <param name="value"> Текущее значение перечисления. </param>
        /// <typeparam name="T"> Перечисление с атрибутом [Flags]. </typeparam>
        /// <returns> Возвращает true, если установлены все возможные флаги, иначе false. </returns>
        public static bool IsEverythingSelected<T>(T value) where T : Enum =>
            Convert.ToInt32(value) == ~0;

        /// <summary> Проверить, не установлен ли ни один флаг. </summary>
        /// <param name="value"> Текущее значение перечисления. </param>
        /// <typeparam name="T"> Перечисление с атрибутом [Flags]. </typeparam>
        /// <returns> Возвращает true, если не установлен ни один флаг, иначе false. </returns>
        public static bool IsNothingSelected<T>(T value) where T : Enum =>
            Convert.ToInt32(value) == 0;
    }
}