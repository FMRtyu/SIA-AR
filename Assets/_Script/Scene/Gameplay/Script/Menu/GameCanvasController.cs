using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        //The states we can choose from
        public enum MenuState
        {
            Selection, Training
        }

        //State-object dictionary to make it easier to activate a menu 
        private Dictionary<MenuState, _MenuState> menuDictionary = new Dictionary<MenuState, _MenuState>();

        //The current active menu
        public _MenuState activeState {  get; private set; }
        private Stack<MenuState> stateHistory = new Stack<MenuState>();

        protected GamePlayController gamePlayController;

        // Start is called before the first frame update
        void Start()
        {
            FadeIn();
            initMenuState();
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region SetData

        public virtual void InitState(GamePlayController gamePlayController)
        {
            this.gamePlayController = gamePlayController;
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
            SetActiveState(MenuState.Training);
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

        private void FadeIn()
        {
            LeanTween.alphaCanvas(fadeImgObject, to: 0, fadeTime).setOnComplete(() =>
            {
                fadeImgObject.blocksRaycasts = false;
            });
        }

        public void FadeOutQuit()
        {
            fadeImgObject.blocksRaycasts = true;
            LeanTween.alphaCanvas(fadeImgObject, to: 0, fadeTime).setOnComplete(() =>
            {
                Application.Quit();
            });
        }

        #endregion
    }
}
