using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.TimeManagement;
using FlavorfulStory.InputSystem;

namespace FlavorfulStory.Crafting
{
    /// <summary> Окно крафта, отображающее рецепты, требования и управление процессом. </summary>
    public class CraftingWindow : MonoBehaviour
    {
        //TODO: комм
        [Inject] private ICraftingRecipeProvider _recipeProvider;
        
        /// <summary> Поле для ввода имени рецепта (фильтр). </summary>
        [Header("Recipe Previews")]
        [SerializeField] private TMP_InputField _recipeNameInput;

        /// <summary> Переключатель отображения заблокированных рецептов. </summary>
        [SerializeField] private Toggle _lockedRecipesToggle;

        /// <summary> Текст, отображающийся при отсутствии найденных рецептов. </summary>
        [SerializeField] private TMP_Text _noRecipesFoundText;

        /// <summary> Изображение текущего выбранного рецепта. </summary>
        [Header("Recipe Info")]
        [SerializeField] private Image _currentRecipeImage;

        /// <summary> Текст с названием текущего рецепта. </summary>
        [SerializeField] private TMP_Text _recipeName;

        /// <summary> Кнопка увеличения количества создаваемых наборов. </summary>
        [Header("Right Footer")]
        [SerializeField] private Button _increaseCraftCountButton;

        /// <summary> Кнопка уменьшения количества создаваемых наборов. </summary>
        [SerializeField] private Button _decreaseCraftCountButton;

        /// <summary> Текст с текущим количеством создаваемых наборов. </summary>
        [SerializeField] private TMP_Text _craftCountText;

        /// <summary> Кнопка запуска крафта. </summary>
        [SerializeField] private Button _craftButton;

        /// <summary> Список вьюх требований к крафту. </summary>
        private List<CraftRequirementView> _requirementViews;

        /// <summary> Список вьюх рецептов. </summary>
        private List<CraftRecipeView> _recipeViews;

        /// <summary> Данные доступных рецептов. </summary>
        private IEnumerable<CraftingRecipe> _recipeData;

        /// <summary> Текущий выбранный рецепт. </summary>
        private CraftingRecipe _currentRecipe;

        /// <summary> Станция крафта, связанная с этим окном. </summary>
        private CraftingStation _craftingStation;

        /// <summary> Делегат, вызываемый при закрытии окна. </summary>
        private Action _onCloseRequested;

        /// <summary> Инвентарь игрока. </summary>
        private Inventory _playerInventory;

        /// <summary> Было ли окно уже инициализировано. </summary>
        private bool _initialized;
        
        /// <summary> Находится ли выбранная станция в процессе крафта. </summary>
        private bool _isCraftingInProgress;

        /// <summary> Делегат, вызываемый при начале крафта. </summary>
        private Action<CraftingRecipe, int> _onCraftRequested;

        /// <summary> Отображается ли окно сейчас. </summary>
        private bool _isOpen;

        /// <summary> Показывать ли заблокированные рецепты. </summary>
        private bool _showLockedRecipes;

        /// <summary> Текущее выбранное количество создаваемых наборов. </summary>
        private int _craftCount = 1;

        /// <summary> Внедряет инвентарь игрока через Zenject. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        [Inject]
        private void Construct(Inventory inventory) => _playerInventory = inventory;

        /// <summary> Проверить ввод пользователя и закрыть окно при необходимости. </summary>
        private void Update()
        {
            if (!_isOpen || !InputWrapper.GetButtonDown(InputButton.SwitchGameMenu)) return;

            Close();
            BlockGameMenuForOneFrame().Forget();
        }

        /// <summary> Заблокировать кнопку игрового меню на один кадр. </summary>
        private static async UniTaskVoid BlockGameMenuForOneFrame() // TODO: УДАЛИТЬ КОСТЫЛЬ НА WINDOW FABRIC
        {
            InputWrapper.BlockInput(InputButton.SwitchGameMenu);
            await UniTask.Yield();
            InputWrapper.UnblockAllInput();
        }

