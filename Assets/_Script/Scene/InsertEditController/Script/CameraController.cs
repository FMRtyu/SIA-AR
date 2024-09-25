using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIAairportSecurity.FileInsert
{
    public class CameraController : MonoBehaviour
    {
        public float rotationSpeed = 0.2f;
        public float zoomSpeed = 0.5f;
        public float minZoom = 5f;
        public float maxZoom = 20f;

        private Vector2 lastTouchPosition;

        void Update()
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 delta = touch.deltaPosition;
                    float rotationX = delta.y * rotationSpeed;
                    float rotationY = -delta.x * rotationSpeed;

                    // Rotate the camera around the object
                    transform.RotateAround(Vector3.zero, Vector3.up, rotationY);
                    transform.RotateAround(Vector3.zero, transform.right, rotationX);
                }

                lastTouchPosition = touch.position;
            }
            else if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Calculate the distance between the two touches in the previous frame and in the current frame
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between the two touches in each frame
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                // Zoom based on the change in distance between the two touches
                Camera.main.fieldOfView += deltaMagnitudeDiff * zoomSpeed;
                Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, minZoom, maxZoom);
            }
        }
    }
}
