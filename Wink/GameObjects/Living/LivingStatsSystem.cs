﻿namespace Wink
{
    public abstract partial class Living : AnimatedGameObject
    {
        public const int MaxActionPoints = 35;
        public const int BaseActionCost = 10;

        protected int manaPoints, healthPoints, actionPoints, baseAttack, baseArmor, vitality, strength, dexterity, wisdom, luck, intelligence, creatureLevel, baseReach;

        public int Dexterity { get { return Ringbonus(RingType.Dexterity, dexterity); } }
        public int Intelligence { get { return Ringbonus(RingType.Intelligence, intelligence); } }
        public int Strength { get { return Ringbonus(RingType.Strength, strength); } }
        public int Wisdom { get { return Ringbonus(RingType.Wisdom, wisdom); } }
        public int Vitality { get { return Ringbonus(RingType.Vitality, vitality); } }
        public int Luck { get { return Ringbonus(RingType.Luck, luck); } }
        public int ActionPoints { get { return actionPoints; } set { actionPoints = value; } }
        public int Health { get { return healthPoints; } set { healthPoints = value; } }
        public int Mana { get { return manaPoints; } set { manaPoints = value; } }
        
        protected int Ringbonus(RingType ringType, int baseValue)
        {
            int i = 0;
            double p = 1;
            if (EquipmentSlots != null)
                foreach (ItemSlot slot in EquipmentSlots.Children)
                {
                    if (slot.SlotItem != null && slot.Id.Contains("ringSlot"))
                    {
                        RingEquipment ring = slot.SlotItem as RingEquipment;
                        foreach (RingEffect effect in ring.RingEffects)
                        {
                            if (effect.EffectType == ringType)
                            {
                                if (effect.Multiplier) { p *= effect.EffectValue; }
                                else { i += (int)effect.EffectValue; }
                            }
                        }
                    }
                }
            return (int)(baseValue * p + i);
        }

        /// <summary>
        /// Sets starting stats when the living object is created
        /// </summary>
        /// <param name="creatureLevel"></param>
        /// <param name="strength"></param>
        /// <param name="dexterity"></param>
        /// <param name="intelligence"></param>
        /// <param name="baseAttack">unarmed attackValue</param>
        /// <param name="baseArmor">natural armorValue</param>
        /// <param name="baseReach">natural attackReach</param>
        public void SetStats(int creatureLevel = 1, int vitality = 2, int strength = 2, int dexterity = 2, int intelligence = 2, int wisdom = 2, int luck = 2, int baseAttack = 40, int baseArmor = 5, int baseReach = 1)
        {
            this.creatureLevel = creatureLevel;
            this.vitality = vitality;
            this.strength = strength;
            this.dexterity = dexterity;
            this.intelligence = intelligence;
            this.wisdom = wisdom;
            this.luck = luck;
            this.baseAttack = baseAttack;
            this.baseArmor = baseArmor;
            
            this.baseReach = baseReach;
            healthPoints = MaxHP();
            manaPoints = MaxManaPoints();
        }

        /// <summary>
        /// Calculetes a stat based value for living objects
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="stat1"></param>
        /// <param name="extra">Sum of extra effects</param>
        /// <param name="modifier1"></param>
        /// <returns></returns>
        public double CalculateValue(double baseValue, int stat1 = 0, double modifier1 = 1, double extra = 0, int stat2 = 0, double modifier2 = 0, int stat3 = 0, double modifier3 = 0)
        {
            double value = baseValue + baseValue * modifier1 * stat1 + extra + baseValue * modifier2 * stat2 + baseValue * modifier3 * stat3;
            return value;
        }

        /// <summary>
        /// returns the maximum of healtpoints the living object can have
        /// </summary>
        /// <returns></returns>
        protected int MaxHP()
        {
            int maxHP = (int)CalculateValue(40, Vitality - 1, 0.1);
            return maxHP;
        }
        public int MaxHealth { get { return Ringbonus(RingType.Vitality, MaxHP()); } }

        /// <summary>
        /// returns the maximum of manapoints the living object can have
        /// </summary>
        /// <returns></returns>
        protected int MaxManaPoints()
        {
            int maxManaPoints = (int)CalculateValue(50, Wisdom, 0.1);
            return maxManaPoints;
        }
        public int MaxMana { get { return MaxManaPoints(); } }

        /// <summary>
        /// returns HitChance based on stats
        /// </summary>
        /// <returns></returns>
        protected double HitChance()
        {
            double hitChance = CalculateValue(0.7, Luck, 0.01);
            return hitChance;
        }

        /// <summary>
        /// returns DodgeChance based on stats
        /// </summary>
        /// <returns></returns>
        protected double DodgeChance()
        {
            double dodgeChance = CalculateValue(0.3, Luck, 0.01);
            return dodgeChance;
        }

        public int Reach
        {
            get
            {
                int reach;
                if (Weapon.SlotItem != null)
                {
                    WeaponEquipment weaponItem = Weapon.SlotItem as WeaponEquipment;
                    reach = weaponItem.Reach;
                }
                else
                    reach = baseReach;

                return reach;
            }
        }

        /// <summary>
        /// returns attackvalue based on the equiped weapon item (if unarmed, uses livingObject baseAttack)
        /// </summary>
        /// <returns></returns>
        protected int AttackValue()
        {
            // get the baseattack value of the equiped weapon, if non equiped use baseattack of living
            // min max base attack value for each weapon and random inbetween or random between 0.8 and 1.2 of base (for example)

            int attack = baseAttack;
            double mod = 1;
            int attackValue;
            if (Weapon.SlotItem !=null)
            {
                WeaponEquipment weaponItem = Weapon.SlotItem as WeaponEquipment;
                attackValue = weaponItem.Value(this);
            }
            else
                attackValue = (int)CalculateValue(attack, Strength, mod);

            return attackValue;
        }

        /// <summary>
        /// returns armorvalue based on the summ the equiped armoritems
        /// </summary>
        /// <returns></returns>
        protected int ArmorValue(DamageType damageType)
        {
            int armorValue = baseArmor;
            if (Body.SlotItem != null)
            {
                BodyEquipment ArmorItem = Body.SlotItem as BodyEquipment;
                armorValue = ArmorItem.Value(this, damageType);
            }

            if (armorValue <= 0)
                armorValue = 1;

            return armorValue;
        }
    }
}
