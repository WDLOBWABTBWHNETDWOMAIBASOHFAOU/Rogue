using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Wink
{
    public enum ArmorType
    {
        light,
        normal,
        heavy,
        robes,

        gen//ALWAYS LAST
    }

    [Serializable]
    public abstract class ArmorEquipment : Equipment
    {
        int physicalValue;
        int magicValue;
        int reqPenalty;
        ArmorType armorType;
        float walkCostMod, fast = 0.8f, normal = 1.0f, slowed = 1.2f, slow = 1.4f;

        public float WalkCostMod { get { return walkCostMod; } }

        //public ArmorEquipment(int floorNumber, int reqPenalty = 0, ArmorType genType = ArmorType.gen, int stackSize = 1, int layer = 0) : base(floorNumber, layer, stackSize)
        //{
        //    this.reqPenalty = reqPenalty;
        //    SetRandomBodyAmor(floorNumber,genType);
        //    SetID();
        //}

        public ArmorEquipment(ArmorType armorType, int physicalValue = 0, int magicValue = 0, int reqPenalty = 0, int layer = 0, int stackSize = 1) : base("", "", layer, stackSize)
        {
            this.physicalValue = physicalValue;
            this.magicValue = magicValue;
            this.reqPenalty = reqPenalty;
            this.armorType = armorType;
            SetTypeSpecs();
            SetID();
        }

        void SetID()
        {
            id = armorType.ToString() + ":" + physicalValue + ":" + magicValue;
        }

        #region Serialization
        public ArmorEquipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            physicalValue = info.GetInt32("physicalValue");
            magicValue = info.GetInt32("magicValue");
            reqPenalty = info.GetInt32("reqPenalty");
            walkCostMod = (float)info.GetDouble("walkCostMod");
            armorType = (ArmorType)info.GetValue("armorType", typeof(ArmorType));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("physicalValue", physicalValue);
            info.AddValue("magicValue", magicValue);
            info.AddValue("reqPenalty", reqPenalty);
            info.AddValue("walkCostMod", walkCostMod);
            info.AddValue("armorType", armorType);
        }
        #endregion

        public override void ItemInfo(ItemSlot caller)
        {
            base.ItemInfo(caller);

            TextGameObject armorinfo = new TextGameObject("Arial12", cameraSensitivity: 0, layer: 0, id: "ArmorValueTypeInfo." + this);
            armorinfo.Text = "ArmorValue: " + physicalValue + " physical, " + magicValue + " magic";
            armorinfo.Color = Color.Red;
            armorinfo.Parent = infoList;
            infoList.Children.Insert(1, armorinfo);
        }

        void SetTypeSpecs()
        {
            switch (armorType)
            {
                case ArmorType.light:
                    walkCostMod = normal;
                    spriteAssetName = "Sprites/Armor/elven_leather_armor";
                    break;
                case ArmorType.robes:
                    walkCostMod = fast;
                    spriteAssetName = "Sprites/Armor/robe2";
                    break;
                case ArmorType.normal:
                    walkCostMod = slowed;
                    spriteAssetName = "Sprites/Armor/dwarven_ringmail";
                    break;
                case ArmorType.heavy:
                    walkCostMod = slow;
                    spriteAssetName = "Sprites/Armor/orcish_platemail";
                    break;
                default:
                    break;
            }
        }

        public int Value(Living l,DamageType damageType)
        {
            int value;
            switch (damageType)
            {
                case DamageType.Physical:
                    if (MeetsRequirements(l))
                    {
                        value = physicalValue;
                        return value;
                    }
                    else
                    {
                        // now it's a armorvalue reduction as a penalty, it might do something compleetly different
                        value = (int)l.CalculateValue(physicalValue, 1, reqPenalty);
                        return value;
                    }
                case DamageType.Magic:
                    if (MeetsRequirements(l))
                    {
                        value = magicValue;
                        return value;
                    }
                    else
                    {                        
                        value = (int)l.CalculateValue(magicValue, 1, reqPenalty);
                        return value;
                    }
                default:
                    throw new Exception("invalide damageType");
            }
        }
    }
}
