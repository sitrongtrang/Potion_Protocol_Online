using UnityEngine;

public class AABBCollider : MonoBehaviour
{
    public Vector2 BottomLeft;
    public Vector2 Size;

    public Rect Bounds => new Rect(BottomLeft, Size);

    public bool IsColliding(AABBCollider other)
    {
        return Bounds.Overlaps(other.Bounds);
    } 
}