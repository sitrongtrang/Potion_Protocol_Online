using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    private Queue<ItemController> _activeItems = new Queue<ItemController>();
    private Dictionary<string, Queue<ItemController>> _pooledObjects = new Dictionary<string, Queue<ItemController>>();
    public static ItemPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public ItemController SpawnItem(ItemConfig config, Vector2 position)
    {
        // Exceed cap
        if (_activeItems.Count >= GameConstants.MaxItems)
        {
            ItemController oldest = _activeItems.Dequeue();
            ReturnToPool(oldest);
        }

        // Get from pool or instantiate
        ItemController item = GetFromPool(config);
        item.transform.position = position;
        item.transform.rotation = Quaternion.identity;
        item.gameObject.SetActive(true);

        _activeItems.Enqueue(item);
        return item;
    }

    public void RemoveItem(ItemController item)
    {
        // Remove from active queue
        Queue<ItemController> newQueue = new Queue<ItemController>();
        foreach (var it in _activeItems)
        {
            if (it != item)
                newQueue.Enqueue(it);
        }
        _activeItems = newQueue;

        ReturnToPool(item);
    }

    private void ReturnToPool(ItemController item)
    {
        item.gameObject.SetActive(false);

        // Return item to the appropriate pool
        string name = item.Config.Name;
        if (!_pooledObjects.ContainsKey(name))
            _pooledObjects[name] = new Queue<ItemController>();

        _pooledObjects[name].Enqueue(item);
    }

    private ItemController GetFromPool(ItemConfig config)
    {
        string name = config.Name;

        // Retrieve an item from pool or instantiate a new one
        if (_pooledObjects.ContainsKey(name) && _pooledObjects[name].Count > 0)
        {
            return _pooledObjects[name].Dequeue();
        }

        // No available object in pool, instantiate a new one
        ItemController newObj = Instantiate(config.Prefab);
        return newObj;
    }
}