using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Wink
{
    class Pathfinding
    {
        static Tile startingNode, endingNode;

        public static List<Tile> ShortestPath(Vector2 start, Vector2 end, TileField tf, int cost = 10, int diagCost = 14)
        {
            return ShortestPath(start, end, tf, tile => tile.Passable, cost, diagCost);
        }

        public static List<Tile> ShortestPath(Vector2 start, Vector2 end, TileField tf, Func<Tile, bool> validTile, int cost = 10, int diagCost = 14)
        {
            List<Tile> openTile = new List<Tile>();
            List<Tile> closedTile = new List<Tile>();
            List<Tile> path = new List<Tile>();
            // Need grid positions
            startingNode = tf[(int)start.X, (int)start.Y] as Tile;
            endingNode = tf[(int)end.X, (int)end.Y] as Tile;
            openTile.Add(startingNode);

            while (openTile.Count > 0)
            {
                Tile currentNode = openTile[0];

                // Replaces current node with new current node
                for (int i = 0; i < openTile.Count; i++)
                {
                    if (currentNode.fCost > openTile[i].fCost || (currentNode.fCost == openTile[i].fCost && currentNode.hCost > openTile[i].hCost))
                    {
                        currentNode = openTile[i];
                    }
                }

                closedTile.Add(currentNode);
                openTile.Remove(currentNode);
                if (!validTile.Invoke(currentNode)) continue;
                // Opens the surrounding Tile
                // Add the surrounding 8 Tile to the openTile if they are walkable and not in the closedTile list and set gCost and hCost(hCost will be calculated and wont change later on)
                // If a node is already in the openTile list check if the gCost is lower from this currentNode and update the gCost and originNode when true
                // Also check if the endingNode is one of these 8
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Tile surroundingNode = tf[currentNode.TilePosition.X + x, currentNode.TilePosition.Y + y] as Tile; // Again with the grid positions
                        if (surroundingNode == endingNode)
                        {
                            openTile.Clear();
                            closedTile.Clear();
                            surroundingNode.originNode = currentNode;
                            path = FindPath();
                            path.Reverse();
                            return path;
                        }

                        if (!closedTile.Contains(surroundingNode) && surroundingNode != null)
                        {
                            if (openTile.Contains(surroundingNode))
                            {
                                if (x == 0 || y == 0)
                                {
                                    if (surroundingNode.gCost > currentNode.gCost + cost)
                                    {
                                        surroundingNode.gCost = currentNode.gCost + cost;
                                        surroundingNode.originNode = currentNode;
                                    }
                                }
                                else
                                {
                                    if (surroundingNode.gCost > currentNode.gCost + diagCost)
                                    {
                                        surroundingNode.gCost = currentNode.gCost + diagCost;
                                        surroundingNode.originNode = currentNode;
                                    }
                                }
                                
                            }
                            else
                            {
                                openTile.Add(surroundingNode);
                                surroundingNode.originNode = currentNode;
                                if (x == 0 || y == 0)
                                {
                                    surroundingNode.gCost = currentNode.gCost + cost;
                                }
                                else
                                {
                                    surroundingNode.gCost = currentNode.gCost + diagCost;
                                }
                            }
                        }
                    }
                }
            }
            return path;
        }

        private static List<Tile> FindPath()
        {
            Tile currentNode = endingNode;
            List<Tile> path = new List<Tile>();
            while (!currentNode.Equals(startingNode))
            {
                path.Add(currentNode);
                currentNode = currentNode.originNode;
            }
            return path;
        }
    }
}