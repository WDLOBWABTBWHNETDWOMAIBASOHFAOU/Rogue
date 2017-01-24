using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;
using System.IO;

namespace Wink
{
    [Serializable]
    class Boss : Enemy, IGUIGameObject
    {
        private Bar<Boss> hpBar;
        EnemyType type = EnemyType.random;
        int floorNumber;
        string enemySprite;
        int actionpoints_cooldown;
        int actionpoints_used = 0;
        bool special_ableToHit;
        double mod = 1;

        /// <summary>
        /// Return True if health > 0 meaning the enemy is blocking the tile it's standing on
        /// </summary>
        public override bool BlocksTile
        {
            get { return Health > 0; }
        }

        public Boss(int levelIndex) : base(0, 5)
        {
            floorNumber = levelIndex + 1;
            string path = "Content/Bosses/Boss.txt";
            if (File.Exists(path))
            {
                StreamReader fileReader = new StreamReader(path);
                string line = fileReader.ReadLine();

                string[] splitline = line.Split(new char[] {','}, StringSplitOptions.None);

                string string_type = splitline[0];
                switch(string_type)
                {
                    case "warrior":
                        type = EnemyType.warrior;
                        break;
                    case "archer":
                        type = EnemyType.archer;
                        break;
                    case "mage":
                        type = EnemyType.mage;
                        break;
                    case "random":
                        type = EnemyType.random;
                        break;
                }

                int actionpoints_cooldown_turns = Int32.Parse(splitline[1]);
                actionpoints_cooldown = actionpoints_cooldown_turns * MaxActionPoints;
                actionpoints_used = actionpoints_cooldown - MaxActionPoints;

                specialReach = Int32.Parse(splitline[2]);
                mod = Int32.Parse(splitline[3]);

                SetupType(type, floorNumber);

                InitAnimationVariables();

                //TODO calculate creature level and set stats accordingly
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Set up the enemy
        /// </summary>
        /// <param name="etype">The Enemy type</param>
        /// <param name="floorNumber">The floor the enemy is on</param>
        private void SetupType(EnemyType etype, int floorNumber)
        {
            if (etype == EnemyType.random)
            {
                //select random armorType
                Array eTypeValues = Enum.GetValues(typeof(EnemyType));
                etype = (EnemyType)eTypeValues.GetValue(GameEnvironment.Random.Next(eTypeValues.Length - 1));
            }
            id += " : " + etype.ToString();
            int eLvl = GameEnvironment.Random.Next(floorNumber, floorNumber + 5);

            switch (etype)
            {
                case EnemyType.warrior:
                    {
                        EquipmentSlot weaponslot = EquipmentSlots.Find("weaponSlot") as EquipmentSlot;
                        weaponslot.ChangeItem(new WeaponEquipment(floorNumber + 2, WeaponType.melee));
                        EquipmentSlot bodyslot = EquipmentSlots.Find("bodySlot") as EquipmentSlot;
                        //bodyslot.ChangeItem(new BodyEquipment(floorNumber/10, 2, ArmorType.normal));
                    SetStats(eLvl, 3 + (eLvl), 3 + (eLvl), 2 + (eLvl / 2), 1 + (eLvl / 2), 1 + (eLvl / 2), 2 + (eLvl / 2), 20 + eLvl * 3, 2, 1);
                    enemySprite = "empty:65:65:12:Brown";
                    }
                    
                    break;

                case EnemyType.archer:
                    {
                        EquipmentSlot weaponslot = EquipmentSlots.Find("weaponSlot") as EquipmentSlot;
                        weaponslot.ChangeItem(new WeaponEquipment(floorNumber + 2, WeaponType.bow));
                        EquipmentSlot bodyslot = EquipmentSlots.Find("bodySlot") as EquipmentSlot;
                        //bodyslot.ChangeItem(new BodyEquipment(floorNumber/10, 2, ArmorType.normal));
                    SetStats(eLvl, 2 + (eLvl / 2), 1 + (eLvl / 2), 3 + (eLvl), 1 + (eLvl / 2), 1 + (eLvl / 2), 3 + (eLvl), 20 + eLvl * 3, 2, 1);
                    enemySprite = "empty:65:65:12:Yellow";
                    }
                        
                    break;
                case EnemyType.mage:
                    {
                        EquipmentSlot weaponslot = EquipmentSlots.Find("weaponSlot") as EquipmentSlot;
                        weaponslot.ChangeItem(new WeaponEquipment(floorNumber + 2, WeaponType.staff));
                        EquipmentSlot bodyslot = EquipmentSlots.Find("bodySlot") as EquipmentSlot;
                        //bodyslot.ChangeItem(new BodyEquipment(floorNumber/10, 2, ArmorType.robes));
                    SetStats(eLvl, 1 + (eLvl / 2), 1 + (eLvl / 2), 1 + (eLvl / 2), 3 + (eLvl), 3 + (eLvl), 1 + (eLvl / 2), 20 + eLvl * 3, 2, 2);
                    enemySprite = "empty:65:65:12:CornflowerBlue";
                    }
                    
                        
                    break;
                default:
                    throw new Exception("invalid enemy type");
            }
            ItemSlot slot_0_1 = Inventory.ItemGrid[0, 1] as ItemSlot;
            slot_0_1.ChangeItem(new Potion(floorNumber + 5, 10, PotionType.Health));
            ItemSlot slot_1_1 = Inventory.ItemGrid[1, 1] as ItemSlot;
            slot_1_1.ChangeItem(new WeaponEquipment(floorNumber + 2));
            ItemSlot slot_2_1 = Inventory.ItemGrid[2, 1] as ItemSlot;
            slot_2_1.ChangeItem(new BodyEquipment(floorNumber + 2, 3));
        }

        #region Serialization
        public Boss(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        #endregion

        protected override void InitAnimationVariables()
        {
            base.InitAnimationVariables();
        }

        public override void Death()
        {
            // call recive exp for every player
            base.Death();
        }

        protected override void DoBehaviour(List<GameObject> changedObjects)
        {
            GoTo(changedObjects, GameWorld.Find(Player.LocalPlayerName) as Player);
            if (actionPoints <= 0)
            {
                actionpoints_used = actionpoints_used + MaxActionPoints;
            }
        }

        /// <summary>
        /// Pathfind towards a given player
        /// </summary>
        /// <param name="player">The player to target with Pathfinding</param>
        public override void GoTo(List<GameObject> changedObjects, Player player)
        {
            TileField tf = GameWorld.Find("TileField") as TileField;


            if (player.Tile.SeenBy.ContainsKey(this))
            {
                bool ableToHit = AttackEvent.AbleToHit(this, player);

                Special_AbleToHit(player);

                if (special_ableToHit && actionpoints_used >= actionpoints_cooldown)
                {
                    Special_Attack(player, mod);

                    actionpoints_used = 0;
                    actionPoints = 0;
                    changedObjects.Add(player);
                }
                else if (ableToHit)
                {
                    Attack(player);

                    int cost = BaseActionCost;
                    if ((EquipmentSlots.Find("bodySlot") as EquipmentSlot).SlotItem != null)
                    {
                        cost = (int)(cost * ((EquipmentSlots.Find("bodySlot") as EquipmentSlot).SlotItem as BodyEquipment).WalkCostMod);
                    }
                    actionPoints -= cost;
                    changedObjects.Add(player);
                }
                else
                {
                    PathFinder pf = new PathFinder(tf);
                    List<Tile> path = pf.ShortestPath(Tile, player.Tile);
                    // TODO?:(assuming there are tiles that cannot be walked over but can be fired over)
                    // check if there is a path to a spot that can hit the player (move closer water to fire over it)
                    if (path.Count > 0)
                    {
                        changedObjects.Add(this);
                        changedObjects.Add(Tile);
                        changedObjects.Add(path[0]);

                        MoveTo(path[0]);
                        actionPoints -= BaseActionCost;
                    }
                    else
                    {
                        Idle();
                    }
                }
            }
            else
            {
                Idle();
            }
        }


        /// <summary>
        /// Determines whether or not the selected player is within reach of the special.
        /// </summary>
        /// <param name="player">The player being targeted</param>
        private void Special_AbleToHit(Player player)
        {
            if (player.Tile.SeenBy.ContainsKey(this))
            {
                Point delta = player.Tile.TilePosition - this.Tile.TilePosition;
                double reach = this.Special_Reach + 0.5f;

                double distance = Math.Sqrt(Math.Pow(delta.X, 2) + Math.Pow(delta.Y, 2));

                bool result = distance <= reach;
                special_ableToHit = result;
            }
            else
            {
                special_ableToHit = false;
            }
        }

        private void Idle()
        {
            //TODO: implement idle behaviour (right now for if there is no path to the player, later for if it can't see the player.)
            actionPoints=0;
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            if (Health > 0 && animations["die"] != Current)
            {
                Action onClick = () =>
                {
                    Player player = GameWorld.Find(Player.LocalPlayerName) as Player;
                    AttackEvent aE = new AttackEvent(player, this);
                    Server.Send(aE);
                };

                inputHelper.IfMouseLeftButtonPressedOn(this, onClick);

                base.HandleInput(inputHelper);
            }
        }

        private void PositionHPBar()
        {
            hpBar.Position = Tile.GlobalPosition - new Vector2(Math.Abs(Tile.Width - hpBar.Width) / 2, 0);
        }
    }
}
