using UnityEngine;
using HavenwoodHollow.Inventory;

namespace HavenwoodHollow.UI
{
    /// <summary>
    /// Manages the inventory panel display, creating slot UI elements
    /// and refreshing them when inventory contents change.
    /// Reference: Plan Section 9 Phase 2 - Inventory UI.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Panel")]
        [Tooltip("The root inventory panel toggled on/off")]
        [SerializeField] private GameObject inventoryPanel;

        [Header("Slots")]
        [Tooltip("Parent transform where slot UI elements are instantiated")]
        [SerializeField] private Transform slotContainer;
        [Tooltip("Prefab for a single inventory slot UI element")]
        [SerializeField] private GameObject slotPrefab;

        [Header("Input")]
        [Tooltip("Key used to toggle the inventory panel")]
        [SerializeField] private KeyCode toggleKey = KeyCode.Tab;

        #endregion

        #region Private Fields

        private bool isOpen;
        private InventorySlotUI[] slotUIs;

        #endregion

        #region Properties

        /// <summary>Whether the inventory panel is currently open.</summary>
        public bool IsOpen => isOpen;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            CreateSlots();
            CloseInventory();
        }

        private void OnEnable()
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged += RefreshSlot;
            }
        }

        private void OnDisable()
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged -= RefreshSlot;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                ToggleInventory();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens the inventory panel and refreshes all slot displays.
        /// </summary>
        public void OpenInventory()
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(true);
            }

            isOpen = true;
            RefreshAll();
        }

        /// <summary>
        /// Closes the inventory panel.
        /// </summary>
        public void CloseInventory()
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            isOpen = false;
        }

        /// <summary>
        /// Toggles the inventory panel open or closed.
        /// </summary>
        public void ToggleInventory()
        {
            if (isOpen)
                CloseInventory();
            else
                OpenInventory();
        }

        /// <summary>
        /// Refreshes the display for a single slot by index.
        /// </summary>
        public void RefreshSlot(int index)
        {
            if (slotUIs == null || index < 0 || index >= slotUIs.Length)
                return;

            InventorySlot slot = InventoryManager.Instance != null
                ? InventoryManager.Instance.GetItem(index)
                : null;

            slotUIs[index].UpdateDisplay(slot);
        }

        /// <summary>
        /// Refreshes the display for all inventory slots.
        /// </summary>
        public void RefreshAll()
        {
            if (slotUIs == null || InventoryManager.Instance == null)
                return;

            for (int i = 0; i < slotUIs.Length; i++)
            {
                InventorySlot slot = InventoryManager.Instance.GetItem(i);
                slotUIs[i].UpdateDisplay(slot);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Instantiates slot UI elements to match the inventory size.
        /// </summary>
        private void CreateSlots()
        {
            if (InventoryManager.Instance == null || slotPrefab == null || slotContainer == null)
                return;

            int size = InventoryManager.Instance.InventorySize;
            slotUIs = new InventorySlotUI[size];

            for (int i = 0; i < size; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab, slotContainer);
                InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
                slotUI.Initialize(i);
                slotUIs[i] = slotUI;
            }
        }

        #endregion
    }
}
