using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

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

        //Spawned Object
        private GameObject _spawnedObjects;
        public ItemProperty itemProperty {  get; private set; }
        public bool isSpawnConformed { get; private set; }

        public SurfaceManager _surfaceManager {  get; private set; }

        //AR plane manager
        private ARPlaneManager arPlaneManager;
        private ARPlaneManager arPlaneManagerTemplate;
        private GameObject arPlaneManagerParent;

        [Header("SFX")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _tapAudio;
        [SerializeField] private AudioClip _SpawnAudio;

        //Game State Event
        [SerializeField] private GameState currentGameState = GameState.Scanning;

        public delegate void OnGameStateChanged(GameState currentGameState);
        public event OnGameStateChanged onStateChange;

        //object manipulation
        [SerializeField] private ObjectManipulation currentObjectManipulation = ObjectManipulation.None;

        public delegate void OnObjectManipulationChange(ObjectManipulation currentObjectManipulation);
        public event OnObjectManipulationChange onObjectManipulationChange;

        bool isARPlaneAlreadyOpen = false;

        private void Awake()
        {
            onStateChange += ChangeState;
            onObjectManipulationChange += ChangeObjectManipulation;
            init();
        }

        private void OnDestroy()
        {
            onStateChange -= ChangeState;
            onObjectManipulationChange -= ChangeObjectManipulation;
        }

        //initial operation
        private void init()
        {
            // Apply delay to all buttons in the scene
            ButtonDelay.ApplyDelayToAllButtons(0.3f, this);

            //set gamePlayController to all controller
            _gameCanvasController.InitState(this);

            _surfaceManager = FindObjectOfType<SurfaceManager>();
            _raycastController= GetComponent<RaycastSpawnObject>();

            //get and setup arplane value
            arPlaneManager = FindObjectOfType<ARPlaneManager>();
            arPlaneManagerTemplate = new ARPlaneManager();
            arPlaneManagerTemplate.planePrefab = arPlaneManager.planePrefab;
            arPlaneManagerParent = arPlaneManager.gameObject;
        }

        public void SetButtonSFX()
        {
            ButtonSFX.ApplySFXToAllButtons(_audioSource, _tapAudio);
        }

        #region SetData
        public void SetIsConfirmedPosition(bool newState)
        {
            isSpawnConformed = newState;
        }
        //Set object to spawn
        public void SetGameObject(int gameobjectIndex)
        {
            _selectedObjectPrefab = _itemDatabase.items[gameobjectIndex].itemPrefabs;

            //if (!arPlaneManager.enabled && !isInnit)
            //{
            //    arPlaneManager.enabled = true;

            //    isInnit = true;
            //}
            
        }

        //reset training session
        public void ResetObject()
        {
            if (_spawnedObjects == null)
            {
                return;
            }
            if (_spawnedObjects)
            {
                Destroy(_spawnedObjects);
                _spawnedObjects = null;
            }

            isSpawnConformed = false;
            RaiseManipulationObjectChangeEvent(ObjectManipulation.Move);

            _gameCanvasController.Training.SwitchManipulationState(false);

            RaiseStateChangeEvent(GameState.PlaceItem);
        }
        public void DestroySpawnedObject()
        {
            if (_spawnedObjects != null)
            {
                Destroy(_spawnedObjects);
                _spawnedObjects = null;
            }

            isSpawnConformed = false;
            RaiseManipulationObjectChangeEvent(ObjectManipulation.Move);
        }

        //confirm items placement after click place item
        public void ConfirmObjectPosition()
        {
            itemProperty.ConfirmObjectPosition();
            _gameCanvasController.ShowPlacedItemBTN(false);
            _gameCanvasController.Training.ActivatedDeactivatedEditButton(true);
            _surfaceManager.ShowDotsPlane(false);

            RaiseManipulationObjectChangeEvent(ObjectManipulation.None);

            isSpawnConformed = true;
        }

        public void ResetPlane()
        {
            StartCoroutine(ResetPlaneDelay());
        }

        private IEnumerator ResetPlaneDelay()
        {
            _surfaceManager.ResetPlane();

            DestroySpawnedObject();

            if (_spawnedObjects == null)
            {
                _gameCanvasController.ShowPlacedItemBTN(false);
            }
            yield return new WaitForSeconds(3f);
            _surfaceManager.EnabledARManager();
            yield return new WaitForSeconds(1f);
            _raycastController.isDelayed = false;
        }
        #endregion

        #region GetData

        public GameState GetCurrentGameState()
        {
            return currentGameState;
        }

        public ObjectManipulation GetCurrentObjectManipulation()
        {
            return currentObjectManipulation;
        }
        //return current layer canvas
        public MenuState GetCurrentScreen()
        {
            return _gameCanvasController.activeState.state;
        }

        //get item data from dataset
        public Dictionary<int, (Sprite, string, bool)> GetSelectionData()
        {
            Dictionary<int, (Sprite, string, bool)> temp = new Dictionary<int, (Sprite, string, bool)>();


            for (int i = 0; i < _itemDatabase.items.Length; i++)
            {
                temp.Add(_itemDatabase.items[i].itemID, (_itemDatabase.items[i].itemSprite, _itemDatabase.items[i].itemName, CheckIfAvaible(i)));
            }

            return temp;
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

        public void EnableARPlane(bool newCondition)
        {
            if (!arPlaneManager.enabled && !isARPlaneAlreadyOpen)
            {
                _surfaceManager.StartStopScanning(newCondition);

                isARPlaneAlreadyOpen = true;
            }
        }
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
                    itemProperty = _spawnedObjects.GetComponent<ItemProperty>();
                    itemProperty.SetGameController(this);

                    PlayPlaceSound();

                    RaiseManipulationObjectChangeEvent(ObjectManipulation.Move);

                    //UI Changes
                    _gameCanvasController.ChangeButtonInteractable(true);
                    _gameCanvasController.ShowPlacedItemBTN(true);
                    _gameCanvasController.Training.SwitchManipulationState(true);
                    _gameCanvasController.EnabledMoveBTN();
                    _gameCanvasController.DisableInstruction();

                    _gameCanvasController.Training.ShowMoveRotate();
                }
            }
        }

        //show all ARPlane
        public void ShowAllPlane()
        {
            _surfaceManager.ShowDotsPlane(true);
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

        public void ResetAllPlane()
        {
            //arPlaneManager.ResetTrackables();

            StartCoroutine(DelayCreateManager());
        }

        private IEnumerator DelayCreateManager()
        {
            Destroy(arPlaneManager);
            yield return new WaitForSeconds(1f);
            arPlaneManager = arPlaneManagerParent.AddComponent<ARPlaneManager>();
            arPlaneManager.planePrefab = arPlaneManagerTemplate.planePrefab;
        }
        #endregion

        #region Event

        public void RaiseStateChangeEvent(GameState gameState)
        {
            if (onStateChange != null)
            {
                onStateChange(gameState);
            }
        }

        public void RaiseManipulationObjectChangeEvent(ObjectManipulation gameState)
        {
            if (onObjectManipulationChange != null)
            {
                onObjectManipulationChange(gameState);
            }
        }

        private void ChangeState(GameState gameState)
        {
            currentGameState = gameState;
        }

        private void ChangeObjectManipulation(ObjectManipulation newState)
        {
            currentObjectManipulation = newState;
        }

        #endregion
    }
}
