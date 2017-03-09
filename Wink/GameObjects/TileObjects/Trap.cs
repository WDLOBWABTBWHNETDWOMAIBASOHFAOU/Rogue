using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using System.Linq;

namespace Wink
{
    [Serializable]
    class Trap : Tile
    {
        private bool triggered;
        private int trapStrength;
        private DamageType damageType;
        int trapExp;

        public Trap(string assetName, int trapStrength = 150, DamageType damageType=DamageType.Physical,int trapExp = 10, TileType tp = TileType.Floor, int layer = 0, string id = "") : base(assetName,tp,layer,id)
        {
            this.trapStrength = trapStrength;
            triggered = false;
            this.damageType = damageType;
            this.trapExp = trapExp;
        }

        public override bool PutOnTile<T>(T tileObject)
        {
            if(base.PutOnTile<T>(tileObject))
            {
                if (tileObject.GetType().BaseType == typeof(Living) && triggered == false)
                {
                    triggerdTrap(tileObject as Living);                    
                }
                return true;//return true because base is true (does not indicate wheter or not the trap was sprung)
            }
            return false;//base is false
        }

        #region TrapMecanic
        private void triggerdTrap(Living victim)
        {
            //not taking armor in to account
            if (victim.DodgeChance() >= GameEnvironment.Random.Next(100))
            {
                victim.Health -= 10;
            }

            //taking armor in to account
            //victim.TakeDamage(trapStrength, damageType);

            triggered = true;
        }

        public void disarmTrap(Living l)
        {
            //attempt to disarm the trap, if succesfull trap is disarmed whithout the livingObject taking damage, else trap is sprung (same effect as stepping on it)
            int disarmChance = (int)l.CalculateValue(40, l.GetStat(Stat.Luck), 0,1);
            int disarmValue = GameEnvironment.Random.Next(100);

            if (disarmChance >= disarmValue)//check if the livingObject succesfully disarms the trap
            {
                triggered = true;
                if (l.GetType() == typeof(Player))
                {
                    (l as Player).ReceiveExp(trapExp);
                }
            }
            else
            {
                triggerdTrap(l);
            }
        }
        #endregion

        public override void HandleInput(InputHelper inputHelper)
        {
            // assuming it is posible to see if a tile is a trap, still works whitout it but the user will either rightclick every tile before moving or never use this function at all

            if (!triggered)
            {
                Action onRightClick = () =>
                {
                    Player player = GameWorld.Find(Player.LocalPlayerName) as Player;
                    DisarmTrapEvent DTE = new DisarmTrapEvent(player, this);
                    Server.Send(DTE);
                };
                inputHelper.IfMouseRightButtonPressedOn(this, onRightClick);
            }            

            base.HandleInput(inputHelper);
        }

        #region Serialization
        public Trap(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            triggered = info.GetBoolean("triggered");
            trapStrength = info.GetInt32("trapStrength");
            trapExp = info.GetInt32("trapExp");
            damageType = (DamageType)info.GetValue("damageType", typeof(DamageType));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("triggered", triggered);
            info.AddValue("trapStrength", trapStrength);
            info.AddValue("trapExp", trapExp);
            info.AddValue("damageType", damageType);
            base.GetObjectData(info, context);
        }
        #endregion
        
    }
}
