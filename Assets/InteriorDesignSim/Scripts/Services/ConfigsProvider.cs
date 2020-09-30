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
        private Dictionary<FurnitureType, List<FurnitureConfig>> furnitureConfigsByType;
        private List<CatalogConfig> catalogConfigs;

        public ConfigsProvider()
        {
            InitializeVariables();
            FetchConfigs();
        }

        public List<FurnitureConfig> GetFurnituresOfType(FurnitureType furnitureType)
        {
            return furnitureConfigsByType[furnitureType];
        }

        private void InitializeVariables()
        {
            furnitureConfigsByType = new Dictionary<FurnitureType, List<FurnitureConfig>>();

            foreach (FurnitureType value in Enum.GetValues(typeof(FurnitureType)))
            {
                furnitureConfigsByType[value] = new List<FurnitureConfig>();
            }
        }

        private void FetchConfigs()
        {
            catalogConfigs = Resources.LoadAll<CatalogConfig>("CatalogConfigs").ToList();

            foreach (var catalogConfig in catalogConfigs)
            {
                foreach (var furnitureConfig in catalogConfig.FurnitureConfigs)
                {
                    furnitureConfigsByType[furnitureConfig.FurnitureType].Add(furnitureConfig);
                }
            }

            // TODO Arthur Optional: Use AssetBundles / Addressables
        }
    }
}