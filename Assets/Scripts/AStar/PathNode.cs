using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.AStar
{
    public class PathNode
    {
        private Vector3 position;
        private List<PathNode> childNodes;
        private Dictionary<int, int> distances;

        public bool walkable = true;
        public PathNode parent;

        public int hCost = 0;
        public int gCost = int.MaxValue;

        public int FCost
        {
            get { return hCost + gCost; }
        }

        public IEnumerable<PathNode> Neightbors
        {
            get { return childNodes; }
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public PathNode(Vector3 position)
        {
            this.position = position;
            childNodes = new List<PathNode>();
            distances = new Dictionary<int, int>();
        }

        public void AddChild(PathNode pathNode)
        {
            int distance = Mathf.RoundToInt(Vector3.Distance(pathNode.Position, position) * 10);
            distances.Add(childNodes.Count, distance);
            childNodes.Add(pathNode);
        }

        public void RemoveChild(PathNode pathNode)
        {
            childNodes.Remove(pathNode);
        }

        public int GetGCost(PathNode pathNode)
        {
            return gCost + distances[childNodes.IndexOf(pathNode)];
        }
    }
}
