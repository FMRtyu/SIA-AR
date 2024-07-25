using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SIAairportSecurity.MainMenu
{
    public class SplashController : MonoBehaviour
    {
        public void StartTrainingScene(int sceneIndex)
        {
            if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings) 
            {
                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                Debug.LogError("Scene Out Of Index");
            }
        }
    }
}
