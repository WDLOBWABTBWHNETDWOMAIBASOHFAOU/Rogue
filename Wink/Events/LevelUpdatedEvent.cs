using System;

namespace Wink
{
    [Serializable]
    public class LevelUpdatedEvent : Event
    {
        private Level updatedLevel;

        public LevelUpdatedEvent(Level level) : base()
        {
            updatedLevel = level;
        }

        public override void OnClientReceive(LocalClient client)
        {
            if (client.Level != null)
                foreach (GameObject obj in client.Level.FindAll(obj => obj is IGUIGameObject))
                    (obj as IGUIGameObject).CleanupGUI();
            
            client.Level = updatedLevel;

            foreach (GameObject obj in updatedLevel.FindAll(obj => obj is IGUIGameObject)) 
                (obj as IGUIGameObject).InitGUI();
        }

        public override void OnServerReceive(LocalServer server)
        {
            throw new NotImplementedException();
        }

        public override bool Validate(Level level)
        {
            return true;
        }
    }
}
