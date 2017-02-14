
namespace Wink
{
    class WarriorFactory : EnemyFactory
    {
        public WarriorFactory(int floorNumber) : base(floorNumber)
        {
        }

        public override Enemy CreateEnemy()
        {
            Enemy enemy = new Enemy(floorNumber, 0, "Enemy : Warrior");
            if (WeaponChance < GameEnvironment.Random.Next(100))
            {
                RestrictedItemSlot weaponslot = enemy.EquipmentSlots.Find("weaponSlot") as RestrictedItemSlot;
                weaponslot.ChangeItem(new WeaponEquipment(floorNumber, WeaponType.melee));
            }
            if (ArmorChance < GameEnvironment.Random.Next(100))
            {
                RestrictedItemSlot bodyslot = enemy.EquipmentSlots.Find("bodySlot") as RestrictedItemSlot;
                // bodyslot.ChangeItem(new BodyEquipment(floorNumber, 2, ArmorType.normal));
            }

            int eLvl = ELvl;
            enemy.SetStats(eLvl, 3 + (eLvl), 3 + (eLvl), 2 + (eLvl / 2), 1 + (eLvl / 2), 1 + (eLvl / 2), 2 + (eLvl / 2), 20 + eLvl * 3, 2, 1);
            return enemy;
        }
    }
}
