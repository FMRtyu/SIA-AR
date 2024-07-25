using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHandler : MonoBehaviour
{
    public GameObject green;
    public GameObject red;
    public GameObject occulusionOut;
    public GameObject occulusionDoor;
    
    // Start is called before the first frame update
    void Start()
    {
        green.SetActive(false);
        red.SetActive(true);
        occulusionDoor.SetActive(true);
        occulusionOut.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.name == "AR Camera")
        {
            occulusionDoor.SetActive(true);
            occulusionOut.SetActive(true);
            green.SetActive(true);
            red.SetActive(true);
        }
        
    }

    void OnTriggerExit (Collider other)
    {
        if (other.name == "AR Camera")
        {
            Debug.Log(other.name);
            green.SetActive(false);
            red.SetActive(true);
            occulusionDoor.SetActive(false);
        }
            
    }
}
