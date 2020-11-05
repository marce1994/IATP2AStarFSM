using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    private int gold = 0;
    private List<Mine> mines = new List<Mine>();

    public float instantiateMineTime = 10f;
    public float instantiateWorkerTime = 10f;
    public GameObject minePrefab;
    public GameObject workerPrefab;
    public GameObject explorerPrefab;
    public GameObject homePrefab;

    public GameObject nonWalkableTilePrefab;
    public GameObject walkableTilePrefab;

    public void AddGold(int gold)
    {
        this.gold += gold;
    }

    public Mine GetCollidedMine(GameObject collidedMine)
    {
        return mines.SingleOrDefault(x => x.gameObject == collidedMine);
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
        Instantiate(homePrefab);

        var walkable_nodes = PathFinderManager.Instance.Nodes.Where(x => x.walkable);
        var home_position = walkable_nodes.ElementAt(Random.Range(0, walkable_nodes.Count() - 1)).Position;
        homePrefab.transform.position = home_position;

        Debug.Log("InstantiateHome");
    }

    IEnumerator MineInstantiator(float time)
    {
        for (; ; )
        {
            Debug.Log("InstantiateMine");

            var walkableNodes = PathFinderManager.Instance.Nodes.Where(x => x.walkable == true);
            var randomPosition = Random.Range(0, walkableNodes.Count() - 1);
            var node = walkableNodes.ElementAt(randomPosition);

            if (mines.Any(x => x.Position == node.Position)) continue;

            mines.Add(new Mine(node.Position, minePrefab));
            yield return new WaitForSeconds(time);
        }
    }

    IEnumerator WorkerInstantiator(float time)
    {
        int workerQuantity = 0;

        for (; ; )
        {
            yield return new WaitForSeconds(time);

            if (gold < 10 && workerQuantity < 2)
                continue;
            workerQuantity++;
            Debug.Log("InstantiateWorker");
            Instantiate(workerPrefab);
            gold -= 10;
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