using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlavorfulStory.LocationManager
{
    /// <summary> Управляет взаимодействием с объектами в радиусе игрока. </summary>
    public class InteractWithObjects : MonoBehaviour
    {
        /// <summary> Радиус, в пределах которого можно взаимодействовать с объектами. </summary>
        [SerializeField, Range(1f, 10f)] private float _radius;
        
        /// <summary> Клавиша взаимодействия. </summary>
        [SerializeField] private KeyCode _interactKey = KeyCode.E;
        
        /// <summary> Подсказка взаимодействия с использованием мыши. </summary>
        [SerializeField] private GameObject _mouseTip;
        
        /// <summary> Подсказка взаимодействия с использованием клавиатуры. </summary>
        [SerializeField] private GameObject _keyboardTip;
        
        /// <summary> Текущий объект, с которым можно взаимодействовать. </summary>
        private InteractableObject2 _currentTarget;
        
        /// <summary> Указывает, является ли текущий объект целью курсора. </summary>
        private bool _isCursorTarget;

        /// <summary> Массив для хранения результатов столкновений. </summary>
        private RaycastHit[] _hits = new RaycastHit[20];
        
        /// <summary> Обновляет состояние взаимодействия с объектами. </summary>
        private void Update()
        {
            // Отключение подсказок и обводки для предыдущей цели.
            if (_currentTarget)
            {
                _currentTarget.SwitchOutline(false);
                _mouseTip.SetActive(false);
                _keyboardTip.SetActive(false);
            }

            // Поиск нового объекта для взаимодействия.
            _currentTarget = FindTarget();

            // Включение обводки для текущей цели.
            if (_currentTarget) _currentTarget.SwitchOutline(true);
            
            // Отображение подсказок и выполнение взаимодействия.
            if (CanInteract())
            {
                _keyboardTip.SetActive(true);
                if (_isCursorTarget) _mouseTip.SetActive(true);
                
                if (Input.GetMouseButtonDown(0) && _isCursorTarget)
                    _currentTarget.Interact();
                else if (Input.GetKeyDown(_interactKey))
                    _currentTarget.Interact();
            }
        }
        
        /// <summary> Находит текущую цель для взаимодействия. </summary>
        /// <returns> Объект для взаимодействия или null, если цель не найдена. </returns>
        private InteractableObject2 FindTarget()
        {
            InteractableObject2 target = GetCursorTarget();
            _isCursorTarget = true;

            if (!target)
            {
                _isCursorTarget = false;
                var nearbyInteractables = GetNearbyObjects();
                if (nearbyInteractables.Any())
                    target = GetClosestInteractable(nearbyInteractables);
            }

            return target;
        }
        
        /// <summary> Находит цель под курсором. </summary>
        /// <returns> Объект под курсором или null, если цель не найдена. </returns>
        private InteractableObject2 GetCursorTarget()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool isHit = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity,
                LayerMask.GetMask("Interactable"));
            return isHit ? hitInfo.collider.GetComponent<InteractableObject2>() : null;
        }
        
        /// <summary> Находит все объекты в радиусе взаимодействия. </summary>
        /// <returns> Коллекция объектов для взаимодействия. </returns>
        private IEnumerable<InteractableObject2> GetNearbyObjects()
        {
            Physics.SphereCastNonAlloc(
                transform.position, 
                _radius,
                Vector3.one,
                _hits,
                0,
                LayerMask.GetMask("Interactable")
            );
            foreach (var hit in _hits)
            {
                if (hit.collider != null)
                    yield return hit.collider.GetComponent<InteractableObject2>();
            }
        }
        
        /// <summary> Находит ближайший объект из коллекции. </summary>
        /// <param name="interactables"> Коллекция объектов для взаимодействия. </param>
        /// <returns> Ближайший объект или null, если объекты отсутствуют. </returns>
        private InteractableObject2 GetClosestInteractable(IEnumerable<InteractableObject2> interactables)
        {
            InteractableObject2 closest = null;
            float minDistance = float.MaxValue;

            foreach (var interactable in interactables)
            {
                float distance = Vector3.Distance(transform.position, interactable.transform.position);
                if (distance >= minDistance) continue;

                minDistance = distance;
                closest = interactable;
            }

            return closest;
        }
        
        /// <summary> Проверяет возможность взаимодействия с текущей целью. </summary>
        /// <returns> true, если взаимодействие возможно, иначе false. </returns>
        private bool CanInteract() => 
            _currentTarget && Vector3.Distance(transform.position, _currentTarget.transform.position) <= _radius;

        /// <summary> Рисует визуальные подсказки в редакторе сцены. </summary>
        private void OnDrawGizmos()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(player.transform.position, _radius);

            if (_currentTarget)
                Gizmos.DrawLine(player.transform.position, _currentTarget.transform.position);
        }
    }
}
