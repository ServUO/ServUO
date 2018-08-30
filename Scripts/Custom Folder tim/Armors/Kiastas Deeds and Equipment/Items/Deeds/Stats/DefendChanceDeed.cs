using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Kiasta.Deeds
{
    public class DefendChanceTarget : BaseDeedTarget
    {
        DefendChanceDeed m_Deed;

        public DefendChanceTarget(DefendChanceDeed deed)
        {
            m_Deed = deed;
            AttributeName = Settings.AttributeName.DefendChance;
            Max = Settings.AttributeMaxValue.DefendChance;
            Modifier = Settings.AttributeModifierValue.DefendChance;
            Attribute = new object[] { AosAttribute.DefendChance };
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

    public class DefendChanceDeed : BaseDeed
    {
        [Constructable]
        public DefendChanceDeed()
            : base(Settings.Misc.DeedItemID)
        {
            AttributeName = Settings.AttributeName.DefendChance;
            Name = "a " + AttributeName + " deed";
        }

        public DefendChanceDeed(Serial serial)
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
                from.SendMessage("Which item would you like to add {0} to?", Settings.AttributeName.DefendChance);
                from.Target = new DefendChanceTarget(this);
            }
        }
    }
}