using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Wink
{
    [Serializable]
    public class WeaponEquipment : Equipment
    {
        int reach;
        public int Reach { get { return reach; } }
        protected int baseValue;
        protected DamageType damageType;
        public DamageType DamageType { get { return damageType; } }
        protected double strScalingFactor;
        protected double dexScalingFactor;
        protected double intScalingFactor;

        public WeaponEquipment(string assetName, string id, int baseValue, DamageType damageType, int stackSize = 1, int reach=1, int strRequirement=0, int dexRequirement = 0, int intRequirement=0, double strScalingFactor=0, double dexScalingFactor=0, double intScalingFactor=0, int layer = 0) : base(assetName,id, layer, stackSize,strRequirement,dexRequirement,intRequirement)
        {
            this.reach = reach;
            this.baseValue = baseValue;
            this.damageType = damageType;
            this.strScalingFactor = strScalingFactor;
            this.dexScalingFactor = dexScalingFactor;
            this.intScalingFactor = intScalingFactor;
        }

        public WeaponEquipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            reach = info.GetInt32("reach");
            baseValue = info.GetInt32("baseValue");
            damageType = (DamageType)info.GetValue("damageType", typeof(DamageType));
            strScalingFactor = info.GetDouble("strScalingFactor");
            dexScalingFactor = info.GetDouble("dexScalingFactor");
            intScalingFactor = info.GetDouble("intScalingFactor");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("reach", reach);
            info.AddValue("baseValue", baseValue);
            info.AddValue("damageType", damageType);
            info.AddValue("strScalingFactor", strScalingFactor);
            info.AddValue("dexScalingFactor", dexScalingFactor);
            info.AddValue("intScalingFactor", intScalingFactor);
        }

        public override void ItemInfo(ItemSlot caller)
        {
            string ClientName = Environment.MachineName;
            Player player = caller.GameWorld.Find("player_" + ClientName) as Player;
            base.ItemInfo(caller);

            TextGameObject dType = new TextGameObject("Arial12", 0, 0, "DamageTypeInfo." + this);
            dType.Text = DamageType + " Damage: " + baseValue + " + " + (Value(player) -baseValue);
            dType.Color = Color.Red;
            dType.Parent = infoList;
            infoList.Children.Insert(1, dType);

            TextGameObject scalingText = new TextGameObject("Arial12", 0, 0, "scalingInfoText." + this);
            scalingText.Text = "Statbonus:";
            switch (damageType)
            {
                case DamageType.physical:
                    {
                        scalingText.Text += " str " + strScalingFactor*baseValue + " dex " + dexScalingFactor * baseValue;
                        break;
                    }
                case DamageType.magic:
                    {
                        scalingText.Text += " int " + intScalingFactor * baseValue;
                        break;
                    }
                default:
                    throw new Exception("invalid damageType");
            }
            scalingText.Color = Color.Red;
            infoList.Add(scalingText);
        }

        public int Value(Living l)
        {
            int value;
            switch (damageType)
            {
                case DamageType.physical:
                    if (MeetsRequirements(l))
                    {
                        value = (int)l.CalculateValue(baseValue, l.Strength - strRequirement, strScalingFactor, 0, l.Dexterity - dexRequirement, dexScalingFactor);
                        return value;
                    }
                    else
                    {
                        value = (int)l.CalculateValue(baseValue, strRequirement - l.Strength, -strScalingFactor, 0, dexRequirement - l.Dexterity, -dexScalingFactor);
                        return value;
                    }
                case DamageType.magic:
                    if (MeetsRequirements(l))
                    {
                        value = (int)l.CalculateValue(baseValue, l.Intelligence - intRequirement, intScalingFactor);
                        return value;
                    }
                    else
                    {
                        value = (int)l.CalculateValue(baseValue, intRequirement - l.Intelligence, -intScalingFactor);
                        return value;
                    }
                default:
                    throw new Exception("invalid damageType");
            }
        }
    }
}
