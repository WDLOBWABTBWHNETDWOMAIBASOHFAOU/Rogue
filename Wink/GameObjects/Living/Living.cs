using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public abstract partial class Living : AnimatedGameObject, ITileObject, IGameObjectContainer, IViewer
    {
        private int timeleft;
        private bool startTimer;

        protected string idleAnimation, moveAnimation, dieAnimation;
        private string dieSound;
        
        protected float viewDistance;
        
        public InventoryBox Inventory { get { return inventory; } }
        private InventoryBox inventory;

        #region EquipmentSlots
        private GameObjectList equipmentSlots;
        public GameObjectList EquipmentSlots
        {
            get { return equipmentSlots; }
        }
        private EquipmentSlot Weapon { get { return equipmentSlots.Find("weaponSlot") as EquipmentSlot; } }
        private EquipmentSlot Body { get { return equipmentSlots.Find("bodySlot") as EquipmentSlot; } }
        private EquipmentSlot Ring1 { get { return equipmentSlots.Find("ringSlot1") as EquipmentSlot; } }
        private EquipmentSlot Ring2 { get { return equipmentSlots.Find("ringSlot2") as EquipmentSlot; } }
        #endregion

        public float ViewDistance
        {
            get { return viewDistance; }
        }
        public Tile Tile
        {
            get
            {
                if (parent != null)
                    return parent.Parent as Tile;
                else
                    return null;
            }
        }
        public virtual Point PointInTile
        {
            get { return new Point(Tile.TileWidth / 2, Tile.TileHeight); }
        }
        public virtual bool BlocksTile
        {
            get { return true; }
        }

        public Living(int layer = 0, string id = "", float FOVlength = 8.5f, float scale = 1.0f) : base(layer, id, scale)
        {
            SetStats();
            InitAnimation();
            timeleft = 1000;
            viewDistance = FOVlength;

            GameObjectGrid itemGrid = new GameObjectGrid(3, 6, 0, "");
            inventory = new InventoryBox(itemGrid);

            equipmentSlots = new GameObjectList();
            equipmentSlots.Add(new EquipmentSlot(typeof(WeaponEquipment), id: "weaponSlot"));
            equipmentSlots.Add(new EquipmentSlot(typeof(BodyEquipment), id: "bodySlot"));
            equipmentSlots.Add(new EquipmentSlot(typeof(RingEquipment), id: "ringSlot1"));
            equipmentSlots.Add(new EquipmentSlot(typeof(RingEquipment), id: "ringSlot2"));
        }

        #region Serialization
        public Living(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            //timer and turn
            timeleft = info.GetInt32("timeleft");
            startTimer = info.GetBoolean("startTimer");

            //animations
            idleAnimation = info.GetString("idleAnimation");
            moveAnimation = info.GetString("moveAnimation");
            dieAnimation = info.GetString("dieAnimation");
            dieSound = info.GetString("dieSound");

            if (context.GetVars().GUIDSerialization)
            {
                inventory = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("inventoryGUID"))) as InventoryBox;
                equipmentSlots = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("equipmentSlotsGUID"))) as GameObjectList;
            }
            else
            {
                inventory = info.GetValue("inventory", typeof(InventoryBox)) as InventoryBox;
                equipmentSlots = info.GetValue("equipmentSlots", typeof(GameObjectList)) as GameObjectList;
            }

            manaPoints = info.GetInt32("manaPoints");
            healthPoints = info.GetInt32("healthPoints");
            actionPoints = info.GetInt32("actionPoints");
            baseAttack = info.GetInt32("baseAttack");
            strength = info.GetInt32("strength");
            dexterity = info.GetInt32("dexterity");
            intelligence = info.GetInt32("intelligence");
            wisdom = info.GetInt32("wisdom");
            luck = info.GetInt32("luck");
            vitality = info.GetInt32("vitality");
            creatureLevel = info.GetInt32("creatureLevel");
            baseReach = info.GetInt32("baseReach");
            viewDistance = info.GetInt32("viewDistance");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            // time and turn
            info.AddValue("timeleft", timeleft);
            info.AddValue("startTimer", startTimer);

            //animations
            info.AddValue("idleAnimation", idleAnimation);
            info.AddValue("moveAnimation", moveAnimation);
            info.AddValue("dieAnimation", dieAnimation);
            info.AddValue("dieSound", dieSound);
            
            if (context.GetVars().GUIDSerialization)
            {
                info.AddValue("inventoryGUID", inventory.GUID.ToString());
                info.AddValue("equipmentSlotsGUID", equipmentSlots.GUID.ToString());
            }
            else
            {
                info.AddValue("inventory", inventory);
                info.AddValue("equipmentSlots", equipmentSlots);
            }

            info.AddValue("manaPoints", manaPoints);
            info.AddValue("healthPoints", healthPoints);
            info.AddValue("actionPoints", actionPoints);
            info.AddValue("baseAttack", baseAttack);
            info.AddValue("strength", strength);
            info.AddValue("dexterity", dexterity);
            info.AddValue("intelligence", intelligence);
            info.AddValue("creatureLevel", creatureLevel);
            info.AddValue("vitality", vitality);
            info.AddValue("wisdom", wisdom);
            info.AddValue("luck", luck);
            info.AddValue("baseReach", baseReach);
            info.AddValue("viewDistance", viewDistance);
        }
        #endregion

        public override void Replace(GameObject replacement)
        {
            if (inventory != null && inventory.GUID == replacement.GUID)
                inventory = replacement as InventoryBox;

            base.Replace(replacement);
        }

        public void ComputeVisibility()
        {
            TileField tf = GameWorld.Find("TileField") as TileField;
            Point pos = Tile.TilePosition;

            ShadowCast.ComputeVisibility(tf, pos.X, pos.Y, this);
            //skill idea: peek corner, allows the player to move its FOV position 1 tile in N,S,E or W direction,
            //allowing the player to peek around a corner into a halway whithout actualy stepping out
        }

        public List<GameObject> DoAllBehaviour()
        {
            List<GameObject> changedObjects = new List<GameObject>();
            if (Health > 0)
            {
                int previousActionPoints = int.MinValue;
                while (actionPoints > 0 && actionPoints != previousActionPoints)
                {
                    previousActionPoints = actionPoints;
                    DoBehaviour(changedObjects);
                }
            }
            else
                actionPoints = 0;

            return changedObjects;
        }

        protected abstract void DoBehaviour(List<GameObject> changedObjects);

        protected virtual void InitAnimation(string idleColor = "empty:64:64:10:Magenta")
        {
            //General animations
            idleAnimation = idleColor;
            moveAnimation = "empty:64:64:10:DarkBlue";
            dieAnimation = "empty:64:64:10:LightBlue";
            LoadAnimation(idleAnimation, "idle", true);
            LoadAnimation(moveAnimation, "move", true, 0.05f);
            LoadAnimation(dieAnimation, "die", false);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (healthPoints <= 0)
            {
                startTimer = true;
                DeathFeedback("die", dieSound);
                if (startTimer)
                {
                    if (timeleft <= 0)
                        Death();
                    else
                        timeleft -= gameTime.TotalGameTime.Seconds;
                }
            }
            else if (healthPoints > MaxHealth)
            {
                healthPoints = MaxHealth;
            }

            if (manaPoints > MaxMana)
            {
                manaPoints = MaxMana;
            }
            // Stijn zegt voer hier een ring bonus methode uit
            // met de manier waarop ik ring heb gemaakt valt er niks te bonussen
            // alles moet bij stat calculation worden afgehandeld
        }
        
        public virtual void MoveTo(Tile t)
        {
            Tile oldTile = Tile;
            if (oldTile != null)
                oldTile.Remove(this);

            if (!t.PutOnTile(this))
            {
                if (!oldTile.PutOnTile(this))
                    throw new Exception();
            }
            else if (Visible)
            {
                LocalServer.SendToClients(new LivingMoveAnimationEvent(this, t));
            }
        }

        public virtual List<GameObject> FindAll(Func<GameObject, bool> del)
        {
            List<GameObject> result = new List<GameObject>();
            result.AddRange(inventory.FindAll(del));
            result.AddRange(equipmentSlots.FindAll(del));
            return result;
        }

        public virtual GameObject Find(Func<GameObject, bool> del)
        {
            if (del.Invoke(inventory))
                return inventory;
            if (del.Invoke(equipmentSlots))
                return equipmentSlots;

            return inventory.Find(del) ?? equipmentSlots.Find(del);
        }
    }
}
