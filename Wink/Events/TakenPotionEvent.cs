using System;
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
            potion = info.GetValue("potion", typeof(Potion)) as Potion;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("potion", potion);
            base.GetObjectData(info, context);
        }
        #endregion

        protected override int Cost
        {
            get { return 1; }
        }

        public override bool GUIDSerialization
        {
            get { return true; }
        }

        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        protected override void DoAction(LocalServer server)
        {
            potion.stackCount--;
            switch (potion.PotionType)
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
            switch (potion.PotionType)
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
