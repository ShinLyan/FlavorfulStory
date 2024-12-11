using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Класс, отвечающий за постановку на паузу.
/// </summary>
public class PauseHandler : MonoBehaviour
{
    [Header("Objects")]
    /// <summary>Список объектов UI объектов, которые открываются при паузе.</summary>
    [SerializeField] private List<GameObject> _enableObjects;
    /// <summary>Список объектов UI объектов, которые скрываются при паузе.</summary>
    [SerializeField] private List<GameObject> _disableObjects;
    ///<summary>Переменная состояния паузы.</summary>
    private bool _isPaused;
    ///<summary>По умолчанию игра не на паузе.</summary>
    private void Start()
    {
        _isPaused = false;
    }
    ///<summary>Считывания нажатия кнопки ESC. </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPause();
        }
    }
    ///<summary>Метод для включения/выключения объектов UI при паузе.</summary>
    private void SetPause()
    {
        if (!_isPaused)
        {
            ActivateObjects(_enableObjects);
            DeactivateObjects(_disableObjects);
            Time.timeScale = 0;
            _isPaused = true;
        }
        else if (_isPaused)
        {
            ActivateObjects(_disableObjects);
            DeactivateObjects(_enableObjects);
            Time.timeScale = 1;
            _isPaused = false;
        }
    }

    // DELETE
    private void ActivateObjects(List<GameObject> objects)
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(true);
        }
    }

    // DELETE
    private void DeactivateObjects(List<GameObject> objects)
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
    }
}