using UnityEngine;
using UnityEngine.UI;
using XRAccelerator.Configs;

namespace XRAccelerator.Gameplay
{
    public class FurnitureCatalogEntry : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to the thumbnail image component")]
        private Image thumbnail;

        [SerializeField]
        [Tooltip("Reference to the name text component")]
        private Text nameText;

        [SerializeField]
        [Tooltip("Reference to the entry cell holder image component")]
        private Image holderImage;

        public FurnitureConfig Config { get; private set; }

        // TODO Arthur: Make UI pretty

        public void Highlight()
        {
            var color = holderImage.color;
            color.a = 1;
            holderImage.color = color;
        }

        public void DeHighlight()
        {
            var color = holderImage.color;
            color.a = 0.4f;
            holderImage.color = color;
        }

        public void Setup(FurnitureConfig furnitureConfig)
        {
            gameObject.name = $"[CatalogEntry] {furnitureConfig.Id}";
            thumbnail.sprite = furnitureConfig.Thumbnail;

            // TODO Arthur: Localization
            nameText.text = furnitureConfig.Id;

            Config = furnitureConfig;
        }
    }
}