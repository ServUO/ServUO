using System;
using Server.Gumps;

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
            this.m_Stone = stone;

            if (stone != null)
            {
                this.m_PlotLocation = stone.GetPlotCenter();
                this.m_Facet = stone.PlotFacet;
            }

            this.m_AwardedTo = m;

            this.LootType = LootType.Blessed;
            this.Hue = 0x501;
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
                return this.m_Stone;
            }
            set
            {
                this.m_Stone = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public Point3D PlotLocation
        {
            get
            {
                return this.m_PlotLocation;
            }
            set
            {
                this.m_PlotLocation = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public Map PlotFacet
        {
            get
            {
                return this.m_Facet;
            }
            set
            {
                this.m_Facet = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public Mobile AwardedTo
        {
            get
            {
                return this.m_AwardedTo;
            }
            set
            {
                this.m_AwardedTo = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public bool IsExpired
        {
            get
            {
                return (this.m_Stone == null || this.m_Stone.Deleted || this.m_Stone.IsExpired);
            }
        }
        public override string DefaultName
        {
            get
            {
                return "a writ of lease";
            }
        }
        public override double DefaultWeight
        {
            get
            {
                return 1.0;
            }
        }
        public bool ValidLocation()
        {
            return (this.m_PlotLocation != Point3D.Zero && this.m_Facet != null && this.m_Facet != Map.Internal);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.ValidLocation())
            {
                list.Add(1060658, "location\t{0}", HouseRaffleStone.FormatLocation(this.m_PlotLocation, this.m_Facet, false)); // ~1_val~: ~2_val~
                list.Add(1060659, "facet\t{0}", this.m_Facet); // ~1_val~: ~2_val~
                list.Add(1150486); // [Marked Item]
            }

            if (this.IsExpired)
                list.Add(1150487); // [Expired]
            //list.Add( 1060660, "shard\t{0}", ServerList.ServerName ); // ~1_val~: ~2_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.ValidLocation())
                return;

            if (this.IsChildOf(from.Backpack))
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

            writer.Write((int)1); // version

            writer.Write(this.m_Stone);
            writer.Write(this.m_PlotLocation);
            writer.Write(this.m_Facet);
            writer.Write(this.m_AwardedTo);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Stone = reader.ReadItem<HouseRaffleStone>();

                        goto case 0;
                    }
                case 0:
                    {
                        this.m_PlotLocation = reader.ReadPoint3D();
                        this.m_Facet = reader.ReadMap();
                        this.m_AwardedTo = reader.ReadMobile();

                        break;
                    }
            }
        }

        private class WritOfLeaseGump : Gump
        {
            public WritOfLeaseGump(HouseRaffleDeed deed)
                : base(150, 50)
            {
                this.AddPage(0);

                this.AddImage(0, 0, 9380);
                this.AddImage(114, 0, 9381);
                this.AddImage(171, 0, 9382);
                this.AddImage(0, 140, 9383);
                this.AddImage(114, 140, 9384);
                this.AddImage(171, 140, 9385);
                this.AddImage(0, 182, 9383);
                this.AddImage(114, 182, 9384);
                this.AddImage(171, 182, 9385);
                this.AddImage(0, 224, 9383);
                this.AddImage(114, 224, 9384);
                this.AddImage(171, 224, 9385);
                this.AddImage(0, 266, 9386);
                this.AddImage(114, 266, 9387);
                this.AddImage(171, 266, 9388);

                this.AddHtmlLocalized(30, 48, 229, 20, 1150484, 200, false, false); // WRIT OF LEASE
                this.AddHtml(28, 75, 231, 280, FormatDescription(deed), false, true);
            }

            private static string FormatDescription(HouseRaffleDeed deed)
            {
                if (deed == null)
                    return String.Empty;

                if (deed.IsExpired)
                {
                    return String.Format(
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

                    return String.Format(
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