using System;
using UnityEngine;

public class Mine
{
    private int _content { get; set; }
    private Vector3 _position { get; set; }
    private GameObject _gameObject { get; set; }
    private bool flagged { get; set; } = false;

    public bool Flagged
    {
        get { return flagged; }
        set { flagged = value; }
    }

    public GameObject gameObject
    {
        get
        {
            return _gameObject;
        }
    }

    public Vector3 Position
    {
        get
        {
            return _position;
        }
    }

    public Mine(Vector3 position, GameObject prefab)
    {
        _content = 35;
        _position = position;
        _gameObject = GameObject.Instantiate(prefab);
        _gameObject.transform.position = _position;
    }

    public bool CanMine()
    {
        return _content > 0;
    }

    public int CollectGold(int quantity)
    {
        int quantityToCollect = Math.Max(_content - quantity, 0);
        _content -= quantityToCollect;

        if (quantityToCollect == 0)
            GameManager.Instance.DeleteMine(this);

        return quantityToCollect;
    }
}
