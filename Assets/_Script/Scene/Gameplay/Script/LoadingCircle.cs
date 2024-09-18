using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIAairportSecurity.Training
{
    public class LoadingCircle : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectComponent;
        [SerializeField] private float _rotateSpeed = 200f;
        private bool _loading = false;
        private void OnEnable()
        {
            _loading = true;
        }

        private void OnDisable()
        {
            _loading = false;

            _rectComponent.Rotate(0, 0, 0);
        }
        private void Update()
        {
            if (_loading)
            {
                _rectComponent.Rotate(0f, 0f, _rotateSpeed * Time.deltaTime);
            }
        }
    }
}
