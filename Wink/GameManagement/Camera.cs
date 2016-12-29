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
        float realSpeed = cameraMoveSpeed * (inputHelper.IsKeyDown(Keys.LeftShift) ? 2.5f : 1);
        if (inputHelper.IsKeyDown(Keys.W))
            Position += new Vector2(0, -realSpeed);

        if (inputHelper.IsKeyDown(Keys.A))
            Position += new Vector2(-realSpeed, 0);

        if (inputHelper.IsKeyDown(Keys.S))
            Position += new Vector2(0, realSpeed);

        if (inputHelper.IsKeyDown(Keys.D))
            Position += new Vector2(realSpeed, 0);
    }

    public Vector2 CalculateScreenPosition(SpriteGameObject go)
    {
        if (go != null)
        {
            return go.GlobalPosition - (go.CameraSensitivity * GlobalPosition);
        }
        else
        {
            throw new System.NullReferenceException();
        }
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
