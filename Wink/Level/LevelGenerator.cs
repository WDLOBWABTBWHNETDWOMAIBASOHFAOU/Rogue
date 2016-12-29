using Microsoft.Xna.Framework;
using XNAPoint = Microsoft.Xna.Framework.Point;
using System;
using System.Collections.Generic;
using TriangleNet;
using TriangleNet.Geometry;
using System.Linq;
using TriangleNet.Data;
using QuickGraph;
using QuickGraph.Algorithms;

namespace Wink
{
    public static class LevelExtensions
    {
        public static Vector2 GetMiddleOfSide(this Rectangle rect, Collision.Side side)
        {
            switch (side)
            {
                case Collision.Side.Top:
                    return new Vector2(rect.Center.X, rect.Top);
                case Collision.Side.Bottom:
                    return new Vector2(rect.Center.X, rect.Bottom - 1);
                case Collision.Side.Left:
                    return new Vector2(rect.Left, rect.Center.Y);
                case Collision.Side.Right:
                    return new Vector2(rect.Right - 1, rect.Center.Y);
                default:
                    return Vector2.Zero;
            }
        }

        public static XNAPoint ToRoundedPoint(this Vector2 vector)
        {
            return new XNAPoint((int)Math.Round(vector.X), (int)Math.Round(vector.Y));
        }
    }

    public partial class Level
    {
        private class Room : ICloneable
        {
            public Room(Vector2 location, XNAPoint size)
            {
                Location = location;
                Size = size;
            }

            public Vector2 Location { get; set; }
            public XNAPoint Size { get; set; }

            //public Vector2 Velocity { get; set; }
            public Rectangle BoundingBox
            {
                get
                {
                    return new Rectangle(Location.ToRoundedPoint(), Size);
                }
            }

            public object Clone()
            {
                return new Room(Location, Size);
            }
        }

        private Random random = new Random();
        Random Random { get { return random; } }
        
        const double circleRadius = 7;
        
        double CircleArea
        {
            get { return Math.PI * Math.Pow(circleRadius, 2); }
        }

        //These are the minimum and maximum width and height of any room
        const int minDim = 4;
        const int maxDim = 10;

        int gaussMean = (minDim + maxDim) / 2;
        const double gaussVariance = 5d;

        double GaussianDistribution(double x)
        {
            double a = 1 / (Math.Sqrt(2 * gaussVariance * Math.PI));
            double b = -Math.Pow(x - gaussMean, 2) / 2 * gaussVariance;
            return a * Math.Pow(Math.E, b);
        }

        double GaussianDistribution(Vector2 v)
        {
            return (GaussianDistribution(v.X) + GaussianDistribution(v.Y)) / 2;
        }

        Vector2 GetRandomPointInCircle()
        {
            double t = 2 * Math.PI * Random.NextDouble();
            double u = Random.NextDouble() + Random.NextDouble();
            double r;
            if (u > 1)
                r = 2;
            else
                r = u;

            return new Vector2((float)(circleRadius * r * Math.Cos(t)), (float)(circleRadius * r * Math.Sin(t)));
        }
        
        const double TargetSurfaceArea = 1000;

