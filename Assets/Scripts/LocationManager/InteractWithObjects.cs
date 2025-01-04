using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlavorfulStory.LocationManager
{
    /// <summary> Взаимодействие с объектами.</summary>
    public class InteractWithObjects : MonoBehaviour
    {
        /// <summary> Радиус взаимодействия</summary>
        [SerializeField, Range(1f, 10f)] private float _radius;
        
        /// <summary> Кнопка взаимодействия.</summary>
        [SerializeField] private KeyCode _interactKey = KeyCode.E;
        
        /// <summary> Подсказка для взаимодействия мышью.</summary>
        [SerializeField] private GameObject _mouseTip;
        
        /// <summary> Подсказка взаимодействия через клавиатуру.</summary>
        [SerializeField] private GameObject _keyboardTip;
        
        /// <summary> Текущая цель.</summary>
        private InteractableObject2 _currentTarget;
        
        /// <summary> Цель принадлежит курсору.</summary>
        private bool _isCursorTarget;

        private void Update()
        {
            if (_currentTarget)
            {
                _currentTarget.SwitchOutline(false);
                _mouseTip.SetActive(false);
                _keyboardTip.SetActive(false);
            }

            _currentTarget = FindTarget();

            if (_currentTarget) _currentTarget.SwitchOutline(true);
            
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
        
        /// <summary> Поиск цели.</summary>
        private InteractableObject2 FindTarget()
        {
            InteractableObject2 target = GetCursorTarget();
            _isCursorTarget = true;
            if (!target)
            {
                _isCursorTarget = false;
                var nearbyInteractables = GetNearbyObjects();
                if (nearbyInteractables.Count() > 0)
                    target = GetClosestInteractable(nearbyInteractables);
            }
            return target;
        }
        
        /// <summary> Получение цели через курсор.</summary>
        private InteractableObject2 GetCursorTarget()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool isHit = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity,
                LayerMask.GetMask("Interactable"));
            return isHit ? hitInfo.collider.GetComponent<InteractableObject2>() : null;
        }
        
        /// <summary> Получение объектов в радуисе взаимодействия.</summary>
        private IEnumerable<InteractableObject2> GetNearbyObjects()
        {
            RaycastHit[] hits = Physics.SphereCastAll(
                transform.position, 
                _radius,
                Vector3.one,
                0,
                LayerMask.GetMask("Interactable")
            );
            return hits.Select(hit => hit.collider.GetComponent<InteractableObject2>());
        }
        
        /// <summary> Получение ближайшего объекта.</summary>
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
        
        /// <summary> Можно взаимодействовать.</summary>
        private bool CanInteract() => 
            _currentTarget && Vector3.Distance(transform.position, _currentTarget.transform.position) <= _radius;
        

        /// <summary> Рисование в окне сцены.</summary>
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