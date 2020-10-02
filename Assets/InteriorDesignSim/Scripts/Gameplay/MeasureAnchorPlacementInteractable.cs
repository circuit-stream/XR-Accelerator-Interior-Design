using System;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace XRAccelerator.Gameplay
{
    public class MeasureAnchorPlacementInteractable : ARPlacementInteractable
    {
        [NonSerialized]
        public bool IsPlacementEnabled;

        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            return IsPlacementEnabled && base.CanStartManipulationForGesture(gesture);
        }
    }
}