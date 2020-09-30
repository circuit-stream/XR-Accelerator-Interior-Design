using UnityEngine;
using XRAccelerator.Enums;
using XRAccelerator.Gameplay;

namespace XRAccelerator.Configs
{
    public class FurnitureConfig : ScriptableObject
    {
        [SerializeField]
        [Tooltip("This furniture unique identifier")]
        public string Id;

        [SerializeField]
        [Tooltip("The type of this furniture")]
        public FurnitureType FurnitureType;

        [SerializeField]
        [Tooltip("The 3D prefab instantiated")]
        public FurnitureGraphics FurniturePrefab;

        [SerializeField]
        [Tooltip("Furniture preview displayed during furniture seleciton")]
        public Sprite Thumbnail;

        [SerializeField]
        [Tooltip("How expensive is this furniture")]
        public float Price;
    }
}
