using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SIAairportSecurity.Training
{
    public class ScanSurface : _TrainingSubState
    {
        [Header("ScanInstruction")]
        [SerializeField] private GameObject _mapInstruction;
        [SerializeField] private GameObject _scanInstruction;
        [SerializeField] private GameObject _finishResetBTN;
        private Vector3 _mapInstructionInitScale;
        private Vector3 _scanInstructionInitScale;
        public override void InitState(SubCanvasTraining _trainingController)
        {
            base.InitState(_trainingController);

            state = GameState.Scanning;
            init();
        }

        private void init()
        {
            _mapInstructionInitScale = _mapInstruction.transform.localScale;
            _scanInstructionInitScale = _scanInstruction.transform.localScale;

            _mapInstruction.transform.localScale = Vector3.zero;
            _finishResetBTN.SetActive(false);
        }

        public void ShowMappingInstruction(bool showInstruction)
        {
            if (showInstruction)
            {
                LeanTween.scale(_mapInstruction, to: _mapInstructionInitScale, 1f).setEase(LeanTweenType.easeInOutExpo);
                LeanTween.scale(_scanInstruction, to: Vector3.zero, 1f).setEase(LeanTweenType.easeInOutExpo);
                _finishResetBTN.SetActive(true);
            }
            else
            {
                LeanTween.scale(_mapInstruction, to: Vector3.zero, 1f).setEase(LeanTweenType.easeInOutExpo);
                LeanTween.scale(_scanInstruction, to: _scanInstructionInitScale, 1f).setEase(LeanTweenType.easeInOutExpo);
                _finishResetBTN.SetActive(false);
            }
        }

        public void ResetPlane(Button button)
        {
            _trainingController.ResetPlane(button);
        }

        public void JumpToPlaceItem()
        {
            _trainingController.SetActiveState(GameState.PlaceItem);
        }

    }
}
