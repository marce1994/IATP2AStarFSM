﻿using Assets.Scripts.AStar;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    private int _gold = 0;

    private int Gold
    {
        get { return _gold; }
        set
        {
            _gold = value;
            UIManager.Instance.GoldCollected = _gold;
        }
    }

    private List<Mine> mines = new List<Mine>();

    public float instantiateMineTime = 10f;
    public float instantiateWorkerTime = 10f;
    public GameObject minePrefab;
    public GameObject workerPrefab;
    public GameObject explorerPrefab;
    public GameObject homePrefab;

    public GameObject nonWalkableTilePrefab;
    public GameObject walkableTilePrefab;

    public static int WORKER_COST = 10;
    public static int EXPLORER_COST = 15;

    private void Start()
    {
        Gold = 0;
    }

    public void AddGold(int gold)
    {
        this.Gold += gold;
    }

    internal void BuyWorker()
    {
        Gold -= WORKER_COST;
        Instantiate(workerPrefab);
        UIManager.Instance.WorkerCount++;
    }

    internal void BuyExplorer()
    {
        Gold -= EXPLORER_COST;
        Instantiate(explorerPrefab);
        UIManager.Instance.ExplorerCount++;
    }

    public Mine GetCollidedMine(GameObject collidedMine)
    {
        return mines.SingleOrDefault(x => x.gameObject == collidedMine);
    }

    private void Awake()
    {
        StartCoroutine(InstantiateHome());
        StartCoroutine(MineInstantiator(instantiateMineTime));
    }

    IEnumerator InstantiateHome()
    {
        yield return new WaitForSeconds(5f);
        Instantiate(homePrefab);
        homePrefab.transform.position = PathFinderManager.Instance.RandomWalkablePosition;

        Debug.Log("InstantiateHome");
    }

    IEnumerator MineInstantiator(float time)
    {
        for (; ; )
        {
            Debug.Log("InstantiateMine");

            IEnumerable<PathNode> walkableNodes = PathFinderManager.Instance.Nodes.Where(x => x.walkable == true);
            PathNode node = walkableNodes.ElementAt(Random.Range(0, walkableNodes.Count() - 1));

            if (mines.Any(x => x.Position.Equals(node.Position))) continue;

            mines.Add(new Mine(node.Position, minePrefab));
            yield return new WaitForSeconds(time);
        }
    }

    public bool DeleteMine(Mine mine)
    {
        Destroy(mine.gameObject);
        return mines.Remove(mine);
    }

    public Mine GetRandomFlaggedMine()
    {
        return mines
            .Where(x => x.Flagged)
            .ElementAtOrDefault(Random.Range(0, mines.Count() - 1));
    }

    public GameObject GetBase()
    {
        var bases = GameObject.FindGameObjectsWithTag("Base");
        return bases.First();
    }

    private new void OnDestroy()
    {
        StopAllCoroutines();
        base.OnDestroy();
    }
}