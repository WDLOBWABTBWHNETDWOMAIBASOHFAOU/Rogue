﻿using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Wink
{
    public enum DamageType
    {
        Physical,
        Magic,
        NonDamage
    }

    public abstract class Equipment:Item
    {

        /// <summary>
        /// specific equipment
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="id"></param>
        /// <param name="layer"></param>
        /// <param name="stackSize"></param>
        /// <param name="strRequirement"></param>
        /// <param name="dexRequirement"></param>
        /// <param name="intRequirement"></param>
        public Equipment(string assetName, string id, int layer = 0, int stackSize = 1) : base(assetName, stackSize, layer, id)
        {
        }

        #region Serialization
        public Equipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        #endregion

        protected abstract bool MeetsRequirements(Living l);

        public override void ItemInfo(ItemSlot caller)
        {
            base.ItemInfo(caller);
        }

        #region BonusValues
        protected int LowBonusValue(int baseBonusValue)
        { return (GameEnvironment.Random.Next(baseBonusValue) - baseBonusValue / 2) / 10; }
        protected int MediumBonusValue(int baseBonusValue)
        { return (GameEnvironment.Random.Next(baseBonusValue)) / 10; }
        protected int HighBonusValue(int baseBonusValue)
        { return (GameEnvironment.Random.Next(baseBonusValue) + baseBonusValue / 2) / 10; }
        #endregion

        public override void ItemAction(Living caller)
        {   
            //TODO: send event to switch this equipment with the equipment currently in the appropriate slot.
        }

        public override void DoBonus(Living living)
        {
            //throw new NotImplementedException();
        }

        public override void RemoveBonus(Living living)
        {
           // throw new NotImplementedException();
        }
    }
}
