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
            CheckPlaneScanned();
        }

        #endregion

        #region Initialization

        private void Initialize()
        {
            _gamePlayController = GetComponent<GamePlayController>();
            _raycastManager = FindObjectOfType<ARRaycastManager>();
            _arPlaneManager = FindObjectOfType<ARPlaneManager>();
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
                    _gamePlayController.SpawnObject(_hits[0]);
                }
            }
        }

        private void DragObject(Touch touch)
        {
            if (_raycastManager.Raycast(touch.position, _hits, TrackableType.PlaneWithinBounds))
            {
                Pose hitPose = _hits[0].pose;
                _selectedObject.transform.position = hitPose.position;
            }
        }

        private void RotateObject(Touch touch)
        {
            _selectedObject.transform.Rotate(new Vector3(0, -touch.deltaPosition.x, 0) * 0.5f, Space.World);
        }

        #endregion

        #region Plane Scanning

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
