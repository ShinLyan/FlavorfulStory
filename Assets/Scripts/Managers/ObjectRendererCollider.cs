using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRendererCollider : MonoBehaviour
{
    /// <summary> Триггер выхода из коллайдера, прикрепленного к сетке GridObject.</summary>
    private void OnTriggerStay(Collider other)
    {
        MeshRenderer mr = null;
        other.TryGetComponent<MeshRenderer>(out mr);
        if (mr != null)
            mr.enabled = true;
    }

    /// <summary> Триггер выхода из коллайдера, прикрепленного к сетке GridObject.</summary>
    private void OnTriggerExit(Collider other)
    {
        MeshRenderer mr = null;
        other.TryGetComponent<MeshRenderer>(out mr);
        if(mr != null)
            mr.enabled = false;
    }
}
