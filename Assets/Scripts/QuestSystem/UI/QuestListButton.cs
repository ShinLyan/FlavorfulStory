using FlavorfulStory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FlavorfulStory.QuestSystem.UI
{
    /// <summary> Кнопка в списке квестов, открывающая описание выбранного квеста. </summary>
    public class QuestListButton : CustomButton
    {
        /// <summary> Текстовое поле с названием квеста. </summary>
        [SerializeField] private TMP_Text _questNameText;

        /// <summary> Изображение, показывающее состояние наведения на кнопку. </summary>
        [SerializeField] private Image _hoverImage;

        /// <summary> Ссылка на представление описания квеста. </summary>
        private QuestDescriptionView _questDescriptionView;

        /// <summary> Статус связанного квеста. </summary>
        private QuestStatus _questStatus;

        /// <summary> Внедрение зависимостей через Zenject. </summary>
        /// <param name="questDescriptionView"> Представление описания квеста. </param>
        [Inject]
        private void Construct(QuestDescriptionView questDescriptionView) =>
            _questDescriptionView = questDescriptionView;

        /// <summary> Настраивает кнопку с данными квеста. </summary>
        /// <param name="questStatus"> Статус квеста для отображения. </param>
        public void Setup(QuestStatus questStatus)
        {
            _questStatus = questStatus;
            _questNameText.text = questStatus.Quest.QuestName;
        }

        /// <summary> Обработчик включения взаимодействия — скрывает индикатор наведения. </summary>
        protected override void OnInteractionEnabled() => _hoverImage.gameObject.SetActive(false);

        /// <summary> Обработчик отключения взаимодействия — показывает индикатор наведения. </summary>
        protected override void OnInteractionDisabled() => _hoverImage.gameObject.SetActive(true);

        /// <summary> Обработчик клика по кнопке — вызывает выбор квеста. </summary>
        protected override void Click()
        {
            base.Click();
            Select();
        }

        /// <summary> Делает кнопку неактивной и отображает описание квеста. </summary>
        public void Select()
        {
            Interactable = false;
            _questDescriptionView.UpdateView(_questStatus);
        }
    }
}