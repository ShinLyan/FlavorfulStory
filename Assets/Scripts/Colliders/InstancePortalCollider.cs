using NPC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary> Класс, описывающий логику работы порталов.</summary>
public class InstancePortalCollider : MonoBehaviour //все коллайдеры оптимизировать. Много лишних действий в OnTriggerEnter / Stay
{
    /// <summary> Точка телепортации. </summary>
    [SerializeField] NPC.NpcPathPoint destination;
    /// <summary> Префаб загрузочного экрана.</summary>
    [SerializeField] GameObject loadingScreen;
    /// <summary> Флаг завершения телепортации.</summary>
    public bool isTeleporting = false;
    /// <summary>Список объектов UI объектов, которые скрываются при паузе.</summary>
    [SerializeField] private List<GameObject> _disableObjects;

    /// <summary> Свойство, возвращающее точку телепортации.</summary>
    public NPC.NpcPathPoint Destination
    {
        get { return destination; }
    }

    /// <summary>Метод вызывающийся при нахождении в коллайдере объекта на котором этот скрипт висит .</summary>
    /// <param name="other"> Коллайдер входящего объекта.</param>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!isTeleporting)
            {
                if (destination != this.GetComponent<NpcPathPoint>())
                {
                    StartCoroutine(LoadScreen());
                    destination.GetComponentInChildren<InstancePortalCollider>().isTeleporting = true;
                    other.GetComponentInChildren<Rigidbody>().MovePosition(destination.Coordinate);
                }
            }
        }
    }

    /// <summary>Метод вызывающийся при выходе из коллайдера объекта на котором этот скрипт висит .</summary>
    /// <param name="other"> Коллайдер входящего объекта.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isTeleporting = false;
        }
    }

    
    /// <summary> Метод-корутина для включения загрузочного экрана.</summary>
    IEnumerator LoadScreen()
    {
        float Duration = 2;
        float LoadScreenTimer = 0;
        loadingScreen.SetActive(true);
        DeactivateObjects(_disableObjects);
        while (LoadScreenTimer < Duration)
        {
            //Debug.Log("CORUTINE STARTED");
            LoadScreenTimer++;
            yield return new WaitForSeconds(1f);
        }
        //Debug.Log("CORUTINE ENDED");
        loadingScreen.SetActive(false);
        ActivateObjects(_disableObjects);
    }

    private void DeactivateObjects(List<GameObject> objects)
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
    }
    private void ActivateObjects(List<GameObject> objects)
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(true);
        }
    }

}