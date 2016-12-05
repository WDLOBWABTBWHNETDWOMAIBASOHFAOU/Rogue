using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

public class AssetManager
{
    protected ContentManager contentManager;
    protected GraphicsDevice graphicsDevice;
    protected SpriteFont defaultFont;

    protected Dictionary<string, Texture2D> generatedTextures;

    public AssetManager(ContentManager content, GraphicsDevice graphics)
    {
        contentManager = content;
        graphicsDevice = graphics;
        defaultFont = Content.Load<SpriteFont>("default");
        generatedTextures = new Dictionary<string, Texture2D>();
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
        if (generatedTextures.ContainsKey(emptyString))
        {
            return generatedTextures[emptyString];
        }
        else
        {
            string[] parts = emptyString.Split(':');
            int width = int.Parse(parts[1]);
            int height = int.Parse(parts[2]);
            int size = int.Parse(parts[3]);
            Color color = Color.Black;
            color = (Color)color.GetType().GetProperty(parts[4]).GetValue(null);

            Texture2D emptyTexture = new Texture2D(graphicsDevice, width, height);
            Color[] pixels = new Color[width * height];
            for (int y1 = 0; y1 < height; y1++)
            {
                bool pinkStart = y1 % (2*size) < size;
                for (int x1 = 0; x1 < width; x1++)
                {
                    bool pink = x1 % (2*size) < size ? pinkStart : !pinkStart;
                    pixels[y1 * width + x1] = pink ? color : Color.Black;
                }
            }
            emptyTexture.SetData<Color>(pixels);

            generatedTextures.Add(emptyString, emptyTexture);
            return emptyTexture;
        }
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