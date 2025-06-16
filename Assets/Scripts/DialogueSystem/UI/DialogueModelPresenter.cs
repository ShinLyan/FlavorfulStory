using UnityEngine;

namespace FlavorfulStory.DialogueSystem.UI
{
    /// <summary> Отображение модели персонажа в диалогах. </summary>
    public class DialogueModelPresenter : MonoBehaviour
    {
        /// <summary> Точка появления модели. </summary>
        [SerializeField] private Transform _spawnPoint;

        /// <summary> Текущая активная модель. </summary>
        private GameObject _currentModel;

        /// <summary> Удаляет все дочерние объекты из точки появления при инициализации. </summary>
        private void Awake()
        {
            foreach (Transform child in _spawnPoint) Destroy(child.gameObject);
        }

        /// <summary> Создать экземпляр модели и установить ей нужный слой. </summary>
        /// <param name="modelPrefab"> Префаб модели для отображения. </param>
        /// <returns> Ссылка на созданную модель. </returns>
        public GameObject InstantiateModel(GameObject modelPrefab)
        {
            _currentModel = Instantiate(modelPrefab, _spawnPoint);
            SetLayerRecursively(_currentModel.transform, LayerMask.NameToLayer("DialogueModel"));
            return _currentModel;
        }

        /// <summary> Рекурсивно установить слой для объекта и всех его потомков. </summary>
        /// <param name="root"> Корневой трансформ. </param>
        /// <param name="layer"> Целевой слой. </param>
        private static void SetLayerRecursively(Transform root, int layer)
        {
            root.gameObject.layer = layer;
            foreach (Transform child in root) SetLayerRecursively(child, layer);
        }

        /// <summary> Очистить текущую активную модель без уничтожения. </summary>
        public void ClearModel() => _currentModel = null;
    }
}