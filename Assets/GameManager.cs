using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public float instantiateMineTime = 10f;
    public float instantiateWorkerTime = 10f;

    private int gold = 0;

    List<Mine> mines = new List<Mine>();

    public GameObject minePrefab;
    public GameObject workerPrefab;
    public GameObject explorerPrefab;

    public GameObject nonWalkableTilePrefab;
    public GameObject walkableTilePrefab;

    public void AddGold(int gold)
    {
        this.gold += gold;
    }

    private void Awake()
    {
        StartCoroutine(InstantiateHome());
        StartCoroutine(MineInstantiator(instantiateMineTime));
        StartCoroutine(WorkerInstantiator(instantiateWorkerTime));
    }

    IEnumerator InstantiateHome()
    {
        yield return new WaitForSeconds(5f);
        Debug.Log("InstantiateHome");
    }

    IEnumerator MineInstantiator(float time)
    {
        for (; ; )
        {
            Debug.Log("InstantiateMine");

            var walkableNodes = PathFinderManager.Instance.Nodes.Where(x => x.walkable == true);
            var randomPosition = UnityEngine.Random.Range(0, walkableNodes.Count() - 1);
            var node = walkableNodes.ElementAt(randomPosition);

            if (mines.Any(x => x.Position == node.Position)) continue;

            mines.Add(new Mine(node.Position, minePrefab));
            yield return new WaitForSeconds(time);
        }
    }

    IEnumerator WorkerInstantiator(float time)
    {
        for (; ; )
        {
            yield return new WaitForSeconds(time);
            if (gold < 50) continue;
            Debug.Log("InstantiateWorker");
        }
    }

    public bool DeleteMine(Mine mine)
    {
        Destroy(mine.gameObject, 0.1f);
        return mines.Remove(mine);
    }

    public Mine GetRandomFlaggedMine()
    {
        return mines.Where(x => x.Flagged).ElementAt(UnityEngine.Random.Range(0, mines.Count() - 1));
    }

    private new void OnDestroy()
    {
        StopAllCoroutines();
        base.OnDestroy();
    }
}

