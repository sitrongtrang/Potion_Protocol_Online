using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{
    [SerializeField] private PlayerController _player;
    private ItemConfig[] items = new ItemConfig[GameConstants.MaxSlot];
    private int _choosingSlot = 0;
    public int ChoosingSlot
    {
        get => _choosingSlot;
        set
        {
            int oldSlot = _choosingSlot;
            _choosingSlot = value;
            if (oldSlot != value) OnSlotChanged?.Invoke();
        }
    }
    public event Action OnSlotChanged;
    private bool _isAutoFocus = true; // Auto focus mode, if true, will choose the current slot when adding item

    public void Initialize(PlayerController player)
    {
        _player = player;
        _isAutoFocus = true; // TODO: Make this configurable in the UI
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
    }

    public void ChooseSlot(int index)
    {
        if (index < 0 || index >= GameConstants.MaxSlot) return;
        ChoosingSlot = index;
        Debug.Log($"Slot {index + 1} chosen.");
    }

    public void NextSlot()
    {
        ChoosingSlot = (_choosingSlot + 1) % GameConstants.MaxSlot;
        Debug.Log($"Next slot chosen: {_choosingSlot + 1}");
    }

    public int GetAddSlot()
    {
        int idx = -1;
        // If choosing slot is empty, put the item into that slot; else put in the first slot that is empty
        if (items[_choosingSlot] == null) idx = _choosingSlot;
        else idx = FindEmptySlot();
        return idx;
    }

    public int GetRemoveSlot()
    {
        if (_choosingSlot < 0 || _choosingSlot >= GameConstants.MaxSlot || items[_choosingSlot] == null) return -1;
        return _choosingSlot;
    }

    public void HandleNetworkMessage(NetworkMessage message)
    {
        var result = message.MessageType switch
        {
            NetworkMessageTypes.Player.Inventory => HandlePlayerInventory(message),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public object HandlePlayerInventory(NetworkMessage message)
    {
        var inventoryMessage = (PlayerInventoryMessage)message;
        if (inventoryMessage.PlayerId != _player.PlayerId) return null;

        if (inventoryMessage.SlotIndex < 0 || inventoryMessage.SlotIndex >= GameConstants.MaxSlot)
        {
            Debug.LogWarning("Invalid slot index in inventory message.");
            return null;
        }

        object item = null;
        switch (inventoryMessage.AcTionType)
        {
            case "Pickup":
                item = AddItem(inventoryMessage.ItemId, inventoryMessage.SlotIndex);
                if (item != null && item is ItemController icPickup)
                {
                    ItemPool.Instance.RemoveItem(icPickup);
                }
                break;
            case "Drop":
                item = RemoveItem(inventoryMessage.ItemId, inventoryMessage.SlotIndex);
                if (item is not null and ItemController icDrop)
                {
                    ItemPool.Instance.SpawnItem(icDrop.Config, inventoryMessage.DropPosition);
                }
                break;
            case "Transfer":
            case "Submit":
                item = RemoveItem(inventoryMessage.ItemId, inventoryMessage.SlotIndex);
                break;
            default:
                Debug.LogWarning($"Unknown action type: {inventoryMessage.AcTionType}");
                break;
        }

        return item;
    }

    private object AddItem(string itemId, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= GameConstants.MaxSlot)
        {
            Debug.LogWarning("Invalid slot index to add.");
            return null;
        }

        // Simulate item add
        ItemController itemToAdd = ItemPool.Instance.GetItemById(itemId); // TODO: find a way to get item by ID
        if (itemToAdd == null)
        {
            Debug.LogWarning($"Item {itemId} not found.");
            return null;
        }

        items[slotIndex] = itemToAdd.Config;
        if (_isAutoFocus) ChoosingSlot = slotIndex;
        Debug.Log($"Added up item {itemId} into slot {slotIndex + 1}");
        return itemToAdd;
    }

    private object RemoveItem(string itemId, int slotIndex)
    {
        // int idx = Array.FindIndex(items, item => item != null && item.Id == itemId);
        // if (idx < 0)
        // {
        //     Debug.LogWarning($"Item {itemId} not found in inventory.");
        //     return null;
        // }
        if (slotIndex < 0 || slotIndex >= GameConstants.MaxSlot || items[slotIndex] == null)
        {
            Debug.LogWarning("Invalid slot index for drop.");
            return null;
        }

        ItemController itemToRemove = ItemPool.Instance.GetItemById(itemId);
        if (itemToRemove == null)
        {
            Debug.LogWarning($"Item {itemId} not found.");
            return null;
        }

        // Simulate removing the item
        items[slotIndex] = null; // Remove the item from inventory
        Debug.Log($"Removed up item {itemId} in slot {slotIndex + 1}");
        return itemToRemove;
    }

    private int FindEmptySlot()
    {
        // Find the first empty slot
        for (int i = 0; i < GameConstants.MaxSlot; i++)
        {
            if (items[i] == null) return i;
        }

        return -1;
    } 
}