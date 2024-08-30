using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using NativeFilePickerNamespace;

namespace SIAairportSecurity.FileInsert
{
    public class FilePicker : MonoBehaviour
    {
        [SerializeField] private Button pickFileButton;
        [SerializeField] private Transform parentObject; // The anchor GameObject in the scene
        private GameObject _spawnedObject;

        private string _fileModelPath;
        private string _fileModelName;

        protected InsertEditController _insertEditController;

        public void SetController(InsertEditController _insertEditController)
        {
            this._insertEditController = _insertEditController;
        }
        void Start()
        {
            pickFileButton.onClick.AddListener(OpenFilePicker);
        }

        void OpenFilePicker()
        {
            // Open the file picker
            NativeFilePicker.PickFile((path) =>
            {
                if (path == null)
                {
                    Debug.Log("Operation cancelled");
                }
                else
                {
                    _fileModelName = Path.GetFileName(path); // Get the file name
                    _fileModelPath = path; // Set the destination path
                    Debug.Log("Picked file: " + path);
                    LoadFile(path);
                }
            }, null); // No MIME type filter specified
        }

        void LoadFile(string path)
        {
            if (IsAssetBundle(path))
            {
                StartCoroutine(LoadAndInstantiateAssetBundle(path));
            }
            else
            {
                Debug.LogError("The selected file is not a valid Asset Bundle.");
            }
        }

        // Validate whether the selected file is an Asset Bundle
        private bool IsAssetBundle(string filePath)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
            if (bundle == null)
            {
                return false;
            }
            else
            {
                bundle.Unload(true);
                return true;
            }
        }

        private IEnumerator LoadAndInstantiateAssetBundle(string filePath)
        {
            // Load Asset Bundle from selected file
            AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(filePath);
            yield return bundleRequest;

            AssetBundle bundle = bundleRequest.assetBundle;
            if (bundle != null)
            {
                // Load the first asset from the bundle (adjust to load specific assets if needed)
                string assetName = bundle.GetAllAssetNames()[0];
                GameObject prefab = bundle.LoadAsset<GameObject>(assetName);

                if (prefab != null)
                {
                    // Instantiate the prefab as a child of the parentObject
                    GameObject instance = Instantiate(prefab, parentObject);
                    instance.transform.localPosition = Vector3.zero;
                    instance.transform.localRotation = Quaternion.identity;

                    if (_spawnedObject == null)
                    {
                        _spawnedObject = instance;
                    }
                    else
                    {
                        Destroy(_spawnedObject);
                        _spawnedObject = instance;
                    }
                    Debug.Log("Asset instantiated from Asset Bundle.");
                    //pickFileButton.gameObject.GetComponentInParent<GameObject>().SetActive(false);
                    _insertEditController.EditMenu();
                }
                else
                {
                    Debug.LogError("Failed to load asset from Asset Bundle.");
                }

                // Unload the Asset Bundle after use
                bundle.Unload(false);
            }
            else
            {
                Debug.LogError("Failed to load Asset Bundle from file.");
            }
        }

        public void SaveFile()
        {
            try
            {
                string destinationPath = Path.Combine(Application.persistentDataPath, _fileModelName);
                // Read the file from the selected path
                byte[] fileData = File.ReadAllBytes(_fileModelPath);

                // Write the file to the persistent data path
                File.WriteAllBytes(destinationPath, fileData);

                Debug.Log($"File copied to: {destinationPath}");
            }
            catch (IOException ex)
            {
                Debug.LogError($"File copy failed: {ex.Message}");
            }
        }
    }
}
