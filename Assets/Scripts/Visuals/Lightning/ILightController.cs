using FlavorfulStory.TimeManagement;
using UnityEngine;

namespace FlavorfulStory.Visuals.Lightning
{
    /// <summary> Интерфейс для контроллеров освещения, определяющий базовые методы управления источниками света
    /// в зависимости от времени суток. Реализуется контроллерами солнечного и лунного света. </summary>
    public interface ILightController
    {
        /// <summary> Определяет, должен ли источник света быть активным в указанное время. </summary>
        /// <param name="time"> Текущее игровое время для проверки активности света. </param>
        /// <returns> True, если источник света должен быть включен; иначе false. </returns>
        bool IsActive(DateTime time);

        /// <summary> Вычисляет прогресс освещения для указанного времени.
        /// Используется для интерполяции параметров света (цвет, интенсивность, поворот). </summary>
        /// <param name="time"> Текущее игровое время для расчета прогресса. </param>
        /// <returns> Значение от 0.0 до 1.0, представляющее прогресс цикла освещения. </returns>
        float CalculateProgress(DateTime time);

        /// <summary> Создает конфигурацию освещения для указанного источника света и времени.
        /// Конфигурация содержит все необходимые параметры для настройки света. </summary>
        /// <param name="time"> Текущее игровое время. </param>
        /// <param name="light"> Источник света для настройки. </param>
        /// <param name="settings"> Настройки освещения, зависящие от погодных условий. </param>
        /// <returns> Конфигурация света с параметрами цвета, интенсивности, теней и стратегий поведения. </returns>
        LightConfig CreateLightConfig(DateTime time, Light light, LightSettings settings);
    }
}