        private List<Room> GenerateRooms()
        {
            //This double will have the average total area of a random room.
            double averageTotalArea = 0;
            for (int x = minDim; x <= maxDim; x++)
            {
                for (int y = minDim; y <= maxDim; y++)
                {
                    averageTotalArea += GaussianDistribution(new Vector2(x, y)) * x * y;
                }
            }
            
            double multiplier = 100 * CircleArea / averageTotalArea;

            List<Room> allRooms = new List<Room>();
            for (int x = minDim; x <= maxDim; x++)
            {
                for (int y = minDim; y <= maxDim; y++)
                {
                    XNAPoint p = new XNAPoint(x, y);
                    double gaussianDistribution = GaussianDistribution(p.ToVector2());
                    int roomAmount = (int)(multiplier * gaussianDistribution);
                    for (int i = 0; i < roomAmount; i++)
                    {
                        allRooms.Add(new Room(new Vector2(), p));
                    }
                }
            }

            List<Room> roomSelection = new List<Room>();
            int totalRoomArea = 0;
            while (totalRoomArea < TargetSurfaceArea)
            {
                int randomIndex = Random.Next(allRooms.Count);
                Room toAdd = allRooms[randomIndex].Clone() as Room;
                Vector2 pointInCircle = GetRandomPointInCircle();
                toAdd.Location = pointInCircle - toAdd.Size.ToVector2() / 2;
                roomSelection.Add(toAdd);

                totalRoomArea += toAdd.Size.X * toAdd.Size.Y;
            }

            int collisions = int.MaxValue;
            XNAPoint buffer = new XNAPoint(2, 2);
            while (collisions > 0)
            {
                collisions = 0;
                for (int i = 0; i < roomSelection.Count; i++)
                {
                    Room room1 = roomSelection[i];
                    Rectangle r1 = new Rectangle(room1.BoundingBox.Location + buffer, room1.BoundingBox.Size + buffer + buffer);
                    for (int j = i + 1; j < roomSelection.Count; j++)
                    {
                        Room room2 = roomSelection[j];
                        Rectangle r2 = new Rectangle(room2.BoundingBox.Location + buffer, room2.BoundingBox.Size + buffer + buffer);
                        if (room1 != room2 && r1.Intersects(r2))
                        {
                            Rectangle intersection = Rectangle.Intersect(r1, r2);
                            room1.Location += (r1.Center.ToVector2() - intersection.Center.ToVector2()) / 8;
                            room2.Location += (r2.Center.ToVector2() - intersection.Center.ToVector2()) / 8;
                            collisions++;
                        }
                    }
                }
            }

            //Get the lowest coodinates so we can adjust and bring everything into the positive.
            float lowestX = float.MaxValue;
            float lowestY = float.MaxValue;
            foreach (Room r in roomSelection)
            {
                if (r.Location.X < lowestX)
                    lowestX = r.Location.X;

                if (r.Location.Y < lowestY)
                    lowestY = r.Location.Y;
            }

            foreach (Room r in roomSelection)
            {
                r.Location -= new Vector2(lowestX, lowestY);
            }

             return roomSelection;
        }

        private List<Tuple<Room, Room>> GenerateHallwayPairs(List<Room> rooms)
        {
            InputGeometry ig = new InputGeometry();
            foreach (Room r in rooms)
            {
                double centerX = r.BoundingBox.Center.X;
                double centerY = r.BoundingBox.Center.Y;
                ig.AddPoint(centerX, centerY);
            }

            //use Triangulate method to generate Delaunay triangulation based on the points added to the InputGeometry.
            Mesh mesh = new Mesh();
            mesh.Triangulate(ig);

            //List of vertices.
            List<Vertex> vertices = mesh.Vertices.ToList();

            //Add all the vertices and edges to a QuickGraph graph.
            UndirectedGraph<int, TaggedUndirectedEdge<int, string>> graph = new UndirectedGraph<int, TaggedUndirectedEdge<int, string>>();
            for (int i = 0; i < vertices.Count; i++)
            {
                graph.AddVertex(i);
            }
            foreach (Edge e in mesh.Edges)
            {
                graph.AddEdge(new TaggedUndirectedEdge<int, string>(e.P0, e.P1, ""));
            }

            //Use QuickGraph to find the Minimum Spanning Tree using the distance as the weight.
            var mst = graph.MinimumSpanningTreePrim(edge =>
            {
                double dx = vertices[edge.Source].X - vertices[edge.Target].X;
                double dy = vertices[edge.Source].Y - vertices[edge.Target].Y;
                return Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
            }).ToList();

            //Randomly add back 10 percent of edges.
            int tenPercent = graph.EdgeCount / 10;
            List<TaggedUndirectedEdge<int, string>> allEdges = graph.Edges.ToList();
            for (int i = 0; i < tenPercent; i++)
            {
                int randomEdge = Random.Next(graph.EdgeCount);
                if (!mst.Contains(allEdges[randomEdge]))
                {
                    mst.Add(allEdges[randomEdge]);
                }
            }

            //Convert the Minimum Spanning Tree to a list of Room pairs.
            List<Tuple<Room, Room>> roomPairs = new List<Tuple<Room, Room>>();
            foreach (TaggedUndirectedEdge<int, string> edge in mst)
            {
                Room r1 = null;
                Room r2 = null;
                foreach(Room r in rooms)
                {
                    if (r.BoundingBox.Center.X == vertices[edge.Source].X && r.BoundingBox.Center.Y == vertices[edge.Source].Y)
                    {
                        r1 = r;
                    }
                    if (r.BoundingBox.Center.X == vertices[edge.Target].X && r.BoundingBox.Center.Y == vertices[edge.Target].Y)
                    {
                        r2 = r;
                    }
                }
                Tuple<Room, Room> roomPair = new Tuple<Room, Room>(r1, r2);
                roomPairs.Add(roomPair);
            }
            return roomPairs;
        }

