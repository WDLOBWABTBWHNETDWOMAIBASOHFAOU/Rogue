using Microsoft.Xna.Framework;
 

public class Camera : GameObject
{
    public Camera()
    {
        id = "camera";
    }

    //The bounding box of the camera, begins at its position and is the same size as the screen.
    public override Rectangle BoundingBox
    {
        get
        {
            return new Rectangle(GlobalPosition.ToPoint(), GameEnvironment.Screen);
        }
    }
}
