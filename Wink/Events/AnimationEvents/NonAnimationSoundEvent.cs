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
        /// <summary>
        /// Event for playing sounds that are not included within an animation. If sound sould be played during animation, inplement PlaySound method within that animation event
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="soundLength"></param>
        /// <param name="playerSpecific">True if only a specific user should hear the soundeffect</param>
        /// <param name="LocalPlayerName">if playerSpecific, id of player that should hear the sound</param>
        public NonAnimationSoundEvent(string assetName, bool playerSpecific = false, string LocalPlayerName = "") : base(assetName, playerSpecific, LocalPlayerName)
        {
        }

        #region Serialization
        public NonAnimationSoundEvent(SerializationInfo info, StreamingContext context):base(info,context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        #endregion

        protected override int Length
        {
            get
            {
                int delay = 6;//ensure there is a delay between sounds
                float miliseconds = GameEnvironment.AssetManager.Duration(soundAssetName).Milliseconds;
                float frames = ((miliseconds / 1000) * 60) + delay;
                return (int)frames;
            }
        }

        public override void Animate()
        {
        }

        public override void PostAnimate()
        {
        }

        public override void PreAnimate(LocalClient client)
        {
            if (playerSpecific && LocalPlayerName == "")
            {
                throw new Exception("forgot to specify specific player");
            }

            if (!soundAssetName.Contains("Sounds/"))
            {
                soundAssetName = "Sounds/" + soundAssetName;//forgot to specify the sound folder
                throw new Exception("forgot to specify the sound folder");//exeption for now to show the programmer forgot the sound folder
            }

            if (!playerSpecific)
            {
                GameEnvironment.AssetManager.PlaySound(soundAssetName);
            }
            else if (Player.LocalPlayerName == LocalPlayerName)
            {
                GameEnvironment.AssetManager.PlaySound(soundAssetName);
            }
        }
    }
}
