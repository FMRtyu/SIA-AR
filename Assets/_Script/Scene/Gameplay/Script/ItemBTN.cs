using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SIAairportSecurity.Training
{
    public class ItemBTN : MonoBehaviour
    {

        private int _itemIndex;
        [SerializeField] private Image _imageChild;
        [SerializeField] private TMP_Text _itemName;
        protected Selection _selection;

        private void Start()
        {
            init();
        }

        public void InitSelection(Selection _selection, int _itemIndex)
        {
            this._selection = _selection;

            this._itemIndex = _itemIndex;
        }

        public void init()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                SetObject();
            });
        }

        private void SetObject()
        {
            _selection.SetObject(_itemIndex, GetComponent<Button>());
        }

        public void SetIcon(Sprite sprite)
        {
            _imageChild.sprite = sprite;
        }

        public void PlayButtonSound()
        {
            _selection.PlayButtonSound();
        }

        public void SetItemName(string itemName)
        {
            _itemName.text = itemName;
        }
    }
}
