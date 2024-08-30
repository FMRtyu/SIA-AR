using SIAairportSecurity.Training;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SIAairportSecurity.FileInsert
{
    public class InsertEditController : MonoBehaviour
    {
        [Header("Controllers")]
        [SerializeField] private InsertEditCanvasController _canvasController;
        [SerializeField] private FilePicker _filePickerController;
        [SerializeField] private ItemDatabase _itemDatabase;

        [Header("SFX")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _tapAudio;
        [SerializeField] private AudioClip _SpawnAudio;

        private void Start()
        {
            init();
        }

        private void Update()
        {
            
        }

        private void init()
        {
            _canvasController.SetController(this);
            _filePickerController.SetController(this);
        }
        #region GetData

        public Dictionary<int, (Sprite, string, bool, bool)> GetSelectionData()
        {
            Dictionary<int, (Sprite, string, bool, bool)> temp = new Dictionary<int, (Sprite, string, bool, bool)>();


            for (int i = 0; i < _itemDatabase.items.Length; i++)
            {
                temp.Add(_itemDatabase.items[i].itemID, (_itemDatabase.items[i].itemSprite, _itemDatabase.items[i].itemName, CheckIfAvaible(i), _itemDatabase.items[i].isObjectSmall));
            }

            return temp;
        }

        private bool CheckIfAvaible(int itemIndex)
        {
            if (_itemDatabase.items[itemIndex].itemPrefabs != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region sound SFX

        //play button SFX
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
        public void PlayPlaceSound()
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }

            // Set the clip and play it
            if (_audioSource.clip != _SpawnAudio)
            {
                _audioSource.clip = _SpawnAudio;
            }
            _audioSource.Play();
        }

        #endregion

        public void BackToTraining()
        {
            SceneManager.LoadScene(1);
        }

        public void EditMenu()
        {
            _canvasController.SetActiveState(InsertEditCanvasController.MenuState.Edit);
        }
    }
}
