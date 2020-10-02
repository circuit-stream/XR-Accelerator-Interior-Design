using System.Collections.Generic;
using Signals;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using XRAccelerator.Configs;
using XRAccelerator.Services;
using XRAccelerator.Signals;

namespace XRAccelerator.Gameplay
{
    public class PlacementReticle : MonoBehaviour
    {
        private const float minScaleDistance = 0.0f;
        private const float maxScaleDistance = 1.0f;
        private const float scaleMod = 1.0f;

        private readonly List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();

        [SerializeField]
        [Tooltip("TooltipText")]
        private bool scaleReticleWithDistance = true;
        [SerializeField]
        [Tooltip("TooltipText")]
        private GameObject reticlePrefab;

        [Header("Scene References")]
        [SerializeField]
        [Tooltip("TooltipText")]
        private ARRaycastManager raycastManager;
        [SerializeField]
        [Tooltip("TooltipText")]
        private Transform cameraTransform;

        private GameObject spawnedReticleGameObject;
        private Transform spawnedReticleTransform;

        private FurniturePreviewGraphics spawnedPreview;
        private FurnitureConfig currentFurnitureConfig;

        public bool ValidReticlePosition => spawnedReticleGameObject.activeSelf;

        public Transform GetReticleTransform()
        {
            return spawnedReticleTransform;
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

            ServiceLocator.GetService<SignalDispatcher>().Dispatch(new FurniturePlaced
            {
                Config = currentFurnitureConfig,
                Graphics = newObject
            });
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
            StartCoroutine(spawnedPreview.DestroyXRInteractable());
            CreateDefiniteFurniture();
        }

        private void OnFurnitureSelected(FurnitureSelected signalParams)
        {
            if (signalParams.Config == currentFurnitureConfig)
            {
                return;
            }

            currentFurnitureConfig = signalParams.Config;
            CreatePreviewFurniture();
        }

        private void OnSelectingFurniture(SelectingFurniture signalParams)
        {
            if (spawnedPreview != null)
            {
                StartCoroutine(spawnedPreview.DestroyXRInteractable());
            }

            spawnedPreview = null;
            currentFurnitureConfig = null;
        }

        private void OnFurniturePlaced(FurniturePlaced signalParams)
        {
            currentFurnitureConfig = null;
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
            RegisterSignals();
        }

        private void RegisterSignals()
        {
            var signalDispatcher = ServiceLocator.GetService<SignalDispatcher>();
            signalDispatcher.AddListener<FurnitureSelected>(OnFurnitureSelected);
            signalDispatcher.AddListener<SelectingFurniture>(OnSelectingFurniture);
            signalDispatcher.AddListener<FurniturePlaced>(OnFurniturePlaced);
        }
    }
}