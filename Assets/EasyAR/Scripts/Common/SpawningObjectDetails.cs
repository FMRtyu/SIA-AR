using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpawningObjectDetails : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Select Object Plane detection mode")]
    private PlaneDetectionMode _planeDetectionMode;

    [SerializeField]
    [Tooltip("Remove tick if you dont need Draging feature")]
    bool _enbleDragFeature = true;

    [SerializeField]
    [Tooltip("Remove tick if you dont need Rotating feature")]
    bool _enableRotateFeature = true;

    [SerializeField]
    [Tooltip("Enter rotate factor")]
    int _rotateFactor = 750;

    [SerializeField]
    [Tooltip("Remove tick if you dont need Scaling feature")]
    bool _enableScaleFeature = true;

    [SerializeField]
    [Tooltip("Remove tick if you dont need Scaling feature")]
    bool _enableARQualityControl = true;

    [SerializeField]
    [Tooltip("Use lower scalling factor for increase the scalling speed")]
    int _scaleFactor = 400;

    [SerializeField]
    [Tooltip("Remove tick if you dont need shadow plane feature")]
    bool _enableShadowPlane = true;

    [SerializeField]
    [Tooltip("Drag and drop shadow palne here")]
    private GameObject _shadowPlane;

    [SerializeField]
    [Tooltip("Remove tick if you dont need touch indicator feature")]
    bool _enableTouchIndicator = true;

    [SerializeField]
    [Tooltip("Drag and drop Touch Indicator here")]
    private GameObject _touchIndicator;

    [SerializeField]
    [Tooltip("Drag and drop Scale perceantage here")]
    private GameObject _scalePersentageIndicator;

   

    private Vector3 _initialScale = new Vector3(0, 0, 0);

    private Vector3 _minimumScaleValue = new Vector3(0, 0, 0);

    private Vector3 _maximumScaleValue = new Vector3(0, 0, 0);

    private Vector3 _limitScaleValue = new Vector3(0, 0, 0);

    private Quaternion _initialRotation;

    private Quaternion _initialPlacedRotation;


    public bool enableTouchIndicator
    {
        get { return _enableTouchIndicator; }
    }
    public bool enableShadowPlane
    {
        get { return _enableShadowPlane; }
    }

    public bool enbleDragFeature
    {
        get { return _enbleDragFeature; }
    }
    public bool enableRotateFeature
    {
        get { return _enableRotateFeature; }
    }
    public bool enableScaleFeature
    {
        get { return _enableScaleFeature; }
    }

    public bool enableARQualityControl
    {
        get { return _enableARQualityControl; }
    }

    public int rotateFactor
    {
        get { return _rotateFactor; }
    }
    public int scaleFactor
    {
        get { return _scaleFactor; }
    }



    public Quaternion initialPlacedRotation
    {
        get { return _initialPlacedRotation; }
        set { _initialPlacedRotation = value; }
    }
    public Quaternion initialRotation
    {
        get { return _initialRotation; }
        set { _initialRotation = value; }
    }
    public Vector3 initialScale
    {
        get { return _initialScale; }
        set { _initialScale = value; }
    }

    public Vector3 minimumScaleValue
    {
        get { return _minimumScaleValue; }
        set { _minimumScaleValue = value; }
    }

    public Vector3 maximumScaleValue
    {
        get { return _maximumScaleValue; }
        set { _maximumScaleValue = value; }
    }

    public Vector3 limitScaleValue
    {
        get { return _limitScaleValue; }
        set { _limitScaleValue = value; }
    }


    public PlaneDetectionMode planeDetectionMode
    {
        get { return _planeDetectionMode; }
        set { _planeDetectionMode = value; }
    }

    public GameObject shadowPlane
    {
        get { return _shadowPlane; }
        set { _shadowPlane = value; }
    }
    public GameObject scalePersentageIndicator
    {
        get { return _scalePersentageIndicator; }
        set { _scalePersentageIndicator = value; }
    }
    public GameObject touchIndicator
    {
        get { return _touchIndicator; }
        set { _touchIndicator = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _initialPlacedRotation = gameObject.transform.rotation;
        _initialScale = gameObject.transform.localScale;
        _shadowPlane.SetActive(false);
        _scalePersentageIndicator.SetActive(false);
        _touchIndicator.SetActive(false);
    }
}
