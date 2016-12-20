using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    class Window : GameObjectList
    {
        private Vector2 mouseOffset;
        private bool dragging;

        private bool hasCloseButton;
        private bool isDraggable;
        
        private int width, height;
        private Button closeButton;

        const int BorderWidth = 2;
        const int TitleBarHeight = 20;

        public override Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(Position.ToPoint(), new Point(width, height));
            }
        }

        private Rectangle TitleBar
        {
            get
            {
                return new Rectangle(
                    (int)Position.X - BorderWidth, 
                    (int)Position.Y - 2 * BorderWidth - TitleBarHeight,
                    width + 2 * BorderWidth,
                    2 * BorderWidth + TitleBarHeight);
            }
        }

        public Window(int width, int height, bool hasCloseButton = true, bool isDraggable = true) : this(width, height, Color.DarkGray, hasCloseButton, isDraggable)
        {
        }

        public Window(int width, int height, Color fillColor, bool hasCloseButton = true, bool isDraggable = true)
        {
            this.hasCloseButton = hasCloseButton;
            this.isDraggable = isDraggable;
            this.width = width;
            this.height = height;
            if (hasCloseButton)
            {
                SpriteFont arial12Bold = GameEnvironment.AssetManager.GetFont("Arial12Bold");
                closeButton = new Button("empty:" + TitleBarHeight + ":" + TitleBarHeight + ":" + TitleBarHeight + ":Red", "X", arial12Bold, Color.Black);
                closeButton.Position = new Vector2(width - closeButton.Width - BorderWidth, -closeButton.Height - BorderWidth);
                Add(closeButton);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (GameObject c in Children)
            {
                c.Visible = visible;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            if (visible)
            {
                Texture2D blackTex = GameEnvironment.AssetManager.GetSingleColorPixel(Color.Black);
                Texture2D darkGrayTex = GameEnvironment.AssetManager.GetSingleColorPixel(Color.DarkGray);
                Rectangle outer = new Rectangle(
                    (int)Position.X - BorderWidth,
                    (int)Position.Y - TitleBarHeight - 2 * BorderWidth,
                    width + BorderWidth,
                    height + TitleBarHeight + 3 * BorderWidth
                );

                spriteBatch.Draw(blackTex, outer, Color.White);
                spriteBatch.Draw(darkGrayTex, BoundingBox, Color.White);

            }
            base.Draw(gameTime, spriteBatch, camera);
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            if (visible)
            {
                base.HandleInput(inputHelper);
            }
            if (closeButton != null && closeButton.Pressed)
            {
                visible = false;
            }

            if (isDraggable)
            {
                if (TitleBar.Contains(inputHelper.MousePosition))
                {
                    if (inputHelper.MouseLeftButtonPressed())
                    {
                        mouseOffset = inputHelper.MousePosition - Position;
                        dragging = true;
                    }
                }

                if (inputHelper.MouseLeftButtonDown() && dragging)
                {
                    Position = inputHelper.MousePosition - mouseOffset;
                }
                else
                {
                    dragging = false;
                }
            }
        }
    }
}
