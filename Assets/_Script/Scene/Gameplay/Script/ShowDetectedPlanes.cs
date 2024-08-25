using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowDetectedPlanes : MonoBehaviour
{
    ARPlaneManager _planeManager;
    ARSession ar_session;
    ARPointCloudManager pointCloud;

    [Header("ARPlane Material")]
    [SerializeField] private Material shadowMaterial;
    [SerializeField] private Material dotsMaterial;
    private bool useDotsMaterial = true; // Toggle flag

    // Start is called before the first frame update
    void Awake()
    {
        initial();
    }

    private void initial()
    {
        _planeManager = FindObjectOfType<ARPlaneManager>();
        ar_session = FindObjectOfType<ARSession>();
        pointCloud = FindObjectOfType<ARPointCloudManager>();

        // Set initial material for all existing planes
        foreach (ARPlane plane in _planeManager.trackables)
        {
            SetMaterial(plane);
        }

        // Subscribe to the plane added event
        _planeManager.planesChanged += OnPlanesChanged;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // Change the material of all newly detected planes
        foreach (ARPlane plane in args.added)
        {
            SetMaterial(plane);
        }
    }

    void SetMaterial(ARPlane plane)
    {
        // Get the MeshRenderer component of the ARPlane
        MeshRenderer meshRenderer = plane.GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            // Set the material based on the toggle flag
            meshRenderer.material = useDotsMaterial ? dotsMaterial : shadowMaterial;
        }
    }

    //switch between dots or shadow
    public void ShowDotsPlane(bool condition)
    {
        // Toggle between the two materials
        useDotsMaterial = condition;
        pointCloud.SetTrackablesActive(condition);

        // Update the material for all currently tracked planes
        foreach (ARPlane plane in _planeManager.trackables)
        {
            SetMaterial(plane);
        }
    }
}
