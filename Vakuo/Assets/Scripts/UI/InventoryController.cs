using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.UI;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour {

    #region references
    [Header("Inventory References")]
    public EntityInventory entityInventory; // reference to the inventory displayed in the window
    [Header("UI References")]
    public GameObject slotPanel; // a panel that holds all the slots
    [Header("Prefabs")]
    public GameObject slotPrefab; // a prefab of a slot
    #endregion
    #region private 
    private bool updateInventory = false; // a flag that is set when the inventory needs refreshing that is reset each frame, we do this not to have more than 1 refresh each frame
    private List<ItemSlot> slots = new List<ItemSlot>(); // a list of all the slots in the container
    #endregion
    // Use this for initialization
    void Start()
    {
        // populate the slots
        UpdateSlots();
        // the main event from the BasicContainer should trigger our redrawing of the inventory:
        entityInventory.mainContainer.OnInventoryChange.AddListener(delegate { UpdateInventory(); });
        // force refresh for the first time
        RefreshInventoryWindow();
    }
    // Method for handling double clicking the item
    private void OnDoubleClickItem(ItemSlot itemSlot)
    {
        if (itemSlot.inventoryReference != null && itemSlot.inventoryItemReference != null)
        {
            Debug.Log("Double clicked: " + itemSlot.inventoryItemReference.Name);
        }
    }
    // Method for handling dragging events of the ItemSlot component
    private void OnMoveItemBetweenSlots(ItemSlot from, ItemSlot to)
    {
        int fromSlot = from.slotNumber;
        int toSlot = to.slotNumber;
        // Calling the basic container static method to handle the movement
        BasicContainer.MoveItemsBetweenSlots(fromSlot, toSlot, from.inventoryReference.mainContainer, to.inventoryReference.mainContainer);
    }
    // Method for handling dropping item outside the inventory
    private void OnDropItemOutside(ItemSlot itemSlot)
    {
        if (itemSlot.inventoryReference != null && itemSlot.inventoryItemReference != null)
        {
            Debug.Log("Drop item outside inventory: " + itemSlot.inventoryItemReference.Name);
        }
    }
    // This method applies a BasicItem entry from a container to an ItemSlot element of the ContainerWindow
    // Setting up all the game objects, texts, icons, etc, and its events.
    private void ApplyInventoryItemToSlot(ItemSlot itemSlot, BasicItem inventoryItem)
    {
        // Remove previous events
        itemSlot.OnDoubleClickItem.RemoveAllListeners();
        itemSlot.OnDropFromSlotToSlot.RemoveAllListeners();
        itemSlot.OnDropItemOutside.RemoveAllListeners();
        // Set up references to the inventory
        itemSlot.inventoryItemReference = inventoryItem;
        itemSlot.inventoryReference = entityInventory;
        // Load the icon from resources
        itemSlot.nameText.gameObject.SetActive(true);
        itemSlot.imageIcon.gameObject.SetActive(true);
        Sprite myFruit = Resources.Load<Sprite>("Items/" + inventoryItem.Graphic);
        if (myFruit != null)
        {
            itemSlot.imageIcon.sprite = myFruit;
        }
        else
        {
            Debug.Log("Cannot find the sprite for: " + inventoryItem.Graphic);
        }
        // Add listeners to slot events
        itemSlot.OnDoubleClickItem.AddListener(OnDoubleClickItem);
        itemSlot.OnDropFromSlotToSlot.AddListener(OnMoveItemBetweenSlots);
        itemSlot.OnDropItemOutside.AddListener(OnDropItemOutside);
    }
    // This method empties the slot of all references, events, and hides its unneeded game objects like amount text, icon etc.
    private void EmptySlot(ItemSlot itemSlot)
    {
        // Remove all events
        itemSlot.OnDoubleClickItem.RemoveAllListeners();
        itemSlot.OnDropFromSlotToSlot.RemoveAllListeners();
        itemSlot.OnDropItemOutside.RemoveAllListeners();
        // Clear references
        itemSlot.inventoryItemReference = null;
        itemSlot.inventoryReference = entityInventory;
        // Hide unneeded game objects
        itemSlot.nameText.gameObject.SetActive(false);
        itemSlot.imageIcon.gameObject.SetActive(false);
    }
    // This method refreshes the inventory window slots and sets them up with necessary references
    private void RefreshInventoryWindow()
    {
        if (slots.Count <= 0) return;
        for (int i = 0; i < entityInventory.mainContainer.slotAmount; i++) // itirate through all the slots
        {
            ItemSlot itemSlot = slots[i]; // get the slot
            BasicItem inventoryItem = entityInventory.mainContainer.GetItemOfSlotIndex(i); // get the item
            if (inventoryItem != null) // if the item exists fill the inventory slot
            {
                ApplyInventoryItemToSlot(itemSlot, inventoryItem);
            }
            else // if it doesnt empty the slot
            {
                EmptySlot(itemSlot);
            }
        }
    }

    // Method to poulate the slots
    void UpdateSlots()
    {
        foreach (Transform children in slotPanel.gameObject.transform)
        {
            Destroy(children.gameObject); // destroy all the preset slots
        }
        slots.Clear(); // clear the list
        AddSlots();
        UpdateInventory();
    }
    // A method called to set the updateInventory flag
    void UpdateInventory()
    {
        updateInventory = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (updateInventory) // if the flag is set, clear it and refresh the inventory, we do this to avoid more than 1 refresh per frame
        {
            RefreshInventoryWindow();
            updateInventory = false;
        }
    }

    void AddSlots()
    {
        Vector3 lastVector = new Vector3();
        lastVector.x = -400;
        lastVector.y = 100;
        lastVector.z = 0;

        for (int i = 0; i < entityInventory.mainContainer.slotAmount; i++)
        {
            Vector3 vector = new Vector3();
            if(i == 4)
            {
                vector.x = -230;
                vector.y = lastVector.y - 200;
            }
            else
            {
                vector.x = lastVector.x + 170;
                vector.y = lastVector.y;
            }
            vector.z = 0;

            Debug.Log(vector);
            // Instantiate all the slots and set their slot number inside the ItemSlot class
            RectTransform rt = slotPrefab.GetComponent<RectTransform>();
            rt.anchoredPosition3D = vector;
            lastVector = vector;
            GameObject slotObject = Instantiate(slotPrefab, slotPanel.gameObject.transform);
            ItemSlot itemSlot = slotObject.GetComponent<ItemSlot>();
            itemSlot.slotNumber = i;
            slots.Add(itemSlot);
        }
    }
}
