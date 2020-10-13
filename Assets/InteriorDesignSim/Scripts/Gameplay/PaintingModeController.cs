using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using XRAccelerator.Enums;

namespace XRAccelerator.Gameplay
{
    public class PaintingModeController : ModeController
    {
        private static readonly int RangeShaderID = Shader.PropertyToID("_Range");
        private static readonly int FuzzinessShaderID = Shader.PropertyToID("_Fuzziness");
        private static readonly int FromColorShaderID = Shader.PropertyToID("_FromColor");
        private static readonly int TargetColorShaderID = Shader.PropertyToID("_TargetColor");

        private static readonly float rangeShaderValue = 0.2f;
        private static readonly float fuzzinessShaderValue = 0f;
        private static readonly int XRCpuImageSize = 3;

        public override Mode Mode => Mode.Painting;

        [Header("UI Related")]
        [SerializeField]
        [Tooltip("Reference to the button that resets the painting")]
        private Button clearButton;

        [SerializeField]
        [Tooltip("Reference to the button that will detect screen clicks")]
        private Button screenOverlayButton;

        [SerializeField]
        [Tooltip("Reference to the available painting colors")]
        private List<PaintingColorEntry> possibleReplaceColors;

        [SerializeField]
        [Tooltip("Reference to the holder of painting instructions")]
        private GameObject instructionsHolder;

        [Header("AR Camera Related")]
        [SerializeField]
        [Tooltip("Reference to the ARCameraManager component")]
        private ARCameraManager arCameraManager;

        [SerializeField]
        [Tooltip("Referemce to the ARCameraBackground component")]
        private ARCameraBackground arCameraBackground;

        [SerializeField]
        [Tooltip("The material applied when painting wall on android devices")]
        private Material androidMaterial;

        [SerializeField]
        [Tooltip("The material applied when painting wall on IOS devices")]
        private Material iosMaterial;

        private Material usedCustomMaterial;

        private int selectedReplaceColorIndex;
        private PaintingColorEntry SelectedColorEntry => possibleReplaceColors[selectedReplaceColorIndex];

        private Color paintColor;
        private Color? replaceColor;

        private bool paintedOnce;

        public override void EnableMode()
        {
            screenOverlayButton.gameObject.SetActive(true);
            instructionsHolder.SetActive(!paintedOnce);

            base.EnableMode();
        }

        public override void DisableMode()
        {
            screenOverlayButton.gameObject.SetActive(false);
            instructionsHolder.SetActive(false);

            base.DisableMode();
        }

        private Color GetScreenColorAtTouch()
        {
            if (!arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
            {
                return replaceColor.Value;
            }

            var rect = GetConversionInputRect(cpuImage);

            var conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = rect,
                outputDimensions = new Vector2Int(1, 1), // Smaller size forces downsampling (using nearest neighbor)
                outputFormat = TextureFormat.RGB24, // 8 bit per channel color texture format
                transformation = XRCpuImage.Transformation.MirrorY
            };

            int size = cpuImage.GetConvertedDataSize(conversionParams);
            var buffer = new NativeArray<byte>(size, Allocator.Temp);
            cpuImage.Convert(conversionParams, buffer);
            cpuImage.Dispose();

            // TODO Arthur: Cache texture
            var tempTexture = new Texture2D(
                conversionParams.outputDimensions.x,
                conversionParams.outputDimensions.y,
                conversionParams.outputFormat,
                false);

            tempTexture.LoadRawTextureData(buffer);
            tempTexture.Apply();
            buffer.Dispose();

            return tempTexture.GetPixel(0, 0);
        }

        private RectInt GetConversionInputRect(XRCpuImage cpuImage)
        {
            Touch touch = Input.GetTouch(0);

            float factor;
            int screenOverflow;

            switch (Screen.orientation)
            {
                case ScreenOrientation.Portrait:
                    factor = (float)cpuImage.width / Screen.height;
                    screenOverflow = cpuImage.height - (int)(Screen.width * factor);

                    return new RectInt(
                        (int)(touch.position.y * factor),
                        cpuImage.height - ((int) (touch.position.x * factor) + screenOverflow / 2),
                        XRCpuImageSize,
                        XRCpuImageSize);

                case ScreenOrientation.PortraitUpsideDown:
                    factor = (float)cpuImage.width / Screen.height;
                    screenOverflow = cpuImage.height - (int)(Screen.width * factor);

                    return new RectInt(
                        (int) (touch.position.y * factor),
                        (int) (touch.position.x * factor) + screenOverflow / 2,
                        XRCpuImageSize,
                        XRCpuImageSize);

                    break;
                case ScreenOrientation.LandscapeLeft:
                    factor = (float)cpuImage.width / Screen.width;
                    screenOverflow = cpuImage.height - (int)(Screen.height * factor);

                    return new RectInt(
                        cpuImage.width - (int) (touch.position.x * factor),
                        cpuImage.height - ((int) (touch.position.y * factor) + screenOverflow / 2),
                        XRCpuImageSize,
                        XRCpuImageSize);

                case ScreenOrientation.LandscapeRight:
                    factor = (float)cpuImage.width / Screen.width;
                    screenOverflow = cpuImage.height - (int)(Screen.height * factor);

                    return new RectInt(
                        (int) (touch.position.x * factor),
                        (int) (touch.position.y * factor) + screenOverflow / 2,
                        XRCpuImageSize,
                        XRCpuImageSize);
            }

            throw new NotImplementedException();
        }

        private void OnScreenOverlayClicked()
        {
            instructionsHolder.SetActive(false);
            paintedOnce = true;

            replaceColor = GetScreenColorAtTouch();
            usedCustomMaterial.SetColor(FromColorShaderID, replaceColor.Value);
            arCameraBackground.useCustomMaterial = true;
        }

        private void OnClearButtonClicked()
        {
            replaceColor = null;
            arCameraBackground.useCustomMaterial = false;
        }

        private void OnColorSelected(Color newColor, int colorIndex)
        {
            if (colorIndex == selectedReplaceColorIndex)
            {
                return;
            }

            SetReplaceColor(newColor);

            SelectedColorEntry.Dehighlight();
            selectedReplaceColorIndex = colorIndex;
            SelectedColorEntry.Highlight();
        }

        private void SetReplaceColor(Color newColor)
        {
            paintColor = newColor;
            usedCustomMaterial.SetColor(TargetColorShaderID, paintColor);
        }

        private void SetupBackgroundMaterial()
        {
#if UNITY_ANDROID
            usedCustomMaterial = androidMaterial;
#elif UNITY_IOS
            usedCustomMaterial = iosMaterial;
#else
            return;
#endif

            usedCustomMaterial.SetFloat(RangeShaderID, rangeShaderValue);
            usedCustomMaterial.SetFloat(FuzzinessShaderID, fuzzinessShaderValue);
            arCameraBackground.customMaterial = usedCustomMaterial;
        }

        private void SetupUI()
        {
            clearButton.onClick.AddListener(OnClearButtonClicked);
            screenOverlayButton.onClick.AddListener(OnScreenOverlayClicked);

            for (var index = 0; index < possibleReplaceColors.Count; index++)
            {
                var colorEntry = possibleReplaceColors[index];
                colorEntry.Setup(index, OnColorSelected);
            }

            selectedReplaceColorIndex = 0;
            SetReplaceColor(SelectedColorEntry.GetEntryColor());
            SelectedColorEntry.Highlight();
        }

        private void Awake()
        {
            SetupBackgroundMaterial();
            SetupUI();
        }
    }
}