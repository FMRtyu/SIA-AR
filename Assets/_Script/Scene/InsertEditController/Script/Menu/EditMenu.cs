using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIAairportSecurity.FileInsert
{
    public class EditMenu : _MenuStateInsertEdit
    {
        //Specific for this state
        public override void InitState(InsertEditCanvasController menuController)
        {
            base.InitState(menuController);

            state = InsertEditCanvasController.MenuState.Edit;
        }

        public void JumpToInital()
        {
            _menuController.SetActiveState(InsertEditCanvasController.MenuState.Inital);
        }
    }
}
