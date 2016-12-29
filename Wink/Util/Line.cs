using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Wink
{
    /// <summary>
    /// The code in this class is based on:
    /// http://gamedev.stackexchange.com/a/44016
    /// 
    /// As a stackexchange contribution it is licensed using The MIT License.
    /// </summary>
    class Line
    {
        public static void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color, int thickness = 1)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            
            sb.Draw(
                GameEnvironment.AssetManager.GetSingleColorPixel(Color.White),
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    thickness), //width of line, change this to make thicker line
                null,
                Color.Red, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0
            );
        }
    }
}
