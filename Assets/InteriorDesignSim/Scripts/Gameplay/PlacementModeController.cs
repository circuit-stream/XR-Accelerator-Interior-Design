using System.Collections;
using System.Collections.Generic;
using DanielLochner.Assets.SimpleScrollSnap;
using Signals;
using UnityEngine;
using XRAccelerator.Configs;
using XRAccelerator.Enums;
using XRAccelerator.Services;
using XRAccelerator.Signals;

namespace XRAccelerator.Gameplay
{
    public class PlacementModeController : ModeController
    {
        [Header("Placement Mode Specific")]
        [SerializeField]
        [Tooltip("Reference to the ScrollSnapBase component in the catalog scroll UI gameObject")]
        public SimpleScrollSnap catalogScroll;

        [SerializeField]
        [Tooltip("Reference to the parent of the dynamically instantiated catalogEntries")]
        private RectTransform catalogEntriesHolder;

        [SerializeField]
        [Tooltip("Reference to the placementReticle component")]
        private PlacementReticle placementReticle;

        [SerializeField]
        [Tooltip("Catalog entry prefab reference")]
        private FurnitureCatalogEntry catalogEntryPrefab;

        private List<FurnitureCatalogEntry> catalogEntries;
        private int selectedEntryIndex;
        private FurnitureCatalogEntry SelectedEntry => catalogEntries[selectedEntryIndex];
        private bool IsValidSelectedEntry => selectedEntryIndex != -1;

        private SignalDispatcher signalDispatcher;

        public bool IsSelectingFurniture { get; private set; }

        public override Mode Mode => Mode.Placement;

        public FurnitureConfig GetSelectedFurniture()
        {
            return SelectedEntry.Config;
        }

        public override void EnableMode()
        {
            placementReticle.Enable();

            base.EnableMode();
        }

        public override void DisableMode()
        {
            placementReticle.Disable();

            base.DisableMode();
        }

        private void OnStartedSelectingFurniture()
        {
            if (IsValidSelectedEntry)
            {
                SelectedEntry.DeHighlight();
            }

            selectedEntryIndex = -1;
            IsSelectingFurniture = true;
            signalDispatcher.Dispatch<SelectingFurniture>();
        }

        private void OnChangedFurnitureSelection()
        {
            if (selectedEntryIndex == catalogScroll.CurrentPanel)
            {
                return;
            }

            selectedEntryIndex = catalogScroll.CurrentPanel;
            SelectedEntry.Highlight();

            IsSelectingFurniture = false;
            signalDispatcher.Dispatch(new FurnitureSelected {Config = GetSelectedFurniture()});
        }

        private void CreateCatalogEntries(List<FurnitureConfig> configs)
        {
            catalogEntries = new List<FurnitureCatalogEntry>(configs.Count);

            foreach (var furnitureConfig in configs)
            {
                var newEntry = Instantiate(catalogEntryPrefab, catalogEntriesHolder);
                newEntry.Setup(furnitureConfig);
                catalogEntries.Add(newEntry);
            }
        }

        private IEnumerator SetupScroll()
        {
            Debug.Assert(!catalogScroll.enabled, "Catalog scroll component should start disabled", gameObject);
            Debug.Assert(catalogEntriesHolder.childCount == 0, "Catalog should start with no children", gameObject);

            CreateCatalogEntries(ServiceLocator.GetService<ConfigsProvider>().allFurnitureConfigs);

            // Wait for scrollRect to reposition children
            yield return null;

            catalogScroll.enabled = true;

            // Wait SimpleScrollSnap setup
            yield return null;

            selectedEntryIndex = -1;
            OnChangedFurnitureSelection();
        }

        private void Start()
        {
            StartCoroutine(SetupScroll());

            catalogScroll.onPanelChanged.AddListener(OnChangedFurnitureSelection);
            catalogScroll.onPanelSelecting.AddListener(OnStartedSelectingFurniture);

            signalDispatcher = ServiceLocator.GetService<SignalDispatcher>();
        }
    }
}