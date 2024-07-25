using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static SIAairportSecurity.Training.GameCanvasController;

namespace SIAairportSecurity.Training
{
    public class RaycastSpawnObject : MonoBehaviour
    {
        private GamePlayController _gamePlayController;
        private ARRaycastManager raycastManager;
        // Start is called before the first frame update
        void Start()
        {
            //getting needed value
            init();
        }

        // Update is called once per frame
        void Update()
        {
            //spawning object using raycast
            if (raycastManager == null)
            {
                Raycast();
            }
        }

        private void init()
        {
            _gamePlayController = GetComponent<GamePlayController>();
            raycastManager = FindObjectOfType<ARRaycastManager>();
        }

        private void Raycast()
        {
            if (_gamePlayController.GetCurrentScreen() != MenuState.Selection)
            {
                // Check if the screen is touched
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    // Check if the touch phase is the beginning of a touch
                    if (touch.phase == TouchPhase.Began)
                    {
                        // Perform AR raycast from the touch position
                        List<ARRaycastHit> hits = new List<ARRaycastHit>();
                        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                        {
                            _gamePlayController.SpawnObject(hits[0]);
                        }
                        else
                        {
                            Debug.Log("Raycast did not hit a plane.");
                        }
                    }
                }
            }
        }
    }
}
