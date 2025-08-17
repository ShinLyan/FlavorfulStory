using System;
using UnityEngine;

namespace FlavorfulStory
{
    public abstract class BaseWindow : MonoBehaviour
    {
        public bool IsOpened { get; private set; }
        
        public event Action Opened;
        public event Action Closed;

        public void Open()
        {
            if (IsOpened) return;

            transform.SetAsLastSibling();
            gameObject.SetActive(true);
            IsOpened = true;
            OnOpened();
            Opened?.Invoke();
        }

        public virtual void Close()
        {
            if (!IsOpened) return;

            gameObject.SetActive(false);
            IsOpened = false;
            OnClosed();
            Closed?.Invoke();
        }

        protected virtual void OnOpened() { }
        protected virtual void OnClosed() { }

        public void SetActive(bool value) => gameObject.SetActive(value);
    }
}