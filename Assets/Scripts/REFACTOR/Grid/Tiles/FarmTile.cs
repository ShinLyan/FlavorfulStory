using System;
using System.Collections;
using UnityEngine;

/// <summary> Класс ячейки сетки для огорода.</summary>
public class FarmTile : Tile
{
    //private bool isAgricultureTaken = false;
    //[SerializeField] private GameObject _tilePrefab;
    //private Color _baseColor;
    //private Color _baseParentColor;

    //private bool _isPour;
    //private bool _isDig;

    //public bool IsDig { get { return _isDig; } set { _isDig = value; } }
    //public bool IsPour { get { return _isPour; } set { _isPour = value; if (value == true) { StartCoroutine(PourCoroutine(6000)); } } }

    //public void Start()
    //{
    //    _baseParentColor = Color.black;
    //    _baseColor = _tilePrefab.GetComponent<MeshRenderer>().material.color;
    //    IsGrown = false;
    //}

    //public void Update()
    //{
    //    if (TileObject != null)
    //    {
    //        AgricultureObject agricultureObject = TileObject as AgricultureObject;
    //        agricultureObject.IsCanGrow = IsPour;
    //        if (!IsPour) _tilePrefab.GetComponent<MeshRenderer>().material.color = Color.red;
    //        else
    //        if (!agricultureObject.IsGrown)
    //            _tilePrefab.GetComponent<MeshRenderer>().material.color = Color.yellow;
    //        else
    //            _tilePrefab.GetComponent<MeshRenderer>().material.color = Color.red;

    //        if (agricultureObject.IsGrown) _tilePrefab.GetComponent<MeshRenderer>().material.color = Color.green;
    //    }
    //    else
    //    {
    //        if (IsDig && !IsPour) _tilePrefab.GetComponent<MeshRenderer>().material.color = Color.white;
    //        if (IsPour) _tilePrefab.GetComponent<MeshRenderer>().material.color = Color.blue;
    //    }
    //}

    ///// <summary> Метод для подписки на эвент созревания агрокультуры.</summary>
    //public void SubscribeToAgriculture()
    //{
    //    if (TileObject != null)
    //    {
    //        AgricultureObject agricultureObject = TileObject as AgricultureObject;
    //        agricultureObject.AgricultureGrown += AgricultureGrown;
    //        _tilePrefab.GetComponent<MeshRenderer>().material.color = Color.yellow;
    //        IsBisy = true;
    //        IsGrown = false;
    //    }
    //}
    //public bool IsGrown
    //{
    //    get; set;
    //}
    //public AgricultureObject GetAgriculture() //ToDo
    //{
    //    if (TileObject != null)
    //    {
    //        AgricultureObject agricultureObject = TileObject as AgricultureObject;
    //        agricultureObject.AgricultureGrown -= AgricultureGrown;
    //        TileObject = null;
    //        IsBisy = false;
    //        isAgricultureTaken = true;
    //        _tilePrefab.GetComponent<MeshRenderer>().material.color = _baseColor;
    //        this.gameObject.GetComponent<MeshRenderer>().material.color = _baseParentColor;
    //        return agricultureObject;
    //    }
    //    return null;
    //}
    ///// <summary> Метод , вызывающийся, когда агрокультура созреет.</summary>
    //private void AgricultureGrown()
    //{
    //    if (!isAgricultureTaken)
    //    {
    //        _tilePrefab.GetComponent<MeshRenderer>().material.color = Color.green;
    //        IsGrown = true;
    //    }
    //}

    //public void ResetTile()
    //{
    //    IsGrown = false;
    //    isAgricultureTaken = false;
    //    IsBisy = false;
    //    TileObject = null;
    //    IsDig = false;
    //    IsPour = false;
    //}

    //public IEnumerator PourCoroutine(int timer)
    //{
    //    for (float i = 0; i < timer; i += WorldTime.Instance.Tick)
    //    {
    //        Debug.Log("PourTimer: " + i.ToString());
    //        yield return null;
    //    }
    //    IsPour = false;
    //}
}