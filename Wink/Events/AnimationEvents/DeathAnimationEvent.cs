﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class DeathAnimationEvent : AnimationEvent
    {
        private Living dead;

        public DeathAnimationEvent(Living dead, string soundAssetName, bool playerSpecific = false, string LocalPlayerName = "") : base(soundAssetName, playerSpecific, LocalPlayerName)
        {
            this.dead = dead;
        }

        #region Serialization
        public DeathAnimationEvent(SerializationInfo info, StreamingContext context):base(info,context)
        {
            dead = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("deadGUID"))) as Living;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("deadGUID", dead.GUID.ToString());
        }
        #endregion

        protected override int Length
        {
            get { return 45; }
        }

        public override void Animate()
        {

        }

        public override void PostAnimate()
        {
            dead.DeathFeedback();
        }

        public override void PreAnimate(LocalClient client)
        {
            dead.PlayAnimation("die");

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
