using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Crafting;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.DropSystem;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.Player;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using UnityEngine;
using Zenject;

namespace FlavorfulStory
{
    /// <summary> Процессор ресурсов: принимает вход и периодически дропает готовый аутпут в мир. </summary>
    public class ResourceProcessor : MonoBehaviour, IInteractable
    {
        [SerializeField] private CraftingRecipe _recipe;

        // === События для внешних вьюх ===
        public event Action<int,int> OnQueueChanged;          // (itemsLeft, capacity)
        public event Action<bool>   OnProcessingStateChanged; // true/false
        public event Action         OnCycleStarted;           // начало одного цикла
        public event Action         OnCycleCompleted;         // завершение одного цикла
        private Action _startCycleHandler;

        /// <summary> Доступ к рецепту извне (например, для Setup вьюхи). </summary>
        public CraftingRecipe Recipe => _recipe;

        private PlayerController _playerController;
        private Inventory _playerInventory;
        private Toolbar _toolbar;
        private IItemDropService _itemDropService;

        private int _itemsLeft;                // сколько единиц входа ещё переработать
        private bool _isProcessing;            // крутится ли сейчас цикл
        private CancellationTokenSource _cts;  // отмена цикла

        // Параметры дропа
        private const float DropForce = 5f;
        private const float DropOffsetRange = 1.25f;

        // Необязательная вьюха (World Space Canvas)
        [SerializeField] private ResourceProcessorUIView _view;

        [Inject]
        private void Construct(Toolbar toolbar, IItemDropService itemDropService)
        {
            _toolbar = toolbar;
            _itemDropService = itemDropService;
        }

        private void Awake()
        {
            _playerController = FindAnyObjectByType<PlayerController>();
            _playerInventory  = _playerController ? _playerController.GetComponent<Inventory>() : null;
            _itemsLeft = 0;
            _cts = new CancellationTokenSource();

            if (_view && HasValidRecipe)
            {
                _view.AttachTo(transform);
                _view.SetOffset(new Vector3(0f, 1.8f, 0f)); // подгони высоту как нравится
                var input  = _recipe.InputItems[0];
                var output = _recipe.OutputItems[0];
                _view.Setup(input.Item.Icon, output.Item.Icon, output.Number);
                _view.SetQueue(_itemsLeft, input.Item.StackSize);
            }

            // Сообщим стартовое состояние подписчикам
            if (HasValidRecipe)
                OnQueueChanged?.Invoke(_itemsLeft, _recipe.InputItems[0].Item.StackSize);
            
            _view.SetVisible(false); // NEW: по умолчанию скрыта
        }

        private void OnEnable()
        {
            if (_view)
            {
                OnQueueChanged += _view.SetQueue;
                OnProcessingStateChanged += _view.SetProcessingState;
                _startCycleHandler = () => _view.StartCycle(Recipe.Duration);
                OnCycleStarted += _startCycleHandler;
                OnCycleCompleted += _view.CompleteCycle;
            }
        }

