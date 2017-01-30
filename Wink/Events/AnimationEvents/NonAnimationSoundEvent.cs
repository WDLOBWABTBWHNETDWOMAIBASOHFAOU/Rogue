using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class NonAnimationSoundEvent : AnimationEvent
    {
        string assetName;
        int soundLength;
        bool playerSpecific;
        string LocalPlayerName;

        /// <summary>
        /// Event for playing sounds that are not included within an animation. If sound sould be played during animation, inplement PlaySound method within that animation event
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="soundLength"></param>
        /// <param name="playerSpecific">True if only a specific user should hear the soundeffect</param>
        /// <param name="LocalPlayerName">if playerSpecific, id of player that should hear the sound</param>
        public NonAnimationSoundEvent(string assetName, int soundLength, bool playerSpecific=false, string LocalPlayerName ="")
        {
            this.assetName=assetName;
            this.soundLength = soundLength;
            this.playerSpecific = playerSpecific;
            this.LocalPlayerName = LocalPlayerName ;
        }

        #region Serialization
        public NonAnimationSoundEvent(SerializationInfo info, StreamingContext context)
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
                return soundLength + 10;
            }
        }

        public override void Animate()
        {
            if (playerSpecific && LocalPlayerName == "")
            {
                throw new Exception("forgot to specify specific player");
            }

            if (!assetName.Contains("Sounds/"))
            {
                assetName = "Sounds/" + assetName;//forgot to specify the sound folder
                throw new Exception("forgot to specify the sound folder");//exeption for now to show the programmer forgot the sound folder
            }

            if (!playerSpecific)
            {
                GameEnvironment.AssetManager.PlaySound(assetName);
            }
            else if (Player.LocalPlayerName == LocalPlayerName)
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
