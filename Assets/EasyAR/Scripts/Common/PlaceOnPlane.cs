using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using System.Linq;

/// <summary>
/// Required components for this component.
/// </summary>

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
[RequireComponent(typeof(InitialData))]
public class PlaceOnPlane : MonoBehaviour
{
    /// <summary>
    /// Spawning object in the AR view
    /// </summary>
    public static GameObject spawnedObject;
    /// <summary>
    /// Max value for keep the object within the clipping values
    /// </summary>
    float MaxScaleNumber;

    /// <summary>
    /// Access the AR camera
    /// </summary>
    GameObject ArCamera;


    /// <summary>
    /// To check Ar object placed or not 
    /// </summary>
    public static bool isObjectPlaced = false;

    /// <summary>
    /// Initial scale of the Spawned Object
    /// </summary>
    static Vector3 initialScale;

    /// <summary>
    /// Previous rotation of the spawned Object
    /// </summary>
    Quaternion previousRotation;

    /// <summary>
    /// Initial rotation of the spawned object
    /// </summary>
    Quaternion InitialRotation;

    /// <summary>
    /// Previuos position of the spawned object
    /// </summary>
    Vector3 previousPosition;

    /// <summary>
    /// Previuos Scale of the spawned object
    /// </summary>
    Vector3 previousScale;

    /// <summary>
    /// Time to delay the object appear infront of the camera
    /// </summary>
    float time = 0;

    /// <summary>
    /// Hits of the AR raycast
    /// </summary>
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    /// <summary>
    /// Access the AR raycast Manager
    /// </summary>
    ARRaycastManager m_RaycastManager;


    /// <summary>
    /// To check positioning condition
    /// </summary>
    bool isPositioning = false;

    /// <summary>
    /// To check Multiple touches
    /// </summary>
    bool gotMultipleTouchs = false;

    /// <summary>
    /// Object Start position
    /// </summary>
    public Vector3 startMarker;

    /// <summary>
    /// Object destination position
    /// </summary>
    public Vector3 endMarker;

    /// <summary>
    /// Movement speed in units per second.
    /// </summary>
    public float speed = 100f;

    /// <summary>
    ///     Time when the movement started.
    /// </summary>
    private float startTime;

    /// <summary>
    /// Total distance between the markers.
    /// </summary>
    private float journeyLength;

    /// <summary>
    /// Final Rotation of the object
    /// </summary>
    Quaternion toRotation;

    /// <summary>
    /// Starting rotaion of the object
    /// </summary>
    Quaternion FromRotation;

    /// <summary>
    /// Rotating speed in units per second.
    /// </summary>
    float speedR = 2.5f;

    /// <summary>
    /// To check object still rotating or not
    /// </summary>
    bool rotate = false;

    /// <summary>
    /// To check object still Moving to destination
    /// </summary>
    bool wentToPosition = false;


    /// <summary>
    /// Access shadow plane
    /// </summary>
    PrefabMaterialHandler prefabMaterialHandler;

    /// <summary>
    /// Inintial Position for Dragging
    /// </summary>
    Vector2 initialPosition = new Vector2(0, 0);

    /// <summary>
    /// current Position for Dragging
    /// </summary>
    Vector3 ObjectScreenPosition = new Vector2(0, 0);
    /// <summary>
    /// Difference between Inintial Position 
    /// </summary>
    Vector2 DistanceDifference = new Vector2(0, 0);

    /// <summary>
    /// Access the percentage indicator
    /// </summary>
    public static GameObject percentageIndicator;

    public GameObject qualityControlButton;

    /// <summary>
    /// Ref for AR Manager
    /// </summary>
    ARPlaneManager aRPlaneManager;

    private List<SpawningObj> spawningObj = new List<SpawningObj>();

