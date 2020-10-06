using System;
using UnityEngine;
using UnityEngine.UI;
using XRAccelerator.Enums;

namespace XRAccelerator.Gameplay
{
    public class InspectionModeController : ModeController
    {
        public override Mode Mode => Mode.Inspection;

        [SerializeField]
        [Tooltip("Reference to the holder of the clear measurement anchors button")]
        private Button deleteButton;

        public Action OnDeleteRequest;

        public override void Setup(ModesController controller)
        {
            base.Setup(controller);

            deleteButton.onClick.AddListener(() => OnDeleteRequest?.Invoke());
        }
    }
}