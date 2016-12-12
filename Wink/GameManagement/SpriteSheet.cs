using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class SpriteSheet
{
    protected Texture2D sprite;
    protected bool[] collisionMask;
    protected int sheetIndex;
    protected int sheetColumns;
    protected int sheetRows;
    protected bool mirror;

    public SpriteSheet(string assetname, int sheetIndex = 0)
    {
        // retrieve the sprite
        sprite = GameEnvironment.AssetManager.GetSprite(assetname);

        if (sprite == null)
            return;

        // construct the collision mask
        Color[] colorData = new Color[sprite.Width * sprite.Height];
        collisionMask = new bool[sprite.Width * sprite.Height];
        sprite.GetData(colorData);
        for (int i = 0; i < colorData.Length; ++i)
        {
            collisionMask[i] = colorData[i].A != 0;
        }

        this.sheetIndex = sheetIndex;
        sheetColumns = 1;
        sheetRows = 1;

        // see if we can extract the number of sheet elements from the assetname
        string[] assetSplit = assetname.Split('@');
        if (assetSplit.Length <= 1)
        {
            return;
        }

        string sheetNrData = assetSplit[assetSplit.Length - 1];
        string[] colRow = sheetNrData.Split('x');
        sheetColumns = int.Parse(colRow[0]);
        if (colRow.Length == 2)
        {
            sheetRows = int.Parse(colRow[1]);
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 origin,float scale)
    {
        int columnIndex = sheetIndex % sheetColumns;
        int rowIndex = sheetIndex / sheetColumns % sheetRows;
        Rectangle spritePart = new Rectangle(columnIndex * Width, rowIndex * Height, Width, Height);
        SpriteEffects spriteEffects = SpriteEffects.None;
        if (mirror)
        {
            spriteEffects = SpriteEffects.FlipHorizontally;
        }
        spriteBatch.Draw(sprite, position, spritePart, Color.White,
            0.0f, origin, scale, spriteEffects, 0.0f);
    }
    
    public bool IsTranslucent(int x, int y)
    {
        int column_index = sheetIndex % sheetColumns;
        int row_index = sheetIndex / sheetColumns % sheetRows;

        return collisionMask[column_index * Width + x + (row_index * Height + y) * sprite.Width];
    }

    public Texture2D Sprite
    {
        get { return sprite; }
    }

    public Vector2 Center
    {
        get { return new Vector2(Width, Height) / 2; }
    }

    public int Width
    {
        get
        { return sprite.Width / sheetColumns; }
    }

    public int Height
    {
        get
        { return sprite.Height / sheetRows; }
    }

    public bool Mirror
    {
        get { return mirror; }
        set { mirror = value; }
    }

    public int SheetIndex
    {
        get
        { return sheetIndex; }
        set
        {
            if (value < sheetColumns * sheetRows && value >= 0)
            {
                sheetIndex = value;
            }
        }
    }

    public int NumberSheetElements
    {
        get { return sheetColumns * sheetRows; }
    }
}