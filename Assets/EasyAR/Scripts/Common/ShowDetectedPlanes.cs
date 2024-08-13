using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowDetectedPlanes : MonoBehaviour
{
    /// <summary>
    /// ARPlaneManager
    /// </summary>
    ARPlaneManager _planeManager;
    ARSession ar_session;
    MultipleObjectPlacement allObject;

    public bool planeEnable = false;
    [SerializeField] private Material newMaterial;
    [SerializeField] private Material pointMaterial;

    /// <summary>
    /// Access the Shadow plane
    /// </summary>
    GameObject shadowPlane;

    // Start is called before the first frame update
    void Awake()
    {
        _planeManager = FindObjectOfType<ARPlaneManager>();
        ar_session = FindObjectOfType<ARSession>();
        allObject = FindObjectOfType<MultipleObjectPlacement>();

        shadowPlane = GameObject.FindWithTag("ShadowPlane");
    }

    // Update is called once per frame
    void Update()
    {
        if (planeEnable)
        {
            SetAllPlanesActive(true);
            
        }
        else
        {
            SetAllPlanesActive(false);
        }
    }

    void SetAllPlanesActive(bool value)
    {
        foreach (var plane in _planeManager.trackables)
            plane.gameObject.SetActive(value);
    }

    public void ShowPlanes()
    {
        planeEnable = true;

        MeshRenderer[] meshRenderer = _planeManager.GetComponentsInChildren<MeshRenderer>();
        if (meshRenderer != null)
        {
            foreach (var renderer in meshRenderer)
            {
                renderer.material = pointMaterial;
            }
        }
        _planeManager.enabled = true;
    }

    public void HidePlanes()
    {
        MeshRenderer[] meshRenderer = _planeManager.GetComponentsInChildren<MeshRenderer>();
        if (meshRenderer != null)
        {
            foreach (var renderer in meshRenderer)
            {
                renderer.material = newMaterial;
            }
        }
        _planeManager.enabled = false;

    }
    public void ResetPlanes()
    {
        ar_session.Reset();

        foreach (GameObject SpawnedObject in allObject.allSpawnedObject)
        {
            Destroy(SpawnedObject);
        }
        ShowPlanes();
    }
}
