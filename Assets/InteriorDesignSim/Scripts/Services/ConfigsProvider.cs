using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XRAccelerator.Configs;
using XRAccelerator.Enums;

namespace XRAccelerator.Services
{
    public class ConfigsProvider
    {
        public readonly List<FurnitureConfig> allFurnitureConfigs;
        private readonly Dictionary<FurnitureType, List<FurnitureConfig>> furnitureConfigsByType;
        private List<CatalogConfig> catalogConfigs;

        public ConfigsProvider()
        {
            allFurnitureConfigs = new List<FurnitureConfig>();
            catalogConfigs = Resources.LoadAll<CatalogConfig>("CatalogConfigs").ToList();
            furnitureConfigsByType = new Dictionary<FurnitureType, List<FurnitureConfig>>();

            InitializeVariables();
            FetchConfigs();
        }

        public List<FurnitureConfig> GetFurnituresOfType(FurnitureType furnitureType)
        {
            return furnitureConfigsByType[furnitureType];
        }

        private void InitializeVariables()
        {
            foreach (FurnitureType value in Enum.GetValues(typeof(FurnitureType)))
            {
                furnitureConfigsByType[value] = new List<FurnitureConfig>();
            }
        }

        private void FetchConfigs()
        {
            foreach (var catalogConfig in catalogConfigs)
            {
                foreach (var furnitureConfig in catalogConfig.FurnitureConfigs)
                {
                    furnitureConfigsByType[furnitureConfig.FurnitureType].Add(furnitureConfig);
                    allFurnitureConfigs.Add(furnitureConfig);
                }
            }

            // TODO Arthur Optional: Use AssetBundles / Addressables
        }
    }
}