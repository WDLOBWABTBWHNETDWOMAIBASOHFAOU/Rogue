using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Wink
{
    public class LocalClient : Client, IGameLoopObject, ILocal
    {
        private FrameCounter frameCounter;

        private GameObjectList gameObjects;
        private Camera newCamera;

        private List<Event> pendingEvents;

        public Level Level
        {
            get { return gameObjects.Find("Level") as Level; }
            set
            {
                Level lvl = Level;
                if (lvl != null)
                    gameObjects.Children.Remove(lvl); //Needs to remove from children directly because nothing gets updated on clientside.

                gameObjects.Add(value);
            }
        }
        public PlayingGUI GUI
        {
            get { return gameObjects.Find("PlayingGui") as PlayingGUI; }
        }
        public override Player Player
        {
            get { return gameObjects.Find(Player.LocalPlayerName) as Player; }
        }

        public bool LevelBeingUpdated
        {
            get { return pendingEvents.Where(obj => obj is LevelUpdatedEvent).Count() > 0; }
        }

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
            if (guid == Guid.Empty || Level == null)
                return null;

            GameObject obj = Level.Find(o => o.GUID == guid);
            return obj;
        }

        public LocalClient(Server server) : base(server)
        {
            frameCounter = new FrameCounter();

            ClientName =  GameEnvironment.GameSettingsManager.GetValue("user_name");

            newCamera = new Camera();
            GameEnvironment.InputHelper.Camera = newCamera;

            gameObjects = new GameObjectList();
            gameObjects.Add(new PlayingGUI());

            pendingEvents = new List<Event>();
        }

        public void Replace(GameObject go)
        {
            gameObjects.Replace(go);
        }

        public override void Update(GameTime gameTime)
        {
            GUI.Update(gameTime);
            
            if (LevelBeingUpdated)
            {
                LevelUpdatedEvent e = pendingEvents.Where(obj => obj is LevelUpdatedEvent).First() as LevelUpdatedEvent;
                int luIndex = pendingEvents.IndexOf(e);
                for (int i = 0; i < luIndex; i++)
                {
                    pendingEvents.RemoveAt(0);
                }
            }

            if (pendingEvents.Count > 0)
                ExecuteIfValid(pendingEvents[0]);
        }

        public void HandleInput(InputHelper inputHelper) 
        {
            newCamera.HandleInput(inputHelper);
            gameObjects.HandleInput(inputHelper);
            
            if (inputHelper.KeyPressed(Keys.Space))
                Server.Send(new EndTurnEvent(Player));
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

        public void IncomingEvent(Event e)
        {
            pendingEvents.Add(e);
        }

        public override void SendPreSerialized(MemoryStream ms)
        {
            ms.Seek(0, SeekOrigin.Begin);
            if (pendingEvents.Where(obj => obj is LevelUpdatedEvent).Count() == 0)
            {
                Event e = SerializationHelper.Deserialize(ms, this) as Event;
                pendingEvents.Add(e);
            }
            else
            {
                throw new Exception("Event sent in same tick as LevelUpdatedEvent, this causes issues when deserializing because new level has not yet been put in place.");
            }
        }

        private void ExecuteIfValid(Event e)
        {
            if (e.Validate(Level))
                if (e.OnClientReceive(this))
                    pendingEvents.Remove(e);
        }

        public override void Reset()
        {
        }
    }
}
