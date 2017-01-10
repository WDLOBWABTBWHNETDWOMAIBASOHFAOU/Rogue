using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace Wink
{
    public class LocalClient : Client, IGameLoopObject, ILocal
    {
        private FrameCounter frameCounter;

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
                Level lvl = Level;
                if(lvl != null)
                    gameObjects.Children.Remove(lvl); //Needs to remove from children directly because nothing gets updated on clientside.

                gameObjects.Add(value);
            }
        }
        public PlayingGUI GUI {
            get { return gameObjects.Find("PlayingGui") as PlayingGUI; }
        }
        public override Player Player
        {
            get { return gameObjects.Find("player_" + ClientName) as Player; }
        }

        public bool IsMyTurn { set; get; }

        public Camera Camera
        {
            get { return newCamera; }
        }

        /// <summary>
        /// Method is used to deserialize based on GUID.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public GameObject GetGameObjectByGUID(Guid guid)
        {
            if (guid == Guid.Empty)
                return null;

            GameObject obj = Level.Find(o => o.GUID == guid);
            return obj;
        }

        public LocalClient(Server server) : base(server)
        {
            frameCounter = new FrameCounter();

            ClientName = System.Environment.MachineName;

            newCamera = new Camera();
            GameEnvironment.InputHelper.Camera = newCamera;

            gameObjects = new GameObjectList();
            gameObjects.Add(new PlayingGUI());
        }

        public void LoadPlayerGUI()
        {
            PlayingGUI pgui = gameObjects.Find(obj => obj is PlayingGUI) as PlayingGUI;
            pgui.AddPlayerGUI(this);
        }

        public void Replace(GameObject go)
        {
            gameObjects.Replace(go);
        }

        public void Update(GameTime gameTime)
        {
            GUI.Update(gameTime);
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

        public void DrawDebug(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            gameObjects.DrawDebug(gameTime, spriteBatch, newCamera);

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter.Update(deltaTime);

            float frameRate = (float)Math.Round(frameCounter.AverageFramesPerSecond * 10) / 10;
            string fps = string.Format("FPS: {0}", frameRate);
            SpriteFont sf = GameEnvironment.AssetManager.GetFont("Arial26");
            spriteBatch.DrawString(sf, fps, new Vector2(1, 100), Color.Red);
        }

        public override void Send(Event e)
        {
            e = SerializationHelper.Clone(e, this, e.GUIDSerialization);

            if (e.Validate(Level))
                e.OnClientReceive(this);
        }

        public override void SendPreSerialized(MemoryStream ms)
        {
            ms.Seek(0, SeekOrigin.Begin);
            Event e = SerializationHelper.Deserialize(ms, this, false) as Event;
            if (e.Validate(Level))
                e.OnClientReceive(this);
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
