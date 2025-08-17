using System.Linq;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace FlavorfulStory
{
    //TODO: Закрытие на Esc в update() не добавляю, тк это будет заложено в ближайшую фабрику окон
    public class RecipeUnlockedNotificationWindow : MonoBehaviour
    {
        [Inject] 
        private ICraftingRecipeProvider _craftingRecipeProvider;
        
        [SerializeField] private TMP_Text _recipeItemName;
        [SerializeField] private TMP_Text _price;
        [SerializeField] private TMP_Text _recipeDescription;
        [SerializeField] private Button _closeButton;
        [SerializeField] private RecipeIOStrip _recipeIOStrip;

        private bool _isSetup;
        
        private void Awake() => _closeButton.onClick.AddListener(Close);

        public void Show()
        {
            if (!_isSetup) Debug.LogError($"You have to setup {nameof(RecipeUnlockedNotificationWindow)} first. " +
                                          "Call Setup() before calling Show()");
            WorldTime.Pause();
            gameObject.SetActive(true);
        }

        public void Setup(string recipeId)
        {
            var recipe = _craftingRecipeProvider.GetById(recipeId);
            _recipeItemName.text = recipe.RecipeName;
            _price.text = recipe.OutputItems.Sum(stack => stack.Item.BuyPrice * stack.Number).ToString();
            _recipeDescription.text = recipe.Description;
            _recipeIOStrip.Build(recipe);
            _isSetup = true;
        }

        private void Close()
        {
            _isSetup = false;
            WorldTime.Unpause();
            gameObject.SetActive(false);
        }  
    }
}