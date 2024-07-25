using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabMaterialHandler : MonoBehaviour
{
    /// <summary>
    /// Spwaning Object Materias
    /// </summary>
    public Material[] ObjectMaterials;
    public static Material[] SpawningObjectMaterials;


    public void SetMaterials()
    {
        SpawningObjectMaterials = ObjectMaterials;
    }


}
