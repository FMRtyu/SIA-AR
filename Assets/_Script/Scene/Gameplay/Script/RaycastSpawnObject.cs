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

        private Transform _selectedParentObject;
        private Transform _selectedChildObject;
        private List<ARRaycastHit> hits = new List<ARRaycastHit>();

        private ObjectTransformGizmo _objectTransformGizmoRotate;
        private ObjectTransformGizmo _objectTransformGizmoMove;

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
            UpdateRotateGizmo();
        }

        private void init()
        {
            _gamePlayController = GetComponent<GamePlayController>();
            raycastManager = FindObjectOfType<ARRaycastManager>();

            _objectTransformGizmoRotate = RTGizmosEngine.Get.CreateObjectRotationGizmo();
            _objectTransformGizmoMove = RTGizmosEngine.Get.CreateObjectMoveGizmo();

            _objectTransformGizmoMove.SetTransformSpace(GizmoSpace.Global);
            _objectTransformGizmoRotate.SetTransformSpace(GizmoSpace.Global);

            ShowGizmo(false);
        }

        private void Raycast()
        {
            if (_gamePlayController.GetCurrentScreen() != MenuState.Selection)
            {
                // Check if the screen is touched
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Began)
                    {
                        // Perform AR raycast
                        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                        {
                            // Perform a physics raycast from the camera to the hit position
                            Ray ray = Camera.main.ScreenPointToRay(touch.position);
                            RaycastHit hita;

                            if (Physics.Raycast(ray, out hita))
                            {
                                if (!IsPointerOverUIObject() && _gamePlayController._spawnedObjects == null)
                                {
                                    _gamePlayController.SpawnObject(hits[0]);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateRotateGizmo()
        {
            if (_selectedParentObject != null && _selectedChildObject != null)
            {
                // Ensure the rotate gizmo follows the object's position
                _objectTransformGizmoRotate.SetTargetObject(_selectedChildObject.gameObject);
                _objectTransformGizmoMove.SetTargetObject(_selectedParentObject.gameObject);
            }
        }

        public void ShowGizmo(bool condition)
        {
            _objectTransformGizmoRotate.Gizmo.SetEnabled(condition);
            _objectTransformGizmoMove.Gizmo.SetEnabled(condition);
        }

        public void SetGizmoPosition(GameObject SpawnedParentObject, GameObject SpawnedChildObject)
        {
            _selectedParentObject = SpawnedParentObject.transform;
            _selectedChildObject = SpawnedChildObject.transform;

            ShowGizmo(true);

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
