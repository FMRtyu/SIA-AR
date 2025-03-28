using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SIAairportSecurity.MainMenu
{
    public class SplashController : MonoBehaviour
    {
        [Header("SFX")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _tapAudio;
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

        

        public void PlayButtonSound()
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }

            // Set the clip and play it
            if (_audioSource.clip != _tapAudio)
            {
                _audioSource.clip = _tapAudio;
            }
            _audioSource.Play();
        }
    }
}
