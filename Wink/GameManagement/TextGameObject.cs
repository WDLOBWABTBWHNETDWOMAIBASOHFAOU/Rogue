using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

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

    public TextGameObject(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        spriteFont = info.GetValue("spriteFont", typeof(SpriteFont)) as SpriteFont;
        color = new Color((float)info.GetDouble("color.R"), (float)info.GetDouble("color.G"), (float)info.GetDouble("color.B"), (float)info.GetDouble("color.A"));
        text = info.GetString("text");
        CameraSensitivity = (float)info.GetDouble("CameraSensitivity");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);

        info.AddValue("spriteFont", spriteFont);
        info.AddValue("color.R", color.R);
        info.AddValue("color.G", color.G);
        info.AddValue("color.B", color.B);
        info.AddValue("color.A", color.A);
        info.AddValue("text", text);
        info.AddValue("CameraSensitivity", CameraSensitivity);
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