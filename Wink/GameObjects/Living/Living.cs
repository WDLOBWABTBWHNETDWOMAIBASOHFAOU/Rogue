using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public abstract partial class Living : AnimatedGameObject, ITileObject, IGameObjectContainer, IViewer
    {
        protected string idleAnimation, moveAnimation, dieAnimation;
        private string dieSound;
        protected float viewDistance;
        private InventoryBox inventory;

        protected Skill currentSkill;
        public Skill CurrentSkill
        {
            get { return currentSkill; }
            set { currentSkill = value; }
        }

        private GameObjectList skillList;
        public GameObjectList SkillList
        {
            get { return skillList; }
        }

        #region EquipmentSlots
        // Accessors for all the different equipment slots
        private GameObjectList equipmentSlots;
        public GameObjectList EquipmentSlots
        {
            get { return equipmentSlots; }
        }
        private RestrictedItemSlot Weapon { get { return equipmentSlots.Find("weaponSlot") as RestrictedItemSlot; } }
        private RestrictedItemSlot Body { get { return equipmentSlots.Find("bodySlot") as RestrictedItemSlot; } }
        private RestrictedItemSlot Ring1 { get { return equipmentSlots.Find("ringSlot1") as RestrictedItemSlot; } }
        private RestrictedItemSlot Ring2 { get { return equipmentSlots.Find("ringSlot2") as RestrictedItemSlot; } }
        #endregion

        public InventoryBox Inventory
        {
            get { return inventory; }
        }
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

        /// <summary>
        /// Returns a point with position (0.5 * TileWidth, TileHeight)
        /// </summary>
        public virtual Point PointInTile
        {
            get { return new Point(Tile.TileWidth / 2, Tile.TileHeight); }
        }

        /// <summary>
        /// Return true because by default all living objects block the tile they're standing on
        /// </summary>
        public virtual bool BlocksTile
        {
            get { return true; }
        }

        /// <summary>
        /// Create a new Living object
        /// </summary>
        /// <param name="layer">The layer for drawing the object</param>
        /// <param name="id">The (unique) object ID</param>
        /// <param name="FOVlength">The view distance for the object</param>
        /// <param name="scale">The scale (multiplier) for the sprite size</param>
        public Living(int layer = 0, string id = "", float FOVlength = 8.5f, float scale = 1.0f) : base(layer, id, scale)
        {
            SetStats();
            viewDistance = FOVlength;

            skillList = new GameObjectList ();
            for (int y = 0; y < 1; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    skillList.Add(new RestrictedItemSlot(typeof(Skill), "inventory/slot", id:"skillSlot"+x));
                }
            }
            
            inventory = new InventoryBox(4, 4, 0, "");

            equipmentSlots = new GameObjectList();
            equipmentSlots.Add(new RestrictedItemSlot(typeof(WeaponEquipment), "inventory/weaponSlot", id: "weaponSlot"));
            equipmentSlots.Add(new RestrictedItemSlot(typeof(BodyEquipment), "inventory/bodySlot", id: "bodySlot"));
            equipmentSlots.Add(new RestrictedItemSlot(typeof(RingEquipment), "inventory/ringSlot", id: "ringSlot1"));
            equipmentSlots.Add(new RestrictedItemSlot(typeof(RingEquipment), "inventory/ringSlot", id: "ringSlot2"));
            equipmentSlots.Add(new RestrictedItemSlot(typeof(HeadEquipment), "inventory/headSlot", id: "headSlot"));

            InitAnimationVariables();
            LoadAnimations();
            PlayAnimation("idle");
        }

        #region Serialization
        public Living(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            //animations
            idleAnimation = info.GetString("idleAnimation");
            moveAnimation = info.GetString("moveAnimation");
            dieAnimation = info.GetString("dieAnimation");
            dieSound = info.GetString("dieSound");

            inventory = info.TryGUIDThenFull<InventoryBox>(context, "inventory");
            equipmentSlots = info.TryGUIDThenFull<GameObjectList>(context, "equipmentSlots");
            skillList = info.TryGUIDThenFull<GameObjectList>(context, "skillList");
            currentSkill = info.TryGUIDThenFull<Skill>(context, "currentSkill");
            
            //stats
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
            specialReach = info.GetInt32("specialReach");
            viewDistance = info.GetInt32("viewDistance");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            
            //animations
            info.AddValue("idleAnimation", idleAnimation);
            info.AddValue("moveAnimation", moveAnimation);
            info.AddValue("dieAnimation", dieAnimation);
            info.AddValue("dieSound", dieSound);

            SerializationHelper.Variables v = context.GetVars();
            if (v.FullySerializeEverything || v.FullySerialized.Contains(inventory.GUID))
                info.AddValue("inventory", inventory);
            else
                info.AddValue("inventoryGUID", inventory.GUID.ToString());
            
            if (v.FullySerializeEverything || v.FullySerialized.Contains(equipmentSlots.GUID))
                info.AddValue("equipmentSlots", equipmentSlots); 
            else
                info.AddValue("equipmentSlotsGUID", equipmentSlots.GUID.ToString());

            if (v.FullySerializeEverything || v.FullySerialized.Contains(skillList.GUID))
                info.AddValue("skillList", skillList);
            else
                info.AddValue("skillListGUID", skillList.GUID.ToString());

            if (currentSkill == null || v.FullySerializeEverything || v.FullySerialized.Contains(currentSkill.GUID))
                info.AddValue("currentSkill", currentSkill);
            else
                info.AddValue("currentSkillGUID", currentSkill.GUID.ToString());

            //stats
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
            info.AddValue("specialReach", specialReach);
            info.AddValue("viewDistance", viewDistance);
        }
        #endregion

        /// <summary>
        /// Replace the inventory with a given replacement
        /// </summary>
        /// <param name="replacement">The replacement object</param>
        public override void Replace(GameObject replacement)
        {
            if (inventory != null && inventory.GUID == replacement.GUID)
                inventory = replacement as InventoryBox;

            base.Replace(replacement);
        }

        /// <summary>
        /// Process which tiles the living object can and cannot see using ShadowCast
        /// </summary>
        public void ComputeVisibility()
        {
            TileField tf = GameWorld.Find("TileField") as TileField;
            Point pos = Tile.TilePosition;
            
            if (tf.Find(obj => obj is Tile && (obj as Tile).SeenBy.ContainsKey(this)) != null)
                foreach (Tile t in tf.Objects)
                    t.SeenBy.Remove(this);

            ShadowCast.ComputeVisibility(tf, pos.X, pos.Y, this);
            //skill idea: peek corner, allows the player to move its FOV position 1 tile in N,S,E or W direction,
            //allowing the player to peek around a corner into a halway whithout actualy stepping out
        }

        /// <summary>
        /// Do behavior until action points run out
        /// </summary>
        /// <returns>All objects that changed</returns>
        public HashSet<GameObject> DoAllBehaviour()
        {
            HashSet<GameObject> changedObjects = new HashSet<GameObject>();
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

        protected abstract void DoBehaviour(HashSet<GameObject> changedObjects);

        protected abstract void InitAnimationVariables();

        public override void LoadAnimations()
        {
            LoadAnimation(idleAnimation, "idle", true);
            LoadAnimation(moveAnimation, "move", true, 0.05f);
            LoadAnimation(dieAnimation, "die", false);
        }

        /// <summary>
        /// Updates the Living object
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        
        /// <summary>
        /// Puts the Living object on a given tile (if possible)
        /// </summary>
        /// <param name="t">Destination tile</param>
        public virtual void MoveTo(Tile t)
        {
            Tile oldTile = Tile;
            if (oldTile != null)
                oldTile.RemoveImmediatly(this);

            if (!t.PutOnTile(this))
            {
                if (!oldTile.PutOnTile(this))
                    throw new Exception();
            }
            else if (Visible)
            {   // Movement animation
                // This is oncomplete and therefore turned off
                LocalServer.SendToClients(new LivingMoveAnimationEvent(this, t));
            }
        }

        /// <summary>
        /// Find all children with a given condition
        /// </summary>
        /// <param name="del">The function to filter results with</param>
        /// <returns>All children to which "del" applies</returns>
        public virtual List<GameObject> FindAll(Func<GameObject, bool> del)
        {
            List<GameObject> result = new List<GameObject>();
            if (inventory != null)
                result.AddRange(inventory.FindAll(del));
            if (equipmentSlots != null)
                result.AddRange(equipmentSlots.FindAll(del));
            if (skillList != null)
                result.AddRange(skillList.FindAll(del));
            return result;
        }

        /// <summary>
        /// Find an object among children with a given condition
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public virtual GameObject Find(Func<GameObject, bool> del)
        {
            if (inventory != null)
            {
                if (del.Invoke(inventory)) // Check if the inventory fits "del"
                    return inventory;
                GameObject invResult = inventory.Find(del); // Find the object matching "del" among the children of inventory
                if (invResult != null)
                    return invResult;
            }
            if (equipmentSlots != null)
            {
                if (del.Invoke(equipmentSlots)) // Check if equipmentSlots fits "del"
                    return equipmentSlots;
                GameObject eqResult = equipmentSlots.Find(del); // Find the object matching "del" among the children of equipmentSlots
                if (eqResult != null)
                    return eqResult;
            }
            if (skillList != null)
            {
                if (del.Invoke(skillList)) // Check if equipmentSlots fits "del"
                    return skillList;
                GameObject slResult = skillList.Find(del); // Find the object matching "del" among the children of equipmentSlots
                if (slResult != null)
                    return slResult;
            }
            return null;
        }
    }
}
