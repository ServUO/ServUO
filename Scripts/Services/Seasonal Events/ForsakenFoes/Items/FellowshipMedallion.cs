using System;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class FellowshipMedallion : Item
    {
        public override int LabelNumber { get { return 1159248; } } // Fellowship Medallion

        [Constructable]
        public FellowshipMedallion()
            : base(0xA429)
        {
            Weight = 1.0;
            Layer = Layer.Neck;
        }

        public FellowshipMedallion(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(FellowshipMedallionGump)))
            {
                from.SendGump(new FellowshipMedallionGump());
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1157722, "its origin"); // *Your proficiency in ~1_SKILL~ reveals more about the item*
                from.PlaySound(1050);
            }
        }

        private class FellowshipMedallionGump : Gump
        {
            public FellowshipMedallionGump()
                : base(100, 100)
            {
                AddPage(0);

                AddBackground(0, 0, 454, 400, 0x24A4);
                AddItem(75, 120, 0xA429);
                AddHtmlLocalized(177, 50, 250, 18, 1114513, "#1159248", 0x3442, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
                AddHtmlLocalized(177, 77, 250, 36, 1114513, "#1159033", 0x3442, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
                AddHtmlLocalized(177, 122, 250, 228, 1159247, 0xC63, true, true); // This is an otherwise unassuming metal medallion in the shape of a triangle.  The letters T, W, and U are engraved on it. It is almost immediately recognizable as a sign of the Fellowship.
            }
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
    }
}
