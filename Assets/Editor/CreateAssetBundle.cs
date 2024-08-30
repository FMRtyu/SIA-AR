using UnityEditor;
using UnityEngine;

public class CreateAssetBundle
{
    [MenuItem("Assets/Create Assets Bundle")]
    private static void BuildAllAssetBundle()
    {
        string bundleName = "bundletest";
        string assetBundleDirectoryPath = Application.dataPath + "/../Assetbundle";

        // Create the AssetBundle directory if it does not exist
        if (!System.IO.Directory.Exists(assetBundleDirectoryPath))
        {
            System.IO.Directory.CreateDirectory(assetBundleDirectoryPath);
        }

        // Get all asset paths for the specified Asset Bundle
        string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
        if (assetPaths.Length >= 0)
        {
            try
            {
                // Build the Asset Bundle
                BuildPipeline.BuildAssetBundles(assetBundleDirectoryPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
                Debug.Log("Asset Bundle built successfully.");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error building Asset Bundle: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("No assets found for the specified Asset Bundle: " + bundleName);
        }
    }
}
