using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoundaryMGR : MonoBehaviour
{
    public GameObject placementIndicator;

    [SerializeField]
    private ARSessionOrigin arSessionOrigin;
    [SerializeField]
    private ARRaycastManager arRaycastManager;
    [SerializeField]
    private LineController line;
    [SerializeField]
    private ShapeCreator shape;

    private bool setShapeActive = false;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private Vector3 previousPose = new Vector3(0, 0, 0);
    private float distance = 0;
    private int numOfPoints = 0;
    List<int> trangulation = new List<int>();
    private List<Transform> pointList = new List<Transform>();

    [SerializeField]
    TrackableType trackableType = TrackableType.PlaneWithinPolygon;

    //UI
    public Button goButton;
    public GameObject place;
    public GameObject go;
    public GameObject back;
    public GameObject startPanel;
    public GameObject lineRenderer;
    LineRenderer lineRendererCom;
    public Material lineRendererShadowMtl;
    private float scalingFactor = 2f;
    private float finalScale = 0;
    private GameObject arCamera;

    public GameObject objectToPlace;
    private float startingPointHeight = 0;
    /// <summary>
    /// Ref for AR Manager
    /// </summary>
    ARPlaneManager aRPlaneManager;

    private void Awake()
    {
        aRPlaneManager = FindObjectOfType<ARPlaneManager>();
        aRPlaneManager.requestedDetectionMode = PlaneDetectionMode.Horizontal;
    }

    private void Start()
    {
        back.SetActive(true);
        go.SetActive(true);
        lineRenderer.SetActive(true);
        goButton.interactable = false;
        arCamera = GameObject.FindWithTag("MainCamera");
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        UpdateDistanceFromCamera();
    }

    public void PlaceObject()
    {

        if (placementPoseIsValid)
        {
            GameObject point;

            if (numOfPoints == 0)
            {
                point = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
                startingPointHeight = point.transform.position.y;
            }
            else
            {
                point = Instantiate(objectToPlace, new Vector3(placementPose.position.x, startingPointHeight, placementPose.position.z), placementPose.rotation);
            }

            pointList.Add(point.transform);
            numOfPoints += 1;

            if (numOfPoints == 3)
            {

                goButton.interactable = true;
            }

            if (numOfPoints > 2)
            {

                CheckTrangulation(numOfPoints - 1);
            }

            distance = Vector3.Distance(previousPose, placementPose.position);

            line.SetupLine(pointList.ToArray());


            previousPose = placementPose.position;
        }
    }

    void UpdateDistanceFromCamera()
    {
        float cameraDistance = Vector3.Distance(arCamera.transform.position, placementPose.position);
        finalScale = cameraDistance * scalingFactor;
        placementIndicator.transform.localScale = new Vector3(finalScale / 2, finalScale / 2, finalScale / 2);
    }
    public void GoBack()
    {
        goButton.interactable = false;
        place.SetActive(true);
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
        SceneManager.LoadScene("ARTilling");
        placementIndicator.SetActive(true);
    }

    public void Clear()
    {
        SceneManager.LoadScene("ARTilling");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        LoaderUtility.Deinitialize();
    }

    public void SetShape()
    {
        setShapeActive = true;
        shape.SetupShape(pointList.ToArray());
        shape.Trangulator(trangulation.ToArray());
        place.SetActive(false);
        lineRendererCom = lineRenderer.GetComponent<LineRenderer>();
        lineRendererCom.SetWidth(0.02f, 0.02f);
        lineRendererCom.material = lineRendererShadowMtl;
        GameObject[] targets = GameObject.FindGameObjectsWithTag("TileTargets");
        foreach (GameObject tar in targets)
        {
            tar.SetActive(false);
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid && !setShapeActive)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = arSessionOrigin.camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arRaycastManager.Raycast(screenCenter, hits, trackableType);
        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose; 
            startPanel.SetActive(false);
        }
    }

    private void CheckTrangulation(int point)
    {

        if (pointList[2].position.x > pointList[0].position.x)
        {
            trangulation.Add(0);
            for (int i = 1; i >= 0; i--)
            {
                trangulation.Add(point - i);
            }
        }
        else
        {
            trangulation.Add(0);
            for (int i = 0; i < 2; i++)
            {
                trangulation.Add(point - i);
            }
        }

    }

}