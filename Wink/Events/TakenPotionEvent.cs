using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class TakenPotionEvent:ActionEvent
    {
        public Player Player { get; set; }
        public Potion Potion { get; set; }

        public TakenPotionEvent(Player player, Potion potion) : base(player)
        {
            this.Player = player;
            this.Potion = potion;
        }

        protected override int Cost
        {
            get
            {
                return 1;
            }
        }

        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        protected override void DoAction(LocalServer server)
        {
            Potion.stackCount--;
            switch (Potion.GetPotionType)
            {
                case PotionType.health:
                    Player.Health += Potion.PotionValue;
                    break;
                case PotionType.mana:
                    Player.Mana += Potion.PotionValue;
                    break;
                default:
                    throw new Exception("invalid potionType");
            }
        }

        protected override bool ValidateAction(Level level)
        {
            switch (Potion.GetPotionType)
            {
                case PotionType.health:
                    if(!(player.Health >= player.MaxHealth))
                    {
                        return true;
                    }
                    break;
                case PotionType.mana:
                    Player.Mana += Potion.PotionValue;

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
