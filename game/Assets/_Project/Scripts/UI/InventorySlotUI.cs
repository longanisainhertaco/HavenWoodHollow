using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HavenwoodHollow.Inventory;

namespace HavenwoodHollow.UI
{
    /// <summary>
    /// UI element representing a single inventory slot.
    /// Displays the item icon, stack quantity, and selection highlight.
    /// Reference: Plan Section 9 Phase 2 - Inventory UI.
    /// </summary>
    public class InventorySlotUI : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Display")]
        [Tooltip("Image showing the item's icon sprite")]
        [SerializeField] private Image iconImage;
        [Tooltip("Text showing the current stack quantity")]
        [SerializeField] private TextMeshProUGUI quantityText;
        [Tooltip("Highlight overlay shown when this slot is selected")]
        [SerializeField] private GameObject selectedHighlight;

        #endregion

        #region Private Fields

        private int slotIndex;

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes this slot UI with its inventory index.
        /// </summary>
        public void Initialize(int index)
        {
            slotIndex = index;
            SetSelected(false);
        }

        /// <summary>
        /// Updates the icon and quantity text to reflect the given inventory slot data.
        /// </summary>
        public void UpdateDisplay(InventorySlot slot)
        {
            if (slot == null || slot.IsEmpty)
            {
                if (iconImage != null)
                {
                    iconImage.sprite = null;
                    iconImage.enabled = false;
                }

                if (quantityText != null)
                {
                    quantityText.text = string.Empty;
                }

                return;
            }

            if (iconImage != null)
            {
                iconImage.sprite = slot.ItemData.Icon;
                iconImage.enabled = slot.ItemData.Icon != null;
            }

            if (quantityText != null)
            {
                quantityText.text = slot.Quantity > 1 ? slot.Quantity.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Toggles the selection highlight on or off.
        /// </summary>
        public void SetSelected(bool selected)
        {
            if (selectedHighlight != null)
            {
                selectedHighlight.SetActive(selected);
            }
        }

        /// <summary>
        /// Called by the UI button's OnClick event.
        /// Tells the InventoryManager to select this slot.
        /// </summary>
        public void OnClick()
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.SelectSlot(slotIndex);
            }
        }

        #endregion
    }
}
