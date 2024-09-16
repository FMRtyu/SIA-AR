using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SIAairportSecurity.Training
{
    public class SubCanvasTraining : MonoBehaviour
    {
        [Header("All Menus")]
        //Drags = the different menus we have
        public _TrainingSubState[] allMenus;

        private Stack<GameState> stateHistory = new Stack<GameState>();
        private Dictionary<GameState, _TrainingSubState> menuDictionary = new Dictionary<GameState, _TrainingSubState>();
        public _TrainingSubState activeState { get; private set; }

        protected Training _trainingMenu;

        public void initTraining(Training _trainingMenu)
        {
            this._trainingMenu = _trainingMenu;
        }

        private GamePlayController _gamePlayController;
        // Start is called before the first frame update
        void Awake()
        {
            init();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void init()
        {
            initMenuState();

            _gamePlayController = FindAnyObjectByType<GamePlayController>();

            _gamePlayController.onStateChange += ChangeInstructionPanel;
        }

        #region StateMenu
        private void initMenuState()
        {
            //Put all menus into a dictionary
            foreach (_TrainingSubState menu in allMenus)
            {
                if (menu == null)
                {
                    continue;
                }

                //Inject a reference to this script into all menus
                menu.InitState(_trainingController: this);

                //Check if this key already exists, because it means we have forgotten to give a menu its unique key
                if (menuDictionary.ContainsKey(menu.state))
                {
                    Debug.LogWarning($"The key <b>{menu.state}</b> already exists in the menu dictionary!");

                    continue;
                }

                menuDictionary.Add(menu.state, menu);
            }

            //Deactivate all menus
            foreach (GameState state in menuDictionary.Keys)
            {
                menuDictionary[state].gameObject.SetActive(false);
            }

            //Activate the default menu
            SetActiveState(GameState.Scanning);
        }

        private void ChangeActiveMenu(GameState newState)
        {
            SetActiveState(newState);
        }

        public void JumpBack()
        {
            //If we have just one item in the stack then, it means we are at the state we set at start, so we have to jump forward
            if (stateHistory.Count <= 1)
            {
                SetActiveState(GameState.Scanning);
            }
            else
            {
                //Remove one from the stack
                stateHistory.Pop();

                //Activate the menu that's on the top of the stack
                SetActiveState(stateHistory.Peek(), isJumpingBack: true);
            }
        }

        public void SetActiveState(GameState newState, bool isJumpingBack = false)
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

        #region operation

        public void ShowMappingInstruction(bool showInstruction)
        {
            if (activeState.state == GameState.Scanning)
            {
                activeState.gameObject.GetComponent<ScanSurface>().ShowMappingInstruction(showInstruction);
            }
        }

        public void ResetPlane(Button button)
        {
            _trainingMenu.ResetPlane(button);
        }

        public void ShowConformButton()
        {
            _trainingMenu.ShowConformButton(true);
        }

        public bool CheckMenuToSpawn()
        {
            if (activeState.state == GameState.PlaceItem)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void OpenCloseInstructionTap(bool condition)
        {
            if (activeState.state == GameState.PlaceItem)
            {
                activeState.GetComponent<PlaceItem>().OpenCloseInstructionTap(condition);
            }
        }

        private void ChangeInstructionPanel(GameState gameState)
        {

            activeState.GetComponent<PlaceItem>().ChangeInstructionPanel(gameState);
        }
        #endregion

        public GamePlayController GetGameplayController()
        {
            return _gamePlayController;
        }
    }
}
