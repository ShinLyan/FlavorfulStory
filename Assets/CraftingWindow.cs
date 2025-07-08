using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.InventorySystem;
using UnityEngine;
using FlavorfulStory.UI;
using TMPro;
using UnityEngine.UI;
using Zenject;

namespace FlavorfulStory
{
    public class CraftingWindow : MonoBehaviour
    {
        [SerializeField] private Image _currentRecipeImage;
        [SerializeField] private TMP_Text _recipeName;
        private List<CraftRequirementView> _requirementViews;
        
        [SerializeField] private Button _craftButton;
        
        [SerializeField] private TMP_InputField _recipeNameInput;
        
        private List<CraftRecipeView> _recipeViews;
        private IEnumerable<CraftingRecipe> _recipeData;

        private CraftingRecipe _currentRecipe;
        
        [SerializeField] private Button _increaseCraftCountButton;
        [SerializeField] private Button _decreaseCraftCountButton;
        [SerializeField] private TMP_Text _craftCountText;
        private int _craftCount;
        
        [SerializeField] private TMP_Text _noRecipesFoundText;
        
        [SerializeField] private Toggle _lockedRecipesToggle;
        private bool _showLockedRecipes;
        
        //TODO:: После сундуков => добавить учитывание ресои из сундуков при крафте
        private Inventory _playerInventory;
        
        /// <summary> Внедрение зависимости — инвентарь игрока. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        [Inject]
        private void Construct(Inventory inventory) => _playerInventory = inventory;
        
        private void Awake() => Initialize();

        private void Initialize()
        {
            _recipeViews = GetComponentsInChildren<CraftRecipeView>().ToList();
            _requirementViews = GetComponentsInChildren<CraftRequirementView>().ToList();
            _craftCount = 1;
            UpdateCraftCountText();
            _increaseCraftCountButton.onClick.AddListener(IncreaseCraftCount);
            _decreaseCraftCountButton.onClick.AddListener(DecreaseCraftCount);
            _noRecipesFoundText.gameObject.SetActive(false);
            _lockedRecipesToggle.onValueChanged.AddListener(ToggleLockedRecipes);
        }

        public void Setup(IEnumerable<CraftingRecipe> recipeData)
        {
            _recipeData = recipeData;
            SetupViews(_recipeData.ToList());
            UpdateRecipeViews();
        }

        private void UpdateView()
        {
            // Учесть ShowUnknown
            // Учесть RecipeNamePrefix
            // Отсортировать вьюшки => показать вьюшки
            // Сбросить превьюшку и пункты справа
        }

        private void ChooseRecipe(CraftingRecipe recipe)
        {
            _currentRecipe = recipe;
            UpdateCraftInfo();
        }

        //TODO::Дописать внутри
        private bool CanItemBeCrafted()
        {
            // Добавить проверки:
            // 1) Ресурсные требования удовлетворены
            // 2) В инвентаре есть место для размещения предмета (с учетом сожранных в крафте ресурсов)
            // 3) На верстаке не запущен процесс крафта
            return true;
        }
        
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
                recipeView.Setup(recipe.Locked, recipe.Sprite, () => ChooseRecipe(recipe));
            }
        }

        private void SetupRequirements()
        {
            for (int i = 0; i < _requirementViews.Count; i++)
            {
                var requirementView = _requirementViews[i];
                requirementView.gameObject.SetActive(i < _currentRecipe.InputItems.Count);
                
                if (i >= _currentRecipe.InputItems.Count) continue;
                
                var requirement = _currentRecipe.InputItems.ElementAt(i);
                var requirementText = CreateRequirementText(_playerInventory.GetItemNumber(requirement.Item),
                                                            _currentRecipe.InputItems.ElementAt(i).Quantity * _craftCount);
                bool requirementAchieved = _playerInventory.GetItemNumber(requirement.Item) >= requirement.Quantity * _craftCount;
                requirementView.Setup(requirement.Item.Icon, requirement.Item.ItemName,
                                      requirementText, requirementAchieved);
            }
        }

        private static string CreateRequirementText(int inventoryQuantity, int requiredQuantity)
        {
            return inventoryQuantity >= requiredQuantity ? 
                $"{inventoryQuantity}" :
                $"{inventoryQuantity}/{requiredQuantity}";
        }

        private void IncreaseCraftCount()
        {
            _craftCount++;// TODO: Добавить ограничение на макс. кол-во предметов и тут прокинуть?
            UpdateCraftCountText();
            UpdateCraftInfo();
        }
        private void DecreaseCraftCount()
        {
            _craftCount = Mathf.Max(1, _craftCount - 1);
            UpdateCraftCountText();
            UpdateCraftInfo();
        }

        private void UpdateCraftInfo()
        {
            _currentRecipeImage.sprite = _currentRecipe.Sprite;
            _recipeName.text = _currentRecipe.RecipeName;
            SetupRequirements();
            _craftButton.interactable = CanItemBeCrafted();
        }
        
        public void UpdateRecipeViews()
        {
            var recipeNamePrefix = _recipeNameInput.text.Trim();

            var query = _recipeData
                .Where(r => _showLockedRecipes || !r.Locked); // Учитываем флаг чекбокса
       
            if (!string.IsNullOrEmpty(recipeNamePrefix))
            {
                query = query.Where(r => r.RecipeName.StartsWith(recipeNamePrefix, System.StringComparison.OrdinalIgnoreCase));
            }

            // Сортировка: сначала открытые, потом закрытые, оба блока — по алфавиту
            var filtered = query
                .OrderBy(r => r.Locked) // false (открытые) идут первыми
                .ThenBy(r => r.RecipeName)
                .ToList();

            SetupViews(filtered);
            _noRecipesFoundText.gameObject.SetActive(filtered.Count == 0);
        }
        
        private void UpdateCraftCountText() => _craftCountText.text = _craftCount.ToString();

        private void ToggleLockedRecipes(bool showLockedRecipes)
        {
            _showLockedRecipes = showLockedRecipes;
            UpdateRecipeViews();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            _craftCount = 1;
            UpdateCraftCountText();
        }
        
        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}