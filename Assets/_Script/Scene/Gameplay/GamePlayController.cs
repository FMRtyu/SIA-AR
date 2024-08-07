using SIAairportSecurity.MainMenu;
using SIAairportSecurity.Training;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static SIAairportSecurity.Training.GameCanvasController;

namespace SIAairportSecurity.Training
{
    public class GamePlayController : MonoBehaviour
    {
        [Header("Controllers")]
        [SerializeField]private GameCanvasController _gameCanvasController;
        [SerializeField]private ItemDatabase _itemDatabase;
        [SerializeField]private TouchIndicatorHandler _touchIndicatorHandler;
        private RaycastSpawnObject _raycastController;

        //which object that selected and list of object in scene
        private GameObject _selectedObject;
        public GameObject _spawnedObjects { private set; get; }

        private bool isSpawnConformed = false;

        private ARSession arSession;
        private ARSessionOrigin arSessionOrigin;
        private MultipleObjectPlacement multipleObject;

        private Dictionary<GameObject, bool> _spawnedObjectsDictionary;
        private ShowDetectedPlanes showDetectedPlanes;

        public TMP_Text test;

        private void Start()
        {
            init();
        }

        private void Update()
        {

        }

        //initial operation
        private void init()
        {
            //set gamePlayController to all controller
            _gameCanvasController.InitState(this);

            //get value
            arSession = FindObjectOfType<ARSession>();
            arSessionOrigin = FindObjectOfType<ARSessionOrigin>();
            multipleObject = FindObjectOfType<MultipleObjectPlacement>();
            showDetectedPlanes = FindObjectOfType<ShowDetectedPlanes>();
            _raycastController= GetComponent<RaycastSpawnObject>();
        }

        #region SetData

        //Set object to spawn
        public void SetGameObject(int gameobjectIndex)
        {
            _selectedObject = _itemDatabase.items[gameobjectIndex].itemPrefabs;

            if (!arSession.enabled)
            {
                arSession.enabled = true;
            }
            GetComponent<ShowDetectedPlanes>().planeEnable = true;
        }

        public void ResetObject()
        {
            Destroy(_spawnedObjects);
            _spawnedObjects = null;

            isSpawnConformed = false;
            _raycastController.ShowGizmo(false);

            _touchIndicatorHandler.SetMoveabled(true);
        }

        public void ConformObjectPosition()
        {
            isSpawnConformed = true;
            _gameCanvasController.ShowConformedBTN(false);
            _raycastController.ShowGizmo(false);
            showDetectedPlanes.HidePlanes();

            _touchIndicatorHandler.SetMoveabled(false);
        }
        #endregion

        #region GetData

        //return current layer canvas
        public MenuState GetCurrentScreen()
        {
            return _gameCanvasController.activeState.state;
        }

        public Dictionary<int, (Sprite, bool, bool)> GetSelectionData()
        {
            Dictionary<int, (Sprite, bool, bool)> temp = new Dictionary<int, (Sprite, bool, bool)>();


            for (int i = 0; i < _itemDatabase.items.Length; i++)
            {
                temp.Add(_itemDatabase.items[i].itemID, (_itemDatabase.items[i].itemSprite, CheckIfAvaible(i), _itemDatabase.items[i].isObjectSmall));
            }

            return temp;
        }

        #endregion

        #region Operation

        //Spawn selected object
        public void SpawnObject(ARRaycastHit hit)
        {
            if (isSpawnConformed)
            {
                return;
            }
            else
            {
                //check if selected object already spawn in scene
                if (_spawnedObjects != null)
                {
                    Destroy(_spawnedObjects);
                    _spawnedObjects = null;
                }
                // Raycast hit a plane, get the hit pose
                Pose hitPose = hit.pose;

                if (hitPose != null)
                {
                    // Instantiate and place the object at the hit pose
                    GameObject spawnedObject = Instantiate(_selectedObject, hitPose.position, hitPose.rotation);
                    //GameObject spawnedObject2 = Instantiate(_selectedObject, hitPose.position, hitPose.rotation);
                    _spawnedObjects = spawnedObject;
                    //Destroy(spawnedObject2);

                    _gameCanvasController.EnableDisableInstrruction(false);
                    _gameCanvasController.ShowConformedBTN(true);
                    Debug.Log("Object spawned at: " + hitPose.position);

                    _raycastController.ShowGizmo(true);

                    Transform interactableObject = FindChildWithTag(spawnedObject.transform, "Interactable");

                    _raycastController.SetGizmoPosition(spawnedObject, interactableObject.gameObject);

                }
            }
        }

        Transform FindChildWithTag(Transform parent, string tag)
        {
            foreach (Transform child in parent)
            {
                if (child.CompareTag(tag))
                {
                    return child;
                }
            }
            return null;
        }

        //show all plane

        public void ShowAllPlane()
        {
            showDetectedPlanes.ShowPlanes();
        }

        private bool CheckIfAvaible(int itemIndex)
        {
            if (_itemDatabase.items[itemIndex].itemPrefabs != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
