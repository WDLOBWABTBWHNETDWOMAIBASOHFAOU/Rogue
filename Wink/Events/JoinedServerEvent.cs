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
            client.Camera.CenterOn(client.Player);
            GameEnvironment.GameStateManager.SwitchTo("playingState");
        }
    }
}
