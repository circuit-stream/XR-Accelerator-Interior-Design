using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace XRAccelerator.Gameplay
{
    [RequireComponent(typeof(Light))]
    public class LightEstimation : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to the ARCameraManager component")]
        private ARCameraManager arCameraManager;

        [SerializeField]
        [Tooltip("Referente to a light that will be controlled by the AR lightEstimations")]
        private Light mainLight;

        private void FrameChanged(ARCameraFrameEventArgs args)
        {
            if (args.lightEstimation.averageBrightness.HasValue)
            {
                mainLight.intensity = args.lightEstimation.averageBrightness.Value;
            }

            if (args.lightEstimation.averageColorTemperature.HasValue)
            {
                mainLight.colorTemperature = args.lightEstimation.averageColorTemperature.Value;
            }

            if (args.lightEstimation.colorCorrection.HasValue)
            {
                mainLight.color = args.lightEstimation.colorCorrection.Value;
            }

            if (args.lightEstimation.mainLightDirection.HasValue)
            {
                mainLight.transform.rotation = Quaternion.LookRotation(args.lightEstimation.mainLightDirection.Value);
            }

            if (args.lightEstimation.mainLightColor.HasValue)
            {
#if PLATFORM_ANDROID
                // ARCore needs to apply energy conservation term (1 / PI) and be placed in gamma
                mainLight.color = args.lightEstimation.mainLightColor.Value / Mathf.PI;
                mainLight.color = mainLight.color.gamma;
#endif
            }

            if (args.lightEstimation.averageMainLightBrightness.HasValue)
            {
                mainLight.intensity = args.lightEstimation.averageMainLightBrightness.Value;
            }
        }

        private void OnEnable()
        {
            if (arCameraManager != null)
            {
                arCameraManager.frameReceived += FrameChanged;
            }
        }

        private void OnDisable()
        {
            if (arCameraManager != null)
            {
                arCameraManager.frameReceived -= FrameChanged;
            }
        }
    }
}