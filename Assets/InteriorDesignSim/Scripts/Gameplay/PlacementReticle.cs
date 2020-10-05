using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using XRAccelerator.Configs;

namespace XRAccelerator.Gameplay
{
    public class PlacementReticle : MonoBehaviour
    {
        private const float minScaleDistance = 0.0f;
        private const float maxScaleDistance = 1.0f;
        private const float scaleMod = 1.0f;

        private readonly List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();

        [SerializeField]
        [Tooltip("Should the reticle scale with the distance to the camera")]
        private bool scaleReticleWithDistance = true;
        [SerializeField]
        [Tooltip("The reticle that will be instantiated")]
        private GameObject reticlePrefab;

        [Header("Scene References")]
        [SerializeField]
        [Tooltip("Reference to the ARRaycastManager component")]
        private ARRaycastManager raycastManager;
        [SerializeField]
        [Tooltip("Reference to the camera transform")]
        private Transform cameraTransform;

        private GameObject spawnedReticleGameObject;
        private Transform spawnedReticleTransform;

        private FurniturePreviewGraphics spawnedPreview;
        private FurnitureConfig currentFurnitureConfig;
        private Action<FurnitureGraphics> furniturePlacedCallback;

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
        }

        public void DisablePreviewFurniture()
        {
            if (spawnedPreview != null)
            {
                DestroyPreview();
            }

            currentFurnitureConfig = null;
        }

        public void Setup(Action<FurnitureGraphics> callback)
        {
            furniturePlacedCallback = callback;
        }

        private void RepositionReticle()
        {
            // TODO Arthur Optional: use GestureTransformationUtility.Raycast
            if (raycastManager.Raycast(ScreenUtils.CenterScreen, raycastHits, TrackableType.PlaneWithinPolygon))
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
            }

            spawnedReticleGameObject.SetActive(false);
        }

        private void ScaleReticle()
        {
            if (!scaleReticleWithDistance)
            {
                return;
            }

            var currentDistance = Vector3.Distance(spawnedReticleTransform.position, cameraTransform.position);

            var currentNormalizedDistance =
                (Mathf.Abs(currentDistance - minScaleDistance) / (maxScaleDistance - minScaleDistance)) +
                scaleMod;

            spawnedReticleTransform.localScale = new Vector3(currentNormalizedDistance,
                currentNormalizedDistance, currentNormalizedDistance);
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

            spawnedPreview.OnWasTapped += OnPreviewWasTapped;
        }

        private void OnPreviewWasTapped()
        {
            CreateDefiniteFurniture();
        }

        private void DestroyPreview()
        {
            StartCoroutine(spawnedPreview.DestroyXRInteractable());
            spawnedPreview = null;
        }

        private void Update()
        {
            RepositionReticle();
            ScaleReticle();
        }

        private void Awake()
        {
            spawnedReticleGameObject = Instantiate(reticlePrefab);
            spawnedReticleGameObject.SetActive(false);
            spawnedReticleTransform = spawnedReticleGameObject.transform;
        }
    }
}