using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.AStar
{
    public static class AStar
    {
        public static List<PathNode> FindPath(PathNode[,] nodes, PathNode start, PathNode end)
        {
            var open = new List<PathNode>() { start };
            var close = new List<PathNode>();

            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    nodes[i, j].gCost = int.MaxValue;
                    nodes[i, j].parent = null;
                }
            }

            start.gCost = 0;
            start.hCost = Mathf.RoundToInt(Vector3.Distance(start.Position, end.Position) * 10);

            while (open.Any())
            {
                PathNode currentNode = open.First(x => x.FCost == open.Min(y => y.FCost));
                if (currentNode == end)
                    return CalculatePath(end);

                open.Remove(currentNode);
                close.Add(currentNode);

                var color = new Color(1,1,0,0.1f);
                foreach (var neightor in currentNode.Neightbors)
                {
                    //Debug.DrawLine(neightor.Position, currentNode.Position, color, 2f);
                    if (close.Contains(neightor)) continue;

                    int tentativeGCost = currentNode.GetGCost(neightor);
                    if (tentativeGCost < neightor.gCost)
                    {
                        neightor.parent = currentNode;
                        neightor.gCost = tentativeGCost;
                        neightor.hCost = Mathf.RoundToInt(Vector3.Distance(neightor.Position, end.Position) * 10);

                        if (!open.Contains(neightor) && neightor.walkable)
                        {
                            open.Add(neightor);
                        }
                    }
                }
            }

            return null;
        }

        private static List<PathNode> CalculatePath(PathNode end)
        {
            List<PathNode> pathNodes = new List<PathNode>();

            pathNodes.Add(end);

            PathNode pathNode = end;
            while (pathNode.parent != null)
            {
                //Debug.DrawLine(pathNode.Position, pathNode.parent.Position, Color.red, 2f);
                pathNodes.Add(pathNode.parent);
                pathNode = pathNode.parent;
            }

            pathNodes.Reverse();

            return pathNodes;
        }
    }
}
