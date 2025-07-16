// using System.Collections;
// using System.Collections.Generic;
// using UnityEditorInternal.Profiling.Memory.Experimental;
// using UnityEngine;

// public class StationController : MonoBehaviour
// {
//     private List<ItemConfig> _items;

//     public void Initialize()
//     {
//         _items.Clear();
//         NetworkEvents.OnMessageReceived += HandleNetworkMessage;
//     }

//     public void HandleNetworkMessage(ServerMessage message)
//     {
//         if (message.MessageType == NetworkMessageTypes.Station.Update)
//         {
//             StationUpdateMessage updateMessage = message as StationUpdateMessage;
//             if (updateMessage == null) return;
//             for (int i = 0; i < updateMessage.ItemIds.Length; i++)
//             {
//                 _items[i] = GetItemById(updateMessage.ItemIds[i]);
//             }
//             if (_items.Count > updateMessage.ItemIds.Length)
//                 _items.RemoveRange(updateMessage.ItemIds.Length, _items.Count - updateMessage.ItemIds.Length);
//         }
//         else if (message.MessageType == NetworkMessageTypes.Station.Craft)
//         {
//             StationCraftMessage craftMessage = message as StationCraftMessage;
//             if (craftMessage == null) return;
//             StartCoroutine(WaitForCraft(craftMessage.CraftTime));
//         }
//         else if (message.MessageType == NetworkMessageTypes.Item.Drop)
//         {
//             ItemDropMessage dropMessage = message as ItemDropMessage;
//             ItemConfig item = GetItemById(dropMessage.ItemId);
//             ItemPool.Instance.SpawnItem(item, dropMessage.Position);
//         }
//     }

//     private IEnumerator WaitForCraft(float craftTime)
//     {
//         // TODO: add progress bar
//         yield return new WaitForSeconds(craftTime);
//     }

//     private ItemConfig GetItemById(string id)
//     {
//         return null;
//     }
// }