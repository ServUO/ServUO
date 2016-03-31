using System;
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

        public override bool NeedsWall
        {
            get
            {
                return true;
            }
        }
        public override Point3D WallPosition
        {
            get
            {
                return this.East ? new Point3D(-1, 0, 0) : new Point3D(0, -1, 0);
            }
        }
        public bool East
        {
            get
            {
                return ((WallBanner)this.Addon).East;
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

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (this.Deleted)
                return false;

            if (this.Addon != null)
                this.Addon.Hue = sender.DyedHue;

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
            this.m_East = ((bannerID % 2) == 1);

            switch ( bannerID )
            {
                case 1: 
                    this.AddComponent(new WallBannerComponent(0x161F), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x161E), 0, 1, 0);
                    this.AddComponent(new WallBannerComponent(0x161D), 0, 2, 0);
                    break;
                case 2: 
                    this.AddComponent(new WallBannerComponent(0x1586), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x1587), 1, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x1588), 2, 0, 0);
                    break;
                case 3: 
                    this.AddComponent(new WallBannerComponent(0x1622), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x1621), 0, 1, 0);
                    this.AddComponent(new WallBannerComponent(0x1620), 0, 2, 0);
                    break;
                case 4: 
                    this.AddComponent(new WallBannerComponent(0x1589), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x158A), 1, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x158B), 2, 0, 0);
                    break;
                case 5: 
                    this.AddComponent(new WallBannerComponent(0x1625), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x1624), 0, 1, 0);
                    this.AddComponent(new WallBannerComponent(0x1623), 0, 2, 0);
                    break;
                case 6: 
                    this.AddComponent(new WallBannerComponent(0x158C), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x158D), 1, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x158E), 2, 0, 0);
                    break;
                case 7: 
                    this.AddComponent(new WallBannerComponent(0x1628), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x1627), 0, 1, 0);
                    this.AddComponent(new WallBannerComponent(0x1626), 0, 2, 0);
                    break;
                case 8: 
                    this.AddComponent(new WallBannerComponent(0x1590), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x1591), 1, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x158F), 2, 0, 0);
                    break;
                case 9: 
                    this.AddComponent(new WallBannerComponent(0x162A), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x1629), 0, 1, 0);
                    this.AddComponent(new WallBannerComponent(0x1626), 0, 2, 0);
                    break;
                case 10: 
                    this.AddComponent(new WallBannerComponent(0x1592), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x1593), 1, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x158F), 2, 0, 0);
                    break;
                case 11: 
                    this.AddComponent(new WallBannerComponent(0x162D), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x162C), 0, 1, 0);
                    this.AddComponent(new WallBannerComponent(0x162B), 0, 2, 0);
                    break;
                case 12: 
                    this.AddComponent(new WallBannerComponent(0x1594), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x1595), 1, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x1596), 2, 0, 0);
                    break;
                case 13: 
                    this.AddComponent(new WallBannerComponent(0x1632), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x1631), 0, 1, 0);
                    this.AddComponent(new WallBannerComponent(0x162E), 0, 2, 0);
                    break;
                case 14: 
                    this.AddComponent(new WallBannerComponent(0x1598), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x159B), 1, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x159C), 2, 0, 0);
                    break;
                case 15: 
                    this.AddComponent(new WallBannerComponent(0x1633), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x1630), 0, 1, 0);
                    this.AddComponent(new WallBannerComponent(0x162F), 0, 2, 0);
                    break;
                case 16: 
                    this.AddComponent(new WallBannerComponent(0x1599), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x159A), 1, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x159D), 2, 0, 0);
                    break;
                case 17: 
                    this.AddComponent(new WallBannerComponent(0x1610), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x160F), 0, 1, 0);
                    break;
                case 18: 
                    this.AddComponent(new WallBannerComponent(0x15A0), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x15A1), 1, 0, 0);
                    break;
                case 19: 
                    this.AddComponent(new WallBannerComponent(0x1612), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x1611), 0, 1, 0);
                    break;
                case 20: 
                    this.AddComponent(new WallBannerComponent(0x15A2), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x15A3), 1, 0, 0);
                    break;
                case 21: 
                    this.AddComponent(new WallBannerComponent(0x1614), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x1613), 0, 1, 0);
                    break;
                case 22: 
                    this.AddComponent(new WallBannerComponent(0x15A4), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x15A5), 1, 0, 0);
                    break;
                case 23: 
                    this.AddComponent(new WallBannerComponent(0x1616), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x1615), 0, 1, 0);
                    break;
                case 24: 
                    this.AddComponent(new WallBannerComponent(0x15A6), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x15A7), 1, 0, 0);
                    break;
                case 25: 
                    this.AddComponent(new WallBannerComponent(0x1618), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x1617), 0, 1, 0);
                    break;
                case 26: 
                    this.AddComponent(new WallBannerComponent(0x15A8), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x15A9), 1, 0, 0);
                    break;
                case 27: 
                    this.AddComponent(new WallBannerComponent(0x161A), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x1619), 0, 1, 0);
                    break;
                case 28: 
                    this.AddComponent(new WallBannerComponent(0x15AA), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x15AB), 1, 0, 0);
                    break;
                case 29: 
                    this.AddComponent(new WallBannerComponent(0x161C), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x161B), 0, 1, 0);
                    break;
                case 30: 
                    this.AddComponent(new WallBannerComponent(0x15AC), 0, 0, 0);
                    this.AddComponent(new WallBannerComponent(0x15AD), 1, 0, 0);
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
                WallBannerDeed deed = new WallBannerDeed();
                deed.IsRewardItem = this.m_IsRewardItem;

                return deed;	
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return this.m_IsRewardItem;
            }
            set
            {
                this.m_IsRewardItem = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool East
        {
            get
            {
                return this.m_East;
            }
            set
            {
                this.m_IsRewardItem = value;
                this.InvalidateProperties();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
			
            writer.Write((bool)this.m_East);
            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
			
            this.m_East = reader.ReadBool();
            this.m_IsRewardItem = reader.ReadBool();
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
            this.LootType = LootType.Blessed;
        }

        public WallBannerDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1080549;
            }
        }// Wall Banner Deed
        public override BaseAddon Addon
        {
            get
            {
                WallBanner addon = new WallBanner(this.m_BannerID);
                addon.IsRewardItem = this.m_IsRewardItem;

                return addon;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return this.m_IsRewardItem;
            }
            set
            {
                this.m_IsRewardItem = value;
                this.InvalidateProperties();
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
            if (this.m_IsRewardItem)
                list.Add(1076225); // 9th Year Veteran Reward
        }

        public override void OnDoubleClick(Mobile from)
        { 
            if (this.m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
                return;
		
            if (this.IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump(this));
            }
            else
                from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it.          	
        }

        public void Use(Mobile m, int bannerID)
        {
            this.m_BannerID = bannerID;
		
            base.OnDoubleClick(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
			
            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
			
            this.m_IsRewardItem = reader.ReadBool();
        }

        private class InternalGump : Gump
        {
            private readonly WallBannerDeed m_WallBanner;
            public InternalGump(WallBannerDeed WallBanner)
                : base(150, 50)
            {
                this.m_WallBanner = WallBanner;
				
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
				
                this.AddBackground(25, 0, 500, 265, 0xA28);
                this.AddLabel(70, 12, 0x3E3, "Choose a Wall Banner:");

                this.AddPage(1);

                this.AddItem(55, 110, 0x161D);
                this.AddItem(75, 90, 0x161E);
                this.AddItem(95, 70, 0x161F);
                this.AddButton(70, 50, 0x845, 0x846, 1, GumpButtonType.Reply, 0);
                this.AddItem(105, 70, 0x1586);
                this.AddItem(125, 90, 0x1587);
                this.AddItem(145, 110, 0x1588);
                this.AddButton(145, 50, 0x845, 0x846, 2, GumpButtonType.Reply, 0);
                this.AddItem(200, 110, 0x1620);
                this.AddItem(220, 90, 0x1621);
                this.AddItem(240, 70, 0x1622);
                this.AddButton(220, 50, 0x845, 0x846, 3, GumpButtonType.Reply, 0);
                this.AddItem(250, 70, 0x1589);
                this.AddItem(270, 90, 0x158A);
                this.AddItem(290, 110, 0x158B);
                this.AddButton(300, 50, 0x845, 0x846, 4, GumpButtonType.Reply, 0);
                this.AddItem(350, 110, 0x1623);
                this.AddItem(370, 90, 0x1624);
                this.AddItem(390, 70, 0x1625);
                this.AddButton(365, 50, 0x845, 0x846, 5, GumpButtonType.Reply, 0);
                this.AddItem(400, 70, 0x158C);
                this.AddItem(420, 90, 0x158D);
                this.AddItem(440, 110, 0x158E);
                this.AddButton(445, 50, 0x845, 0x846, 6, GumpButtonType.Reply, 0);
                this.AddButton(455, 205, 0x8B0, 0x8B0, 0, GumpButtonType.Page, 2);

                this.AddPage(2);

                this.AddItem(52, 110, 0x1626);
                this.AddItem(72, 90, 0x1627);
                this.AddItem(95, 70, 0x1628);
                this.AddButton(70, 50, 0x845, 0x846, 7, GumpButtonType.Reply, 0);
                this.AddItem(105, 70, 0x1590);
                this.AddItem(125, 90, 0x1591);
                this.AddItem(145, 110, 0x158F);
                this.AddButton(145, 50, 0x845, 0x846, 8, GumpButtonType.Reply, 0);
                this.AddItem(197, 110, 0x1626);
                this.AddItem(217, 90, 0x1629);
                this.AddItem(240, 70, 0x162A);
                this.AddButton(220, 50, 0x845, 0x846, 9, GumpButtonType.Reply, 0);
                this.AddItem(250, 70, 0x1592);
                this.AddItem(270, 90, 0x1593);
                this.AddItem(290, 110, 0x158F);
                this.AddButton(300, 50, 0x845, 0x846, 10, GumpButtonType.Reply, 0);
                this.AddItem(340, 110, 0x162B);
                this.AddItem(363, 90, 0x162C);
                this.AddItem(385, 70, 0x162D);
                this.AddButton(365, 50, 0x845, 0x846, 11, GumpButtonType.Reply, 0);
                this.AddItem(395, 70, 0x1594);
                this.AddItem(417, 90, 0x1595);
                this.AddItem(439, 111, 0x1596);
                this.AddButton(445, 50, 0x845, 0x846, 12, GumpButtonType.Reply, 0);
                this.AddButton(70, 205, 0x8AF, 0x8AF, 0, GumpButtonType.Page, 1);
                this.AddButton(455, 205, 0x8B0, 0x8B0, 0, GumpButtonType.Page, 3);

                this.AddPage(3);

                this.AddItem(55, 110, 0x162E);
                this.AddItem(75, 93, 0x1631);
                this.AddItem(95, 70, 0x1632);
                this.AddButton(70, 50, 0x845, 0x846, 13, GumpButtonType.Reply, 0);
                this.AddItem(118, 70, 0x1598);
                this.AddItem(138, 94, 0x159B);
                this.AddItem(159, 113, 0x159C);
                this.AddButton(160, 50, 0x845, 0x846, 14, GumpButtonType.Reply, 0);
                this.AddItem(219, 111, 0x162F);
                this.AddItem(238, 94, 0x1630);
                this.AddItem(258, 70, 0x1633);
                this.AddButton(240, 50, 0x845, 0x846, 15, GumpButtonType.Reply, 0);
                this.AddItem(279, 70, 0x1599);
                this.AddItem(298, 93, 0x159A);
                this.AddItem(319, 113, 0x159D);
                this.AddButton(320, 50, 0x845, 0x846, 16, GumpButtonType.Reply, 0);
                this.AddItem(380, 90, 0x160F);
                this.AddItem(400, 70, 0x1610);
                this.AddButton(390, 50, 0x845, 0x846, 17, GumpButtonType.Reply, 0);
                this.AddItem(420, 70, 0x15A0);
                this.AddItem(440, 90, 0x15A1);
                this.AddButton(455, 50, 0x845, 0x846, 18, GumpButtonType.Reply, 0);
                this.AddButton(70, 205, 0x8AF, 0x8AF, 0, GumpButtonType.Page, 2);
                this.AddButton(455, 205, 0x8B0, 0x8B0, 0, GumpButtonType.Page, 4);

                this.AddPage(4);

                this.AddItem(55, 90, 0x1611);
                this.AddItem(75, 70, 0x1612);
                this.AddButton(70, 50, 0x845, 0x846, 19, GumpButtonType.Reply, 0);
                this.AddItem(105, 70, 0x15A2);
                this.AddItem(125, 90, 0x15A3);
                this.AddButton(145, 50, 0x845, 0x846, 20, GumpButtonType.Reply, 0);
                this.AddItem(200, 84, 0x1613);
                this.AddItem(220, 70, 0x1614);
                this.AddButton(215, 50, 0x845, 0x846, 21, GumpButtonType.Reply, 0);
                this.AddItem(250, 70, 0x15A4);
                this.AddItem(270, 84, 0x15A5);
                this.AddButton(290, 50, 0x845, 0x846, 22, GumpButtonType.Reply, 0);
                this.AddItem(350, 90, 0x1615);
                this.AddItem(370, 70, 0x1616);
                this.AddButton(365, 50, 0x845, 0x846, 23, GumpButtonType.Reply, 0);
                this.AddItem(400, 70, 0x15A6);
                this.AddItem(420, 90, 0x15A7);
                this.AddButton(445, 50, 0x845, 0x846, 24, GumpButtonType.Reply, 0);
                this.AddButton(70, 205, 0x8AF, 0x8AF, 0, GumpButtonType.Page, 3);
                this.AddButton(455, 205, 0x8B0, 0x8B0, 0, GumpButtonType.Page, 5);

                this.AddPage(5);

                this.AddItem(55, 90, 0x1617);
                this.AddItem(77, 70, 0x1618);
                this.AddButton(70, 50, 0x845, 0x846, 25, GumpButtonType.Reply, 0);
                this.AddItem(105, 70, 0x15A8);
                this.AddItem(127, 90, 0x15A9);
                this.AddButton(145, 50, 0x845, 0x846, 26, GumpButtonType.Reply, 0);
                this.AddItem(200, 90, 0x1619);
                this.AddItem(222, 70, 0x161A);
                this.AddButton(220, 50, 0x845, 0x846, 27, GumpButtonType.Reply, 0);
                this.AddItem(250, 70, 0x15AA);
                this.AddItem(272, 90, 0x15AB);
                this.AddButton(300, 50, 0x845, 0x846, 28, GumpButtonType.Reply, 0);
                this.AddItem(350, 90, 0x161B);
                this.AddItem(372, 70, 0x161C);
                this.AddButton(365, 50, 0x845, 0x846, 29, GumpButtonType.Reply, 0);
                this.AddItem(400, 70, 0x15AC);
                this.AddItem(422, 90, 0x15AD);
                this.AddButton(445, 50, 0x845, 0x846, 30, GumpButtonType.Reply, 0);
                this.AddButton(70, 205, 0x8AF, 0x8AF, 0, GumpButtonType.Page, 4);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_WallBanner == null || this.m_WallBanner.Deleted)
                    return;

                if (info.ButtonID > 0 && info.ButtonID < 31)
                    this.m_WallBanner.Use(sender.Mobile, info.ButtonID);
            }
        }
    }
}