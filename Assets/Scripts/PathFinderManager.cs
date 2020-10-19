using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Transactions;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class PathFinderManager : Singleton<PathFinderManager>
{
    public int sizeX, sizeZ = 10;
    private PathNode[,] nodes;
    private float[,] noise;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Awake();

        if (Input.GetMouseButton(0)) {
            Vector3 mouse = Input.mousePosition;
            Ray castPoint = Camera.main.ScreenPointToRay(mouse);
            RaycastHit hit;
            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
            {
                var path = FindPath(nodes[0, 0], nodes[Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z)]);
                if (path != null)
                {
                    for (int i = 0; i < path.Count(); i++)
                    {
                        var from = path.ElementAtOrDefault(i);
                        var to = path.ElementAtOrDefault(i + 1);
                        if (to != null)
                        {
                            Debug.DrawLine(from.Position, to.Position, Color.red, 0.01f);
                        }
                    }
                }
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
                size.y = 0;
                Gizmos.color = new Color(noise[i, j] > .5f? 0:1, noise[i, j], 0);
                Gizmos.DrawCube(pathNode.Position, size);
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
        noiseGenerator.scale = 2f;

        noise = noiseGenerator.GenerateNoise();

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                nodes[i, j] = new PathNode(new Vector3(i, 0, j));
                nodes[i, j].walkable = noise[i, j] > 0.5f;
            }
        }

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                IEnumerable<PathNode> neightbors = GetNeightbors(ref nodes,i, j);
                foreach (var neightbor in neightbors)
                {
                    nodes[i, j].AddChild(neightbor);
                }
            }
        }
    }

    public IEnumerable<PathNode> FindPath(PathNode start, PathNode end)
    {
        var open = new List<PathNode>() { start };
        var close = new List<PathNode>();

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                nodes[i, j].gCost = int.MaxValue;
                nodes[i, j].parent = null;
            }
        }

        start.gCost = 0;
        start.hCost = Mathf.RoundToInt(Vector3.Distance(start.Position, end.Position));

        while (open.Any()) {
            PathNode currentNode = open.First(x => x.fCost == open.Min(y => y.fCost));
            if (currentNode == end)
                return CalculatePath(end);

            open.Remove(currentNode);
            close.Add(currentNode);

            foreach (var neightor in currentNode.Neightbors)
            {
                Debug.DrawLine(neightor.Position, currentNode.Position, Color.cyan, 1f);

                if (close.Contains(neightor)) continue;

                int tentativeGCost = currentNode.GetGCost(neightor);
                if (tentativeGCost < neightor.gCost)
                {
                    neightor.parent = currentNode;
                    neightor.gCost = tentativeGCost;
                    neightor.hCost = Mathf.RoundToInt(Vector3.Distance(neightor.Position, end.Position));

                    if (!open.Contains(neightor) && neightor.walkable) {
                        open.Add(neightor);
                    }
                }
            }
        }

        return null;
    }

    private List<PathNode> CalculatePath(PathNode end) {
        List<PathNode> pathNodes = new List<PathNode>();

        pathNodes.Add(end);

        PathNode pathNode = end;
        while (pathNode.parent != null) {
            pathNodes.Add(pathNode.parent);
            pathNode = pathNode.parent;
        }

        pathNodes.Reverse();

        return pathNodes;
    }

    private IEnumerable<PathNode> GetNeightbors(ref PathNode[,] nodes, int i, int j)
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
                        pathNodes.Add(nodes[x,y]);
                    }
                }
            }
        }

        return pathNodes;
    }
}

public class PathNode {
    private Vector3 position;
    private List<PathNode> childNodes;
    private Dictionary<int, int> distances;

    public bool walkable = true;
    public PathNode parent;

    public int hCost = 0;
    public int gCost = int.MaxValue;

    public int fCost
    {
        get { return hCost + gCost; }
    }


    public IEnumerable<PathNode> Neightbors
    {
        get { return childNodes; }
    }

    public Vector3 Position {
        get { return position; }
    }

    public PathNode(Vector3 position) {
        this.position = position;
        childNodes = new List<PathNode>();
        distances = new Dictionary<int, int>();
    }

    public void AddChild(PathNode pathNode) {
        int distance = Mathf.RoundToInt(Vector3.Distance(pathNode.Position, position) * 10);
        distances.Add(childNodes.Count, distance);
        childNodes.Add(pathNode);
    }

    public void RemoveChild(PathNode pathNode) {
        childNodes.Remove(pathNode);
    }

    public int GetGCost(PathNode pathNode) {
        return gCost + distances[childNodes.IndexOf(pathNode)];
    }
}