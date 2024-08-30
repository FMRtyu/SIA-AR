using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        [SerializeField] private GameObject _tapInstruction;
        

        //The states we can choose from
        public enum MenuState
        {
            Splash, Selection, Training
        }

        //State-object dictionary to make it easier to activate a menu 
        private Dictionary<MenuState, _MenuState> menuDictionary = new Dictionary<MenuState, _MenuState>();

        //The current active menu
        public _MenuState activeState { get; private set; }
        private Stack<MenuState> stateHistory = new Stack<MenuState>();

        protected GamePlayController _gamePlayController;

        private int _itemSelected = -1;
        private string _itemSelectedPath;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region SetData

        public void InitState(GamePlayController gamePlayController)
        {
            this._gamePlayController = gamePlayController;
            initMenuState();
            FadeIn();
        }

        public void SetObject(int objectIndex)
        {
            _itemSelected = objectIndex;
        }

        public void SetObject(string objectIndex)
        {
            _itemSelectedPath = objectIndex;
        }

        public void ShowConformedBTN(bool Condition)
        {
            if (activeState.state == MenuState.Training)
            {
                activeState.gameObject.GetComponent<Training>().ShowConformButton(Condition);
            }
        }

        public void JumpToTraining()
        {
            if (_itemSelected > -1)
            {
                _gamePlayController.SetGameObject(_itemSelected);
            }else if (_itemSelectedPath != null)
            {
                _gamePlayController.SetGameObject(_itemSelectedPath);
            }

            EnableDisableInstrruction(true);

            SetActiveState(MenuState.Training);


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

        public void EnableDisableInstrruction(bool Intruction)
        {
            _tapInstruction.SetActive(Intruction);
        }

        public void ResetObject()
        {
            _gamePlayController.ResetObject();
            _gamePlayController.ShowAllPlane();

            _itemSelected = -1;
            _itemSelectedPath = null;
        }

        public void ConformObject()
        {
            _gamePlayController.ConformObjectPosition();
        }

        #endregion

        #region GetData

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

        public void SetActiveState(MenuState newState, bool isJumpingBack = false)
        {

            //First check if this menu exists
            if (!menuDictionary.ContainsKey(newState))
            {
                Debug.LogWarning($"The key <b>{newState}</b> doesn't exist so you can't activate the menu!");

                return;
            }

            //Deactivate the old state
            if (activeState != null)
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

        public void FadeOutChangeScene()
        {
            fadeImgObject.blocksRaycasts = true;
            fadeImgObject.interactable = true;
            LeanTween.alphaCanvas(fadeImgObject, to: 1, fadeTime).setOnComplete(() =>
            {
                SceneManager.LoadScene(2);
            });
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

        public void SwitchToRotate()
        {
            _gamePlayController.SwitchToRotate();
        }

        public void ShowHideMoveRotateBTN(bool Condition)
        {
            GetTrainingScript().ShowHideMoveRotateBTN(Condition);
        }

        public void ResetMoveRotate()
        {
            _gamePlayController.ResetMoveRotate();
        }
        #endregion

        public void PlayButtonSound()
        {
            _gamePlayController.PlayButtonSound();
        }
    }
}
