namespace FlavorfulStory.TimeManagement
{
    /// <summary> Сигнал, отправляемый при наступлении ночи. </summary>
    public class NightStartedSignal
    {
        /// <summary> Внутриигровое время, когда наступила ночь. </summary>
        public DateTime Time {get; private set;}

        /// <summary> Создаёт новый сигнал с указанием времени. </summary>
        /// <param name="time"> Текущее внутриигровое время. </param>
        public NightStartedSignal(DateTime time) => Time = time;
    }
}