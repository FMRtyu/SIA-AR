using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Line handler
/// </summary>
public class LineHandler : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Transform[] points;

    // Start is called before the first frame update
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;

    }

    public void AddLine(Vector3 previousPose, Vector3 newPose)
    {
        lineRenderer.SetPosition(0, previousPose);
        lineRenderer.SetPosition(1, newPose);
    }
}
