using System;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние ожидания NPC, в котором персонаж не выполняет активных действий. </summary>
    public class WaitingState : CharacterState
    {
        /// <summary> Инициализирует новое состояние ожидания. </summary>
        /// <param name="stateController"> Контроллер состояний. </param>
        public WaitingState(Func<StateController> stateController) : base(stateController)
        {
        }

        /// <summary> Вызывается при входе в состояние. 
        /// Используется для выполнения начальных действий при переходе в это состояние. </summary>
        public override void Enter()
        {
        }

        /// <summary> Вызывается при выходе из состояния.
        /// Используется для очистки или завершения действий перед выходом из состояния. </summary>
        public override void Exit()
        {
        }

        /// <summary> Вызывается в каждом кадре во время нахождения в этом состоянии.
        /// Используется для обновления логики взаимодействия. </summary>
        /// <param name="deltaTime"> Время, прошедшее с предыдущего кадра. </param>
        public override void Update(float deltaTime)
        {
        }

        public override void Reset()
        {
        }
    }
}