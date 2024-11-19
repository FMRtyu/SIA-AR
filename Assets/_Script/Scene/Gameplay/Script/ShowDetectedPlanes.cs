using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SIAairportSecurity.Training;

public class ShowDetectedPlanes : MonoBehaviour
{
    ARSession ar_session;
    ARPointCloudManager pointCloud;

    [Header("ARPlane Material")]
    [SerializeField] private Material shadowMaterial;
    [SerializeField] private Material dotsMaterial;
    private bool useDotsMaterial = true; // Toggle flag

    //temp
    bool isPlaneScan = true;

    private GamePlayController _gamePlayController;

    private Dictionary<TrackableId, Material> planeColors = new Dictionary<TrackableId, Material>();
    // Start is called before the first frame update
    void Awake()
    {
        initial();
    }

    private void initial()
    {
        _gamePlayController = GetComponent<GamePlayController>();

        ar_session = FindObjectOfType<ARSession>();
        pointCloud = FindObjectOfType<ARPointCloudManager>();

        // Set initial material for all existing planes
        foreach (ARPlane plane in _gamePlayController.GetARPlaneManager().trackables)
        {
            SetMaterial(plane);
        }

        // Subscribe to the plane added event
        _gamePlayController.GetARPlaneManager().planesChanged += OnPlanesChanged;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // Change the material of all newly detected planes
        foreach (ARPlane plane in args.updated)
        {
            //SetMaterial(plane);
            planeColors[plane.trackableId] = plane.GetComponent<MeshRenderer>().material;
            //plane.GetComponent<DualSurfacePlane>().ChangeSecondSurfaceMaterial(useDotsMaterial ? dotsMaterial : shadowMaterial);
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
        foreach (var plane in _gamePlayController.GetARPlaneManager().trackables)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.HorizontalDown)
            {
                plane.GetComponent<MeshRenderer>().material = condition ? _gamePlayController.colorMaterials[colorIndex] : shadowMaterial;
            }
            else
            {
                plane.GetComponent<MeshRenderer>().material = condition ? _gamePlayController.verticalPlaneMaterial : shadowMaterial;

            }
            colorIndex++;
            if (colorIndex >= _gamePlayController.colorMaterials.Length)
            {
                colorIndex = 0;
            }
        }
        //// Toggle between the two materials
        //useDotsMaterial = condition;
        //pointCloud.SetTrackablesActive(condition);

        //// Update the material for all currently tracked planes
        //foreach (ARPlane plane in _gamePlayController.GetARPlaneManager().trackables)
        //{
        //    SetMaterial(plane);

        //    //if (!condition)
        //    //{
        //    //    plane.GetComponent<DualSurfacePlane>().CreateSecondSurface();
        //    //}
        //    //else
        //    //{
        //    //    foreach (Transform child in plane.gameObject.transform)
        //    //    {
        //    //        Destroy(child.gameObject);
        //    //    }
        //    //}
        //}
    }

    public void StartStopScanning(Button button)
    {
        isPlaneScan = !isPlaneScan;
        _gamePlayController.GetARPlaneManager().enabled = isPlaneScan;

        StartCoroutine(ButtonDelay.EnabledBTNAfterSecond(button));
    }

    public void StartStopScanning(bool condition)
    {
        isPlaneScan = condition;
        _gamePlayController.GetARPlaneManager().enabled = isPlaneScan;
    }

    public void ResetPlane()
    {
        ar_session.Reset();
        isPlaneScan = true;
        _gamePlayController.GetARPlaneManager().enabled = isPlaneScan;
    }

    public bool CheckARPlaneScanned()
    {
        if (_gamePlayController.GetARPlaneManager().trackables.count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
