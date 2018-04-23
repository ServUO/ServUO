using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Kiasta.Deeds
{
    public class BonusDexTarget : BaseDeedTarget
    {
        BonusDexDeed m_Deed;

        public BonusDexTarget(BonusDexDeed deed)
        {
            m_Deed = deed;
            AttributeName = Settings.AttributeName.BonusDex;
            Max = Settings.AttributeMaxValue.BonusDex;
            Modifier = Settings.AttributeModifierValue.BonusDex;
            Attribute = new object[] { AosAttribute.BonusDex };
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

    public class BonusDexDeed : BaseDeed
    {
        [Constructable]
        public BonusDexDeed() : base(Settings.Misc.DeedItemID)
        {
            AttributeName = Settings.AttributeName.BonusDex;
            Name = "a " + AttributeName + " deed";
        }

        public BonusDexDeed(Serial serial)
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
                from.SendMessage("Which item would you like to add {0} to?", Settings.AttributeName.BonusDex);
                from.Target = new BonusDexTarget(this);
            }
        }
    }
}