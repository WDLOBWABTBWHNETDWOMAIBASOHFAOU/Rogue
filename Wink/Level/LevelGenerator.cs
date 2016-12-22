using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Wink
{
    public partial class Level
    {
        private class Room
        {
            public Room(Vector2 location, Point size)
            {
                Location = location;
                Size = size;
            }

            public Vector2 Location { get; set; }
            public Point Size { get; set; }

            //public Vector2 Velocity { get; set; }
            public Rectangle BoundingBox
            {
                get
                {
                    return new Rectangle(Location.ToPoint(), Size);
                }
            }
        }

        private Random random = new Random();
        Random Random { get { return random; } }
        
        const double circleRadius = 4;
        
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

        int NthTriangleNumber(int n)
        {
            return n * (n + 1) / 2;
        }

        const double maxCircleToRoomsAreaRatio = 20;

        private Level Generate()
        {
            //This double will have the average total area of a random collection of rooms.
            double averageTotalArea = 0;
            for (Vector2 v = new Vector2(4, 4); v.X <= 10; v.X++)
            {
                for (; v.Y <= 10; v.Y++)
                {
                    averageTotalArea += GaussianDistribution(v) * v.X * v.Y;
                }
            }

            //double totalRoomArea = Math.Pow(NthTriangleNumber(maxDim) - NthTriangleNumber(minDim - 1), 2);
            double multiplier = CircleArea / averageTotalArea;

            List<Room> allRooms = new List<Room>();
            for (int x = 4; x <= 10; x++)
            {
                for (int y = 4; y <= 10; y++)
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
            while (totalRoomArea < maxCircleToRoomsAreaRatio * CircleArea)
            {
                int randomIndex = Random.Next(allRooms.Count);
                Room toAdd = allRooms[randomIndex];
                Vector2 pointInCircle = GetRandomPointInCircle();
                toAdd.Location = pointInCircle - toAdd.Size.ToVector2() / 2;
                roomSelection.Add(toAdd);

                totalRoomArea += toAdd.Size.X * toAdd.Size.Y;
            }

            int collisions = int.MaxValue;
            while (collisions > 0)
            {
                collisions = 0;
                for (int i = 0; i < roomSelection.Count; i++)
                {
                    Room r1 = roomSelection[i];
                    for (int j = i + 1; j < roomSelection.Count; j++)
                    {
                        Room r2 = roomSelection[j];
                        if (r1 != r2 && r1.BoundingBox.Intersects(r2.BoundingBox))
                        {
                            Rectangle intersection = Rectangle.Intersect(r1.BoundingBox, r2.BoundingBox);
                            r1.Location += (r1.Location + r1.Size.ToVector2() / 2 - intersection.Center.ToVector2()) / 8;
                            r2.Location += (r2.Location + r2.Size.ToVector2() / 2 - intersection.Center.ToVector2()) / 8;
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
            
            //Get the highest coordinates so we know the size of the level.
            int highestX = int.MinValue;
            int highestY = int.MinValue;
            foreach (Room r in roomSelection)
            {
                r.Location -= new Vector2(lowestX, lowestY);
                if (r.BoundingBox.X > highestX)
                    highestX = r.BoundingBox.Right;

                if (r.BoundingBox.Y > highestY)
                    highestY = r.BoundingBox.Bottom;
            }
            
            char[,] char2 = new char[highestX + 1, highestY + 1];

            foreach (Room r in roomSelection)
            {
                int amount = 2 * (r.BoundingBox.Width + r.BoundingBox.Height) - 4;
                for (int i = 0; i < amount; i++)
                {
                    int x =  i > 2 * (r.BoundingBox.Width - 1) ? (i % 2) * (r.BoundingBox.Width - 1) : i % (r.BoundingBox.Width - 1);
                    int y =  i > 2 * (r.BoundingBox.Width - 1) ? i % (r.BoundingBox.Height - 1) : (i % 2) * (r.BoundingBox.Height - 1);
                    char2[r.BoundingBox.X + x, r.BoundingBox.Y + y] = 'W';
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

            string test = new string(char1);

            return null;
        }
    }
}
