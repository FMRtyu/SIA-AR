
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Getting Initial Data for handlers
/// </summary>
public class InitialData : MonoBehaviour
{
    /// <summary>
    /// Game Object To place 
    /// </summary>
    public static GameObject SpawningObject;

    /// <summary>
    /// Value return true if scene is a single object placement
    /// </summary>
    public static bool _singleObjectPlacement;

    public void ShowPrefabInARView(GameObject spwaningObject)
    {
        SpawningObject = spwaningObject;
        SceneHandler.GoToNextView();
    }

    public void ARConfigurator(GameObject spwaningObject)
    {
        SpawningObject = spwaningObject;
        SceneHandler.ARConfigurator();
    }
}
