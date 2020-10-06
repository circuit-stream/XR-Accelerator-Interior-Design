using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit;
using XRAccelerator.Configs;

namespace XRAccelerator.Gameplay
{
    public class PlacementReticle : MonoBehaviour
    {
        private readonly List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();

        [SerializeField]
        [Tooltip("The reticle that will be instantiated")]
        private GameObject reticlePrefab;

        [Header("Scene References")]
        [SerializeField]
        [Tooltip("Reference to the ARRaycastManager component")]
        private ARRaycastManager raycastManager;
        [SerializeField]
        [Tooltip("Reference to the ARPlaneManager component")]
        private ARPlaneManager arPlaneManager;
        [SerializeField]
        [Tooltip("Reference to the camera transform")]
        private Transform cameraTransform;

        private GameObject spawnedReticleGameObject;
        private Transform spawnedReticleTransform;

        private SafeARSelectionInteractable spawnedPreview;
        private FurnitureConfig currentFurnitureConfig;
        private Action<SafeARSelectionInteractable> furniturePlacedCallback;

        public void Enable()
        {
            gameObject.SetActive(true);

            if (currentFurnitureConfig != null)
            {
                CreatePreviewFurniture();
            }
        }

        public void Disable()
        {
            if (spawnedPreview != null)
            {
                DestroyPreview();
            }

            gameObject.SetActive(false);
        }

        public void EnablePreviewFurniture(FurnitureConfig newConfig)
        {
            if (newConfig == currentFurnitureConfig)
            {
                return;
            }

            currentFurnitureConfig = newConfig;
            CreatePreviewFurniture();
            UpdatePreviewVisibility();
        }

        public void DisablePreviewFurniture()
        {
            if (spawnedPreview != null)
            {
                DestroyPreview();
            }

            currentFurnitureConfig = null;
        }

        public void Setup(Action<SafeARSelectionInteractable> callback)
        {
            furniturePlacedCallback = callback;
        }

        private void RepositionReticle()
        {
            Pose hitPose = raycastHits[0].pose;

            // Use hit pose and camera pose to check if the hit is not from the back of the plane
            if (Vector3.Dot(cameraTransform.position - hitPose.position,
                hitPose.rotation * Vector3.up) >= 0)
            {
                spawnedReticleTransform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
                spawnedReticleGameObject.SetActive(true);

                return;
            }

            spawnedReticleGameObject.SetActive(false);
        }

        private void UpdatePreviewVisibility()
        {
            if (spawnedPreview == null)
            {
                return;
            }

            if (arPlaneManager.trackables.TryGetTrackable(raycastHits[0].trackableId, out ARPlane arPlane))
            {
                // TODO: Disable Interactions
                spawnedPreview.gameObject.SetActive(spawnedReticleGameObject.activeSelf && currentFurnitureConfig.CanFitInPlane(arPlane));
            }
        }

        private void CreateDefiniteFurniture()
        {
            var transformCache = spawnedPreview.transform;
            var newObject = Instantiate(currentFurnitureConfig.FurniturePrefab, transformCache.position, transformCache.rotation);
            furniturePlacedCallback(newObject);
        }

        private void CreatePreviewFurniture()
        {
            spawnedPreview = Instantiate(currentFurnitureConfig.furniturePreviewPrefab, spawnedReticleTransform);
            spawnedPreview.transform.localPosition = Vector3.zero;
            spawnedPreview.transform.localRotation = Quaternion.identity;

            spawnedPreview.onSelectEnter.AddListener(OnPreviewWasTapped);
        }

        private void OnPreviewWasTapped(XRBaseInteractor interactor)
        {
            CreateDefiniteFurniture();
        }

        private void DestroyPreview()
        {
            spawnedPreview.GetComponent<SafeARRotationInteractable>().DestroyXRInteractable();
            spawnedPreview.DestroyXRInteractable(true);
            spawnedPreview = null;
        }

        private void Update()
        {
            // TODO Arthur Optional: use GestureTransformationUtility.Raycast
            if (!raycastManager.Raycast(ScreenUtils.CenterScreen, raycastHits, TrackableType.PlaneWithinPolygon))
            {
                return;
            }

            RepositionReticle();
            UpdatePreviewVisibility();
        }

        private void Awake()
        {
            spawnedReticleGameObject = Instantiate(reticlePrefab);
            spawnedReticleGameObject.SetActive(false);
            spawnedReticleTransform = spawnedReticleGameObject.transform;
        }
    }
}