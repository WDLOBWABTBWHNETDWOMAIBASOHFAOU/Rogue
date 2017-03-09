using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Wink
{
    public enum EffectType
    {
        StatBonus,
        StatMultiplier,
        // Make sure types of magic effects or other types that shouldn't be randomly put on rings are at the end of the enum
        Reflection
    }

    [Serializable]
    public class RingEquipment : Equipment
    {
        // mulchance is the chance a ring effect will be a multiplier, I'm keeping this relatively high because
        // a multiplier is much easier to balance than a set value
        protected double mulchance = 0.7;
        // acceptablePower is the maximum power for a single stat boost on a ring, can be changed for balancing
        // This should eventually be a function based on the floor the ring was found on
        protected int acceptablePower = 5;

        // This dictionary will hold all of the multipliers for balancing rings for the different stats
        // for now these will all be set to 1
        protected Dictionary<EffectType, double> balanceMultiplier = new Dictionary<EffectType, double>();
        private void SetBalance()
        {
            //balanceMultiplier.Add(EffectType.Strength, 0.8);
            //balanceMultiplier.Add(EffectType.Vitality, 1);
            //balanceMultiplier.Add(EffectType.Dexterity, 0.8);
            //balanceMultiplier.Add(EffectType.Luck, 1);
            //balanceMultiplier.Add(EffectType.Intelligence, 1.2);
            //balanceMultiplier.Add(EffectType.Wisdom, 1);
        }

        protected List<RingEffect> ringEffects = new List<RingEffect>();

        // the amount of magic effects in the EffectType enum, these will not be put on randomly generated rings
        protected int magicEffects = 1;

        public RingEquipment(double ringValue, EffectType ringType, string assetName, bool multiplier = false, int stackSize = 1, int layer = 0, string id = "") : base(assetName, id, stackSize, layer)
        {
            AddEffect(ringType);
            SetId();
        }

        public RingEquipment(string assetName, int maxEffects = 3, int stackSize = 1, int layer = 0, string id = "", bool reflectEffect = false) : base(assetName, id, stackSize, layer)
        {
            SetBalance();
            GenerateRing(maxEffects + 1);
            if (reflectEffect)
                AddReflection();
            SetId();
            SetSprite();
        }

        void SetSprite()
        {
            spriteAssetName = "Sprites/Rings/";//set folder refrence
            if (ringEffects.Count == 0)
            {
                spriteAssetName += "brass";
            }
            else
            {
                spriteAssetName += ringEffects[0].SpriteString;//sprite based on first (main) effect                
            }
        }

        void SetId()
        {
            if ( ringEffects.Count == 0)
            {
                id = "Plain Ring";
            }
            else
            {
                id = "Ring of ";
                for (int i = 0; i < ringEffects.Count; i++)
                {
                    RingEffect rE = ringEffects[i];
                    switch (rE.EffectType)
                    {
                        case EffectType.StatBonus:
                            id += (rE as StatBonusEffect).Stat.ToString();
                            break;
                        case EffectType.StatMultiplier:
                            id += (rE as StatMultiplier).Stat.ToString() + " multiplier";
                            break;
                        case EffectType.Reflection:
                            id += rE.EffectType.ToString();
                            break;
                    }
                    if (i != ringEffects.Count - 1)
                    {
                        id += rE.EffectType.ToString()+ ",";
                    }
                }
            }
        }

        #region Serialization
        public RingEquipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ringEffects = info.GetValue("ringEffects", typeof(List<RingEffect>)) as List<RingEffect>;
            balanceMultiplier = info.GetValue("balanceMultiplier", typeof(Dictionary<EffectType, double>)) as Dictionary<EffectType, double>;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ringEffects", ringEffects, typeof(List<RingEffect>));
            info.AddValue("balanceMultiplier", balanceMultiplier, typeof(Dictionary<EffectType, double>));
        }
        #endregion

        protected void AddEffect(EffectType effectType)
        {
            RingEffect effect = new RingEffect(effectType);
            ringEffects.Add(effect);
        }

        protected void AddReflection()
        {
            // This method adds a special effect to a ring, this being the "Reflection" effect.
            // It's the first attempt of magic in the game
            ReflectionEffect reflectionEffect = new ReflectionEffect(2.5, 1.5);
            ringEffects.Add(reflectionEffect);
        }

        protected void GenerateRing(int maxEffects = 3)
        {
            int effectAmt = GameEnvironment.Random.Next(0, maxEffects);
            for (int i = 0; i < effectAmt; i++)
                GenerateEffect();
        }

        protected void GenerateEffect()
        {
            Array values = EffectType.GetValues(typeof(EffectType));
            EffectType effectType = (EffectType)values.GetValue(GameEnvironment.Random.Next(values.Length - magicEffects));

            bool multiplier = true;
            if (GameEnvironment.Random.NextDouble() < mulchance)
                multiplier = false;

            Dictionary<string, object> ret = new Dictionary<string, object>(); 
            double effectValue = 0;

            if (multiplier) 
                effectValue = 1 + Math.Round(GameEnvironment.Random.NextDouble(), 2); 
            else 
                effectValue = Math.Round(acceptablePower * GameEnvironment.Random.NextDouble()); 

            effectValue *= balanceMultiplier[effectType];
            AddEffect(effectType);
        }

        public List<RingEffect> RingEffects
        {
            get { return ringEffects; }
        }

        public override void ItemInfo(ItemSlot caller)
        {
            base.ItemInfo(caller);

            foreach(RingEffect e in ringEffects)
            {
                TextGameObject ringInfo = new TextGameObject("Arial12", cameraSensitivity: 0, layer: 0, id: "RingInfo." + this);
                ringInfo.Text = e.InfoString;
                ringInfo.Color = Color.Red;
                ringInfo.Parent = infoList;
                infoList.Children.Insert(1,ringInfo);
            }
        }

        protected override bool MeetsRequirements(Living l)
        {
            throw new NotImplementedException();
        }

        public override void DoBonus(Living living)
        {
            foreach (RingEffect rE in RingEffects)//check every effect that is on the ring
            {
                rE.DoBonus(living);
            }
        }

        public override void RemoveBonus(Living living)
        {
            foreach (RingEffect rE in RingEffects)
            {
                rE.RemoveBonus(living);
            }
        }
    }

    [Serializable]
    public class RingEffect : ISerializable
    {
        protected EffectType effectType;
        public EffectType EffectType { get { return effectType; } }

        protected string spriteString;
        public string SpriteString { get { return spriteString; } }

        protected string infoString;
        public string InfoString { get { return infoString; } }

        public RingEffect(EffectType effectType)
        {
            this.effectType = effectType;
        }

        public virtual void DoBonus(Living target) { }
        public virtual void RemoveBonus(Living target) { }
        protected virtual void setSpriteString() { }
        protected virtual void setInfoString() { }

        #region Serialization
        public RingEffect(SerializationInfo info, StreamingContext context)
        {
            effectType = (EffectType)info.GetValue("effectType", typeof(EffectType));
            spriteString = info.GetString("spriteString");
            infoString = info.GetString("infoString");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("effectType", effectType, typeof(EffectType));
            info.AddValue("spriteString", spriteString);
            info.AddValue("infoString", infoString);
        }
        #endregion
    }

    [Serializable]
    public class StatBonusEffect : RingEffect
    {
        private Stat stat;
        public Stat Stat { get { return stat; } }
        private int bonusValue;

        public StatBonusEffect(Stat stat) : base(EffectType.StatBonus)
        {
            this.stat = stat;
        }

        public override void DoBonus(Living target)
        {
            target.statsBonus[stat] += bonusValue;
        }

        public override void RemoveBonus(Living target)
        {
            target.statsBonus[stat] -= bonusValue;
        }

        protected override void setInfoString()
        {
            infoString = stat.ToString() + " + " + bonusValue;
        }

        protected override void setSpriteString()
        {
            switch (stat)
            {
                case Stat.Vitality:
                    spriteString += "agate";
                    break;
                case Stat.Strength:
                    spriteString += "bronze";
                    break;
                case Stat.Dexterity:
                    spriteString += "clay";
                    break;
                case Stat.Luck:
                    spriteString += "copper";
                    break;
                case Stat.Intelligence:
                    spriteString += "coral";
                    break;
                case Stat.Wisdom:
                    spriteString += "diamond";
                    break;
            }
        }

        #region Serialization
        public StatBonusEffect(SerializationInfo info, StreamingContext context):base(info,context)
        {
            stat = (Stat)info.GetValue("stat", typeof(Stat));
            bonusValue = info.GetInt32("bonusValue");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("stat", effectType, typeof(Stat));
            info.AddValue("bonusValue", bonusValue);
        }
        #endregion
    }

    [Serializable]
    public class StatMultiplier : RingEffect
    {
        private Stat stat;
        public Stat Stat { get { return stat; } }
        private double multiplier;

        public StatMultiplier(Stat stat) : base(EffectType.StatMultiplier)
        {
            this.stat = stat;
        }

        public override void DoBonus(Living target)
        {
            target.statsMultiplier[stat] += multiplier;
            // will result in added stacking ( 0.2 and 0,3 results in 0.5 bonus (so times 1.5)), instead of multiplied stacking ( 1.2 and 1,3 results in 1.56 bonus)
            // need to deside which one we want
        }

        public override void RemoveBonus(Living target)
        {
            target.statsMultiplier[stat] -= multiplier;
        }

        protected override void setInfoString()
        {
            infoString = stat.ToString() + " * " + multiplier;
        }

        protected override void setSpriteString()
        {
            switch (stat)
            {
                case Stat.Vitality:
                    spriteString += "agate";
                    break;
                case Stat.Strength:
                    spriteString += "bronze";
                    break;
                case Stat.Dexterity:
                    spriteString += "clay";
                    break;
                case Stat.Luck:
                    spriteString += "copper";
                    break;
                case Stat.Intelligence:
                    spriteString += "coral";
                    break;
                case Stat.Wisdom:
                    spriteString += "diamond";
                    break;
            }
        }

        #region Serialization
        public StatMultiplier(SerializationInfo info, StreamingContext context):base(info,context)
        {
            stat = (Stat)info.GetValue("stat", typeof(Stat));
            multiplier = info.GetInt32("multiplier");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("stat", effectType, typeof(Stat));
            info.AddValue("multiplier", multiplier);
        }
        #endregion
    }

    [Serializable]
    public class ReflectionEffect : RingEffect
    {
        //protected double power;
        //protected double chance;

        //public double Power { get { return power; } }
        //public double Chance { get { return chance; } }

        public ReflectionEffect(double power, double chance) : base(EffectType.Reflection)
        {
            //this.power = power;
            //this.chance = chance;
        }

        protected override void setInfoString()
        {
            infoString = "damage reflected";
        }

        protected override void setSpriteString()
        {
            spriteString += "emerald";
        }

        #region Serialization
        public ReflectionEffect(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            //power = info.GetDouble("power");
            //chance = info.GetDouble("chance");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            //info.AddValue("power", power);
            //info.AddValue("chance", chance);
        }
        #endregion

    //    public double this[string val]
    //    {
    //        get
    //        {
    //            if (val == "power")
    //            {
    //                return Power;
    //            }
    //            else if (val == "chance")
    //            {
    //                return Chance;
    //            }
    //            else
    //            {
    //                throw new KeyNotFoundException("The value " + val + " is not in ReflectionEffect");
    //            }
    //        }
    //    }
    }
}
