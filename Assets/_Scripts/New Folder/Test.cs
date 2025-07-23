using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public QuadTree quadTree;
    void Start()
    {
        quadTree = new QuadTree(5, new Rect(-10, -10, 20, 20));
        AABBCollider coll = new(
            new Vector2(-0.5f, -0.5f),
            new Vector2(1, 1)
        );
        coll.Layer = 1;
        coll.Mask.SetLayer(1);
        quadTree.Insert(coll);
    }

}

public static class CollisionSnaper
{
    public const int MAX_ITERATION = 10;
    public const float PRECISION = 0.001f;

    public static Vector2 SnapBack(Vector2 from, Vector2 to, AABBCollider collider, QuadTree collisionTree)
    {
        AABBCollider clone = new AABBCollider(collider); // Assumes copy constructor
        clone.SetBottomLeft(to);

        List<AABBCollider> collided = collisionTree.RetrieveCollided(clone, new List<AABBCollider>());
        if (collided.Count == 0)
        {
            return to;
        }

        float moveDistance = Vector2.Distance(from, to);
        int maxIterations = Mathf.Min(MAX_ITERATION, Mathf.CeilToInt(Mathf.Log(moveDistance / PRECISION, 2)));

        Vector2 start = from;
        Vector2 end = to;
        float precisionSqr = PRECISION * PRECISION;

        for (int i = 0; i < maxIterations; i++)
        {
            Vector2 mid = Vector2.Lerp(start, end, 0.5f);
            clone.SetBottomLeft(mid);

            collided = collisionTree.RetrieveCollided(clone, new List<AABBCollider>());
            if (collided.Count != 0)
            {
                end = mid;
            }
            else
            {
                start = mid;
            }

            if ((start - end).sqrMagnitude < precisionSqr)
            {
                break;
            }
        }

        return start;
    }
}
