using Server.Gumps;
using Server.Mobiles;
using System;

namespace Server.Engines.Astronomy
{
    public class ConstellationLedger : Item
    {
        public override int LabelNumber => 1158520;  // Constellation Ledger

        [Constructable]
        public ConstellationLedger()
            : base(0xFF4)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && m.InRange(GetWorldLocation(), 3))
            {
                BaseGump.SendGump(new ConstellationLedgerGump((PlayerMobile)m));
            }
        }

        public class ConstellationLedgerGump : BaseGump
        {
            public int Page { get; set; }

            public int Pages => (int)Math.Ceiling(AstronomySystem.DiscoveredConstellations.Count / 20.0);

            public ConstellationLedgerGump(PlayerMobile pm)
                : base(pm, 100, 100)
            {
                Page = 0;
            }

            public override void AddGumpLayout()
            {
                AddPage(0);

                AddBackground(0, 0, 820, 620, 0x2454);
                AddHtmlLocalized(10, 28, 800, 18, 1114513, "#1158520", 0x0, false, false); // Constellation Ledger
                AddHtmlLocalized(295, 55, 515, 36, 1158521, string.Format("{0}\t{1}", AstronomySystem.DiscoveredConstellations.Count, AstronomySystem.MaxConstellations), 0x0, false, false); // Constellations Discovered: ~1_VAL~ / ~2_VAL~

                AddHtmlLocalized(55, 100, 100, 36, 1114513, "#1158522", 0x0, false, false); // Constellation Name
                AddHtmlLocalized(245, 100, 80, 36, 1114513, "#1158523", 0x0, false, false); // Astronomer
                AddHtmlLocalized(375, 100, 80, 36, 1114513, "#1158524", 0x0, false, false); // Discovery Date
                AddHtmlLocalized(505, 100, 80, 36, 1114513, "#1158525", 0x0, false, false); // Night Period
                AddHtmlLocalized(635, 100, 80, 36, 1114513, "#1158526", 0x0, false, false); // Coordinates

                int start = Page * 20;
                int y = 145;

                for (int i = start; i < AstronomySystem.DiscoveredConstellations.Count && i <= start + 20; i++)
                {
                    ConstellationInfo info = AstronomySystem.GetConstellation(AstronomySystem.DiscoveredConstellations[i]);

                    AddHtml(15, y, 200, 18, Color("#0040FF", info.Name), false, false);
                    AddHtml(240, y, 112, 18, Color("#0040FF", info.DiscoveredBy != null ? info.DiscoveredBy.Name : "Unknown"), false, false);
                    AddHtml(380, y, 112, 18, Color("#0040FF", info.DiscoveredOn.ToShortDateString()), false, false);
                    AddHtmlLocalized(492, y, 130, 18, AstronomySystem.TimeCoordinateLocalization(info.TimeCoordinate), 0x1F, false, false);
                    AddHtmlLocalized(632, y, 150, 18, 1158527, string.Format("{0}\t{1}", info.CoordRA, info.CoordDEC), 0x1F, false, false); // RA ~1_VAL~  DEC ~2_VAL~

                    y += 18;
                }

                AddButton(340, 540, 0x605, 0x606, 1, GumpButtonType.Reply, 0);
                AddButton(370, 540, 0x609, 0x60A, 2, GumpButtonType.Reply, 0);
                AddButton(460, 540, 0x607, 0x608, 3, GumpButtonType.Reply, 0);
                AddButton(484, 540, 0x603, 0x604, 4, GumpButtonType.Reply, 0);

                AddLabel(415, 570, 0, string.Format("{0}/{1}", Page + 1, Pages.ToString()));
            }

            public override void OnResponse(RelayInfo info)
            {
                switch (info.ButtonID)
                {
                    case 0:
                        return;
                    case 1:
                        Page = 0;
                        break;
                    case 2:
                        Page = Math.Max(0, Page - 1);
                        break;
                    case 3:
                        Page = Math.Min(Page + 1, Math.Max(0, Pages - 1));
                        break;
                    case 4:
                        Page = Math.Max(0, Pages - 1);
                        break;
                }

                Refresh();
            }
        }

        public ConstellationLedger(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
