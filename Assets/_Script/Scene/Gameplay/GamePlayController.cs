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

        //which object that selected and list of object in scene
        private GameObject _selectedObject;
        private List<GameObject> _spawnedObjects = new List<GameObject>();

        private int whichObject;

        private ARSession arSession;
        private ARSessionOrigin arSessionOrigin;
        private MultipleObjectPlacement multipleObject;
        private GameObject selectedGameobject;

        private Dictionary<GameObject, bool> _spawnedObjectsDictionary;
        private ShowDetectedPlanes showDetectedPlanes;
        private bool isMenuOn;

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
        }

        #region SetData

        //Set object to spawn
        public void SetGameObject(int gameobjectIndex)
        {
            //_selectedObject = go;

            if (!arSession.enabled)
            {
                arSession.enabled = true;
            }
            isMenuOn = false;
        }

        //Spawn selected object
        public void SpawnObject(ARRaycastHit hit)
        {
            //check if selected object already spawn in scene
            if (_spawnedObjects.Contains(selectedGameobject))
            {
                Debug.Log("Object is already spawned.");
                return;
            }

            // Raycast hit a plane, get the hit pose
            Pose hitPose = hit.pose;

            if (hitPose != null)
            {
                // Instantiate and place the object at the hit pose
                GameObject spawnedObject = Instantiate(selectedGameobject, hitPose.position, hitPose.rotation);
                _spawnedObjects.Add(spawnedObject);
                showDetectedPlanes.HidePlanes();

                Debug.Log("Object spawned at: " + hitPose.position);
            }
        }


        #endregion

        #region GetData

        //return current layer canvas
        public MenuState GetCurrentScreen()
        {
            return _gameCanvasController.activeState.state;
        }

        #endregion
        public void ResetObject()
        {
            foreach (GameObject temp in _spawnedObjects)
            {
                Destroy(temp);
            }
        }
    }
}
