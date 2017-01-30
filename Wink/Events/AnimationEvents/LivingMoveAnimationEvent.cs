using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using System;

namespace Wink
{
    [Serializable]
    class LivingMoveAnimationEvent : AnimationEvent
    {
        protected override int Length
        {
            get
            {
                //return 30;
                //for now as sound effect is longer than 30 frames
                int delay = 6;//ensure there is a delay between sounds
                float miliseconds = GameEnvironment.AssetManager.Duration(assetName).Milliseconds;
                float frames = ((miliseconds / 1000) * 60) + delay;
                return (int)frames;
            }
        }

        private Living toMove;
        private Tile origin, destination;

        public LivingMoveAnimationEvent(Living toMove, Tile destination, string soundAssetName, bool playerSpecific = false, string LocalPlayerName = "") : base(soundAssetName, playerSpecific, LocalPlayerName)
        {
            this.toMove = toMove;
            this.destination = destination;
        }

        #region Serialization
        public LivingMoveAnimationEvent(SerializationInfo info, StreamingContext context):base(info,context)
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
            origin.RemoveImmediatly(toMove);

            if (!playerSpecific)
            {
                GameEnvironment.AssetManager.PlaySound(assetName);
            }
            else if (Player.LocalPlayerName == LocalPlayerName)
            {
                GameEnvironment.AssetManager.PlaySound(assetName);
            }
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
