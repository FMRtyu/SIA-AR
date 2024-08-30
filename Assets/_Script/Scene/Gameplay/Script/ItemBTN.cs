using SIAairportSecurity.FileInsert;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SIAairportSecurity.Training
{
    public class ItemBTN : MonoBehaviour
    {

        private int _itemIndex = -1;
        private string _itemPath;
        [SerializeField] private Image _imageChild;
        [SerializeField] private TMP_Text _itemName;
        protected Selection _selection;
        protected SelectionMenu _selectionMenu;

        private void Start()
        {
            init();
        }

        public void InitSelection(SelectionMenu _selectionMenu, int _itemIndex)
        {
            this._selectionMenu = _selectionMenu;

            this._itemIndex = _itemIndex;
        }

        public void InitSelection(Selection _selectionMenu, string _itemPath)
        {
            this._selection = _selectionMenu;

            this._itemPath = _itemPath;
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
            if (_selection != null)
            {
                if (_itemIndex > -1)
                {
                    _selection.SetObject(_itemIndex, GetComponent<Button>());
                }else if(_itemPath != null)
                {
                    _selection.SetObject(_itemPath, GetComponent<Button>());
                }
            }
            else if (_selectionMenu != null)
            {
                _selectionMenu.SetObject(_itemIndex, GetComponent<Button>());
            }
        }

        public void SetIcon(Sprite sprite)
        {
            _imageChild.sprite = sprite;
        }

        public void PlayButtonSound()
        {

            if (_selection != null)
            {
                _selection.PlayButtonSound();
            }
            else if (_selectionMenu != null)
            {
                _selectionMenu.PlayButtonSound();
            }
        }

        public void SetItemName(string itemName)
        {
            _itemName.text = itemName;
        }
    }
}
