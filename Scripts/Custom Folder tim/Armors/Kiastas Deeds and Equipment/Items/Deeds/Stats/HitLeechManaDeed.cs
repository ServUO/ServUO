using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Kiasta.Deeds
{
    public class HitLeechManaTarget : BaseDeedTarget
    {
        HitLeechManaDeed m_Deed;

        public HitLeechManaTarget(HitLeechManaDeed deed)
        {
            m_Deed = deed;
            AttributeName = Settings.AttributeName.HitLeechMana;
            Max = Settings.AttributeMaxValue.HitLeechMana;
            Modifier = Settings.AttributeModifierValue.HitLeechMana;
            Attribute = new object[] { AosWeaponAttribute.HitLeechMana };
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

    public class HitLeechManaDeed : BaseDeed
    {
        [Constructable]
        public HitLeechManaDeed()
            : base(Settings.Misc.DeedItemID)
        {
            AttributeName = Settings.AttributeName.HitLeechMana;
            Name = "a " + AttributeName + " deed";
        }

        public HitLeechManaDeed(Serial serial)
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
                from.SendMessage("Which item would you like to add {0} to?", Settings.AttributeName.HitLeechMana);
                from.Target = new HitLeechManaTarget(this);
            }
        }
    }
}