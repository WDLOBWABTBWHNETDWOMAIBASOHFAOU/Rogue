using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wink
{
    public enum RingType
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
        protected int acceptablePower = 500;

        // This dictionary will hold all of the multipliers for balancing rings for the different stats
        // for now these will all be set to 1
        protected Dictionary<RingType, double> balanceMultiplier = new Dictionary<RingType, double>(); 

        private void setBalance()
        {
            balanceMultiplier.Add(RingType.Strength, 0.8);
            balanceMultiplier.Add(RingType.Vitality, 1);
            balanceMultiplier.Add(RingType.Dexterity, 0.8);
            balanceMultiplier.Add(RingType.Luck, 1);
            balanceMultiplier.Add(RingType.Intelligence, 1.2);
            balanceMultiplier.Add(RingType.Wisdom, 1);
        }

        protected List<RingEffect> ringEffects = new List<RingEffect>();

        // the amount of magic effects in the RingType enum, these will not be put on randomly generated rings
        protected int magicEffects = 1;

        public RingEquipment(double ringValue, RingType ringType, string assetName, bool multiplier = false, int stackSize = 1, int layer = 0, string id = "") : base(assetName, id, stackSize, layer)
        {
            AddEffect(ringType, ringValue, multiplier);
        }

        public RingEquipment(string assetName, int maxEffects = 3, int stackSize = 1, int layer = 0, string id = "", bool reflectEffect = false) : base(assetName, id, stackSize, layer)
        {
            setBalance();
            GenerateRing(maxEffects + 1);
            if (reflectEffect) AddReflection();
        }

        public RingEquipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ringEffects = info.GetValue("ringEffects", typeof(List<RingEffect>)) as List<RingEffect>;
            balanceMultiplier = info.GetValue("balanceMultiplier", typeof(Dictionary<RingType, double>)) as Dictionary<RingType, double>;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ringEffects", ringEffects, typeof(List<RingEffect>));
            info.AddValue("balanceMultiplier", balanceMultiplier, typeof(Dictionary<RingType, double>));
        }

        protected void AddEffect(RingType effectType, double effectValue, bool multiplier)
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
            for(int i = 0; i < GameEnvironment.Random.Next(0, maxEffects); i++)
            {
                GenerateEffect();
            }
        }

        protected void GenerateEffect()
        {
            Array values = RingType.GetValues(typeof(RingType));
            RingType effectType = (RingType)values.GetValue(GameEnvironment.Random.Next(values.Length - magicEffects));

            bool multiplier = true;
            if (GameEnvironment.Random.NextDouble() < mulchance) multiplier = false;

            Dictionary<string, object> ret = new Dictionary<string, object>();

            double effectValue = 0;

            if (multiplier)
            {
                effectValue = 1 + Math.Round(GameEnvironment.Random.NextDouble(), 2);
            }
            else
            {
                effectValue = Math.Round(acceptablePower * GameEnvironment.Random.NextDouble());
            }
            effectValue *= balanceMultiplier[effectType];
            AddEffect(effectType, effectValue, multiplier);
        }

        public List<RingEffect> RingEffects
        {
            get { return ringEffects; }
        }

        // Change in code broke this part

        /*public override void ItemInfo(ItemSlot caller)
        {
            base.ItemInfo(caller);

            if (Multiplier)
            {
                TextGameObject ringInfo = new TextGameObject("Arial12", 0, 0, "RingInfo." + this);
                ringInfo.Text = "Multiplies " + RingType + " by " + RingValue;
                ringInfo.Color = Color.Red;
                ringInfo.Parent = infoList;
                infoList.Children.Insert(1, ringInfo);
            }
            else
            {
                TextGameObject ringInfo = new TextGameObject("Arial12", 0, 0, "RingInfo." + this);
                ringInfo.Text = "Adds " + RingValue + " to " + RingType;
                ringInfo.Color = Color.Red;
                ringInfo.Parent = infoList;
                infoList.Children.Insert(1, ringInfo);
            }
        }*/
    }
    [Serializable]
    class RingEffect : ISerializable
    {
        protected RingType effectType;
        protected bool multiplier;
        protected double effectValue;

        public RingEffect(RingType effectType, bool multiplier, double effectValue)
        {
            this.effectType = effectType;
            this.multiplier = multiplier;
            this.effectValue = effectValue;
        }

        #region Serialization
        public RingEffect(SerializationInfo info, StreamingContext context)
        {
            effectType = (RingType)info.GetValue("effectType", typeof(RingType));
            multiplier = info.GetBoolean("multiplier");
            effectValue = info.GetDouble("effectValue");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("effectType", effectType, typeof(RingType));
            info.AddValue("multiplier", multiplier);
            info.AddValue("effectValue", effectValue);
        }
        #endregion

        public RingType EffectType { get { return effectType; } }
        public bool Multiplier { get { return multiplier; } }
        public double EffectValue { get { return effectValue; } }
    }

    [Serializable]
    class ReflectionEffect : RingEffect
    {
        protected double power;
        protected double chance;

        public ReflectionEffect(double power, double chance) : base(RingType.Reflection, false, -1)
        {
            this.power = power;
            this.chance = chance;
        }

        #region Serialization
        public ReflectionEffect(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            effectType = (RingType)info.GetValue("effectType", typeof(RingType));
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

        public double Power { get { return power; } }
        public double Chance { get { return chance; } }
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
