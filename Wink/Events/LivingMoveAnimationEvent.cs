using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Wink
{
    [Serializable]
    class LivingMoveAnimationEvent : AnimationEvent
    {
        protected override int Length
        {
            get { return 30; }
        }

        private Living toMove;
        private Tile origin, destination;

        public LivingMoveAnimationEvent(Living toMove, Tile destination)
        {
            this.toMove = toMove;
            this.destination = destination;
        }

        #region Serialization
        public LivingMoveAnimationEvent(SerializationInfo info, StreamingContext context)
        {
            toMove = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("toMoveGUID"))) as Living;
            destination = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("destinationGUID"))) as Tile;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("toMoveGUID", toMove.GUID.ToString());
            info.AddValue("destinationGUID", destination.GUID.ToString());
        }
        #endregion

        private Vector2 Diff
        {
            get { return origin.GlobalPosition - destination.GlobalPosition; }
        }
        private Point Delta
        {
            get { return new Point(1, 1) + destination.TilePosition - origin.TilePosition; }
        }
        private bool Before //Whether or not the destination is drawn before the origin.
        {
            get { return Delta.X * 3 + Delta.Y < 4; }
        }

        public override void PreAnimate(LocalClient client)
        {
            origin = toMove.Tile;
            if (!Before)
            { //The destination is drawn after the origin so we move the Living object to the tile here, before the animation.
                origin.RemoveImmediatly(toMove);
                destination.PutOnTile(toMove);
                toMove.Position += origin.Position - destination.Position;
            }
        }

        public override void Animate()
        {
            toMove.Position -= Diff / Length;
        }
        
        public override void PostAnimate()
        {
            int z = Delta.X * 3 + Delta.Y;
            if (Before)
            { //The destination is drawn before the origin so we move the Living object to the tile here, after the animation.
                origin.RemoveImmediatly(toMove);
                destination.PutOnTile(toMove);
            }
        }
    }
}
