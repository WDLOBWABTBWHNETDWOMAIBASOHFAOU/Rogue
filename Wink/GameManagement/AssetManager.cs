using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.IO;

public class AssetManager
{
    protected ContentManager contentManager;
    protected GraphicsDevice graphicsDevice;
    protected SpriteBatch spriteBatch;
    protected SpriteFont defaultFont;

    protected Dictionary<string, Texture2D> textures;
    protected Dictionary<string, SpriteFont> fonts;

    public AssetManager(ContentManager content, GraphicsDevice graphics, SpriteBatch spriteBatch)
    {
        contentManager = content;
        graphicsDevice = graphics;
        this.spriteBatch = spriteBatch;
        textures = new Dictionary<string, Texture2D>();
        fonts = new Dictionary<string, SpriteFont>();
    }

    /// <summary>
    /// Get a 1x1 Texture2D that has the specified color.
    /// </summary>
    public Texture2D GetSingleColorPixel(Color color)
    {
        string key = "singleColor:" + color.ToString();
        if (!textures.ContainsKey(key))
        {
            Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData<Color>(new Color[] { color });
            textures.Add(key, texture);
        }
        return textures[key];
    }

    public SpriteFont GetFont(string assetName)
    {
        if (!fonts.ContainsKey(assetName))
        {
            fonts.Add(assetName, contentManager.Load<SpriteFont>(assetName));
        }
        return fonts[assetName];
    }

    public Texture2D GetSprite(string assetName)
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
        else if (assetName.StartsWith("*"))
        {
            string name = assetName.Substring(1);
            textures.Add(assetName, GetTransparentTallSprite(name, GetSprite(name)));
            return textures[assetName];
        }
        else
        {
            textures.Add(assetName, contentManager.Load<Texture2D>(assetName));
            return textures[assetName];
        }
    }

    /// <summary>
    /// This method is used to add transparency to textures that are taller than one tile, e.g. Walls.
    /// It works by rendering the texture to a rendertarget and then applying a mask.
    /// </summary>
    /// <returns></returns>
    public Texture2D GetTransparentTallSprite(string assetName, Texture2D asset)
    {
        int columns = 1;
        int rows = 1;

        if (assetName.Contains("@"))
        {
            string[] parts = assetName.Split('@')[1].Split('x');
            rows = int.Parse(parts[1]);
            columns = int.Parse(parts[0]);
        }

        PresentationParameters pp = graphicsDevice.PresentationParameters;
        RenderTarget2D maskRenderTarget = new RenderTarget2D(graphicsDevice, asset.Width, asset.Height, false, SurfaceFormat.Color, pp.DepthStencilFormat);

        graphicsDevice.SetRenderTarget(maskRenderTarget);
        graphicsDevice.Clear(Color.Transparent);

        spriteBatch.Begin();
        spriteBatch.Draw(asset, new Vector2(0, 0), Color.White);
        spriteBatch.End();

        BlendState bs = new BlendState();
        bs.AlphaBlendFunction = BlendFunction.Add;
        bs.AlphaDestinationBlend = Blend.Zero;
        bs.AlphaSourceBlend = Blend.One;
        bs.ColorWriteChannels = ColorWriteChannels.Alpha;

        spriteBatch.Begin(SpriteSortMode.Immediate, bs);
        int rowHeight = asset.Height / rows;
        int columnWidth = asset.Width / columns;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                spriteBatch.Draw(GetSingleColorPixel(new Color(0, 0, 0, 160)), new Rectangle(c * columnWidth, r * rowHeight + 4, columnWidth - 4, rowHeight - 4 - Wink.Tile.TileHeight), Color.White);
            }
        }
        spriteBatch.End();
        
        graphicsDevice.SetRenderTarget(null);
        maskRenderTarget.SaveAsPng(File.Create("transparent_texture.png"), asset.Width, asset.Height);
        return maskRenderTarget;
    }

    public Texture2D GetEmptySprite(string emptyString)
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

    public void PlaySound(string assetName)
    {
        SoundEffect snd = contentManager.Load<SoundEffect>(assetName);
        snd.Play();
    }

    public void PlayMusic(string assetName, bool repeat = true)
    {
        MediaPlayer.IsRepeating = repeat;
        MediaPlayer.Play(contentManager.Load<Song>(assetName));
    }

    public ContentManager Content
    {
        get { return contentManager; }
    }
}