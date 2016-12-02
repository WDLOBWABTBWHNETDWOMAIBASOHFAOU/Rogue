using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wink;

class Program
{
    static PlayingState playingState;
    static DateTime startTime;
    static DateTime lastTime;

    static void Main(string[] args)
    {
        startTime = DateTime.UtcNow;
        //Rebuild system that updates 60 times a second (or less?) and gives a gametime.
        playingState = new PlayingState();

        Timer timer = new Timer(tick, null, 0, (int)(1000f/60f));
        GameTime gameTime = new GameTime();
    }

    static void tick(object state)
    {
        if (lastTime == null)
            lastTime = DateTime.UtcNow;

        TimeSpan total = startTime - DateTime.UtcNow;
        TimeSpan last = lastTime - DateTime.UtcNow;

        playingState.Update(new GameTime(total, last));
    }
}
