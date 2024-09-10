using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static SIAairportSecurity.Training.GameCanvasController;

namespace SIAairportSecurity.Training
{
    public class RaycastSpawnObject : MonoBehaviour
    {
        private ObjectManipulation _objectManipulation;

        private GamePlayController _gamePlayController;
        private ARRaycastManager raycastManager;
        private ARPlaneManager _arPlaneManager;

        private GameObject _selectedObject;
        private List<ARRaycastHit> hits = new List<ARRaycastHit>();

        //scan surface UI
        [SerializeField] private GameObject _scanSurface;
        [SerializeField] private LayerMask itemLayerMask;

        // Rotate the selected object based on touch movement
        private Vector2 lastTouchPosition;
        private bool isMovingObject = true;
        private bool isSurfaceDetected;

        // Start is called before the first frame update
        void Start()
        {
            //get value
            init();
        }

        // Update is called once per frame
        void Update()
        {
            //spawning object using raycast
            if (raycastManager != null)
            {
                Raycast();
                CheckPlaneScanned();
            }
        }

        private void init()
        {
            _gamePlayController = GetComponent<GamePlayController>();
            raycastManager = FindObjectOfType<ARRaycastManager>();

            _arPlaneManager = FindAnyObjectByType<ARPlaneManager>();

            _scanSurface.SetActive(true);
            _gamePlayController.SetTapInstruction(false);
        }

        #region RayCast
        private void Raycast()
        {
            if (_gamePlayController.GetCurrentScreen() != MenuState.Selection)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if (IsPointerOverUIObject(touch))
                        return;

                    if (touch.phase == TouchPhase.Began && _gamePlayController.GetIsConfirmedPosition() == false)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(touch.position);

                        if (!_gamePlayController.GetIfObjectSpawned())
                        {
                            SpawnItem(touch);
                        }
                        else
                        {
                            SelectItem(ray);
                        }
                    }
                    else if (touch.phase == TouchPhase.Moved && _selectedObject != null)
                    {
                        if (_objectManipulation == ObjectManipulation.Move)
                        {
                            // Drag the selected object
                            DragObject(touch);
                        }
                        else if (_objectManipulation == ObjectManipulation.Rotate)
                        {
                            RotateObject(touch);
                        }
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        // Deselect the object
                        _selectedObject = null;
                    }
                }
            }
        }

        private void SelectItem(Ray ray)
        {
            RaycastHit hitObject;

            // First, try to select an existing object
            if (Physics.Raycast(ray, out hitObject, Mathf.Infinity, itemLayerMask))
            {
                _selectedObject = hitObject.transform.gameObject;
            }
        }

        private void SpawnItem(Touch touch)
        {
            TrackableId curId = hits[0].trackableId;
            ARPlane plane = _arPlaneManager.GetPlane(curId);

            if (plane.alignment != PlaneAlignment.Vertical)
            {
                _gamePlayController.SpawnObject(hits[0]);
            }
        }

        private void DragObject(Touch touch)
        {
            // Raycast to get the position in the AR world
            if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinBounds))
            {
                Pose hitPose = hits[0].pose;
                _selectedObject.transform.position = hitPose.position;
            }
        }

        private void RotateObject(Touch touch)
        {
            _selectedObject.transform.Rotate(new Vector3(0, -touch.deltaPosition.x, 0) * 0.5f, Space.World);
        }
        #endregion

        private void CheckPlaneScanned()
        {
            Vector3 rayEmitPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            if (raycastManager.Raycast(rayEmitPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                if (!isSurfaceDetected)
                {
                    _scanSurface.SetActive(false);
                    _gamePlayController.SetTapInstruction(true);
                    isSurfaceDetected = true;
                }
            }
        }
        private bool IsPointerOverUIObject(Touch touch)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(touch.position.x, touch.position.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public void ChangeState(ObjectManipulation newState)
        {
            _objectManipulation = newState;
        }
    }
}
