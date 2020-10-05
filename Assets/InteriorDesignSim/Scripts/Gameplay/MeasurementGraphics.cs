using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XRAccelerator.Gameplay
{
    public class MeasurementGraphics : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("TooltipText")]
        private TMP_Text measurementText;

        [SerializeField]
        [Tooltip("TooltipText")]
        private LayoutGroup layoutGroup;

        private Transform anchor1;
        private Vector3 lastAnchor1Position;

        private Transform anchor2;
        private Vector3 lastAnchor2Position;
        private Transform mainCameraTransform;
        private Transform _transform;

        public void ChangeAnchor2(Transform newAnchor)
        {
            anchor2 = newAnchor;
            ResetMeasurement();
        }

        public void Setup(Transform anchor1, Transform anchor2)
        {
            this.anchor1 = anchor1;
            this.anchor2 = anchor2;

            ResetMeasurement();
        }

        private void ResetMeasurement()
        {
            lastAnchor1Position = anchor1.position;
            lastAnchor2Position = anchor2.position;

            var direction = lastAnchor1Position - lastAnchor2Position;
            var distance = direction.magnitude;

            measurementText.text = ParseDistance(distance);

            _transform.position = lastAnchor2Position + distance * 0.5f * direction.normalized + new Vector3(0, 0.05f, 0);

            layoutGroup.enabled = false;
            Canvas.ForceUpdateCanvases();
            layoutGroup.enabled = true;
        }

        private string ParseDistance(float distance)
        {
            if (distance > 1)
                return $"{distance:F2} m";

            return $"{(distance * 100):f0} cm";
        }

        private void LateUpdate()
        {
            if (lastAnchor1Position != anchor1.position || lastAnchor2Position != anchor2.position)
            {
                ResetMeasurement();
            }

            _transform.forward = mainCameraTransform.forward;
        }

        private void Awake()
        {
            mainCameraTransform = Camera.main.transform;
            _transform = transform;
        }
    }
}