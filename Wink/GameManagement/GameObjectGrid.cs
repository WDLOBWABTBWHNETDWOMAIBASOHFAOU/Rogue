using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class GameObjectGrid : GameObject
{
    protected GameObject[,] grid;
    protected int cellWidth, cellHeight;

    public GameObjectGrid(int rows, int columns, int layer = 0, string id = "") : base(layer, id)
    {
        grid = new GameObject[columns, rows];
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                grid[x, y] = null;
            }
        }
    }

    public GameObjectGrid(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        grid = (GameObject[,])info.GetValue("grid", typeof(GameObject[,]));
        cellWidth = info.GetInt32("cellWidth");
        cellHeight = info.GetInt32("cellHeight");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("grid", grid);
        info.AddValue("cellWidth", cellWidth);
        info.AddValue("cellHeight", cellHeight);
        base.GetObjectData(info, context);
    }

    public void Add(GameObject obj, int x, int y)
    {
        if (obj != null)
        {
        grid[x, y] = obj;
        obj.Parent = this;
        obj.Position = new Vector2(x * cellWidth, y * cellHeight);
        }
    }

    public GameObject this[int x, int y]
    {
        get
        {
            return Get(x, y);
        }
    }

    public GameObject Get(int x, int y)
    {
        if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
        {
            return grid[x, y];
        }
        else
        {
            return null;
        }
    }
    
    public GameObject[,] Objects
    {
        get
        {
            return grid;
        }
    }

    public Vector2 GetAnchorPosition(GameObject s)
    {
        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                if (grid[x, y] == s)
                {
                    return new Vector2(x * cellWidth, y * cellHeight);
                }
            }
        }
        return Vector2.Zero;
    }
    
    public int Rows
    {
        get { return grid.GetLength(1); }
    }
    
    public int Columns
    {
        get { return grid.GetLength(0); }
    }
    
    public int CellWidth
    {
        get { return cellWidth; }
        set { cellWidth = value; }
    }
    
    public int CellHeight
    {
        get { return cellHeight; }
        set { cellHeight = value; }
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        foreach (GameObject obj in grid)
        {
            if (obj != null)
            {
                obj.HandleInput(inputHelper);
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        foreach (GameObject obj in grid)
        {
            if(obj !=null)
            obj.Update(gameTime);
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
    {
        foreach (GameObject obj in grid)
        {
            if (obj != null)
            {
            obj.Draw(gameTime, spriteBatch, camera);
            }
        }
    }

    public override void Reset()
    {
        base.Reset();
        foreach (GameObject obj in grid)
        {
            obj.Reset();
        }
    }

    public GameObject Find(Func<GameObject, bool> del)
    {
        foreach (GameObject obj in grid)
        {
            if(obj != null)
            {
                if (del.Invoke(obj))
                {
                    return obj;
                }
            }
        }
        return null;
    }

    public List<GameObject> FindAll(Func<GameObject, bool> del)
    {
        List<GameObject> result = new List<GameObject>();
        foreach (GameObject obj in grid)
        {
            if (del.Invoke(obj))
            {
                result.Add(obj);
            }
            if (obj is GameObjectList)
            {
                GameObjectList objList = obj as GameObjectList;
                result.AddRange(objList.FindAll(del));
            }
            if (obj is GameObjectGrid)
            {
                GameObjectGrid objGrid = obj as GameObjectGrid;
                result.AddRange(objGrid.FindAll(del));
            }
        }
        return result;
    }

    public override Rectangle BoundingBox
    {
        get
        {
            return new Rectangle((int)GlobalPosition.X, (int)GlobalPosition.Y, Columns*CellWidth, Rows*CellHeight);
        }
    }
}
