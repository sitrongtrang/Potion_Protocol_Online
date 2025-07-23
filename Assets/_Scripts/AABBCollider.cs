using UnityEngine;

public class AABBCollider
{
    private Vector2 _bottomLeft;
    private Vector2 _size;

    public MyLayerMask Mask = new MyLayerMask();
    public int Layer;

    public AABBCollider(Vector2 bottomLeft, Vector2 size)
    {
        _bottomLeft = bottomLeft;
        _size = size; 
    }
    public AABBCollider(AABBCollider other)
    {
        _bottomLeft = other._bottomLeft;
        _size = other._size;
        Layer = other.Layer;
        Mask = other.Mask;
        
    }
    public Rect Bounds => new Rect(_bottomLeft, _size);

    public bool IsColliding(AABBCollider other)
    {
        return Bounds.Overlaps(other.Bounds);
    }
    public void SetBottomLeft(Vector2 newBottomLeft)
    {
        _bottomLeft = newBottomLeft;
    }
}