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

            TextGameObject requirementsText = new TextGameObject("Arial12", 0, 0, "reqInfoText." + this);
            requirementsText.Text = "Requirements";
            requirementsText.Color = Color.Red;
            infoList.Add(requirementsText);

            TextGameObject requirements = new TextGameObject("Arial12", 0, 0, "reqInfo." + this);
            requirements.Text = "Str: " + strRequirement+ "   Dex: " + dexRequirement + "   Int: " + intRequirement ;
            requirements.Color = Color.Red;
            infoList.Add(requirements);
        }

        public override void ItemAction(Living caller)
        {
            //TODO: send event to switch this equipment with the equipment currently in the appropriate slot.
        }
    }
}
