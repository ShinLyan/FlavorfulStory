using TMPro;
using UnityEngine;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Отображение цели квеста. </summary>
    public class QuestObjectiveView : MonoBehaviour
    {
        /// <summary> Текстовое поле для описания цели. </summary>
        [SerializeField] private TMP_Text _objectiveText;

        /// <summary> Иконка, показывающая завершение цели. </summary>
        [SerializeField] private GameObject _iconOn;

        /// <summary> Флаг, отражающий состояние выполнения цели. </summary>
        [SerializeField] private bool _isComplete;

        /// <summary> Обновляет иконку в редакторе при изменении значения <see cref="_isComplete"/>. </summary>
        private void OnValidate() => _iconOn.SetActive(_isComplete);

        /// <summary> Инициализирует элемент цели квеста. </summary>
        /// <param name="objective"> Текст цели. </param>
        /// <param name="isObjectiveComplete"> Статус выполнения цели. </param>
        public void Setup(string objective, bool isObjectiveComplete)
        {
            _objectiveText.text = objective;
            _objectiveText.fontStyle = isObjectiveComplete ? FontStyles.Strikethrough : FontStyles.Normal;
            _isComplete = isObjectiveComplete;
            _iconOn.SetActive(isObjectiveComplete);
        }
    }
}