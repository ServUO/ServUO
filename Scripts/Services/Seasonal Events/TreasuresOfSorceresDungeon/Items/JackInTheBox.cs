using System;

using Server;
using Server.Events.Halloween;

namespace Server.Items
{
    public class JackInTheBox : Item
    {
        public override int LabelNumber { get { return 1157655; } } // Jack in the Box

        private DateTime _LastUse;

        [Constructable]
        public JackInTheBox()
            : base(0x9F64)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (_LastUse + TimeSpan.FromSeconds(20) < DateTime.UtcNow)
            {
                ItemID = 0x9F64;

                Effects.PlaySound(GetWorldLocation(), Map, 1661);

                Timer.DelayCall(TimeSpan.FromSeconds(9), () =>
                    {
                        ItemID = 0x9F65;

                        Timer.DelayCall(TimeSpan.FromSeconds(5), () =>
                            {
                                ItemID = 0x9F64;
                            });
                    });

                _LastUse = DateTime.UtcNow;
            }
            else
            {
                from.SendLocalizedMessage(501789); // You must wait before trying again.
            }
        }

        public JackInTheBox(Serial serial)
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
            int v = reader.ReadInt();
        }
    }
}