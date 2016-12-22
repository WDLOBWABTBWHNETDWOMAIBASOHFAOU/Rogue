using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    public class LocalClient : Client
    {
        private GameObjectList gameObjects;
        private Camera camera;

        public Level Level
        {
            get
            {
                return gameObjects.Find("Level") as Level;
            }
            set
            {
                if(Level != null)
                    gameObjects.Remove(Level);

                gameObjects.Add(value);
            }
        }
        
        public Player Player
        {
            get { return gameObjects.Find("player_" + ClientName) as Player; }
        }

        public LocalClient(Server server) : base(server)
        {
            ClientName = System.Environment.MachineName;

            camera = new Camera();
            GameEnvironment.InputHelper.Camera = camera;

            gameObjects = new GameObjectList();
            gameObjects.Add(new PlayingGUI());
        }

        public void LoadPlayerGUI()
        {
            PlayingGUI pgui = gameObjects.Find(obj => obj is PlayingGUI) as PlayingGUI;
            pgui.AddPlayerGUI(Player);
        }

        public void Update(GameTime gameTime)
        {
        }

        public void HandleInput(InputHelper inputHelper) 
        {
            camera.HandleInput(inputHelper);
            gameObjects.HandleInput(inputHelper);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera c)
        {
            gameObjects.Draw(gameTime, spriteBatch, camera);
        }

        public override void Send(Event e)
        {
            if (e.Validate(Level))
            {
                e.OnClientReceive(this);
            }
        }
    }
}
