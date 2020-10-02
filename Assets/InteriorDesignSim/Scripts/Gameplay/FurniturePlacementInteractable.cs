using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace XRAccelerator.Gameplay
{
    public class FurniturePlacementInteractable : ARBaseGestureInteractable
    {
        [SerializeField]
        [Tooltip("Reference to a Furniture Catalog Graphics component")]
        private FurnitureCatalogGraphics furnitureCatalog;

        [SerializeField]
        [Tooltip("Reference to a PlacementReticle component")]
        private PlacementReticle placementReticle;

        public Action<FurnitureGraphics> OnObjectPlaced;

        private readonly List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();

        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            return gesture.TargetObject == null;
        }

        protected override void OnEndManipulation(TapGesture gesture)
        {
            if (gesture.WasCancelled || gesture.TargetObject != null)
            {
                return;
            }

            if (!placementReticle.ValidReticlePosition)
            {
                return;
            }

            if (furnitureCatalog.IsSelectingFurniture)
            {
                return;
            }

            var placementTransform = placementReticle.GetReticleTransform();

            var newObject = Instantiate(furnitureCatalog.GetSelectedFurniture().FurniturePrefab, placementTransform.position, placementTransform.rotation);
            OnObjectPlaced?.Invoke(newObject);
        }
    }
}