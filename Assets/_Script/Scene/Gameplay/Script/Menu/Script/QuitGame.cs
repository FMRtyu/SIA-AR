using SIAairportSecurity.Training;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIAairportSecurity.Training
{
    public class QuitGame : _MenuState
    {
        public override void InitState(GameCanvasController menuController)
        {
            base.InitState(menuController);

            state = MenuState.Quit;
        }

        public void QuitApp()
        {
            _menuCanvasController.FadeOutQuit();
        }
    }
}
