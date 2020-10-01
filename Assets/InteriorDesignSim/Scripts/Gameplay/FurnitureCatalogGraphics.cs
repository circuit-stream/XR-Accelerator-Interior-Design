using System;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace XRAccelerator.Gameplay
{
    public class FurnitureCatalogGraphics : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to the ScrollSnapBase component in the catalog scroll UI gameObject")]
        public ScrollSnapBase catalogScroll;

        private void OnStartedSelectingFurniture()
        {
            // TODO Arthur: Disable model preview / placement
            throw new NotImplementedException();
        }

        private void OnChangedFurnitureSelection(int index)
        {
            // TODO Arthur: Reenable model preview / placement
            throw new NotImplementedException();
        }

        private void Start()
        {
            // TODO Arthur: Get furniture configs and instantiate FurnitureCatalogEntries

            catalogScroll.OnSelectionChangeEndEvent.AddListener(OnChangedFurnitureSelection);
            catalogScroll.OnSelectionChangeStartEvent.AddListener(OnStartedSelectingFurniture);
        }
    }
}