        //TODO: Завести DTO/Контекст под 3 экшена?
        /// <summary> Настраивает окно крафта с рецептами и делегатами. </summary>
        /// <param name="onCraftRequested"> Делегат на запуск крафта. </param>
        /// <param name="onCloseRequested"> Делегат на закрытие окна. </param>
        /// <param name="station"> Крафт-станция, для которой вызываем окно. </param>
        public void Setup(
            Action<CraftingRecipe, int> onCraftRequested,
            Action onCloseRequested,
            CraftingStation station)
        {
            Initialize();
            _recipeData = _recipeProvider.All;
            _onCraftRequested = onCraftRequested;
            _onCloseRequested = onCloseRequested;

            _craftingStation = station;
            _isCraftingInProgress = false;
            
            SetupViews(_recipeData.ToList());
            UpdateRecipeViews();
            
            _craftButton.onClick.RemoveAllListeners();
            _craftButton.onClick.AddListener(TryStartCrafting);
        }

        /// <summary> Обновляет список отображаемых рецептов с учетом фильтра и состояния. </summary>
        private void UpdateRecipeViews()
        {
            string recipeNamePrefix = _recipeNameInput.text.Trim();

            var query = 
                _recipeData.Where(r => _showLockedRecipes || _recipeProvider.IsUnlocked(r.RecipeID));

            if (!string.IsNullOrEmpty(recipeNamePrefix))
                query = query.Where(recipe => recipe.RecipeName.StartsWith
                (
                    recipeNamePrefix, StringComparison.OrdinalIgnoreCase
                ));

            var filtered = query
                .OrderBy(r => !_recipeProvider.IsUnlocked(r.RecipeID)) // открытые первыми
                .ThenBy(r => r.RecipeName)
                .ToList();

            SetupViews(filtered);
            _noRecipesFoundText.gameObject.SetActive(filtered.Count == 0);
        }

        /// <summary> Обновляет отображение текущего рецепта после завершения крафта. </summary>
        public void OnCraftCompleted() => UpdateCraftInfo();

        /// <summary> Инициализирует элементы окна, если это ещё не было сделано. </summary>
        private void Initialize()
        {
            if (_initialized) return;

            _recipeViews = GetComponentsInChildren<CraftRecipeView>(true).ToList();
            _requirementViews = GetComponentsInChildren<CraftRequirementView>(true).ToList();
            UpdateCraftCountText();
            _increaseCraftCountButton.onClick.AddListener(IncreaseCraftCount);
            _decreaseCraftCountButton.onClick.AddListener(DecreaseCraftCount);
            _noRecipesFoundText.gameObject.SetActive(false);
            _lockedRecipesToggle.onValueChanged.AddListener(ToggleLockedRecipes);
            _isOpen = false;
            _recipeProvider.RecipeUnlocked += (_) => UpdateRecipeViews();

            _initialized = true;
        }

        /// <summary> Настраивает представления рецептов. </summary>
        /// <param name="recipeData"> Список рецептов. </param>
        private void SetupViews(List<CraftingRecipe> recipeData)
        {
            for (int i = 0; i < _recipeViews.Count; i++)
            {
                var recipeView = _recipeViews[i];
                if (i < recipeData.Count())
                    recipeView.Enable();
                else
                    recipeView.Disable();

                if (i >= recipeData.Count()) continue;

                var recipe = recipeData.ElementAt(i);
                recipeView.Setup(recipe.RecipeID, () => ChooseRecipe(recipe));
            }
            
            if (recipeData.Count > 0) ChooseRecipe(recipeData.First());
        }

        /// <summary> Настраивает отображение требований к текущему рецепту. </summary>
        private void SetupRequirements()
        {
            var states = CalculateItemStates();

            for (int i = 0; i < _requirementViews.Count; i++)
            {
                var view = _requirementViews[i];
                view.gameObject.SetActive(i < _currentRecipe.InputItems.Count);

                if (i >= _currentRecipe.InputItems.Count) continue;

                var input = _currentRecipe.InputItems[i];
                (int available, int required) = states[input.Item];
                bool hasEnough = available >= required;

                string reqText = $"{available}/{required}";
                view.Setup(input.Item.ItemID, reqText, hasEnough);
            }
        }

