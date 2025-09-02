using System;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Windows.UI
{
    /// <summary> Базовый класс для всех UI-окон. Управляет открытием, закрытием и событиями. </summary>
    public abstract class BaseWindow : MonoBehaviour
    {
        /// <summary> Открыто ли окно в данный момент. </summary>
        public bool IsOpened { get; private set; }

        /// <summary> Событие: окно было открыто. </summary>
        public event Action Opened;

        /// <summary> Событие: окно было закрыто. </summary>
        public event Action Closed;

        /// <summary> Опциональный гейт, управляющий порядком открытия окон. </summary>
        private IWindowOpenGate _windowOpenGate;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="windowOpenGate"> Опциональный гейт, управляющий порядком открытия окон. </param>
        private void Construct([InjectOptional] IWindowOpenGate windowOpenGate) => _windowOpenGate = windowOpenGate;

        /// <summary> Открывает окно. Учитывает гейт открытия, если установлен. </summary>
        public void Open()
        {
            if (IsOpened)
            {
                transform.SetAsLastSibling();
                return;
            }

            if (_windowOpenGate != null)
            {
                _windowOpenGate.RequestOpen(this, DoOpenImmediate);
                return;
            }

            DoOpenImmediate();
        }

        /// <summary> Закрывает окно. Вызывает событие и виртуальный обработчик. </summary>
        public virtual void Close()
        {
            if (!IsOpened) return;

            gameObject.SetActive(false);
            IsOpened = false;
            Closed?.Invoke();
            OnClosed();
        }

        /// <summary> Мгновенное открытие окна (в обход гейта). </summary>
        private void DoOpenImmediate()
        {
            transform.SetAsLastSibling();
            gameObject.SetActive(true);
            IsOpened = true;
            Opened?.Invoke();
            OnOpened();
        }

        /// <summary> Вызывается при открытии окна. </summary>
        protected virtual void OnOpened() { }

        /// <summary> Вызывается при закрытии окна. </summary>
        protected virtual void OnClosed() { }

        /// <summary> Включает или выключает GameObject окна. Не влияет на IsOpened. </summary>
        public void SetActive(bool value) => gameObject.SetActive(value);
    }
}