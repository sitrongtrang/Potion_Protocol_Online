using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    [SerializeField] private Test _test;
    private Vector2 _moveDir;
    private AABBCollider _collider = new(new(-0.5f, -0.5f), new(1f, 1f));
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _collider.SetBottomLeft(transform.position + new Vector3(-0.5f, -0.5f, 0));
        _collider.Layer = 1;
        _collider.Mask.SetLayer(1);
    }

    // Update is called once per frame
    void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        _moveDir = new Vector2(horizontal, vertical).normalized;
        _collider.SetBottomLeft(transform.position + new Vector3(-0.5f, -0.5f, 0));
    }

    void FixedUpdate()
    {
        transform.position = CollisionSnaper.SnapBack(
            transform.position,
            transform.position + 5 * Time.fixedDeltaTime * (Vector3)_moveDir,
            _collider,
            _test.quadTree
        );
    }
}
