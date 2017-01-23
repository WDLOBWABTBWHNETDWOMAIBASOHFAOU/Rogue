using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Wink
{
    class SkillBar : GameObjectList
    {
        GameObjectList skills;

        public SkillBar(GameObjectList skills) : base()
        {
            this.skills = skills;
            for (int i = 0; i < skills.Children.Count; i++)
            {
                skills.Children[i].Position = new Vector2(i * Tile.TileWidth, 0);
            }
            Add(skills);
        }

        public int Width {get { return skills.Children.Count* Tile.TileWidth; } }
    }
}