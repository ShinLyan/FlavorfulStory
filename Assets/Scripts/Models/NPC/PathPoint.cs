using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathPoint : MonoBehaviour
{
    [SerializeField] string scene;
    [SerializeField] bool portal;
    [SerializeField] List<GameObject> PoolDestinations = new();

    int poolSize = 1;

    List<Vector3> pool = new List<Vector3>();

    Dictionary<string, bool> NpcAchievedDict = new Dictionary<string, bool>();

    public void Start()
    {
        poolSize = PoolDestinations.Count;
        if (poolSize > 0)
        {
            foreach (var destination in PoolDestinations)
            {
                pool.Add(destination.transform.position);
                //GameObject go = new GameObject();
                //go.transform.position -= destination;
                //Instantiate(go , this.gameObject.transform);
            }
        }
    }

    public string Scene { get { return scene; } }
    public bool isPortal { get { return portal; } }

    public Vector3 GetPosition()
    {
        if(pool.Count != 0)
        return pool[0];

        return Vector3.zero;
    }

    public void SetAcheved(string npcName)
    {
        if (!NpcAchievedDict.ContainsKey(npcName))
        {
            NpcAchievedDict.Add(npcName, true);
        }
        else
        {
            NpcAchievedDict[npcName] = true;
        }
    }

    public bool IsAchieved(string npcName)
    {
        return NpcAchievedDict[npcName];
    }

    public void ResetAcheved(string npcName)
    {
        if (!NpcAchievedDict.ContainsKey(npcName))
        {
            NpcAchievedDict.Add(npcName, false);
        }
        else
        {
            NpcAchievedDict[npcName] = false;
        }
    }

    public bool IsAnyAchieved()
    {
        return false;
    }
}