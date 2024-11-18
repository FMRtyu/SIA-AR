using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SIAairportSecurity.Training
{
    public class GameCanvasController : MonoBehaviour
    {
        [Header("Fade")]
        public CanvasGroup fadeImgObject;
        public float fadeTime;

        [Header("All Menus")]
        //Drags = the different menus we have
        public _MenuState[] allMenus;

        //State-object dictionary to make it easier to activate a menu 
        private Dictionary<MenuState, _MenuState> menuDictionary = new Dictionary<MenuState, _MenuState>();

        //The current active menu
        public _MenuState activeState { get; private set; }
        private Stack<MenuState> stateHistory = new Stack<MenuState>();

        protected GamePlayController _gamePlayController;

        private int _itemSelected;

        public bool currentMoveRotateBTNState {  get; private set; }
        public bool isBacktoSelection {  get; private set; }

        [SerializeField] private MenuState currentMenuState;
        public delegate void OnMenuStateChanged(MenuState currentGameState);
        public event OnMenuStateChanged onStateChange;
        // Update is called once per frame
        void Update()
        {

        }

        #region SetData
        public void SetisBacktoSelection(bool newState)
        {
            isBacktoSelection = newState;
        }

        public void SetCurrentMoveRotateBTNState(bool newState)
        {
            currentMoveRotateBTNState = newState;
        }
        public void InitState(GamePlayController gamePlayController)
        {
            this._gamePlayController = gamePlayController;
            initMenuState();
            FadeIn();
            _gamePlayController.onStateChange += ChangeActiveSubUI;
            onStateChange += ChangeState;
        }

        public void SetObject(int objectIndex)
        {
            _itemSelected = objectIndex;
        }

        public void ShowPlacedItemBTN(bool Condition)
        {
            if (activeState.state == MenuState.Training)
            {
                activeState.gameObject.GetComponent<Training>().ShowConformButton(Condition);
            }
        }

        public void JumpToTraining()
        {
            _gamePlayController.SetGameObject(_itemSelected);
            _gamePlayController.SetIsConfirmedPosition(false);

            SetActiveState(MenuState.Training);
        }

        public void BackToTraining()
        {
            SetActiveState(MenuState.Training);

            if (true)
            {

            }
        }

        //Jump back one step = what happens when we press escape or one of the back buttons
        public void JumpBack()
        {
            //If we have just one item in the stack then, it means we are at the state we set at start, so we have to jump forward
            if (stateHistory.Count <= 1)
            {
                SetActiveState(MenuState.Selection);
            }
            else
            {
                //Remove one from the stack
                stateHistory.Pop();

                //Activate the menu that's on the top of the stack
                SetActiveState(stateHistory.Peek(), isJumpingBack: true);
            }
        }

        public void ResetObject()
        {
            _gamePlayController.ResetObject();
            _gamePlayController.ShowAllPlane();
        }

        public void ConfirmObject()
        {
            _gamePlayController.ConfirmObjectPosition();
        }
        public void ResetPlane()
        {
            _gamePlayController.ResetPlane();
        }

        public void FinishScan()
        {
            _gamePlayController.RaiseStateChangeEvent(GameState.PlaceItem);
        }

        public void ConfirmItemPlacement()
        {
            _gamePlayController.RaiseStateChangeEvent(GameState.Gameplay);
        }

        public void DisableInstruction()
        {
            activeState.GetComponent<Training>().DisableInstruction();
        }
        #endregion

        #region GetData

        public bool GetIsConfirmedPosition()
        {
            return _gamePlayController.GetIsConfirmedPosition();
        }
        public bool GetIfObjectConfirmed()
        {
            return _gamePlayController.isSpawnConformed;
        }
        public Dictionary<int, (Sprite,string, bool, bool)> GetListData()
        {
            return _gamePlayController.GetSelectionData();
        }

        private Training GetTrainingScript()
        {
            Training trainingMenu = null;
            foreach (_MenuState item in allMenus)
            {
                if (item.state == MenuState.Training)
                {
                    trainingMenu = item.GetComponent<Training>();
                }
            }
            return trainingMenu;
        }

        public bool CheckARPlaneExist()
        {
            return _gamePlayController.CheckARPlaneExist();
        }
        #endregion

        #region MenuState

        private void initMenuState()
        {
            //Put all menus into a dictionary
            foreach (_MenuState menu in allMenus)
            {
                if (menu == null)
                {
                    continue;
                }

                //Inject a reference to this script into all menus
                menu.InitState(menuController: this);

                //Check if this key already exists, because it means we have forgotten to give a menu its unique key
                if (menuDictionary.ContainsKey(menu.state))
                {
                    Debug.LogWarning($"The key <b>{menu.state}</b> already exists in the menu dictionary!");

                    continue;
                }

                menuDictionary.Add(menu.state, menu);
            }

            //Deactivate all menus
            foreach (MenuState state in menuDictionary.Keys)
            {
                menuDictionary[state].gameObject.SetActive(false);
            }

            //Activate the default menu
            SetActiveState(MenuState.Selection);
        }
        // set activated
        public void SetActiveState(MenuState newState, bool isJumpingBack = false)
        {

            //First check if this menu exists
            if (!menuDictionary.ContainsKey(newState))
            {
                Debug.LogWarning($"The key <b>{newState}</b> doesn't exist so you can't activate the menu!");

                return;
            }

            //Deactivate the old state
            if (activeState != null && newState != MenuState.Quit)
            {
                activeState.gameObject.SetActive(false);
            }

            //Activate the new state
            activeState = menuDictionary[newState];

            activeState.gameObject.SetActive(true);

            //If we are jumping back we shouldn't add to history because then we will get doubles
            if (!isJumpingBack)
            {
                stateHistory.Push(newState);
            }

            RaiseStateChangeEvent(newState);
        }

        #endregion

        #region FadeScreen

        public void FadeIn()
        {
            LeanTween.alphaCanvas(fadeImgObject, to: 0, fadeTime).setOnComplete(() =>
            {
                fadeImgObject.blocksRaycasts = false;
            });
        }

        public void FadeOut()
        {
            fadeImgObject.blocksRaycasts = true;
            LeanTween.alphaCanvas(fadeImgObject, to: 1, fadeTime);
        }

        public void FadeOutQuit()
        {
            fadeImgObject.blocksRaycasts = true;
            LeanTween.alphaCanvas(fadeImgObject, to: 1, fadeTime).setOnComplete(() =>
            {
                Application.Quit();
            });
        }

        #endregion

        #region MoveRotateBTN

        public void SwitchToMove()
        {
            _gamePlayController.SwitchToMove();
        }

        public void EnabledMoveBTN()
        {
            if (activeState.state == MenuState.Training)
            {
                activeState.GetComponent<Training>().SwitchToMove();
            }
        }

        public void SwitchToRotate()
        {
            _gamePlayController.SwitchToRotate();
        }

        public void ResetMoveRotate()
        {
            _gamePlayController.ResetMoveRotate();
        }

        public void SnapXAxis()
        {
            _gamePlayController.SnapObjectXAxis();
        }
        public void SnapYAxis()
        {
            _gamePlayController.SnapObjectYAxis();
        }

        public void ChangeButtonInteractable(bool newCondition)
        {
            if (activeState.state == MenuState.Training)
            {
                activeState.GetComponent<Training>().ChangeButtonInteractable(newCondition);
            }
        }


        #endregion

        #region Event

        public void RaiseStateChangeEvent(MenuState menuState)
        {
            if (onStateChange != null)
            {
                onStateChange(menuState);
            }
        }

        private void ChangeState(MenuState menuState)
        {
            currentMenuState = menuState;
        }

        #endregion

        private void ChangeActiveSubUI(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Scanning:
                    activeState.GetComponent<Training>().ShowScanningSurfaceAnimation();
                    break;
                case GameState.MapArea:
                    activeState.GetComponent<Training>().ShowMapTheAreaInstruction();
                    break;
                case GameState.PlaceItem:
                    activeState.GetComponent<Training>().ShowPlaceInstruction();
                    break;
                case GameState.Gameplay:
                    activeState.GetComponent<Training>().ConfirmPosition();
                    ConfirmObject();
                    break;
                default:
                    Debug.LogError("Value was not set");
                    break;
            }
            
        }
        public void PlayButtonSound()
        {
            _gamePlayController.PlayButtonSound();
        }

        public GamePlayController GetGamePlayController() { return _gamePlayController; }

        public void RaisedGameState(GameState newState)
        {
            _gamePlayController.RaiseStateChangeEvent(newState);
        }
    }
}
