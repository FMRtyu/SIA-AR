using SIAairportSecurity.Training;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIAairportSecurity.FileInsert
{
    public class InsertEditCanvasController : MonoBehaviour
    {
        [Header("Fade")]
        [SerializeField] private CanvasGroup _fadeObject;
        [SerializeField] private float _fadeTime;

        //Drags = the different menus we have
        public _MenuStateInsertEdit[] allMenus;

        //The states we can choose from
        public enum MenuState
        {
            Inital, Selection, Edit
        }

        //State-object dictionary to make it easier to activate a menu 
        private Dictionary<MenuState, _MenuStateInsertEdit> menuDictionary = new Dictionary<MenuState, _MenuStateInsertEdit>();

        //The current active menu
        private _MenuStateInsertEdit activeState;

        //To easier jump back one step, we can use a stack
        //This was also suggested in the Game Programming Patterns book
        //If so we don't have to hard-code in each state what happens when we jump back one step
        private Stack<MenuState> stateHistory = new Stack<MenuState>();

        protected InsertEditController _insertEditController;

        private int _itemSelected;

        public void SetController(InsertEditController _insertEditController)
        {
            this._insertEditController = _insertEditController;
            init();
        }

        void Start()
        {
            
        }



        void Update()
        {
            //Jump back one menu step when we press escape
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                activeState.JumpBack();
            }
        }

        private void init()
        {
            //Put all menus into a dictionary
            foreach (_MenuStateInsertEdit menu in allMenus)
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
            SetActiveState(MenuState.Inital);
            FadeIn();
        }

        //Jump back one step = what happens when we press escape or one of the back buttons
        public void JumpBack()
        {
            //If we have just one item in the stack then, it means we are at the state we set at start, so we have to jump forward
            if (stateHistory.Count <= 1)
            {
                //SetActiveState(MenuState.Inital);
                FadeOutBack();
            }
            else
            {
                //Remove one from the stack
                stateHistory.Pop();

                //Activate the menu that's on the top of the stack
                SetActiveState(stateHistory.Peek(), isJumpingBack: true);
            }
        }

        #region Menu Operation
        //Activate a menu
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

        #region GetData

        public Dictionary<int, (Sprite, string, bool, bool)> GetListData()
        {
            return _insertEditController.GetSelectionData();
        }

        #endregion

        #region SetData

        public void SetObject(int objectIndex)
        {
            _itemSelected = objectIndex;
        }

        #endregion

        #region

        private void FadeIn()
        {
            LeanTween.alphaCanvas(_fadeObject, to: 0, _fadeTime).setOnComplete(() =>
            {
                _fadeObject.blocksRaycasts = false;
                _fadeObject.interactable = false;
            });
        }

        private void FadeOutBack()
        {
            _fadeObject.blocksRaycasts = true;
            _fadeObject.interactable = true;
            LeanTween.alphaCanvas(_fadeObject, to: 1, _fadeTime).setOnComplete(() =>
            {
                _insertEditController.BackToTraining();
            });
        }

        #endregion

        public void PlayButtonSound()
        {
            _insertEditController.PlayButtonSound();
        }
        //Quit game
        public void QuitGame()
        {
            Debug.Log("You quit game!");

            Application.Quit();
        }
    }
}