        private void OnDisable()
        {
            if (_view)
            {
                OnQueueChanged -= _view.SetQueue;
                OnProcessingStateChanged -= _view.SetProcessingState;
                if (_startCycleHandler != null) OnCycleStarted -= _startCycleHandler;
                OnCycleCompleted -= _view.CompleteCycle;
            }
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        // ————— Helpers —————
        private bool HasValidRecipe =>
            _recipe && _recipe.IsProcessorRecipe &&
            _recipe.InputItems != null && _recipe.InputItems.Count > 0 &&
            _recipe.OutputItems != null && _recipe.OutputItems.Count > 0;

        private InventoryItem InputItem => HasValidRecipe ? _recipe.InputItems[0].Item : null;

        /// <summary> Совпадает ли выбранный в тулбаре предмет с входом рецепта. </summary>
        private bool CanDepositSelected =>
            HasValidRecipe &&
            _toolbar != null &&
            _toolbar.SelectedItem != null &&
            ReferenceEquals(_toolbar.SelectedItem, InputItem);

        // ————— IInteractable —————
        public ActionTooltipData ActionTooltip =>
            CanDepositSelected
                ? new ActionTooltipData("E", Actions.ActionType.Craft, $"Add {InputItem.ItemName} to Processor")
                : new ActionTooltipData("E", Actions.ActionType.Craft, "Open Recipe Book");

        // Разрешаем взаимодействие всегда при наличии рецепта — даже если предмет не совпал (тогда покажем “книгу”)
        public bool IsInteractionAllowed => HasValidRecipe;

        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(otherTransform.position, transform.position);

        public void EndInteraction(PlayerController player) { }

        public void OnInteractionTriggerEnter()
        {
            if (!_view) return;
            _view.SetVisible(true);
            // обновим актуальные значения, пока вьюха была скрыта
            if (HasValidRecipe)
            {
                _view.SetQueue(_itemsLeft, _recipe.InputItems[0].Item.StackSize);
                _view.SetProcessingState(_isProcessing);
            }
            
        }

        public void OnInteractionTriggerExit()
        {
            if (_view) _view.SetVisible(false);
        }

        public void BeginInteraction(PlayerController player)
        {
            if (!HasValidRecipe || _playerInventory == null) return;

            if (!CanDepositSelected)
            {
                //TODO: Показывать существующее окно крафта, но без кнопок создания(добавить категории?)
                Debug.Log("[Processor] Open Recipe Book (TODO: окно рецептов).");
                return;
            }

            var inputItem = InputItem;
            if (!inputItem)
            {
                Debug.LogError("[Processor] Input item is null in recipe.");
                return;
            }

            // есть ли предмет у игрока
            if (_playerInventory.GetItemNumber(inputItem) <= 0)
            {
                Debug.LogWarning("[Processor] У игрока нет нужного предмета.");
                return;
            }

            // не переполняем буфер (лимит = StackSize входного предмета)
            int stackSize = inputItem.StackSize;
            if (_itemsLeft >= stackSize)
            {
                Debug.Log("[Processor] Бафер входа полон (StackSize).");
                return;
            }

            // забираем 1 ед. и кладём в буфер станка
            _playerInventory.RemoveItem(inputItem, 1);
            _itemsLeft = Mathf.Min(stackSize, _itemsLeft + 1);

            OnQueueChanged?.Invoke(_itemsLeft, stackSize);

            // анимация
            player.TriggerAnimation(AnimationType.Gather);

            // запуск/продолжение процесса
            UpdateProcessing();
        }

        /// <summary>Запускает цикл переработки, если он ещё не идёт.</summary>
        private void UpdateProcessing()
        {
            if (_isProcessing || _itemsLeft <= 0 || !HasValidRecipe) return;
            ProcessLoopAsync(_cts.Token).Forget();
        }

        /// <summary>Основной цикл: по 1 входу → задержка Duration → дроп аутпута в мир.</summary>
        private async UniTaskVoid ProcessLoopAsync(CancellationToken ct)
        {
            _isProcessing = true;
            OnProcessingStateChanged?.Invoke(true);

            try
            {
                var output = _recipe.OutputItems[0];   // 1 цикл → этот ItemStack
                var duration = Mathf.Max(0f, _recipe.Duration);

                while (_itemsLeft > 0 && !ct.IsCancellationRequested)
                {
                    OnCycleStarted?.Invoke();

                    if (duration > 0f)
                        await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: ct);

                    // дропаём результат в мир
                    var dropPos = GetDropPosition();
                    var force = Vector3.up * DropForce;
                    _itemDropService.Drop(output, dropPos, force);

                    _itemsLeft--;
                    OnQueueChanged?.Invoke(_itemsLeft, _recipe.InputItems[0].Item.StackSize);

                    OnCycleCompleted?.Invoke();
                }
            }
            catch (OperationCanceledException) { /* нормальное завершение */ }
            finally
            {
                _isProcessing = false;
                OnProcessingStateChanged?.Invoke(false);

                // если пока крутили нам подложили ещё — перезапустимся
                if (_itemsLeft > 0 && !ct.IsCancellationRequested)
                    UpdateProcessing();
            }
        }

        private Vector3 GetDropPosition()
        {
            float ox = UnityEngine.Random.Range(-DropOffsetRange, DropOffsetRange);
            float oz = UnityEngine.Random.Range(-DropOffsetRange, DropOffsetRange);
            return transform.position + new Vector3(ox, 1f, oz);
        }
    }
}
