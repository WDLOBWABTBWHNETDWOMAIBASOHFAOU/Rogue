using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wink
{
    public abstract class AnimationEvent : Event
    {
        protected abstract int Length { get; }
        protected int counter;

        public AnimationEvent()
        { }

        #region Serialization
        public AnimationEvent(SerializationInfo info, StreamingContext context) : base (info, context)
        {

        }
        #endregion

        public override List<Guid> GetFullySerialized(Level level)
        {
            return new List<Guid>();
        }

        public override bool OnClientReceive(LocalClient client)
        {
            if (counter == 0)
                PreAnimate(client);

            Animate();
            counter++;

            if (counter == Length)
            {
                PostAnimate();
                return true;
            }
            return false;
        }

        public override bool OnServerReceive(LocalServer server)
        {
            throw new NotImplementedException();
        }

        public abstract void PreAnimate(LocalClient client);
        public abstract void Animate();
        public abstract void PostAnimate();

        public override bool Validate(Level level)
        {
            return true;
        }
    }
}
