using System.Collections.Generic;
using UnityEngine;

namespace XRAccelerator.Gameplay
{
    public class ModesController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("List of all modeGraphics")]
        private List<ModeController> modeControllers;

        private ModeController currentMode;

        public void ChangeMode(ModeController newMode)
        {
            if (newMode == currentMode)
            {
                return;
            }

            if (currentMode != null)
            {
                currentMode.DisableMode();
            }

            currentMode = newMode;
            currentMode.EnableMode();
        }

        private void SetupModes()
        {

            foreach (var modeGraphics in modeControllers)
            {
                modeGraphics.Setup(this);
                modeGraphics.DisableMode();
            }
        }

        private void Awake()
        {
            SetupModes();
        }
    }
}