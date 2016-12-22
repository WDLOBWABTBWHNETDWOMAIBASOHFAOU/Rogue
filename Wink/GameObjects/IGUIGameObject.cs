using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    interface IGUIGameObject
    {
        /// <summary>
        /// write check to only excute 1 time
        /// </summary>
        void InitGUI();
    }
}
