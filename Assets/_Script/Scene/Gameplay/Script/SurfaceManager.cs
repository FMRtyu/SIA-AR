using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SIAairportSecurity.Training;

public class SurfaceManager : MonoBehaviour
{
    private GamePlayController _gamePlayController;

    [Header("AR Property")]
    [SerializeField] private ARSession _arSession;
    [SerializeField] private ARPlaneManager _planeManager;

    [Header("ARPlane Material")]
    [SerializeField] private Material shadowMaterial;
    [SerializeField] private Material dotsMaterial;
    [SerializeField] private Material[] colorMaterials;
    [SerializeField] private Material verticalPlaneMaterial;

    private int horizontalPlaneCount = 0;
    private int verticalPlaneCount = 0;

    private Dictionary<TrackableId, Material> planeColors = new Dictionary<TrackableId, Material>();

    private int colorIndex = 0;
    private bool useDotsMaterial = true; // Toggle flag

    bool isPlaneScan = true;

    // Start is called before the first frame update
    void Awake()
    {
        initial();
    }

    private void initial()
    {
        _gamePlayController = GetComponent<GamePlayController>();

        _arSession = FindObjectOfType<ARSession>();

        // Set initial material for all existing planes
        foreach (ARPlane plane in _planeManager.trackables)
        {
            SetMaterial(plane);
        }

        _planeManager.planesChanged += PlaneTextUpdater;
    }

    private void PlaneTextUpdater(ARPlanesChangedEventArgs eventArgs)
    {
        // Reset counts
        horizontalPlaneCount = 0;
        verticalPlaneCount = 0;
        foreach (var plane in eventArgs.updated)
        {

            if (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.HorizontalDown)
            {
                if (colorIndex < colorMaterials.Length)
                {
                    if (!planeColors.ContainsKey(plane.trackableId))
                    {
                        plane.GetComponent<MeshRenderer>().material = colorMaterials[colorIndex];
                        planeColors[plane.trackableId] = colorMaterials[colorIndex];
                        colorIndex++;
                    }
                }
                else
                {
                    if (!planeColors.ContainsKey(plane.trackableId))
                    {
                        colorIndex = 0;
                        plane.GetComponent<MeshRenderer>().material = colorMaterials[colorIndex];
                        planeColors[plane.trackableId] = colorMaterials[colorIndex];
                        colorIndex++;
                    }
                }
            }
            else if (plane.alignment == PlaneAlignment.Vertical)
            {

                plane.GetComponent<MeshRenderer>().material = verticalPlaneMaterial;
            }
        }

        foreach (var plane in _planeManager.trackables)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.HorizontalDown)
            {
                horizontalPlaneCount++;
            }
            else if (plane.alignment == PlaneAlignment.Vertical)
            {
                verticalPlaneCount++;
            }
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
        int colorIndex = 0;
        foreach (var plane in _planeManager.trackables)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.HorizontalDown)
            {
                plane.GetComponent<MeshRenderer>().material = condition ? colorMaterials[colorIndex] : shadowMaterial;
            }
            else
            {
                plane.GetComponent<MeshRenderer>().material = condition ? verticalPlaneMaterial : shadowMaterial;

            }
            colorIndex++;
            if (colorIndex >= colorMaterials.Length)
            {
                colorIndex = 0;
            }
        }
    }

    public void StartStopScanning()
    {
        isPlaneScan = !isPlaneScan;
        _planeManager.enabled = isPlaneScan;
    }

    public void StartStopScanning(bool condition)
    {
        isPlaneScan = condition;
        _planeManager.enabled = isPlaneScan;
    }

    public void ResetPlane()
    {
        _arSession.Reset();
        isPlaneScan = true;
        _planeManager.enabled = isPlaneScan;
    }
}
