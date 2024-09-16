using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SIAairportSecurity.Training
{
    public class PlaceItem : _TrainingSubState
    {
        [SerializeField] private GameObject _instructionPanel;

        [Header("On/Of Scan Button")]
        [SerializeField] private Image _onOfButtonImage;
        [SerializeField] private Image _onOfButtonImageIcon;
        [SerializeField] private Sprite _inactiveImageIcon;
        [SerializeField] private Sprite _activeImageIcon;

        [SerializeField] private Sprite _onSprite;
        [SerializeField] private Sprite _offSprite;
        private bool _isButtonOn;
        public override void InitState(SubCanvasTraining _trainingController)
        {
            base.InitState(_trainingController);

            state = GameState.PlaceItem;
        }

        private void OnEnable()
        {
            _instructionPanel.SetActive(true);
        }

        public void ChangeOnOffSprite()
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

        public void ChangeOnOffSprite(bool condition)
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

        public void ResetObject()
        {
            ChangeOnOffSprite(false);
        }

        public void OpenCloseInstructionTap(bool condition)
        {
            _instructionPanel.SetActive(condition);
        }

        public void ChangeInstructionPanel(GameState gameState)
        {
            if (gameState == GameState.PlaceItem)
            {
                OpenCloseInstructionTap(true);
            }
        }
    }
}
