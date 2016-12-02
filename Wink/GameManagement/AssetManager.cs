using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

public class AssetManager
{
    protected ContentManager contentManager;
    protected GraphicsDevice graphicsDevice;
    protected SpriteFont defaultFont;
    
    public AssetManager(ContentManager content, GraphicsDevice graphics)
    {
        contentManager = content;
        graphicsDevice = graphics;
        defaultFont = Content.Load<SpriteFont>("default");
    }

    public Texture2D GetSprite(string assetName)
    {
        if (assetName == "")
        { 
            return null;
        }
        else if (assetName.StartsWith("empty"))
        {
            return GetEmptySprite(assetName);
        }
        return contentManager.Load<Texture2D>(assetName);
    }

    public Texture2D GetEmptySprite(string emptyString)
    {
        //TODO: Create Dictionary so these don't have to be generated everytime.
        int xIndex = emptyString.LastIndexOf('x');
        int width = int.Parse(emptyString.Substring(5, xIndex - 5));
        int height = int.Parse(emptyString.Substring(xIndex+1, emptyString.Length - xIndex - 1));

        Texture2D emptyTexture = new Texture2D(graphicsDevice, width, height);
        Color[] pixels = new Color[width * height];
        for (int y1 = 0; y1 < height; y1++)
        {
            bool pinkStart = y1 % 20 < 10;
            for (int x1 = 0; x1 < width; x1++)
            {
                bool pink = x1 % 20 < 10 ? pinkStart : !pinkStart;
                pixels[y1 * width + x1] = pink ? Color.Magenta : Color.Black;
            }
        }

        emptyTexture.SetData<Color>(pixels);

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