        private TileField GenerateTiles(List<Room> rooms, List<Tuple<Room,Room>> hallwayPairs)
        {
            //Get the highest coordinates so we know the size of the TileField.
            int highestX = int.MinValue;
            int highestY = int.MinValue;
            foreach (Room r in rooms)
            {
                if (r.BoundingBox.Right + 1 > highestX)
                    highestX = r.BoundingBox.Right + 1;

                if (r.BoundingBox.Bottom + 1 > highestY)
                    highestY = r.BoundingBox.Bottom + 1;
            }

            //Make a dictionary with rooms and an empty list that will contain their exitpoints.
            Dictionary<Room, Dictionary<XNAPoint, Tuple<Room, XNAPoint>>> roomExitPoints =
                new Dictionary<Room, Dictionary<XNAPoint, Tuple<Room, XNAPoint>>>();

            foreach (Room r in rooms)
            {
                roomExitPoints.Add(r, new Dictionary<XNAPoint, Tuple<Room, XNAPoint>>());
            }

            //Make the tilefield and fill with default Tiles.
            TileField tf = new TileField(highestY + 1, highestX + 1, 0, "TileField");
            for (int x = 0; x < tf.Columns; x++)
            {
                for (int y = 0; y < tf.Rows; y++)
                {
                    tf.Add(new Tile(), x, y);
                }
            }

            //Find good points for the hallways to meet the rooms.
            foreach (Tuple<Room, Room> pair in hallwayPairs)
            {
                Vector2 center1 = pair.Item1.BoundingBox.Center.ToVector2();
                Vector2 center2 = pair.Item2.BoundingBox.Center.ToVector2();

                //Vector from center of item1 to center of item2 and vice versa
                Vector2 v1 = center2 - center1;
                Vector2 v2 = center1 - center2;

                Collision.Side s1 = CalculateExitSide(pair.Item1, v1);
                Collision.Side s2 = CalculateExitSide(pair.Item2, v2);

                v1.Normalize();
                v2.Normalize();

                XNAPoint exit1 = CalculateExit(v1, center1, s1, pair.Item1.BoundingBox.GetMiddleOfSide(s1)).ToPoint();
                XNAPoint exit2 = CalculateExit(v2, center2, s2, pair.Item2.BoundingBox.GetMiddleOfSide(s2)).ToPoint();

                XNAPoint exit1Point = exit1 + pair.Item1.BoundingBox.Center - pair.Item1.BoundingBox.Location;
                XNAPoint exit2Point = exit2 + pair.Item2.BoundingBox.Center - pair.Item2.BoundingBox.Location;

                roomExitPoints[pair.Item1].Add(exit1Point, new Tuple<Room, XNAPoint>(pair.Item2, exit2Point));
                roomExitPoints[pair.Item2].Add(exit2Point, new Tuple<Room, XNAPoint>(pair.Item1, exit1Point));
                //TODO: Fix issue that sometimes an item with same key gets added (multiple hallways same exit)

                //TODO: Generate hallways here
            }
            
            #region for each room, add floor and wall tiles to the tilefield.
            foreach (Room r in rooms)
            {
                int width = r.BoundingBox.Width;
                int height = r.BoundingBox.Height;
                
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Tile tile = null;
                        if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                        {
                            KeyValuePair<XNAPoint, Tuple<Room, XNAPoint>>? exitPair = null;
                            foreach (KeyValuePair<XNAPoint, Tuple<Room, XNAPoint>> kvp in roomExitPoints[r])
                            {
                                XNAPoint p = kvp.Key;
                                if (p.X == x && p.Y == y)
                                {
                                    exitPair = kvp;
                                }
                            }
                            if (exitPair.HasValue)
                            {
                                XNAPoint thisExit = exitPair.Value.Key;
                                Room connectingRoom = exitPair.Value.Value.Item1;
                                XNAPoint connectingPoint = exitPair.Value.Value.Item2;
                                int cx = connectingRoom.BoundingBox.X + connectingPoint.X;
                                int cy = connectingRoom.BoundingBox.Y + connectingPoint.Y;
                                tile = LoadFloorTile();
                                tile.AddDebugTag("ExitConnectionPoint", cx + "," + cy);
                            }
                            else
                                tile = LoadWallTile();
                        }
                        else if (x == r.BoundingBox.Center.X && y == r.BoundingBox.Center.Y)
                        {
                            tile = LoadFloorTile();
                            //tile.AddDebugTag("Room", );
                        }
                        else
                        {
                            tile = LoadFloorTile();
                        }
                        tf.Add(tile, x + r.BoundingBox.X, y + r.BoundingBox.Y);
                    }
                }
            }
            #endregion

