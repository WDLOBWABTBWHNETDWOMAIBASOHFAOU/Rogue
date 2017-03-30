using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Wink
{
    public enum EnemyType { warrior, archer, mage, random }
    [Serializable]

    public class Enemy : Living, IGUIGameObject
    {
        private Bar<Enemy> hpBar;
        private EnemyType type;
        private int floorNumber;

        public int FloorNumber
        {
            get { return floorNumber; }
        }

        /// <summary>
        /// Return True if health > 0 meaning the enemy is blocking the tile it's standing on
        /// </summary>
        public override bool BlocksTile
        {
            get { return Health > 0; }
        }

        public override Vector2 Position
        {
            get { return base.Position; }
            set
            {
                base.Position = value;
                if (hpBar != null)
                    hpBar.Position = GlobalPosition - new Vector2(Math.Abs(64 - hpBar.Width) / 2, 0) - origin;
            }
        }

        /// <summary>
        /// Create a new Enemy object
        /// </summary>
        /// <param name="layer">The layer for drawing the object</param>
        /// <param name="floorNumber">The floor the Enemy is placed on</param>
        /// <param name="enemyType">The enemy type</param>
        /// <param name="id">The (unique) object ID</param>
        /// <param name="FOVlength">The view distance for the Enemy</param>
        /// <param name="scale">The scale (multiplier) for the sprite size</param>
        public Enemy(int layer, int floorNumber, EnemyType enemyType = EnemyType.random, string id = "Enemy", float FOVlength = 8.5f) : base(layer, id, FOVlength)
        {
            if (floorNumber < 1)
                floorNumber = 1;

            this.floorNumber = floorNumber;
            this.type = SetupType(enemyType, floorNumber);
            InitAnimationVariables();
            LoadAnimations();
            PlayAnimation("idle");
        }

        /// <summary>
        /// Set up the enemy
        /// </summary>
        /// <param name="etype">The Enemy type</param>
        /// <param name="floorNumber">The floor the enemy is on</param>
        private EnemyType SetupType(EnemyType etype, int floorNumber)
        {
            if (etype == EnemyType.random)
            {
                //select random armorType
                Array eTypeValues = Enum.GetValues(typeof(EnemyType));
                etype = (EnemyType)eTypeValues.GetValue(GameEnvironment.Random.Next(eTypeValues.Length - 1));
            }

            id += " : " + etype.ToString();
            int eLvl = GameEnvironment.Random.Next(1, floorNumber);
            int weaponChance = 15 * floorNumber; // higher chance the deeper you go
            int armorChance = 15 * floorNumber;  //

            switch (etype)
            {
                case EnemyType.warrior:
                    //if (weaponChance < GameEnvironment.Random.Next(100))
                    //{
                    //    //RestrictedItemSlot weaponslot = EquipmentSlots.Find("weaponSlot") as RestrictedItemSlot;
                    //    weaponslot.ChangeItem(new MeleeWeapon("",30));
                    //}
                    //if (armorChance < GameEnvironment.Random.Next(100))
                    //{
                    //    RestrictedItemSlot bodyslot = EquipmentSlots.Find("bodySlot") as RestrictedItemSlot;
                    //    // bodyslot.ChangeItem(new BodyEquipment(floorNumber, 2, ArmorType.normal));
                    //}
                    SetStats(eLvl, 3 + (eLvl), 3 + (eLvl), 2 + (eLvl / 2), 1 + (eLvl / 2), 1 + (eLvl / 2), 2 + (eLvl / 2), 20 + eLvl * 3, 2, 1);
                    break;
                case EnemyType.archer:
                    //if (weaponChance < GameEnvironment.Random.Next(100))
                    //{
                    //    RestrictedItemSlot weaponslot = EquipmentSlots.Find("weaponSlot") as RestrictedItemSlot;
                    //    weaponslot.ChangeItem(new RangedWeapon("", 30));
                    //}
                    //if (armorChance < GameEnvironment.Random.Next(100))
                    //{
                    //    RestrictedItemSlot bodyslot = EquipmentSlots.Find("bodySlot") as RestrictedItemSlot;
                    //    //bodyslot.ChangeItem(new BodyEquipment(floorNumber, 2, ArmorType.normal));
                    //}
                    SetStats(eLvl, 2 + (eLvl / 2), 1 + (eLvl / 2), 3 + (eLvl), 1 + (eLvl / 2), 1 + (eLvl / 2), 3 + (eLvl), 20 + eLvl * 3, 2, 1);
                    break;
                case EnemyType.mage:
                    //if (weaponChance < GameEnvironment.Random.Next(100))
                    //{
                    //    RestrictedItemSlot weaponslot = EquipmentSlots.Find("weaponSlot") as RestrictedItemSlot;
                    //    weaponslot.ChangeItem(new MageWeapon("", 30));
                    //}
                    //if (armorChance < GameEnvironment.Random.Next(100))
                    //{
                    //    RestrictedItemSlot bodyslot = EquipmentSlots.Find("bodySlot") as RestrictedItemSlot;
                    //    //bodyslot.ChangeItem(new BodyEquipment(floorNumber, 2, ArmorType.robes));
                    //}
                    SetStats(eLvl, 1 + (eLvl / 2), 1 + (eLvl / 2), 1 + (eLvl / 2), 3 + (eLvl), 3 + (eLvl), 1 + (eLvl / 2), 20 + eLvl * 3, 2, 2);
                    break;
                default:
                    throw new Exception("invalid enemy type");
            }
            return etype;
        }

        #region Serialization
        public Enemy(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            type = (EnemyType)info.GetValue("type", typeof(EnemyType));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("type", type);
        }
        #endregion

        protected override void InitAnimationVariables()
        {
            switch (type)
            {
                case EnemyType.warrior:
                    idleAnimation = "Sprites/Living/norbert";
                    moveAnimation = "Sprites/Living/norbert";
                    dieAnimation = "Sprites/Living/norbert";
                    break;
                case EnemyType.archer:
                    idleAnimation = "Sprites/Living/centaur";
                    moveAnimation = "Sprites/Living/centaur";
                    dieAnimation = "Sprites/Living/centaur";
                    break;
                case EnemyType.mage:
                    idleAnimation = "Sprites/Living/francis";
                    moveAnimation = "Sprites/Living/francis";
                    dieAnimation = "Sprites/Living/francis";
                    break;
            }
        }

        public override void Death()
        {
            List<Player> playerList = (GameWorld.FindAll(p => p is Player).Cast<Player>().ToList());
            foreach (Player p in playerList)
            {
                p.ReceiveExp(0, this);
            }

            //Drop equipment/loot, remove itself from world, etc
            Tile tile = Tile;
            if (tile != null)
                tile.RemoveImmediatly(this);

            LootSack ls = new LootSack(this);
            tile.PutOnTile(ls);

            List<GameObject> changes = new List<GameObject>() { ls, Inventory, tile.OnTile };
            changes.AddRange(Inventory.Objects.Cast<GameObject>().ToList());
            changes.AddRange(Inventory.Items);
            LocalServer.SendToClients(new LevelChangedEvent(changes));

            base.Death();
        }

        protected override void DoBehaviour(HashSet<GameObject> changedObjects)
        {
            //TODO: replace by closest player.
            Player player = GameWorld.Find(Player.LocalPlayerName) as Player;

            if (player.Tile.SeenBy.ContainsKey(this))
            {
                bool ableToHit = AttackEvent.AbleToHit(this, player.Tile, Reach);
                if (ableToHit)
                {
                    Attack(player);
                    changedObjects.Add(player);

                    int cost = (int)(BaseActionCost*equipedItems.MoveCostModAverige());
                    actionPoints -= cost;
                }
                else
                {
                    GoTo(changedObjects, player);
                }
            }
            else
            {
                Idle();
            }
        }

        /// <summary>
        /// Pathfind towards a given player
        /// </summary>
        /// <param name="player">The player to target with Pathfinding</param>
        public virtual void GoTo(HashSet<GameObject> changedObjects, Player player)
        {
            TileField tf = GameWorld.Find("TileField") as TileField;
            PathFinder pf = new PathFinder(tf);

            List<Tile> path = pf.ShortestPath(Tile, player.Tile);
            // TODO?:(assuming there are tiles that cannot be walked over but can be fired over)
            // check if there is a path to a spot that can hit the player (move closer water to fire over it)
            if (path.Count > 0)
            {
                changedObjects.Add(this);
                changedObjects.Add(Tile.OnTile);
                changedObjects.Add(path[0].OnTile);

                MoveTo(path[0]);
                actionPoints -= BaseActionCost;
            }
            else
            { //No path possible.
                Idle();
            }
        }

        private void Idle()
        {
            //TODO: implement idle behaviour (seeing the player part done)
            //If this is reached the enemy has no other options than to skip its turn.
            actionPoints = 0;
        }
        
        public override void HandleInput(InputHelper inputHelper)
        {
            if (Health > 0 && animations["die"] != CurrentAnimation)
            {
                Action onLeftClick = () =>
                {
                    Player player = GameWorld.Find(Player.LocalPlayerName) as Player;
                    AttackEvent aE = new AttackEvent(player, this);
                    Server.Send(aE);
                };

                Action onRightClick = () =>
                {
                    Player player = GameWorld.Find(Player.LocalPlayerName) as Player;
                    SkillEvent sE = new SkillEvent(player, this);
                    Server.Send(sE);                
                };
            
                inputHelper.IfMouseLeftButtonPressedOn(this, onLeftClick);
                inputHelper.IfMouseRightButtonPressedOn(this, onRightClick);

                base.HandleInput(inputHelper);
            }
        }

        public void InitGUI(Dictionary<string, object> guiState)
        {
            if (Health > 0)
            {
                SpriteFont textfieldFont = GameEnvironment.AssetManager.GetFont("Arial26");
                hpBar = new Bar<Enemy>(this, e => e.Health, e => e.MaxHealth(), textfieldFont, Color.Red, "", 2, "HealthBar" + guid.ToString(), 1.0f, 1f, false);
                (GameWorld.Find("PlayingGui") as PlayingGUI).Add(hpBar);
                hpBar.Visible = !Tile.Visible ? false : Visible;
                hpBar.Position = Tile.GlobalPosition - new Vector2(Math.Abs(Tile.Width - hpBar.Width) / 2, 0);
            }
        }

        public void CleanupGUI(Dictionary<string, object> guiState)
        {
            if (hpBar != null && GameWorld != null)
            {
                PlayingGUI pg = GameWorld.Find("PlayingGui") as PlayingGUI;
                pg.RemoveImmediatly(hpBar);
            }
        }
    }
}