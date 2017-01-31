using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Wink
{
    public enum EffectType
    {
        Vitality,
        Strength,
        Dexterity,
        Luck,
        Intelligence,
        Wisdom,
        // Make sure types of magic effects or other types that shouldn't be randomly put on rings are at the end of the enum
        Reflection
    }

    [Serializable]
    class RingEquipment : Equipment
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
            balanceMultiplier.Add(EffectType.Strength, 0.8);
            balanceMultiplier.Add(EffectType.Vitality, 1);
            balanceMultiplier.Add(EffectType.Dexterity, 0.8);
            balanceMultiplier.Add(EffectType.Luck, 1);
            balanceMultiplier.Add(EffectType.Intelligence, 1.2);
            balanceMultiplier.Add(EffectType.Wisdom, 1);
        }

        protected List<RingEffect> ringEffects = new List<RingEffect>();

        // the amount of magic effects in the EffectType enum, these will not be put on randomly generated rings
        protected int magicEffects = 1;

        public RingEquipment(double ringValue, EffectType ringType, string assetName, bool multiplier = false, int stackSize = 1, int layer = 0, string id = "") : base(assetName, id, stackSize, layer)
        {
            AddEffect(ringType, ringValue, multiplier);
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
                switch (ringEffects[0].EffectType)
                {
                    case EffectType.Vitality:
                        spriteAssetName += "agate";
                        break;
                    case EffectType.Strength:
                        spriteAssetName += "bronze";
                        break;
                    case EffectType.Dexterity:
                        spriteAssetName += "clay";
                        break;
                    case EffectType.Luck:
                        spriteAssetName += "copper";
                        break;
                    case EffectType.Intelligence:
                        spriteAssetName += "coral";
                        break;
                    case EffectType.Wisdom:
                        spriteAssetName += "diamond";
                        break;
                    case EffectType.Reflection:
                        spriteAssetName += "emerald";
                        break;
                    default:
                        break;
                }
            }
        }

        void SetId()
        {
            id = "Ring of ";
            foreach (RingEffect rE in RingEffects)
            {
                id += rE.EffectType.ToString()+ ",";
            }
            if ( ringEffects.Count == 0)
            {
                id = "Plain Ring";
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

        protected void AddEffect(EffectType effectType, double effectValue, bool multiplier)
        {
            RingEffect effect = new RingEffect(effectType, multiplier, effectValue);
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
            AddEffect(effectType, effectValue, multiplier);
        }

        public List<RingEffect> RingEffects
        {
            get { return ringEffects; }
        }

        // Change in code broke this part

        public override void ItemInfo(ItemSlot caller)
        {
            base.ItemInfo(caller);

            foreach(RingEffect e in ringEffects)
            {
                TextGameObject ringInfo = new TextGameObject("Arial12", cameraSensitivity: 0, layer: 0, id: "RingInfo." + this);
                ringInfo.Text = e.EffectType.ToString() + " Ring" + " : " + e.EffectValue;// needs more telling information but not sure what to use/ how to read/reach it
                if (e.Multiplier)
                {
                    ringInfo.Text += " : multipier";
                }
                else
                {
                    ringInfo.Text += " : bonus";
                }
                ringInfo.Color = Color.Red;
                ringInfo.Parent = infoList;
                infoList.Children.Insert(1,ringInfo);
            }
        }
    }
    [Serializable]
    class RingEffect : ISerializable
    {
        protected EffectType effectType;
        protected bool multiplier;
        protected double effectValue;

        public EffectType EffectType { get { return effectType; } }
        public bool Multiplier { get { return multiplier; } }
        public double EffectValue { get { return effectValue; } }

        public RingEffect(EffectType effectType, bool multiplier, double effectValue)
        {
            this.effectType = effectType;
            this.multiplier = multiplier;
            this.effectValue = effectValue;
        }

        #region Serialization
        public RingEffect(SerializationInfo info, StreamingContext context)
        {
            effectType = (EffectType)info.GetValue("effectType", typeof(EffectType));
            multiplier = info.GetBoolean("multiplier");
            effectValue = info.GetDouble("effectValue");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("effectType", effectType, typeof(EffectType));
            info.AddValue("multiplier", multiplier);
            info.AddValue("effectValue", effectValue);
        }
        #endregion
    }

    [Serializable]
    class ReflectionEffect : RingEffect
    {
        protected double power;
        protected double chance;

        public double Power { get { return power; } }
        public double Chance { get { return chance; } }

        public ReflectionEffect(double power, double chance) : base(EffectType.Reflection, false, -1)
        {
            this.power = power;
            this.chance = chance;
        }

        #region Serialization
        public ReflectionEffect(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            effectType = (EffectType)info.GetValue("effectType", typeof(EffectType));
            power = info.GetDouble("power");
            chance = info.GetDouble("chance");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("power", power);
            info.AddValue("chance", chance);
        }
        #endregion

        public double this[string val]
        {
            get
            {
                if (val == "power")
                {
                    return Power;
                }
                else if (val == "chance")
                {
                    return Chance;
                }
                else
                {
                    throw new KeyNotFoundException("The value " + val + " is not in ReflectionEffect");
                }
            }
        }
    }
}
