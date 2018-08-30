using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Items
{
    public class VelocityTargetx : Target
    {
        private VelocityDeed m_Deed;

        public VelocityTargetx(VelocityDeed deed)
            : base(1, false, TargetFlags.None)
        {
            m_Deed = deed;
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (m_Deed.Deleted || m_Deed.RootParent != from)
            {
                from.SendMessage("You cannot add velocity to that.");
                return;
            }
            if (target is BaseRanged)
            {
                BaseRanged item = (BaseRanged)target;
                if (item is BaseRanged)
                {              
					((BaseRanged)item).Velocity += 5;
					from.SendMessage("Velocity successfully added to item.");
					m_Deed.Delete();
                }                  
            }
            else
            {
                from.SendMessage("You cannot put velocity on that.");
            }
        }
    }

    public class VelocityDeed : Item
    {
        [Constructable]
        public VelocityDeed()
            : base(0x14F0)
        {
            LootType = LootType.Blessed;
            Name = "a velocity (+5)";
            Hue = 1150;
            Weight = 1.0;
        }

        public VelocityDeed(Serial serial)
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
                from.SendMessage("Which item would you like to add velocity to?");
                from.Target = new VelocityTargetx(this);
            }
        }
    }
}