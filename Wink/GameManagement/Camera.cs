using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class Camera : GameObject
{
    public Camera()
    {
        id = "camera";
    }
    
    private const int cameraMoveSpeed = 4;

    public override void HandleInput(InputHelper inputHelper)
    {
        if (inputHelper.IsKeyDown(Keys.W))
            Position += new Vector2(0, -cameraMoveSpeed);

        if (inputHelper.IsKeyDown(Keys.A))
            Position += new Vector2(-cameraMoveSpeed, 0);

        if (inputHelper.IsKeyDown(Keys.S))
            Position += new Vector2(0, cameraMoveSpeed);

        if (inputHelper.IsKeyDown(Keys.D))
            Position += new Vector2(cameraMoveSpeed, 0);
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
