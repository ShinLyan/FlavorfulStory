using UnityEngine;
using UnityEngine.EventSystems;

namespace FlavorfulStory.InventorySystem.UI.Dragging
{
    /// <summary> Позволяет перетаскивать элемент UI из контейнера в контейнер.</summary>
    /// <remarks> Создайте подкласс для типа, который вы хотите использовать для перетаскивания.
    /// Затем поместите в элемент UI, который вы хотите сделать доступным для перетаскивания.
    /// 
    /// Во время перетаскивания элемент отображается на родительском канвасе.\n
    /// После перетаскивания элемента он будет автоматически возвращен к исходному родителю в UI.
    /// 
    /// Задача компонентов, реализующих "IDragContainer", "IDragDestination" и "IDragSource", 
    /// заключается в обновлении интерфейса после того, как произошло перетаскивание.
    /// </remarks>
    /// <typeparam name="T">Тип, представляющий перетаскиваемый элемент.</typeparam>
    public abstract class DragItem<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
        where T : class
    {
        /// <summary> Начальная позиция элемента перед началом перетаскивания.</summary>
        private Vector3 _startPosition;

        /// <summary> Изначальный родительский контейнер элемента.</summary>
        private Transform _originalParent;

        /// <summary> Источник, из которого был взят элемент для перетаскивания.</summary>
        private IDragSource<T> _source;

        /// <summary> Родительский канвас, используемый для отображения элемента во время перетаскивания.</summary>
        private Canvas _parentCanvas;

        /// <summary>Инициализирует источники данных и родительский канвас.</summary>
        private void Awake()
        {
            _source = GetComponentInParent<IDragSource<T>>();
            _parentCanvas = GetComponentInParent<Canvas>();
        }

        /// <summary> Вызывается при начале перетаскивания элемента.</summary>
        /// <remarks> Перемещает элемент на канвас и отключает блокировку лучей для CanvasGroup.</remarks>
        /// <param name="eventData"> Данные события курсора.</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPosition = transform.position;
            _originalParent = transform.parent;

            // В противном случае событие удаления не будет получено.
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            transform.SetParent(_parentCanvas.transform, true);
        }

        /// <summary> Обновляет позицию элемента в соответствии с положением
        /// курсора мыши во время перетаскивания.</summary>
        /// <param name="eventData"> Данные события курсора.</param>
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;

