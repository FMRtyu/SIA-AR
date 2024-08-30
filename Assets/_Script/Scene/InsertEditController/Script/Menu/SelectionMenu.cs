using SIAairportSecurity.Training;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SIAairportSecurity.FileInsert
{
    public class SelectionMenu : _MenuStateInsertEdit
    {
        [Header("Item Property")]
        [SerializeField] private GameObject _itemBTNPrefabs;
        [SerializeField] private Transform _itemBTNParent;
        [SerializeField] private Sprite _tempSprite;
        [SerializeField] private Button _selectItemBTN;
        private Button _previousSelectedItem;

        //Load Local Model
        public string localPath = "/MyApp/AssetBundles";
        public string bundleName = "mybundle";
        public string assetName = "MyModelPrefab";

        private List<GameObject> _itemBTNList = new List<GameObject>();
        protected Dictionary<int, (Sprite, string, bool, bool)> _itemDatabase;
        //Specific for this state
        public override void InitState(InsertEditCanvasController menuController)
        {
            base.InitState(menuController);

            state = InsertEditCanvasController.MenuState.Selection;
        }

        public void JumpToEdit()
        {
            _menuController.SetActiveState(InsertEditCanvasController.MenuState.Edit);
        }

        private void OnEnable()
        {
            if (_menuController != null)
            {
                ShowAllItem();
            }
        }
        #region ShowItemList

        private void ShowAllItem()
        {
            if (_itemBTNList.Count > 0)
            {
                return;
            }

            _itemDatabase = _menuController.GetListData();

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

        public void SetObject(int itemIndex, Button _selectedItemBTN)
        {
            _menuController.SetObject(itemIndex);
            _selectItemBTN.interactable = true;

            if (_previousSelectedItem == null)
            {
                _selectedItemBTN.interactable = false;

                _previousSelectedItem = _selectedItemBTN;
            }
            else
            {
                _previousSelectedItem.interactable = true;
                _selectedItemBTN.interactable = false;

                _previousSelectedItem = _selectedItemBTN;
            }
        }

        private IEnumerator LoadBundle()
        {
            string localFilePath = System.IO.Path.Combine(Application.persistentDataPath, localPath, bundleName + ".bundle");

            if (System.IO.File.Exists(localFilePath))
            {
                AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(localFilePath);
                yield return bundleRequest;

                AssetBundle bundle = bundleRequest.assetBundle;
                if (bundle != null)
                {
                    AssetBundleRequest assetRequest = bundle.LoadAssetAsync<GameObject>(assetName);
                    yield return assetRequest;

                    GameObject prefab = assetRequest.asset as GameObject;
                    if (prefab != null)
                    {
                        Instantiate(prefab);
                    }
                    else
                    {
                        Debug.LogError("Failed to load asset from Asset Bundle.");
                    }

                    bundle.Unload(false);
                }
                else
                {
                    Debug.LogError("Failed to load Asset Bundle from local storage.");
                }
            }
            else
            {
                Debug.LogError("Asset Bundle not found at: " + localFilePath);
            }
        }
        #endregion


    }
}
