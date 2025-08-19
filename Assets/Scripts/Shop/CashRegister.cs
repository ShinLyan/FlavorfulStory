using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Actions;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.Audio;
using FlavorfulStory.Economy;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.Player;
using FlavorfulStory.Saving;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using UnityEditor;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace FlavorfulStory.Shop
{
    /// <summary> Касса магазина — хранит валюту, точки доступа, умеет взаимодействовать и сохраняться. </summary>
    [RequireComponent(typeof(CashRegisterAnimator))]
    public class CashRegister : ShopObject, ICurrencyStorage, ISaveable, IInteractable
    {
        /// <summary> Словарь доступности точек доступа к кассе. </summary>
        private Dictionary<Transform, bool> _accessPointsAvailability;

        /// <summary> Сервис транзакций, отвечающий за покупку и продажу предметов. </summary>
        private TransactionService _transactionService;

        /// <summary> Аниматор кассы. </summary>
        private CashRegisterAnimator _cashRegisterAnimator;

        /// <summary> Выполнить внедрение зависимостей. </summary>
        /// <param name="transactionService"> Сервис транзакций магазина. </param>
        [Inject]
        private void Construct(TransactionService transactionService) => _transactionService = transactionService;

        /// <summary> Выполнить инициализацию компонента. </summary>
        private void Awake() => _cashRegisterAnimator = GetComponent<CashRegisterAnimator>();

        /// <summary> Запустить инициализацию состояния кассы. </summary>
        private void Start()
        {
            InitializeAccessPoints();
            OnAmountChanged?.Invoke(Amount);
        }

        /// <summary> Инициализирует словарь доступности точек доступа, устанавливая все точки как свободные. </summary>
        private void InitializeAccessPoints()
        {
            _accessPointsAvailability = new Dictionary<Transform, bool>();
            foreach (var point in _accessiblePositions) _accessPointsAvailability.Add(point, false);
        }

        /// <summary> Возвращает случайную свободную точку доступа и помечает её как занятую. </summary>
        /// <returns> Transform свободной точки доступа или null, если все точки заняты. </returns>
        public override NpcDestinationPoint? GetAccessiblePoint()
        {
            var freePoints = _accessPointsAvailability.Where(pair => !pair.Value).Select(pair => pair.Key).ToList();
            if (freePoints.Count == 0) return null;

            var randomPosition = freePoints[Random.Range(0, freePoints.Count)];
            return new NpcDestinationPoint(randomPosition.position, randomPosition.rotation);
        }

        /// <summary> Освобождает указанную точку доступа, делая её доступной для использования. </summary>
        /// <param name="point"> Transform точки доступа для освобождения. </param>
        /// <param name="isOccupied"> <c>true</c> — если занята; <c>false</c> — если свободна. </param>
        public void SetPointOccupancy(Vector3 point, bool isOccupied)
        {
            var keysToUpdate = _accessPointsAvailability.Where(pair => pair.Key.position == point)
                .Select(pair => pair.Key).ToList();

            foreach (var key in keysToUpdate) _accessPointsAvailability[key] = isOccupied;
        }

        #region ICurrencyStorage

        /// <summary> Текущее количество золота. </summary>
        public int Amount { get; private set; }

        /// <summary> Событие, вызываемое при изменении количества золота. </summary>
        public event Action<int> OnAmountChanged;

        /// <summary> Добавляет указанное количество золота. </summary>
        /// <param name="value"> Количество золота для добавления. </param>
        public void Add(int value)
        {
            Amount += value;
            OnAmountChanged?.Invoke(Amount);
        }

        /// <summary> Пытается потратить указанное количество золота. </summary>
        /// <param name="value"> Сумма, которую нужно потратить. </param>
        /// <returns> <c>true</c>, если золото было успешно потрачено; иначе — <c>false</c>. </returns>
        public bool TrySpend(int value)
        {
            if (Amount < value) return false;

            Amount -= value;
            OnAmountChanged?.Invoke(Amount);
            return true;
        }

        #endregion

        #region IInteractable

        /// <summary> Описание действия с объектом. </summary>
        public ActionTooltipData ActionTooltip => new("E", ActionType.GetMoney, "from Cash Desk");

        /// <summary> Возвращает возможность взаимодействия с объектом. </summary>
        public bool IsInteractionAllowed => true;

        /// <summary> Вычисляет расстояние до указанного трансформа. </summary>
        /// <param name="otherTransform"> Трансформ, до которого вычисляется расстояние. </param>
        /// <returns> Расстояние до объекта. </returns>
        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(transform.position, otherTransform.position);

        /// <summary> Начинает взаимодействие с кассой. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public void BeginInteraction(PlayerController player)
        {
            player.SetBusyState(false);
            if (Amount <= 0) return;

            _transactionService.TransferRegisterToPlayer();
            _cashRegisterAnimator.ToggleCoin(false);
            SfxPlayer.Play(SfxType.CashRegister);
        }

        /// <summary> Завершает взаимодействие с кассой. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public void EndInteraction(PlayerController player) { }

        #endregion

        #region ISaveable

        /// <summary> Структура, представляющая сериализуемое состояние кассы. </summary>
        [Serializable]
        private readonly struct CashRegisterData
        {
            /// <summary> Количество золота. </summary>
            public int Gold { get; }

            /// <summary> Конструктор с параметрами. </summary>
            /// <param name="gold"> Количество золота. </param>
            public CashRegisterData(int gold) => Gold = gold;
        }

        /// <summary> Сохраняет текущее состояние кассы. </summary>
        /// <returns> Объект состояния для сериализации. </returns>
        public object CaptureState() => new CashRegisterData(Amount);

        /// <summary> Восстанавливает состояние кассы из сериализованных данных. </summary>
        /// <param name="state"> Объект состояния, полученный при сохранении. </param>
        public void RestoreState(object state)
        {
            if (state is not CashRegisterData data) return;

            Amount = data.Gold;
        }

        #endregion

#if UNITY_EDITOR

        /// <summary> Отрисовывает гизмо кассы с детализированной информацией о состоянии каждой точки доступа. </summary>
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (_accessiblePositions == null) return;

            Color occupiedPointColor = new(1f, 0.3f, 0.3f, 0.8f);
            Color freePointColor = new(0.3f, 1f, 0.3f, 0.8f);
            const float PointLabelHeight = 0.5f;
            const int LabelFontSize = 11;

            var labelStyle = new GUIStyle
            {
                fontSize = LabelFontSize,
                normal = new GUIStyleState { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };

            foreach (var point in _accessiblePositions)
            {
                if (!point) continue;

                bool isOccupied = _accessPointsAvailability != null &&
                                  _accessPointsAvailability.ContainsKey(point) &&
                                  _accessPointsAvailability[point];

                Gizmos.color = isOccupied ? occupiedPointColor : freePointColor;
                Gizmos.DrawLine(transform.position, point.position);

                string statusText = isOccupied ? "<color=#ff3333>Occupied</color>" : "<color=#33ff33>Free</color>";
                var labelPosition = point.position + Vector3.up * PointLabelHeight;

                Handles.Label(labelPosition, statusText, labelStyle);
            }
        }

#endif
    }
}