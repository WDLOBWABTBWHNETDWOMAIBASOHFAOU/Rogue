using System;
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

    public class Equipment:Item
    {
        protected int strRequirement;
        protected int dexRequirement;
        protected int intRequirement;

        /// <summary>
        /// Generated equipment
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="stackSize"></param>
        public Equipment(int floorNumber,  int layer = 0, int stackSize = 1) : base(floorNumber,stackSize, layer)
        {
        }

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
        public Equipment(string assetName, string id, int layer = 0, int stackSize = 1, int strRequirement = 0, int dexRequirement = 0, int intRequirement = 0) : base(assetName, stackSize, layer, id)
        {
            this.strRequirement = strRequirement;
            this.dexRequirement = dexRequirement;
            this.intRequirement = intRequirement;
        }

        #region Serialization
        public Equipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            strRequirement = info.GetInt32("strRequirement");
            dexRequirement = info.GetInt32("dexRequirement");
            intRequirement = info.GetInt32("intRequirement");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("strRequirement", strRequirement);
            info.AddValue("dexRequirement", dexRequirement);
            info.AddValue("intRequirement", intRequirement);
        }
        #endregion

        protected bool MeetsRequirements(Living l)
        {
            if (l.Strength >= strRequirement && l.Dexterity >= dexRequirement && l.Intelligence >= intRequirement)
            {
                return true;
            }
            return false;
        }

        public override void ItemInfo(ItemSlot caller)
        {
            base.ItemInfo(caller);

            if(strRequirement != 0 || dexRequirement != 0 || intRequirement != 0)
            {
                TextGameObject emptyLine = new TextGameObject("Arial12", 0, 0, "emptyLine." + this);
                emptyLine.Text = " ";
                emptyLine.Color = Color.Red;
                infoList.Add(emptyLine);

                TextGameObject requirementsText = new TextGameObject("Arial12", 0, 0, "reqInfoText." + this);
                requirementsText.Text = "Requirements";
                requirementsText.Color = Color.Red;
                infoList.Add(requirementsText);

                TextGameObject requirements = new TextGameObject("Arial12", 0, 0, "reqInfo." + this);
                requirements.Text = "Str: " + strRequirement+ "   Dex: " + dexRequirement + "   Int: " + intRequirement ;
                requirements.Color = Color.Red;
                infoList.Add(requirements);
            }
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
    }
}