        /// <summary> Обновляет отображение текущего рецепта и его состояния. </summary>
        private void UpdateCraftInfo()
        {
            if (_currentRecipe == null) return;

            _currentRecipeImage.sprite = _currentRecipe.Sprite;
            _recipeName.text = _currentRecipe.RecipeName;
            SetupRequirements();

            bool canCraft = 
                CraftingProcessor.CanCraft(_currentRecipe, _craftCount, _playerInventory) == CraftingResult.Success;
            
            bool stationBusy = _craftingStation != null && !_craftingStation.IsInteractionAllowed;
            _craftButton.interactable = canCraft && !stationBusy;
        }

        /// <summary> Устанавливает состояние процесса крафта. </summary>
        /// <param name="inProgress"> Идет ли процесс крафта у выбранной станции. </param>
        public void SetCraftingInProgress(bool inProgress)
        {
            _isCraftingInProgress = inProgress;
            UpdateCraftInfo();
        }        
        
        /// <summary> Обновляет текст количества создаваемых предметов. </summary>
        private void UpdateCraftCountText() => _craftCountText.text = _craftCount.ToString();

        /// <summary> Пытается запустить крафт текущего рецепта. </summary>
        private void TryStartCrafting()
        {
            if (_currentRecipe == null) return;

            _craftButton.interactable = false;
            _onCraftRequested?.Invoke(_currentRecipe, _craftCount);
        }

        /// <summary> Выбирает рецепт и обновляет отображение. </summary>
        /// <param name="recipe"> Выбранный рецепт. </param>
        private void ChooseRecipe(CraftingRecipe recipe)
        {
            _currentRecipe = recipe;
            UpdateCraftInfo();
        }

        /// <summary> Вычисляет доступное и требуемое количество для каждого ресурса. </summary>
        /// <returns> Словарь ресурсов с информацией. </returns>
        private Dictionary<InventoryItem, (int available, int required)> CalculateItemStates()
        {
            var result = new Dictionary<InventoryItem, (int available, int required)>();

            foreach (var input in _currentRecipe.InputItems)
            {
                int required = input.Number * _craftCount;
                int available = _playerInventory.GetItemNumber(input.Item);
                result[input.Item] = (available, required);
            }

            return result;
        }

        /// <summary> Переключает отображение заблокированных рецептов. </summary>
        /// <param name="showLockedRecipes"> Нужно ли показывать заблокированные рецепты. </param>
        private void ToggleLockedRecipes(bool showLockedRecipes)
        {
            _showLockedRecipes = showLockedRecipes;
            UpdateRecipeViews();
        }

        /// <summary> Открывает окно крафта. </summary>
        public void Open()
        {
            Initialize();
            
            _isOpen = true;
            gameObject.SetActive(true);
            WorldTime.Pause();
            InputWrapper.BlockAllInput();
            InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
            UpdateCraftCountText();
        }

        /// <summary> Закрывает окно и снимает блокировку управления. </summary>
        private void Close()
        {
            _isOpen = false;
            gameObject.SetActive(false);
            WorldTime.Unpause();
            InputWrapper.UnblockAllInput();
            _onCloseRequested?.Invoke();
            
            _craftingStation = null;
        }

        /// <summary> Увеличивает количество создаваемых наборов. </summary>
        private void IncreaseCraftCount()
        {
            _craftCount++;
            UpdateCraftCountText();
            UpdateCraftInfo();
        }

        /// <summary> Уменьшает количество создаваемых наборов. </summary>
        private void DecreaseCraftCount()
        {
            _craftCount = Mathf.Max(1, _craftCount - 1);
            UpdateCraftCountText();
            UpdateCraftInfo();
        }
        
        public void Refresh() => UpdateCraftInfo();
    }
}