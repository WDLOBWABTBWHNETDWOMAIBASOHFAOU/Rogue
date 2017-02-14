using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wink
{
    public abstract class EnemyFactory
    {
        protected int floorNumber;

        protected int ELvl
        {
            get { return GameEnvironment.Random.Next(1, floorNumber); }
        }
        protected int WeaponChance
        {
            get { return 15 * floorNumber; } //higher chance the deeper you go
        }
        protected int ArmorChance
        {
            get { return 15 * floorNumber; } //higher chance the deeper you go
        }

        public EnemyFactory(int floorNumber)
        {
            this.floorNumber = floorNumber;
        }

        public static Enemy CreateRandomEnemy(int floorNumber)
        {
            Assembly assembly = typeof(EnemyFactory).Assembly;

            //Get all Types that inherit from Tetronimo
            IEnumerable<Type> factoryTypes = assembly.GetTypes().Where(t => t.BaseType == typeof(EnemyFactory));
            int r = GameEnvironment.Random.Next(factoryTypes.Count());

            //Use Activator class to create an instance of the randomly selected type.
            EnemyFactory factory = (EnemyFactory)Activator.CreateInstance(factoryTypes.ElementAt(r), new object[] { floorNumber });

            return factory.CreateEnemy();
        }

        public abstract Enemy CreateEnemy();
    }
}
