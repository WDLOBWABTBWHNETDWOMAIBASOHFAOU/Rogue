using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class SpriteGameObject : GameObject
{
    protected SpriteSheet sprite;
    protected string spriteAssetName;
    protected int spriteSheetIndex;

    protected Vector2 origin;
    protected float scale;
    protected float cameraSensitivity;

    public bool PerPixelCollisionDetection = true;

    protected Dictionary<string, string> debugTags;

    public Color DrawColor { get; set; }
    public float CameraSensitivity { get { return cameraSensitivity; } }

    public SpriteGameObject(string assetName, int layer = 0, string id = "", int sheetIndex = 0, float cameraSensitivity = 1.0f, float scale = 1.0f) : base(layer, id)
    {
        this.DrawColor = Color.White;
        this.cameraSensitivity = cameraSensitivity;
        this.scale = scale;
        this.spriteAssetName = assetName;
        this.spriteSheetIndex = sheetIndex;
        this.debugTags = new Dictionary<string, string>();
        LoadSprite();
    }

    public SpriteGameObject(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        spriteAssetName = info.GetString("spriteAssetName");
        spriteSheetIndex = info.GetInt32("spriteSheetIndex");
        origin = new Vector2((float)info.GetDouble("originX"), (float)info.GetDouble("originY"));
        scale = (float)info.GetDouble("scale");
        cameraSensitivity = (float)info.GetDouble("cameraSensitivity");

        debugTags = (Dictionary<string, string>)info.GetValue("debugTags", typeof(Dictionary<string, string>));
        LoadSprite();
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("spriteAssetName", spriteAssetName);
        info.AddValue("spriteSheetIndex", spriteSheetIndex);
        info.AddValue("originX", origin.X);
        info.AddValue("originY", origin.Y);
        info.AddValue("scale", scale);
        info.AddValue("cameraSensitivity", cameraSensitivity);
        info.AddValue("debugTags", debugTags);
        base.GetObjectData(info, context);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
    {
        if (!visible || sprite == null)
        {
            return;
        }
        //Draw every SpriteGameObject in its position relative to the camera but only to the extent specified by the CameraSensitivity property.
        sprite.Draw(spriteBatch, GlobalPosition - (cameraSensitivity * camera.GlobalPosition), origin, scale, DrawColor);
    }

    public override void DrawDebug(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
    {
        Dictionary<string, string>.Enumerator enumerator = debugTags.GetEnumerator();
        while (enumerator.MoveNext())
        {
            SpriteFont arial12 = GameEnvironment.AssetManager.GetFont("Arial12");
            spriteBatch.DrawString(arial12, enumerator.Current.Key + ": " + enumerator.Current.Value, camera.CalculateScreenPosition(this), Color.White);
        }
    }

    public void AddDebugTag(string key, string value)
    {
        debugTags.Add(key, value);
    }

    public void LoadSprite()
    {
        if (spriteAssetName != "")
        {
            sprite = new SpriteSheet(spriteAssetName, spriteSheetIndex);
        }
        else
        {
            sprite = null;
        }
    }

    public SpriteSheet Sprite
    {
        get { return sprite; }
    }

    public Vector2 Center
    {
        get { return new Vector2(Width, Height) / 2; }
    }

    public virtual int Width
    {
        get
        {
            return (int)(sprite.Width * scale);
        }
    }

    public virtual int Height
    {
        get
        {
            return (int)(sprite.Height * scale);
        }
    }

    public bool Mirror
    {
        get { return sprite.Mirror; }
        set { sprite.Mirror = value; }
    }

    public Vector2 Origin
    {
        get { return origin; }
        set { origin = value; }
    }

    public override Rectangle BoundingBox
    {
        get
        {
            int left = (int)(GlobalPosition.X - origin.X * scale);
            int top = (int)(GlobalPosition.Y - origin.Y * scale);
            return new Rectangle(left, top, Width, Height);
        }
    }

    public bool CollidesWith(SpriteGameObject obj)
    {
        if (!visible || !obj.visible || !BoundingBox.Intersects(obj.BoundingBox))
        {
            return false;
        }
        if (!PerPixelCollisionDetection)
        {
            return true;
        }
        Rectangle b = Collision.Intersection(BoundingBox, obj.BoundingBox);
        for (int x = 0; x < b.Width; x++)
        {
            for (int y = 0; y < b.Height; y++)
            {
                int thisx = b.X - (int)(GlobalPosition.X - origin.X) + x;
                int thisy = b.Y - (int)(GlobalPosition.Y - origin.Y) + y;
                int objx = b.X - (int)(obj.GlobalPosition.X - obj.origin.X) + x;
                int objy = b.Y - (int)(obj.GlobalPosition.Y - obj.origin.Y) + y;
                if (sprite.IsTranslucent(thisx, thisy) && obj.sprite.IsTranslucent(objx, objy))
                {
                    return true;
                }
            }
        }
        return false;
    }
}

