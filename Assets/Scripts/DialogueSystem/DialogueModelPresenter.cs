using UnityEngine;

namespace FlavorfulStory.DialogueSystem
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

        /// <summary> Отображает новую модель персонажа. </summary>
        /// <param name="modelPrefab"> Префаб модели для отображения. </param>
        public void ShowModel(GameObject modelPrefab)
        {
            DestroyModel();

            _currentModel = Instantiate(modelPrefab, _spawnPoint);
            SetLayerForModelRecursively(_currentModel.transform, LayerMask.NameToLayer("DialogueModel"));
        }

        /// <summary> Удаляет предыдущую модель, если она существует. </summary>
        public void DestroyModel()
        {
            if (!_currentModel) return;

            Destroy(_currentModel);
            _currentModel = null;
        }

        /// <summary> Рекурсивно устанавливает слой для корневого объекта и всех его потомков. </summary>
        /// <param name="root"> Корневой трансформ. </param>
        /// <param name="layer"> Целевой слой. </param>
        private static void SetLayerForModelRecursively(Transform root, int layer)
        {
            root.gameObject.layer = layer;
            foreach (Transform child in root) SetLayerForModelRecursively(child, layer);
        }
    }
}