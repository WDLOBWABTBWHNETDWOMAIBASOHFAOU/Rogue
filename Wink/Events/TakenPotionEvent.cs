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
            get { return Living.BaseActionCost/3; }
        }

        public override List<Guid> GetFullySerialized(Level level)
        {
            return null; //Irrelevant because client->server
        }

        protected override void DoAction(LocalServer server)
        {
            potion.stackCount--;
            switch (potion.GetPotionType)
            {
                case PotionType.Health:
                    player.Health += potion.PotionValue;
                    break;
                case PotionType.Mana:
                    player.Mana += potion.PotionValue;
                    break;
                default:
                    throw new Exception("invalid potionType");
            }
        }

        protected override bool ValidateAction(Level level)
        {
            switch (potion.GetPotionType)
            {
                case PotionType.Health:
                    if(!(player.Health >= player.MaxHealth))
                    {
                        return true;
                    }
                    break;
                case PotionType.Mana:
                    player.Mana += potion.PotionValue;

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
