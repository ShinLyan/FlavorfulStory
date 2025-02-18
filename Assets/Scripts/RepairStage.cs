using System;
using System.Collections.Generic;
using UnityEngine;
using FlavorfulStory.InventorySystem;

[Serializable]
public struct ResourceRequirement
{
    [Tooltip("Требуемый ресурс.")]
    public InventoryItem Item;
    
    [Tooltip("Количество требуемого ресурса .")]
    public int Quantity;
}


[Serializable]
public struct RepairStage
{
    [Tooltip("Ресурсные требования для выполнения стадии ремонта.")]
    public string objectName;

    [Tooltip("Префаб объекта выполненной стадии ремонта.")]
    public GameObject Gameobject;
    
    [Tooltip("Ресурсные требования для выполнения стадии ремонта.")]
    public List<ResourceRequirement> Requirements;
}