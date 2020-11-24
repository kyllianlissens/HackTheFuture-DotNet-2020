﻿using System.Collections;

namespace TheFellowshipOfCode.DotNet.YourAdventure
{
    public class Grid
    {
        // grid size
        private int gridSizeX, gridSizeY;

        // nodes in grid
        public Node[,] nodes;

        public Grid(float[,] tiles_costs)
        {
            // create nodes
            CreateNodes(tiles_costs.GetLength(0), tiles_costs.GetLength(1));

            // init nodes
            for (var x = 0; x < gridSizeX; x++)
            for (var y = 0; y < gridSizeY; y++)
                nodes[x, y] = new Node(tiles_costs[x, y], x, y);
        }

        
        public Grid(bool[,] walkable_tiles)
        {
            // create nodes
            CreateNodes(walkable_tiles.GetLength(0), walkable_tiles.GetLength(1));

            // init nodes
            for (var x = 0; x < gridSizeX; x++)
            for (var y = 0; y < gridSizeY; y++)
                nodes[x, y] = new Node(walkable_tiles[x, y] ? 1.0f : 0.0f, x, y);
        }

        
        private void CreateNodes(int width, int height)
        {
            gridSizeX = width;
            gridSizeY = height;
            nodes = new Node[gridSizeX, gridSizeY];
        }

       
        public void UpdateGrid(float[,] tiles_costs)
        {
            // check if need to re-create grid
            if (nodes == null ||
                gridSizeX != tiles_costs.GetLength(0) ||
                gridSizeY != tiles_costs.GetLength(1))
                CreateNodes(tiles_costs.GetLength(0), tiles_costs.GetLength(1));

            // update nodes
            for (var x = 0; x < gridSizeX; x++)
            for (var y = 0; y < gridSizeY; y++)
                nodes[x, y].Update(tiles_costs[x, y], x, y);
        }

       
        public void UpdateGrid(bool[,] walkable_tiles)
        {
            // check if need to re-create grid
            if (nodes == null ||
                gridSizeX != walkable_tiles.GetLength(0) ||
                gridSizeY != walkable_tiles.GetLength(1))
                CreateNodes(walkable_tiles.GetLength(0), walkable_tiles.GetLength(1));

            // update grid
            for (var x = 0; x < gridSizeX; x++)
            for (var y = 0; y < gridSizeY; y++)
                nodes[x, y].Update(walkable_tiles[x, y] ? 1.0f : 0.0f, x, y);
        }

        
        public IEnumerable GetNeighbours(Node node, Pathfinding.DistanceType distanceType)
        {
            int x = 0, y = 0;
            switch (distanceType)
            {
                case Pathfinding.DistanceType.Manhattan:
                    y = 0;
                    for (x = -1; x <= 1; ++x)
                    {
                        var neighbor = AddNodeNeighbour(x, y, node);
                        if (neighbor != null)
                            yield return neighbor;
                    }

                    x = 0;
                    for (y = -1; y <= 1; ++y)
                    {
                        var neighbor = AddNodeNeighbour(x, y, node);
                        if (neighbor != null)
                            yield return neighbor;
                    }

                    break;

                case Pathfinding.DistanceType.Euclidean:
                    for (x = -1; x <= 1; x++)
                    for (y = -1; y <= 1; y++)
                    {
                        var neighbor = AddNodeNeighbour(x, y, node);
                        if (neighbor != null)
                            yield return neighbor;
                    }

                    break;
            }
        }

        
        private Node AddNodeNeighbour(int x, int y, Node node)
        {
            if (x == 0 && y == 0) return null;

            var checkX = node.gridX + x;
            var checkY = node.gridY + y;

            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) return nodes[checkX, checkY];

            return null;
        }
    }
}