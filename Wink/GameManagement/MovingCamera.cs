using Microsoft.Xna.Framework;

public class MovingCamera : Camera
{
    /// <summary>
    /// The ID of the gameobject to follow.
    /// </summary>
    private string centerObjectID;

    /// <summary>
    /// The Rectangle in which the camera can move.
    /// </summary>
    Rectangle limitBB;

    public Rectangle CameraLimit { get { return limitBB; } }

    public MovingCamera(string centerObjectID, Rectangle limitBB)
    {
        this.centerObjectID = centerObjectID;
        this.limitBB = limitBB;
    }

    public override void Update(GameTime gameTime)
    {
        if (centerObjectID != null)
        {
            //Set position so that centerObject is in middle and camera does not cross level borders.
            GameObjectList parentList = (GameObjectList)parent;
            GameObject centerObject = parentList.Find(centerObjectID);
            position = centerObject.Position - GameEnvironment.Screen.ToVector2() / 2;

            //Check if new position is out of bounds, if so correct.
            if (position.X < limitBB.Left)
                position.X = limitBB.Left;
            else if (position.X > limitBB.Right - BoundingBox.Width)
                position.X = limitBB.Right - BoundingBox.Width;

            if (position.Y > limitBB.Bottom - BoundingBox.Height)
                position.Y = limitBB.Bottom - BoundingBox.Height;                
            else if (position.Y < limitBB.Top)
                position.Y = limitBB.Top;
        }
    }
}
