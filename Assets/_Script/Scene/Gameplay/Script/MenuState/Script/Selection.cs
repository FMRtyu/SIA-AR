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
        [SerializeField] private SpriteDatabase _spriteDB;

        private RectTransform _selectItemBTNTransform;
        private Vector3 _initialScaleSelectItemBTN;

        private List<GameObject> _itemBTNList = new List<GameObject>();
        private Button _selectedItem;
        protected Dictionary<int, (Sprite, string, bool)> _itemDatabase;
        //Specific for this state
        public override void InitState(GameCanvasController menuController)
        {
            base.InitState(menuController);

            state = MenuState.Selection;
        }


        private void OnEnable()
        {
            ShowAllItem();
        }

        private void Start()
        {
            init();
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

                (Sprite itemIcon, string itemName, bool isObjectAvaible) = item.Value;

                tempItemBTN.SetIcon(itemIcon);
                tempItemBTN.SetItemName(itemName);

                //check if 3D object available
                if (!isObjectAvaible)
                {
                    tempItemBTN.GetComponent<Image>().sprite = _spriteDB.SquareButton.activatedSprite;

                    Color color = tempItemBTN.GetComponent<Image>().color;
                    Color color2 = tempItemBTN.GetComponentInChildren<Image>().color;

                    color.a = 0.25f;
                    color2.a = 0.25f;

                    tempItemBTN.GetComponent<Image>().color = color;
                    temp.transform.Find("IconBTN").GetComponent<Image>().color = color2;
                    Destroy(tempItemBTN.GetComponent<Button>());
                }

                _itemBTNList.Add(temp);
            }
            _menuCanvasController.GetGamePlayController().SetButtonSFX();
        }

        private void init()
        {
            _selectItemBTNTransform = _selectItemBTN.GetComponent<RectTransform>();
            _initialScaleSelectItemBTN = _selectItemBTNTransform.localScale;

            _selectItemBTNTransform.localScale = Vector3.zero;
            ShowAllItem();
        }

        public void SetObject(int itemIndex , Button _selectedItemBTN)
        {
            _menuCanvasController.SetObject(itemIndex);

            if (_selectItemBTNTransform.localScale == Vector3.zero)
            {
                LeanTween.scale(_selectItemBTNTransform, _initialScaleSelectItemBTN, 0.5f).setEase(LeanTweenType.easeOutBack).setOnComplete(() =>
                {
                    _selectItemBTN.interactable = true;
                });
            }

            if (_selectedItem == null)
            {
                _selectedItemBTN.interactable = false;

                _selectedItem = _selectedItemBTN;
            }
            else
            {
                _selectedItem.interactable = true;
                _selectedItem.GetComponent<ItemBTN>().ResetNameColor();
                _selectedItemBTN.interactable = false;

                _selectedItem = _selectedItemBTN;
            }
        }
        #endregion
        public void JumpToTraining()
        {
            _selectItemBTN.interactable = false;

            _selectedItem.GetComponent<ItemBTN>().ResetNameColor();
            _selectedItem.interactable = true;

            _selectItemBTNTransform.localScale = Vector3.zero;

            _menuCanvasController.JumpToTraining();
        }

        public void BackToTraining()
        {
            _menuCanvasController.BackToTraining();
        }
    }
}
