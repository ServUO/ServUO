using System;
using Server.Targeting;

namespace Server.Items
{
    public class Scales : Item
    {
        [Constructable]
        public Scales()
            : base(0x1852)
        {
            this.Weight = 4.0;
        }

        public Scales(Serial serial)
            : base(serial)
        {
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

        public override void OnDoubleClick(Mobile from)
        {
            from.SendLocalizedMessage(502431); // What would you like to weigh?
            from.Target = new InternalTarget(this);
        }

        private class InternalTarget : Target
        {
            private readonly Scales m_Item;
            public InternalTarget(Scales item)
                : base(1, false, TargetFlags.None)
            {
                this.m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                string message;

                if (targeted == this.m_Item)
                {
                    message = "It cannot weight itself.";
                }
                else if (targeted is Item)
                {
                    Item item = (Item)targeted;
                    object root = item.RootParent;

                    if ((root != null && root != from) || item.Parent == from)
                    {
                        message = "You decide that item's current location is too awkward to get an accurate result.";
                    }
                    else if (item.Movable)
                    {
                        if (item.Amount > 1)
                            message = "You place one item on the scale. ";
                        else
                            message = "You place that item on the scale. ";

                        double weight = item.Weight;

                        if (weight <= 0.0)
                            message += "It is lighter than a feather.";
                        else
                            message += String.Format("It weighs {0} stones.", weight);
                    }
                    else
                    {
                        message = "You cannot weigh that object.";
                    }
                }
                else
                {
                    message = "You cannot weigh that object.";
                }

                from.SendMessage(message);
            }
        }
    }
}