
using System;
using System.Runtime.Serialization;

namespace Wink
{
    class ArrowAnimationEvent : AnimationEvent
    {
        private SpriteGameObject arrow;

        public ArrowAnimationEvent() : base("")
        {

        }

        #region Serialization
        public ArrowAnimationEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        #endregion

        protected override int Length
        {
            get { return 60; }
        }

        public override void Animate()
        {
            //move arrow
        }

        public override void PostAnimate()
        {
            //remove arrow
        }

        public override void PreAnimate(LocalClient client)
        {
            //Create arrow
        }
    }
}
