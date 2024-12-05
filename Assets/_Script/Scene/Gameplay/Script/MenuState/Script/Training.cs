using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SIAairportSecurity.Training
{
    public class Training : _MenuState
    {
        [Header("Database")]
        [SerializeField] private SpriteDatabase _spriteDB;

        [Header("Menu Container")]
        [SerializeField] private RectTransform _menuContainer;
        [SerializeField] private Animator _menuAnimator;

        [SerializeField] private float _bottomOffset = 10f; // Offset from the bottom of the screen
        [SerializeField] private float _moveSpeed = 1f; // movement speed to show menu container

        [SerializeField] private GameObject _conformBTN;

        private Vector2 _menuContainerInitialPos;
        private bool _isMenuOpen = false;

        [Header("Rotate and Move property")]
        [SerializeField] private Button _moveBTN;
        [SerializeField] private Button _rotateBTN;
        
        private Image _MoveBTNIcon;
        private Image _RotateBTNIcon;

        [SerializeField] private Animator _moveRotateAnim;
        [SerializeField] private Animator _rotateAdvanceAnim;
        private bool isAlreadyOpen;

        [Header("Loading")]
        [SerializeField] private CanvasGroup _loadingCanvasGroup;

        [Header("Information Panel")]
        [SerializeField] private GameObject _infoPanel;
        [SerializeField] private GameObject _instructionPanel;

        [Header("Scan Surface")]
        [SerializeField] private GameObject _scanInstruction;
        [SerializeField] private GameObject _finishResetBTN;
        private Vector3 _instructionPanelInitScale;
        private Vector3 _scanInstructionInitScale;

        [Header("Scan Button")]
        [SerializeField] private Image _ScanButton;
        private bool _isButtonOn;

        public Button soundButtonToggler;
        //event
        public GameState _gameState;

        //Specific for this state
        public override void InitState(GameCanvasController menuController)
        {
            base.InitState(menuController);

            state = MenuState.Training;
            init();
        }

        private void Awake()
        {
            //save initial container pos
            _menuContainerInitialPos = _menuContainer.anchoredPosition;

            _menuCanvasController.GetGamePlayController().onStateChange += OnGameStateChange;

        }

        private void OnEnable()
        {
            //move menu container down
            MoveToBottom(true);

            if(isAlreadyOpen)
            {
                if (_menuCanvasController.isBacktoSelection && _menuCanvasController.GetIfObjectConfirmed())
                {
                    _moveRotateAnim.SetBool("Open", false);
                    _moveRotateAnim.SetBool("BackClose", true);
                }
                else
                {
                    _moveRotateAnim.SetBool("AleadyOpen", true);
                    _moveRotateAnim.SetBool("BackClose", false);
                }
            }
            if (_menuCanvasController.currentMoveRotateBTNState)
            {
                ChangeButtonInteractable(_menuCanvasController.currentMoveRotateBTNState);
                ShowConformButton(!_menuCanvasController.GetGamePlayController().isSpawnConformed);
                if (_menuCanvasController.isBacktoSelection)
                {

                }
            }
            else
            {
                ChangeButtonInteractable(false);
                ShowConformButton(false);
            }
        }

        private void init()
        {
            _instructionPanelInitScale = _instructionPanel.transform.localScale;
            _scanInstructionInitScale = _scanInstruction.transform.localScale;

            _MoveBTNIcon = _moveBTN.transform.Find("IconIMG").GetComponent<Image>();
            _RotateBTNIcon = _rotateBTN.transform.Find("IconIMG").GetComponent<Image>(); ;

            _instructionPanel.transform.localScale = Vector3.zero;
            _finishResetBTN.SetActive(false);
            _scanInstruction.SetActive(false);
        }

        #region Menupanel
        private void MoveToBottom(bool isInit)
        {
            _isMenuOpen = false;


            if (_menuAnimator)
            {
                _menuAnimator.SetBool("IsMenuOpen", _isMenuOpen);
            }
            else
            {
                CanvasGroup canvasGroup = _menuContainer.GetComponent<CanvasGroup>();
                LeanTween.alphaCanvas(canvasGroup, to: 0f, 0.5f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
                {
                    canvasGroup.blocksRaycasts = false;
                    canvasGroup.interactable = false;
                });
            }
        }

        private void MoveToUp()
        {
            _isMenuOpen = true;

            if (_menuAnimator)
            {
                _menuAnimator.SetBool("IsMenuOpen", _isMenuOpen);
            }
            else
            {
                CanvasGroup canvasGroup = _menuContainer.GetComponent<CanvasGroup>();
                LeanTween.alphaCanvas(canvasGroup, to: 1f, 0.5f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
                {
                    canvasGroup.blocksRaycasts = true;
                    canvasGroup.interactable = true;
                });
            }
        }

        private void OnGameStateChange(GameState newGameState)
        {
            _gameState = newGameState;

            if (_isMenuOpen)
            {
                MoveToUp();
            }
        }
        #endregion

        #region operation

        public void JumpToQuit()
        {
            _menuCanvasController.SetActiveState(MenuState.Quit);
        }

        public void ShowCloseMenuContainer()
        {
            if (_isMenuOpen)
            {
                MoveToBottom(false);
            }
            else
            {
                MoveToUp();
                _isMenuOpen = true;
            }
        }
        public void JumpToSelection()
        {
            //_menuCanvasController.ResetObject();

            //SwitchRotateMoveToDefault();

            _menuCanvasController.SetActiveState(MenuState.Selection); 
        }

        #region ShowHideMoveRotateBTN
        public void ShowConformButton(bool Condition)
        {
            _conformBTN.SetActive(Condition);
        }

        public void ShowTopMenuPanel()
        {
            _moveRotateAnim.SetBool("InitalOpen", true);

            if (!isAlreadyOpen)
            {
                isAlreadyOpen = true;
            }
        }

        public void ConfirmPosition()
        {
            ShowConformButton(false);
            _moveRotateAnim.SetBool("Open", false);
            _rotateAdvanceAnim.SetBool("isOpen", false);

            ScaleUpAnimation(_instructionPanel, Vector3.zero);
            SwitchRotateMoveToDefault();
        }

        public void ReopenMoveRotateBTN()
        {
            _moveRotateAnim.SetBool("Open", true);
            ShowConformButton(true);
            _menuCanvasController.ResetMoveRotate();
            SwitchToMove();

            _menuCanvasController.FinishScan();
        }

        #endregion

        public void ShowHideInfoPanel(Button button)
        {
            if (_infoPanel.active)
            {
                _infoPanel.SetActive(false);
                button.GetComponent<Image>().sprite = _spriteDB.RoundButton.defaultSprite;
                button.transform.Find("IconIMG").GetComponent<Image>().sprite = _spriteDB.Info.defaultSprite;
                button.gameObject.GetComponentInChildren<TMP_Text>().color = Color.white;
            }
            else
            {
                _infoPanel.SetActive(true);
                button.GetComponent<Image>().sprite = _spriteDB.RoundButton.activatedSprite;
                button.transform.Find("IconIMG").GetComponent<Image>().sprite = _spriteDB.Info.activatedSprite;
                button.gameObject.GetComponentInChildren<TMP_Text>().color = Color.black;
            }
        }

        public void ShowHideInfoPanel(bool newCondition)
        {
            _infoPanel.SetActive(newCondition);
        }

        public void QuitGame()
        {
            _menuCanvasController.FadeOutQuit();
        }

        public void ResetMapArea()
        {
            _menuCanvasController.RaisedGameState(GameState.Scanning);
            _scanInstruction.SetActive(true);
            _finishResetBTN.SetActive(false);
            ScaleUpAnimation(_instructionPanel, Vector3.zero);
        }

        public void ResetPlane(Button button)
        {
            _loadingCanvasGroup.gameObject.SetActive(true);

            ChangeScanSurfaceSprite(true);

            ChangeButtonInteractable(false);

            _scanInstruction.SetActive(true);
            _menuCanvasController.GetGamePlayController().RaiseStateChangeEvent(GameState.Scanning);

            SwitchRotateMoveToDefault();
            ScaleUpAnimation(_instructionPanel, Vector3.zero);

            LeanTween.alphaCanvas(_loadingCanvasGroup, to: 1, 1f).setOnComplete(() =>
            {
                _menuCanvasController.ResetPlane();
                LeanTween.alphaCanvas(_loadingCanvasGroup, to: 0, 1f).setDelay(1f).setOnComplete(() =>
                {
                    _loadingCanvasGroup.gameObject.SetActive(false);

                });
            });
        }

        public void ShowInstructionUI()
        {
            ScaleUpAnimation(_instructionPanel, _instructionPanelInitScale);
        }

        public Vector3 GetCurrentInstructionUI()
        {
            return _instructionPanel.transform.localScale;
        }
        #endregion

        #region MoveRotateBTN

        private void SetButtonState(GameObject button, Image icon, Sprite buttonSprite, Sprite iconSprite, Color textColor)
        {
            button.GetComponent<Image>().sprite = buttonSprite;
            icon.sprite = iconSprite;
            button.GetComponentInChildren<TMP_Text>().color = textColor;
        }

        public void SwitchRotateMoveToDefault()
        {
            _rotateAdvanceAnim.SetBool("isOpen", false);

            SetButtonState(_moveBTN.gameObject, _MoveBTNIcon, _spriteDB.RoundButton.defaultSprite, _spriteDB.Move.defaultSprite, Color.white);
            SetButtonState(_rotateBTN.gameObject, _RotateBTNIcon, _spriteDB.RoundButton.defaultSprite, _spriteDB.Rotate.defaultSprite, Color.white);
        }

        public void SwitchToMove()
        {
            SetButtonState(_moveBTN.gameObject, _MoveBTNIcon, _spriteDB.RoundButton.activatedSprite, _spriteDB.Move.activatedSprite, Color.black);
            SetButtonState(_rotateBTN.gameObject, _RotateBTNIcon, _spriteDB.RoundButton.defaultSprite, _spriteDB.Rotate.defaultSprite, Color.white);

            _menuCanvasController.SwitchToMove();
            _rotateAdvanceAnim.SetBool("isOpen", false);
        }

        public void SwitchManipulationState(bool newCondition)
        {
            _moveBTN.interactable = newCondition;
            _rotateBTN.interactable = newCondition;
        }

        public void SwitchToRotate()
        {
            SetButtonState(_moveBTN.gameObject, _MoveBTNIcon, _spriteDB.RoundButton.defaultSprite, _spriteDB.Move.defaultSprite, Color.white);
            SetButtonState(_rotateBTN.gameObject, _RotateBTNIcon, _spriteDB.RoundButton.activatedSprite, _spriteDB.Rotate.activatedSprite, Color.black);

            _menuCanvasController.SwitchToRotate();

            if (!_rotateAdvanceAnim.GetBool("isOpen"))
            {
                _rotateAdvanceAnim.SetBool("isOpen", true);
            }
        }


        public void ChangeButtonInteractable(bool newCondition)
        {
            _menuCanvasController.SetCurrentMoveRotateBTNState(newCondition);
            Debug.Log("changing to " + newCondition);
            _moveBTN.interactable = newCondition;
            _rotateBTN.interactable = newCondition;

            if (newCondition)
            {
                _MoveBTNIcon.color = Color.white;
                _RotateBTNIcon.color = Color.white;

                _moveBTN.GetComponentInChildren<TMP_Text>().color = Color.white;
                _rotateBTN.GetComponentInChildren<TMP_Text>().color = Color.white;
            }
            else
            {
                _MoveBTNIcon.color = Color.gray;
                _RotateBTNIcon.color = Color.gray;

                _moveBTN.GetComponentInChildren<TMP_Text>().color = Color.gray;
                _rotateBTN.GetComponentInChildren<TMP_Text>().color = Color.gray;
            }
        }

        public void HideMoveRotateBTN()
        {
            _moveRotateAnim.SetBool("Back", true);
            _moveRotateAnim.SetBool("Open", false);
            _moveRotateAnim.SetBool("InitalOpen", false);

        }

        public void ShowMoveRotate()
        {
            _moveRotateAnim.SetBool("Back", false);
                _moveRotateAnim.SetBool("Open", true);
        }

        public void SnapXRotation(Button button)
        {
            _menuCanvasController.SnapXAxis();
            button.interactable = false;
        }

        public void SnapYRotation(Button button)
        {
            _menuCanvasController.SnapYAxis();
            button.interactable = false;
        }
        #endregion

        #region SubMenu

        public void ShowScanningSurfaceAnimation()
        {
            ScaleUpAnimation(_scanInstruction, _scanInstructionInitScale);
        }

        public void ShowMapTheAreaInstruction()
        {
            ScaleUpAnimation(_scanInstruction, Vector3.zero);
            _instructionPanel.GetComponentInChildren<TMP_Text>().text = "Map The Area";

            RectTransform rectTransform = _instructionPanel.GetComponent<RectTransform>();

            Vector2 newPosition = rectTransform.anchoredPosition;
            newPosition.y = 865;
            rectTransform.anchoredPosition = newPosition;

            ScaleUpAnimation(_instructionPanel, _instructionPanelInitScale);
            _finishResetBTN.SetActive(true);
        }

        public void ShowPlaceInstruction()
        {
            if (!_menuCanvasController.GetIfObjectConfirmed())
            {
                _instructionPanel.transform.localScale = Vector3.zero;
                _instructionPanel.GetComponentInChildren<TMP_Text>().text = "Tap to Place Item";

                RectTransform rectTransform = _instructionPanel.GetComponent<RectTransform>();

                Vector2 newPosition = rectTransform.anchoredPosition;
                newPosition.y = 620;
                rectTransform.anchoredPosition = newPosition;

                ScaleUpAnimation(_instructionPanel, _instructionPanelInitScale);
            }

            ShowTopMenuPanel();
            _finishResetBTN.SetActive(false);
        }

        public void DisableInstruction()
        {
            ScaleUpAnimation(_instructionPanel, Vector3.zero);
        }

        private void ScaleUpAnimation(GameObject animateObject, Vector3 value)
        {
            LeanTween.scale(animateObject, to: value, 0.5f);
        }

        public void FinishScan()
        {
            _menuCanvasController.FinishScan();
        }

        public void ChangeScanSurfaceSprite()
        {
            // Toggle the state when called by the button
            _isButtonOn = !_isButtonOn;
            UpdateScanSurfaceSprite();
        }

        public void ChangeScanSurfaceSprite(bool condition)
        {
            // Set the state directly when called by the script
            _isButtonOn = condition;
            UpdateScanSurfaceSprite();
        }

        private void UpdateScanSurfaceSprite()
        {
            TMP_Text tempTXT = _ScanButton.GetComponentInChildren<TMP_Text>();

            if (_isButtonOn)
            {
                _ScanButton.transform.Find("IconIMG").GetComponent<Image>().sprite = _spriteDB.Scan.activatedSprite;
                _ScanButton.sprite = _spriteDB.RoundButton.activatedSprite;
                tempTXT.text = "Scan\nOn";
                tempTXT.color = Color.black;
            }
            else
            {
                _ScanButton.transform.Find("IconIMG").GetComponent<Image>().sprite = _spriteDB.Scan.defaultSprite;
                _ScanButton.sprite = _spriteDB.RoundButton.defaultSprite;
                tempTXT.text = "Scan\nOff";
                tempTXT.color = Color.white;
            }
        }

        #endregion
    }
}
