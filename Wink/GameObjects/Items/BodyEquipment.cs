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
    public class BodyEquipment : Equipment
    {
        int physicalValue;
        int magicValue;
        int reqPenalty;
        ArmorType armorType;
        float walkCostMod, fast = 0.8f, normal = 1.0f, slow = 1.2f;

        public float WalkCostMod { get { return walkCostMod; } }

        public BodyEquipment(int floorNumber, int reqPenalty = 0, ArmorType genType = ArmorType.gen, int stackSize = 1, int layer = 0) : base(floorNumber, layer, stackSize)
        {
            this.reqPenalty = reqPenalty;
            SetRandomBodyAmor(floorNumber,genType);
            SetID();
        }

        public BodyEquipment(string assetName, string id, ArmorType armorType, int physicalValue = 0, int magicValue = 0, int reqPenalty = 0, int strRequirement = 0, int dexRequirement = 0, int intRequirement = 0, int layer = 0, int stackSize = 1) : base(assetName, id, layer, stackSize, strRequirement, dexRequirement, intRequirement)
        {
            this.physicalValue = physicalValue;
            this.magicValue = magicValue;
            this.reqPenalty = reqPenalty;
            this.armorType = armorType;
            SetWalkCostMod();
            SetID();
        }

        void SetID()
        {
            id = armorType.ToString() + ":" + physicalValue + ":" + magicValue;
        }

        void SetRandomBodyAmor(int floorNumber, ArmorType genType)
        {
            armorType = genType;
            if(armorType == ArmorType.gen)
            {
                //select random armorType
                Array aTypeValues = Enum.GetValues(typeof(ArmorType));
                armorType = (ArmorType)aTypeValues.GetValue(GameEnvironment.Random.Next(aTypeValues.Length-1));
            }

            // some values used to calculate final values
            int someBaseValue = 20;
            int baseBonusValue = 50 * floorNumber;
            int highPriority = 10;
            int mediumPriority = 20;
            int lowPriority = 30;

            //calculate final values based on armorType
            switch (armorType)
            {
                case ArmorType.light:
                    physicalValue = someBaseValue + HighBonusValue(baseBonusValue);
                    magicValue = someBaseValue + LowBonusValue(baseBonusValue);
                    walkCostMod = fast;
                    strRequirement = GameEnvironment.Random.Next(physicalValue) / mediumPriority;
                    dexRequirement = GameEnvironment.Random.Next(physicalValue) / highPriority;
                    intRequirement = GameEnvironment.Random.Next(magicValue) / lowPriority;

                    spriteAssetName = "empty:64:64:10:DarkGray";//TODO: replace by correct spritename
                    break;
                case ArmorType.normal:
                    physicalValue = someBaseValue + MediumBonusValue(baseBonusValue);
                    magicValue = someBaseValue + MediumBonusValue(baseBonusValue);
                    walkCostMod = normal;
                    strRequirement = GameEnvironment.Random.Next(physicalValue) / mediumPriority;
                    dexRequirement = GameEnvironment.Random.Next(physicalValue) / mediumPriority;
                    intRequirement = GameEnvironment.Random.Next(magicValue) / mediumPriority;

                    spriteAssetName = "empty:64:64:10:Yellow";//TODO: replace by correct spritename
                    break;
                case ArmorType.heavy:
                    physicalValue = someBaseValue + HighBonusValue(baseBonusValue) + HighBonusValue(baseBonusValue);
                    magicValue = someBaseValue + LowBonusValue(baseBonusValue);
                    walkCostMod = slow;
                    strRequirement = GameEnvironment.Random.Next(physicalValue) / highPriority;
                    dexRequirement = GameEnvironment.Random.Next(physicalValue) / mediumPriority;
                    intRequirement = GameEnvironment.Random.Next(magicValue) / lowPriority;

                    spriteAssetName = "empty:64:64:10:Brown";//TODO: replace by correct spritename
                    break;
                case ArmorType.robes:
                    physicalValue = someBaseValue + LowBonusValue(baseBonusValue);
                    magicValue = someBaseValue + HighBonusValue(baseBonusValue) + HighBonusValue(baseBonusValue);
                    walkCostMod = fast;
                    strRequirement = GameEnvironment.Random.Next(physicalValue) / lowPriority;
                    dexRequirement = GameEnvironment.Random.Next(physicalValue) / lowPriority;
                    intRequirement = GameEnvironment.Random.Next(magicValue) / highPriority;

                    spriteAssetName = "empty:64:64:10:Purple";//TODO: replace by correct spritename
                    break;
                default:
                    throw new Exception("invalid ArmorType");
            }

            //correct negative values
            //althoug negative defences on armor could be fun to play with, for now lets say they cann't
            if (physicalValue < 0)
                physicalValue = 0;
            if (magicValue < 0)
                magicValue = 0;
        }

        #region Serialization
        public BodyEquipment(SerializationInfo info, StreamingContext context) : base(info, context)
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

        void SetWalkCostMod()
        {
            switch (armorType)
            {
                case ArmorType.light:
                case ArmorType.robes:
                    walkCostMod = fast;
                    break;
                case ArmorType.normal:
                    walkCostMod = normal;
                    break;
                case ArmorType.heavy:
                    walkCostMod = slow;
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
                        value = (int)l.CalculateValue(physicalValue, penaltyDif(l), reqPenalty);
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
                        value = (int)l.CalculateValue(magicValue, penaltyDif(l), reqPenalty);
                        return value;
                    }
                default:
                    throw new Exception("invalide damageType");
            }
        }

        private int penaltyDif(Living l)
        {
            int dif = 0;
            if (l.Strength < strRequirement)
            {
                dif += strRequirement - l.Strength;
            }
            if (l.Dexterity < dexRequirement)
            {
                dif += dexRequirement - l.Dexterity;
            }
            if (l.Intelligence < intRequirement)
            {
                dif += intRequirement - l.Intelligence;
            }
            return dif;
        }
    }
}
