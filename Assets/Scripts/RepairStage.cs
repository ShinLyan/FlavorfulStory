using System;
using System.Collections.Generic;
using UnityEngine;
using FlavorfulStory.InventorySystem;

/// <summary> Структура, определяющая требование ресурса для ремонта. </summary>
[Serializable]
public struct ResourceRequirement
{
    /// <summary> Требуемый предмет из инвентаря. </summary>
    [Tooltip("Требуемый ресурс.")] public InventoryItem Item;

    /// <summary> Необходимое количество предмета. </summary>
    [Tooltip("Количество требуемого ресурса .")]
    public int Quantity;
}

/// <summary> Структура, определяющая стадию ремонта здания. </summary>
[Serializable]
public struct RepairStage
{
    /// <summary> Название объекта на данной стадии ремонта. </summary>
    [Tooltip("Ресурсные требования для выполнения стадии ремонта.")]
    public string ObjectName;

    /// <summary> Префаб для визуального представления этой стадии ремонта. </summary>
    [Tooltip("Префаб объекта выполненной стадии ремонта.")]
    public GameObject Gameobject;

    /// <summary> Список требуемых ресурсов для завершения этой стадии. </summary>
    [Tooltip("Ресурсные требования для выполнения стадии ремонта.")]
    public List<ResourceRequirement> Requirements;
}