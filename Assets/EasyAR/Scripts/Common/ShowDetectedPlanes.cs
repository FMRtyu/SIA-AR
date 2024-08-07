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
    ARPlaneManager m_plane;
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
        m_plane = FindObjectOfType<ARPlaneManager>();
        ar_session = FindObjectOfType<ARSession>();
        allObject = FindObjectOfType<MultipleObjectPlacement>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (shadowPlane == null)
        {
            shadowPlane = GameObject.FindWithTag("ShadowPlane");
        }

        if (planeEnable)
        {
            SetAllPlanesActive(true);
            if (shadowPlane != null)
            {
                shadowPlane.SetActive(false);
            }
            
        }
        else
        {
            SetAllPlanesActive(false);
            
            if (shadowPlane != null)
            {
                shadowPlane.SetActive(true);
            }
        }
    }

    void SetAllPlanesActive(bool value)
    {
        foreach (var plane in m_plane.trackables)
            plane.gameObject.SetActive(value);
    }

    public void ShowPlanes()
    {
        planeEnable = true;

        MeshRenderer[] meshRenderer = m_plane.GetComponentsInChildren<MeshRenderer>();
        if (meshRenderer != null)
        {
            foreach (var renderer in meshRenderer)
            {
                renderer.material = pointMaterial;
            }
        }
    }

    public void HidePlanes()
    {
        MeshRenderer[] meshRenderer = m_plane.GetComponentsInChildren<MeshRenderer>();
        if (meshRenderer != null)
        {
            foreach (var renderer in meshRenderer)
            {
                renderer.material = newMaterial;
            }
        }

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
