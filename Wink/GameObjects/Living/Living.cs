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
        protected LivingEquipment equipedItems;

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

        public InventoryBox Inventory
        {
            get { return inventory; }
        }

        public List<ItriggerEffect> triggerEffects;

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
            //SetStats();
            viewDistance = FOVlength;

            skillList = new GameObjectList ();
            for (int x = 0; x < 10; x++)
                skillList.Add(new RestrictedItemSlot(typeof(Skill), "inventory/slot", id:"skillSlot" + x));
            
            inventory = new InventoryBox(4, 4, 0, "");
            triggerEffects = new List<ItriggerEffect>();

            equipedItems = new LivingEquipment(this);//might give problems
            equipedItems.Parent = this;
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
            equipedItems = info.TryGUIDThenFull<LivingEquipment>(context, "equipedItems");
            skillList = info.TryGUIDThenFull<GameObjectList>(context, "skillList");
            currentSkill = info.TryGUIDThenFull<Skill>(context, "currentSkill");
            triggerEffects = info.GetValue("triggerEffects",typeof(List<ItriggerEffect>)) as List<ItriggerEffect>;

            //stats
            manaPoints = info.GetInt32("manaPoints");
            healthPoints = info.GetInt32("healthPoints");
            actionPoints = info.GetInt32("actionPoints");
            baseAttack = info.GetInt32("baseAttack");
            creatureLevel = info.GetInt32("creatureLevel");
            reach = info.GetInt32("baseReach");
            currentReach = info.GetInt32("currentReach");
            viewDistance = info.GetInt32("viewDistance");

            statsBase = info.GetValue("statsBase", typeof(Dictionary<Stat, int>)) as Dictionary<Stat, int>;
            statsBonus = info.GetValue("statsBonus", typeof(Dictionary<Stat, int>)) as Dictionary<Stat, int>;
            statsMultiplier = info.GetValue("statsMultiplier", typeof(Dictionary<Stat, double>)) as Dictionary<Stat, double>;
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

            if (v.FullySerializeEverything || v.FullySerialized.Contains(equipedItems.GUID))
                info.AddValue("equipedItems", equipedItems);
            else
                info.AddValue("equipedItemsGUID", equipedItems.GUID.ToString());

            if (v.FullySerializeEverything || v.FullySerialized.Contains(skillList.GUID))
                info.AddValue("skillList", skillList);
            else
                info.AddValue("skillListGUID", skillList.GUID.ToString());

            if (currentSkill == null || v.FullySerializeEverything || v.FullySerialized.Contains(currentSkill.GUID))
                info.AddValue("currentSkill", currentSkill);
            else
                info.AddValue("currentSkillGUID", currentSkill.GUID.ToString());

            info.AddValue("triggerEffects", triggerEffects);

            //stats
            info.AddValue("manaPoints", manaPoints);
            info.AddValue("healthPoints", healthPoints);
            info.AddValue("actionPoints", actionPoints);
            info.AddValue("baseAttack", baseAttack);

            info.AddValue("baseReach", reach);
            info.AddValue("currentReach", currentReach);
            info.AddValue("viewDistance", viewDistance);
            info.AddValue("creatureLevel", creatureLevel);

            info.AddValue("statsBase", statsBase);
            info.AddValue("statsBonus", statsBonus);
            info.AddValue("statsMultiplier", statsMultiplier);
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
            //allowing the player to peek around a corner into a halway without actualy stepping out
        }

        public void ExecuteTriggeredEffect(TriggerEffects type, Living target = null, double value = 0)
        {
            foreach (ItriggerEffect t in triggerEffects)
            {
                if(t.effect == type)
                {
                    t.ExecuteTrigger(this,target,value);
                }
            }
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
            InitAnimationVariables();
            LoadAnimation(idleAnimation, "idle", true);
            LoadAnimation(moveAnimation, "move", true, 0.05f);
            LoadAnimation(dieAnimation, "die", false);
            PlayAnimation("idle");
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
            { //Movement animation
                LocalServer.SendToClients(new LivingMoveAnimationEvent(this, t, "Sounds/Footsteps 2 steps"));
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
            if (equipedItems != null)
                result.AddRange(equipedItems.FindAll(del));
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
            if (equipedItems != null)
            {
                if (del.Invoke(equipedItems)) // Check if equipmentSlots fits "del"
                    return equipedItems;
                GameObject eqResult = equipedItems.Find(del); // Find the object matching "del" among the children of equipmentSlots
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
