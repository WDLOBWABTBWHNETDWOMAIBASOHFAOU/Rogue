using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    class TakenPotionEvent : ActionEvent
    {
        private Potion potion;

        public TakenPotionEvent(Player player, Potion potion) : base(player)
        {
            this.player = player;
            this.potion = potion;
        }

        #region Serialization
        public TakenPotionEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            potion = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("potionGUID"))) as Potion;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("potionGUID", potion.GUID.ToString());
            base.GetObjectData(info, context);
        }
        #endregion

        protected override int Cost
        {
            get { return Living.BaseActionCost / 3; }
        }

        protected override void DoAction(LocalServer server, HashSet<GameObject> changedObjects)
        {
            potion.stackCount--;
            switch (potion.GetPotionType)
            {
                case PotionType.Health:
                    player.Health += potion.PotionValue;
                    NonAnimationSoundEvent PotionSoundEvent = new NonAnimationSoundEvent("Sounds/Potion Sound smaller");
                    LocalServer.SendToClients(PotionSoundEvent);
                    
                    break;
                case PotionType.Mana:
                    player.Mana += potion.PotionValue;
                    NonAnimationSoundEvent PotionSoundEvent2 = new NonAnimationSoundEvent("Sounds/Potion Sound smaller");
                    LocalServer.SendToClients(PotionSoundEvent2);
                    break;
                default:
                    throw new Exception("invalid potionType");
            }
            changedObjects.Add(potion);
        }

        protected override bool ValidateAction(Level level)
        {
            switch (potion.GetPotionType)
            {
                case PotionType.Health:
                    if (player.Health < player.MaxHealth)
                        return true;
                    break;
                case PotionType.Mana:
                    if (!(player.Mana >= player.MaxMana))
                    {
                        return true;
                    }
                    break;
                default:
                    throw new Exception("invalid potionType");
            }
            return false;
        }
    }
}
