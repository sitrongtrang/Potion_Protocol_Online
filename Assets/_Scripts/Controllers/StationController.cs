using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour
{
    public string StationId { get; private set; }
    private List<ItemConfig> _items;

    public void Initialize()
    {
        _items.Clear();
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
    }

    public void HandleNetworkMessage(ServerMessage message)
    {
        var result = message.MessageType switch
        {
            NetworkMessageTypes.Station.Update => HandleStationUpdate(message),
            NetworkMessageTypes.Station.Craft => HandleStationCraft(message),
            _ => null,
        };
    }

    private object HandleStationUpdate(ServerMessage message)
    {
        var updateMessage = (StationUpdateMessage)message;
        if (updateMessage == null) return null;

        if (_items.Count > updateMessage.ItemIds.Length)
            _items.RemoveRange(updateMessage.ItemIds.Length, _items.Count - updateMessage.ItemIds.Length);

        else
        {
            for (int i = _items.Count; i < updateMessage.ItemIds.Length; i++)
            {
                var itemConfig = ItemPool.Instance.GetItemById(updateMessage.ItemIds[i]).Config;
                if (itemConfig != null)
                {
                    _items.Add(itemConfig);
                }
            }
        }

        if (updateMessage.CraftSuccess)
        {
            var craftedItem = ItemPool.Instance.GetItemById(updateMessage.CraftedItemId);
            if (craftedItem != null)
            {
                ItemPool.Instance.SpawnItem(craftedItem.Config, updateMessage.DropPosition);
            }
        }

        return null;
    }

    private object HandleStationCraft(ServerMessage message)
    {
        var craftMessage = (StationCraftMessage)message;
        if (craftMessage == null) return null;

        StartCoroutine(WaitForCraft(craftMessage.CraftTime));
        return null;
    }

    private IEnumerator WaitForCraft(float craftTime)
    {
        // TODO: add progress bar
        yield return new WaitForSeconds(craftTime);
    }
}