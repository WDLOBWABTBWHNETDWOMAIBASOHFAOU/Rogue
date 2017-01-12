using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    class Trap : SpriteGameObject, ITileObject
    {
        private bool triggered;
        private int trapStrength;

        public Point PointInTile
        {
            get { return new Point(0, 0); }
        }

        public bool BlocksTile
        {
            get { return true; }
        }

        public Trap(string assetName, int trapStrenght = 40, int layer = 0, string id = "") : base(assetName,layer,id)
        {
            trapStrength = trapStrenght;
            triggered = false;
        }

        #region Serialization
        public Trap(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            triggered = info.GetBoolean("triggered");
            trapStrength = info.GetInt32("trapStrength");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("triggered", triggered);
            info.AddValue("trapStrength", trapStrength);
            base.GetObjectData(info, context);
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            List<GameObject> players = GameWorld.FindAll(p => p is Player);
            foreach (Player p in players)
            {
                int dx = (int)Math.Abs(p.Position.X - p.Origin.X - Position.X);
                int dy = (int)Math.Abs(p.Position.Y - p.Origin.Y - Position.Y);

                if (dx <= 0 && dy <= 0 && !triggered)
                {
                    p.TakeDamage(trapStrength);
                    triggered = true;
                    Visible = false;
                }
            }
        }
    }
}
