using System;
using UnityEngine;
using UnityEngine.UI;

namespace XRAccelerator.Gameplay
{
    public class PaintingColorEntry : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to the button of this color entry")]
        private Button button;

        [SerializeField]
        [Tooltip("Reference to the holder image of this color entry, for highlighting")]
        private Image holderImage;

        [SerializeField]
        [Tooltip("Reference to the image that uses the painting color of this entry")]
        private Image colorImage;

        private int entryIndex;
        private Action<Color, int> onClickCallback;

        public void Setup(int index, Action<Color, int> callback)
        {
            entryIndex = index;
            onClickCallback = callback;
            button.onClick.AddListener(onClick);
            Dehighlight();
        }

        public Color GetEntryColor()
        {
            return colorImage.color;
        }

        public void Highlight()
        {
            holderImage.color = Color.white;
        }

        public void Dehighlight()
        {
            holderImage.color = GetEntryColor();
        }

        private void onClick()
        {
            onClickCallback(GetEntryColor(), entryIndex);
        }
    }
}