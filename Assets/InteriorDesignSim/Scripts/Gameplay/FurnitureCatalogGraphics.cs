using System.Collections;
using System.Collections.Generic;
using Signals;
using UnityEngine;
using UnityEngine.UI.Extensions;
using XRAccelerator.Configs;
using XRAccelerator.Services;
using XRAccelerator.Signals;

namespace XRAccelerator.Gameplay
{
    public class FurnitureCatalogGraphics : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to the ScrollSnapBase component in the catalog scroll UI gameObject")]
        public ScrollSnapBase catalogScroll;

        [SerializeField]
        [Tooltip("TooltipText")]
        private RectTransform catalogEntriesHolder;

        [SerializeField]
        [Tooltip("TooltipText")]
        private FurnitureCatalogEntry catalogEntryPrefab;

        private List<FurnitureCatalogEntry> catalogEntries;
        private int selectedEntryIndex;
        private FurnitureCatalogEntry SelectedEntry => catalogEntries[selectedEntryIndex];

        private SignalDispatcher signalDispatcher;

        public bool IsSelectingFurniture { get; private set; }

        public FurnitureConfig GetSelectedFurniture()
        {
            return SelectedEntry.Config;
        }

        private void OnStartedSelectingFurniture()
        {
            SelectedEntry.DeHighlight();

            selectedEntryIndex = -1;
            IsSelectingFurniture = true;
            signalDispatcher.Dispatch<SelectingFurniture>();
        }

        private void OnChangedFurnitureSelection(int index)
        {
            if (selectedEntryIndex == index)
            {
                return;
            }

            selectedEntryIndex = index;
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

            catalogScroll.GetComponent<UI_InfiniteScroll>().Init();
            catalogScroll.enabled = true;

            // Wait ScrollSnapBase setup
            yield return null;

            catalogScroll.ChangePage(0);
        }

        private void Start()
        {
            StartCoroutine(SetupScroll());

            catalogScroll.OnSelectionChangeEndEvent.AddListener(OnChangedFurnitureSelection);
            catalogScroll.OnSelectionChangeStartEvent.AddListener(OnStartedSelectingFurniture);

            signalDispatcher = ServiceLocator.GetService<SignalDispatcher>();
        }
    }
}