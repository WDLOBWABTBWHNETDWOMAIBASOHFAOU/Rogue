namespace Wink
{
    interface IGUIGameObject
    {
        /// <summary>
        /// write check to only excute 1 time
        /// </summary>
        void InitGUI();
        void CleanupGUI();
    }
}
