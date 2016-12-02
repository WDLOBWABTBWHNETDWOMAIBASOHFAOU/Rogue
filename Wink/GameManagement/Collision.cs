using Microsoft.Xna.Framework;
using System;

public class Collision
{
    public enum CollisionSide { Top, Right, Bottom, Left }

    /// <summary>
    /// Gives the side on rectA that was collided with.
    /// </summary>
    /// <param name="rectA"></param>
    /// <param name="rectB"></param>
    /// <returns></returns>
    public static CollisionSide CalculateCollisionSide(Rectangle rectA, Rectangle rectB)
    {
        Rectangle intersection = Rectangle.Intersect(rectA, rectB);

        float w = 0.5f * (rectA.Width + rectB.Width);
        float h = 0.5f * (rectA.Height + rectB.Height);
        float dx = rectA.Center.X - rectB.Center.X;
        float dy = rectA.Center.Y - rectB.Center.Y;

        float wy = w * dy;
        float hx = h * dx;

        CollisionSide? s = null;

        if (wy > hx)
        {
            if (wy > -hx)
                s = CollisionSide.Top;
            else
                s = CollisionSide.Right;
        }
        else
        {
            if (wy > -hx)
                s = CollisionSide.Left;
            else
                s = CollisionSide.Bottom;
        }

        return s.Value;
    }

    public static Vector2 CalculateIntersectionDepth(Rectangle rectA, Rectangle rectB)
    {
        Vector2 minDistance = new Vector2(rectA.Width + rectB.Width, rectA.Height + rectB.Height) / 2; 
        Vector2 centerA = rectA.Center.ToVector2();
        Vector2 centerB = rectB.Center.ToVector2();
        Vector2 distance = centerA - centerB;
        Vector2 depth = Vector2.Zero;

        if (distance.X > 0)
            depth.X = minDistance.X - distance.X;
        else
            depth.X = -minDistance.X - distance.X;

        if (distance.Y > 0)
            depth.Y = minDistance.Y - distance.Y;
        else
            depth.Y = -minDistance.Y - distance.Y;

        return depth;
    }

    public static Rectangle Intersection(Rectangle rect1, Rectangle rect2)
    {
        int xmin = (int)MathHelper.Max(rect1.Left, rect2.Left);
        int xmax = (int)MathHelper.Min(rect1.Right, rect2.Right);
        int ymin = (int)MathHelper.Max(rect1.Top, rect2.Top);
        int ymax = (int)MathHelper.Min(rect1.Bottom, rect2.Bottom);
        return new Rectangle(xmin, ymin, xmax - xmin, ymax - ymin);
    }
}

