using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : MonoBehaviour
{
    [SerializeField]
    private Pose placementPose;
    private PlacementMGR placementMGR;

    private void Start()
    {
        placementMGR = GameObject.FindWithTag("PlacementMGR").GetComponent<PlacementMGR>();
        placementPose = placementMGR.placementPose;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Indicator")
        {
            other.transform.position = gameObject.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Indicator")
        {
            other.transform.position = placementPose.position;
        }
    }
}
