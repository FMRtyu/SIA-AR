using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MeasurementPlacement : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI m_Object; 

    [SerializeField]
    public GameObject midTextObject;

    string measurement = "";

    private void Start()
    {
        midTextObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        m_Object.text = measurement;
    }

    public void changeMeasurement(string mesurement)
    {
        this.measurement = mesurement;
    }
}
