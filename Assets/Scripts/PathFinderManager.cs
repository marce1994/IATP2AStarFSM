using Assets.Scripts.AStar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathFinderManager : Singleton<PathFinderManager>
{
    public int sizeX, sizeZ = 10;
    private PathNode[,] nodes;
    private float[,] noise;

    public float wakableWeight = 0.3f;

    public List<PathNode> Nodes
    {
        get { return nodes.Cast<PathNode>().ToList(); }
    }

    public Vector3 RandomWalkablePosition
    {
        get
        {
            var walkable_nodes = nodes.Cast<PathNode>().Where(x => x.walkable);
            var random_node = walkable_nodes.ElementAt(Random.Range(0, walkable_nodes.Count() - 1));
            return random_node.Position;
        }
    }

    public Task<Vector3[]> FindRandomPath(Vector3 origin)
    {
        PathNode[,] nodes_clone = (PathNode[,])nodes.Clone();
        var listNodes = nodes_clone.Cast<PathNode>().Where(x => x.walkable).ToList();
        return FindPath(origin, listNodes.ElementAt(Random.Range(0, listNodes.Count - 1)).Position);
    }

    public Task<Vector3[]> FindPath(Vector3 origin, Vector3 destiny)
    {
        PathNode[,] nodes_clone = (PathNode[,])nodes.Clone();
        var listNodes = nodes_clone.Cast<PathNode>().Where(x => x.walkable).ToList();

        PathNode origNode = GetClosest(origin, listNodes);
        PathNode destNode = GetClosest(destiny, listNodes);

        var path = AStar.FindPath(nodes_clone, origNode, destNode);

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

    private int skipFrames = 30;
    private int currFrames = 0;

    private void Update()
    {
        currFrames++;
        if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

        if (Input.GetMouseButton(0) && currFrames >= skipFrames)
        {
            currFrames = 0;
            Vector3 mouse = Input.mousePosition;
            Ray castPoint = Camera.main.ScreenPointToRay(mouse);
            RaycastHit hit;
            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
            {
                var nodeList = nodes.Cast<PathNode>().ToList();

                var posx = Mathf.RoundToInt((sizeX / 2) + hit.point.x);
                var posy = Mathf.RoundToInt((sizeZ / 2) + hit.point.y);

                PathNode destNode = null;

                try
                {
                    destNode = nodes[posx, posy];
                }
                catch (System.Exception)
                {
                    Debug.LogWarning("Clicked out of node grid");
                }

                if (destNode == null) return;

                Instance.FindPath(nodes[0, 0].Position, destNode.Position).ContinueWith((res) =>
                {
                    Debug.LogWarning(res);
                    if (res.IsFaulted) return;
                    if (res.Result == null) return;

                    var go = new GameObject("LineRenderer");
                    Debug.LogWarning("Line renderer");
                    var lineRenderer = go.AddComponent<LineRenderer>();
                    lineRenderer.positionCount = res.Result.Count();
                    lineRenderer.SetPositions(res.Result);
                    lineRenderer.material.color = Color.red;
                    Destroy(go, 0.5f);
                });
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (nodes == null) return;
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                PathNode pathNode = nodes[i, j];
                Vector3 size = Vector3.one * .9f;
                size.z = 0;
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(pathNode.Position, .05f);

                foreach (var neightbor in pathNode.Neightbors)
                {
                    Gizmos.color = Color.cyan * new Color(0, 0, 0, .1f);
                    Gizmos.DrawLine(pathNode.Position, neightbor.Position);
                }
            }
        }
    }

    private void Awake()
    {
        nodes = new PathNode[sizeX, sizeZ];

        var noiseGenerator = new NoiseGenerator(sizeX, sizeZ);

        noiseGenerator.offsetX = Random.Range(100, 99999);
        noiseGenerator.offsetY = Random.Range(100, 99999);
        noiseGenerator.scale = 5f;

        noise = noiseGenerator.GenerateNoise();

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                nodes[i, j] = new PathNode(new Vector3((-sizeX / 2) + i, (-sizeZ / 2 + j), 0));
                nodes[i, j].walkable = noise[i, j] > wakableWeight;
                var go = Instantiate(nodes[i, j].walkable ? GameManager.Instance.walkableTilePrefab : GameManager.Instance.nonWalkableTilePrefab);
                go.transform.position = nodes[i, j].Position;
                go.transform.parent = transform;
            }
        }

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                IEnumerable<PathNode> neightbors = BuildNeightbors(ref nodes, i, j);
                foreach (var neightbor in neightbors)
                {
                    nodes[i, j].AddChild(neightbor);
                }
            }
        }
    }

    private IEnumerable<PathNode> BuildNeightbors(ref PathNode[,] nodes, int i, int j)
    {
        List<PathNode> pathNodes = new List<PathNode>();

        var row_limit = nodes.GetLength(0) - 1;
        if (row_limit > 0)
        {
            var column_limit = nodes.GetLength(1) - 1;
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