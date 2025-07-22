using System.Collections.Generic;
using UnityEngine;

public class QuadTree
{
    private const int MAX_OBJECTS = 16;
    private const int MAX_LEVELS = 5;
    private int _level;
    private List<AABBCollider> _colliders;
    private Rect _bounds;
    private QuadTree[] _nodes;

    public QuadTree(int level, Rect bounds)
    {
        _level = level;
        _colliders = new List<AABBCollider>();
        _bounds = bounds;
        _nodes = new QuadTree[4];
    }

    public void Clear()
    {
        _colliders.Clear();
        for (int i = 0; i < _nodes.Length; i++)
        {
            if (_nodes[i] != null)
            {
                _nodes[i].Clear();
                _nodes[i] = null;
            }
        }
    }

    private void Split()
    {
        float subWidth = _bounds.width / 2;
        float subHeight = _bounds.height / 2;
        float x = _bounds.x;
        float y = _bounds.y;

        _nodes[0] = new QuadTree(_level + 1, new Rect(x + subWidth, y, subWidth, subHeight)); // Top Right
        _nodes[1] = new QuadTree(_level + 1, new Rect(x, y, subWidth, subHeight)); // Top Left
        _nodes[2] = new QuadTree(_level + 1, new Rect(x, y + subHeight, subWidth, subHeight)); // Bottom Left
        _nodes[3] = new QuadTree(_level + 1, new Rect(x + subWidth, y + subHeight, subWidth, subHeight)); // Bottom Right
    }

    public void Insert(AABBCollider collider)
    {
        if (_nodes[0] != null)
        {
            int index = GetIndex(collider.Bounds);
            if (index != -1)
            {
                _nodes[index].Insert(collider);
                return;
            }
        }

        _colliders.Add(collider);

        if (_colliders.Count > MAX_OBJECTS && _level < MAX_LEVELS)
        {
            if (_nodes[0] == null)
            {
                Split();
            }

            for (int i = 0; i < _colliders.Count; i++)
            {
                int index = GetIndex(_colliders[i].Bounds);
                if (index != -1)
                {
                    _nodes[index].Insert(_colliders[i]);
                    _colliders.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    public void Remove(AABBCollider collider)
    {
        if (_colliders.Contains(collider))
        {
            _colliders.Remove(collider);
            return;
        }

        if (_nodes[0] != null)
        {
            int index = GetIndex(collider.Bounds);
            if (index != -1)
            {
                _nodes[index].Remove(collider);
            }
        }
    }

    private int GetIndex(Rect rect)
    {
        if (!IsColliding(rect))
        {
            return -1;
        }

        float midX = _bounds.x + _bounds.width / 2;
        float midY = _bounds.y + _bounds.height / 2;

        bool topQuadrant = rect.yMax <= midY;
        bool bottomQuadrant = rect.yMin > midY;
        bool leftQuadrant = rect.xMax <= midX;
        bool rightQuadrant = rect.xMin > midX;

        if (topQuadrant)
        {
            if (leftQuadrant) return 1; // Top Left
            else if (rightQuadrant) return 0; // Top Right
        }
        else if (bottomQuadrant)
        {
            if (leftQuadrant) return 2; // Bottom Left
            else if (rightQuadrant) return 3; // Bottom Right
        }

        return -1; // Not in any quadrant
    }

    private bool IsColliding(Rect rect)
    {
        return _bounds.Overlaps(rect);
    }
    
    public List<AABBCollider> RetrieveCollided(AABBCollider collider, List<AABBCollider> returnColliders)
    {
        if (!IsColliding(collider.Bounds))
        {
            return returnColliders;
        }

        for (int i = 0; i < _colliders.Count; i++)
        {
            if (collider.Bounds.Overlaps(_colliders[i].Bounds) && collider.Mask.Contains(_colliders[i].Layer))
            {
                returnColliders.Add(_colliders[i]);
            }
        }

        if (_nodes[0] != null)
        {
            for (int i = 0; i < _nodes.Length; i++)
            {
                _nodes[i].RetrieveCollided(collider, returnColliders);
            }
        }

        return returnColliders;
    }
}