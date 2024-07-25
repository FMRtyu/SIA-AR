using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotate the ditance showing cards
/// </summary>
public class CardRotation : MonoBehaviour
{
    /// <summary>
    /// Access the AR camera
    /// </summary>
    private GameObject arCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        arCamera = GameObject.FindWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(arCamera.transform);
    }
}
