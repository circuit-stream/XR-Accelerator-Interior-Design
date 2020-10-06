using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace XRAccelerator.Gameplay
{
    public class SafeARRotationInteractable : ARRotationInteractable
    {
        private bool wasDestroyed;

        // [XRToolkitWorkaround] Something is keeping a reference to this interactable
        // this way we can kill the object and prevent NullReferenceExceptions.
        public IEnumerator DestroyXRInteractable(bool destroyGameObject = false)
        {
            colliders.Clear();
            wasDestroyed = true;
            gameObject.SetActive(false);
            yield return null;

            if (destroyGameObject)
            {
                Destroy(gameObject);
            }
        }

        protected override bool CanStartManipulationForGesture(DragGesture gesture)
        {
            return !wasDestroyed && base.CanStartManipulationForGesture(gesture);
        }

        protected override bool CanStartManipulationForGesture(TwistGesture gesture)
        {
            return !wasDestroyed && base.CanStartManipulationForGesture(gesture);
        }
    }
}