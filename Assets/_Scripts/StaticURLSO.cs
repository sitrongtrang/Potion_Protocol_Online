using UnityEngine;

[CreateAssetMenu(fileName = "StaticURL", menuName = "Scriptable Objects/StaticURL")]
public class StaticURLSO : ScriptableObject
{
    [SerializeField] private string _staticURL;
    public string StaticURL => _staticURL;
}
