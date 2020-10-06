using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.AR;
using XRAccelerator.Enums;

namespace XRAccelerator.Gameplay
{
    public class MeasurementModeController : ModeController
    {
        private static List<string> instructionTexts = new List<string>
        {
            "Tap to place measurement start point.",
            "Tap to place measurement end point.",
            "Tap to place additional measurements."
        };

        [Header("Measurement Mode Specific")]
        [SerializeField]
        [Tooltip("Reference to the MeasureAnchorPlacementInteractable component")]
        private MeasureAnchorPlacementInteractable anchorPlacementInteractable;

        [SerializeField]
        [Tooltip("Reference to the measurement prefab")]
        private MeasurementGraphics measurementPrefab;

        [SerializeField]
        [Tooltip("Reference to the Instruction Text component")]
        private TMP_Text InstructionsText;

        [SerializeField]
        [Tooltip("Reference to the holder of the clear measurement anchors button")]
        private Button ClearButtonHolder;

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

        private void ResetUI()
        {
            InstructionsText.text = instructionTexts[Math.Min(instructionTexts.Count - 1, anchors.Count)];
            ClearButtonHolder.gameObject.SetActive(anchors.Count >= 2);
        }

        private void DeleteAnchors()
        {
            foreach (var anchor in anchors)
            {
                anchor.GetComponentInChildren<SafeARTranslationInteractable>().DestroyXRInteractable();
                anchor.GetComponentInChildren<SafeARSelectionInteractable>().DestroyXRInteractable(true);
            }

            foreach (var graphics in measurementGraphics)
            {
                Destroy(graphics.gameObject);
            }

            anchors.Clear();
            measurementGraphics.Clear();
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
            anchors.Add(newAnchor.transform);
            CreateMeasurement(anchors.Count - 1);
            ResetUI();
        }

        public override void Setup(ModesController controller)
        {
            base.Setup(controller);

            ResetUI();

            anchorPlacementInteractable.onObjectPlaced.AddListener(OnAnchorPlaced);
            ClearButtonHolder.onClick.AddListener(DeleteAnchors);
        }
    }
}