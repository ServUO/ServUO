using System;
using Server.Targeting;

namespace Server.Items
{
    public class RunedSwitch : Item
    {
        [Constructable]
        public RunedSwitch()
            : base(0x2F61)
        {
            this.Weight = 1.0;
        }

        public RunedSwitch(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072896;
            }
        }// runed switch
        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1075101); // Please select an item to recharge.
                from.Target = new InternalTarget(this);
            }
            else
                from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        private class InternalTarget : Target
        {
            private readonly RunedSwitch m_Item;
            public InternalTarget(RunedSwitch item)
                : base(0, false, TargetFlags.None)
            {
                this.m_Item = item;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (this.m_Item == null || this.m_Item.Deleted)
                    return;

                if (o is BaseTalisman)
                {
                    BaseTalisman talisman = (BaseTalisman)o;

                    if (talisman.Charges == 0)
                    {
                        talisman.Charges = talisman.MaxCharges;
                        this.m_Item.Delete();
                        from.SendLocalizedMessage(1075100); // The item has been recharged.
                    }
                    else
                        from.SendLocalizedMessage(1075099); // You cannot recharge that item until all of its current charges have been used.
                }
                else
                    from.SendLocalizedMessage(1046439); // That is not a valid target.
            }
        }
    }
}