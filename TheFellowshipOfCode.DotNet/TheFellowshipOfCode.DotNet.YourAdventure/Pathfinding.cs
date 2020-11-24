using System;
using System.Collections.Generic;

namespace TheFellowshipOfCode.DotNet.YourAdventure
{
    public class Pathfinding
    {
        
        public enum DistanceType
        {
            Euclidean,
            Manhattan
        }

        
        public static List<Point> FindPath(Grid grid, Point startPos, Point targetPos,
            DistanceType distance = DistanceType.Euclidean, bool ignorePrices = false)
        {
            // find path
            var nodes_path = _ImpFindPath(grid, startPos, targetPos, distance, ignorePrices);

            // convert to a list of points and return
            var ret = new List<Point>();
            if (nodes_path != null)
                foreach (var node in nodes_path)
                    ret.Add(new Point(node.gridX, node.gridY));
            return ret;
        }


        private static List<Node> _ImpFindPath(Grid grid, Point startPos, Point targetPos,
            DistanceType distance = DistanceType.Euclidean, bool ignorePrices = false)
        {
            var startNode = grid.nodes[startPos.x, startPos.y];
            var targetNode = grid.nodes[targetPos.x, targetPos.y];

            var openSet = new List<Node>();
            var closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet[0];
                for (var i = 1; i < openSet.Count; i++)
                    if (openSet[i].fCost < currentNode.fCost ||
                        openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                        currentNode = openSet[i];

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode) return RetracePath(grid, startNode, targetNode);

                foreach (Node neighbour in grid.GetNeighbours(currentNode, distance))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                    var newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) *
                        (ignorePrices ? 1 : (int) (10.0f * neighbour.price));
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            return null;
        }

        
        private static List<Node> RetracePath(Grid grid, Node startNode, Node endNode)
        {
            var path = new List<Node>();
            var currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();
            return path;
        }

        
        private static int GetDistance(Node nodeA, Node nodeB)
        {
            var dstX = Math.Abs(nodeA.gridX - nodeB.gridX);
            var dstY = Math.Abs(nodeA.gridY - nodeB.gridY);
            return dstX > dstY ? 14 * dstY + 10 * (dstX - dstY) : 14 * dstX + 10 * (dstY - dstX);
        }
    }
}