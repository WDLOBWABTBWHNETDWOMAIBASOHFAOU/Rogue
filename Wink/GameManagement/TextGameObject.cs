using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class TextGameObject : GameObject
{
    protected SpriteFont spriteFont;
    protected Color color;
    protected string text;
    public float CameraSensitivity { get; protected set; }

    public TextGameObject(string fontName, float cameraSensitivity = 1.0f, int layer = 0, string id = "") : base(layer, id)
    {
        spriteFont = GameEnvironment.AssetManager.Content.Load<SpriteFont>(fontName);
        color = Color.White;
        CameraSensitivity = cameraSensitivity;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
    {
        if (visible)
        {
            spriteBatch.DrawString(spriteFont, Text, GlobalPosition - (CameraSensitivity * camera.GlobalPosition), color);
        }
    }

    public Color Color
    {
        get { return color; }
        set { color = value; }
    }

    public virtual string Text
    {
        get { return text; }
        set { text = value; }
    }

    public Vector2 Size
    {
        get { return spriteFont.MeasureString(text); }
    }
}