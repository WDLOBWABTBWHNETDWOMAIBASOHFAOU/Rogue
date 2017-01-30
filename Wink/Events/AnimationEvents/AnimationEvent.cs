using System;
using System.Runtime.Serialization;

namespace Wink
{
    public abstract class AnimationEvent : Event
    {
        protected abstract int Length { get; }
        protected int counter;
        protected string assetName;
        protected bool playerSpecific;
        protected string LocalPlayerName;

        public AnimationEvent(string assetName, bool playerSpecific = false, string LocalPlayerName = "")
        {
            this.assetName=assetName;
            this.playerSpecific = playerSpecific;
            this.LocalPlayerName = LocalPlayerName;
        }

        #region Serialization
        public AnimationEvent(SerializationInfo info, StreamingContext context) : base (info, context)
        {            assetName = info.GetString("assetName");
            playerSpecific = info.GetBoolean("playerSpecific");
            LocalPlayerName = info.GetString("LocalPlayerName");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("assetName", assetName);
            info.AddValue("playerSpecific", playerSpecific);
            info.AddValue("LocalPlayerName", LocalPlayerName);
        }
        #endregion

        public override bool GUIDSerialization
        {
            get { return true; }
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
