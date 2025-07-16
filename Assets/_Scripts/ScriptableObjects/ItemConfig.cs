using UnityEngine;

public class ItemConfig : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;
    
    [SerializeField] private Sprite _icon; 
    public Sprite Icon => _icon;
}