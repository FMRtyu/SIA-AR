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
        private Color _itemInitNameColor;
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

            _itemInitNameColor = _itemName.color;
        }

        private void SetObject()
        {
            _selection.SetObject(_itemIndex, GetComponent<Button>());
            _itemName.color = Color.black;
        }

        public void SetIcon(Sprite sprite)
        {
            _imageChild.sprite = sprite;
        }

        public void SetItemName(string itemName)
        {
            _itemName.text = itemName;
        }

        public void ResetNameColor()
        {
            _itemName.color = _itemInitNameColor;
        }
    }
}
