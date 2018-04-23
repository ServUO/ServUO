using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Kiasta.Deeds
{
    public class ResistPhysicalBonusTarget : BaseDeedTarget
    {
        ResistPhysicalBonusDeed m_Deed;

        public ResistPhysicalBonusTarget(ResistPhysicalBonusDeed deed)
        {
            m_Deed = deed;
            AttributeName = Settings.AttributeName.ResistPhysicalBonus;
            Max = Settings.AttributeMaxValue.ResistPhysicalBonus;
            Modifier = Settings.AttributeModifierValue.ResistPhysicalBonus;
            Attribute = new object[] { AosWeaponAttribute.ResistPhysicalBonus };
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

    public class ResistPhysicalBonusDeed : BaseDeed
    {
        [Constructable]
        public ResistPhysicalBonusDeed()
            : base(Settings.Misc.DeedItemID)
        {
            AttributeName = Settings.AttributeName.ResistPhysicalBonus;
            Name = "a " + AttributeName + " deed";
        }

        public ResistPhysicalBonusDeed(Serial serial)
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
                from.SendMessage("Which item would you like to add {0} to?", Settings.AttributeName.ResistPhysicalBonus);
                from.Target = new ResistPhysicalBonusTarget(this);
            }
        }
    }
}