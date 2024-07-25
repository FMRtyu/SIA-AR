using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class WaterPumpHandler : MonoBehaviour
{
    /// <summary>
    /// Make Pipe enable disable
    /// </summary>
    bool isVisible;

    /// <summary>
    /// Change the transparency
    /// </summary>
    public Slider transparencySlider;

    /// <summary>
    /// Refernece for Pipes
    /// </summary>
    public GameObject [] pipes;

    /// <summary>
    /// Refernece for Sprite Material
    /// </summary>
    public Material spriteMaterial;

    /// <summary>
    /// Ref for AR Manager
    /// </summary>
    ARPlaneManager aRPlaneManager;

    private void Awake()
    {
        aRPlaneManager = FindObjectOfType<ARPlaneManager>();
        aRPlaneManager.requestedDetectionMode = PlaneDetectionMode.Horizontal;
    }

    void Start()
    {
        isVisible = true;
    }

    // Update is called once per frame
    void Update()
    {
        pipes[0].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, (byte)(transparencySlider.value * 255));
        spriteMaterial.color = new Color32(255, 255, 255, (byte)(transparencySlider.value * 255));
        pipes[0].GetComponent<Renderer>().material.renderQueue = 2000;
    }

    public void MakePipeVisible()
    {
        isVisible = !isVisible;
        foreach (GameObject pipeitems in pipes)
        {
            pipeitems.SetActive(isVisible);
        }
    }
}
