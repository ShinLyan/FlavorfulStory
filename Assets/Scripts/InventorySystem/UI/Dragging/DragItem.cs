using UnityEngine;
using UnityEngine.EventSystems;

namespace FlavorfulStory.InventorySystem.UI.Dragging
{
    /// <summary> Позволяет перетаскивать элементы UI из одного контейнера в другой. </summary>
    /// <remarks> Создайте подкласс для типа элемента, который вы хотите перетаскивать. 
    /// Добавьте этот скрипт в элемент UI, который должен быть доступен для перетаскивания.
    /// Элемент перемещается на родительский Canvas во время перетаскивания.
    /// После завершения он возвращается в исходный контейнер или перемещается в новый. </remarks>
    /// Задача компонентов, реализующих "IDragContainer", "IDragDestination" и "IDragSource", 
    /// заключается в обновлении интерфейса после того, как произошло перетаскивание. </remarks>
    /// <typeparam name="T"> Тип объекта, который можно перетаскивать. </typeparam>
    public abstract class DragItem<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
        where T : class
    {
        /// <summary> Начальная позиция элемента до начала перетаскивания. </summary>
        private Vector3 _startPosition;

        /// <summary> Исходный родительский контейнер элемента. </summary>
        private Transform _originalParent;

        /// <summary> Источник, из которого был взят элемент для перетаскивания. </summary>
        private IDragSource<T> _source;

        /// <summary> Родительский Canvas, используемый для отображения элемента во время перетаскивания. </summary>
        private Canvas _parentCanvas;

        /// <summary> Инициализирует источник данных и родительский Canvas. </summary>
        private void Awake()
        {
            _source = GetComponentInParent<IDragSource<T>>();
            _parentCanvas = GetComponentInParent<Canvas>();
        }

        /// <summary> Начало перетаскивания элемента. </summary>
        /// <remarks> Перемещает элемент на канвас и отключает блокировку лучей для CanvasGroup. </remarks>
        /// <param name="eventData"> Данные события курсора. </param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPosition = transform.position;
            _originalParent = transform.parent;

            // В противном случае событие удаления не будет получено.
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            transform.SetParent(_parentCanvas.transform, true);
        }

        /// <summary> Обновление позиции элемента во время перетаскивания. </summary>
        /// <param name="eventData"> Данные события курсора. </param>
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        /// <summary> Завершение перетаскивания элемента. </summary>
        /// <remarks> Возвращает элемент в исходное положение или перемещает его в целевой контейнер. </remarks>
        /// <param name="eventData"> Данные события курсора. </param>
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
            }

            if (container != null) DropItemIntoContainer(container);
        }

        /// <summary> Поиск целевого контейнера под указателем мыши. </summary>
        /// <param name="eventData"> Данные события курсора. </param>
        /// <returns> Целевой контейнер или null, если контейнер не найден. </returns>
        private static IDragDestination<T> GetContainer(PointerEventData eventData)
        {
            if (!eventData.pointerEnter) return null;

            var container = eventData.pointerEnter.GetComponentInParent<IDragDestination<T>>();
            return container;
        }

        /// <summary> Перемещение элемента в целевой контейнер, обновляя его состояние. </summary>
        /// <param name="destination"> Целевой контейнер для перемещения элемента. </param>
        private void DropItemIntoContainer(IDragDestination<T> destination)
        {
            if (ReferenceEquals(destination, _source)) return;

            // Обмен невозможен.
            if (destination is not IDragContainer<T> destinationContainer ||
                _source is not IDragContainer<T> sourceContainer ||
                destinationContainer.GetItem() == null ||
                ReferenceEquals(destinationContainer.GetItem(), sourceContainer.GetItem()))
            {
                AttemptSimpleTransfer(destination);
                return;
            }

            AttemptSwap(destinationContainer, sourceContainer);
        }

        /// <summary> Выполняет передачу элемента между контейнерами. </summary>
        /// <param name="destination"> Целевой контейнер для передачи элемента. </param>
        /// <returns> true, если передача невозможна, иначе false. </returns>
        private bool AttemptSimpleTransfer(IDragDestination<T> destination)
        {
            var draggingItem = _source.GetItem();
            var draggingNumber = _source.GetNumber();

            int acceptable = destination.GetMaxAcceptableItemsNumber(draggingItem);
            int toTransfer = Mathf.Min(acceptable, draggingNumber);
            if (toTransfer <= 0) return true;

            _source.RemoveItems(toTransfer);
            destination.AddItems(draggingItem, toTransfer);
            return false;
        }

        /// <summary> Выполняет обмен элементами между контейнерами. </summary>
        /// <param name="destination"> Целевой контейнер для обмена. </param>
        /// <param name="source"> Исходный контейнер для обмена. </param>
        private static void AttemptSwap(IDragContainer<T> destination, IDragContainer<T> source)
        {
            // Предварительно снимаем элемент с обеих сторон.
            int removedSourceNumber = source.GetNumber();
            var removedSourceItem = source.GetItem();
            int removedDestinationNumber = destination.GetNumber();
            var removedDestinationItem = destination.GetItem();

            // Удаление элементов из обоих контейнеров.
            source.RemoveItems(removedSourceNumber);
            destination.RemoveItems(removedDestinationNumber);

            // Рассчитываем количество предметов, которые необходимо вернуть.
            int sourceTakeBackNumber = CalculateTakeBack(removedSourceItem, removedSourceNumber, source, destination);
            int destinationTakeBackNumber = CalculateTakeBack(
                removedDestinationItem, removedDestinationNumber, destination, source);
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

            // Проверяем, можно ли выполнить успешный обмен.
            if (source.GetMaxAcceptableItemsNumber(removedDestinationItem) < removedDestinationNumber ||
                destination.GetMaxAcceptableItemsNumber(removedSourceItem) < removedSourceNumber)
            {
                // Возвращаем предметы в исходные контейнеры, если обмен невозможен.
                destination.AddItems(removedDestinationItem, removedDestinationNumber);
                source.AddItems(removedSourceItem, removedSourceNumber);
                return;
            }

            // Выполняем обмен.
            if (removedDestinationNumber > 0)
            {
                source.AddItems(removedDestinationItem, removedDestinationNumber);
            }

            if (removedSourceNumber > 0)
            {
                destination.AddItems(removedSourceItem, removedSourceNumber);
            }
        }
        
        /// <summary> Рассчитывает количество предметов, которые необходимо вернуть в исходный контейнер. </summary>
        /// <param name="removedItem"> Предмет, который был удален. </param>
        /// <param name="removedNumber"> Количество удаленных предметов. </param>
        /// <param name="removeSource"> Исходный контейнер, из которого был удален предмет. </param>
        /// <param name="destination"> Целевой контейнер, куда был передан предмет. </param>
        /// <returns> Количество предметов, которые нужно вернуть обратно в исходный контейнер. </returns>
        private static int CalculateTakeBack(T removedItem, int removedNumber,
            IDragContainer<T> removeSource, IDragContainer<T> destination)
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