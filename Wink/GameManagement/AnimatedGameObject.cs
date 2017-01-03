using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

public class AnimatedGameObject : SpriteGameObject
{
    protected Dictionary<string, Animation> animations;

    public AnimatedGameObject(int layer = 0, string id = "", float scale=1.0f) : base("", layer, id, 0, 1, scale)
    {
        animations = new Dictionary<string, Animation>();
    }

    public AnimatedGameObject(SerializationInfo info, StreamingContext context) : base(info, context)
    {

    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        foreach(KeyValuePair<string, Animation> pair in animations)
        {
            info.AddValue(pair.Key, pair.Value);
        }
        
        base.GetObjectData(info, context);
    }

    public void LoadAnimation(string assetName, string id, bool looping, float frameTime = 0.1f)
    {
        Animation anim = new Animation(assetName, looping, frameTime);
        animations[id] = anim;        
    }

    public void PlayAnimation(string id)
    {
        if (sprite == animations[id])
        {
            return;
        }
        if (sprite != null)
        {
            animations[id].Mirror = sprite.Mirror;
        }
        animations[id].Play();
        sprite = animations[id];
        origin = new Vector2(sprite.Width / 2, sprite.Height);
    }

    public void PlaySound(string id)
    {
        GameEnvironment.AssetManager.PlaySound(id);
    }

    public override void Update(GameTime gameTime)
    {
        if (sprite == null)
        {
            return;
        }
        Current.Update(gameTime);
        base.Update(gameTime);
    }

    public Animation Current
    {
        get { return sprite as Animation; }
    }
}