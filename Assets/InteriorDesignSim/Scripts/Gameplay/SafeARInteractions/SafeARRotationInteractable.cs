using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace XRAccelerator.Gameplay
{
    public class SafeARRotationInteractable : ARRotationInteractable
    {
        [SerializeField]
        [Tooltip("If this object should always try to be selected")]
        private bool forceSelection;

        private bool wasDestroyed;

        // [XRToolkitWorkaround] Something is keeping a reference to this interactable
        // this way we can kill the object and prevent NullReferenceExceptions.
        public async void DestroyXRInteractable(bool destroyGameObject = false)
        {
            colliders.Clear();
            wasDestroyed = true;
            gameObject.SetActive(false);

            await Task.Delay(100);

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

        protected override bool IsGameObjectSelected()
        {
            // [XRToolkitWorkaround] Force selection is used here so the preview can rotate without being tapped before
            // XRToolkit provides no way to select an interactable by code.
            return !wasDestroyed && (forceSelection || base.IsGameObjectSelected());
        }
    }
}