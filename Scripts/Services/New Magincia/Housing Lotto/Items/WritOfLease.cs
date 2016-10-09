using System;
using Server;
using Server.Items;
using Server.Gumps;

namespace Server.Engines.NewMagincia
{
    public class WritOfLease : Item
    {
        public override int LabelNumber { get { return 1150489; } } // a writ of lease

        private MaginciaHousingPlot m_Plot;
        private DateTime m_Expires;
        private bool m_Expired;
        private Map m_Facet;
        private string m_Identifier;
        private Point3D m_RecallLoc;

        [CommandProperty(AccessLevel.GameMaster)]
        public MaginciaHousingPlot Plot { get { return m_Plot; } set { m_Plot = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Expires { get { return m_Expires; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Expired { get { return m_Expired; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map Facet { get { return m_Facet; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Identifier { get { return m_Identifier; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D RecallLoc { get { return m_RecallLoc; } }

        public WritOfLease(MaginciaHousingPlot plot)
            : base(5358)
        {
            Hue = 0x9A;
            m_Plot = plot;
            m_Expires = plot.Expires;
            m_Expired = false;

            if (plot != null)
            {
                m_Facet = plot.Map;
                m_Identifier = plot.Identifier;
                m_RecallLoc = plot.RecallLoc;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1150547, m_Identifier != null ? m_Identifier : "Unkonwn"); // Lot: ~1_LOTNAME~

            list.Add(m_Facet == Map.Trammel ? 1150549 : 1150548); // Facet: Felucca

            list.Add(1150546, Server.Misc.ServerList.ServerName); // Shard: ~1_SHARDNAME~

            if (m_Expired)
                list.Add(1150487); // [Expired]
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 2))
            {
                from.CloseGump(typeof(WritNoteGump));
                from.SendGump(new WritNoteGump(this));
            }
        }

        public void CheckExpired()
        {
            if (DateTime.UtcNow > m_Expires)
                OnExpired();
        }

        public void OnExpired()
        {
            MaginciaLottoSystem.UnregisterPlot(m_Plot);
            m_Plot = null;
            m_Expired = true;
            m_Expires = DateTime.MinValue;
            InvalidateProperties();
        }

        public override void Delete()
        {
            if (!m_Expired)
                OnExpired();

            base.Delete();
        }

        public WritOfLease(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(m_Expired);
            writer.Write(m_Expires);
            writer.Write(m_Facet);
            writer.Write(m_Identifier);
            writer.Write(m_RecallLoc);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Expired = reader.ReadBool();
            m_Expires = reader.ReadDateTime();
            m_Facet = reader.ReadMap();
            m_Identifier = reader.ReadString();
            m_RecallLoc = reader.ReadPoint3D();
        }

        private class WritNoteGump : Gump
        {
            private WritOfLease m_Lease;

            public WritNoteGump(WritOfLease lease)
                : base(100, 100)
            {
                m_Lease = lease;

                AddImage(0, 0, 9380);
                AddImage(114, 0, 9381);
                AddImage(171, 0, 9382);
                AddImage(0, 140, 9386);
                AddImage(114, 140, 9387);
                AddImage(171, 140, 9388);

                AddHtmlLocalized(90, 5, 200, 16, 1150484, 1, false, false); // WRIT OF LEASE

                string args;

                if (lease.Expired)
                {
                    args = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", lease.Identifier, lease.Facet.ToString(), "Alexandria", "", String.Format("{0} {1}", lease.RecallLoc.X, lease.RecallLoc.Y));
                    AddHtmlLocalized(38, 55, 215, 178, 1150488, args, 1, false, true);
                    //This deed once entitled the bearer to build a house on the plot of land designated "~1_PLOT~" (located at ~5_SEXTANT~) in the City of New Magincia on the ~2_FACET~ facet of the ~3_SHARD~ shard.<br><br>The deed has expired, and now the indicated plot of land is subject to normal house construction rules.<br><br>This deed was won by lottery, and while it is no longer valid for land ownership it does serve to commemorate the winning of land during the Rebuilding of Magincia.<br><br>This deed functions as a recall rune marked for the location of the plot it represents.
                }
                else
                {
                    args = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", lease.Identifier, lease.Facet.ToString(), "Alexandria", MaginciaLottoSystem.WritExpirePeriod.ToString(), String.Format("{0} {1}", lease.RecallLoc.X, lease.RecallLoc.Y));
                    AddHtmlLocalized(38, 55, 215, 178, 1150483, args, 1, false, true);
                    //This deed entitles the bearer to build a house on the plot of land designated "~1_PLOT~" (located at ~5_SEXTANT~) in the City of New Magincia on the ~2_FACET~ facet of the ~3_SHARD~ shard.<br><br>The deed will expire once it is used to construct a house, and thereafter the indicated plot of land will be subject to normal house construction rules. The deed will expire after ~4_DAYS~ more days have passed, and at that time the right to place a house reverts to normal house construction rules.<br><br>This deed functions as a recall rune marked for the location of the plot it represents.<br><br>To place a house on the deeded plot, you must simply have this deed in your backpack or bank box when using a House Placement Tool there.
                }
            }
        }
    }
}