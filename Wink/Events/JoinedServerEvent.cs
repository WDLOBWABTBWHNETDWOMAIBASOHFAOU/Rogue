using System;

namespace Wink
{
    [Serializable]
    class JoinedServerEvent : LevelUpdatedEvent
    {
        public JoinedServerEvent(Level level) : base(level)
        {
        }

        public override void OnClientReceive(LocalClient client)
        {
            base.OnClientReceive(client);
            client.LoadPlayerGUI();
            GameEnvironment.GameStateManager.SwitchTo("playingState");
        }
    }
}
