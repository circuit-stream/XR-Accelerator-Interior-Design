using System.Collections.Generic;
using UnityEngine;

namespace XRAccelerator.Configs
{
    [CreateAssetMenu(fileName = "New Furniture Prefabs Generator", menuName = "Configs/Furniture Prefabs Generator")]
    public class FurnitureAssetsGenerator : ScriptableObject
    {
        [SerializeField]
        [Tooltip("TooltipText")]
        public string generatedPrefabsRelativePath;

        [SerializeField]
        [Tooltip("TooltipText")]
        public string generatedConfigsRelativePath;

        [SerializeField]
        [Tooltip("TooltipText")]
        public string thumbnailsRelativePath;

        [SerializeField]
        [Tooltip("TooltipText")]
        public List<GameObject> furnitureModels;

        [SerializeField]
        [Tooltip("TooltipText")]
        public GameObject furnitureBasePrefab;

        [SerializeField]
        [Tooltip("TooltipText")]
        public GameObject furniturePreviewBasePrefab;

        [SerializeField]
        [Tooltip("TooltipText")]
        public bool deleteExistingAssets;
    }
}