    void Start()
    {
        InitialData._singleObjectPlacement = true;
        aRPlaneManager = FindObjectOfType<ARPlaneManager>();
        ArCamera = GameObject.FindWithTag("MainCamera");
        m_RaycastManager = GetComponent<ARRaycastManager>();
        GameObject.FindWithTag("ScanSurfaceAnim").SetActive(true);
        isObjectPlaced = false;
        spawnedObject = Instantiate(InitialData.SpawningObject);

        MeshRenderer[] allmeshRenders = spawnedObject.GetComponentsInChildren<MeshRenderer>();

        if(spawnedObject.GetComponent<SpawningObjectDetails>().enableARQualityControl == true)
        {
            qualityControlButton.SetActive(true);
        }
        else
        {
            qualityControlButton.SetActive(false);
        }
      
        foreach (MeshRenderer mRender in allmeshRenders)
        {
            SpawningObj spj = new SpawningObj();
            spj.meshRenderer = mRender;
            Material[] mats = mRender.materials;
            int x = 0;
            spj.shaders = new Shader[mRender.materials.Length];
            foreach (Material m in mats) {
                if (m.shader.name != "AR/Occlusion")
                {
                    spj.shaders[x] = m.shader;
                    x++;
                    m.shader = (Shader)Resources.Load("TransparentShader", typeof(Shader));
                }
            }

            spawningObj.Add(spj);
        }

        aRPlaneManager.requestedDetectionMode = spawnedObject.GetComponent<SpawningObjectDetails>().planeDetectionMode;
        spawnedObject.GetComponent<SpawningObjectDetails>().scalePersentageIndicator.transform.localScale = spawnedObject.GetComponent<Collider>().bounds.size * 0.0015f;
        spawnedObject.GetComponent<SpawningObjectDetails>().scalePersentageIndicator.transform.position = new Vector3(0, spawnedObject.GetComponent<Collider>().bounds.size.y * 1.2f, 0);
        initialScale = spawnedObject.transform.localScale;
        spawnedObject.GetComponent<SpawningObjectDetails>().initialScale = initialScale;
        keepObjectWithInClippingValue(spawnedObject);
        spawnedObject.transform.parent = ArCamera.transform.transform;
        InitialRotation = spawnedObject.transform.rotation;
        spawnedObject.transform.position = ArCamera.transform.position + new Vector3(0, 0, 0.3f);
        spawnedObject.SetActive(false);
        delayToShowSpawnedObject();
    }

