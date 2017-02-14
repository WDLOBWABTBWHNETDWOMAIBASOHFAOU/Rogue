
namespace Wink
{
    class ArcherFactory : EnemyFactory
    {
        public ArcherFactory(int floorNumber) : base(floorNumber)
        {
        }

        public override Enemy CreateEnemy()
        {
            Enemy enemy = new Enemy(floorNumber, 0, "Enemy : Archer");
            if (WeaponChance < GameEnvironment.Random.Next(100))
            {
                RestrictedItemSlot weaponslot = enemy.EquipmentSlots.Find("weaponSlot") as RestrictedItemSlot;
                weaponslot.ChangeItem(new WeaponEquipment(floorNumber, WeaponType.bow));
            }
            if (ArmorChance < GameEnvironment.Random.Next(100))
            {
                RestrictedItemSlot bodyslot = enemy.EquipmentSlots.Find("bodySlot") as RestrictedItemSlot;
                //bodyslot.ChangeItem(new BodyEquipment(floorNumber, 2, ArmorType.normal));
            }

            int eLvl = ELvl;
            enemy.SetStats(eLvl, 2 + (eLvl / 2), 1 + (eLvl / 2), 3 + (eLvl), 1 + (eLvl / 2), 1 + (eLvl / 2), 3 + (eLvl), 20 + eLvl * 3, 2, 1);
            return enemy;
        }
    }
}
