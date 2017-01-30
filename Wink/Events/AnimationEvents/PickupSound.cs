using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class PickupSound : AnimationEvent
    {
        string assetName;
        int soundLength;
        bool playerSpecific;
        string LocalPlayerName;

        public PickupSound(string assetName, int soundLength, bool playerSpecific=false, string LocalPlayerName ="")
        {
            this.assetName=assetName;
            this.soundLength = soundLength;
            this.playerSpecific = playerSpecific;
            this.LocalPlayerName = LocalPlayerName ;
        }

        #region Serialization
        public PickupSound(SerializationInfo info, StreamingContext context)
        {
            assetName = info.GetString("assetName");
            soundLength = info.GetInt32("soundLength");
            playerSpecific = info.GetBoolean("playerSpecific");
            LocalPlayerName = info.GetString("LocalPlayerName");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("assetName", assetName);
            info.AddValue("soundLength", soundLength);
            info.AddValue("playerSpecific", playerSpecific);
            info.AddValue("LocalPlayerName", LocalPlayerName);


        }
        #endregion

        protected override int Length
        {
            get
            {
                return soundLength;
            }
        }

        public override void Animate()
        {
            if (!playerSpecific)
            {
                GameEnvironment.AssetManager.PlaySound(assetName);
            }
            else if(Player.LocalPlayerName == LocalPlayerName)
            {
                GameEnvironment.AssetManager.PlaySound(assetName);
            }
        }

        public override void PostAnimate()
        {
        }

        public override void PreAnimate(LocalClient client)
        {
        }
    }
}
