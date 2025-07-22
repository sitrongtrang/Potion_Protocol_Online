using UnityEngine;

public class AABBCollider : MonoBehaviour
{
    private Vector2 _bottomLeft;
    private Vector2 _size;

    public MyLayerMask Mask;
    public int Layer;

    public Rect Bounds => new Rect(_bottomLeft, _size);

    public bool IsColliding(AABBCollider other)
    {
        return Bounds.Overlaps(other.Bounds);
    } 
}