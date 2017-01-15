namespace Wink
{
    public interface ICellGrid
    {
        int xDim { get; }
        int yDim { get; }
        bool IsWall(int x, int y);
        void SetLight(int x, int y, float distanceSquared, IViewer seenBy);
    }
}
