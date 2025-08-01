using FlavorfulStory.LocalizationSystem;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.QuestSystem
{
    // TODO: Данила СДЕЛАЙ КРАСИВО
    public class QuestNotificationView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _questNameText;

        private void Awake() => Hide();

        /// <summary> Показать уведомление. </summary>
        /// <param name="questName"> Название квеста. </param>
        public void Show(string questName)
        {
            _questNameText.text = LocalizationService.GetLocalizedString(questName);
            gameObject.SetActive(true);

            CancelInvoke(nameof(Hide));
            Invoke(nameof(Hide), 3f);
        }

        /// <summary> Скрыть уведомление. </summary>
        private void Hide() => gameObject.SetActive(false);
    }
}