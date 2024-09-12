using SIAairportSecurity.Training;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIAairportSecurity.Training
{
    public class _TrainingSubState : MonoBehaviour
    {
        //Which state is this?
        public GameState state { get; protected set; }


        protected SubCanvasTraining _trainingController;


        //Dependency injection of the MenuController to make it easier to reference it from each menu
        public virtual void InitState(SubCanvasTraining _trainingController)
        {
            this._trainingController = _trainingController;
        }


        //Jump back to the menu before it when we press a back button or escape key
        //You have to manually hook up each back-button to this method
        public void JumpBack()
        {
            _trainingController.JumpBack();
        }
    }
}
