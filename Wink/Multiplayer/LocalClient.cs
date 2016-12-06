using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    public class LocalClient : Client
    {
        public Level level { get; set; }

        private Camera camera;
        private InputHelper ih;

        private const int cameraMoveSpeed = 4;

        public LocalClient(Server server) : base(server)
        {
            clientName = System.Environment.MachineName;
            ih = new InputHelper();
            camera = new Camera();
            server.AddLocalClient(this);
            
        }

        public void Update(GameTime gameTime)
        {
            ih.Update();

            if (ih.IsKeyDown(Keys.W))
                camera.Position += new Vector2(0, -cameraMoveSpeed);

            if (ih.IsKeyDown(Keys.A))
                camera.Position += new Vector2(-cameraMoveSpeed, 0);

            if (ih.IsKeyDown(Keys.S))
                camera.Position += new Vector2(0, cameraMoveSpeed);

            if (ih.IsKeyDown(Keys.D))
                camera.Position += new Vector2(cameraMoveSpeed, 0);

            
            if(ih.MouseLeftButtonPressed() && level != null)
            {
                Vector2 mousePos = ih.MousePosition;
                Vector2 globalPos = mousePos + camera.GlobalPosition;
                FindClicked(level.Children, globalPos);
            }
        }

        private void FindClicked(List<GameObject> gameObjects, Vector2 mousePos)
        {
            //Right way round?
            gameObjects.Sort((obj1, obj2) => obj1.Layer - obj2.Layer);

            for (int i = 0; i < gameObjects.Count; i++)
            {
                bool emptyBB = gameObjects[i].BoundingBox.Width * gameObjects[i].BoundingBox.Height == 0;
                if (gameObjects[i].BoundingBox.Contains(mousePos) || emptyBB)
                {
                    if (gameObjects[i] is ClickableGameObject)
                    {
                        (gameObjects[i] as ClickableGameObject).OnClick(server);
                        return;
                    }
                    else if (gameObjects[i] is GameObjectList)
                    {
                        FindClicked((gameObjects[i] as GameObjectList).Children, mousePos);
                    }
                    else if (gameObjects[i] is GameObjectGrid)
                    {
                        GameObject gObj = (gameObjects[i] as GameObjectGrid).Find(obj => obj.BoundingBox.Contains(mousePos));
                        if(gObj is ClickableGameObject)
                        {
                            (gObj as ClickableGameObject).OnClick(server);
                        }
                    }
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera cam)
        {
            if(level != null)
                level.Draw(gameTime, spriteBatch, camera);
        }

        public override void Send(Event e)
        {
            e.OnClientReceive(this);
        }
    }
}
