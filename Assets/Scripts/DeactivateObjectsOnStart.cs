using System.Collections.Generic;
using UnityEngine;
/// <summary> Класс отвечающий за отключение рендера объектов на старте (если их не нужно рендерить) .</summary>
///<remarks> Служебный класс. Вешается на 1 объект сцены, отвечающий за данную логику .</remarks>
public class DeactivateObjectsOnStart : MonoBehaviour
{
    [SerializeField] List<GameObject> ObjectsToDeactivate;
    void Update()
    {
        if (AllActivated()) { DeactivateAll(); this.gameObject.SetActive(false); }
    }

    /// <summary>Метод для деактивации объектов .</summary>
    public void DeactivateAll()
    {
        foreach (var item in ObjectsToDeactivate)
        {
            item.SetActive(false);
        }
    }

    /// <summary>Bool , отражающий значения: активированы ли все объекты/активированы не все .</summary>
    bool AllActivated()
    {
        foreach (var item in ObjectsToDeactivate)
        {
            if (item.activeSelf == false) { return false; };
        }
        return true;
    }
}
