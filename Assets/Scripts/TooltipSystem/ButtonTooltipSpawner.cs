using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FlavorfulStory.TooltipSystem
{
    /// <summary> Спавнер тултипа для кнопки. </summary>
    [RequireComponent(typeof(Button))]
    public class ButtonTooltipSpawner : TooltipSpawner
    {
        /// <summary> Информация о кнопке. </summary>
        [SerializeField] private string _info;

        /// <summary> Компонент кнопки, для которой показывается тултип. </summary>
        private Button _button;

        /// <summary> Внедряет префаб тултипа кнопки. </summary>
        /// <param name="buttonTooltipPrefab"> Префаб тултипа кнопки. </param>
        [Inject]
        private void Construct(ButtonTooltipView buttonTooltipPrefab) => TooltipPrefab = buttonTooltipPrefab.gameObject;

        /// <summary> Инициализация полей. </summary>
        private void Awake() => _button = GetComponent<Button>();

        #region Override Methods

        /// <summary> Можно ли создать тултип? </summary>
        /// <returns> <c>true</c>, если кнопка активна и тултип можно создать; иначе <c>false</c>. </returns>
        protected override bool CanCreateTooltip() => _button && _button.interactable;

        /// <summary> Обновляет содержимое тултипа на основе кнопки. </summary>
        /// <param name="tooltip"> Заспавненный префаб тултипа для обновления. </param>
        protected override void UpdateTooltip(GameObject tooltip)
        {
            if (!tooltip.TryGetComponent<ButtonTooltipView>(out var buttonTooltip)) return;

            buttonTooltip.Setup(_info);
        }

        #endregion
    }
}