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

        private Transform _selectedObject;
        private List<ARRaycastHit> hits = new List<ARRaycastHit>();

        // Rotate the selected object based on touch movement
        [SerializeField]private float rotationSpeed = 0.1f;
        // Start is called before the first frame update
        void Start()
        {
            //getting needed value
            init();
        }

        // Update is called once per frame
        void Update()
        {
            //spawning object using raycast
            if (raycastManager != null)
            {
                Raycast();
            }
        }

        private void init()
        {
            _gamePlayController = GetComponent<GamePlayController>();
            raycastManager = FindObjectOfType<ARRaycastManager>();
        }

        private void Raycast()
        {
            if (_gamePlayController.GetCurrentScreen() != MenuState.Selection)
            {
                // Check if the screen is touched
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Began && _gamePlayController.GetIsConfirmedPosition() == false)
                    {
                        // Perform AR raycast
                        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                        {
                            // Perform a physics raycast from the camera to the hit position
                            Ray ray = Camera.main.ScreenPointToRay(touch.position);
                            RaycastHit hita;

                            if (Physics.Raycast(ray, out hita))
                            {
                                if (_gamePlayController._spawnedObjects == null)
                                {
                                    _gamePlayController.SpawnObject(hits[0]);
                                }
                                else
                                {
                                    Debug.Log("object selecting " + hita.transform.name);
                                    if (hita.transform.CompareTag("Interactable"))
                                    {
                                        _selectedObject = hita.transform;
                                    }
                                    // Check if the hit object has a specific tag or component
                                }
                            }
                        }
                    }
                    else if (touch.phase == TouchPhase.Moved && _selectedObject != null)
                    {
                        _selectedObject.Rotate(new Vector3(touch.deltaPosition.y, -touch.deltaPosition.x, 0) * rotationSpeed, Space.World);
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        // Deselect the object when touch ends
                        _selectedObject = null;
                    }
                }
            }
        }

        private bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }
}
