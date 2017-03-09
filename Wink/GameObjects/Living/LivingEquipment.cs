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
        //public equipment accessors
        public float MoveMod { get { return equipedItems.MoveCostModAverige(); } }
        public GameObjectList Lootlist { get { return equipedItems.lootList(); } }//returns gameobjectlist so it's possible to acces the children but not the methods of LivingEquipment

        [Serializable]
        protected class LivingEquipment:GameObjectList
        {
            #region equipmentslots and accesors for whithin living
            private GameObjectList Armor;
            private GameObjectList Rings;

            private EquipmentSlot weaponSlot;
            public EquipmentSlot WeaponSlot { get { return weaponSlot; } }
            public WeaponEquipment Weapon { get { return weaponSlot.SlotItem as WeaponEquipment; } }
            
            private EquipmentSlot bodySlot;
            public EquipmentSlot BodySlot { get { return bodySlot; } }
            public ArmorEquipment BodyArmor { get { return bodySlot.SlotItem as ArmorEquipment; } }


            private EquipmentSlot headSlot;
            public EquipmentSlot HeadSlot { get { return headSlot; } }
            public ArmorEquipment HeadArmor { get { return bodySlot.SlotItem as ArmorEquipment; } }
            
            private EquipmentSlot ringSlot1;
            public EquipmentSlot RingSlot1 { get { return ringSlot1; } }
            public RingEquipment Ring1 { get { return ringSlot1.SlotItem as RingEquipment; } }


            private EquipmentSlot ringSlot2;
            public EquipmentSlot RingSlot2 { get { return ringSlot2; } }
            public RingEquipment Ring2 { get { return ringSlot2.SlotItem as RingEquipment; } }
            #endregion
            
            public LivingEquipment(Living partof)
            {                
                weaponSlot = new EquipmentSlot(typeof(WeaponEquipment), partof,"inventory/weaponSlot", id: "weaponSlot");
                Add(weaponSlot);

                Armor = new GameObjectList();
                Add(Armor);
                bodySlot = new EquipmentSlot(typeof(ChestArmor), partof, "inventory/bodySlot", id: "bodySlot");
                Armor.Add(bodySlot);
                headSlot = new EquipmentSlot(typeof(HeadEquipment), partof, "inventory/headSlot", id: "headSlot");
                Armor.Add(headSlot);

                Rings = new GameObjectList();
                Add(Rings);
                ringSlot1 = new EquipmentSlot(typeof(RingEquipment), partof, "inventory/ringSlot", id: "ringSlot1");
                Rings.Add(ringSlot1);
                ringSlot2 = new EquipmentSlot(typeof(RingEquipment), partof, "inventory/ringSlot", id: "ringSlot2");
                Rings.Add(ringSlot2);
            }


            #region Serialization
            public LivingEquipment(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                weaponSlot = info.GetValue("weaponSlot", typeof(EquipmentSlot)) as EquipmentSlot;

                Armor = info.GetValue("Armor",typeof(GameObjectList)) as GameObjectList;
                bodySlot = info.GetValue("bodySlot", typeof(EquipmentSlot)) as EquipmentSlot;
                headSlot = info.GetValue("headSlot", typeof(EquipmentSlot)) as EquipmentSlot;

                Rings = info.GetValue("Armor", typeof(GameObjectList)) as GameObjectList;
                ringSlot1 = info.GetValue("ringSlot1", typeof(EquipmentSlot)) as EquipmentSlot;
                ringSlot2 = info.GetValue("ringSlot2", typeof(EquipmentSlot)) as EquipmentSlot;
            }

            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue("weaponSlot", weaponSlot);
                info.AddValue("Armor", Armor);
                info.AddValue("bodySlot", bodySlot);
                info.AddValue("headSlot", headSlot);
                info.AddValue("Rings", Rings);
                info.AddValue("ringSlot1", ringSlot1);
                info.AddValue("ringSlot2", ringSlot2);
            }
            #endregion

            public int TotalArmorvalue(DamageType damageType)
            {
                int armorValue = 0;
                foreach(EquipmentSlot A in Armor.Children)
                {
                    if (A.SlotItem != null)
                    {
                        armorValue += (A.SlotItem as ArmorEquipment).Value(parent as Living, damageType);
                    }
                }
                return armorValue;
            }

            public float MoveCostModAverige()
            {
                float total = 0;
                int n = 0;
                foreach (EquipmentSlot A in Armor.Children)
                {
                    if (A.SlotItem != null)
                    {
                        total += (A.SlotItem as ArmorEquipment).WalkCostMod;
                    }
                    else
                    {
                        total += 1;
                    }
                    n++;
                }
                float moveModav = total/n;
                return moveModav;
            }

            public GameObjectList lootList()
            {
                GameObjectList lootlist = new GameObjectList();
                if(weaponSlot.SlotItem !=null)
                    lootlist.Add(weaponSlot.SlotItem);

                foreach(EquipmentSlot A in Armor.Children)
                {
                    if(A.SlotItem != null)
                        lootlist.Add(A.SlotItem);
                }
                foreach (EquipmentSlot R in Rings.Children)
                {
                    if (R.SlotItem != null)
                        lootlist.Add(R.SlotItem);
                }
                return lootlist;
            }
        }
    }
}
