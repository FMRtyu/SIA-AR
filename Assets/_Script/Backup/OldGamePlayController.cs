using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace SIAairportSecurity
{
    public class OldGamePlayController : MonoBehaviour
    {
        public GameObject objectToSpawn;

        public GameObject[] allObject;
        private int whichObject;

        private ARSession arSession;
        private ARSessionOrigin arSessionOrigin;
        private ARRaycastManager raycastManager;
        private MultipleObjectPlacement multipleObject;
        private GameObject selectedGameobject;
        private List<GameObject> spawnedObjects;
        private Dictionary<GameObject, bool> spawnedObjectsDictionary;
        private ShowDetectedPlanes showDetectedPlanes;
        private bool isMenuOn;

        public TMP_Text test;

        private void Start()
        {
            isMenuOn = true;
            spawnedObjects = new List<GameObject>();

            spawnedObjectsDictionary = new Dictionary<GameObject, bool>();
            foreach (GameObject Prefabs in allObject)
            {
                spawnedObjectsDictionary.Add(Prefabs, false);
            }


            arSession = FindObjectOfType<ARSession>();
            arSessionOrigin = FindObjectOfType<ARSessionOrigin>();
            multipleObject = FindObjectOfType<MultipleObjectPlacement>();
            showDetectedPlanes = FindObjectOfType<ShowDetectedPlanes>();
            raycastManager = FindObjectOfType<ARRaycastManager>();
        }

        private void Update()
        {
            if (!isMenuOn)
            {
                // Check if the screen is touched
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    // Check if the touch phase is the beginning of a touch
                    if (touch.phase == TouchPhase.Began)
                    {
                        // Perform AR raycast from the touch position
                        List<ARRaycastHit> hits = new List<ARRaycastHit>();
                        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                        {
                            // Raycast hit a plane, get the hit pose
                            Pose hitPose = hits[0].pose;

                            // Check if object is already spawned
                            if (!CheckObject(objectToSpawn))
                            {
                                // Instantiate and place the object at the hit pose
                                GameObject spawnedObject = Instantiate(objectToSpawn, hitPose.position, hitPose.rotation);
                                GameObject spawnedObject2 = Instantiate(objectToSpawn, hitPose.position, hitPose.rotation);
                                spawnedObjects.Add(spawnedObject);
                                spawnedObjects.Add(spawnedObject2);
                                spawnedObjectsDictionary[objectToSpawn] = true;
                                showDetectedPlanes.HidePlanes();

                                Debug.Log("Object spawned at: " + hitPose.position);
                            }
                            else
                            {
                                Debug.Log("Object is already spawned.");
                            }
                        }
                        else
                        {
                            Debug.Log("Raycast did not hit a plane.");
                        }
                    }
                }
            }
        }

        public void SetGameObject(GameObject go)
        {
            selectedGameobject = go;

            if (!arSession.enabled)
            {
                arSession.enabled = true;
            }
            isMenuOn = false;
            objectToSpawn = selectedGameobject;
        }

        public void SetOnMenu()
        {
            isMenuOn = true;
        }

        private bool CheckObject(GameObject objectToCheck)
        {
            bool isSpawned;
            if (spawnedObjectsDictionary.TryGetValue(objectToCheck, out isSpawned))
            {
                return isSpawned;
            }
            return false;
        }

        private void SpawnObject(Pose objectPos)
        {
            //Instantiate(selectedGameobject, objectPos.position, objectPos.rotation);
            //if (!CheckObject(selectedGameobject))
            //{
            //   multipleObject.spawnObject(selectedGameobject, objectPos);
            //  isGameObjectSpawned.Add(selectedGameobject);
            // showDetectedPlanes.HidePlanes();
            //}

            if (!CheckObject(selectedGameobject))
            {
                GameObject obejct = Instantiate(selectedGameobject, objectPos.position, objectPos.rotation);
                GameObject obejct2 = Instantiate(selectedGameobject, objectPos.position, objectPos.rotation);
                spawnedObjects.Add(obejct);

                test.gameObject.SetActive(true);
                test.text = obejct.name + " has been spawned at " + obejct.transform.position;
            }
        }
        public void ResetObject()
        {
            foreach(GameObject temp in spawnedObjects)
            {
                Destroy(temp);
            }

            foreach(var key in spawnedObjectsDictionary.Keys.ToList())
            {
                spawnedObjectsDictionary[key] = false;
            }

            spawnedObjects.Clear();
        }
    }
}
