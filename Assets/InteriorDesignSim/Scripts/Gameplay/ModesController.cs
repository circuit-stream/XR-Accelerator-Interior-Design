using System.Collections.Generic;
using UnityEngine;
using XRAccelerator.Enums;

namespace XRAccelerator.Gameplay
{
    public class ModesController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("List of all modeGraphics")]
        private List<ModeController> modeControllers;

        private Dictionary<Mode, ModeController> controllersByMode;

        private Mode currentMode;
        private ModeController CurrentModeController => controllersByMode[currentMode];

        public void ChangeMode(Mode newMode)
        {
            if (newMode == currentMode)
            {
                return;
            }

            if (currentMode != Mode.Inactive)
            {
                CurrentModeController.DisableMode();
            }

            currentMode = newMode;
            CurrentModeController.EnableMode();
        }

        public ModeController GetMode(Mode targetMode)
        {
            return controllersByMode[targetMode];
        }

        private void SetupModes()
        {
            currentMode = Mode.Inactive;
            controllersByMode = new Dictionary<Mode, ModeController>();

            foreach (var modeController in modeControllers)
            {
                modeController.Setup(this);
                modeController.DisableMode();
                controllersByMode[modeController.Mode] = modeController;
            }
        }

        private void Awake()
        {
            SetupModes();
        }
    }
}