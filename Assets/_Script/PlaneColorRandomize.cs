using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaneColorRandomize : MonoBehaviour
{
    [Header("ARPlane Color Material")]
    [Tooltip("Color cycling effect for horizontal planes")]
    [SerializeField] private Material[] horizontalPlaneMaterial;
    private int horizontalColorIndex = 0;

    [Tooltip("Color cycling effect for vertical planes")]
    [SerializeField] private Material[] verticalPlaneMaterial;
    private int verticalColorIndex = 0;

    [Header("ARPlane Shadow Material")]
    [Tooltip("Material for shadow-casting planes.")]
    [SerializeField] private Material shadowMaterial;

    // Stores materials assigned to detected AR planes to prevent reapplying the same material.
    private Dictionary<TrackableId, Material> detectedPlaneMaterials = new Dictionary<TrackableId, Material>();

    private ARPlaneManager planeManager;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    private void Setup()
    {
        //try to get AR Plane script
        try
        {
            planeManager = FindAnyObjectByType<ARPlaneManager>();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }

        //Subsribe to AR Plane for every changes
        planeManager.planesChanged += PlaneTextUpdater;
    }

    private void PlaneTextUpdater(ARPlanesChangedEventArgs eventArgs)
    {
        foreach (var plane in eventArgs.updated)
        {
            //set if plane horizontal
            if (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.HorizontalDown)
            {
                if (horizontalColorIndex < horizontalPlaneMaterial.Length)
                {
                    if (!detectedPlaneMaterials.ContainsKey(plane.trackableId))
                    {
                        plane.GetComponent<MeshRenderer>().material = horizontalPlaneMaterial[horizontalColorIndex];
                        detectedPlaneMaterials[plane.trackableId] = horizontalPlaneMaterial[horizontalColorIndex];
                        horizontalColorIndex++;
                    }
                }
                else
                {
                    if (!detectedPlaneMaterials.ContainsKey(plane.trackableId))
                    {
                        horizontalColorIndex = 0;
                        plane.GetComponent<MeshRenderer>().material = horizontalPlaneMaterial[horizontalColorIndex];
                        detectedPlaneMaterials[plane.trackableId] = horizontalPlaneMaterial[horizontalColorIndex];
                        horizontalColorIndex++;
                    }
                }
            }
            //set if plane Vertical
            else if (plane.alignment == PlaneAlignment.Vertical)
            {
                if (verticalColorIndex < verticalPlaneMaterial.Length)
                {
                    if (!detectedPlaneMaterials.ContainsKey(plane.trackableId))
                    {
                        plane.GetComponent<MeshRenderer>().material = verticalPlaneMaterial[verticalColorIndex];
                        detectedPlaneMaterials[plane.trackableId] = verticalPlaneMaterial[verticalColorIndex];
                        verticalColorIndex++;
                    }
                }
                else
                {
                    if (!detectedPlaneMaterials.ContainsKey(plane.trackableId))
                    {
                        verticalColorIndex = 0;
                        plane.GetComponent<MeshRenderer>().material = verticalPlaneMaterial[verticalColorIndex];
                        detectedPlaneMaterials[plane.trackableId] = verticalPlaneMaterial[verticalColorIndex];
                        verticalColorIndex++;
                    }
                }
            }
        }
    }

    public void ShowPlaneColor(bool showColor)
    {
        horizontalColorIndex = 0;
        verticalColorIndex = 0;
        foreach (var plane in planeManager.trackables)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.HorizontalDown)
            {
                plane.GetComponent<MeshRenderer>().material = showColor ? horizontalPlaneMaterial[horizontalColorIndex] : shadowMaterial;

                horizontalColorIndex++;
                if (horizontalColorIndex >= horizontalPlaneMaterial.Length)
                {
                    horizontalColorIndex = 0;
                }
            }
            else
            {
                plane.GetComponent<MeshRenderer>().material = showColor ? verticalPlaneMaterial[verticalColorIndex] : shadowMaterial;
                
                verticalColorIndex++;
                if (verticalColorIndex >= verticalPlaneMaterial.Length)
                {
                    verticalColorIndex = 0;
                }
            }
        }
    }
}
