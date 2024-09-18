using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SIAairportSecurity.Training
{
    public class Training : _MenuState
    {
        [Header("Menu Container")]
        [SerializeField] private RectTransform _menuContainer;
        [SerializeField] private float _bottomOffset = 10f; // Offset from the bottom of the screen
        [SerializeField] private float _moveSpeed = 1f; // movement speed to show menu container
        [SerializeField] private GameObject _conformBTN;
        private Vector2 _menuContainerInitialPos;
        private bool _isMenuOpen = false;

        [Header("RotateMoveUI")]
        [SerializeField] private GameObject _moveRotateParent;
        [SerializeField] private Button _MoveBTN;
        [SerializeField] private Button _RotateBTN;

        [Header("Move Rotate Icon")]
        [SerializeField] private Image _MoveBTNIcon;
        [SerializeField] private Image _RotateBTNIcon;
        [SerializeField] private Sprite _moveActive;
        [SerializeField] private Sprite _moveInactive;
        [SerializeField] private Sprite _rotateActive;
        [SerializeField] private Sprite _rotateInactive;

        [Header("Information Icon")]
        [SerializeField] private Sprite _infoInactiveIcon;
        [SerializeField] private Sprite _infoActiveIcon;

        [Header("ButtonSprite")]
        [SerializeField] private Sprite _inactiveSprite;
        [SerializeField] private Sprite _activeSprite;

        [Header("RotateMoveAnimation")]
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

        [Header("Place Item")]
        [SerializeField] private Image _onOfButtonImage;
        [SerializeField] private Image _onOfButtonImageIcon;
        [SerializeField] private Sprite _inactiveImageIcon;
        [SerializeField] private Sprite _activeImageIcon;

        [SerializeField] private Sprite _onSprite;
        [SerializeField] private Sprite _offSprite;
        private bool _isButtonOn;

        //Specific for this state
        public override void InitState(GameCanvasController menuController)
        {
            base.InitState(menuController);

            state = GameCanvasController.MenuState.Training;
            init();
        }

        private void Awake()
        {
            //save initial container pos
            _menuContainerInitialPos = _menuContainer.anchoredPosition;

        }

        private void OnEnable()
        {
            //move menu container down
            MoveToBottom(true);

            ShowConformButton(false);

            if (isAlreadyOpen)
            {
                _moveRotateAnim.SetBool("AleadyOpen", true);
                ChangeButtonInteractable(false);
            }
            //SwitchToMove();
        }

        private void init()
        {
            _instructionPanelInitScale = _instructionPanel.transform.localScale;
            _scanInstructionInitScale = _scanInstruction.transform.localScale;

            _instructionPanel.transform.localScale = Vector3.zero;
            _finishResetBTN.SetActive(false);
            _scanInstruction.SetActive(false);
        }

        #region Menupanel
        private void MoveToBottom(bool isInit)
        {
            _isMenuOpen = false;
            _menuContainer.gameObject.SetActive(true);

            // Get the height of the parent RectTransform
            RectTransform parentRectTransform = _menuContainer.parent.GetComponent<RectTransform>();

            // Calculate the new anchored position
            float bottomYPosition = -parentRectTransform.rect.height / 2 + _menuContainer.rect.height / 2 + _bottomOffset;
            Vector2 newPos = new Vector2(_menuContainer.anchoredPosition.x, bottomYPosition);

            if (isInit)
            {
                // Set the anchored position
                _menuContainer.anchoredPosition = newPos;
            }
            else
            {
                LeanTween.move(_menuContainer, newPos, _moveSpeed).setEase(LeanTweenType.easeInOutQuad);
            }
        }

        private void MoveToUp()
        {
            _isMenuOpen = true;
            LeanTween.move(_menuContainer, _menuContainerInitialPos, _moveSpeed).setEase(LeanTweenType.easeInOutQuad);
        }
        #endregion

        #region operation
        public void JumpToQuit()
        {
            _menuCanvasController.SetActiveState(GameCanvasController.MenuState.Quit);
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
            _menuCanvasController.ResetObject();

            _menuCanvasController.SetActiveState(GameCanvasController.MenuState.Selection);
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
            PlayButtonSound();
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
                button.GetComponent<Image>().sprite = _inactiveSprite;
                button.transform.Find("IconIMG").GetComponent<Image>().sprite = _infoInactiveIcon;
                button.gameObject.GetComponentInChildren<TMP_Text>().color = Color.white;
            }
            else
            {
                _infoPanel.SetActive(true);
                button.GetComponent<Image>().sprite = _activeSprite;
                button.transform.Find("IconIMG").GetComponent<Image>().sprite = _infoActiveIcon;
                button.gameObject.GetComponentInChildren<TMP_Text>().color = Color.black;
            }
        }

        public void QuitGame()
        {
            _menuCanvasController.FadeOutQuit();
        }

        public void ResetPlane(Button button)
        {
            StartCoroutine(ButtonDelay.EnabledBTNAfterSecond(button));
            _loadingCanvasGroup.gameObject.SetActive(true);

            ChangeScanSurfaceSprite(true);
            if (!_menuCanvasController.GetIfObjectConfirmed())
            {
                ChangeButtonInteractable(false);
                ShowConformButton(false);
                SwitchRotateMoveToDefault();
                ScaleUpAnimation(_instructionPanel, _instructionPanelInitScale);
            }
            else
            {
                ShowConformButton(true);
            }

            LeanTween.alphaCanvas(_loadingCanvasGroup, to: 1, 1f).setOnComplete(() =>
            {
                _menuCanvasController.ResetPlane();
                LeanTween.alphaCanvas(_loadingCanvasGroup, to: 0, 1f).setDelay(1f).setOnComplete(() =>
                {
                    _loadingCanvasGroup.gameObject.SetActive(false);
                });
            });
        }
        #endregion

        #region MoveRotateBTN

        public void SwitchRotateMoveToDefault()
        {
            _rotateAdvanceAnim.SetBool("isOpen", false);

            _MoveBTN.GetComponent<Image>().sprite = _inactiveSprite;
            _RotateBTN.GetComponent<Image>().sprite = _inactiveSprite;

            _MoveBTNIcon.sprite = _moveInactive;
            _RotateBTNIcon.sprite = _rotateInactive;
        }

        public void SwitchToMove()
        {
            _MoveBTN.GetComponent<Image>().sprite = _activeSprite;
            _RotateBTN.GetComponent<Image>().sprite = _inactiveSprite;

            _MoveBTNIcon.sprite = _moveActive;
            _RotateBTNIcon.sprite = _rotateInactive;

            _MoveBTN.GetComponentInChildren<TMP_Text>().color = Color.black;
            _RotateBTN.GetComponentInChildren<TMP_Text>().color = Color.white;

            _menuCanvasController.SwitchToMove();
            _rotateAdvanceAnim.SetBool("isOpen", false);
        }

        public void ChangeButtonInteractable(bool newCondition)
        {
            _MoveBTN.interactable = newCondition;
            _RotateBTN.interactable = newCondition;

            if (newCondition)
            {
                _MoveBTNIcon.color = Color.white;
                _RotateBTNIcon.color = Color.white;

                _MoveBTN.GetComponentInChildren<TMP_Text>().color = Color.white;
                _RotateBTN.GetComponentInChildren<TMP_Text>().color = Color.white;
            }
            else
            {
                _MoveBTNIcon.color = Color.gray;
                _RotateBTNIcon.color = Color.gray;

                _MoveBTN.GetComponentInChildren<TMP_Text>().color = Color.gray;
                _RotateBTN.GetComponentInChildren<TMP_Text>().color = Color.gray;
            }
        }

        public void SwitchToRotate()
        {
            _MoveBTN.GetComponent<Image>().sprite = _inactiveSprite;
            _RotateBTN.GetComponent<Image>().sprite = _activeSprite;

            _MoveBTNIcon.sprite = _moveInactive;
            _RotateBTNIcon.sprite = _rotateActive;

            _MoveBTN.GetComponentInChildren<TMP_Text>().color = Color.white;
            _RotateBTN.GetComponentInChildren<TMP_Text>().color = Color.black;

            _menuCanvasController.SwitchToRotate();

            if (!_rotateAdvanceAnim.GetBool("isOpen"))
            {
                _rotateAdvanceAnim.SetBool("isOpen", true);
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
            StartCoroutine(EnabledBTNAfterSecond(button));
        }

        public void SnapYRotation(Button button)
        {
            _menuCanvasController.SnapYAxis();
            button.interactable = false;
            StartCoroutine(EnabledBTNAfterSecond(button));
        }

        private IEnumerator EnabledBTNAfterSecond(Button button)
        {
            yield return new WaitForSeconds(0.3f);
            button.interactable = true;
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
            ScaleUpAnimation(_instructionPanel, _instructionPanelInitScale);
            _finishResetBTN.SetActive(true);
        }

        public void ShowPlaceInstruction()
        {
            if (!_menuCanvasController.GetIfObjectConfirmed())
            {
                _instructionPanel.transform.localScale = Vector3.zero;
                _instructionPanel.GetComponentInChildren<TMP_Text>().text = "Tap to Place Item";
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
            _isButtonOn = !_isButtonOn;
            TMP_Text tempTXT = _onOfButtonImage.GetComponentInChildren<TMP_Text>();

            if (_isButtonOn)
            {
                _onOfButtonImageIcon.sprite = _activeImageIcon;
                _onOfButtonImage.sprite = _onSprite;
                tempTXT.text = "Scan\nOn";
                tempTXT.color = Color.black;
            }
            else
            {
                _onOfButtonImageIcon.sprite = _inactiveImageIcon;
                _onOfButtonImage.sprite = _offSprite;
                tempTXT.text = "Scan\nOff";
                tempTXT.color = Color.white;
            }
        }

        public void ChangeScanSurfaceSprite(bool condition)
        {
            _isButtonOn = condition;
            TMP_Text tempTXT = _onOfButtonImage.GetComponentInChildren<TMP_Text>();

            if (_isButtonOn)
            {
                _onOfButtonImageIcon.sprite = _activeImageIcon;
                _onOfButtonImage.sprite = _onSprite;
                tempTXT.text = "Scan\nOn";
                tempTXT.color = Color.black;
            }
            else
            {
                _onOfButtonImageIcon.sprite = _inactiveImageIcon;
                _onOfButtonImage.sprite = _offSprite;
                tempTXT.text = "Scan\nOff";
                tempTXT.color = Color.white;
            }
        }
        #endregion
        public void PlayButtonSound()
        {
            _menuCanvasController.PlayButtonSound();
        }
    }
}
