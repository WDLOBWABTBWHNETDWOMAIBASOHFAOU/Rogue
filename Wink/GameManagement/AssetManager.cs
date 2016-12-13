using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

public class AssetManager
{
    protected ContentManager contentManager;
    protected GraphicsDevice graphicsDevice;
    protected SpriteFont defaultFont;

    protected Dictionary<string, Texture2D> textures;

    public AssetManager(ContentManager content, GraphicsDevice graphics)
    {
        contentManager = content;
        graphicsDevice = graphics;
        textures = new Dictionary<string, Texture2D>();
    }

    /// <summary>
    /// Get a 1x1 Texture2D that has the specified color.
    /// </summary>
    public Texture2D GetSingleColorPixel(Color color)
    {
        Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData<Color>(new Color[] { color });
        return texture;
    }

    public SpriteFont GetFont(string assetName)
    {
        return contentManager.Load<SpriteFont>(assetName);
    }

    public virtual Texture2D GetSprite(string assetName)
    {
        if (assetName == "" || assetName == null)
        { 
            return null;
        }
        else if (textures.ContainsKey(assetName))
        {
            return textures[assetName];
        }
        else if (assetName.StartsWith("empty"))
        {
            return GetEmptySprite(assetName);
        }
        else
        {
            textures.Add(assetName, contentManager.Load<Texture2D>(assetName));
            return textures[assetName];
        }
    }

    public virtual Texture2D GetEmptySprite(string emptyString)
    {
        //Split up the string and get the parameters
        string[] parts = emptyString.Split(':');
        int width = int.Parse(parts[1]);
        int height = int.Parse(parts[2]);
        int size = int.Parse(parts[3]);

        //Use Reflection to get the specified Color Property.
        Color color = Color.Black;
        color = (Color)color.GetType().GetProperty(parts[4]).GetValue(null);

        //New empty texture
        Texture2D emptyTexture = new Texture2D(graphicsDevice, width, height);
        Color[] pixels = new Color[width * height];
        for (int y1 = 0; y1 < height; y1++)
        {
            //Whether or not to start with a colored square.
            bool pinkStart = y1 % (2*size) < size;
            for (int x1 = 0; x1 < width; x1++)
            {
                //Whether or not this square is colored or black.
                bool pink = x1 % (2*size) < size ? pinkStart : !pinkStart;
                pixels[y1 * width + x1] = pink ? color : Color.Black;
            }
        }
        emptyTexture.SetData<Color>(pixels);

        textures.Add(emptyString, emptyTexture);
        return emptyTexture;
    }

    public virtual void PlaySound(string assetName)
    {
        SoundEffect snd = contentManager.Load<SoundEffect>(assetName);
        snd.Play();
    }

    public virtual void PlayMusic(string assetName, bool repeat = true)
    {
        MediaPlayer.IsRepeating = repeat;
        MediaPlayer.Play(contentManager.Load<Song>(assetName));
    }

    public ContentManager Content
    {
        get { return contentManager; }
    }
}

public class EmptyAssetManager : AssetManager
{
    public EmptyAssetManager() : base(null, null)
    {
    }

    public override Texture2D GetSprite(string assetName)
    {
        return null;
    }

    public override Texture2D GetEmptySprite(string emptyString)
    {
        return null;
    }

    public override void PlaySound(string assetName)
    {
    }

    public override void PlayMusic(string assetName, bool repeat = true)
    {
    }
}