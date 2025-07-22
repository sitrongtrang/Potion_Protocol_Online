using UnityEngine;

public class MyLayerMask
{
    private int _mask = 0;

    public MyLayerMask(int mask = 0)
    {
        _mask = mask;
    }

    public void SetLayer(int layer)
    {
        _mask |= (1 << layer);
    }

    public void ClearLayer(int layer)
    {
        _mask &= ~(1 << layer);
    }

    public bool Contains(int layer)
    {
        return (_mask & (1 << layer)) != 0;
    }

    public int Mask
    {
        get { return _mask; }
        set { _mask = value; }
    }
}