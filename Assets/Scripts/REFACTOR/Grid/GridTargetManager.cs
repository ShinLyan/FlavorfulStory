using Unity.Burst.CompilerServices;
using UnityEngine;

/// <summary> Класс, описывающий взаимодействие с сеткой GridObject.</summary>
public class GridTargetManager : MonoBehaviour //ToDo: Create + GridGetTileManager to get Tile
{
    /// <summary> Объект камеры.</summary>
    [SerializeField] Camera _camera;
    /// <summary> Слой для рейкаста.</summary>
    [SerializeField] LayerMask _layerMask;
    /// <summary> Позиция для размещения на сетке.</summary>
    private Vector3 _position;
    /// <summary> Подсвеченная ячейка сетки.</summary>
    private GameObject highlightedTile = null;

    public void HighlightTile() //Упростить
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _camera.nearClipPlane; //вероятно из-за этого отклик плохой и не всегда попадает на ячейку нужную
        Ray ray = _camera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000, _layerMask))
        {
            Tile selectedTile = null;
            _position = hit.point;
            if (hit.transform.CompareTag("Tile"))
            {
                Vector3 tilePos = hit.transform.gameObject.transform.position;
                if (highlightedTile != null)
                {
                    if (!highlightedTile.Equals(hit.transform.gameObject))
                    {
                        highlightedTile.TryGetComponent<Tile>(out selectedTile);
                        if (selectedTile != null)
                        {
                            highlightedTile.GetComponent<MeshRenderer>().material.color = Color.black;
                        }
                        highlightedTile = hit.transform.gameObject;
                    }
                }
                else
                {
                    highlightedTile = hit.transform.gameObject;
                }
                highlightedTile.TryGetComponent<Tile>(out selectedTile);
                if (selectedTile != null)
                {
                    if (!selectedTile.IsBisy)
                    {
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    }
                }
                // Call SetColor using the shader property name "_Color" and setting the color to red
            }
        }
    }

    public void UnhighlightTile() //Упростить
    {
        if (highlightedTile != null)
            highlightedTile.GetComponent<MeshRenderer>().material.color = Color.black;
    }

    /// <summary> Метод для получения координат для установки на сетку.</summary>
    public Tile GetSelectedTile()
    {
        Tile selectedTile = null;
        highlightedTile.TryGetComponent<Tile>(out selectedTile);
        return selectedTile;
    }
    /// <summary> Метод для установки на сетку grid.</summary>
    //public bool PlaceOnGrid(IPlaceableItem item, ObjectGrid grid, PlaceableTile gridTile) //вынести в нужный класс Tile
    //{
    //    PlaceableTile tile = gridTile;
    //    if (tile.TileObject == null)
    //    {
    //        tile.TileObject = item.Place(new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z), grid.gameObject.transform.parent);
    //        return true;
    //    }
    //    return false;
    //}

    /// <summary> Метод для установки на сетку grid.</summary>
    //public bool PlantOnGrid(IAgricultureItem item, ObjectGrid grid, FarmTile gridTile) //вынести в нужный класс Tile
    //{
    //    FarmTile tile = gridTile;
    //    if (tile.TileObject == null)
    //    {
    //        tile.TileObject = item.Plant(new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z), grid.gameObject.transform.parent);
    //        tile.SubscribeToAgriculture();
    //        return true;
    //    }
    //    return false;
    //}
}