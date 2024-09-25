using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIAairportSecurity.FileInsert
{
    public class InitalMenu : _MenuStateInsertEdit
    {
        //Specific for this state
        public override void InitState(InsertEditCanvasController menuController)
        {
            base.InitState(menuController);

            state = InsertEditCanvasController.MenuState.Inital;
        }

        public void JumpToSelection()
        {
            _menuController.SetActiveState(InsertEditCanvasController.MenuState.Selection);
        }

        public void JumpToEdit()
        {
            _menuController.SetActiveState(InsertEditCanvasController.MenuState.Edit);
        }
    }
}
