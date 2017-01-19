using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public class Enemy : Living, IGUIGameObject
    {
        private Bar<Enemy> hpBar;
        
        public override bool BlocksTile
        {
            get { return Health > 0; }
        }
        
        public Enemy(int layer, string id = "Enemy", float FOVlength = 8.5f) : base(layer, id, FOVlength)
        {
        }

        #region Serialization
        public Enemy(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        #endregion

        protected override void InitAnimation(string idleColor = "empty:64:64:10:Magenta")
        {
            base.InitAnimation("empty:64:64:10:Purple");
            PlayAnimation("idle");
        }

        protected override void Death()
        {
            // call recive exp for every player
            base.Death();
        }

        protected override void DoBehaviour(List<GameObject> changedObjects)
        {
            GoTo(changedObjects, GameWorld.Find(Player.LocalPlayerName) as Player);
        }

        public void GoTo(List<GameObject> changedObjects, Player player)
        {
            TileField tf = GameWorld.Find("TileField") as TileField;

            if (player.Tile.GetSeenBy.ContainsKey(this))
            {
                bool ableToHit = AttackEvent.AbleToHit(this, player);
                if (ableToHit)
                {
                    Attack(player);

                    int cost = BaseActionCost;
                    if((EquipmentSlots.Find("bodySlot") as EquipmentSlot).SlotItem != null)
                    {
                        cost =(int)(cost * ((EquipmentSlots.Find("bodySlot") as EquipmentSlot).SlotItem as BodyEquipment).WalkCostMod);
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

        private void Idle()
        {
            //TODO: implement idle behaviour (seeing the player part done)
            actionPoints=0;//if this is reached the enemy has no other options than to skip its turn (reduses number of GoTo loops executed) compared to actionpoints--;
        }
        
        public override void HandleInput(InputHelper inputHelper)
        {
            if (Health > 0)
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

        public void InitGUI(Dictionary<string, object> guiState)
        {
            if (GameWorld.Find("HealthBar" + guid.ToString()) == null)
            {
                SpriteFont textfieldFont = GameEnvironment.AssetManager.GetFont("Arial26");
                hpBar = new Bar<Enemy>(this, e => e.Health, e => e.MaxHealth, textfieldFont, Color.Red, 2, "HealthBar" + guid.ToString(), 1.0f, 1f, false);
                (GameWorld.Find("PlayingGui") as PlayingGUI).Add(hpBar);
            }
            else
            {
                hpBar = GameWorld.Find("HealthBar" + guid.ToString()) as Bar<Enemy>;
                hpBar.SetValueObject(this);
            }
            hpBar.Visible = !Tile.Visible ? false : Visible;
            PositionHPBar();
        }

        public void CleanupGUI(Dictionary<string, object> guiState)
        {

        }
    }
}