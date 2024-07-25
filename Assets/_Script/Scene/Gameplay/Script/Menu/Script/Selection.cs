using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIAairportSecurity.Training
{
    public class Selection : _MenuState
    {
        //Specific for this state
        public override void InitState(GameCanvasController menuController)
        {
            base.InitState(menuController);

            state = GameCanvasController.MenuState.Selection;
        }



        public void JumpToTraining()
        {
            _menuCanvasController.SetActiveState(GameCanvasController.MenuState.Training);
        }
    }
}
