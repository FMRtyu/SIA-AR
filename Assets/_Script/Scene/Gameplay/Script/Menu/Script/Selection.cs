using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace SIAairportSecurity.Training
{
    public class Selection : _MenuState
    {
        [Header("MenuList")]
        [SerializeField] private GameObject _itemBTNPrefabs;
        [SerializeField] private RectTransform _itemBTNParent;
        [SerializeField] private Button _selectItemBTN;

        private List<GameObject> _itemBTNList = new List<GameObject>();
        private Button _previousSelectedItem;

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

            LoadAllAssetBundle();
        }

        /// <summary>
        /// Get all data from persistance data path
        /// </summary>
        /// <returns></returns>
        private void LoadAllAssetBundle()
        {
            string directoryPath = Application.persistentDataPath;

            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath); // Get all files in the directory

                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file); // Extract the file name

                    GameObject temp = Instantiate(_itemBTNPrefabs, _itemBTNParent);

                    ItemBTN tempItemBTN = temp.GetComponent<ItemBTN>();

                    tempItemBTN.InitSelection(this, file);

                    StartCoroutine(SetButtonIcon(Path.Combine(Application.persistentDataPath, fileName), tempItemBTN));
                    tempItemBTN.SetItemName(fileName);

                    _itemBTNList.Add(temp);
                }
            }
            else
            {
                Debug.LogWarning($"Directory does not exist: {directoryPath}");
            }
        }

        private IEnumerator SetButtonIcon(string path, ItemBTN tempItemBTN)
        {
            AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(path);
            yield return bundleRequest;

            AssetBundle bundle = bundleRequest.assetBundle;

            // List all assets in the Asset Bundle
            string[] assetNames = bundle.GetAllAssetNames();
            if (assetNames.Length == 0)
            {
                Debug.LogError("No assets found in AssetBundle.");
                yield break;
            }
            foreach (string assetName in assetNames)
            {
                if (assetName.EndsWith(".png") || assetName.EndsWith(".jpg")) // Example criteria for sprites
                {
                    AssetBundleRequest spriteRequest = bundle.LoadAssetAsync<Sprite>(assetName);
                    yield return spriteRequest;

                    Sprite sprite = spriteRequest.asset as Sprite;
                    if (sprite != null)
                    {

                        tempItemBTN.SetIcon(sprite);
                        // Optionally, unload the Asset Bundle if no longer needed
                        bundle.Unload(false);
                        yield break;
                    }
                    else
                    {
                        Debug.LogWarning("Failed to load sprite from AssetBundle: " + assetName);
                    }
                }
            }
        }

        public void SetObject(int itemIndex , Button _selectedItemBTN)
        {
            _menuCanvasController.SetObject(itemIndex);
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

        public void SetObject(string itemPath, Button _selectedItemBTN)
        {
            _menuCanvasController.SetObject(itemPath);
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
        #endregion
        public void JumpToTraining()
        {
            _selectItemBTN.interactable = false;

            _previousSelectedItem.interactable = true;

            _menuCanvasController.JumpToTraining();
        }

        public void JumpToEditScene()
        {
            _menuCanvasController.FadeOutChangeScene();
        }

        public void PlayButtonSound()
        {
            _menuCanvasController.PlayButtonSound();
        }
    }
}
