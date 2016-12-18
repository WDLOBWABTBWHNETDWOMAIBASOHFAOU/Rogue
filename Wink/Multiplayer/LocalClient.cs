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
            if (inputHelper.MouseLeftButtonPressed() && Level != null)
            {
                Vector2 globalPos = inputHelper.MousePosition + camera.GlobalPosition;
                FindClicked(Level.Children, globalPos);
            }

            camera.HandleInput(inputHelper);
            gameObjects.HandleInput(inputHelper);
        }

        private void FindClicked(List<GameObject> gameObjects, Vector2 mousePos)
        {
            //Search through gameObjects from back to front, so highest layer objects go first.
            for (int i = gameObjects.Count-1; i >= 0; i--)
            {
                //First check whether current gameObject was clicked
                if (gameObjects[i].BoundingBox.Contains(mousePos))
                {
                    //Then check whether the gameObject is clickable
                    if (gameObjects[i] is ClickableGameObject)
                    {
                        //If so execute OnClick and return
                        (gameObjects[i] as ClickableGameObject).OnClick(server, this);
                        return;
                    }
                    else if (gameObjects[i] is GameObjectList)
                    {
                        //If the object is not clickable, but is a list, search the list
                        FindClicked((gameObjects[i] as GameObjectList).Children, mousePos);
                        return;
                    }
                    else if (gameObjects[i] is GameObjectGrid)
                    {
                        //If the object is a grid, find the gameObject that was clicked and call this method again 
                        //(to make sure it is not a list or grid too)
                        GameObjectGrid gObjGrid = gameObjects[i] as GameObjectGrid;
                        GameObject gObj = gObjGrid.Find(obj => obj.BoundingBox.Contains(mousePos));
                        FindClicked(new List<GameObject>() { gObj }, mousePos);
                        return;
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
            if (e.Validate(Level))
            {
                e.OnClientReceive(this);
            }
        }
    }
}
