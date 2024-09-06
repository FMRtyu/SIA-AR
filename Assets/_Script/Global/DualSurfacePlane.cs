using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;

namespace SIAairportSecurity.Training
{
    [RequireComponent(typeof(ARPlaneMeshVisualizer))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class DualSurfacePlane : MonoBehaviour
    {
        [SerializeField] private float _surfaceOffset = 0.01f; // The distance between the two surfaces
        [SerializeField] private Material _dotsMat;

        private ARPlaneMeshVisualizer _meshVisualizer;
        private MeshFilter _meshFilter;
        private GameObject _secondSurface;
        private MeshFilter _secondSurfaceMeshFilter;
        private MeshRenderer _secondSurfaceRenderer;
        private MeshCollider _secondSurfaceCollider;

        void Awake()
        {
            _meshVisualizer = GetComponent<ARPlaneMeshVisualizer>();
            _meshFilter = GetComponent<MeshFilter>();

            // Create the second surface
            CreateSecondSurface();
        }

        void OnEnable()
        {
            // Subscribe to the ARPlane's boundaryChanged event
            //GetComponent<ARPlane>().boundaryChanged += OnPlaneBoundaryChanged;
        }

        void OnDisable()
        {
            // Unsubscribe from the boundaryChanged event to avoid memory leaks
            GetComponent<ARPlane>().boundaryChanged -= OnPlaneBoundaryChanged;
        }

        void CreateSecondSurface()
        {
            // Create a new GameObject for the second surface
            _secondSurface = new GameObject("SecondSurface");
            _secondSurface.transform.SetParent(transform, false);
            _secondSurface.transform.localPosition = new Vector3(0, _surfaceOffset, 0); // Offset above the original surface

            // Set the layer of the second surface to "ARPlane"
            _secondSurface.layer = LayerMask.NameToLayer("ARPlane");

            // Add necessary components to the second surface
            _secondSurfaceMeshFilter = _secondSurface.AddComponent<MeshFilter>();
            _secondSurfaceRenderer = _secondSurface.AddComponent<MeshRenderer>();
            _secondSurfaceCollider = _secondSurface.AddComponent<MeshCollider>();
            _secondSurface.AddComponent<ARFeatheredPlaneMeshVisualizer>();

            // Use the same material as the original surface or assign a different one if needed
            _secondSurfaceRenderer.material = _dotsMat;

            // Initialize the mesh
            UpdateSecondSurfaceMesh();
        }

        void OnPlaneBoundaryChanged(ARPlaneBoundaryChangedEventArgs eventArgs)
        {
            // Update the second surface mesh whenever the ARPlane boundary changes
            UpdateSecondSurfaceMesh();
        }

        void UpdateSecondSurfaceMesh()
        {
            if (_secondSurfaceMeshFilter != null)
            {
                // Duplicate the mesh from the ARPlane's mesh visualizer
                Mesh newMesh = Instantiate(_meshVisualizer.mesh);
                _secondSurfaceMeshFilter.mesh = newMesh;

                // Update the MeshCollider with the new mesh
                if (_secondSurfaceCollider != null)
                {
                    _secondSurfaceCollider.sharedMesh = newMesh;
                }
            }
        }

        // Function to change the material of the second surface
        public void ChangeSecondSurfaceMaterial(Material newMaterial)
        {
            if (_secondSurfaceRenderer != null)
            {
                _secondSurfaceRenderer.material = newMaterial;
            }
        }
    }
}
