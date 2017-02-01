using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wink
{
    public abstract class AnimationEvent : Event
    {
        protected abstract int Length { get; }
        protected int counter;
        protected string soundAssetName;
        protected bool playerSpecific;
        protected string LocalPlayerName;

        public AnimationEvent(string soundAssetName, bool playerSpecific = false, string LocalPlayerName = "")
        {
            this.soundAssetName = soundAssetName;
            this.playerSpecific = playerSpecific;
            this.LocalPlayerName = LocalPlayerName;
        }

        #region Serialization
        public AnimationEvent(SerializationInfo info, StreamingContext context) : base (info, context)
        {            soundAssetName = info.GetString("assetName");
            playerSpecific = info.GetBoolean("playerSpecific");
            LocalPlayerName = info.GetString("LocalPlayerName");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("assetName", soundAssetName);
            info.AddValue("playerSpecific", playerSpecific);
            info.AddValue("LocalPlayerName", LocalPlayerName);
        }
        #endregion

        public override List<Guid> GetFullySerialized(Level level)
        {
            return new List<Guid>();
        }

        public override bool OnClientReceive(LocalClient client)
        {
            if (counter == 0)
                PreAnimate(client);

            Animate();
            counter++;//TODO: make GameTime dependent

            if (counter >= Length)
            {
                PostAnimate();
                return true;
            }
            return false;
        }

        public override bool OnServerReceive(LocalServer server)
        {
            throw new NotImplementedException();
        }

        public abstract void PreAnimate(LocalClient client);
        public abstract void Animate();
        public abstract void PostAnimate();

        public override bool Validate(Level level)
        {
            return true;
        }
    }
}
