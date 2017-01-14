using System.Collections.Generic;

namespace Wink
{
    interface IGUIGameObject
    {
        void InitGUI(Dictionary<string, object> guiState);
        void CleanupGUI(Dictionary<string, object> guiState);
    }
}
