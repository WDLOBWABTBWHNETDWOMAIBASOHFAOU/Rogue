namespace Wink
{
    public abstract partial class Living : AnimatedGameObject
    {
        public const int MaxActionPoints = 40;
        public const int BaseActionCost = 10;

        #region stats
        protected int manaPoints, healthPoints, actionPoints, baseAttack, baseArmor ;
        protected int vitality, strength, dexterity, wisdom, luck, intelligence, creatureLevel, reach;
        protected int currentVitality, currentStrength, currentDexterity, currentWisdom, currentLuck, currentIntelligence,currentReach;

        public int CreatureLevel { get { return creatureLevel; } }
        
        //base stats
        public int BaseDexterity { get { return dexterity; } }
        public int BaseIntelligence { get { return intelligence; } }
        public int BaseStrength { get { return strength; } }
        public int BaseWisdom { get { return wisdom; } }
        public int BaseVitality { get { return vitality; } }
        public int BaseLuck { get { return luck; } }
        public int BaseReach { get { return reach; } }

        public int GetBaseStat(Stat s)
        {
            switch (s)
            {
                case Stat.Vitality:
                    return BaseVitality;
                case Stat.Strength:
                    return BaseStrength;
                case Stat.Dexterity:
                    return BaseDexterity;
                case Stat.Wisdom:
                    return BaseWisdom;
                case Stat.Luck:
                    return BaseLuck;
                case Stat.Intelligence:
                    return Intelligence;
                default:
                    return int.MinValue;
            }
        }

        //current stats
        public int Dexterity { get { return currentDexterity; } set { currentDexterity = value; } }
        public int Intelligence { get { return currentIntelligence; } set { currentIntelligence = value; } }
        public int Strength { get { return currentStrength; } set { currentStrength = value; } }
        public int Wisdom { get { return currentWisdom; } set { currentWisdom = value; } }
        public int Vitality { get { return currentVitality; } set { currentVitality = value; } }
        public int Luck { get { return currentLuck; } set { currentLuck = value; } }
        public int Reach { get { return currentReach; } set { currentReach = value; } }

        public int GetStat(Stat s)
        {
            switch (s)
            {
                case Stat.Vitality:
                    return Vitality;
                case Stat.Strength:
                    return Strength;
                case Stat.Dexterity:
                    return Dexterity;
                case Stat.Wisdom:
                    return Wisdom;
                case Stat.Luck:
                    return Luck;
                case Stat.Intelligence:
                    return Intelligence;
                default:
                    return int.MinValue;
            }
        }

        public float statAverige { get { return (strength + dexterity + intelligence + wisdom + vitality + luck) / 6; } }//lowercases else the ring stat bonuses are taken in to account

        public int ActionPoints
        {
            get
            { return actionPoints; }
            set
            {
                if (value <= MaxActionPoints)
                    actionPoints = value;
                else
                    actionPoints = MaxActionPoints;
            }
        }

        #region Health
        public int Health
        {
            get
            { return healthPoints; }
            set
            {
                if (value <= MaxHealth)
                    healthPoints = value;
                else
                    healthPoints = MaxHealth;

                if (healthPoints <= 0)
                {
                    LocalServer.SendToClients(new DeathAnimationEvent(this, "Sounds/video_game_announcer_grunt"));
                    Death();
                }
            }
        }

        public int MaxHealth { get { return MaxHP(); } }
        
        protected int MaxHP()
        {
            int maxHP = (int)CalculateValue(40, Vitality - 1, 0.1);
            return maxHP;
        }
        
        #endregion

        #region Mana
        public int Mana
        {
            get
            { return manaPoints; }
            set
            {
                if (value <= MaxMana)
                    manaPoints = value;
                else
                    manaPoints = MaxMana;
            }
        }

        public int MaxMana { get { return MaxManaPoints(); } }

        protected int MaxManaPoints()
        {
            int maxManaPoints = (int)CalculateValue(50, Wisdom, 0.1);
            return maxManaPoints;
        }

        #endregion
        #endregion
        

        /// <summary>
        /// Sets starting stats when the living object is created
        /// </summary>
        /// <param name="creatureLevel">Level of the living object</param>
        /// <param name="vitality">base vitality</param>
        /// <param name="strength">base strength</param>
        /// <param name="dexterity">base dexterity</param>
        /// <param name="intelligence">base intelligence</param>
        /// <param name="wisdom">base wisdom</param>
        /// <param name="luck">base luck</param>
        /// <param name="baseAttack">unarmed attackValue</param>
        /// <param name="baseArmor">natural armorValue</param>
        /// <param name="baseReach">natural attackReach</param>
        public void SetStats(int creatureLevel = 1, int vitality = 2, int strength = 2, int dexterity = 2, int intelligence = 2, int wisdom = 2, int luck = 2, int baseAttack = 40, int baseArmor = 5, int baseReach = 1)
        {
            //set base stats
            this.creatureLevel = creatureLevel;
            this.vitality = vitality;
            this.strength = strength;
            this.dexterity = dexterity;
            this.intelligence = intelligence;
            this.wisdom = wisdom;
            this.luck = luck;
            this.reach = baseReach;

            //set current stats to base stats
            currentVitality = vitality;
            currentStrength = strength;
            currentDexterity = dexterity;
            currentIntelligence = intelligence;
            currentWisdom = wisdom;
            currentLuck = luck;
            currentReach = baseReach;

            //set misc
            this.baseAttack = baseAttack;
            this.baseArmor = baseArmor;
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
        /// returns HitChance based on stats
        /// </summary>
        /// <returns></returns>
        protected double HitChance()
        {
            double hitChance = CalculateValue(0.7, Luck, 0.1);
            return hitChance;
        }

        /// <summary>
        /// returns DodgeChance based on stats
        /// </summary>
        /// <returns></returns>
        public double DodgeChance()
        {
            double dodgeChance = CalculateValue(0.3, Luck, 0.1);
            return dodgeChance;
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
            int attackValue = (int)CalculateValue(attack, Strength, mod);

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
                ArmorEquipment ArmorItem = Body.SlotItem as ArmorEquipment;
                armorValue = ArmorItem.Value(this, damageType);
            }

            if (armorValue <= 0)
                armorValue = 1;

            return armorValue;
        }
    }
}
