using System.Collections.Generic;
using UnityEngine;

namespace XRAccelerator.Configs
{
    public class CatalogConfig : ScriptableObject
    {
        [SerializeField]
        [Tooltip("This catalog unique identifier")]
        public string Id;

        [SerializeField]
        [Tooltip("Brand UI Icon")]
        public Sprite brandIcon;

        [SerializeField]
        [Tooltip("All available furnitures from this catalog")]
        public List<FurnitureConfig> FurnitureConfigs;
    }
}