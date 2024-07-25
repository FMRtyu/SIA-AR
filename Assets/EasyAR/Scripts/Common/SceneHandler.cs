using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    private static ARSession aRSession;
    /// <summary>
    /// Method use for go to ar view scene 
    /// </summary>


    public static void GoToNextView()
    {
        LoaderUtility.Initialize();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }

    /// <summary>
    /// 
    /// </summary>
    public void BackFromCurrentScene()
    {
        if (Application.CanStreamedLevelBeLoaded("Menu"))
        {
            PlaceOnPlane.isObjectPlaced = false;
            MultipleObjectPlacement.isObjectPlaced = false;
            Destroy(PlaceOnPlane.spawnedObject);
            //PrefabMaterialHandler.SpawningObjectMaterials = null;
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
            LoaderUtility.Deinitialize();
            //StartCoroutine(LoadYourAsyncScene());
        }
    }

    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

    }



    /// <summary>
    /// Method use for go to ar measurement scene 
    /// </summary>
    public static void ARMeasurement()
    {
        LoaderUtility.Initialize();
        SceneManager.LoadScene("ARMeasurement", LoadSceneMode.Single);
    }

    /// <summary>
    /// Method use for go to ar measurement scene 
    /// </summary>
    public static void ARTile()
    {
        LoaderUtility.Initialize();
        SceneManager.LoadScene("ARTilling", LoadSceneMode.Single);
    }

    /// <summary>
    /// Method use for go to ar measurement scene 
    /// </summary>
    public static void ARMultipleObjects()
    {
        LoaderUtility.Initialize();
        SceneManager.LoadScene("ARMultipleObjects", LoadSceneMode.Single);
    }

    /// <summary>
    /// Method use for go to ar measurement scene 
    /// </summary>
    public static void ARConfigurator()
    {
        LoaderUtility.Initialize();
        SceneManager.LoadScene("ARConfigurator", LoadSceneMode.Single);
    }
}
