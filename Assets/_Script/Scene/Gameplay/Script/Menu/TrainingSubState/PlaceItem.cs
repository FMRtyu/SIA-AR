using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIAairportSecurity.Training
{
    public class PlaceItem : _TrainingSubState
    {
        [SerializeField] private GameObject _instructionPanel;
        public override void InitState(SubCanvasTraining _trainingController)
        {
            base.InitState(_trainingController);

            state = GameState.PlaceItem;
        }

        private void OnEnable()
        {
            _instructionPanel.SetActive(true);
        }
    }
}
