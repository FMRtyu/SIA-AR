using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SIAairportSecurity.Training
{
    public class SFXManager : MonoBehaviour
    {
        private GameCanvasController _canvasController;
        private Button _soundButton;
        private Image _soundButtonImage;
        private Image _soundIconImage;
        private Color _soundInitialIconColor;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private SpriteDatabase _spriteDB;

        bool BombSound = true;
        // Start is called before the first frame update
        void Start()
        {
            _canvasController = FindAnyObjectByType<GameCanvasController>();

            _soundButton = GameObject.Find("Training").GetComponent<Training>().soundButtonToggler;
            _soundIconImage = _soundButton.transform.Find("IconIMG").GetComponent<Image>();
            _soundButtonImage = _soundButton.GetComponent<Image>();
            _soundInitialIconColor = _soundIconImage.color;

            _soundButton.gameObject.SetActive(true);

            _soundButton.onClick.AddListener(OnSoundButtonClicked);

            _canvasController.onStateChange += OnMenuStateChange;
            _audioSource.Play();
        }

        private void OnDestroy()
        {
            _soundButtonImage.sprite = _spriteDB.RoundButton.activatedSprite;
            _soundIconImage.sprite = _spriteDB.Sound.activatedSprite;
            _soundButtonImage.GetComponentInChildren<TMP_Text>().text = "Sound\nOn";
            _soundButton.onClick.RemoveListener(OnSoundButtonClicked);
            _soundButton.gameObject.SetActive(false);

            _canvasController.onStateChange -= OnMenuStateChange;
        }

        private void OnMenuStateChange(MenuState newState)
        {
            if (newState != MenuState.Training)
            {
                
                _audioSource.Stop();
            }else
            {
                if (BombSound)
                {
                    _audioSource.Play();
                }
                else
                {
                    _audioSource.Stop();
                }
            }
        }

        private void OnSoundButtonClicked()
        {
            TMP_Text tempTXT = _soundButtonImage.GetComponentInChildren<TMP_Text>();
            if (_audioSource.isPlaying)
            {
                _soundButtonImage.sprite = _spriteDB.RoundButton.defaultSprite;
                _soundIconImage.sprite = _spriteDB.Sound.defaultSprite;
                _soundIconImage.color = Color.white;
                tempTXT.text = "Sound\nOff";
                tempTXT.color = Color.white;
                _audioSource.Stop();
                BombSound = false;
            }
            else
            {
                _soundButtonImage.sprite = _spriteDB.RoundButton.activatedSprite;
                _soundIconImage.sprite = _spriteDB.Sound.activatedSprite;
                _soundIconImage.color = _soundInitialIconColor;
                tempTXT.text = "Sound\nOn";
                tempTXT.color = Color.black;
                _audioSource.Play();
                BombSound = true;
            }
        }
    }
}
