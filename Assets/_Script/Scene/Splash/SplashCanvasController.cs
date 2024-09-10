using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SIAairportSecurity.MainMenu
{
    public class SplashCanvasController : MonoBehaviour
    {
        [Header("Splash Scene Controller")]
        public SplashController splashController;

        [Header("Fade Options")]
        public CanvasGroup fadeImgObject;
        public float fadeTime;

        private void Start()
        {

            FadeOut();
        }

        public void ChangeToTraining()
        {
            fadeImgObject.interactable = false;
            fadeImgObject.blocksRaycasts = false;
            LeanTween.alphaCanvas(fadeImgObject, to:1, fadeTime).setOnComplete(() =>
            {
                splashController.StartTrainingScene(1);
            });
        }

        #region Fade Fuctions
        private void FadeOut()
        {
            LeanTween.alphaCanvas(fadeImgObject, to: 0, fadeTime).setOnComplete(() =>
            {
                fadeImgObject.interactable = false;
                fadeImgObject.blocksRaycasts = false;
            });
        }
        #endregion
    }
}
