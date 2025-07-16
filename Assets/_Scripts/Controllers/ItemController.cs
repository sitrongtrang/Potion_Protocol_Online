using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] public ItemConfig Config { get; private set; }

    public string ItemId { get; private set; }
}