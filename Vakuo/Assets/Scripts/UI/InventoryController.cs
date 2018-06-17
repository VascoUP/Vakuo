using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.UI;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour {

    #region references
    [SerializeField]
    private GameManager _gameManager;
    [Header("Inventory References")]
    // reference to the inventory displayed in the window
    public EntityInventory entityInventory;
    [Header("UI References")]
    // a panel that holds all the slots
    public GameObject slotPanel;
    [Header("Prefabs")]
    // a prefab of a slot
    public GameObject slotPrefab; 
    #endregion

    #region public
    public InventoryEvent onItemSelected;
    #endregion

    #region private 
    [SerializeField]
    private Button _backButton;
    // a flag that is set when the inventory needs refreshing that is reset each frame, we do this not to have more than 1 refresh each frame
    private bool updateInventory = false;
    // a list of all the slots in the container
    private List<ItemSlot> slots = new List<ItemSlot>(); 
    #endregion

    // Use this for initialization
    private void Start()
    {
        // populate the slots
        UpdateSlots();
        // the main event from the BasicContainer should trigger our redrawing of the inventory:
        entityInventory.mainContainer.OnInventoryChange.AddListener(delegate { UpdateInventory(); });
        // force refresh for the first time
        RefreshInventoryWindow();

        _backButton.onClick.AddListener(delegate {
            _gameManager.ChangeState(GameStatus.RUNNING);
        });
    }

    // Method for handling double clicking the item
    private void OnDoubleClickItem(ItemSlot itemSlot)
    {
        if (itemSlot.inventoryReference != null && itemSlot.inventoryItemReference != null)
        {
            if(onItemSelected != null)
            {
                onItemSelected(itemSlot.inventoryItemReference.Name);
            }
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
        }
        // Add listeners to slot events
        itemSlot.OnDoubleClickItem.AddListener(OnDoubleClickItem);
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
        lastVector.x = -24;
        lastVector.y = 40;
        lastVector.z = 0;

        for (int i = 0; i < entityInventory.mainContainer.slotAmount; i++)
        {
            Vector3 vector = new Vector3();
            if (i == 0)
            {
                vector.x = lastVector.x;
                vector.y = lastVector.y;
            }
            else if(i < 3 || (i > 3 && i < 6) || (i > 6 && i <9))
            {
                vector.x = lastVector.x + 40;
                vector.y = lastVector.y;
            }
            else if(i == 3 || i == 6)
            {
                vector.x = -24;
                vector.y = lastVector.y - 36;
            }
            vector.z = 0;

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
