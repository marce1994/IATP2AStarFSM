﻿using System;
using System.Linq;
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
        set
        {
            try
            {
                if(value)
                    UIManager.Instance.FlaggedMines++; //TODO: que bestia :v

                var go = _gameObject.GetComponentsInChildren<SpriteRenderer>(includeInactive: true).Single(x => x.gameObject.name == "flag");
                go.gameObject.SetActive(value);
                flagged = value;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
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
        Flagged = false;
    }

    public bool CanMine()
    {
        return _content > 0;
    }

    public int CollectGold(int quantity)
    {
        int quantityToCollect;
        if (_content - quantity <= 0)
        {
            quantityToCollect = _content;
            _content = 0;
        }
        else
        {
            _content -= quantity;
            quantityToCollect = quantity;
        }

        if (_content == 0) {
            UIManager.Instance.FlaggedMines--; //TODO: que bestia :v
            GameManager.Instance.DeleteMine(this);
        }

        Debug.LogWarning($"collected: {quantityToCollect}/{quantity}");

        return quantityToCollect;
    }
}
