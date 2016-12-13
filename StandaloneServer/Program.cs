extern alias monogameframework;

using monogameframework::Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Wink;

class Program
{
    static PlayingState playingState;
    static DateTime startTime;
    static DateTime lastTime;

    static void Main(string[] args)
    {
        FieldInfo assetManagerField = typeof(GameEnvironment).GetField("assetManager", BindingFlags.Static | BindingFlags.NonPublic);
        assetManagerField.SetValue(null, new EmptyAssetManager());

        LocalServer server = new LocalServer();
        RemoteClient rc = new RemoteClient(server);

        //JoinServerEvent jse = new JoinServerEvent();
        //jse.clientName = "test";
        LevelUpdatedEvent lue = new LevelUpdatedEvent();
        lue.updatedLevel = server.level;

        rc.Send(lue);


        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream("MyTestFile.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
        Event obj = (Event)formatter.Deserialize(stream);
        stream.Close();

        

        startTime = DateTime.UtcNow;

        playingState = new PlayingState();

        Timer timer = new Timer(tick, null, 0, (int)(1000f/60f));
        Console.Read();

    }

    static void tick(object state)
    {
        if (lastTime.Equals(new DateTime()))
            lastTime = DateTime.UtcNow;

        TimeSpan total = startTime - DateTime.UtcNow;
        TimeSpan last = lastTime - DateTime.UtcNow;

        playingState.Update(new GameTime(total, last));
    }
}
