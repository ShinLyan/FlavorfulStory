using NPC;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [SerializeField] GameObject Route;
    public void Update()
    {
        NpcRoute npcRoute = Route.GetComponent<NpcRoute>();
        //Debug.Log("Is Route achieved by NPC1 : " + npcRoute.IsAchieved("NPC1").ToString());
        //Debug.Log("Is Route achieved by NPC2 : " + npcRoute.IsAchieved("NPC2").ToString());
    }
}