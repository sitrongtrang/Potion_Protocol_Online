using UnityEngine;

public class ItemConfig : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;
}