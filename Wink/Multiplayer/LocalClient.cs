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
    class LocalClient : Client
    {
        public string clientName { get; set; }
        public Level level { get; set; }

        private Camera camera;
        private InputHelper ih;

        private const int cameraMoveSpeed = 4;

        public LocalClient(Server server) : base(server)
        {
            ih = new InputHelper();
            camera = new Camera();
            server.AddLocalClient(this);
            clientName = System.Environment.MachineName;
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

                TileField tf = (TileField)level.Find("TileField");
                int width = tf.Columns * tf.CellWidth;
                int height = tf.Rows * tf.CellHeight;

                if(new Rectangle((int)tf.GlobalPosition.X, (int)tf.GlobalPosition.Y, width, height).Contains(globalPos))
                {
                    Vector2 tileFielfOffSet = globalPos - tf.GlobalPosition;
                    int x = (int)(tileFielfOffSet.X / tf.CellWidth);
                    int y = (int)(tileFielfOffSet.Y / tf.CellHeight);
                    Tile clicked = (Tile)tf.Objects[x, y];
                    clicked.OnClicked();
                    //System.Diagnostics.Debug.WriteLine("clicked x="+x+" y="+y);
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
