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
        
        public Enemy(int layer, string id = "Enemy", float FOVlength = 8.5f) : base(layer, id,FOVlength)
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

        protected override void DoBehaviour(List<GameObject> changedObjects)
        {
            GoTo(changedObjects, GameWorld.Find(Player.LocalPlayerName) as Player);
        }

        public void GoTo(List<GameObject> changedObjects, Player player)
        {
            TileField tf = GameWorld.Find("TileField") as TileField;
            
            int dx = (int)Math.Abs(player.Tile.Position.X - Tile.Position.X);
            int dy = (int)Math.Abs(player.Tile.Position.Y - Tile.Position.Y);

            double distance = Math.Abs(Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
            double reach = Tile.TileWidth * Reach;

            bool withinReach = distance <= reach;

            if (withinReach)
            {
                Attack(player);
                actionPoints--;
                changedObjects.Add(player);
            }
            else
            {
                PathFinder pf = new PathFinder(tf);
                List<Tile> path = pf.ShortestPath(Tile, player.Tile);
                if (path.Count > 0)
                {
                    changedObjects.Add(this);
                    changedObjects.Add(Tile);
                    changedObjects.Add(path[0]);

                    MoveTo(path[0]);
                    actionPoints--;
                }
                else
                {
                    Idle();
                }
            }
        }

        private void Idle()
        {
            //TODO: implement idle behaviour (right now for if there is no path to the player, later for if it can't see the player.)
            actionPoints--;
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