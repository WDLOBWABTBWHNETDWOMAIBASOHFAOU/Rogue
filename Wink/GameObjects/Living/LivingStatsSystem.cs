namespace Wink
{
    public abstract partial class Living : AnimatedGameObject
    {
        public const int MaxActionPoints = 4;

        protected int manaPoints, healthPoints, actionPoints, baseAttack, baseArmor, strength, dexterity, intelligence, creatureLevel, reach;

        public int Dexterity { get { return dexterity; } }
        public int ActionPoints { get { return actionPoints; } set { actionPoints = value; } }

        public int Health { get { return healthPoints; } set { healthPoints = value; } }
        public int Mana { get { return manaPoints; } }


        public int Intelligence
        {
            get
            {

                return intelligence;
            }
        }

        /// <summary>
        /// Sets starting stats when the living object is created
        /// </summary>
        /// <param name="creatureLevel"></param>
        /// <param name="strength"></param>
        /// <param name="dexterity"></param>
        /// <param name="intelligence"></param>
        /// <param name="baseAttack">unarmed attackValue</param>
        protected void SetStats(int creatureLevel = 1, int strength = 2, int dexterity = 2, int intelligence = 2, int baseAttack = 40, int baseArmor = 5)
        {
            this.creatureLevel = creatureLevel;
            this.strength = strength;
            this.dexterity = dexterity;
            this.intelligence = intelligence;
            this.baseAttack = baseAttack;
            this.baseArmor = baseArmor;
            actionPoints = MaxActionPoints;
            reach = 1;
            healthPoints = MaxHP();
            manaPoints = MaxManaPoints();
        }

        /// <summary>
        /// Calculetes a stat based value for living objects
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="stat"></param>
        /// <param name="extra">Sum of extra effects</param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        protected double CalculateValue(double baseValue, int stat = 0, double modifier = 1, double extra = 0)
        {
            double value = baseValue + modifier * stat + extra;
            return value;
        }

        /// <summary>
        /// returns the maximum of healtpoints the living object can have
        /// </summary>
        /// <returns></returns>
        protected int MaxHP()
        {
            int maxHP = (int)CalculateValue(40, creatureLevel - 1, 4);
            return maxHP;
        }
        public int MaxHealth { get {return MaxHP(); } }

        /// <summary>
        /// returns the maximum of manapoints the living object can have
        /// </summary>
        /// <returns></returns>
        protected int MaxManaPoints()
        {
            int maxManaPoints = (int)CalculateValue(50, intelligence, 15);
            return maxManaPoints;
        }
        public int MaxMana { get { return MaxManaPoints(); } }

        /// <summary>
        /// returns HitChance based on stats
        /// </summary>
        /// <returns></returns>
        protected double HitChance()
        {
            double hitChance = CalculateValue(0.7, dexterity, 0.01);
            return hitChance;
        }

        /// <summary>
        /// returns DodgeChance based on stats
        /// </summary>
        /// <returns></returns>
        protected double DodgeChance()
        {
            double dodgeChance = CalculateValue(0.3, dexterity, 0.01);
            return dodgeChance;
        }

        public int Reach
        {
            get
            {
                if (weapon.SlotItem != null)
                {
                    WeaponEquipment weaponItem = weapon.SlotItem as WeaponEquipment;
                    reach = weaponItem.Reach;
                }
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

            if (weapon.SlotItem !=null)
            {
                WeaponEquipment weaponItem = weapon.SlotItem as WeaponEquipment;

                if(weaponItem.StrRequirement > strength)
                {
                    int dif = weaponItem.StrRequirement - strength;
                    double penaltyMod = 0.4; // needs balancing, possibly dependent on weapon
                    attack = (int)(weaponItem.BaseDamage - weaponItem.BaseDamage*(dif * penaltyMod));
                }
                else
                {
                    attack = weaponItem.BaseDamage;
                    mod = weaponItem.ScalingFactor;
                }
            }            
            int attackValue = (int)CalculateValue(attack, strength,mod);     

            return attackValue;
        }

        /// <summary>
        /// returns armorvalue based on the summ the equiped armoritems
        /// </summary>
        /// <returns></returns>
        protected int ArmorValue()
        {
            int armorValue=baseArmor;
            if (body.SlotItem != null)
            {
                BodyEquipment ArmorItem = body.SlotItem as BodyEquipment;
                if(ArmorItem.StrRequirement > strength)
                {
                    int dif = ArmorItem.StrRequirement - strength;
                    double penaltyMod = 0.2;// needs balancing, possibly dependent on armor
                    armorValue = baseArmor + (int)(ArmorItem.ArmorValue - ArmorItem.ArmorValue*(dif * penaltyMod));
                }
                else
                {
                    armorValue = baseArmor + ArmorItem.ArmorValue;
                }
            }

            if(armorValue <= 0)
            {
                armorValue = 1;
            }
            return armorValue;
        }
    }
}
