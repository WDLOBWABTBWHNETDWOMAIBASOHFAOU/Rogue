using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Wink
{
    public enum RingEffectType
    {
        StatBonus,
        StatMultiplier,
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
        protected Dictionary<RingEffectType, double> balanceMultiplier = new Dictionary<RingEffectType, double>();
        private void SetBalance()
        {
            //balanceMultiplier.Add(EffectType.Strength, 0.8);
            //balanceMultiplier.Add(EffectType.Vitality, 1);
            //balanceMultiplier.Add(EffectType.Dexterity, 0.8);
            //balanceMultiplier.Add(EffectType.Luck, 1);
            //balanceMultiplier.Add(EffectType.Intelligence, 1.2);
            //balanceMultiplier.Add(EffectType.Wisdom, 1);
        }

        // the amount of magic effects in the EffectType enum, these will not be put on randomly generated rings
        protected int magicEffects = 1;

        //public RingEquipment(double ringValue, RingEffectType ringType, string assetName, bool multiplier = false, int stackSize = 1, int layer = 0, string id = "") : base(assetName, id, stackSize, layer)
        //{
        //    AddEffect(ringType);
        //    SetId();
        //}

        public RingEquipment(int maxEffects = 3, int stackSize = 1, int layer = 0, string id = "", bool reflectEffect = false) : base("", id, stackSize, layer)
        {
            SetBalance();
            GenerateRing(maxEffects + 1);
            SetId();
            SetSprite();
        }

        protected void GenerateEffect()
        {
            Array values = RingEffectType.GetValues(typeof(RingEffectType));
            RingEffectType effectType = (RingEffectType)values.GetValue(GameEnvironment.Random.Next(values.Length));
            RingEffect effect;
            switch (effectType)
            {
                case RingEffectType.StatBonus:;
                    effect = new StatBonusEffect((Stat)GameEnvironment.Random.Next(typeof(Stat).GetEnumValues().Length), GameEnvironment.Random.Next(5));
                    ringEffects.Add(effect);
                    break;
                case RingEffectType.StatMultiplier:
                    effect = new StatMultiplier((Stat)GameEnvironment.Random.Next(typeof(Stat).GetEnumValues().Length), Math.Round(GameEnvironment.Random.NextDouble(),2));
                    ringEffects.Add(effect);
                    break;
                case RingEffectType.Reflection:
                    effect = new ReflectionEffect(2);
                    ringEffects.Add(effect);
                    break;
            }
        }

        #region Serialization
        public RingEquipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ringEffects = info.GetValue("ringEffects", typeof(List<RingEffect>)) as List<RingEffect>;
            balanceMultiplier = info.GetValue("balanceMultiplier", typeof(Dictionary<RingEffectType, double>)) as Dictionary<RingEffectType, double>;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ringEffects", ringEffects, typeof(List<RingEffect>));
            info.AddValue("balanceMultiplier", balanceMultiplier);
        }
        #endregion

        protected void GenerateRing(int maxEffects = 3)
        {
            int effectAmt = GameEnvironment.Random.Next(0, maxEffects);
            for (int i = 0; i < effectAmt; i++)
                GenerateEffect();
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
                        case RingEffectType.StatBonus:
                            id += (rE as StatBonusEffect).Stat.ToString();
                            break;
                        case RingEffectType.StatMultiplier:
                            id += (rE as StatMultiplier).Stat.ToString();
                            break;
                        case RingEffectType.Reflection:
                            id += rE.EffectType.ToString();
                            break;
                    }

                    if (i != ringEffects.Count - 1)
                    {
                        id += ", ";
                    }
                }
            }
        }

        protected List<RingEffect> ringEffects = new List<RingEffect>();

        public List<RingEffect> RingEffects
        {
            get { return ringEffects; }
        }

        public override void ItemInfo(ItemSlot caller)
        {
            displayedName = id;
            base.ItemInfo(caller);

            foreach(RingEffect e in ringEffects)
            {
                TextGameObject ringInfo = new TextGameObject("Arial12", cameraSensitivity: 0, layer: 0, id: "RingInfo." + this);
                ringInfo.Text = e.InfoString;
                ringInfo.Color = Color.Red;
                ringInfo.Parent = infoList;
                infoList.Children.Add(ringInfo);
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
    public abstract class RingEffect : ISerializable
    {
        protected RingEffectType effectType;
        public RingEffectType EffectType { get { return effectType; } }

        protected string spriteString;
        public string SpriteString { get { return spriteString; } }

        protected string infoString;
        public string InfoString { get { return infoString; } }

        public RingEffect(RingEffectType effectType)
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
            effectType = (RingEffectType)info.GetValue("effectType", typeof(RingEffectType));
            spriteString = info.GetString("spriteString");
            infoString = info.GetString("infoString");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("effectType", effectType);
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

        public StatBonusEffect(Stat stat, int bonusValue) : base(RingEffectType.StatBonus)
        {
            this.stat = stat;
            this.bonusValue = bonusValue;
            setInfoString();
            setSpriteString();
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
            info.AddValue("stat", stat);
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

        public StatMultiplier(Stat stat, double multiplier) : base(RingEffectType.StatMultiplier)
        {
            this.stat = stat;
            this.multiplier = multiplier;
            setInfoString();
            setSpriteString();
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
            info.AddValue("stat", stat);
            info.AddValue("multiplier", multiplier);
        }
        #endregion
    }

    [Serializable]
    public class ReflectionEffect : RingEffect,ItriggerEffect
    {
        public virtual TriggerEffects effect { get { return TriggerEffects.Reflection; } }
        protected double power;
        protected double chance;
        protected double luckmod;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="power"></param>
        /// <param name="chance">chance reflection hits/has effect. if higher it's than 1 it's guaranteed to hit 
        /// if the luck the opponent/source is lower than (chance-1)/luckmod</param>
        public ReflectionEffect(double power, double chance = 1.0, double luckmod = 0.05) : base(RingEffectType.Reflection)
        {
            this.power = power;
            this.chance = chance;
            this.luckmod = luckmod;
            setInfoString();
            setSpriteString();
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
            power = info.GetDouble("power");
            chance = info.GetDouble("chance");
            luckmod = info.GetDouble("luckmod");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("power", power);
            info.AddValue("chance", chance);
            info.AddValue("luckmod", luckmod);
        }
        #endregion

        public override void DoBonus(Living target)
        {
            target.triggerEffects.Add(this);
        }

        public override void RemoveBonus(Living target)
        {
            target.triggerEffects.Remove(this);
        }

        public void ExecuteTrigger(Living source, Living target, double value)
        {
            double randomNumber = GameEnvironment.Random.NextDouble();
            double reflectChance = chance - luckmod * source.GetStat(Stat.Luck); // Calculate reflection chance
            if (randomNumber <= reflectChance)
            { // Deal Reflection Damage
                double reflectAmount = 0.8 - 2 / source.GetStat(Stat.Intelligence);
                if (reflectAmount < 0) // makes sure you can never heal the opponent by dealing 
                    reflectAmount = 0; //     negative reflection damage
                int damageTaken = (int)(value * reflectAmount * power)*1000;
                target.Health -= damageTaken;
                // TODO: Visual feedback of reflection
            }
        }
    }
}
