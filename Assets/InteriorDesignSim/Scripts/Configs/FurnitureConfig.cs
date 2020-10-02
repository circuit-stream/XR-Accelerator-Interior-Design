using UnityEngine;
using XRAccelerator.Enums;
using XRAccelerator.Gameplay;

namespace XRAccelerator.Configs
{
    [CreateAssetMenu(fileName = "New Furniture Config", menuName = "Configs/Furniture")]
    public class FurnitureConfig : ScriptableObject
    {
        [SerializeField]
        [Tooltip("This furniture unique identifier")]
        public string Id;

        [SerializeField]
        [Tooltip("The type of this furniture")]
        public FurnitureType FurnitureType;

        [SerializeField]
        [Tooltip("The prefab instantiated")]
        public FurnitureGraphics FurniturePrefab;

        [SerializeField]
        [Tooltip("The preview prefab showed during placement")]
        public FurniturePreviewGraphics furniturePreviewPrefab;

        [SerializeField]
        [Tooltip("Furniture preview displayed during furniture seleciton")]
        public Sprite Thumbnail;

        [SerializeField]
        [Tooltip("How expensive is this furniture")]
        public float Price;
    }
}
