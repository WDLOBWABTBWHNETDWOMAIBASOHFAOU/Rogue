using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    class GameOverState : IGameLoopObject
    {
        // ToDo:
        //GameOver overlay
        //MainMenu button
        //restart button
        //show score / acievements / unlocked content


        public void Update(GameTime gameTime)
        {
            // for now go direcly back to main menu
            GameEnvironment.GameStateManager.SwitchTo("mainMenuState");
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
        }

        public void HandleInput(InputHelper inputHelper)
        {
        }

        public void Reset()
        {            
        }
    }
}
