namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние взаимодействия, наследующий базовый класс <see cref="CharacterState"/>.
    /// Используется для реализации логики, связанной с взаимодействиями в конечном автомате (FSM). </summary>
    public class InteractionState : CharacterState
    {
        /// <summary> Вызывается при входе в состояние. </summary>
        public override void Enter() { }

        /// <summary> Вызывается при выходе из состояния. </summary>
        public override void Exit() { }

        /// <summary> Обновление логики состояния, вызываемое каждый кадр. </summary>
        /// <param name="deltaTime"> Время, прошедшее с последнего кадра. </param>
        public override void Update(float deltaTime) { }

        /// <summary> Сброс состояния в начальное состояние. </summary>
        public override void Reset() { }
    }
}