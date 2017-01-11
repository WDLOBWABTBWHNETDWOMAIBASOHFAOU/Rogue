using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    public interface ICellGrid
    {
        int xDim { get; }
        int yDim { get; }
        bool IsWall(int x, int y);
        void SetLight(int x, int y, float distanceSquared);
    }
}
