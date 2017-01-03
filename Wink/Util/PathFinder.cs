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
                        return tile.TilePosition - originNode.tile.TilePosition;
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
        private int baseCost = 100;
        private TileField tileField;

        private Dictionary<Tile, Node> nodeTable;

        public PathFinder(TileField tf)
        {
            addedCosts = new List<Func<Node, Node, int>>();
            nodeTable = new Dictionary<Tile, Node>();
            tileField = tf;

            //Add diagonal cost.
            AddCost((current, adjacent) =>
            {
                Point diff = adjacent.tile.TilePosition - current.tile.TilePosition;
                return diff.X == 0 || diff.Y == 0 ? 0 : 41;
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
            AddCost((current, adjacent) =>
            {
                Point diff = adjacent.tile.TilePosition - current.tile.TilePosition;
                return diff.X == 0 || diff.Y == 0 ? 0 : 259; // Adding another 259 for a total diagonal cost of 300 (meaning it won't be used)
            });

            //Add a cost that punishes going in a different direction, to avoid zigzagging.
            AddCost((current, adjacent) =>
                {
                    Point diff = adjacent.tile.TilePosition - current.tile.TilePosition;
                    if (current.DirectionFromOrigin != diff)
                    {
                        float dStart = (adjacent.tile.TilePosition - startingNode.tile.TilePosition).ToVector2().Length();
                        float dEnd = (endingNode.tile.TilePosition - adjacent.tile.TilePosition).ToVector2().Length();
                        float totalLength = dStart + dEnd;
                        //TODO: find out why this only works when effectively rewarding a bend in the middle, and not when evening out.
                        //Maybe because A* only visits opennodes with the lowest cost?
                        return (int)(50 - (1 - Math.Abs(dStart / totalLength - dEnd / totalLength)) * 60);
                    }
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

        public List<Tile> ShortestPath(Tile start, Tile end)
        {
            return ShortestPath(start, end, tile => tile.Passable);
        }

        /// <summary>
        /// </summary>
        /// <param name="start">Position of start tile in the tilefield.</param>
        /// <param name="end">Position of end tile in the tilefield.</param>
        /// <param name="validTile">A Func that returns true for valid tiles and false for invalid tiles.</param>
        /// <returns></returns>
        public List<Tile> ShortestPath(Tile start, Tile end, Func<Tile, bool> validTile)
        {
            List<Node> openNodes = new List<Node>();
            List<Node> closedNodes = new List<Node>();
            List<Tile> path = new List<Tile>();

            // Need grid positions
            startingNode = GetNode(start);
            endingNode = GetNode(end);
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
                
                if (currentNode.tile.Equals(endingNode.tile))
                { //Found the tile to reach.
                    openNodes.Clear();
                    closedNodes.Clear();
                    nodeTable.Clear();
                    path = FindPath();
                    path.Reverse();
                    return path;
                }

                // Opens the surrounding Tile
                // Add the surrounding 8 Tile to the openTile if they are valid and not in the closedTile list and set gCost and hCost(hCost will be calculated and wont change later on)
                // If a node is already in the openTile list check if the gCost is lower from this currentNode and update the gCost and originNode when true
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Tile surroundingTile = tileField[currentNode.X + x, currentNode.Y + y] as Tile;
                        Node surroundingNode = GetNode(surroundingTile);
                        if (surroundingNode == null)
                            continue;

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
                            else if (validTile.Invoke(surroundingNode.tile))
                            {   // This is a valid newly discovered node (it was contained in neither the closednodes nor the opennodes)
                                openNodes.Add(surroundingNode);
                                surroundingNode.originNode = currentNode;
                                surroundingNode.gCost = currentNode.gCost + CalculateCost(currentNode, surroundingNode);
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