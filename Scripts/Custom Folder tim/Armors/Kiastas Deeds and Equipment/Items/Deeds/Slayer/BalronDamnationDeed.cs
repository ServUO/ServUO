using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Kiasta.Deeds
{
    public class BalronDamnationTarget : BaseDeedTarget
    {
        BalronDamnationDeed m_Deed;

        public BalronDamnationTarget(BalronDamnationDeed deed)
        {
            m_Deed = deed;
            AttributeName = Settings.SlayerNameName.BalronDamnation;
            Modifier = Settings.SlayerNameModifier.BalronDamnation;
            Attribute = new object[] { SlayerName.BalronDamnation };
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

    public class BalronDamnationDeed : BaseDeed
    {
        [Constructable]
        public BalronDamnationDeed()
            : base(Settings.Misc.DeedItemID)
        {
            AttributeName = Settings.SlayerNameName.BalronDamnation;
            Name = "a " + AttributeName + " deed";
        }

        public BalronDamnationDeed(Serial serial)
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
                from.SendMessage("Which item would you like to add {0} to?", Settings.SlayerNameName.BalronDamnation);
                from.Target = new BalronDamnationTarget(this);
            }
        }
    }
}
