using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    public class Window : GameObjectList
    {
        private Vector2 mouseOffset;
        private bool dragging;

        private bool hasCloseButton;
        private bool isDraggable;
        
        private int width, height;
        private Button closeButton;
        public int Width { get { return width; } }
        public int Height { get { return height; } }

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
            height = height++;
            width = width++;
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
                closeButton.Action = () => {
                    visible = false;
                };
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
                base.HandleInput(inputHelper);

            if (isDraggable)
            {
                Action onClick = () =>
                {
                    mouseOffset = inputHelper.MousePosition - Position;
                    dragging = true;
                };
                inputHelper.IfMouseLeftButtonPressedOn(this, onClick, TitleBar);

                if (inputHelper.MouseLeftButtonDown() && dragging)
                    Position = inputHelper.MousePosition - mouseOffset;
                else
                    dragging = false;
            }
        }
    }
}
