using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Wink
{
    [Serializable]
    class Trap:SpriteGameObject
    {
        List<GameObject> players;
        bool triggerd;
        int trapStrenght;

        public Trap(string assetName, int trapStrenght = 40, int layer = 0, string id = "") : base(assetName,layer,id)
        {
            this.trapStrenght = trapStrenght;
            triggerd = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            players = GameWorld.FindAll(p => p is Player);

            foreach (Player p in players)
            {
                int dx = (int)Math.Abs(p.Position.X - p.Origin.X - Position.X);
                int dy = (int)Math.Abs(p.Position.Y - p.Origin.Y - Position.Y);

                if (dx <= 0 && dy <= 0 && !triggerd)
                {
                    p.TakeDamage(trapStrenght);
                    triggerd = true;
                    Visible = false;
                }
            }
        }
    }
}
