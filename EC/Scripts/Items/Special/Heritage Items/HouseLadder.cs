using System;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class HouseLadderAddon : BaseAddon
    {
        [Constructable]
        public HouseLadderAddon(int type)
            : base()
        {
            switch ( type )
            {
                case 0: // castle south
                    this.AddComponent(new LocalizedAddonComponent(0x3DB2, 1076791), 0, 0, 0);
                    this.AddComponent(new LocalizedAddonComponent(0x3F28, 1076791), 0, 1, 28);
                    this.AddComponent(new LocalizedAddonComponent(0x3DB4, 1076791), 0, 2, 20);
                    break;
                case 1: // castle east
                    this.AddComponent(new LocalizedAddonComponent(0x3DB3, 1076791), 0, 0, 0);
                    this.AddComponent(new LocalizedAddonComponent(0x3F28, 1076791), 1, 0, 28);
                    this.AddComponent(new LocalizedAddonComponent(0x3DB5, 1076791), 2, 0, 20);
                    break;
                case 2: // castle north
                    this.AddComponent(new LocalizedAddonComponent(0x2FDF, 1076791), 0, 0, 0);
                    this.AddComponent(new LocalizedAddonComponent(0x3F28, 1076791), 0, -1, 28);
                    this.AddComponent(new LocalizedAddonComponent(0x3DB6, 1076791), 0, -2, 20);
                    break;
                case 3: // castle west
                    this.AddComponent(new LocalizedAddonComponent(0x2FDE, 1076791), 0, 0, 0);
                    this.AddComponent(new LocalizedAddonComponent(0x3F28, 1076791), -1, 0, 28);
                    this.AddComponent(new LocalizedAddonComponent(0x3DB7, 1076791), -2, 0, 20);
                    break;
                case 4: // south
                    this.AddComponent(new LocalizedAddonComponent(0x3DB2, 1076287), 0, 0, 0);
                    break;
                case 5: // east
                    this.AddComponent(new LocalizedAddonComponent(0x3DB3, 1076287), 0, 0, 0);
                    break;
                case 6: // north
                    this.AddComponent(new LocalizedAddonComponent(0x2FDF, 1076287), 0, 0, 0);
                    break;
                case 7: // west
                    this.AddComponent(new LocalizedAddonComponent(0x2FDE, 1076287), 0, 0, 0);
                    break;
            }
        }

        public HouseLadderAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new HouseLadderDeed();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class HouseLadderDeed : BaseAddonDeed
    {
        private int m_Type;
        [Constructable]
        public HouseLadderDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public HouseLadderDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new HouseLadderAddon(this.m_Type);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076287;
            }
        }// Ladder
        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump(this));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        private void SendTarget(Mobile m)
        {
            base.OnDoubleClick(m);
        }

        private class InternalGump : Gump
        {
            private readonly HouseLadderDeed m_Deed;
            public InternalGump(HouseLadderDeed deed)
                : base(60, 36)
            {
                this.m_Deed = deed;

                this.AddPage(0);

                this.AddBackground(0, 0, 273, 324, 0x13BE);
                this.AddImageTiled(10, 10, 253, 20, 0xA40);
                this.AddImageTiled(10, 40, 253, 244, 0xA40);
                this.AddImageTiled(10, 294, 253, 20, 0xA40);
                this.AddAlphaRegion(10, 10, 253, 304);
                this.AddButton(10, 294, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 296, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
                this.AddHtmlLocalized(14, 12, 273, 20, 1076780, 0x7FFF, false, false); // Please select your ladder position.  <br>Use the ladders marked (castle) <br> for accessing the tops of keeps <br> and castles.

                this.AddPage(1);

                this.AddButton(19, 49, 0x845, 0x846, 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 47, 213, 20, 1076794, 0x7FFF, false, false); // South (Castle)
                this.AddButton(19, 73, 0x845, 0x846, 2, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 71, 213, 20, 1076795, 0x7FFF, false, false); // East (Castle)
                this.AddButton(19, 97, 0x845, 0x846, 3, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 95, 213, 20, 1076792, 0x7FFF, false, false); // North (Castle)
                this.AddButton(19, 121, 0x845, 0x846, 4, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 119, 213, 20, 1076793, 0x7FFF, false, false); // West (Castle)
                this.AddButton(19, 145, 0x845, 0x846, 5, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 143, 213, 20, 1075386, 0x7FFF, false, false); // South
                this.AddButton(19, 169, 0x845, 0x846, 6, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 167, 213, 20, 1075387, 0x7FFF, false, false); // East
                this.AddButton(19, 193, 0x845, 0x846, 7, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 191, 213, 20, 1075389, 0x7FFF, false, false); // North
                this.AddButton(19, 217, 0x845, 0x846, 8, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 215, 213, 20, 1075390, 0x7FFF, false, false); // West
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Deed == null || this.m_Deed.Deleted || info.ButtonID == 0)
                    return;

                if (info.ButtonID >= 1 && info.ButtonID <= 8)
                {
                    this.m_Deed.m_Type = info.ButtonID - 1;
                    this.m_Deed.SendTarget(sender.Mobile);
                }
            }
        }
    }
}