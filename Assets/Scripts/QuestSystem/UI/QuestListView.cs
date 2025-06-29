using UnityEngine;

namespace FlavorfulStory.QuestSystem
{
    public class QuestListView : MonoBehaviour
    {
        [SerializeField] private Quest[] _tempQuests;

        [SerializeField] private QuestListButton _questListButtonPrefab;

        private void Start()
        {
            foreach (Transform child in transform) Destroy(child.gameObject);

            foreach (var quest in _tempQuests)
            {
                var instance = Instantiate(_questListButtonPrefab, transform);
                instance.Setup(quest);
            }
        }
    }
}