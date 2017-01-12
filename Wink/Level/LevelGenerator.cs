using Microsoft.Xna.Framework;
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
                    return new Vector2(rect.Center.X, rect.Bottom);
                case Collision.Side.Left:
                    return new Vector2(rect.Left, rect.Center.Y);
                case Collision.Side.Right:
                    return new Vector2(rect.Right, rect.Center.Y);
                default:
                    return Vector2.Zero;
            }
        }
    }

    public partial class Level
    {
        private class Room : ICloneable
        {
            public Room(Vector2 location, Microsoft.Xna.Framework.Point size)
            {
                Location = location;
                Size = size;
            }

            public Vector2 Location { get; set; }
            public Microsoft.Xna.Framework.Point Size { get; set; }

            //public Vector2 Velocity { get; set; }
            public Rectangle BoundingBox
            {
                get
                {
                    return new Rectangle(Location.ToPoint(), Size);
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
                    Vector2 v = new Vector2(x, y);
                    double gaussianDistribution = GaussianDistribution(v);
                    int roomAmount = (int)(multiplier * gaussianDistribution);
                    for (int i = 0; i < roomAmount; i++)
                    {
                        allRooms.Add(new Room(new Vector2(), v.ToPoint()));
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
            Microsoft.Xna.Framework.Point buffer = new Microsoft.Xna.Framework.Point(1, 1);
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
            //List<Room> roomsClone = rooms.Select(item => (Room)item.Clone()).ToList();
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

        private string ConvertRoomsToTestString(List<Room> rooms)
        {
            //Get the highest coordinates so we know the size of the level.
            int highestX = int.MinValue;
            int highestY = int.MinValue;
            foreach (Room r in rooms)
            {
                if (r.BoundingBox.Right > highestX)
                    highestX = r.BoundingBox.Right;

                if (r.BoundingBox.Bottom > highestY)
                    highestY = r.BoundingBox.Bottom;
            }

            //Make a two-dimensional character array that is big enough.
            char[,] char2 = new char[highestX + 1, highestY + 1];

            //Fill the 2D array with characters
            for (int r = 0; r < rooms.Count; r++)
            {
                Room room = rooms[r];
                int width = room.BoundingBox.Width;
                int height = room.BoundingBox.Height;
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        char c = x == 0 || x == width - 1 || y == 0 || y == height - 1 ? '+' : '-';
                        char2[room.BoundingBox.X + x, room.BoundingBox.Y + y] = c;
                    }
                }
            }

            //Convert 2-dimensional char array to 1-dimensional array with \n charachters
            char[] char1 = new char[(highestX + 1 + 1) * (highestY + 1)];
            for (int y = 0; y < highestY + 1; y++)
            {
                for (int x = 0; x < highestX + 1; x++)
                {
                    char1[y * (highestX + 2) + x] = char2[x, y] != '\0' ? char2[x, y] : '.';
                }
                char1[y * (highestX + 2) + highestX + 1] = '\n';
            }

            return new string(char1);
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

            //Find good points for the hallways to meet the rooms.
            Dictionary<Room, List<Microsoft.Xna.Framework.Point>> roomExitPoints = new Dictionary<Room, List<Microsoft.Xna.Framework.Point>>();
            foreach (Room r in rooms)
            {
                roomExitPoints.Add(r, new List<Microsoft.Xna.Framework.Point>());
            }
            
            foreach (Tuple<Room, Room> pair in hallwayPairs)
            {
                Vector2 center1 = pair.Item1.BoundingBox.Center.ToVector2();
                Vector2 center2 = pair.Item2.BoundingBox.Center.ToVector2();

                //Vector from center of item1 to center of item2
                Vector2 v1 = center2 - center1;
                Vector2 v2 = center1 - center2;

                Collision.Side s1 = CalculateExitSides(pair.Item1, v1);
                Collision.Side s2 = CalculateExitSides(pair.Item2, v2);

                v1.Normalize();
                v2.Normalize();

                Vector2 exit1 = CalculateExit(v1, center1, s1, pair.Item1.BoundingBox.GetMiddleOfSide(s1));
                Vector2 exit2 = CalculateExit(v2, center2, s2, pair.Item2.BoundingBox.GetMiddleOfSide(s2));

                roomExitPoints[pair.Item1].Add(exit1.ToPoint());
                roomExitPoints[pair.Item2].Add(exit2.ToPoint());

                //TODO: Generate hallways here
            }

            TileField tf = new TileField(highestY + 1, highestX + 1, 0, "TileField");
            for (int x = 0; x < tf.Columns; x++)
            {
                for (int y = 0; y < tf.Rows; y++)
                {
                    tf.Add(new Tile(), x, y);
                }
            }
            
            foreach (Room r in rooms)
            {
                int width = r.BoundingBox.Width;
                int height = r.BoundingBox.Height;
                
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        TileType t = TileType.Normal;
                        if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                        {
                            t = TileType.Wall;
                            foreach (Microsoft.Xna.Framework.Point p in roomExitPoints[r])
                            {
                                if (p.X == x && p.Y == y)
                                {
                                    t = TileType.Normal;
                                }
                            }
                        }

                        Tile tile = null;
                        switch (t)
                        {
                            case TileType.Normal:
                                tile = LoadFloorTile("");
                                break;
                            case TileType.Wall:
                                tile = LoadWallTile("");
                                break;
                        }
                        tf.Add(tile, x + r.BoundingBox.X, y + r.BoundingBox.Y);
                    }
                }
            }
            tf.Add(LoadStartTile(), (int)rooms[0].Location.X, (int)rooms[0].Location.Y);
            return tf;
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

        private Collision.Side CalculateExitSides(Room r, Vector2 relVector)
        {
            Vector2 center = r.BoundingBox.Center.ToVector2();

            //Calculate angles of vector.
            double relAngle = Math.Atan2(relVector.Y, relVector.X);

            //Calculate 2 corner angles per room the other 2 are these + pi
            Vector2[] cornerVectors = new Vector2[2];
            cornerVectors[0] = new Vector2(r.BoundingBox.Left, r.BoundingBox.Top) - center;
            cornerVectors[1] = new Vector2(r.BoundingBox.Right, r.BoundingBox.Top) - center;

            double[] cornerAngles = new double[4];
            cornerAngles[0] = Math.Atan2(cornerVectors[0].Y, cornerVectors[0].X);
            cornerAngles[1] = Math.Atan2(cornerVectors[1].Y, cornerVectors[1].X);
            cornerAngles[2] = cornerAngles[0] + Math.PI;
            cornerAngles[3] = cornerAngles[1] + Math.PI;
            
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
    }
}