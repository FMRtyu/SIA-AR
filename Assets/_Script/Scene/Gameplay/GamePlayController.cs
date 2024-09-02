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

        //ARF
        private ARSession arSession;
        private ShowDetectedPlanes showDetectedPlanes;

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
            showDetectedPlanes = FindObjectOfType<ShowDetectedPlanes>();
            _raycastController= GetComponent<RaycastSpawnObject>();
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
            _raycastController.IsSetToMove(true);
        }

        //place item
        public void ConformObjectPosition()
        {
            isSpawnConformed = true;
            _gameCanvasController.ShowConformedBTN(false);
            showDetectedPlanes.ShowDotsPlane(false);

            //move down the object
            _spawnedObjectRotateObject.localPosition = _intialposition;

            ShowHideMoveRotateBTN(false);

            //delete box collider in rotate object
            _spawnedObjectRotateObject.GetComponent<BoxCollider>().enabled = false;
            _spawnedObjectCollider.enabled = true;

            //disable touch indicator
            FindChildWithTag(_spawnedObjects.transform, "TouchIndicator").gameObject.SetActive(false);

            //add rigidbody
            _spawnedObjectRigidbody = _spawnedObjects.AddComponent<Rigidbody>();
            _spawnedObjectRigidbody.freezeRotation = true;

            //delete rigidbody after a second
            Invoke("DeleteRigidbody", 2.5f);
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

                    _gameCanvasController.EnableDisableInstrruction(false);
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

        //show all plane
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
        #endregion

        #region SwitchRotateMove

        //switch to move object 1 finger
        public void SwitchToMove()
        {
            if (_spawnedObjectCollider != null)
            {
                _spawnedObjectCollider.enabled = true;
                _raycastController.IsSetToMove(true);

                //move down the object
                _spawnedObjectRotateObject.localPosition = _intialposition;
                _spawnedObjectRotateObject.GetComponent<BoxCollider>().enabled = false;
            }
        }
        //switch to rotate object 1 finger
        public void SwitchToRotate()
        {
            if (_spawnedObjectCollider != null)
            {
                _spawnedObjectCollider.enabled = false;
                _raycastController.IsSetToMove(false);

                //move up the object
                Vector3 newPos = new Vector3(_intialposition.x, _intialposition.y + 0.15f, _intialposition.z);
                _spawnedObjectRotateObject.localPosition = newPos;
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

            _raycastController.IsSetToMove(true);
            showDetectedPlanes.ShowDotsPlane(true);
            FindChildWithTag(_spawnedObjects.transform, "TouchIndicator").gameObject.SetActive(true);
        }
        #endregion
    }
}
