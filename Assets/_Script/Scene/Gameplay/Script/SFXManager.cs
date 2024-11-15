using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIAairportSecurity.Training
{
    public class SFXManager : MonoBehaviour
    {
        private GameCanvasController _canvasController;
        [SerializeField] private AudioSource _audioSource;
        // Start is called before the first frame update
        void Start()
        {
            _canvasController = FindAnyObjectByType<GameCanvasController>();
            _canvasController.onStateChange += OnMenuStateChange;
            _audioSource.Play();
        }

        private void OnDestroy()
        {
            _canvasController.onStateChange -= OnMenuStateChange;
        }

        private void OnMenuStateChange(MenuState newState)
        {
            if (newState != MenuState.Training)
            {
                _audioSource.Stop();
            }else
            {
                _audioSource.Play();
            }
        }
    }
}
