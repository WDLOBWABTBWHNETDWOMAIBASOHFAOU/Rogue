using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;

[Serializable]
public class GameObjectList : GameObject, IGameObjectContainer
{
    protected List<GameObject> children;

    [NonSerialized]
    protected List<GameObject> toRemove;

    public GameObjectList(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        children = (List<GameObject>)info.GetValue("children", typeof(List<GameObject>));
    }

    public GameObjectList(int layer = 0, string id = "") : base(layer, id)
    {
        children = new List<GameObject>();
        toRemove = new List<GameObject>();
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("children", children);
        base.GetObjectData(info, context);
    }

    public List<GameObject> Children
    {
        get { return children; }
    }

    public virtual void Add(GameObject obj)
    {
        obj.Parent = this;
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i].Layer > obj.Layer)
            {
                children.Insert(i, obj);
                return;
            }
        }
        children.Add(obj);
    }

    public void Remove(GameObject obj)
    {
        toRemove.Add(obj);

        if (obj.Parent == this)
            obj.Parent = null;
    }

    public List<GameObject> FindAll(Func<GameObject, bool> del)
    {
        List<GameObject> result = new List<GameObject>();

        foreach (GameObject obj in children)
        {
            if (obj != null)
            {
                if (del.Invoke(obj))
                {
                    result.Add(obj);
                }
                if (obj is IGameObjectContainer)
                {
                    IGameObjectContainer objContainer = obj as IGameObjectContainer;
                    result.AddRange(objContainer.FindAll(del));
                }
            }
        }
        return result;
    }

    public GameObject Find(Func<GameObject, bool> del)
    {
        foreach (GameObject obj in children)
        {
            if (obj != null)
            {
                if (del.Invoke(obj))
                {
                    return obj;
                }
                else if (obj is IGameObjectContainer)
                {
                    IGameObjectContainer objContainer = obj as IGameObjectContainer;
                    GameObject subObj = objContainer.Find(del);
                    if (subObj != null)
                    {
                        return subObj;
                    }
                }
            }
        }
        return null;
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        for (int i = children.Count - 1; i >= 0; i--)
        {
            children[i].HandleInput(inputHelper);
        }
    }

    public override void Update(GameTime gameTime)
    {
        foreach (GameObject obj in toRemove)
        {
            children.Remove(obj);
        }

        toRemove.Clear();

        foreach (GameObject obj  in children)
        {
            obj.Update(gameTime);
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
    {
        if (!visible)
        {
            return;
        }
        List<GameObject>.Enumerator e = children.GetEnumerator();
        while (e.MoveNext())
        {
            e.Current.Draw(gameTime, spriteBatch, camera);
        }
    }

    public override void DrawDebug(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
    {
        if (!visible)
        {
            return;
        }
        List<GameObject>.Enumerator e = children.GetEnumerator();
        while (e.MoveNext())
        {
            e.Current.DrawDebug(gameTime, spriteBatch, camera);
        }
    }

    public override void Reset()
    {
        base.Reset();
        foreach (GameObject obj in children)
        {
            obj.Reset();
        }
    }
    
    public override Rectangle BoundingBox
    {
        get
        {
            Rectangle bb = new Rectangle();
            foreach(GameObject go in Children)
            {
                bb = Rectangle.Union(bb, go.BoundingBox);
            }
            return bb;
        }
    }
}
