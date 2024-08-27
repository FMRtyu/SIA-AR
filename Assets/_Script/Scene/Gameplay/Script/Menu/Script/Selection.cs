using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SIAairportSecurity.Training
{
    public class Selection : _MenuState
    {
        [Header("MenuList")]
        [SerializeField] private GameObject _itemBTNPrefabs;
        [SerializeField] private RectTransform _itemBTNParent;
        [SerializeField] private Button _selectItemBTN;

        private List<GameObject> _itemBTNList = new List<GameObject>();
        private Button _selectedItem;

        [SerializeField] private Sprite _tempSprite;

        protected Dictionary<int, (Sprite, string, bool, bool)> _itemDatabase;
        //Specific for this state
        public override void InitState(GameCanvasController menuController)
        {
            base.InitState(menuController);

            state = GameCanvasController.MenuState.Selection;
        }


        private void OnEnable()
        {
            ShowAllItem();
        }

        #region Show and set item

        /// <summary>
        /// instantate all items from scriptable data into a button
        /// </summary>
        private void ShowAllItem()
        {
            if (_itemBTNList.Count > 0)
            {
                return;
            }

            _itemDatabase = _menuCanvasController.GetListData();

            foreach (var item in _itemDatabase)
            {
                GameObject temp = Instantiate(_itemBTNPrefabs, _itemBTNParent);

                ItemBTN tempItemBTN = temp.GetComponent<ItemBTN>();

                tempItemBTN.InitSelection(this, item.Key);

                (Sprite itemIcon, string itemName, bool isObjectAvaible, bool isObjectSmall) = item.Value;

                tempItemBTN.SetIcon(itemIcon);
                tempItemBTN.SetItemName(itemName);

                //check if 3D object available
                if (!isObjectAvaible)
                {
                    tempItemBTN.GetComponent<Image>().sprite = _tempSprite;

                    Color color = tempItemBTN.GetComponent<Image>().color;
                    Color color2 = tempItemBTN.GetComponentInChildren<Image>().color;

                    color.a = 0.25f;
                    color2.a = 0.25f;

                    tempItemBTN.GetComponent<Image>().color = color;
                    temp.transform.Find("IconBTN").GetComponent<Image>().color = color2;
                    Destroy(tempItemBTN.GetComponent<Button>());
                }

                //resize the icon
                if (isObjectSmall)
                {
                    RectTransform iconIMG = temp.transform.Find("IconBTN").GetComponent<RectTransform>();

                    Debug.Log(iconIMG.name);

                    iconIMG.offsetMin = new Vector2(100f, iconIMG.offsetMin.y);  // Left offset
                    iconIMG.offsetMax = new Vector2(-100, iconIMG.offsetMax.y); // Right offset
                }

                _itemBTNList.Add(temp);
            }
        }

        public void SetObject(int itemIndex , Button _selectedItemBTN)
        {
            _menuCanvasController.SetObject(itemIndex);
            _selectItemBTN.interactable = true;

            if (_selectedItem == null)
            {
                _selectedItemBTN.interactable = false;

                _selectedItem = _selectedItemBTN;
            }
            else
            {
                _selectedItem.interactable = true;
                _selectedItemBTN.interactable = false;

                _selectedItem = _selectedItemBTN;
            }
        }
        #endregion
        public void JumpToTraining()
        {
            _selectItemBTN.interactable = false;

            _selectedItem.interactable = false;

            _menuCanvasController.JumpToTraining();
        }

        public void PlayButtonSound()
        {
            _menuCanvasController.PlayButtonSound();
        }
    }
}
