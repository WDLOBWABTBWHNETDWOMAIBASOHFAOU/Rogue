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
        
        public override void PreAnimate(LocalClient client)
        {
            origin = toMove.Tile;
            toMove.Position = toMove.GlobalPosition;
            client.Level.Add(toMove);
            if (toMove is Enemy)
            { //TODO: This shouldn't be necessary at all! Fix the problem where there is multiple tilefields...
                Tile trueOrigin = client.Level.Find(obj => obj != origin && obj is Tile && (obj as Tile).TilePosition == origin.TilePosition) as Tile;
                trueOrigin.OnTile.Children.Clear();
            }
            origin.Remove(toMove);
        }

        public override void Animate()
        {
            toMove.Position -= Diff / Length;
        }
        
        public override void PostAnimate()
        {
            (toMove.Parent as GameObjectList).RemoveImmediatly(toMove);
            destination.PutOnTile(toMove);
        }
    }
}
