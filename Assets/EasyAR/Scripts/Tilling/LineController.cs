using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Line controller for set position
/// </summary>
public class LineController : MonoBehaviour
{

    private LineRenderer lineRenderer;
    private Transform[] points;

    // Start is called before the first frame update
    void Awake() 
    {
        lineRenderer = GetComponent<LineRenderer>();      
    }

    public void SetupLine(Transform[] points)
    {        
        lineRenderer.positionCount = points.Length;
        this.points = points;
    }

    void Update()
    {
        if (lineRenderer.positionCount>0) { 
            for (int i = 0; i < points.Length; i++)
                {
                     lineRenderer.SetPosition(i, points[i].position);
            }
        }
    }

}
