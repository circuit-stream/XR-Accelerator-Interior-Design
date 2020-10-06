using System;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace XRAccelerator.Gameplay
{
    public class MeasureAnchorPlacementInteractable : ARPlacementInteractable
    {
        [NonSerialized]
        public bool IsPlacementEnabled;

        // [XRToolkitWorkaround] ARPlacementInteractable doesn't allow disabling / enabling placement
        // [XRToolkitWorkaround] Gestures disregard if the touch was handled by the EventSystem (UI Canvas)
        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            return IsPlacementEnabled &&
                !EventSystem.current.IsPointerOverGameObject(gesture.FingerId) &&
                base.CanStartManipulationForGesture(gesture);
        }
    }
}