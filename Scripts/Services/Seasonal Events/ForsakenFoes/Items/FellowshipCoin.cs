using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class FellowshipCoin : Item
    {
        public override int LabelNumber => 1159036;  // The Fellowship Coin

        [Constructable]
        public FellowshipCoin()
            : base(0x2F60)
        {
            Weight = 1.0;
            Hue = 1912;
            Light = LightType.Circle300;
        }

        public FellowshipCoin(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(FellowshipCoinGump)))
            {
                from.SendGump(new FellowshipCoinGump());
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1157722, "its origin"); // *Your proficiency in ~1_SKILL~ reveals more about the item*
                from.PlaySound(1050);
            }
        }

        private class FellowshipCoinGump : Gump
        {
            public FellowshipCoinGump()
                : base(100, 100)
            {
                AddPage(0);

                AddBackground(0, 0, 454, 400, 0x24A4);
                AddItem(75, 120, 0x2F60);
                AddHtmlLocalized(177, 50, 250, 18, 1114513, "#1159036", 0x3442, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
                AddHtmlLocalized(177, 77, 250, 36, 1114513, "#1159033", 0x3442, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
                AddHtmlLocalized(177, 122, 250, 228, 1159037, 0xC63, true, true); // The coin's gilded brilliance is striking. Engraved on the coin are the letters T, W, U.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
