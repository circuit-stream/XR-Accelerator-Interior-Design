using System.Collections;
using System.Threading.Tasks;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace XRAccelerator.Gameplay
{
    public class SafeARSelectionInteractable : ARSelectionInteractable
    {
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

        public override bool IsSelectableBy(XRBaseInteractor interactor)
        {
            return !wasDestroyed && base.IsSelectableBy(interactor);
        }

        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            return !wasDestroyed && base.CanStartManipulationForGesture(gesture);
        }

        protected override bool IsGameObjectSelected()
        {
            return !wasDestroyed && base.IsGameObjectSelected();
        }
    }
}