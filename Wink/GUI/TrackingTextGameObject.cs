using System;

namespace Wink
{
    /// <summary>
    /// Subclass of regular TextGameObject that overrides the property used to retrieve the contents. 
    /// Instead of a prespecified variable it uses a Func to retrieve a value from a different variable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class TrackingTextGameObject<T> : TextGameObject
    {
        Func<T, string> textFunc;
        T target;

        public TrackingTextGameObject(T target, Func<T, string> textFunc, string fontName, float cameraSensitivity = 1, int layer = 0, string id = "") : base(fontName, cameraSensitivity, layer, id)
        {
            this.target = target;
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
