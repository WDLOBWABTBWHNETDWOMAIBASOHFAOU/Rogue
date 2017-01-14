using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System;

namespace Wink
{
    public class LocalServer : Server
    {
        private List<Client> clients;
        private List<Living> livingObjects;
        private Level level;
        private int levelIndex;

        bool levelChanged;
        private int turnIndex;

        public Level Level
        {
            get { return level; }
            set { level = value; }
        }

        public int LevelIndex
        {
            get { return levelIndex; }
        }

        public LocalServer ()
        {
        }

        public void SetupLevel(int levelIndex, List<Client> clients)
        {
            //level = new Level(levelIndex);
            level = new Level();

            this.clients = clients;
            foreach (Client c in clients)
            {
                //Player adds itself to level.
                Player player = new Player(c.ClientName, Level.Layer);
                Level.Add(player);
                player.InitPosition();
            }

            livingObjects = Level.FindAll(obj => obj is Living).Cast<Living>().ToList();
            livingObjects.Sort((obj1, obj2) => obj1.Dexterity - obj2.Dexterity);
            livingObjects.ElementAt(turnIndex).isTurn = true;

            SendOutUpdatedLevel(true);
        }

        protected override void ReallySend(Event e)
        {
            if (e.Validate(Level))
            {
                e.OnServerReceive(this);
            }
        }

        private void SendOutUpdatedLevel(bool first = false)
        {
            foreach(Client c in clients)
            {
                //Passing null because whenever a client receives an event it'll always be from the server.
                LevelUpdatedEvent e = first ? new JoinedServerEvent(Level) : new LevelUpdatedEvent(Level);
                c.Send(e);
            }
        }

        public override void Update(GameTime gameTime)
        {
            Level.Root.Update(gameTime);
            UpdateTurn();
            if (levelChanged)
            {
                //    Player player = Level.Find(p => p is Player) as Player;
                //    TileField tf = Level.Find("TileField") as TileField;
                //    if (player != null && tf != null)
                //    {
                //        hitable(player, tf);
                //    }
                SendOutUpdatedLevel();
            levelChanged = false;
            }
        }

        // very laggy, but does show you what can be hit
        void hitable(Player player, TileField tf)
        {
            Enemy dummy = new Enemy(1);
            for(int x = 0; x < tf.Columns; x++)
            {
                for(int y = 0; y < tf.Rows; y++)
                {
                    Tile t = tf[x, y] as Tile;
                    dummy.Position = t.TilePosition.ToVector2() * tf.CellHeight + dummy.Origin;

                    int dx = (int)Math.Abs(player.Position.X - dummy.Position.X) - Tile.TileWidth / 2;
                    int dy = (int)Math.Abs(player.Position.Y - dummy.Position.Y) - Tile.TileHeight / 2;

                    double distance = Math.Abs(Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
                    double reach = Tile.TileWidth * player.Reach;

                    bool withinReach = distance <= reach;

                    if (withinReach && !AttackEvent.Blocked(player, dummy) && t.TileType == TileType.Normal)
                    {
                        t.spriteAssetName = "empty:65:65:10:DarkRed";
                        t.LoadSprite();
                    }
                    else
                    {
                        t.spriteAssetName = t.AssetName;
                        t.LoadSprite();
                    }
                }
            }
        }
        
        public void LevelChanged()
        {
            levelChanged = true;
        }

        private void UpdateTurn()
        {
            if (livingObjects.ElementAt(turnIndex).ActionPoints <= 0)
            {
                livingObjects.ElementAt(turnIndex).isTurn = false;
                turnIndex = (turnIndex +1)% livingObjects.Count;
                livingObjects.ElementAt(turnIndex).isTurn = true;
                livingObjects.ElementAt(turnIndex).ActionPoints = 4;
            }
        }
    }
}
