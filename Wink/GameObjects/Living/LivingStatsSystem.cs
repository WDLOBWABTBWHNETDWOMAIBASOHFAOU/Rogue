using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    public abstract partial class Living
    {
        public const int MaxActionPoints = 40;
        public const int BaseActionCost = 10;

        #region stats
        protected int manaPoints, healthPoints, actionPoints, baseAttack, baseArmor, creatureLevel, reach, currentReach;

        protected Dictionary<Stat, int> statsBase = new Dictionary<Stat, int>();
        public Dictionary<Stat, int> statsBonus = new Dictionary<Stat, int>();
        public Dictionary<Stat, double> statsMultiplier = new Dictionary<Stat, double>();

        public int GetStat(Stat s)
        {
            int stat = (int)((statsBase[s] + statsBonus[s]) * statsMultiplier[s]);
            return stat;
        }

        public int CreatureLevel { get { return creatureLevel; } }
        
        //base stats
        public int BaseReach { get { return reach; } }
        public int Reach { get { return currentReach; } set { currentReach = value; } }

        public float statAverige { get { return (statsBase[Stat.Dexterity] + statsBase[Stat.Intelligence] + statsBase[Stat.Luck] + statsBase[Stat.Strength] + statsBase[Stat.Vitality] + statsBase[Stat.Wisdom]) / 6; } }

        public int ActionPoints { get { return actionPoints; } set { actionPoints= System.Math.Min(value, MaxActionPoints); } }

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
            int maxHP = (int)CalculateValue(40, GetStat(Stat.Vitality) - 1, 0.1);
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
            int maxManaPoints = (int)CalculateValue(50, GetStat(Stat.Wisdom), 0.1);
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
            //set stats
            statsBase.Add(Stat.Dexterity, dexterity);
            statsBase.Add(Stat.Intelligence, intelligence);
            statsBase.Add(Stat.Luck, luck);
            statsBase.Add(Stat.Strength, strength);
            statsBase.Add(Stat.Vitality, vitality);
            statsBase.Add(Stat.Wisdom, wisdom);

            statsBonus.Add(Stat.Dexterity, 0);
            statsBonus.Add(Stat.Intelligence, 0);
            statsBonus.Add(Stat.Luck, 0);
            statsBonus.Add(Stat.Strength, 0);
            statsBonus.Add(Stat.Vitality, 0);
            statsBonus.Add(Stat.Wisdom, 0);

            statsMultiplier.Add(Stat.Dexterity, 1);
            statsMultiplier.Add(Stat.Intelligence, 1);
            statsMultiplier.Add(Stat.Luck, 1);
            statsMultiplier.Add(Stat.Strength, 1);
            statsMultiplier.Add(Stat.Vitality, 1);
            statsMultiplier.Add(Stat.Wisdom, 1);

            //set misc
            this.creatureLevel = creatureLevel;
            this.reach = baseReach;
            currentReach = baseReach;
            this.baseAttack = baseAttack;
            this.baseArmor = baseArmor;
            healthPoints = MaxHP();
            manaPoints = MaxManaPoints();
        }

        public void resetMultipliers()
        {
            foreach (Stat s in statsMultiplier.Keys)
            {
                statsMultiplier[s] = 1;
            }
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
            double hitChance = CalculateValue(0.7, GetStat(Stat.Luck), 0.1);
            return hitChance;
        }

        /// <summary>
        /// returns DodgeChance based on stats
        /// </summary>
        /// <returns></returns>
        public double DodgeChance()
        {
            double dodgeChance = CalculateValue(0.3, GetStat(Stat.Luck), 0.1);
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
            int attackValue = (int)CalculateValue(attack, GetStat(Stat.Strength), mod);

            return attackValue;
        }

        /// <summary>
        /// returns armorvalue based on the summ the equiped armoritems
        /// </summary>
        /// <returns></returns>
        protected int ArmorValue(DamageType damageType)
        {
            int armorValue = baseArmor + equipedItems.TotalArmorvalue(damageType);
            if (armorValue <= 0)
                armorValue = 1;

            return armorValue;
        }
    }
}
