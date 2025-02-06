using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace SIAairportSecurity.Training
{
    public class RaycastSpawnObject : MonoBehaviour
    {
        private ARRaycastManager _raycastManager;
        private ARPlaneManager _arPlaneManager;
        private GamePlayController _gamePlayController;

        [SerializeField] private Training _trainingObj;
        [SerializeField] private LayerMask itemLayerMask;
        [SerializeField] private Vector2 requiredSize = new Vector2(1.0f, 1.0f); // required size in meters

        private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
        private GameObject _selectedObject;

        private Vector2 _lastTouchPosition;
        private bool _isMovingObject = true;
        public bool isDelayed = false;

        private string _nameChecker = "";
        private string _horizontalVerticalPlane = "";

        public static RaycastSpawnObject Instance { get; private set; }

        #region Unity Callbacks

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (_raycastManager == null) return;

            HandleRaycast();
            //CheckPlaneScanned();
        }

        #endregion

        #region Initialization

        private void Initialize()
        {
            _gamePlayController = GetComponent<GamePlayController>();
            _raycastManager = FindObjectOfType<ARRaycastManager>();
            _arPlaneManager = FindObjectOfType<ARPlaneManager>();

            _arPlaneManager.planesChanged += PlanesSizeChecker;

        }

        void OnDestroy()
        {
            // Unsubscribe to avoid memory leaks
            if (_arPlaneManager != null)
                _arPlaneManager.planesChanged -= PlanesSizeChecker;
        }

        #endregion

        #region Raycasting

        private void HandleRaycast()
        {
            if (_gamePlayController.GetCurrentGameState() != GameState.MapArea && Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (IsPointerOverUI(touch)) return;

                if (touch.phase == TouchPhase.Began && _gamePlayController.GetCurrentGameState() != GameState.Gameplay)
                {
                    HandleTouchBegan(touch);
                }
                else if (touch.phase == TouchPhase.Moved && _selectedObject != null)
                {
                    HandleTouchMoved(touch);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    _selectedObject = null;
                }
            }
        }

        private void HandleTouchBegan(Touch touch)
        {
            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            if (!_gamePlayController.GetIfObjectSpawned() && _gamePlayController.GetCurrentGameState() != GameState.Scanning)
            {
                Debug.Log("Spawning object...");
                SpawnObject(touch);
            }
            else
            {
                Debug.Log("Selecting object...");
                SelectObject(ray);
            }
        }

        private void HandleTouchMoved(Touch touch)
        {
            if (_gamePlayController.GetCurrentObjectManipulation() == ObjectManipulation.Move)
            {
                DragObject(touch);
            }
            else if (_gamePlayController.GetCurrentObjectManipulation() == ObjectManipulation.Rotate)
            {
                RotateObject(touch);
            }
        }

        private void SelectObject(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, itemLayerMask))
            {
                _selectedObject = hit.transform.gameObject;
                _nameChecker = hit.transform.name;
            }
        }

        private void SpawnObject(Touch touch)
        {
            if (_raycastManager.Raycast(touch.position, _hits, TrackableType.PlaneWithinPolygon))
            {
                ARPlane plane = _arPlaneManager.GetPlane(_hits[0].trackableId);

                _nameChecker = plane.name;
                _horizontalVerticalPlane = plane.alignment.ToString();

                if (plane.alignment != PlaneAlignment.Vertical)
                {
                    Debug.Log($"Plane Size: {plane.size.x}m x {plane.size.y}m");

                    if (plane.size.x >= requiredSize.x && plane.size.y >= requiredSize.y)
                    {
                        // Plane is big enough
                        _gamePlayController.SpawnObject(_hits[0]);
                    }
                }
            }
        }

        private void DragObject(Touch touch)
        {
            if (_raycastManager.Raycast(touch.position, _hits, TrackableType.PlaneWithinBounds))
            {
                ARPlane plane = _arPlaneManager.GetPlane(_hits[0].trackableId);

                _nameChecker = plane.name;
                _horizontalVerticalPlane = plane.alignment.ToString();

                if (plane.alignment != PlaneAlignment.Vertical)
                {
                    Pose hitPose = _hits[0].pose;

                    _selectedObject.transform.position = hitPose.position;
                }
            }
        }

        private void RotateObject(Touch touch)
        {
            _selectedObject.transform.Rotate(new Vector3(0, -touch.deltaPosition.x, 0) * 0.5f, Space.World);
        }

        #endregion

        #region Plane Scanning

        private void PlanesSizeChecker(ARPlanesChangedEventArgs args)
        {
            // Check newly added planes
            foreach (ARPlane plane in args.added)
            {
                CheckPlane(plane);
            }

            // Check updated planes
            foreach (ARPlane plane in args.updated)
            {
                CheckPlane(plane);
            }
        }

        void CheckPlane(ARPlane plane)
        {
            // Only consider non-vertical (horizontal) planes
            if (plane.alignment != PlaneAlignment.Vertical)
            {

                if (plane.size.x >= requiredSize.x && plane.size.y >= requiredSize.y)
                {
                    if (_gamePlayController.GetCurrentGameState() == GameState.Scanning)
                    {
                        Debug.Log($"Plane {plane.trackableId} meets the requirement!");

                        _gamePlayController.RaiseStateChangeEvent(GameState.MapArea);
                        _trainingObj.ShowHideInfoPanel(false);
                    }
                }
                else
                {
                    //_trainingObj.ShowInstructionUI();
                }
            }
        }

        private void CheckPlaneScanned()
        {
            if (isDelayed) return;

            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

            if (_raycastManager.Raycast(screenCenter, _hits, TrackableType.PlaneWithinPolygon) &&
                _gamePlayController.GetCurrentGameState() == GameState.Scanning)
            {
                _gamePlayController.RaiseStateChangeEvent(GameState.MapArea);
                _trainingObj.ShowHideInfoPanel(false);
            }
            else if (_raycastManager.Raycast(screenCenter, _hits, TrackableType.PlaneWithinPolygon) && _trainingObj.GetCurrentInstructionUI() == Vector3.zero && _gamePlayController.GetCurrentGameState() == GameState.PlaceItem && !_gamePlayController.GetIfObjectSpawned())
            {
                _trainingObj.ShowInstructionUI();
            }
        }

        #endregion

        #region Utility Methods

        private bool IsPointerOverUI(Touch touch)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = touch.position
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }

        public void SetDelay(bool isDelayed)
        {
            this.isDelayed = isDelayed;
        }

        #endregion
    }
}
