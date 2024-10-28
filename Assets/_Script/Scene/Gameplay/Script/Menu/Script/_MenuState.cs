using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIAairportSecurity.Training
{
    public class _MenuState : MonoBehaviour
    {
        //Which state is this?
        public MenuState state { get; protected set; }


        protected GameCanvasController _menuCanvasController;


        //Dependency injection of the MenuController to make it easier to reference it from each menu
        public virtual void InitState(GameCanvasController menuController)
        {
            this._menuCanvasController = menuController;
        }


        //Jump back to the menu before it when we press a back button or escape key
        //You have to manually hook up each back-button to this method
        public void JumpBack()
        {
            _menuCanvasController.JumpBack();
        }
    }
}
