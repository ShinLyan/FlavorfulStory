using System;
using UnityEngine;
using Zenject;

namespace FlavorfulStory
{
    public abstract class BaseWindow : MonoBehaviour
    {
        public bool IsOpened { get; private set; }
        
        public event Action Opened;
        public event Action Closed;

        // Инжектим необязательно: на ранних сценах можно жить без гейта.
        [InjectOptional] private IWindowOpenGate _openGate;
        
        public void Open()
        {
            if (IsOpened)
            {
                transform.SetAsLastSibling();
                return;
            }

            if (_openGate != null)
            {
                _openGate.RequestOpen(this, DoOpenImmediate);
                return;
            }

            DoOpenImmediate();
        }

        public virtual void Close()
        {
            if (!IsOpened) return;

            gameObject.SetActive(false);
            IsOpened = false;
            Closed?.Invoke();
            OnClosed();
            
        }

        private void DoOpenImmediate()
        {
            transform.SetAsLastSibling();
            gameObject.SetActive(true);
            IsOpened = true;
            Opened?.Invoke();
            OnOpened();
        }
        
        protected virtual void OnOpened() { }
        protected virtual void OnClosed() { }

        public void SetActive(bool value) => gameObject.SetActive(value);
    }
}