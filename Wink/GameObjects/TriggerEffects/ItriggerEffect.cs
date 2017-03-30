using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    public enum TriggerEffects
    {
        Reflection
    }

    
    public interface ItriggerEffect
    {
        TriggerEffects effect { get; }
        void ExecuteTrigger(Living partof, Living target = null, double value = 0);
    }
}
