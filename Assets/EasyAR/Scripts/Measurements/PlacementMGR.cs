using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlacementMGR : MonoBehaviour
{
  
    [SerializeField]
    private ARSessionOrigin arSessionOrigin;

    [SerializeField]
    private ARRaycastManager arRaycastManager;
    private LineRenderer lineRenderer;

    [SerializeField]
    private GameObject indicator;

    [SerializeField]
    TrackableType trackableType = TrackableType.PlaneWithinPolygon;

    [SerializeField]
    private GameObject warningText;

    private bool placementPoseIsValid = false;
    private bool lineEnable = false;
    private bool isVerticle = false;

    private float distanceFromPrev = 0;
    private float distance = 0;
    private float scalingFactor = 2f;
    private float finalScale = 0;

    private Vector3 pointSize;
    private Vector3 midPointSize;
    private Vector3 mid = new Vector3(0, 0, 0);
    private Vector3 previousPose = new Vector3(0, 0, 0);
    private string disText = "";
   
    //private Transform[] points;
    private int numOfPoints = 0;

    List<Transform> pointList = new List<Transform>();
    List<GameObject> midPoints = new List<GameObject>();
    List<float> distancetoCamera = new List<float>();

    public Pose placementPose;
    public Text distanceText;
    public GameObject placementIndicator;
    public GameObject objectToPlace;
    public GameObject midPointObject;
    public GameObject arCamera;
    private GameObject placementObj;
    //UI
    public GameObject measurementText;

    /// <summary>
    /// Access the percentage scanSurface prefab
    /// </summary>
    GameObject scanSurface;

    private void Start()
    {
        placementObj = GameObject.FindWithTag("Placement");
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        scanSurface = GameObject.FindWithTag("ScanSurfaceAnim");
        scanSurface.SetActive(true);
    }

    void Update()
    {

        UpdatePlacementPose();
        UpdatePlacementIndicator();
        UpdateDistanceFromCamera();
        if (numOfPoints > 0 && lineEnable)
        {
            UpdateMeasurement();
        }
        else if (numOfPoints > 0 && !lineEnable)
        {
            float distanceUpdated = Vector3.Distance(previousPose, pointList[numOfPoints - 2].position) * (float)39.3700787;
            if (distanceUpdated > 36)
            {
                int quotient = (int)distanceUpdated / 12;
                int mod = (int)distanceUpdated % 12;
                distanceText.text = quotient.ToString() + "'" + " " + mod.ToString() + '"';
            }
            else
            {
                distanceText.text = distanceUpdated.ToString("F1") + '"';
            }
            midPoints[numOfPoints - 1].transform.GetChild(0).gameObject.SetActive(false);
        }


    }

    void UpdateMeasurement()
    {
        distance = Vector3.Distance(previousPose, placementPose.position) * (float)39.3700787;
        int quotient = (int)distance / 12;
        int mod = (int)distance % 12;

        mid = midPoints[numOfPoints - 1].transform.position;
        mid.x = previousPose.x + (indicator.transform.position.x - previousPose.x) / 2;
        mid.y = previousPose.y + (indicator.transform.position.y - previousPose.y) / 2 + 0.001f;
        mid.z = previousPose.z + (indicator.transform.position.z - previousPose.z) / 2;
        midPoints[numOfPoints - 1].transform.position = mid;

        if (distance > 36)
        {
            disText = quotient.ToString() + "'" + " " + mod.ToString() + '"';
        }
        else
        {
            disText = distance.ToString("F1") + '"';
        }

        distanceText.text = disText;
        midPoints[numOfPoints - 1].GetComponent<MeasurementPlacement>().changeMeasurement(disText);

        lineRenderer.SetPosition(1, indicator.transform.position);
    }

    public void PlaceObject()
    {

        if (placementPoseIsValid)
        {

            lineEnable = !lineEnable;

            GameObject newPoint;
            GameObject midPoint;

            newPoint = Instantiate(objectToPlace, indicator.transform.position, placementPose.rotation);
            midPoint = Instantiate(midPointObject, indicator.transform.position, placementPose.rotation);
            distancetoCamera.Add(Vector3.Distance(arCamera.transform.position, midPoint.transform.position));

            if (numOfPoints > 1)
            {

                newPoint.transform.localScale = pointSize;
                midPoint.transform.localScale = indicator.transform.localScale * 6;

            }
            else
            {

                newPoint.transform.localScale = indicator.transform.localScale;
                midPoint.transform.localScale = indicator.transform.localScale * 6;

                pointSize = newPoint.transform.localScale;
                midPointSize = midPoint.transform.localScale;
            }


            midPoints.Add(midPoint);
            pointList.Add(newPoint.transform);


            numOfPoints += 1;


            distanceFromPrev = Vector3.Distance(previousPose, indicator.transform.position);

            if (numOfPoints > 0)
            {
                measurementText.SetActive(true);
            }


            if (!lineEnable)
            {
                midPoints[numOfPoints - 1].GetComponent<LineHandler>().AddLine(previousPose, indicator.transform.position);
            }

            previousPose = pointList[numOfPoints - 1].position;
            lineRenderer.SetPosition(0, previousPose);

        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.position = placementPose.position;
            placementIndicator.transform.rotation = placementPose.rotation;

        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    public void Clear()
    {
        measurementText.SetActive(false);
        SceneManager.LoadScene("ARMeasurement");
    }

    public void Back()
    {
        measurementText.SetActive(false);
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        LoaderUtility.Deinitialize();
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = arSessionOrigin.camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arRaycastManager.Raycast(screenCenter, hits, trackableType);

        placementPoseIsValid = hits.Count > 0;

        if (hits[0].trackable.transform.eulerAngles.x == 0 && hits[0].trackable.transform.eulerAngles.z == 0)
        {
            isVerticle = false;
        }
        else
        {
            isVerticle = true;
        }

        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
            scanSurface.SetActive(false);
        }
    }

    void UpdateDistanceFromCamera()
    {
        float cameraDistance = Vector3.Distance(arCamera.transform.position, placementPose.position);
        finalScale = cameraDistance * scalingFactor;
        placementIndicator.transform.localScale = new Vector3(finalScale, finalScale, finalScale);
        placementIndicator.GetComponent<SphereCollider>().radius = finalScale / 50;

        indicator.transform.localScale = new Vector3(finalScale / 200, finalScale / 200, finalScale / 200);


        if (cameraDistance > 2)
        {
            StartCoroutine(ShowWarningText());
            indicator.SetActive(false);
            placementIndicator.SetActive(false);
        }
        else
        {

            indicator.SetActive(true);
            warningText.SetActive(false);
            placementIndicator.SetActive(true);
        }
    }


    IEnumerator ShowWarningText()
    {
        float currentMovementTime = 0f;//The amount of time that has passed
        while (currentMovementTime < 1f)
        {
            currentMovementTime += Time.deltaTime;
            warningText.SetActive(true);
            yield return null;
        }
    }

}