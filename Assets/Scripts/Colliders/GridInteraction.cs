using UnityEngine;

/// <summary> Класс описывающий логику расстановки предметов по сетке ObjectGrid.</summary>
///<remarks> Вешается на объект - сетку объектов для размещения предметов при помощи указателя мыши.</remarks>
public class GridInteraction : MonoBehaviour //Разнести логику по разным сеткам //все коллайдеры оптимизировать. Много лишних действий в OnTriggerEnter / Stay
{
    ///// <summary> Менеджер сетки Grid.</summary>
    //[SerializeField] private GridTargetManager _gridTargetManager;
    ///// <summary> Сыылка на сетку Grid.</summary>
    //private ObjectGrid _grid;
    ///// <summary> Сыылка на сетку Grid.</summary>
    //[SerializeField] private double _interactDistance;
    ///// <summary> Ссылка на игрока.</summary>
    //[SerializeField] private Player _player;
    ///// <summary> Ссылка на контроллер игрока.</summary>
    //PlayerController _playerController;
    ///// <summary> Ссылка на менеджер инвентаря игрока.</summary>
    ////InventoryManager _manager;

    ///// <summary> Флаг режима установки.</summary>
    ////bool isPlacing = false; //ToDo: ->property
    //private Tile selectedTile = null;
    //public Tile CurrentTile { get { return selectedTile; } }

    //public void Update()
    //{
    //    if (_interactDistance > (_grid.gameObject.transform.position - _player.gameObject.transform.position).magnitude)
    //    {
    //        _gridTargetManager.gameObject.SetActive(true);
    //        _gridTargetManager.HighlightTile();
    //        Item itemToPlace = null;
    //        if (Input.GetKeyDown((KeyCode)UserKeys.CommitPlaceObject) && (_manager.GetAllInventoryItems().Count > 0))
    //        {
    //            itemToPlace = _manager.GetCurrentInventoryItem();
    //            Tile tileToPlace = _gridTargetManager.GetSelectedTile();
    //            if (tileToPlace is PlaceableTile)
    //            {
    //                if (itemToPlace is IPlaceableItem)
    //                {
    //                    bool result = _gridTargetManager.PlaceOnGrid((IPlaceableItem)itemToPlace, _grid, tileToPlace as PlaceableTile);
    //                    if (result) _manager.RemoveFromInventory(itemToPlace);
    //                }
    //            }
    //            if (tileToPlace is FarmTile)
    //            {
    //                FarmTile farmTile = tileToPlace as FarmTile;
    //                if (itemToPlace is IAgricultureItem)
    //                {
    //                    if (farmTile.IsDig)
    //                    {
    //                        Animator animator = _player.GetComponent<Animator>();
    //                        StartCoroutine(GatheringAnimationCoroutine(animator, _manager, 50, itemToPlace, _grid, farmTile));
    //                    }
    //                }
    //                if (itemToPlace is ShovelItem)
    //                {
    //                    if (farmTile.IsGrown && farmTile.IsBisy)
    //                    {
    //                        Animator animator = _player.GetComponent<Animator>();
    //                        StartCoroutine(MiningAnimationCoroutine(animator, _manager, 100, itemToPlace as ShovelItem, farmTile));
    //                    }
    //                }
    //                if (itemToPlace is ShovelItem) //Dig
    //                {
    //                    if (!farmTile.IsBisy)
    //                    {
    //                        Animator animator = _player.GetComponent<Animator>();
    //                        StartCoroutine(DiggingAnimationCoroutine(animator, _manager, 100, itemToPlace as ShovelItem, farmTile));
    //                    }
    //                }
    //                if (itemToPlace is PourItem)
    //                {
    //                    Animator animator = _player.GetComponent<Animator>();
    //                    StartCoroutine(PouringAnimationCoroutine(animator, _manager, 50, itemToPlace, _grid, farmTile));
    //                }
    //            }
    //        }
    //    }
    //    else
    //    {
    //        _gridTargetManager.UnhighlightTile(); //Оптимизировать
    //    }
    //}
    //private void Start()
    //{
    //    _playerController = _player.gameObject.GetComponent<PlayerController>();
    //    _manager = _player.GetComponentInChildren<InventoryManager>();
    //    _grid = this.gameObject.GetComponent<ObjectGrid>();
    //}
    ////Coroutines
    //public IEnumerator MiningAnimationCoroutine(Animator animator,
    //    InventoryManager inventory
    //    , int timer
    //    , ShovelItem item
    //    , FarmTile tile)
    //{
    //    if (!LockActionsManager.IsLock)
    //    {
    //        LockActionsManager.Lock();
    //        //animator.SetTrigger("Mining");
    //        //animator.ResetTrigger("Locomotion");
    //        animator.SetBool("Mining", true);
    //        for (int i = 0; i < timer; i++)
    //        {
    //            yield return null;
    //        }
    //        animator.SetBool("Mining", false);
    //        ToolItemArgs args = new ToolItemArgs();
    //        item.UseLkm(tile, args);
    //        tile.GetAgriculture();
    //        tile.ResetTile();
    //        inventory.AddToInventory(args.item);
    //        LockActionsManager.UnLock();
    //    }
    //}
    //public IEnumerator DiggingAnimationCoroutine(Animator animator,
    //InventoryManager inventory
    //, int timer
    //, ShovelItem item
    //, FarmTile tile)
    //{
    //    if (!LockActionsManager.IsLock && !tile.IsDig)
    //    {
    //        LockActionsManager.Lock();
    //        //animator.SetTrigger("Mining");
    //        //animator.ResetTrigger("Locomotion");
    //        animator.SetBool("Mining", true);
    //        for (int i = 0; i < timer; i++)
    //        {
    //            yield return null;
    //        }
    //        animator.SetBool("Mining", false);
    //        tile.IsDig = true;
    //        LockActionsManager.UnLock();
    //    }
    //}
    //public IEnumerator GatheringAnimationCoroutine(Animator animator,
    //    InventoryManager inventory
    //    , int timer
    //    , Item item
    //    , ObjectGrid grid
    //    , FarmTile tile)
    //{
    //    if (!LockActionsManager.IsLock)
    //    {
    //        LockActionsManager.Lock();
    //        //animator.ResetTrigger("Locomotion");
    //        animator.SetBool("Gathering", true);
    //        for (int i = 0; i < timer; i++)
    //        {
    //            yield return null;
    //        }
    //        animator.SetBool("Gathering", false);
    //        bool result = _gridTargetManager.PlantOnGrid((IAgricultureItem)item, grid, tile);
    //        if (result) inventory.RemoveFromInventory(item);
    //        LockActionsManager.UnLock();
    //    }
    //}
    //public IEnumerator PouringAnimationCoroutine(Animator animator,
    //InventoryManager inventory
    //, int timer
    //, Item item
    //, ObjectGrid grid
    //, FarmTile tile)
    //{
    //    if (!LockActionsManager.IsLock && !tile.IsPour)
    //    {
    //        LockActionsManager.Lock();
    //        //animator.ResetTrigger("Locomotion");
    //        animator.SetBool("Gathering", true);
    //        for (int i = 0; i < timer; i++)
    //        {
    //            yield return null;
    //        }
    //        animator.SetBool("Gathering", false);
    //        tile.IsPour = true;
    //        LockActionsManager.UnLock();
    //    }
    //}
}