using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using XRAccelerator.Enums;
using XRAccelerator.Gameplay;

namespace XRAccelerator.Configs
{
    [CreateAssetMenu(fileName = "New Furniture Config", menuName = "Configs/Furniture")]
    public class FurnitureConfig : ScriptableObject
    {
        private static Dictionary<PlaneAlignment, FurnitureType> planeAlignmentToFurnitureType =
            new Dictionary<PlaneAlignment, FurnitureType>
            {
                {PlaneAlignment.Vertical, FurnitureType.Wall},
                {PlaneAlignment.HorizontalUp, FurnitureType.Floor},
                {PlaneAlignment.HorizontalDown, FurnitureType.Ceiling},
            };

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
        public SafeARSelectionInteractable furniturePreviewPrefab;

        [SerializeField]
        [Tooltip("Furniture preview displayed during furniture seleciton")]
        public Sprite Thumbnail;

        [SerializeField]
        [Tooltip("How expensive is this furniture")]
        public float Price;

        public bool CanFitInPlane(ARPlane arPlane)
        {
            return planeAlignmentToFurnitureType[arPlane.alignment] == FurnitureType;
        }
    }
}
