using SIAairportSecurity.MainMenu;
using SIAairportSecurity.Training;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
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
        private RaycastSpawnObject _raycastController;

        //which object that selected and list of object in scene
        private GameObject _selectedObjectPrefab;
        private GameObject _spawnedObjects;

        //_spawnedObjects property
        private Transform _spawnedObjectRotateObject;
        private Vector3 _intialposition;
        private BoxCollider _spawnedObjectCollider;
        private Rigidbody _spawnedObjectRigidbody;
        private bool isSpawnConformed = false;
        private bool isInitRotate = false;

        //ARF
        private ARSession arSession;
        private GameObject arSessionParent;
        private ShowDetectedPlanes showDetectedPlanes;

        //AR plane manager
        private ARPlaneManager arPlaneManager;
        private ARPlaneManager arPlaneManagerTemplate;
        private GameObject arPlaneManagerParent;

        [Header("SFX")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _tapAudio;
        [SerializeField] private AudioClip _SpawnAudio;

        private void Awake()
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
            arSessionParent = arSession.gameObject;
            showDetectedPlanes = FindObjectOfType<ShowDetectedPlanes>();
            _raycastController= GetComponent<RaycastSpawnObject>();

            //get and setup arplane value
            arPlaneManager = FindObjectOfType<ARPlaneManager>();
            arPlaneManagerTemplate = new ARPlaneManager();
            arPlaneManagerTemplate.planePrefab = arPlaneManager.planePrefab;
            arPlaneManagerParent = arPlaneManager.gameObject;
        }

        #region SetData

        //Set object to spawn
        public void SetGameObject(int gameobjectIndex)
        {
            _selectedObjectPrefab = _itemDatabase.items[gameobjectIndex].itemPrefabs;

            if (!arSession.enabled)
            {
                arSession.enabled = true;
            }
            
        }

        //reset training session
        public void ResetObject()
        {
            Destroy(_spawnedObjects);
            _spawnedObjects = null;

            isSpawnConformed = false;

            ShowHideMoveRotateBTN(true);
            _raycastController.ChangeState(ObjectManipulation.Move);
        }
        public void DestroySpawnedObject()
        {
            Destroy(_spawnedObjects);
            _spawnedObjects = null;

            isSpawnConformed = false;
            _raycastController.ChangeState(ObjectManipulation.Move);
        }

        //confirm items placement after click place item
        public void ConformObjectPosition()
        {
            _gameCanvasController.ShowConformedBTN(false);
            showDetectedPlanes.ShowDotsPlane(false);

            //move down the object
            _spawnedObjectRotateObject.localPosition = _intialposition;

            ShowHideMoveRotateBTN(false);

            //delete box collider in rotate object
            _spawnedObjectRotateObject.GetComponent<BoxCollider>().enabled = true;
            _spawnedObjects.GetComponent<BoxCollider>().enabled = false;
            _spawnedObjectCollider.enabled = true;

            //disable touch indicator
            FindChildWithTag(_spawnedObjects.transform, "TouchIndicator").gameObject.SetActive(false);

            //add rigidbody
            _spawnedObjectRigidbody = _spawnedObjectRotateObject.gameObject.AddComponent<Rigidbody>();
            _spawnedObjectRigidbody.freezeRotation = true;

            //delete rigidbody after a second
            Invoke("DeleteRigidbody", 1.5f);

            isSpawnConformed = true;
        }

        public void ShowMappingInstruction(bool showInstruction)
        {
            _gameCanvasController.ShowMappingInstruction(showInstruction);
        }

        public void ResetPlane(Button button)
        {
            showDetectedPlanes.ResetPlane(button);

            //ResetAllPlane();
        }
        #endregion

        #region GetData

        //return current layer canvas
        public MenuState GetCurrentScreen()
        {
            return _gameCanvasController.activeState.state;
        }

        //get item data from dataset
        public Dictionary<int, (Sprite, string, bool, bool)> GetSelectionData()
        {
            Dictionary<int, (Sprite, string, bool, bool)> temp = new Dictionary<int, (Sprite, string, bool, bool)>();


            for (int i = 0; i < _itemDatabase.items.Length; i++)
            {
                temp.Add(_itemDatabase.items[i].itemID, (_itemDatabase.items[i].itemSprite, _itemDatabase.items[i].itemName, CheckIfAvaible(i), _itemDatabase.items[i].isObjectSmall));
            }

            return temp;
        }

        //check if the object has been confirm
        public bool GetIsConfirmedPosition()
        {
            return isSpawnConformed;
        }

        //check if the object already spawned
        public bool GetIfObjectSpawned()
        {
            if (_spawnedObjects == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CheckARPlaneExist()
        {
            return showDetectedPlanes.CheckARPlaneScanned();
        }

        public bool CheckMenuToSpawn()
        {
            return _gameCanvasController.CheckMenuToSpawn();
        }

        
        public ARPlaneManager GetARPlaneManager()
        {
            return arPlaneManager;
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
                    GameObject spawnedObject = Instantiate(_selectedObjectPrefab, hitPose.position, hitPose.rotation);
                    _spawnedObjects = spawnedObject;

                    //set all necessary value
                    Transform[] childTransforms = spawnedObject.GetComponentsInChildren<Transform>();
                    _spawnedObjectRotateObject = FindChildWithTag(spawnedObject.transform, "Interactable");
                    _intialposition = _spawnedObjectRotateObject.localPosition;
                    _spawnedObjectCollider = spawnedObject.GetComponent<BoxCollider>();

                    //scale up object
                    Vector3 InitalScale = _spawnedObjectRotateObject.transform.localScale;
                    _spawnedObjectRotateObject.transform.localScale = Vector3.zero;
                    LeanTween.scale(_spawnedObjectRotateObject.gameObject, to: InitalScale, 1f).setEase(LeanTweenType.easeOutBack);
                    PlayPlaceSound();

                    _gameCanvasController.ShowConformedBTN(true);

                }
            }
        }

        //find child object from parent with tag
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

        //show all ARPlane
        public void ShowAllPlane()
        {
            showDetectedPlanes.ShowDotsPlane(true);
        }

        //check if object prefabs was set
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

        //delete rigidbody from spawned object after i second
        private void DeleteRigidbody()
        {
            Destroy(_spawnedObjectRigidbody);
            _spawnedObjects.GetComponent<BoxCollider>().enabled = true;
        }

        //play button SFX
        public void PlayButtonSound()
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }

            // Set the clip and play it
            if (_audioSource.clip != _tapAudio)
            {
                _audioSource.clip = _tapAudio;
            }
            _audioSource.Play();
        }
        public void PlayPlaceSound()
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }

            // Set the clip and play it
            if (_audioSource.clip != _SpawnAudio)
            {
                _audioSource.clip = _SpawnAudio;
            }
            _audioSource.Play();
        }

        float SnapValue(float value, float snapInterval)
        {
            float snappedValue = Mathf.Round(value / snapInterval) * snapInterval;
            if (snappedValue >= 360f)
            {
                return 0f;
            }
            if (snappedValue == 0f || snappedValue == 90f || snappedValue == 180f || snappedValue == 270f)
            {
                snappedValue += 90f;
            }
            return snappedValue;
        }

        public void ResetAllPlane()
        {
            arPlaneManager.ResetTrackables();
            
            StartCoroutine(DelayCreateManager());
        }

        private IEnumerator DelayCreateManager()
        {
            Destroy(arPlaneManager);
            //Destroy(arSession);
            yield return new WaitForSeconds(1f);
            //arSession = arSessionParent.AddComponent<ARSession>();
            arPlaneManager = arPlaneManagerParent.AddComponent<ARPlaneManager>();
            arPlaneManager.planePrefab = arPlaneManagerTemplate.planePrefab;
        }
        #endregion

        #region RotateMove

        //switch to move object 1 finger
        public void SwitchToMove()
        {
            if (_spawnedObjectCollider != null)
            {
                _spawnedObjectCollider.enabled = true;
                _raycastController.ChangeState(ObjectManipulation.Move);

                //move down the object
                _spawnedObjectRotateObject.localPosition = _intialposition;
                _spawnedObjectRotateObject.GetComponent<BoxCollider>().enabled = false;
                isInitRotate = false;
            }
        }
        //switch to rotate object 1 finger
        public void SwitchToRotate()
        {
            if (_spawnedObjectCollider != null)
            {
                _spawnedObjectCollider.enabled = false;
                _raycastController.ChangeState(ObjectManipulation.Rotate);

                //move up the object
                Vector3 newPos = new Vector3(_intialposition.x, _intialposition.y + 0.15f, _intialposition.z);
                _spawnedObjectRotateObject.localPosition = newPos;
                if (isInitRotate)
                {
                    ChangeRotateOrientation(_spawnedObjectRotateObject);
                }
                else
                {
                    isInitRotate = true;
                }
                _spawnedObjectRotateObject.GetComponent<BoxCollider>().enabled = true;
            }
        }

        //show or hide move rotate panel
        public void ShowHideMoveRotateBTN(bool Condition)
        {
            _gameCanvasController.ShowHideMoveRotateBTN(Condition);
        }

        //reset move rotate panel
        public void ResetMoveRotate()
        {
            isSpawnConformed = false;

            _raycastController.ChangeState(ObjectManipulation.Move);
            showDetectedPlanes.ShowDotsPlane(true);
            FindChildWithTag(_spawnedObjects.transform, "TouchIndicator").gameObject.SetActive(true);
        }

        private void ChangeRotateOrientation(Transform rotatedObject)
        {
            // Get the x rotation in degrees
            float x = rotatedObject.eulerAngles.x;

            // Define a small tolerance for comparison
            float tolerance = 0.1f;

            // Check if the x rotation is approximately 0 degrees
            if (Mathf.Abs(x) < tolerance || Mathf.Abs(x - 360) < tolerance)
            {
                rotatedObject.rotation = Quaternion.Euler(-90, rotatedObject.eulerAngles.y, rotatedObject.eulerAngles.z);
            }
            else
            {
                rotatedObject.rotation = Quaternion.Euler(0, rotatedObject.eulerAngles.y, rotatedObject.eulerAngles.z);
            }
        }

        public void SnapObjectXAxis()
        {
            Quaternion currentRotation = _spawnedObjectRotateObject.rotation;
            float newRotationX = currentRotation.x + 90;
            Debug.Log(newRotationX + " " + currentRotation);
            _spawnedObjectRotateObject.Rotate(new Vector3(newRotationX, _spawnedObjectRotateObject.rotation.y, _spawnedObjectRotateObject.rotation.z), Space.Self);
        }

        public void SnapObjectYAxis()
        {
            Quaternion currentRotation = _spawnedObjectRotateObject.rotation;
            float newRotationY = currentRotation.eulerAngles.y;
            Debug.Log(newRotationY + " " + currentRotation + " " + SnapValue(newRotationY, 90f));
            _spawnedObjectRotateObject.rotation = Quaternion.Euler(_spawnedObjectRotateObject.rotation.eulerAngles.x,
                SnapValue(newRotationY, 90f),
                _spawnedObjectRotateObject.rotation.eulerAngles.z
                );
        }
        #endregion
    }
}
