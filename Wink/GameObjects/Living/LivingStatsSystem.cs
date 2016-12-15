namespace Wink
{
    public abstract partial class Living : AnimatedGameObject
    {
        protected int manaPoints, healthPoints, actionPoints, baseAttack, strength, dexterity, intelligence, creatureLevel;

        public int Dexterity { get { return dexterity; } }

        public int health { get { return healthPoints; } }
        public int mana { get { return manaPoints; } }
        //protected IList<Equipment> EquipedItems;

        /// <summary>
        /// Sets starting stats when the living object is created
        /// </summary>
        /// <param name="creatureLevel"></param>
        /// <param name="strenght"></param>
        /// <param name="dexterity"></param>
        /// <param name="intelligence"></param>
        /// <param name="baseAttack">unarmed attackValue</param>
        protected void SetStats(int creatureLevel = 1, int strenght = 2, int dexterity = 2, int intelligence = 2, int baseAttack = 40)
        {
            this.creatureLevel = creatureLevel;
            this.strength = strenght;
            this.dexterity = dexterity;
            this.intelligence = intelligence;
            //EquipedItems = new List<Equipment>();
            this.baseAttack = baseAttack;
            actionPoints = 3;
        }

        /// <summary>
        /// Calculetes a stat based value for living objects
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="stat"></param>
        /// <param name="extra">Sum of extra effects</param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        protected double calculateValue(double baseValue, int stat = 0, double modifier = 1, double extra = 0)
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
            int maxHP=(int)calculateValue(40, creatureLevel - 1, 4);
            return maxHP;
        }
        public int MaxHealth { get {return MaxHP(); } }

        /// <summary>
        /// returns the maximum of manapoints the living object can have
        /// </summary>
        /// <returns></returns>
        protected int MaxManaPoints()
        {
            int maxManaPoints = (int)calculateValue(50, intelligence, 15);
            return maxManaPoints;
        }
        public int MaxMana { get { return MaxManaPoints(); } }

        /// <summary>
        /// returns HitChance based on stats
        /// </summary>
        /// <returns></returns>
        protected double HitChance()
        {
            double hitChance = calculateValue(0.7, dexterity, 0.01);
            return hitChance;
        }

        /// <summary>
        /// returns DodgeChance based on stats
        /// </summary>
        /// <returns></returns>
        protected double DodgeChance()
        {
            double dodgeChance = calculateValue(0.3, dexterity, 0.01);
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
            int attackValue = (int)calculateValue(attack, strength);     

            return attackValue;
        }

        /// <summary>
        /// returns armorvalue based on the summ the equiped armoritems
        /// </summary>
        /// <returns></returns>
        protected int ArmorValue()
        {
            int armorValue=0;
            
            //foreach(Armor a in EquipedItems)
            //{
                
            //    armorValue +=a.armorbase
            //}
            
            return armorValue;
        }
    }
}
