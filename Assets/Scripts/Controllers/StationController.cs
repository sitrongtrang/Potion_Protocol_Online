using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StationController : MonoBehaviour
{
    private StationConfig _config;
    private List<RecipeConfig> _recipes;
    private List<ItemConfig> _items;

    private Coroutine _craftingCoroutine;

    public int StationId => _config.Id;

    public void Initialize(StationConfig config, List<RecipeConfig> recipes)
    {
        _config = config;
        _recipes = recipes;
        _items = new();
    }

    /// <summary>
    /// Client wants to add item to station — sends request to server.
    /// </summary>
    public void AddItem(ItemConfig item)
    {
        ServerInterface.Instance.SendAddItemToStation(_config.Id, item.Id);
    }

    /// <summary>
    /// Server confirmed item was added. (Optional prediction sync)
    /// </summary>
    public void OnItemAdded(ItemConfig item)
    {
        _items.Add(item);
    }

    /// <summary>
    /// Server confirmed crafting has started.
    /// </summary>
    public void OnCraftingStarted(string recipeId, float duration)
    {
        RecipeConfig recipe = _recipes.FirstOrDefault(r => r.Id == recipeId);
        if (recipe == null)
        {
            Debug.LogWarning($"Unknown recipe ID: {recipeId}");
            return;
        }

        if (_craftingCoroutine != null)
            StopCoroutine(_craftingCoroutine);

        _craftingCoroutine = StartCoroutine(CraftingVisual(recipe, duration));
    }

    /// <summary>
    /// Server confirmed crafting completed and sent the result.
    /// </summary>
    public void OnCraftingCompleted(string productId)
    {
        ItemConfig product = ItemDatabase.GetItem(productId);
        Vector2 dropPosition = (Vector2)transform.position + 0.5f * Vector2.down;

        _items.Clear();
        DropItem(product, dropPosition);
    }

    private IEnumerator CraftingVisual(RecipeConfig recipe, float duration)
    {
        // Optional client-side crafting visuals (progress bar, animation, etc.)
        yield return new WaitForSeconds(duration);
    }

    private void DropItem(ItemConfig item, Vector2 position)
    {
        ItemPool.Instance.SpawnItem(item, position);
    }
}
