using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class WallBannerComponent : AddonComponent, IDyable
    {
        public WallBannerComponent(int itemID)
            : base(itemID)
        {
        }

        public WallBannerComponent(Serial serial)
            : base(serial)
        {
        }

        public override bool NeedsWall => true;
        public override Point3D WallPosition => East ? new Point3D(-1, 0, 0) : new Point3D(0, -1, 0);
        public bool East => ((WallBanner)Addon).East;
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

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            if (Addon != null)
                Addon.Hue = sender.DyedHue;

            return true;
        }
    }

    public class WallBanner : BaseAddon, IRewardItem
    {
        private bool m_IsRewardItem;
        private bool m_East;
        [Constructable]
        public WallBanner(int bannerID)
            : base()
        {
            m_East = ((bannerID % 2) == 1);

            switch (bannerID)
            {
                case 1:
                    AddComponent(new WallBannerComponent(0x161F), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x161E), 0, 1, 0);
                    AddComponent(new WallBannerComponent(0x161D), 0, 2, 0);
                    break;
                case 2:
                    AddComponent(new WallBannerComponent(0x1586), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1587), 1, 0, 0);
                    AddComponent(new WallBannerComponent(0x1588), 2, 0, 0);
                    break;
                case 3:
                    AddComponent(new WallBannerComponent(0x1622), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1621), 0, 1, 0);
                    AddComponent(new WallBannerComponent(0x1620), 0, 2, 0);
                    break;
                case 4:
                    AddComponent(new WallBannerComponent(0x1589), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x158A), 1, 0, 0);
                    AddComponent(new WallBannerComponent(0x158B), 2, 0, 0);
                    break;
                case 5:
                    AddComponent(new WallBannerComponent(0x1625), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1624), 0, 1, 0);
                    AddComponent(new WallBannerComponent(0x1623), 0, 2, 0);
                    break;
                case 6:
                    AddComponent(new WallBannerComponent(0x158C), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x158D), 1, 0, 0);
                    AddComponent(new WallBannerComponent(0x158E), 2, 0, 0);
                    break;
                case 7:
                    AddComponent(new WallBannerComponent(0x1628), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1627), 0, 1, 0);
                    AddComponent(new WallBannerComponent(0x1626), 0, 2, 0);
                    break;
                case 8:
                    AddComponent(new WallBannerComponent(0x1590), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1591), 1, 0, 0);
                    AddComponent(new WallBannerComponent(0x158F), 2, 0, 0);
                    break;
                case 9:
                    AddComponent(new WallBannerComponent(0x162A), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1629), 0, 1, 0);
                    AddComponent(new WallBannerComponent(0x1626), 0, 2, 0);
                    break;
                case 10:
                    AddComponent(new WallBannerComponent(0x1592), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1593), 1, 0, 0);
                    AddComponent(new WallBannerComponent(0x158F), 2, 0, 0);
                    break;
                case 11:
                    AddComponent(new WallBannerComponent(0x162D), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x162C), 0, 1, 0);
                    AddComponent(new WallBannerComponent(0x162B), 0, 2, 0);
                    break;
                case 12:
                    AddComponent(new WallBannerComponent(0x1594), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1595), 1, 0, 0);
                    AddComponent(new WallBannerComponent(0x1596), 2, 0, 0);
                    break;
                case 13:
                    AddComponent(new WallBannerComponent(0x1632), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1631), 0, 1, 0);
                    AddComponent(new WallBannerComponent(0x162E), 0, 2, 0);
                    break;
                case 14:
                    AddComponent(new WallBannerComponent(0x1598), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x159B), 1, 0, 0);
                    AddComponent(new WallBannerComponent(0x159C), 2, 0, 0);
                    break;
                case 15:
                    AddComponent(new WallBannerComponent(0x1633), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1630), 0, 1, 0);
                    AddComponent(new WallBannerComponent(0x162F), 0, 2, 0);
                    break;
                case 16:
                    AddComponent(new WallBannerComponent(0x1599), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x159A), 1, 0, 0);
                    AddComponent(new WallBannerComponent(0x159D), 2, 0, 0);
                    break;
                case 17:
                    AddComponent(new WallBannerComponent(0x1610), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x160F), 0, 1, 0);
                    break;
                case 18:
                    AddComponent(new WallBannerComponent(0x15A0), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x15A1), 1, 0, 0);
                    break;
                case 19:
                    AddComponent(new WallBannerComponent(0x1612), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1611), 0, 1, 0);
                    break;
                case 20:
                    AddComponent(new WallBannerComponent(0x15A2), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x15A3), 1, 0, 0);
                    break;
                case 21:
                    AddComponent(new WallBannerComponent(0x1614), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1613), 0, 1, 0);
                    break;
                case 22:
                    AddComponent(new WallBannerComponent(0x15A4), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x15A5), 1, 0, 0);
                    break;
                case 23:
                    AddComponent(new WallBannerComponent(0x1616), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1615), 0, 1, 0);
                    break;
                case 24:
                    AddComponent(new WallBannerComponent(0x15A6), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x15A7), 1, 0, 0);
                    break;
                case 25:
                    AddComponent(new WallBannerComponent(0x1618), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1617), 0, 1, 0);
                    break;
                case 26:
                    AddComponent(new WallBannerComponent(0x15A8), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x15A9), 1, 0, 0);
                    break;
                case 27:
                    AddComponent(new WallBannerComponent(0x161A), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1619), 0, 1, 0);
                    break;
                case 28:
                    AddComponent(new WallBannerComponent(0x15AA), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x15AB), 1, 0, 0);
                    break;
                case 29:
                    AddComponent(new WallBannerComponent(0x161C), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x161B), 0, 1, 0);
                    break;
                case 30:
                    AddComponent(new WallBannerComponent(0x15AC), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x15AD), 1, 0, 0);
                    break;
            }
        }

        public WallBanner(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                WallBannerDeed deed = new WallBannerDeed
                {
                    IsRewardItem = m_IsRewardItem
                };

                return deed;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return m_IsRewardItem;
            }
            set
            {
                m_IsRewardItem = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool East
        {
            get
            {
                return m_East;
            }
            set
            {
                m_IsRewardItem = value;
                InvalidateProperties();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(m_East);
            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_East = reader.ReadBool();
            m_IsRewardItem = reader.ReadBool();
        }
    }

    public class WallBannerDeed : BaseAddonDeed, IRewardItem
    {
        private int m_BannerID;
        private bool m_IsRewardItem;
        [Constructable]
        public WallBannerDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public WallBannerDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1080549;// Wall Banner Deed
        public override BaseAddon Addon
        {
            get
            {
                WallBanner addon = new WallBanner(m_BannerID)
                {
                    IsRewardItem = m_IsRewardItem
                };

                return addon;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return m_IsRewardItem;
            }
            set
            {
                m_IsRewardItem = value;
                InvalidateProperties();
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1076225); // 9th Year Veteran Reward
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
                return;

            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump(this));
            }
            else
                from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it.          	
        }

        public void Use(Mobile m, int bannerID)
        {
            m_BannerID = bannerID;

            base.OnDoubleClick(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_IsRewardItem = reader.ReadBool();
        }

        private class InternalGump : Gump
        {
            private readonly WallBannerDeed m_WallBanner;
            public InternalGump(WallBannerDeed WallBanner)
                : base(150, 50)
            {
                m_WallBanner = WallBanner;

                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddBackground(25, 0, 500, 265, 0xA28);
                AddLabel(70, 12, 0x3E3, "Choose a Wall Banner:");

                AddPage(1);

                AddItem(55, 110, 0x161D);
                AddItem(75, 90, 0x161E);
                AddItem(95, 70, 0x161F);
                AddButton(70, 50, 0x845, 0x846, 1, GumpButtonType.Reply, 0);
                AddItem(105, 70, 0x1586);
                AddItem(125, 90, 0x1587);
                AddItem(145, 110, 0x1588);
                AddButton(145, 50, 0x845, 0x846, 2, GumpButtonType.Reply, 0);
                AddItem(200, 110, 0x1620);
                AddItem(220, 90, 0x1621);
                AddItem(240, 70, 0x1622);
                AddButton(220, 50, 0x845, 0x846, 3, GumpButtonType.Reply, 0);
                AddItem(250, 70, 0x1589);
                AddItem(270, 90, 0x158A);
                AddItem(290, 110, 0x158B);
                AddButton(300, 50, 0x845, 0x846, 4, GumpButtonType.Reply, 0);
                AddItem(350, 110, 0x1623);
                AddItem(370, 90, 0x1624);
                AddItem(390, 70, 0x1625);
                AddButton(365, 50, 0x845, 0x846, 5, GumpButtonType.Reply, 0);
                AddItem(400, 70, 0x158C);
                AddItem(420, 90, 0x158D);
                AddItem(440, 110, 0x158E);
                AddButton(445, 50, 0x845, 0x846, 6, GumpButtonType.Reply, 0);
                AddButton(455, 205, 0x8B0, 0x8B0, 0, GumpButtonType.Page, 2);

                AddPage(2);

                AddItem(52, 110, 0x1626);
                AddItem(72, 90, 0x1627);
                AddItem(95, 70, 0x1628);
                AddButton(70, 50, 0x845, 0x846, 7, GumpButtonType.Reply, 0);
                AddItem(105, 70, 0x1590);
                AddItem(125, 90, 0x1591);
                AddItem(145, 110, 0x158F);
                AddButton(145, 50, 0x845, 0x846, 8, GumpButtonType.Reply, 0);
                AddItem(197, 110, 0x1626);
                AddItem(217, 90, 0x1629);
                AddItem(240, 70, 0x162A);
                AddButton(220, 50, 0x845, 0x846, 9, GumpButtonType.Reply, 0);
                AddItem(250, 70, 0x1592);
                AddItem(270, 90, 0x1593);
                AddItem(290, 110, 0x158F);
                AddButton(300, 50, 0x845, 0x846, 10, GumpButtonType.Reply, 0);
                AddItem(340, 110, 0x162B);
                AddItem(363, 90, 0x162C);
                AddItem(385, 70, 0x162D);
                AddButton(365, 50, 0x845, 0x846, 11, GumpButtonType.Reply, 0);
                AddItem(395, 70, 0x1594);
                AddItem(417, 90, 0x1595);
                AddItem(439, 111, 0x1596);
                AddButton(445, 50, 0x845, 0x846, 12, GumpButtonType.Reply, 0);
                AddButton(70, 205, 0x8AF, 0x8AF, 0, GumpButtonType.Page, 1);
                AddButton(455, 205, 0x8B0, 0x8B0, 0, GumpButtonType.Page, 3);

                AddPage(3);

                AddItem(55, 110, 0x162E);
                AddItem(75, 93, 0x1631);
                AddItem(95, 70, 0x1632);
                AddButton(70, 50, 0x845, 0x846, 13, GumpButtonType.Reply, 0);
                AddItem(118, 70, 0x1598);
                AddItem(138, 94, 0x159B);
                AddItem(159, 113, 0x159C);
                AddButton(160, 50, 0x845, 0x846, 14, GumpButtonType.Reply, 0);
                AddItem(219, 111, 0x162F);
                AddItem(238, 94, 0x1630);
                AddItem(258, 70, 0x1633);
                AddButton(240, 50, 0x845, 0x846, 15, GumpButtonType.Reply, 0);
                AddItem(279, 70, 0x1599);
                AddItem(298, 93, 0x159A);
                AddItem(319, 113, 0x159D);
                AddButton(320, 50, 0x845, 0x846, 16, GumpButtonType.Reply, 0);
                AddItem(380, 90, 0x160F);
                AddItem(400, 70, 0x1610);
                AddButton(390, 50, 0x845, 0x846, 17, GumpButtonType.Reply, 0);
                AddItem(420, 70, 0x15A0);
                AddItem(440, 90, 0x15A1);
                AddButton(455, 50, 0x845, 0x846, 18, GumpButtonType.Reply, 0);
                AddButton(70, 205, 0x8AF, 0x8AF, 0, GumpButtonType.Page, 2);
                AddButton(455, 205, 0x8B0, 0x8B0, 0, GumpButtonType.Page, 4);

                AddPage(4);

                AddItem(55, 90, 0x1611);
                AddItem(75, 70, 0x1612);
                AddButton(70, 50, 0x845, 0x846, 19, GumpButtonType.Reply, 0);
                AddItem(105, 70, 0x15A2);
                AddItem(125, 90, 0x15A3);
                AddButton(145, 50, 0x845, 0x846, 20, GumpButtonType.Reply, 0);
                AddItem(200, 84, 0x1613);
                AddItem(220, 70, 0x1614);
                AddButton(215, 50, 0x845, 0x846, 21, GumpButtonType.Reply, 0);
                AddItem(250, 70, 0x15A4);
                AddItem(270, 84, 0x15A5);
                AddButton(290, 50, 0x845, 0x846, 22, GumpButtonType.Reply, 0);
                AddItem(350, 90, 0x1615);
                AddItem(370, 70, 0x1616);
                AddButton(365, 50, 0x845, 0x846, 23, GumpButtonType.Reply, 0);
                AddItem(400, 70, 0x15A6);
                AddItem(420, 90, 0x15A7);
                AddButton(445, 50, 0x845, 0x846, 24, GumpButtonType.Reply, 0);
                AddButton(70, 205, 0x8AF, 0x8AF, 0, GumpButtonType.Page, 3);
                AddButton(455, 205, 0x8B0, 0x8B0, 0, GumpButtonType.Page, 5);

                AddPage(5);

                AddItem(55, 90, 0x1617);
                AddItem(77, 70, 0x1618);
                AddButton(70, 50, 0x845, 0x846, 25, GumpButtonType.Reply, 0);
                AddItem(105, 70, 0x15A8);
                AddItem(127, 90, 0x15A9);
                AddButton(145, 50, 0x845, 0x846, 26, GumpButtonType.Reply, 0);
                AddItem(200, 90, 0x1619);
                AddItem(222, 70, 0x161A);
                AddButton(220, 50, 0x845, 0x846, 27, GumpButtonType.Reply, 0);
                AddItem(250, 70, 0x15AA);
                AddItem(272, 90, 0x15AB);
                AddButton(300, 50, 0x845, 0x846, 28, GumpButtonType.Reply, 0);
                AddItem(350, 90, 0x161B);
                AddItem(372, 70, 0x161C);
                AddButton(365, 50, 0x845, 0x846, 29, GumpButtonType.Reply, 0);
                AddItem(400, 70, 0x15AC);
                AddItem(422, 90, 0x15AD);
                AddButton(445, 50, 0x845, 0x846, 30, GumpButtonType.Reply, 0);
                AddButton(70, 205, 0x8AF, 0x8AF, 0, GumpButtonType.Page, 4);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_WallBanner == null || m_WallBanner.Deleted)
                    return;

                if (info.ButtonID > 0 && info.ButtonID < 31)
                    m_WallBanner.Use(sender.Mobile, info.ButtonID);
            }
        }
    }
}
