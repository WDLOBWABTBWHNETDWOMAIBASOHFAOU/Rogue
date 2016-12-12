using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    public class LocalClient : Client
    {
        private GameObjectList gameObjects;
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

        private Camera camera;

        public LocalClient(Server server) : base(server)
        {
            ClientName = System.Environment.MachineName;
            
            camera = new Camera();

            gameObjects = new GameObjectList();
            gameObjects.Add(new PlayingGUI());

            server.AddLocalClient(this);
        }

        public void Update(GameTime gameTime)
        {
        }

        public void HandleInput(InputHelper inputHelper) 
        {
            if (inputHelper.MouseLeftButtonPressed() && Level != null)
            {
                Vector2 mousePos = inputHelper.MousePosition;
                Vector2 globalPos = mousePos + camera.GlobalPosition;
                FindClicked(gameObjects.Children, globalPos);
            }

            camera.HandleInput(inputHelper);
            gameObjects.HandleInput(inputHelper);
        }

        private void FindClicked(List<GameObject> gameObjects, Vector2 mousePos)
        {
            //Right way round?
            //Sort gameobjects by layer
            gameObjects.Sort((obj1, obj2) => obj1.Layer - obj2.Layer);

            for (int i = 0; i < gameObjects.Count; i++)
            {
                //First check whether current gameObject was clicked
                if (gameObjects[i].BoundingBox.Contains(mousePos))
                {
                    //Then check whether the gameObject is clickable
                    if (gameObjects[i] is ClickableGameObject)
                    {
                        //If so execute OnClick and return
                        (gameObjects[i] as ClickableGameObject).OnClick(server);
                        return;
                    }
                    else if (gameObjects[i] is GameObjectList)
                    {
                        //If the object is not clickable, but is a list, search the list
                        FindClicked((gameObjects[i] as GameObjectList).Children, mousePos);
                    }
                    else if (gameObjects[i] is GameObjectGrid)
                    {
                        //If the object is a grid, find the gameObject that was clicked and execute OnClick.
                        GameObject gObj = (gameObjects[i] as GameObjectGrid).Find(obj => obj.BoundingBox.Contains(mousePos));
                        if(gObj is ClickableGameObject)
                        {
                            (gObj as ClickableGameObject).OnClick(server);
                            return;
                        }
                    }
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera cam)
        {
            gameObjects.Draw(gameTime, spriteBatch, camera);
        }

        public override void Send(Event e)
        {
            e.OnClientReceive(this);
        }
    }
}
