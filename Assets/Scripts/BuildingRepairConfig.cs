using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FlavorfulStory/Building/RepairConfig", fileName = "RepairConfig")]
public class BuildingRepairConfig : ScriptableObject
{
    [Tooltip("Префаб ремонтируемого объекта по умолчанию")]
    public GameObject DefaultGameObject; 
 
    [Tooltip("Стадии строительства")]
    public List<RepairStage> Stages;
}