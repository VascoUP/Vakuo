using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Assets.Scripts.UI
{
    public class BasicContainer
    {
        #region events
        public UnityEvent OnInventoryChange = new UnityEvent(); // an event that is invoked whenever the container changes (added, deleted items, etc.)
        #endregion
        #region private
        public Dictionary<int, BasicItem> items = new Dictionary<int, BasicItem>(); // collection of all items present in the container, the key being the slot index
        public int slotAmount = 9; // total number of slots in the container
        #endregion
        #region helperfunctions
        // Finds the first empty Slot and returns its index, or -1 when there is no free slot
        public int FindEmptySlot()
        {
            for (int i = 0; i < slotAmount; i++)
            {
                if (!items.ContainsKey(i))
                {
                    return i;
                }
            }
            return -1;
        }
        //Finds the first item of a given name, or returns null if there is no such item
        public BasicItem FindFirstItemOfName(string itemName)
        {
            BasicItem item = new BasicItem();
            foreach (KeyValuePair<int, BasicItem> entry in items)
            {
                if (entry.Value.Name == itemName)
                {
                    item = entry.Value;
                    return item;
                }
            }
            return item;
        }
        // Returns the item of a given slot index, or null if such item doesn't exist
        public BasicItem GetItemOfSlotIndex(int slotIndex)
        {
            BasicItem item = null;
            if (items.TryGetValue(slotIndex, out item))
            {
                return item;
            }
            return item;
        }
        // Clears the slot and its entry in the dictionary
        public void ClearItemOfSlot(int slotIndex)
        {
            BasicItem item = null;
            if (items.TryGetValue(slotIndex, out item))
            {
                items.Remove(slotIndex);
                item = null;
                OnInventoryChange.Invoke();
            }
        }
        #endregion
        #region inventory manipulation
        // Through this method, whenever an item properties change the container invokes the OnInvenotryChange event for its clients
        private void HookUpItemEventsToContainer(BasicItem item)
        {
            // Remove previous listeners if they exists
            item.OnNameChange.RemoveAllListeners();
            item.OnGraphicChange.RemoveAllListeners();
            item.OnNameChange.AddListener(delegate { OnInventoryChange.Invoke(); });
            item.OnGraphicChange.AddListener(delegate { OnInventoryChange.Invoke(); });
        }
        // Adds an item as a single stack, and returns true if it was successful
        public bool AddItemAsASingleStack(BasicItem item)
        {
            int iEmptySlot = FindEmptySlot();
            if (iEmptySlot == -1) return false; // if there are no free slots, do nothing
            else
            {
                HookUpItemEventsToContainer(item);
                items.Add(iEmptySlot, item);
                OnInventoryChange.Invoke();
                return true;
            }
        }
        #endregion
    }
}
