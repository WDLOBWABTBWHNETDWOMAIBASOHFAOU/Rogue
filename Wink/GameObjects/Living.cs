using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Wink
{
    public abstract class Living : AnimatedGameObject
    {
        public Living(int layer = 0, string id = "", float scale = 1.0f) : base(layer, id, scale)
        {
            
        }
        
        protected int manaPoints, healthPoints, actionPoints, armorValue, baseAttack=25, strenght=2, dexterity=2, intelligence=2, creatureLevel = 1;
        protected double hitChance = 0.7, dodgeChance = 0.3;

        void Attack()
        {
            double hitNumber = GameEnvironment.Random.NextDouble();
            if (hitNumber < hitChance)
            {
                double attackValue = calculateValue(baseAttack, strenght);
                // Call enemy's take damage method
                
            }
            // Display attack missed (feedback on fail)
        }

        void TakeDamage(double attackValue)
        {
            double dodgeNumber = GameEnvironment.Random.NextDouble();
            if (dodgeNumber > dodgeChance)
            {
                double defenceValue = calculateValue(armorValue);
                healthPoints = (int)(attackValue/defenceValue);
                //Display damage taken
            }
            // Display attack dodged (feedback on succes)
        }

        /// <summary>
        /// Calculetes a stat based value for living objects
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="stat"></param>
        /// <param name="extra">Sum of extra effects</param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        double calculateValue(int baseValue, int stat = 0, double modifier = 1, double extra = 0)
        {
            double value = baseValue + modifier * stat + extra;
            return value;
        }

        void Death()
        {

        }

        public void MoveTo(Tile tile)
        {
            if (position.X - (tile.TilePosition.X+1) * Tile.TileWidth <= Tile.TileWidth && position.X - tile.TilePosition.X * Tile.TileWidth >= -Tile.TileWidth)
            {
                if (position.Y - (tile.TilePosition.Y+1) * Tile.TileHeight <= Tile.TileHeight && position.Y - (tile.TilePosition.Y+1) * Tile.TileHeight >= -Tile.TileHeight)
                {
                    position.X = (tile.TilePosition.X + 1) * Tile.TileWidth - 0.5f * Tile.TileWidth;
                    position.Y = (tile.TilePosition.Y + 1) * Tile.TileHeight;
                }
            }            
        }
    }
}
