using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FlavorfulStory/Building/RepairConfig", fileName = "RepairConfig")]
public class BuildingRepairConfig : ScriptableObject
{
    public List<RepairStage> _stages;
}