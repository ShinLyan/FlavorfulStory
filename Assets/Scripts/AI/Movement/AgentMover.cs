using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.Movement
{
    public class AgentMover : MonoBehaviour
    {
        // Компонент агента
        private NavMeshAgent _agent;

        // Массив положений точек назначения
        [SerializeField] private Transform[] _goals;

        // Расстояние на которое необходимо приблизиться к точке
        [SerializeField] private float _distanceToChangeGoal;

        // Номер текущей целевой точки
        private int _currentGoal = 0;

        void Start()
        {
            // Сохранение компонента агента и направление к первой точке
            _agent = GetComponent<NavMeshAgent>();
            _agent.destination = _goals[0].position;
        }

        void Update()
        {
            // Проверка на то, достаточно ли близок агент к цели
            if (_agent.remainingDistance < _distanceToChangeGoal)
            {
                // Смена точки на следующую
                _currentGoal++;
                if (_currentGoal == _goals.Length) _currentGoal = 0;
                _agent.destination = _goals[_currentGoal].position;
            }
        }
    }
}
