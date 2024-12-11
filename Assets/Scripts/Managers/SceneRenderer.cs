using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneRenderer : MonoBehaviour
{
    [SerializeField] List<Scene> scenes;
    public void Start()
    {
        foreach (Scene s in scenes)
        {
                MeshRenderer[] mrs = s.gameObject.GetComponentsInChildren<MeshRenderer>();
                foreach (var mr in mrs)
                {
                    mr.enabled = false;
                }          
        }
    }
    /// <summary>Метод для включения мешей на сцене .</summary>
    /// <param name="sceneName"> . Имя сцены</param>
    public void SceneRenderOn(string sceneName)
    {
        Debug.Log(sceneName + " Render On");
        foreach (Scene s in scenes)
        {
            if (s.name == sceneName)
            {
                MeshRenderer[] mrs = s.gameObject.GetComponentsInChildren<MeshRenderer>();
                foreach (var mr in mrs)
                {
                    Collider col = mr.GetComponentInChildren<Collider>();
                    if (col == null)
                    {
                        mr.enabled = true;
                    }
                }
            }
        }
    }

    /// <summary>Метод для отключения мешей на сцене .</summary>
    /// <param name="sceneName"> . Имя сцены</param>
    public void SceneRenderOff(string sceneName)
    {
        Debug.Log(sceneName + " Render Off");
        foreach (Scene s in scenes)
        {
            if (s.name == sceneName)
            {
                MeshRenderer[] mrs = s.gameObject.GetComponentsInChildren<MeshRenderer>();
                foreach (var mr in mrs)
                {
                    mr.enabled = false;
                }
            }
        }
    }
}