    private void GetWallPlacement(ARRaycastHit _planeHit, out Quaternion orientation, out Quaternion zUp)
    {
        TrackableId planeHit_ID = _planeHit.trackableId;
        ARPlane planeHit = aRPlaneManager.GetPlane(planeHit_ID);
        Vector3 planeNormal = planeHit.normal;
        orientation = Quaternion.FromToRotation(Vector3.up, planeNormal);
        Vector3 forward = _planeHit.pose.position - (_planeHit.pose.position + Vector3.down);
        zUp = Quaternion.LookRotation(forward, planeNormal);
    }
    void Update()
    {
        if (!isObjectPlaced)
        {
            delayToShowSpawnedObject();
            Vector3 rayEmitPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            if (m_RaycastManager.Raycast(rayEmitPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = s_Hits[0].pose;
                FromRotation = spawnedObject.transform.rotation;
                if (aRPlaneManager.requestedDetectionMode == PlaneDetectionMode.Vertical)
                {
                    Quaternion orientation = Quaternion.identity;
                    Quaternion zUp = Quaternion.identity;
                    GetWallPlacement(s_Hits[0], out orientation, out zUp);
                    spawnedObject.transform.rotation = zUp;
                    toRotation = Quaternion.Euler(spawnedObject.transform.eulerAngles.x, spawnedObject.transform.rotation.eulerAngles.y, 0);
                }
                else
                {
                    toRotation = Quaternion.Euler(0, spawnedObject.transform.rotation.eulerAngles.y, 0);
                }

                spawnedObject.transform.parent = null;
                spawnedObject.transform.localScale = initialScale;
                spawnedObject.GetComponent<SpawningObjectDetails>().initialScale = spawnedObject.transform.localScale;
                startTime = Time.time;
                startMarker = spawnedObject.transform.position;
                endMarker = hitPose.position;
                wentToPosition = true;
                journeyLength = Vector3.Distance(startMarker, endMarker);
                rotate = true;
                previousRotation = hitPose.rotation;
                previousPosition = hitPose.position;
                previousScale = spawnedObject.transform.localScale;
                isObjectPlaced = true;

                if (spawnedObject.GetComponent<SpawningObjectDetails>().enableShadowPlane)
                    spawnedObject.GetComponent<SpawningObjectDetails>().shadowPlane.SetActive(true);

                GameObject.FindWithTag("ScanSurfaceAnim").SetActive(false);
                MeshRenderer[] allmeshRenders = spawnedObject.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer mRender in allmeshRenders)
                {
                    Material[] mats = mRender.materials;
                    SpawningObj temp = spawningObj.Where(obj => obj.meshRenderer.name == mRender.name).SingleOrDefault();
                   
                    int x = 0;
             
                    foreach (Material m in mats) {
                        if (m.shader.name != "AR/Occlusion") {
                            m.shader = temp.shaders[x];
                            x++;
                        }
                    }

                    spawningObj.Remove(temp);
                }
                }
        }
        else
        {
            //Finding the Dragging position 
            if (TouchIndicatorHandler.isTouchedTheObject && (Input.touchCount < 2) && !gotMultipleTouchs)
            {
                if (!TryGetTouchPosition(out Vector2 touchPosition))
                    return;

                if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon) && !IsPointerOverUIObject())
                {
                    if (isPositioning)
                    {
                        var hitPose = s_Hits[0].pose;
                        if (TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().enbleDragFeature)
                            TouchIndicatorHandler.hitObject.transform.position = hitPose.position;
                        previousPosition = hitPose.position;

                    }
                }
            }
        }
        if (TouchIndicatorHandler.isTouchedTheObject)
        {
            TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().scalePersentageIndicator.transform.rotation = Quaternion.Euler(ArCamera.transform.rotation.eulerAngles.x, ArCamera.transform.rotation.eulerAngles.y, 0);
        }


        MultipleTouchHandler();
        freezePositionWhenRotate();
        SendObjectToDetectedPosition();
    }



    void MultipleTouchHandler()
    {
        if (Input.touchCount == 0)
        {
            gotMultipleTouchs = false;
            DistanceDifference = new Vector2(0, 0);
        }
        else if (Input.touchCount > 1)
        {
            gotMultipleTouchs = true;
            DistanceDifference = new Vector2(0, 0);
        }
    }

    /// <summary>
    /// To Get the touch position
    /// </summary>
    /// <param name="touchPosition"></param>
    /// <returns></returns>
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {

        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Stationary)
            {
                initialPosition = Input.GetTouch(0).position;
                ObjectScreenPosition = ArCamera.GetComponent<Camera>().WorldToScreenPoint(spawnedObject.transform.position);
                DistanceDifference = new Vector2(ObjectScreenPosition.x, ObjectScreenPosition.y) - initialPosition;
                touchPosition = Input.GetTouch(0).position + DistanceDifference;
                return true;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (DistanceDifference != new Vector2(0, 0))
                {
                    isPositioning = true;
                    touchPosition = Input.GetTouch(0).position + DistanceDifference;
                    return true;
                }
                else
                {
                    initialPosition = Input.GetTouch(0).position;
                    ObjectScreenPosition = ArCamera.GetComponent<Camera>().WorldToScreenPoint(spawnedObject.transform.position);
                    DistanceDifference = new Vector2(ObjectScreenPosition.x, ObjectScreenPosition.y) - initialPosition;
                    touchPosition = Input.GetTouch(0).position + DistanceDifference;
                    return true;
                }

            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                TouchIndicatorHandler.isTouchedTheObject = false;
                isPositioning = false;
                touchPosition = default;
                return false;
            }
            else
            {
                touchPosition = default;
                return false;
            }
        }
        else
        {
            TouchIndicatorHandler.isTouchedTheObject = false;
            isPositioning = false;
        }
        touchPosition = default;
        return false;
    }


    /// <summary>
    /// Sending the object to the detected position
    /// </summary>
    void SendObjectToDetectedPosition()
    {
        if (rotate || wentToPosition)
        {
            spawnedObject.transform.rotation = Quaternion.Lerp(FromRotation, toRotation, (Time.time - startTime) * speedR);
            if (spawnedObject.transform.rotation == toRotation)
            {
                rotate = false;
            }
            speed = Vector3.Distance(startMarker, endMarker);
            float distCovered = (Time.time - startTime) * speed * 100 * Time.deltaTime;
            float fractionOfJourney = distCovered / journeyLength;
            spawnedObject.transform.position = Vector3.Lerp(startMarker, endMarker, fractionOfJourney);
            if (spawnedObject.transform.position == endMarker)
            {
                wentToPosition = false;
                rotate = false;
                spawnedObject.transform.rotation = toRotation;
            }
        }

    }

    /// <summary>
    /// Hide the touch scale Percenntage indicator
    /// </summary>
    public static void hideScalePercentageIndicator()
    {
        TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().scalePersentageIndicator.SetActive(false);
    }

    /// <summary>
    /// Hide the touch scale Percenntage indicator
    /// </summary>
    public static void ShowScalePercentageIndicator(string Percentage)
    {
        TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().scalePersentageIndicator.SetActive(true);
        TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().scalePersentageIndicator.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = Percentage + "%";
    }


    /// <summary>
    /// Hide the touch indicator gameobject
    /// </summary>
    public static void hideTouchIndicator()
    {
        if (isObjectPlaced)
        {
            spawnedObject.GetComponent<SpawningObjectDetails>().touchIndicator.SetActive(false);
        }

    }

    /// <summary>
    /// Show the touch indicator gameobject
    /// </summary>
    public static void showTouchIndicator()
    {
        if (isObjectPlaced && !IsPointerOverUIObject() && spawnedObject.GetComponent<SpawningObjectDetails>().enableTouchIndicator)
        {
            spawnedObject.GetComponent<SpawningObjectDetails>().touchIndicator.SetActive(true);
        }

    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    /// <summary>
    /// Delay the spawning object appear inftront of camera
    /// </summary>
    void delayToShowSpawnedObject()
    {
        if (time > 1.2f)
        {
            if (spawnedObject != null) { 
            if (spawnedObject.GetComponent<SpawningObjectDetails>().planeDetectionMode == PlaneDetectionMode.Vertical)
            {
                spawnedObject.transform.eulerAngles = new Vector3(-90, spawnedObject.transform.eulerAngles.y, spawnedObject.transform.eulerAngles.z);
            }
            spawnedObject.SetActive(true);
            }
        }
        else
        {
            time += Time.deltaTime;
        }
    }

    /// <summary>
    /// Freeze the position when object is rotating
    /// </summary>
    void freezePositionWhenRotate()
    {
        if (isObjectPlaced && (Input.touchCount > 1))
        {
            if (previousRotation != spawnedObject.transform.rotation)
            {
                spawnedObject.transform.position = previousPosition;
                previousRotation = spawnedObject.transform.rotation;
            }
            else if (previousRotation == spawnedObject.transform.rotation)
            {
                previousPosition = spawnedObject.transform.position;
            }
        }
    }


    /// <summary>
    /// Reset scale to initial scale
    /// </summary>
    public static void resetToInitialScale()
    {
        spawnedObject.GetComponent<SpawningObjectDetails>().scalePersentageIndicator.SetActive(false);
        spawnedObject.transform.localScale = spawnedObject.GetComponent<SpawningObjectDetails>().initialScale;
    }


    /// <summary>
    /// Keep the object within in the camera clipping values 
    /// </summary>
    /// <param name="Obj"></param>
    void keepObjectWithInClippingValue(GameObject Obj)
    {
        Collider collider = Obj.GetComponent<Collider>();

        MaxScaleNumber = Mathf.Max(collider.bounds.size.x, collider.bounds.size.y, collider.bounds.size.z);
        if (MaxScaleNumber > 1)
        {
            Obj.transform.localScale = (Obj.transform.localScale / MaxScaleNumber) * 0.1f;
        }
        else
        {
            Obj.transform.localScale = (1 / MaxScaleNumber * Obj.transform.localScale) * 0.1f;
        }
    }
}


[System.Serializable]
public class SpawningObj
{
    public MeshRenderer meshRenderer;
    public Shader[] shaders;
}