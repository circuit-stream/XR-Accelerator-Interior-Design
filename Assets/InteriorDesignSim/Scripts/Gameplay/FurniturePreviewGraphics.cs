using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace XRAccelerator.Gameplay
{
    public class FurniturePreviewGraphics : ARRotationInteractable
    {
        public Action OnWasTapped;

        private bool wasDestroyed;

        // [XRToolkitWorkaround] Something is keeping a reference to this interactable
        // this way we can kill the object and prevent NullReferenceExceptions.
        public IEnumerator DestroyXRInteractable()
        {
            OnWasTapped = null;
            colliders.Clear();
            wasDestroyed = true;
            gameObject.SetActive(false);
            yield return null;

            Destroy(gameObject);
        }

        public override bool IsSelectableBy(XRBaseInteractor interactor)
        {
            return interactor is ARGestureInteractor && !wasDestroyed;
        }

        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            return !wasDestroyed;
        }

        protected override void OnEndManipulation(TapGesture gesture)
        {
            if (gesture.WasCancelled || gestureInteractor == null || gesture.TargetObject != gameObject)
            {
                return;
            }

            OnWasTapped?.Invoke();
        }

        protected override bool IsGameObjectSelected()
        {
            return !wasDestroyed && base.IsGameObjectSelected();
        }
    }
}