            tf.Add(LoadStartTile(), rooms[0].Location.ToRoundedPoint().X + 1, rooms[0].Location.ToRoundedPoint().Y + 1);
            return tf;
        }

        private Collision.Side CalculateExitSide(Room r, Vector2 relVector)
        {
            Vector2 center = r.BoundingBox.Center.ToVector2();

            //Calculate angles of vector.
            double relAngle = Math.Atan2(relVector.Y, relVector.X);

            //Calculate the angles of the line 
            Vector2[] cornerVectors = new Vector2[4];
            cornerVectors[0] = new Vector2(r.BoundingBox.Left, r.BoundingBox.Top) - center;
            cornerVectors[1] = new Vector2(r.BoundingBox.Right, r.BoundingBox.Top) - center;
            cornerVectors[2] = new Vector2(r.BoundingBox.Right, r.BoundingBox.Bottom) - center;
            cornerVectors[3] = new Vector2(r.BoundingBox.Left, r.BoundingBox.Bottom) - center;

            double[] cornerAngles = new double[4];
            cornerAngles[0] = Math.Atan2(cornerVectors[0].Y, cornerVectors[0].X);
            cornerAngles[1] = Math.Atan2(cornerVectors[1].Y, cornerVectors[1].X);
            cornerAngles[2] = Math.Atan2(cornerVectors[2].Y, cornerVectors[2].X);
            cornerAngles[3] = Math.Atan2(cornerVectors[3].Y, cornerVectors[3].X);

            //Calculate Sides based on angle
            Collision.Side s = default(Collision.Side);
            if (relAngle >= cornerAngles[0] && relAngle < cornerAngles[1])
                s = Collision.Side.Top;
            else if (relAngle >= cornerAngles[1] && relAngle < cornerAngles[2])
                s = Collision.Side.Right;
            else if (relAngle >= cornerAngles[2] && relAngle < cornerAngles[3])
                s = Collision.Side.Bottom;
            else if (relAngle >= cornerAngles[3]/* && relAngle < cornerAngles[0] + 2 * Math.PI*/)
                s = Collision.Side.Left;

            return s;
        }

        private Vector2 CalculateExit(Vector2 direction, Vector2 center, Collision.Side s, Vector2 middleOfSide)
        {
            Vector2 exitPoint = middleOfSide - center;
            switch (s)
            {
                case Collision.Side.Top:
                case Collision.Side.Bottom:
                    exitPoint.X = direction.X * (exitPoint.Y / direction.Y);
                    break;
                case Collision.Side.Left:
                case Collision.Side.Right:
                    exitPoint.Y = direction.Y * (exitPoint.X / direction.X);
                    break;
            }

            return exitPoint;
        }
    }
}