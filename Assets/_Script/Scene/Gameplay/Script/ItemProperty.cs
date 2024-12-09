using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIAairportSecurity.Training
{
    public class ItemProperty : MonoBehaviour
    {
        [Header("ItemProperty")]
        [SerializeField] private Transform _objectAnchor;
        private Vector3 _objectInitialPosition;

        private BoxCollider _objectCollider;
        private Rigidbody _objectRigidbody;

        private GamePlayController _gamePlayController;
        public void SetGameController(GamePlayController _gamePlayController)
        {
            this._gamePlayController = _gamePlayController;
        }

        private void Awake()
        {
            _objectInitialPosition = _objectAnchor.localPosition;
        }
        // Start is called before the first frame update
        void Start()
        {
            initItem();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void initItem()
        {
            _objectCollider = GetComponent<BoxCollider>();

            //Scale up animation
            Vector3 InitalScale = _objectAnchor.transform.localScale;
            _objectAnchor.transform.localScale = Vector3.zero;
            LeanTween.scale(_objectAnchor.gameObject, to: InitalScale, 1f).setEase(LeanTweenType.easeOutBack);
        }

        #region Initial Move and Rotate Object
        //switch to move object 1 finger
        public void SwitchToMove()
        {
            if (_objectCollider != null)
            {
                _objectCollider.enabled = true;
                _gamePlayController.RaiseManipulationObjectChangeEvent(ObjectManipulation.Move);

                //move down the object
                _objectAnchor.localPosition = _objectInitialPosition;
                _objectAnchor.GetComponent<BoxCollider>().enabled = false;

            }
        }
        //switch to rotate object 1 finger
        public void SwitchToRotate()
        {
            if (_objectCollider != null)
            {
                _objectCollider.enabled = false;
                _gamePlayController.RaiseManipulationObjectChangeEvent(ObjectManipulation.Rotate);

                //move up the object
                Vector3 newPos = new Vector3(_objectInitialPosition.x, _objectInitialPosition.y + 0.15f, _objectInitialPosition.z);
                _objectAnchor.localPosition = newPos;
                _objectAnchor.GetComponent<BoxCollider>().enabled = true;
            }
        }
        #endregion

        #region Move and Rotate Object

        public void SnapObjectXAxis()
        {
            Quaternion currentRotation = _objectAnchor.rotation;
            float newRotationX = currentRotation.x + 90;
            Debug.Log(newRotationX + " " + currentRotation);
            _objectAnchor.Rotate(new Vector3(newRotationX, _objectAnchor.rotation.y, _objectAnchor.rotation.z), Space.Self);
        }

        public void SnapObjectYAxis()
        {
            Quaternion currentRotation = _objectAnchor.rotation;
            float newRotationY = currentRotation.eulerAngles.y;
            Debug.Log(newRotationY + " " + currentRotation + " " + SnapValue(newRotationY, 90f));
            _objectAnchor.rotation = Quaternion.Euler(_objectAnchor.rotation.eulerAngles.x,
                SnapValue(newRotationY, 90f),
                _objectAnchor.rotation.eulerAngles.z
                );
        }

        float SnapValue(float value, float snapInterval)
        {
            float snappedValue = Mathf.Round(value / snapInterval) * snapInterval;
            if (snappedValue >= 360f)
            {
                return 0f;
            }
            if (snappedValue == 0f || snappedValue == 90f || snappedValue == 180f || snappedValue == 270f)
            {
                snappedValue += 90f;
            }
            return snappedValue;
        }

        #endregion

        public void ConfirmObjectPosition()
        {
            //move down the object
            _objectAnchor.localPosition = _objectInitialPosition;

            //delete box collider in rotate object
            _objectAnchor.GetComponent<BoxCollider>().enabled = true;
            _objectCollider.enabled = false;

            //add rigidbody
            _objectRigidbody = _objectAnchor.gameObject.AddComponent<Rigidbody>();
            _objectRigidbody.freezeRotation = true;

            //delete rigidbody after a second
            Invoke("DeleteRigidbody", 1.5f);
        }
        //delete rigidbody from spawned object after i second
        private void DeleteRigidbody()
        {
            if (_objectRigidbody != null)
            {
                Destroy(_objectRigidbody);
                _objectRigidbody = null;
                _objectCollider.enabled = true;
            }
        }

        //reset move rotate panel
        public void ResetMoveRotate()
        {
            _gamePlayController.RaiseManipulationObjectChangeEvent(ObjectManipulation.Move);
            _gamePlayController._surfaceManager.ShowDotsPlane(true);

            if (_objectRigidbody != null)
            {
                Destroy(_objectRigidbody);
                _objectCollider.enabled = true;
            }
            _objectAnchor.localPosition = _objectInitialPosition;
        }
    }
}
