using RTG;
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
        private GamePlayController _gamePlayController;
        private ARRaycastManager raycastManager;

        private GameObject _selectedObject;
        private List<ARRaycastHit> hits = new List<ARRaycastHit>();

        //scan surface UI
        [SerializeField] private GameObject _scanSurface;

        // Rotate the selected object based on touch movement
        private Vector2 lastTouchPosition;
        private bool isMovingObject = true;

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

            _scanSurface.SetActive(true);
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
                        lastTouchPosition = touch.position;

                        SelectOrSpawnObject(touch);
                    }
                    else if (touch.phase == TouchPhase.Moved && _selectedObject != null)
                    {
                        if (isMovingObject)
                        {
                            // Drag the selected object
                            DragObject(touch);
                        }
                        else
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


        private void SelectOrSpawnObject(Touch touch)
        {
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hitObject;

            // LayerMask to ignore the AR planes
            int layerMask = 1 << LayerMask.NameToLayer("ARPlane");

            // First, try to select an existing object
            if (Physics.Raycast(ray, out hitObject, Mathf.Infinity, ~layerMask))
            {
                Debug.Log(hitObject.transform.name);
                // Check if the object has the tag "Interactable"
                if (hitObject.transform.CompareTag("Interactable"))
                {
                    _selectedObject = hitObject.transform.gameObject;
                }
            }
            else if(!_gamePlayController.GetIfObjectSpawned())
            {
                // Raycast to get the position in the AR world using PlaneWithinPolygon
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    _gamePlayController.SpawnObject(hits[0]);
                }
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
            _selectedObject.transform.Rotate(new Vector3(touch.deltaPosition.y, -touch.deltaPosition.x, 0) * 0.5f, Space.World);
        }
        #endregion

        private void CheckPlaneScanned()
        {
            Vector3 rayEmitPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            if (raycastManager.Raycast(rayEmitPosition, hits, TrackableType.PlaneWithinPolygon))
                _scanSurface.SetActive(false);
        }
        public void IsSetToMove(bool condition)
        {
            isMovingObject = condition;
        }
        private bool IsPointerOverUIObject(Touch touch)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(touch.position.x, touch.position.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }
}
