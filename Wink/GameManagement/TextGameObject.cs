using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;
using System;

[Serializable]
public class TextGameObject : GameObject
{
    protected SpriteFont spriteFont;
    protected Color color;
    protected string text;
    protected string fontName;
    public float scale;
    public float CameraSensitivity { get; protected set; }

    public TextGameObject(string fontName, float cameraSensitivity = 1.0f, int layer = 0, string id = "") : base(layer, id)
    {
        spriteFont = GameEnvironment.AssetManager.Content.Load<SpriteFont>(fontName);
        color = Color.White;
        CameraSensitivity = cameraSensitivity;
        text = "";
        scale = 1;
        this.fontName = fontName;
    }

    #region Serialization
    public TextGameObject(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        spriteFont = GameEnvironment.AssetManager.Content.Load<SpriteFont>(info.GetString("fontName"));
        color = new Color((float)info.GetDouble("color.R"), (float)info.GetDouble("color.G"), (float)info.GetDouble("color.B"), (float)info.GetDouble("color.A"));
        text = info.GetString("text");
        CameraSensitivity = (float)info.GetDouble("CameraSensitivity");
        scale = (float)info.GetDouble("scale");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("scale", scale);
        info.AddValue("fontName", fontName);
        info.AddValue("color.R", color.R);
        info.AddValue("color.G", color.G);
        info.AddValue("color.B", color.B);
        info.AddValue("color.A", color.A);
        info.AddValue("text", text);
        info.AddValue("CameraSensitivity", CameraSensitivity);
    }
    #endregion

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
    {
        if (visible)
        {
            spriteBatch.DrawString(spriteFont, Text, GlobalPosition - (CameraSensitivity * camera.GlobalPosition), color,0,Vector2.Zero,scale,SpriteEffects.None,0);
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