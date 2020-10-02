using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;
using XRAccelerator.Enums;

namespace XRAccelerator.Gameplay
{
    public class MeasurementModeController : ModeController
    {
        [SerializeField]
        [Tooltip("Reference to the MeasureAnchorPlacementInteractable component")]
        private MeasureAnchorPlacementInteractable anchorPlacementInteractable;

        [SerializeField]
        [Tooltip("Reference to the measurement prefab")]
        private MeasurementGraphics measurementPrefab;

        private readonly List<Transform> anchors = new List<Transform>();
        private readonly List<MeasurementGraphics> measurementGraphics = new List<MeasurementGraphics>();

        public override Mode Mode => Mode.Measurement;

        public override void EnableMode()
        {
            anchorPlacementInteractable.IsPlacementEnabled = true;

            base.EnableMode();
        }

        public override void DisableMode()
        {
            anchorPlacementInteractable.IsPlacementEnabled = false;

            base.DisableMode();
        }

        private void DeleteAnchor()
        {

        }

        private void DeleteMeasurement()
        {

        }

        private void CreateMeasurement(int anchor2Index)
        {
            if (anchor2Index - 1 < 0)
            {
                return;
            }

            var newMeasurement = Instantiate(measurementPrefab);
            newMeasurement.Setup(anchors[anchor2Index - 1], anchors[anchor2Index]);
            measurementGraphics.Add(newMeasurement);
        }

        private void OnAnchorPlaced(ARPlacementInteractable interactable, GameObject newAnchor)
        {
            // TODO Arthur: Change UI text

            anchors.Add(newAnchor.transform);
            CreateMeasurement(anchors.Count - 1);
        }

        public override void Setup(ModesController controller)
        {
            base.Setup(controller);

            anchorPlacementInteractable.onObjectPlaced.AddListener(OnAnchorPlaced);
        }
    }
}