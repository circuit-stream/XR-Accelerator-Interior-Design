using TMPro;
using UnityEngine;

namespace XRAccelerator.Gameplay
{
    public class MeasurementGraphics : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("TooltipText")]
        private TMP_Text measurementText;

        private Transform anchor1;
        private Vector3 lastAnchor1Position;

        private Transform anchor2;
        private Vector3 lastAnchor2Position;

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

            // TODO Arthur: Parse to cm / mm / ...
            measurementText.text = distance.ToString("F2");

            transform.position = lastAnchor2Position + distance * 0.5f * direction.normalized;
        }

        private void Update()
        {
            if (lastAnchor1Position != anchor1.position || lastAnchor2Position != anchor2.position)
            {
                ResetMeasurement();
            }
        }
    }
}