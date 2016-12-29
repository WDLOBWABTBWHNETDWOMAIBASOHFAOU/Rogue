using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Wink
{
    public class LocalClient : Client, IGameLoopObject
    {
        private GameObjectList gameObjects;
        private Camera newCamera;

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

            newCamera = new Camera();
            GameEnvironment.InputHelper.Camera = newCamera;

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
            newCamera.HandleInput(inputHelper);
            gameObjects.HandleInput(inputHelper);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            gameObjects.Draw(gameTime, spriteBatch, newCamera);
        }

        public override void Send(Event e)
        {
            if (e.Validate(Level))
            {
                e.OnClientReceive(this);
            }
        }

        public void DrawDebug(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            gameObjects.DrawDebug(gameTime, spriteBatch, newCamera);
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
