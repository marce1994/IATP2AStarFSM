using System.Linq;
using UnityEngine;

public class MineDetector : MonoBehaviour
{
    private Mine _collidedMine;
    public bool shouldBeFlagged;

    public bool Collided
    {
        get
        {
            return _collidedMine != null;
        }
    }

    public Mine Mine
    {
        get
        {
            return _collidedMine != null && _collidedMine.gameObject != null ? _collidedMine : null;
        }
    }

    public void ResetDetector()
    {
        _collidedMine = null;
    }

    PolygonCollider2D polygonCollider2D;
    Vector2[] worldSpaceColliderPoints;

    PathWalker pathWalker;
    private void Update()
    {
        if (pathWalker == null)
            pathWalker = GetComponentInParent<PathWalker>();
        
        Vector3 relative = transform.InverseTransformPoint(pathWalker.NextPosition);
        float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg - 90;
        transform.Rotate(0, 0, -angle);
    }

    private void OnDrawGizmos()
    {
        if (polygonCollider2D == null)
            polygonCollider2D = GetComponent<PolygonCollider2D>();
        
        worldSpaceColliderPoints = polygonCollider2D.points.Select(x => (Vector2)transform.TransformPoint(x)).ToArray();

        for (int i = 0; i < worldSpaceColliderPoints.Length; i++)
        {
            int origin = i;
            int dest = (i + 1) % worldSpaceColliderPoints.Length;

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(worldSpaceColliderPoints[origin],  worldSpaceColliderPoints[dest]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_collidedMine != null) return;

        if (collision.gameObject.CompareTag("Mine"))
        {
            Mine mine = GameManager.Instance.GetCollidedMine(collision.gameObject);
            if (mine.Flagged == shouldBeFlagged)
                _collidedMine = mine;
        }
    }
}
