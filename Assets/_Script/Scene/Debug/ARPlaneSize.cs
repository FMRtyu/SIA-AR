using SIAairportSecurity.Training;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPlaneSize : MonoBehaviour
{
    public ARPlane plane;
    private RaycastSpawnObject raycast;

    public TextMeshProUGUI infoText;

    // Start is called before the first frame update
    void Start()
    {
        raycast = FindObjectOfType<RaycastSpawnObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main != null)
        {
            Vector3 planeCenter = plane.transform.position;
            transform.position = new Vector3(planeCenter.x, planeCenter.y + 0.3f, planeCenter.z);

            // Make this object look at the camera
            transform.LookAt(Camera.main.transform);

            // Optional: flip to face forward (not away from the camera)
            transform.Rotate(0, 180f, 0);

            float width = plane.size.x;
            float height = plane.size.y;
            float area = width * height;

            infoText.text = $"X: {width:F2}m\nY: {height:F2}m\nArea: {area:F2}m²";

            if (planeCenter.x >= raycast.requiredSize.x && planeCenter.y >= raycast.requiredSize.y)
            {
                infoText.color = Color.green;
            }
        }
    }
}
