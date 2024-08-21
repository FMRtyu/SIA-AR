using System.Collections;
using System.Collections.Generic;
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

        [Header("RotateMoveBTN")]
        [SerializeField] private Animator _moveRotateAnim;

        //Specific for this state
        public override void InitState(GameCanvasController menuController)
        {
            base.InitState(menuController);

            state = GameCanvasController.MenuState.Training;
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

            SwitchToMove();
        }

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

        #region operation

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
            _moveRotateAnim.SetBool("InitalOpen", Condition);
        }

        public void ConformPosition()
        {
            _menuCanvasController.ConformObject();
            ShowConformButton(false);
            _moveRotateAnim.SetBool("Open", false);
        }

        public void ReopenMoveRotateBTN()
        {
            _moveRotateAnim.SetBool("Open", true);
            ShowConformButton(true);
            _menuCanvasController.ResetMoveRotate();
            SwitchToMove();
        }
        #endregion

        public void QuitGame()
        {
            _menuCanvasController.FadeOutQuit();
        }

        #endregion

        #region MoveRotateBTN

        public void SwitchToMove()
        {
            _MoveBTN.interactable = false;
            _RotateBTN.interactable = true;

            _menuCanvasController.SwitchToMove();
        }

        public void SwitchToRotate()
        {
            _MoveBTN.interactable = true;
            _RotateBTN.interactable = false;
            _menuCanvasController.SwitchToRotate();
        }

        public void ShowHideMoveRotateBTN(bool Condition)
        {
            //_moveRotateParent.SetActive(Condition);
        }

        #endregion

        public void PlayButtonSound()
        {
            _menuCanvasController.PlayButtonSound();
        }
    }
}
