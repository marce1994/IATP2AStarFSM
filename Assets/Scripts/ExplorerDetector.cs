using System;
using UnityEngine;

public class ExplorerDetector : MonoBehaviour
{
    private Mine _collidedMine;

    public Mine CollidedMine
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Mine")
        {
            _collidedMine = GameManager.Instance.GetCollidedMine(collision.gameObject);
        }
    }
}
