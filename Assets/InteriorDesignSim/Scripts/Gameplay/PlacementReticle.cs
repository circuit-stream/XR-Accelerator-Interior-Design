using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace XRAccelerator.Gameplay
{
    public class PlacementReticle : MonoBehaviour
    {
        private const float minScaleDistance = 0.0f;
        private const float maxScaleDistance = 1.0f;
        private const float scaleMod = 1.0f;

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

        private GameObject spawnedReticle;
        private readonly List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();

        public Transform GetReticlePosition()
        {
            return spawnedReticle.transform;
        }

        private void RepositionReticle()
        {
            if (raycastManager.Raycast(ScreenUtils.CenterScreen, raycastHits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = raycastHits[0].pose;
                spawnedReticle.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
                spawnedReticle.SetActive(true);

                return;
            }

            spawnedReticle.SetActive(false);
        }

        private void ScaleReticle()
        {
            if (!scaleReticleWithDistance)
            {
                return;
            }

            var currentDistance = Vector3.Distance(spawnedReticle.transform.position, cameraTransform.position);

            var currentNormalizedDistance =
                (Mathf.Abs(currentDistance - minScaleDistance) / (maxScaleDistance - minScaleDistance)) +
                scaleMod;
            spawnedReticle.transform.localScale = new Vector3(currentNormalizedDistance,
                currentNormalizedDistance, currentNormalizedDistance);
        }

        private void Update()
        {
            RepositionReticle();
            ScaleReticle();
        }

        private void Start()
        {
            spawnedReticle = Instantiate(reticlePrefab);
            spawnedReticle.SetActive(false);
        }
    }
}