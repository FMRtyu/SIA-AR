using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIAairportSecurity.FileInsert
{
    public class _MenuStateInsertEdit : MonoBehaviour
    {
        //Which state is this?
        public InsertEditCanvasController.MenuState state { get; protected set; }


        protected InsertEditCanvasController _menuController;


        //Dependency injection of the MenuController to make it easier to reference it from each menu
        public virtual void InitState(InsertEditCanvasController menuController)
        {
            this._menuController = menuController;
        }


        //Jump back to the menu before it when we press a back button or escape key
        //You have to manually hook up each back-button to this method
        public void JumpBack()
        {
            _menuController.JumpBack();
        }

        public void PlayButtonSound()
        {
            _menuController.PlayButtonSound();
        }
    }
}
