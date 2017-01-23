using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Wink;

[Serializable]
public abstract class GameObject : IGameLoopObject, ISerializable
{
    protected GameObject parent;
    protected Vector2 position, velocity;
    protected int layer;
    protected string id;
    protected bool visible;

    protected Guid guid;

    public Guid GUID
    {
        get { return guid; }
    }

    public GameObject(int layer = 0, string id = "")
    {
        this.layer = layer;
        this.id = id;
        position = Vector2.Zero;
        velocity = Vector2.Zero; 
        visible = true;

        guid = Guid.NewGuid();
    }

    #region Serialization
    protected GameObject(SerializationInfo info, StreamingContext context)
    {
        parent = info.TryGUIDThenFull<GameObject>(context, "parent");

        position = new Vector2((float)info.GetDouble("posX"), (float)info.GetDouble("posY"));
        velocity = new Vector2((float)info.GetDouble("velX"), (float)info.GetDouble("velY"));
        layer = info.GetInt32("layer");
        id = info.GetString("id");
        visible = info.GetBoolean("visible");
        guid = (Guid)info.GetValue("guid", typeof(Guid));
    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        SerializationHelper.Variables v = context.GetVars();
        if (parent == null || v.FullySerializeEverything || v.FullySerialized.Contains(parent.GUID))
            info.AddValue("parent", parent);
        else
            info.AddValue("parentGUID", parent.GUID.ToString());
        
        info.AddValue("posX", position.X);
        info.AddValue("posY", position.Y);
        info.AddValue("velX", velocity.X);
        info.AddValue("velY", velocity.Y);
        info.AddValue("layer", layer);
        info.AddValue("id", id);
        info.AddValue("visible", visible);
        info.AddValue("guid", guid);
    }
    #endregion

    public virtual void HandleInput(InputHelper inputHelper)
    {
    }

    /// <summary>
    /// Updates the gameobject, this is typically performed once every frame
    /// </summary>
    /// <param name="gameTime">The current gameTime variable</param>
    public virtual void Update(GameTime gameTime)
    {
        position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
    {
    }

    public virtual void DrawDebug(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
    {
    }

    public virtual void Replace(GameObject go)
    {
        if (parent != null && parent.GUID == go.GUID)
        {
            parent = go;
        }
    }

    public virtual void Reset()
    {
        visible = true;
    }

    public virtual Vector2 Position
    {
        get { return position; }
        set { position = value; }
    }
    
    public virtual Vector2 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }
    
    public virtual Vector2 GlobalPosition
    {
        get
        {
            if (parent != null)
            {
                return parent.GlobalPosition + Position;
            }
            else
            {
                return Position;
            }
        }
    }
    
    public GameObject Root
    {
        get
        {
            if (parent != null)
            {
                return parent.Root;
            }
            else
            {
                return this;
            }
        }
    }
    
    public GameObjectList GameWorld
    {
        get
        {
            return Root as GameObjectList;
        }
    }
    
    public virtual int Layer
    {
        get { return layer; }
        set { layer = value; }
    }
    
    public virtual GameObject Parent
    {
        get { return parent; }
        set { parent = value; }
    }
    
    public string Id
    {
        get { return id; }
    }
    
    public virtual bool Visible
    {
        get { return visible; }
        set { visible = value; }
    }
    
    public virtual Rectangle BoundingBox
    {
        get
        {
            return new Rectangle((int)GlobalPosition.X, (int)GlobalPosition.Y, 0, 0);
        }
    }
}