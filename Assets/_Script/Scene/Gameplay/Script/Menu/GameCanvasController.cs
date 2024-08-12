using System.Collections;
using System.Collections.Generic;
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

        [SerializeField] private GameObject _tapInstruction;

        [Header("Gizmo BTN property")]
        [SerializeField] private GameObject _gizmoParent;
        [SerializeField] private Button _gizmomoveBTN;
        [SerializeField] private Button _gizmoRotateBTN;

        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private Sprite _unselectedSprite;

        [SerializeField] private Image _BTNMove;
        [SerializeField] private Image _BTNMoveIcon;
        [SerializeField] private Sprite _selectedMoveSprite;
        [SerializeField] private Sprite _unselectedMoveSprite;

        [SerializeField] private Image _BTNRotate;
        [SerializeField] private Image _BTNRotateIcon;
        [SerializeField] private Sprite _selectedRotateSprite;
        [SerializeField] private Sprite _unselectedRotateSprite;
        public bool isGizmoMove = true;

        //The states we can choose from
        public enum MenuState
        {
            Splash, Selection, Training
        }

        //State-object dictionary to make it easier to activate a menu 
        private Dictionary<MenuState, _MenuState> menuDictionary = new Dictionary<MenuState, _MenuState>();

        //The current active menu
        public _MenuState activeState {  get; private set; }
        private Stack<MenuState> stateHistory = new Stack<MenuState>();

        protected GamePlayController gamePlayController;

        private int _itemSelected;

        // Start is called before the first frame update
        void Awake()
        {
            initMenuState();
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region SetData

        public void InitState(GamePlayController gamePlayController)
        {
            this.gamePlayController = gamePlayController;
        }

        public void SetObject(int objectIndex)
        {
            _itemSelected = objectIndex;
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
            gamePlayController.SetGameObject(_itemSelected);

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

            ShowGizmoGroupBTN(!Intruction);
        }

        public void ResetObject()
        {
            gamePlayController.ResetObject();
            gamePlayController.ShowAllPlane();
        }

        public void ConformObject()
        {
            gamePlayController.ConformObjectPosition();

            ResetGizmoBTN();
        }

        #endregion

        #region GetData

        public Dictionary<int, (Sprite, bool, bool)> GetListData()
        {
            return gamePlayController.GetSelectionData();
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
            SetActiveState(MenuState.Splash);

            //gizmos btn
            InitGizmoBTN();
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

        public void FadeOutQuit()
        {
            fadeImgObject.blocksRaycasts = true;
            LeanTween.alphaCanvas(fadeImgObject, to: 1, fadeTime).setOnComplete(() =>
            {
                Application.Quit();
            });
        }

        #endregion

        #region GizmoShow

        private void InitGizmoBTN()
        {
            ShowGizmoGroupBTN(false);
        }

        public void ShowGizmoGroupBTN(bool condition)
        {
            _gizmoParent.SetActive(condition);
        }

        public void ShowMoveGizmo()
        {
            gamePlayController.SwitchGizmoMove();

            //switch btn sprite
            _gizmomoveBTN.interactable = false;
            _gizmoRotateBTN.interactable = true;

            //move
            _BTNMove.sprite = _selectedSprite;
            _BTNMoveIcon.sprite = _selectedMoveSprite;

            //rotate
            _BTNRotate.sprite = _unselectedSprite;
            _BTNRotateIcon.sprite = _unselectedRotateSprite;
        }

        public void ShowRotateGizmo()
        {
            gamePlayController.SwitchGizmoRotate();

            //switch btn sprite
            _gizmomoveBTN.interactable = true;
            _gizmoRotateBTN.interactable = false;

            //move

            _BTNMove.sprite = _unselectedSprite;
            _BTNMoveIcon.sprite = _unselectedMoveSprite;

            //rotate

            _BTNRotate.sprite = _selectedSprite;
            _BTNRotateIcon.sprite = _selectedRotateSprite;
        }

        private void ResetGizmoBTN()
        {
            ShowGizmoGroupBTN(false);

            //switch btn sprite
            _gizmomoveBTN.interactable = false;
            _gizmoRotateBTN.interactable = true;

            //move
            _BTNMove.sprite = _selectedSprite;
            _BTNMoveIcon.sprite = _selectedMoveSprite;

            //rotate
            _BTNRotate.sprite = _unselectedSprite;
            _BTNRotateIcon.sprite = _unselectedRotateSprite;
        }
        #endregion
    }
}