            // TODO BUG: когда перетягиваешь из инветаря предмет, а потом закрываешь инвентарь
        }

        /// <summary> Вызывается при завершении перетаскивания элемента.</summary>
        /// <remarks> Возвращает элемент в исходное положение или перемещает его в целевой контейнер.</remarks>
        /// <param name="eventData"> Данные события курсора.</param>
        public void OnEndDrag(PointerEventData eventData)
        {
            transform.position = _startPosition;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            transform.SetParent(_originalParent, true);

            IDragDestination<T> container;

            // Проверяем, был ли нажат курсор мыши на UI элемент
            if (EventSystem.current.IsPointerOverGameObject())
            {
                container = GetContainer(eventData);
            }
            else
            {
                container = _parentCanvas.GetComponent<IDragDestination<T>>();
                // DROP ITEM TO THE WORLD
            }

            if (container != null)
            {
                DropItemIntoContainer(container);
            }
        }

        /// <summary> Находит целевой контейнер для элемента, 
        /// если указатель мыши находится над объектом UI.</summary>
        /// <param name="eventData"> Данные события курсора.</param>
        /// <returns> Возращается целевой контейнер или null.</returns>
        private IDragDestination<T> GetContainer(PointerEventData eventData)
        {
            if (eventData.pointerEnter)
            {
                var container = eventData.pointerEnter.GetComponentInParent<IDragDestination<T>>();
                return container;
            }
            return null;
        }

        /// <summary> Перемещает элемент в целевой контейнер, обновляя его состояние.</summary>
        /// <param name="destination"> Целевой контейнер для элемента.</param>
        private void DropItemIntoContainer(IDragDestination<T> destination)
        {
            if (ReferenceEquals(destination, _source)) return;

            var destinationContainer = destination as IDragContainer<T>;
            var sourceContainer = _source as IDragContainer<T>;

            // Обмен невозможен.
            if (destinationContainer == null || sourceContainer == null ||
                destinationContainer.GetItem() == null ||
                ReferenceEquals(destinationContainer.GetItem(), sourceContainer.GetItem()))
            {
                AttemptSimpleTransfer(destination);
                return;
            }

            AttemptSwap(destinationContainer, sourceContainer);
        }

        /// <summary> Пытается выполнить простую передачу элемента между контейнерами.</summary>
        private bool AttemptSimpleTransfer(IDragDestination<T> destination)
        {
            var draggingItem = _source.GetItem();
            var draggingNumber = _source.GetNumber();

            int acceptable = destination.GetMaxAcceptableItemsNumber(draggingItem);
            int toTransfer = Mathf.Min(acceptable, draggingNumber);
            if (toTransfer > 0)
            {
                _source.RemoveItems(toTransfer);
                destination.AddItems(draggingItem, toTransfer);
                return false;
            }

            return true;
        }

        /// <summary> Пытается выполнить обмен элементами между контейнерами.</summary>
        private void AttemptSwap(IDragContainer<T> destination, IDragContainer<T> source)
        {
            // Предварительно снимаем элемент с обеих сторон.
            int removedSourceNumber = source.GetNumber();
            var removedSourceItem = source.GetItem();
            int removedDestinationNumber = destination.GetNumber();
            var removedDestinationItem = destination.GetItem();

            source.RemoveItems(removedSourceNumber);
            destination.RemoveItems(removedDestinationNumber);

            int sourceTakeBackNumber = CalculateTakeBack(removedSourceItem, removedSourceNumber, source, destination);
            int destinationTakeBackNumber = CalculateTakeBack(removedDestinationItem, removedDestinationNumber, destination, source);

            if (sourceTakeBackNumber > 0)
            {
                source.AddItems(removedSourceItem, sourceTakeBackNumber);
                removedSourceNumber -= sourceTakeBackNumber;
            }
            if (destinationTakeBackNumber > 0)
            {
                destination.AddItems(removedDestinationItem, destinationTakeBackNumber);
                removedDestinationNumber -= destinationTakeBackNumber;
            }

            // Отмена, если мы не сможем выполнить успешную замену.
            if (source.GetMaxAcceptableItemsNumber(removedDestinationItem) < removedDestinationNumber ||
                destination.GetMaxAcceptableItemsNumber(removedSourceItem) < removedSourceNumber)
            {
                destination.AddItems(removedDestinationItem, removedDestinationNumber);
                source.AddItems(removedSourceItem, removedSourceNumber);
                return;
            }

            // Свапаем.
            if (removedDestinationNumber > 0)
            {
                source.AddItems(removedDestinationItem, removedDestinationNumber);
            }
            if (removedSourceNumber > 0)
            {
                destination.AddItems(removedSourceItem, removedSourceNumber);
            }
        }

        /// <summary> Вычисляет количество предметов, которые следует вернуть обратно в источник.</summary>
        private int CalculateTakeBack(T removedItem, int removedNumber, IDragContainer<T> removeSource, IDragContainer<T> destination)
        {
            int takeBackNumber = 0;
            int destinationMaxAcceptable = destination.GetMaxAcceptableItemsNumber(removedItem);

            if (destinationMaxAcceptable < removedNumber)
            {
                takeBackNumber = removedNumber - destinationMaxAcceptable;

                int sourceTakeBackAcceptable = removeSource.GetMaxAcceptableItemsNumber(removedItem);
                if (sourceTakeBackAcceptable < takeBackNumber) return 0;
            }
            return takeBackNumber;
        }
    }
}