using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SIAairportSecurity.Training
{
    public class PlaceItem : _TrainingSubState
    {
        [SerializeField] private GameObject _instructionPanel;

        [Header("On/Of Scan Button")]
        [SerializeField] private Image _onOfButtonImage;
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

            if (_isButtonOn)
            {
                _onOfButtonImage.sprite = _onSprite;
            }
            else
            {
                _onOfButtonImage.sprite = _offSprite;
            }
        }

        public void ChangeOnOffSprite(bool condition)
        {
            _isButtonOn = condition;

            if (_isButtonOn)
            {
                _onOfButtonImage.sprite = _onSprite;
            }
            else
            {
                _onOfButtonImage.sprite = _offSprite;
            }
        }
    }
}
