using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    class TabField : GameObjectList
    {
        public class Tab
        {
            public Tab(string title, GameObjectList content)
            {
                this.title = title;
                this.content = content;
            }

            public string title { get; private set; }
            public GameObjectList content { get; private set; }
        }

        private List<Tab> tabs;
        private int width;
        private int height;

        private Color titleColor;
        private SpriteFont titleFont;

        public TabField(List<Tab> tabs, Color titleColor, SpriteFont titleFont, int width, int height)
        {
            GameObjectList tabTitles = new GameObjectList(0, "TabTitles");
            Add(tabTitles);

            GameObjectList content = new GameObjectList(0, "Content");
            Add(content);

            this.titleColor = titleColor;
            this.titleFont = titleFont;
            this.width = width;
            this.height = height;
            this.tabs = tabs;
            foreach (Tab t in tabs)
            {
                AddTab(t);
            }

            ActivateTab(0);
        }

        private void ActivateTab(int index)
        {
            GameObjectList content = Find("Content") as GameObjectList;
            Remove(content);

            GameObjectList newContent = new GameObjectList(0, "Content");
            newContent.Add(tabs[index].content);
            Add(newContent);
        }

        private void AddTab(Tab tab)
        {
            int tabTitleWidth = width / tabs.Count - 4;
            Button b = new Button("empty:" + tabTitleWidth + ":50:" + width + ":LightGray", tab.title, titleFont, titleColor);
            int index = tabs.FindIndex(t => t.Equals(tab));
            b.Position = new Vector2(width / tabs.Count * index + 2, 2);

            (Find("TabTitles") as GameObjectList).Add(b);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            Texture2D white = GameEnvironment.AssetManager.GetSingleColorPixel(Color.White);
            Point screen = GameEnvironment.Screen;
            spriteBatch.Draw(white, null, new Rectangle(0,54,width,height-54));

            base.Draw(gameTime, spriteBatch, camera);
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            GameObjectList titleButtons = Find("TabTitles") as GameObjectList;

            for (int i = 0; i < titleButtons.Children.Count; i++)
            {
                Button b = titleButtons.Children[i] as Button;
                if (b.Pressed)
                {
                    ActivateTab(i);
                }
            }
        }
    }
}
