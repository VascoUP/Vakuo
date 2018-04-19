using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
using Assets.Scripts.UI;
// A generic type for an event that uses two ItemSlots as parameters
// If you don't know what these are, read this: https://docs.unity3d.com/ScriptReference/Events.UnityEvent_1.html
[System.Serializable]
public class DoubleItemSlotEvent : UnityEvent<ItemSlot, ItemSlot>
{
}
// Same as above but for a single parameter
[System.Serializable]
public class SingleItemSlotEvent : UnityEvent<ItemSlot>
{
}
// The main class that implements Pointer events, these are used for mouse actions, such as dragging, clicking, etc.
// Read more here: https://docs.unity3d.com/ScriptReference/EventSystems.IPointerEnterHandler.html
public class ItemSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region public
    [HideInInspector] public int slotNumber; // the index of the container slot this slot prefab holds, set by the ContainerWindow class
    #endregion
    #region events
    public SingleItemSlotEvent OnDoubleClickItem = new SingleItemSlotEvent(); // an event that is invoked when you double click on a slot
    public SingleItemSlotEvent OnDropItemOutside = new SingleItemSlotEvent(); // an event that is invoked when you drag the item outside a ContainerWindow
    public DoubleItemSlotEvent OnDropFromSlotToSlot = new DoubleItemSlotEvent(); // an event that is invoked when you drag the item between two slots
    #endregion
    #region references
    [Header("Inventory References")]
    public EntityInventory inventoryReference; // a reference to an EntityInventory MonoBehviour which holds the item, set by the ContainerWindow class
    public BasicItem inventoryItemReference; // a direct refernce to a BasicItem class that resides in this slot, null if slot is empty, this is set by the ContainerWindow class
    [Header("UI References")]
    public Image imageIcon; // the icon UI Image game object of the prefab
    public Text nameText; // the UI text that displays the amount of the item
    #endregion
    private static Vector2 offset; // these is used later do calculate the offset needed when dragging the item
    private Vector2 originalPos;
    //What should happen when somone clicks the item in the slot
    public void OnPointerClick(PointerEventData eventData)
    {
        //What happens on double click
        if (eventData.clickCount >= 2 && eventData.button == PointerEventData.InputButton.Left)
        {
            OnDoubleClickItem.Invoke(this);
            return;
        }
    }

    // BeginDrag interface
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventoryItemReference == null) return; // if an empty slot, no dragging
        originalPos = imageIcon.transform.position; // sets the offset and origional pos for dragging visual
        offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (inventoryItemReference == null) return; // if empty slot, no dragging
        imageIcon.transform.SetParent(gameObject.transform.parent.parent); // set the parent object so it appears above everything else
        imageIcon.transform.position = eventData.position - offset; // set the imageIcon pos so it appears next to the cursor
    }
    // Finish dragging
    public void OnEndDrag(PointerEventData eventData)
    {
        if (inventoryItemReference == null) return;  // if empty slot, no dragging
        List<RaycastResult> myResults = RaycastMouse(); // get the list of all the UI elements under the cursor
        bool wasOutsideOfUI = true; // have we found a UI element thats a slot or a container window?
        for (int i = 0; i < myResults.Count; i++) // go through the list of all object under the cursor
        {
            // get references to possible Container Winodws or Item Slots where we wish to drag the item
            InventoryController containerWindow = myResults[i].gameObject.GetComponent<InventoryController>();
            ItemSlot itemSlot = myResults[i].gameObject.GetComponent<ItemSlot>();
            if (containerWindow != null || itemSlot != null) // if there's a container or a slot under the cursor, don't drop the item
            {
                wasOutsideOfUI = false;
            }
            if (itemSlot != null) // we found a slot under the cursor, invoke the drop from slot to slot event, to which the ContainerWindow will subscribe
            {
                OnDropFromSlotToSlot.Invoke(this, itemSlot);
            }
        }
        if (wasOutsideOfUI) // if no ContainerWindow or ItemSlot components are found, it means we dragged the item outside the inventory
        {
            OnDropItemOutside.Invoke(this); // invoke the drop outside event
        }
        imageIcon.transform.SetParent(gameObject.transform); // return to the proper parent and position
        imageIcon.transform.position = originalPos;
    }
    // A static method that gets all the UI elements below the cursor, to see if we dragged to a slot, outside a window, etc.
    public static List<RaycastResult> RaycastMouse()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };
        pointerData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        return results;
    }
}