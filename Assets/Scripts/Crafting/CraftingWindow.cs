using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.TimeManagement;
using FlavorfulStory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FlavorfulStory.Crafting
{
    /// <summary> Окно крафта, отображающее рецепты, требования и управление процессом. </summary>
    public class CraftingWindow : MonoBehaviour
    {
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

        /// <summary> Проверяет ввод и закрывает окно по кнопке меню. </summary>
        private void Update()
        {
            if (!_isOpen || !InputWrapper.GetButtonDown(InputButton.SwitchGameMenu)) return;

            StartCoroutine(BlockGameMenuForOneFrame());
            Close();
            InputWrapper.BlockInput(InputButton.SwitchGameMenu);
        }

        //TODO: Завести DTO/Контекст под 3 экшена?
        /// <summary> Настраивает окно крафта с рецептами и делегатами. </summary>
        /// <param name="recipeData"> Список рецептов. </param>
        /// <param name="onCraftRequested"> Делегат на запуск крафта. </param>
        /// <param name="onCloseRequested"> Делегат на закрытие окна. </param>
        public void Setup(IEnumerable<CraftingRecipe> recipeData, Action<CraftingRecipe, int> onCraftRequested,
            Action onCloseRequested)
        {
            Initialize();
            _recipeData = recipeData;
            _onCraftRequested = onCraftRequested;
            _onCloseRequested = onCloseRequested;

            SetupViews(_recipeData.ToList());
            UpdateRecipeViews();

            _craftButton.onClick.RemoveAllListeners();
            _craftButton.onClick.AddListener(TryStartCrafting);
        }

        /// <summary> Обновляет список отображаемых рецептов с учетом фильтра и состояния. </summary>
        private void UpdateRecipeViews()
        {
            string recipeNamePrefix = _recipeNameInput.text.Trim();

            var query = _recipeData
                .Where(recipe => _showLockedRecipes || !recipe.IsLocked);

            if (!string.IsNullOrEmpty(recipeNamePrefix))
                query = query.Where(r => r.RecipeName.StartsWith(recipeNamePrefix, StringComparison.OrdinalIgnoreCase));

            var filtered = query
                .OrderBy(recipe => recipe.IsLocked)
                .ThenBy(recipe => recipe.RecipeName)
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
                recipeView.Setup(recipe.IsLocked, recipe.Sprite, () => ChooseRecipe(recipe));
            }
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

                string reqText = hasEnough ? $"{available}" : $"{available}/{required}";
                view.Setup(input.Item.Icon, input.Item.ItemName, reqText, hasEnough);
            }
        }

        /// <summary> Обновляет отображение текущего рецепта и его состояния. </summary>
        private void UpdateCraftInfo()
        {
            _currentRecipeImage.sprite = _currentRecipe.Sprite;
            _recipeName.text = _currentRecipe.RecipeName;
            SetupRequirements();
            _craftButton.interactable =
                CraftingProcessor.CanCraft(_currentRecipe, _craftCount, _playerInventory) == CraftingResult.Success;
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
                int required = input.Quantity * _craftCount;
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
        }

        /// <summary> Блокирует кнопку меню на один кадр. </summary>
        /// <returns> Корутина для задержки. </returns>
        private static IEnumerator BlockGameMenuForOneFrame()
        {
            InputWrapper.BlockInput(InputButton.SwitchGameMenu);
            yield return null;
            InputWrapper.UnblockAllInput();
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
    }
}