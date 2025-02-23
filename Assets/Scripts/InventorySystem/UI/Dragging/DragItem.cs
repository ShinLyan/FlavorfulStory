using UnityEngine;
using UnityEngine.EventSystems;

namespace FlavorfulStory.InventorySystem.UI.Dragging
{
    /// <summary> Позволяет перетаскивать элементы UI из одного контейнера в другой. </summary>
    /// <remarks> Создайте подкласс для типа элемента, который вы хотите перетаскивать. 
    /// Добавьте этот скрипт в элемент UI, который должен быть доступен для перетаскивания.
    /// Элемент перемещается на родительский Canvas во время перетаскивания.
    /// После завершения он возвращается в исходный контейнер или перемещается в новый.
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

        /// <summary> Группа CanvasGroup, контролирующая взаимодействие с элементом во время перетаскивания. </summary>
        private CanvasGroup _canvasGroup;

        /// <summary> Инициализирует источник данных и родительский Canvas. </summary>
        private void Awake()
        {
            _source = GetComponentInParent<IDragSource<T>>();
            _parentCanvas = GetComponentInParent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        /// <summary> Начало перетаскивания элемента. </summary>
        /// <param name="eventData"> Данные события курсора. </param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // Сохраняем начальные параметры перед началом перетаскивания
            _startPosition = transform.position;
            _originalParent = transform.parent;

            // Отключаем блокировку лучей для корректного взаимодействия с UI
            _canvasGroup.blocksRaycasts = false;

            // Перемещаем элемент на родительский Canvas для корректного отображения
            transform.SetParent(_parentCanvas.transform, true);
        }

        /// <summary> Обновление позиции элемента во время перетаскивания. </summary>
        /// <param name="eventData"> Данные события курсора. </param>
        public void OnDrag(PointerEventData eventData)
        {
            // Обновляем позицию элемента в соответствии с движением курсора
            transform.position = eventData.position;
        }

        /// <summary> Завершение перетаскивания элемента. </summary>
        /// <remarks> Возвращает элемент в исходное положение или перемещает его в целевой контейнер. </remarks>
        /// <param name="eventData"> Данные события курсора. </param>
        public void OnEndDrag(PointerEventData eventData)
        {
            // Возвращаем объект в исходное положение и включаем блокировку лучей
            transform.position = _startPosition;
            _canvasGroup.blocksRaycasts = true;
            transform.SetParent(_originalParent, true);

            // Определяем контейнер, в который может быть помещён предмет
            var container = EventSystem.current.IsPointerOverGameObject()
                ? GetContainer(eventData)
                : _parentCanvas.GetComponent<IDragDestination<T>>();

            // Если контейнер найден, выполняем перемещение предмета
            if (container != null) DropItemIntoContainer(container);
        }

        /// <summary> Поиск целевого контейнера под указателем мыши. </summary>
        /// <param name="eventData"> Данные события курсора. </param>
        /// <returns> Целевой контейнер или null, если контейнер не найден. </returns>
        private static IDragDestination<T> GetContainer(PointerEventData eventData) =>
            eventData.pointerEnter?.GetComponentInParent<IDragDestination<T>>();

        /// <summary> Перемещение элемента в целевой контейнер. </summary>
        /// <param name="destination"> Целевой контейнер для перемещения элемента. </param>
        private void DropItemIntoContainer(IDragDestination<T> destination)
        {
            // Проверяем, чтобы предмет не перемещался в тот же контейнер
            if (ReferenceEquals(destination, _source)) return;

            // Определяем тип перемещения: обмен или перемещение предмета в контейнер
            if (destination is IDragContainer<T> destinationContainer &&
                _source is IDragContainer<T> sourceContainer &&
                destinationContainer.GetItem() != null)
                SwapItems(destinationContainer, sourceContainer);
            else
                TransferItems(destination);
        }

        /// <summary> Выполняет обмен элементами между контейнерами. </summary>
        /// <param name="destination"> Целевой контейнер для обмена. </param>
        /// <param name="source"> Исходный контейнер для обмена. </param>
        private static void SwapItems(IDragContainer<T> destination, IDragContainer<T> source)
        {
            // Сохраняем информацию о предметах и их количестве перед обменом
            var (sourceItem, sourceNumber) = (source.GetItem(), source.GetNumber());
            var (destinationItem, destinationNumber) = (destination.GetItem(), destination.GetNumber());

            // Удаляем предметы из обоих контейнеров перед обменом
            source.RemoveItems(sourceNumber);
            destination.RemoveItems(destinationNumber);

            // Проверяем возможность возврата части предметов обратно в исходные контейнеры
            int returnSource = CalculateReturnAmount(sourceItem, sourceNumber, source, destination);
            int returnDestination = CalculateReturnAmount(destinationItem, destinationNumber, destination, source);

            // Возвращаем излишки предметов обратно в контейнеры
            if (returnSource > 0) source.AddItems(sourceItem, returnSource);
            if (returnDestination > 0) destination.AddItems(destinationItem, returnDestination);

            // Проверяем, возможен ли обмен предметами
            if (source.GetMaxAcceptableItemsNumber(destinationItem) >= destinationNumber &&
                destination.GetMaxAcceptableItemsNumber(sourceItem) >= sourceNumber)
            {
                // Завершаем обмен
                if (destinationNumber > 0) source.AddItems(destinationItem, destinationNumber);
                if (sourceNumber > 0) destination.AddItems(sourceItem, sourceNumber);
            }
            else
            {
                // Если обмен невозможен, возвращаем предметы в исходные контейнеры
                source.RemoveItems(sourceNumber);
                destination.RemoveItems(destinationNumber);
                destination.AddItems(destinationItem, destinationNumber);
                source.AddItems(sourceItem, sourceNumber);
            }
        }

        /// <summary> Выполняет передачу элемента между контейнерами. </summary>
        /// <param name="destination"> Целевой контейнер для передачи элемента. </param>
        private void TransferItems(IDragDestination<T> destination)
        {
            // Получаем перетаскиваемый предмет и его количество
            var item = _source.GetItem();
            int number = Mathf.Min(destination.GetMaxAcceptableItemsNumber(item), _source.GetNumber());

            // Проверяем, можно ли переместить предмет
            if (number > 0)
            {
                // Удаляем предмет из исходного контейнера и добавляем в целевой
                _source.RemoveItems(number);
                destination.AddItems(item, number);
            }
        }

        /// <summary> Рассчитывает количество предметов, которые необходимо вернуть в исходный контейнер. </summary>
        /// <param name="item"> Предмет, который был удален. </param>
        /// <param name="number"> Количество удаленных предметов. </param>
        /// <param name="removeFrom"> Исходный контейнер, из которого был удален предмет. </param>
        /// <param name="destination"> Целевой контейнер, куда был передан предмет. </param>
        /// <returns> Количество предметов, которые нужно вернуть обратно в исходный контейнер. </returns>
        private static int CalculateReturnAmount(T item, int number,
            IDragContainer<T> removeFrom, IDragContainer<T> destination)
        {
            // Рассчитываем излишки предметов, которые не могут быть перенесены в целевой контейнер
            int excess = number - destination.GetMaxAcceptableItemsNumber(item);

            // Проверяем, можно ли вернуть излишки обратно в исходный контейнер
            return excess > 0 && removeFrom.GetMaxAcceptableItemsNumber(item) >= excess ? excess : 0;
        }
    }
}