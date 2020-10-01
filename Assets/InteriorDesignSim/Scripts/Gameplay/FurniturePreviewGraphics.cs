using System;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace XRAccelerator.Gameplay
{
    public class FurniturePreviewGraphics : ARBaseGestureInteractable
    {
        public Action OnWasTapped;

        private bool wasDestroyed;

        // [XRToolkitWorkaround] Something is keeping a reference to this interactable
        // this way we can kill the object and prevent NullReferenceExceptions.
        public void DestroyXRInteractable()
        {
            OnWasTapped = null;
            colliders.Clear();
            Destroy(gameObject);
            wasDestroyed = true;
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
    }
}