using System;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

[Serializable]
public class Animation : SpriteSheet, ISerializable
{
    protected float frameTime;
    protected bool isLooping;
    protected float time;

    public Animation(string assetname, bool isLooping, float frameTime = 0.1f) : base(assetname)
    {
        this.frameTime = frameTime;
        this.isLooping = isLooping;
    }

    public Animation(SerializationInfo info, StreamingContext context) : this(info.GetString("assetname"), info.GetBoolean("isLooping"), (float)info.GetDouble("frameTime"))
    {
    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("assetname", assetName);
        info.AddValue("isLooping", isLooping);
        info.AddValue("frameTime", frameTime);
    }

    public void Play()
    {
        sheetIndex = 0;
        time = 0.0f;
    }

    public void Update(GameTime gameTime)
    {
        time += (float)gameTime.ElapsedGameTime.TotalSeconds;
        while (time > frameTime)
        {
            time -= frameTime;
            if (isLooping)
            {
                sheetIndex = (sheetIndex + 1) % NumberSheetElements;
            }
            else
            {
                sheetIndex = Math.Min(sheetIndex + 1, NumberSheetElements - 1);
            }
        }
    }

    public float FrameTime
    {
        get { return frameTime; }
    }

    public bool IsLooping
    {
        get { return isLooping; }
    }

    public int CountFrames
    {
        get { return NumberSheetElements; }
    }

    public bool AnimationEnded
    {
        get { return !isLooping && sheetIndex >= NumberSheetElements - 1; }
    }
}

