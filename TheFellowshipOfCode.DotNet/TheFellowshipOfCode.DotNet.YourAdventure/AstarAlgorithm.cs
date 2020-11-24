using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TheFellowshipOfCode.DotNet.YourAdventure
{
    public class Vector2
    {
        public int x;
        public int y;

        public Vector2(int xCoord, int yCoord)
        {
            this.x = xCoord;
            this.y = yCoord;
        }
    }

    public class AstarNode
    {
        public AstarNode ParentAstarNode;
        public Vector2 Position;

        public float distanceToTarget;
        public float cost;
        public float weight;

        public float fFactor
        {
            get
            {
                if (distanceToTarget != -1 && cost != -1)
                {
                    return distanceToTarget + cost;
                }
                else
                {
                    return -1;
                }
            }
        }

        public enum Walkable
        {
            Able,
            Not_Able
        }

        public Walkable walkable;

        public AstarNode(Vector2 pos, Walkable walkable, float weight = 1)
        {
            ParentAstarNode = null;
            Position = pos;
            distanceToTarget = -1;
            cost = 1;
            this.weight = weight;
            this.walkable = walkable;
        }
    }

    public class AstarAlgorithm
    {
        private List<List<AstarNode>> Grid;

        int GridRows
        {
            get { return Grid[0].Count; }
        }

        int GridCols
        {
            get { return Grid.Count; }
        }

        public AstarAlgorithm(List<List<AstarNode>> grid)
        {
            Grid = grid;
        }

        public Stack<AstarNode> FindPath(Vector2 startPos, Vector2 endPos)
        {
            AstarNode startNode =
                new AstarNode(new Vector2((startPos.x),(startPos.y)),
                    AstarNode.Walkable.Able);
            AstarNode endNode = new AstarNode(new Vector2(endPos.x, endPos.y), AstarNode.Walkable.Able);



            //Nodes that make the shortest path to treasure (if no treasures left, path to finish)
            Stack<AstarNode> Path = new Stack<AstarNode>();
            //Not visited nodes
            List<AstarNode> openNodes = new List<AstarNode>();
            //Visited nodes
            List<AstarNode> closedNodes = new List<AstarNode>();

            List<AstarNode> adjacentNodes;

            //assign startnode as current node ( at start of run )
            AstarNode currentNode = startNode;

            //add start node to Open List
            openNodes.Add(startNode);

            while (openNodes.Count != 0 && !closedNodes.Exists(x => x.Position == endNode.Position))
            {
                currentNode = openNodes[0];
                //remove first node and add it to closedNodes
                openNodes.Remove(currentNode);
                closedNodes.Add(currentNode);
                //get surrounding nodes from current node
                adjacentNodes = getAdjacentNodes(currentNode);

                foreach (AstarNode node in adjacentNodes)
                {
                    if (!closedNodes.Contains(node) && (node.walkable == AstarNode.Walkable.Able))
                    {
                        if (!openNodes.Contains(node))
                        {
                            node.ParentAstarNode = currentNode;
                            node.distanceToTarget = Math.Abs(node.Position.x - endNode.Position.x) +
                                                    Math.Abs(node.Position.y - endNode.Position.y);
                            node.cost = node.weight + node.ParentAstarNode.cost; // change weight here ? 
                            openNodes.Add(node);
                            openNodes = openNodes.OrderBy(n => n.fFactor).ToList<AstarNode>();
                        }
                    }
                }
            }

            //build path to end node, if end was not closed return null
            if (!closedNodes.Exists(x => x.Position == endNode.Position))
            {
                return null;
            }

            AstarNode tempNode = closedNodes[closedNodes.IndexOf(currentNode)];

            if (tempNode == null) return null;

            do
            {
                Path.Push(tempNode);
                tempNode = tempNode.ParentAstarNode;
            } while (tempNode != startNode && tempNode != null);

            return Path;
        }

        private List<AstarNode> getAdjacentNodes(AstarNode n)
        {
            List<AstarNode> temp = new List<AstarNode>();

            int row = (int) n.Position.y;
            int col = (int) n.Position.x;

            if (row + 1 < GridRows)
            {
                temp.Add(Grid[col][row + 1]);
            }

            if (row - 1 >= 0)
            {
                temp.Add(Grid[col][row - 1]);
            }

            if (col - 1 >= 0)
            {
                temp.Add(Grid[col - 1][row]);
            }

            if (col + 1 < GridCols)
            {
                temp.Add(Grid[col + 1][row]);
            }

            return temp;
        }
    }
}