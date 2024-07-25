using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// To detect Double tap
/// </summary>
public class DoubleTap : MonoBehaviour
{
    /// <summary>
    /// Keep the tapCount
    /// </summary>
    int tapCount;

    /// <summary>
    /// To check waiting time for second tap has been started or not
    /// </summary>
    bool waitingTimeStarted = false;

    /// <summary>
    /// Spent time after got first tap
    /// </summary>
    float time;

    /// <summary>
    /// Waiting time limit for second tap
    /// </summary>
    private float waitingTime;


    void Start()
    {
        waitingTime = 0.5f;
        waitingTimeStarted = false;
        time = 0;
        tapCount = 0;
    }

    void Update()
    {
        // Handle screen touches.
        if (Input.touchCount == 1)
        {
            if (!waitingTimeStarted)
            {
                time = 0;
                tapCount = 0;
            }
            waitingTimeStarted = true;
            Touch touch = Input.GetTouch(0);
            if (tapCount < 1)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    tapCount++;
                }
                else if (touch.phase == TouchPhase.Moved && time > 0.2f)
                {
                    tapCount = 0;
                    time = 0;
                }
            }
            else
            {
                if (touch.phase == TouchPhase.Began)
                {
                    tapCount++;
                }
                else if (touch.phase == TouchPhase.Moved && time > 0.2f)
                {
                    tapCount = 0;
                    time = 0;
                }

            }

            if (time <= waitingTime && tapCount > 1)
            {
                //Double Tap trigger in here
                if (InitialData._singleObjectPlacement)
                {
                    PlaceOnPlane.resetToInitialScale();
                }
                else
                {
                    MultipleObjectPlacement.resetToInitialScale();
                }
                tapCount = 0;
                time = 0;
            }
        }
        if (time > waitingTime)
        {
            time = 0;
            waitingTimeStarted = false;
            tapCount = 0;
        }
        else
        {
            if (waitingTimeStarted)
            {
                time += Time.deltaTime;
            }
        }
    }
}