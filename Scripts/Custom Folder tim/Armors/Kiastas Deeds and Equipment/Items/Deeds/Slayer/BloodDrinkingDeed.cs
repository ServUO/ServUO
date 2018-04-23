using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Kiasta.Deeds
{
    public class BloodDrinkingTarget : BaseDeedTarget
    {
        BloodDrinkingDeed m_Deed;

        public BloodDrinkingTarget(BloodDrinkingDeed deed)
        {
            m_Deed = deed;
            AttributeName = Settings.SlayerNameName.BloodDrinking;
            Modifier = Settings.SlayerNameModifier.BloodDrinking;
            Attribute = new object[] { SlayerName.BloodDrinking };
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (m_Deed.Deleted || m_Deed.RootParent != from)
            {
                from.SendMessage("You cannot apply {0} to that.", AttributeName);
                return;
            }
            else
            {
                ModifyItem modify = new ModifyItem(from, target, Attribute, AttributeName, Max, Modifier);
                if (modify.IsApplied)
                {
                    m_Deed.Delete();
                }
            }
        }
    }

    public class BloodDrinkingDeed : BaseDeed
    {
        [Constructable]
        public BloodDrinkingDeed()
            : base(Settings.Misc.DeedItemID)
        {
            AttributeName = Settings.SlayerNameName.BloodDrinking;
            Name = "a " + AttributeName + " deed";
        }

        public BloodDrinkingDeed(Serial serial)
            : base(serial)
        {
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("The item needs to be in your pack");
            }
            else
            {
                from.SendMessage("Which item would you like to add {0} to?", Settings.SlayerNameName.BloodDrinking);
                from.Target = new BloodDrinkingTarget(this);
            }
        }
    }
}
