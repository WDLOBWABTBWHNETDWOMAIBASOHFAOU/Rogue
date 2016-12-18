using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class TrackingTextGameObject<T> : TextGameObject
    {
        Func<T, string> textFunc;
        T target;

        public TrackingTextGameObject(T target, Func<T, string> textFunc, string fontName, float cameraSensitivity = 1, int layer = 0, string id = "") : base(fontName, cameraSensitivity, layer, id)
        {
            this.textFunc = textFunc;
        }

        public override string Text
        {
            get
            {
                return textFunc.Invoke(target);
            }
            set { }
        }
    }
}
