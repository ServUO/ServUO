using Server.Gumps;
using System;

namespace Server.Items
{
    public class HouseRaffleDeed : Item
    {
        private HouseRaffleStone m_Stone;
        private Point3D m_PlotLocation;
        private Map m_Facet;
        private Mobile m_AwardedTo;
        [Constructable]
        public HouseRaffleDeed()
            : this(null, null)
        {
        }

        public HouseRaffleDeed(HouseRaffleStone stone, Mobile m)
            : base(0x2830)
        {
            m_Stone = stone;

            if (stone != null)
            {
                m_PlotLocation = stone.GetPlotCenter();
                m_Facet = stone.PlotFacet;
            }

            m_AwardedTo = m;

            LootType = LootType.Blessed;
            Hue = 0x501;
        }

        public HouseRaffleDeed(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public HouseRaffleStone Stone
        {
            get
            {
                return m_Stone;
            }
            set
            {
                m_Stone = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public Point3D PlotLocation
        {
            get
            {
                return m_PlotLocation;
            }
            set
            {
                m_PlotLocation = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public Map PlotFacet
        {
            get
            {
                return m_Facet;
            }
            set
            {
                m_Facet = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public Mobile AwardedTo
        {
            get
            {
                return m_AwardedTo;
            }
            set
            {
                m_AwardedTo = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public bool IsExpired => (m_Stone == null || m_Stone.Deleted || m_Stone.IsExpired);
        public override string DefaultName => "a writ of lease";
        public override double DefaultWeight => 1.0;
        public bool ValidLocation()
        {
            return (m_PlotLocation != Point3D.Zero && m_Facet != null && m_Facet != Map.Internal);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (ValidLocation())
            {
                list.Add(1060658, "location\t{0}", HouseRaffleStone.FormatLocation(m_PlotLocation, m_Facet, false)); // ~1_val~: ~2_val~
                list.Add(1060659, "facet\t{0}", m_Facet); // ~1_val~: ~2_val~
                list.Add(1150486); // [Marked Item]
            }

            if (IsExpired)
                list.Add(1150487); // [Expired]
            //list.Add( 1060660, "shard\t{0}", ServerList.ServerName ); // ~1_val~: ~2_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!ValidLocation())
                return;

            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(WritOfLeaseGump));
                from.SendGump(new WritOfLeaseGump(this));
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write(m_Stone);
            writer.Write(m_PlotLocation);
            writer.Write(m_Facet);
            writer.Write(m_AwardedTo);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_Stone = reader.ReadItem<HouseRaffleStone>();

                        goto case 0;
                    }
                case 0:
                    {
                        m_PlotLocation = reader.ReadPoint3D();
                        m_Facet = reader.ReadMap();
                        m_AwardedTo = reader.ReadMobile();

                        break;
                    }
            }
        }

        private class WritOfLeaseGump : Gump
        {
            public WritOfLeaseGump(HouseRaffleDeed deed)
                : base(150, 50)
            {
                AddPage(0);

                AddImage(0, 0, 9380);
                AddImage(114, 0, 9381);
                AddImage(171, 0, 9382);
                AddImage(0, 140, 9383);
                AddImage(114, 140, 9384);
                AddImage(171, 140, 9385);
                AddImage(0, 182, 9383);
                AddImage(114, 182, 9384);
                AddImage(171, 182, 9385);
                AddImage(0, 224, 9383);
                AddImage(114, 224, 9384);
                AddImage(171, 224, 9385);
                AddImage(0, 266, 9386);
                AddImage(114, 266, 9387);
                AddImage(171, 266, 9388);

                AddHtmlLocalized(30, 48, 229, 20, 1150484, 200, false, false); // WRIT OF LEASE
                AddHtml(28, 75, 231, 280, FormatDescription(deed), false, true);
            }

            private static string FormatDescription(HouseRaffleDeed deed)
            {
                if (deed == null)
                    return string.Empty;

                if (deed.IsExpired)
                {
                    return string.Format(
                                         "<bodytextblack>" +
                                         "This deed once entitled the bearer to build a house on the plot of land " +
                                         "located at {0} on the {1} facet.<br><br>" +
                                         "The deed has expired, and now the indicated plot of land " +
                                         "is subject to normal house construction rules.<br><br>" +
                                         "This deed functions as a recall rune marked for the location of the plot it represents." +
                                         "</bodytextblack>",
                        HouseRaffleStone.FormatLocation(deed.PlotLocation, deed.PlotFacet, false),
                        deed.PlotFacet);
                }
                else
                {
                    int daysLeft = (int)Math.Ceiling((deed.Stone.Started + deed.Stone.Duration + HouseRaffleStone.ExpirationTime - DateTime.UtcNow).TotalDays);

                    return string.Format(
                                         "<bodytextblack>" +
                                         "This deed entitles the bearer to build a house on the plot of land " +
                                         "located at {0} on the {1} facet.<br><br>" +
                                         "The deed will expire after {2} more day{3} have passed, and at that time the right to place " +
                                         "a house reverts to normal house construction rules.<br><br>" +
                                         "This deed functions as a recall rune marked for the location of the plot it represents.<br><br>" +
                                         "To place a house on the deeded plot, you must simply have this deed in your backpack " +
                                         "or bank box when using a House Placement Tool there." +
                                         "</bodytextblack>",
                        HouseRaffleStone.FormatLocation(deed.PlotLocation, deed.PlotFacet, false),
                        deed.PlotFacet,
                        daysLeft,
                        (daysLeft == 1) ? "" : "s");
                }
            }
        }
    }
}