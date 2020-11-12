using Assets.Scripts.AStar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathFinderManager : Singleton<PathFinderManager>
{
    private Dictionary<Tuple<Vector3, Vector3>, Vector3[]> oldUsedPaths = new Dictionary<Tuple<Vector3, Vector3>, Vector3[]>();
    private PathNode[,] nodes;
    private float[,] noise;
    private int rescued = 0;

    public float wakableWeight = 0.3f;
    public int sizeX, sizeZ = 10;

    public List<PathNode> Nodes
    {
        get
        {
            return nodes.Cast<PathNode>().ToList();
        }
    }

    public Vector3 RandomWalkablePosition
    {
        get
        {
            var walkable_nodes = nodes.Cast<PathNode>().Where(x => x.walkable);
            var random_node = walkable_nodes.ElementAt(UnityEngine.Random.Range(0, walkable_nodes.Count() - 1));
            return random_node.Position;
        }
    }


    private Vector3[] RescueOldCalculation(Vector3 origin, Vector3 destiny)
    {
        var key = new Tuple<Vector3, Vector3>(origin, destiny);
        var keyReversed = new Tuple<Vector3, Vector3>(destiny, origin);

        if (oldUsedPaths.ContainsKey(key))
            return oldUsedPaths[key];
        if (oldUsedPaths.ContainsKey(keyReversed))
            return oldUsedPaths[keyReversed].Reverse().ToArray();

        return null;
    }

    public Task<Vector3[]> FindRandomPath(Vector3 origin)
    {
        PathNode[,] nodes_clone = (PathNode[,])nodes.Clone();
        var listNodes = nodes_clone.Cast<PathNode>().Where(x => x.walkable).ToList();
        return FindPath(origin, listNodes.ElementAt(UnityEngine.Random.Range(0, listNodes.Count - 1)).Position);
    }

    public Task<Vector3[]> FindPath(Vector3 origin, Vector3 destiny)
    {
        PathNode[,] nodes_clone = (PathNode[,])nodes.Clone();
        List<PathNode> listNodes = nodes_clone.Cast<PathNode>().Where(x => x.walkable).ToList();

        PathNode origNode = GetClosest(origin, listNodes);
        PathNode destNode = GetClosest(destiny, listNodes);

        var rescuedPath = RescueOldCalculation(origin, destiny);

        if (rescuedPath != null)
        {
            rescued++;
            UIManager.Instance.SetOtherDataText($"Path cache lenght: {oldUsedPaths.Count} / reused: {rescued} | Press R to clean the cache | Space to restart");
            return Task.FromResult(rescuedPath);
        }

        List<PathNode> path = AStar.FindPath(nodes_clone, origNode, destNode);

        var key = new Tuple<Vector3, Vector3>(origin, destiny);
        if (!oldUsedPaths.ContainsKey(key))
        {
            oldUsedPaths.Add(key, path.Select(x => x.Position).ToArray());
            UIManager.Instance.SetOtherDataText($"Path cache lenght: {oldUsedPaths.Count} / reused: {rescued} | Press R to clean the cache | Space to restart");
        }

        if (path == null)
            return Task.FromResult<Vector3[]>(null);

        return Task.FromResult(path.Select(x => x.Position).ToArray());
    }

    private PathNode GetClosest(Vector3 startPosition, List<PathNode> allNodes)
    {
        PathNode bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (PathNode potentialTarget in allNodes)
        {
            Vector3 directionToTarget = potentialTarget.Position - startPosition;

            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            oldUsedPaths = new Dictionary<Tuple<Vector3, Vector3>, Vector3[]>();
            UIManager.Instance.SetOtherDataText($"Path cache lenght: {oldUsedPaths.Count} / reused: {rescued} | Press R to clean the cache | Space to restart");
        }
    }

    private void Awake()
    {
        nodes = new PathNode[sizeX, sizeZ];

        NoiseGenerator noiseGenerator = new NoiseGenerator(sizeX, sizeZ);

        noiseGenerator.offsetX = UnityEngine.Random.Range(100, 99999);
        noiseGenerator.offsetY = UnityEngine.Random.Range(100, 99999);
        noiseGenerator.scale = 5f;

        noise = noiseGenerator.GetNoise();

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                nodes[i, j] = new PathNode(position: new Vector3((-sizeX / 2) + i, (-sizeZ / 2 + j), 0));
                nodes[i, j].walkable = noise[i, j] > wakableWeight;
                GameObject go = Instantiate(nodes[i, j].walkable ? GameManager.Instance.walkableTilePrefab : GameManager.Instance.nonWalkableTilePrefab);
                go.transform.position = nodes[i, j].Position;
                go.transform.parent = transform;
            }
        }

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                IEnumerable<PathNode> neightbors = BuildNeightbors(ref nodes, i, j);
                foreach (PathNode neightbor in neightbors)
                {
                    nodes[i, j].AddChild(neightbor);
                }
            }
        }
    }

    private IEnumerable<PathNode> BuildNeightbors(ref PathNode[,] nodes, int i, int j)
    {
        List<PathNode> pathNodes = new List<PathNode>();

        int row_limit = nodes.GetLength(0) - 1;
        if (row_limit > 0)
        {
            int column_limit = nodes.GetLength(1) - 1;
            for (int x = Mathf.Max(0, i - 1); x <= Mathf.Min(i + 1, row_limit); x++)
            {
                for (int y = Mathf.Max(0, j - 1); y <= Mathf.Min(j + 1, column_limit); y++)
                {
                    if (x != i || y != j)
                    {
                        pathNodes.Add(nodes[x, y]);
                    }
                }
            }
        }

        return pathNodes;
    }
}