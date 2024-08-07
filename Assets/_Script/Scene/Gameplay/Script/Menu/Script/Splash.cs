using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIAairportSecurity.Training
{
    public class Splash : _MenuState
    {
        [SerializeField] private float SplashTimer = 3f;

        public override void InitState(GameCanvasController menuController)
        {
            base.InitState(menuController);

            state = GameCanvasController.MenuState.Splash;
        }
        private void OnEnable()
        {
            init();
        }

        private void init()
        {

            StartCoroutine(CallWithDelay());
        }
        IEnumerator CallWithDelay()
        {
            yield return new WaitForSeconds(0.5f);
            _menuCanvasController.FadeIn();
            yield return new WaitForSeconds(SplashTimer);
            _menuCanvasController.FadeOut();
            yield return new WaitForSeconds(1.5f);
            _menuCanvasController.FadeIn();
            _menuCanvasController.SetActiveState(GameCanvasController.MenuState.Selection);
        }
    }
}
