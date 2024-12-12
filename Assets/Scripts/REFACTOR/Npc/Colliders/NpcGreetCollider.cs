using UnityEngine;
using UnityEngine.AI;

namespace NPC
{
    /// <summary> Класс, описывающий логику поворота NPC к главному персонажу при встрече.</summary>
    public class NpcGreetCollider : MonoBehaviour //все коллайдеры оптимизировать. Много лишних действий в OnTriggerEnter / Stay
    {
        /// <summary>RigidBody принадлежащий NPC .</summary>
        private Rigidbody _rb;

        /// <summary>Скорость вращения NPC .</summary>
        [SerializeField] private float _rotationSpeed;

        /// <summary>Котролллер NPC.</summary>
        private NpcController _targetNpcController;

        /// <summary> Метод для инициализации .</summary>
        private void Start()
        {
            _targetNpcController = GetComponent<NpcController>();
            _rb = GetComponent<Rigidbody>();
        }

        /// <summary>Метод вызывающийся при нахождении в коллайдере объекта на котором этот скрипт висит .</summary>
        /// <param name="other"> Коллайдер входящего объекта.</param>
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                _targetNpcController.StopMoving();
                Vector3 dir = other.gameObject.transform.position - this.gameObject.transform.position;
                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0, angle, 0);
                _rb.gameObject.transform.rotation = Quaternion.Lerp(_rb.rotation, rotation, _rotationSpeed * Time.deltaTime);
            }
        }
        /// <summary>Метод вызывающийся при выходе из коллайдера объекта на котором этот скрипт висит .</summary>
        /// <param name="other"> Коллайдер входящего объекта.</param>
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                _targetNpcController.StartMoving();
            }
        }
    }
}