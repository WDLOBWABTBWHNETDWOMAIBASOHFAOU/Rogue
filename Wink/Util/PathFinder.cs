using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Wink
{
    class PathFinder
    {
        public class Node
        {
            public Tile tile;
            public Node originNode;
            public int hCost;
            public int gCost;

            public int fCost
            {
                get { return hCost + gCost; }
            }

            public int X
            {
                get { return tile.TilePosition.X; }
            }

            public int Y
            {
                get { return tile.TilePosition.Y; }
            }

            public Point DirectionFromOrigin
            {
                get
                {
                    if (originNode != null)
                        return originNode.tile.TilePosition - tile.TilePosition;
                    else
                        return Point.Zero;
                }
            }

            public Node(Tile t)
            {
                tile = t;
            }
        }

        private Node startingNode, endingNode;
        private List<Func<Node, Node, int>> addedCosts;
        private int baseCost = 10;

        private Dictionary<Tile, Node> nodeTable;

        public PathFinder(TileField tf)
        {
            addedCosts = new List<Func<Node, Node, int>>();
            nodeTable = new Dictionary<Tile, Node>();

            //Add diagonal cost.
            AddCost((current, adjacent) =>
            {
                Point diff = adjacent.tile.TilePosition - current.tile.TilePosition;
                return diff.X == 0 || diff.Y == 0 ? 0 : 4;
            });
        }

        public Node GetNode(Tile t)
        {
            if (t == null)
            {
                return null;
            }

            if (!nodeTable.ContainsKey(t))
            {
                nodeTable.Add(t, new Node(t));
            }
            return nodeTable[t];
        }

        public void EnableStraightLines()
        {
            //
            AddCost((current, adjacent) =>
            {
                Point diff = adjacent.tile.TilePosition - current.tile.TilePosition;
                return diff.X == 0 || diff.Y == 0 ? 0 : 1000;
            });

            //Add a cost that punishes going in a different direction.
            AddCost(
                (current, adjacent) =>
                {
                    Point diff = adjacent.tile.TilePosition - current.tile.TilePosition;
                    if (
                        current.DirectionFromOrigin != Point.Zero &&
                        current.DirectionFromOrigin != diff
                    )
                        return 50;
                    else
                        return 0;
                }
            );
        }

        /// <summary>
        /// Use this method to add custom costs to the pathfinding algorithm.
        /// </summary>
        /// <param name="cost">
        /// The Func's Node parameter is one of the surrounding nodes, of which the cost must be calculated.
        /// The Func's Vector2 is the difference in position of the surrounding node compared to the current node.
        /// </param>
        public void AddCost(Func<Node, Node, int> cost)
        {
            addedCosts.Add(cost);
        }

        private int CalculateCost(Node current, Node adjacent)
        {
            int totalCost = baseCost;
            foreach(Func<Node, Node, int> f in addedCosts)
            {
                totalCost += f.Invoke(current, adjacent);
            }
            return totalCost;
        }

        public List<Tile> ShortestPath(Point start, Point end, TileField tf)
        {
            return ShortestPath(start, end, tf, tile => tile.Passable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start">Position of start tile in the tilefield.</param>
        /// <param name="end">Position of end tile in the tilefield.</param>
        /// <param name="tf"></param>
        /// <param name="validTile"></param>
        /// <param name="baseCost"></param>
        /// <param name="diagCost"></param>
        /// <returns></returns>
        public List<Tile> ShortestPath(Point start, Point end, TileField tf, Func<Tile, bool> validTile)
        {
            List<Node> openNodes = new List<Node>();
            List<Node> closedNodes = new List<Node>();
            List<Tile> path = new List<Tile>();

            // Need grid positions
            startingNode = GetNode(tf[start.X, start.Y] as Tile);
            endingNode = GetNode(tf[end.X, end.Y] as Tile);
            openNodes.Add(startingNode);

            while (openNodes.Count > 0)
            {
                Node currentNode = openNodes[0];

                // Replaces current node with new current node
                for (int i = 0; i < openNodes.Count; i++)
                {
                    if (currentNode.fCost > openNodes[i].fCost || (currentNode.fCost == openNodes[i].fCost && currentNode.hCost > openNodes[i].hCost))
                    {
                        currentNode = openNodes[i];
                    }
                }

                closedNodes.Add(currentNode);
                openNodes.Remove(currentNode);

                // Check the validity Func, if it returns false, skip this.
                if (!validTile.Invoke(currentNode.tile))
                    continue;

                // Opens the surrounding Tile
                // Add the surrounding 8 Tile to the openTile if they are walkable and not in the closedTile list and set gCost and hCost(hCost will be calculated and wont change later on)
                // If a node is already in the openTile list check if the gCost is lower from this currentNode and update the gCost and originNode when true
                // Also check if the endingNode is one of these 8
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Tile surroundingTile = tf[currentNode.X + x, currentNode.Y + y] as Tile;
                        Node surroundingNode = GetNode(surroundingTile);
                        if (surroundingNode == null)
                            continue;

                        if (endingNode.tile != null && surroundingTile.Equals(endingNode.tile))
                        {
                            openNodes.Clear();
                            closedNodes.Clear();
                            surroundingNode.originNode = currentNode;
                            endingNode = surroundingNode;
                            path = FindPath();
                            path.Reverse();
                            return path;
                        }

                        if (!closedNodes.Contains(surroundingNode))
                        {
                            if (openNodes.Contains(surroundingNode))
                            {
                                int newCost = currentNode.gCost + CalculateCost(currentNode, surroundingNode);
                                if (surroundingNode.gCost > newCost)
                                {
                                    surroundingNode.gCost = newCost;
                                    surroundingNode.originNode = currentNode;
                                }
                            }
                            else
                            { 
                                // This is a newly discovered node (it was contained in neither the closednodes nor the opennodes)
                                openNodes.Add(surroundingNode);
                                surroundingNode.originNode = currentNode;
                                surroundingNode.gCost = CalculateCost(currentNode, surroundingNode);
                            }
                        }
                    }
                }
            }
            return path;
        }

        private List<Tile> FindPath()
        {
            Node currentNode = endingNode;
            List<Tile> path = new List<Tile>();
            while (!currentNode.tile.Equals(startingNode.tile))
            {
                path.Add(currentNode.tile);
                currentNode = currentNode.originNode;
            }
            return path;
        }
    }
}