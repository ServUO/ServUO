using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class BedrollSpawner : Item
    {
        public static Point3D[] m_RoomDestinations = new Point3D[]
        {
            new Point3D(5651, 555, 20),
            new Point3D(5658, 562, 20),
            new Point3D(5666, 558, 20),
            new Point3D(5793, 594, 10),
            new Point3D(5790, 587, 10),
            new Point3D(5789, 579, 10),
            new Point3D(5790, 571, 10),
            new Point3D(5793, 563, 10),
            new Point3D(5792, 555, 10),
            new Point3D(5863, 594, 15),
            new Point3D(5864, 585, 15),
            new Point3D(5864, 578, 15),
            new Point3D(5863, 570, 15),
            new Point3D(5865, 562, 15),
            new Point3D(5868, 554 ,15)
        };

        public static Point3D[] m_OutsideTunnels = new Point3D[]
        {
            new Point3D(5670, 550, 22),
            new Point3D(5721, 550, 20),
            new Point3D(5670, 535, 0),
            new Point3D(5864, 532, 15),
            new Point3D(5782, 536, 10)
        };

        private static readonly BedrollEntry[] m_Entries = new BedrollEntry[]
        {
            // Upper Floor Room 1
            new BedrollEntry(new Point3D(5653, 565, 20), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5651, 564, 20), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5653, 561, 20), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5650, 560, 20), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5651, 555, 20), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5651, 557, 20), typeof(WrongBedrollEast)),

            // Upper Floor Room 2
            new BedrollEntry(new Point3D(5657, 554, 20), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5659, 555, 20), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5661, 558, 20), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5657, 559, 20), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5657, 561, 20), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5660, 561, 20), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5658, 563, 20), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5657, 565, 20), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5661, 563, 20), typeof(WrongBedrollEast)),
                     
            // Upper Floor Room 3
            new BedrollEntry(new Point3D(5666, 554, 20), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5665, 557, 20), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5668, 558, 20), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5665, 560, 20), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5668, 563, 20), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5665, 565, 20), typeof(WrongBedrollEast)),

            // Left Room 1
            new BedrollEntry(new Point3D(5787, 594, 10), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5789, 596, 10), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5792, 596, 10), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5792, 593, 10), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5795, 596, 10), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5795, 593, 10), typeof(WrongBedrollEast)),

            // Left Room 2
            new BedrollEntry(new Point3D(5787, 588, 10), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5790, 589, 10), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5791, 585, 10), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5793, 589, 10), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5795, 585, 10), typeof(WrongBedrollEast)),

            // Left Room 3
            new BedrollEntry(new Point3D(5787, 581, 10), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5791, 577, 10), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5793, 581, 10), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5795, 579, 10), typeof(WrongBedrollSouth)),

            // Left Room 4
            new BedrollEntry(new Point3D(5787, 570, 10), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5789, 573, 10), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5792, 573, 10), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5792, 570, 10), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5795, 570, 10), typeof(WrongBedrollSouth)),

            // Left Room 5
            new BedrollEntry(new Point3D(5787, 561, 10), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5788, 564, 10), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5790, 562, 10), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5791, 565, 10), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5795, 565, 10), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5795, 561, 10), typeof(WrongBedrollEast)),

            // Left Room 6
            new BedrollEntry(new Point3D(5789, 553, 10), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5790, 555, 10), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5791, 557, 10), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5794, 557, 10), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5795, 554, 10), typeof(WrongBedrollSouth)),

            // Rigth Room 1
            new BedrollEntry(new Point3D(5861, 596, 15), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5862, 593, 15), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5865, 596, 15), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5866, 594, 15), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5867, 592, 15), typeof(WrongBedrollEast)),

            // Rigth Room 2
            new BedrollEntry(new Point3D(5860, 585, 15), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5861, 588, 15), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5864, 588, 15), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5866, 585, 15), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5867, 588, 15), typeof(WrongBedrollSouth)),

            // Rigth Room 3
            new BedrollEntry(new Point3D(5860, 577, 15), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5861, 580, 15), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5863, 580, 15), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5864, 576, 15), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5866, 579, 15), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5867, 576, 15), typeof(WrongBedrollEast)),

            // Rigth Room 4
            new BedrollEntry(new Point3D(5860, 568, 15), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5861, 572, 15), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5863, 569, 15), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5865, 571, 15), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5867, 568, 15), typeof(WrongBedrollSouth)),

            // Rigth Room 5
            new BedrollEntry(new Point3D(5860, 562, 15), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5861, 560, 15), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5862, 564, 15), typeof(WrongBedrollSouth)),
            new BedrollEntry(new Point3D(5864, 563, 15), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5864, 561, 15), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5866, 564, 15), typeof(WrongBedrollEast)),
            new BedrollEntry(new Point3D(5867, 561, 15), typeof(WrongBedrollEast)),
        };

        public readonly TimeSpan RestartDelay = TimeSpan.FromHours(2.0);
        private Timer m_Timer;
        private List<WrongBedrollBase> Bedrolls { get; set; }
        public List<MysteriousTunnel> MysteriousTunnels { get; set; }

        public static List<BedrollSpawner> Instances { get; set; }

        public BedrollSpawner()
            : base(3796)
        {
            Movable = false;
            Visible = false;
            Bedrolls = new List<WrongBedrollBase>();
            MysteriousTunnels = new List<MysteriousTunnel>();
            Timer.DelayCall(TimeSpan.FromSeconds(10), CheckRespawn);
            m_Timer = Timer.DelayCall(RestartDelay, RestartDelay, CheckRespawn);
            m_Timer.Start();

            if (Instances == null)
                Instances = new List<BedrollSpawner>();

            Instances.Add(this);
        }

        private void CheckRespawn()
        {
            Cleanup();

            // Bedrolls Spawn

            foreach (BedrollEntry entry in m_Entries)
            {
                WrongBedrollBase item = (WrongBedrollBase)Activator.CreateInstance(entry.Type);

                item.Movable = false;
                item.MoveToWorld(entry.Location, Map);
                Bedrolls.Add(item);
            }

            // Mysterious Tunnels Spawn   

            MysteriousTunnel mt;
            WrongBedrollBase bedroll;
            int mtrandom;

            for (int i = 0; i < m_OutsideTunnels.Length; i++)
            {
                mt = new MysteriousTunnel();

                if (i < 3)
                {
                    mtrandom = Utility.Random(m_Entries.Length);
                    mt.PointDest = Bedrolls[mtrandom].Location;
                    Bedrolls[mtrandom].PointDest = m_OutsideTunnels[i];
                    Bedrolls[mtrandom].BedrollSpawner = this;
                }
                else
                {
                    mt.PointDest = m_RoomDestinations[Utility.Random(m_RoomDestinations.Length)];
                    bedroll = Bedrolls.Where(x => x.InRange(mt.PointDest, 4) && x.PointDest == Point3D.Zero).FirstOrDefault();

                    if (bedroll != null)
                    {
                        bedroll.PointDest = m_OutsideTunnels[i];
                        bedroll.BedrollSpawner = this;
                    }
                }

                mt.MoveToWorld(m_OutsideTunnels[i], Map);
                MysteriousTunnels.Add(mt);
            }
        }

        public BedrollSpawner(Serial serial)
            : base(serial)
        {
        }

        public void Cleanup()
        {
            if (Bedrolls != null)
            {
                Bedrolls.ForEach(f => f.Delete());
                Bedrolls.Clear();
            }

            if (MysteriousTunnels != null)
            {
                MysteriousTunnels.ForEach(f => f.Delete());
                MysteriousTunnels.Clear();
            }
        }

        public override void OnDelete()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            Cleanup();

            base.OnDelete();
        }

        public override string DefaultName => "Wrong Bedrolls Spawner " + Map;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            if (m_Timer != null)
                writer.Write(m_Timer.Next);
            else
                writer.Write(DateTime.UtcNow + RestartDelay);

            writer.Write(Bedrolls == null ? 0 : Bedrolls.Count);

            if (Bedrolls != null)
            {
                Bedrolls.ForEach(x => writer.Write(x));
            }

            writer.Write(MysteriousTunnels == null ? 0 : MysteriousTunnels.Count);

            if (MysteriousTunnels != null)
            {
                MysteriousTunnels.ForEach(y => writer.Write(y));
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (Instances == null)
                Instances = new List<BedrollSpawner>();

            Instances.Add(this);

            DateTime next = reader.ReadDateTime();

            if (next < DateTime.UtcNow)
                next = DateTime.UtcNow;

            m_Timer = Timer.DelayCall(next - DateTime.UtcNow, RestartDelay, CheckRespawn);
            m_Timer.Start();

            Bedrolls = new List<WrongBedrollBase>();
            MysteriousTunnels = new List<MysteriousTunnel>();

            int bedrollcount = reader.ReadInt();
            for (int x = 0; x < bedrollcount; x++)
            {
                WrongBedrollBase wb = reader.ReadItem() as WrongBedrollBase;

                if (wb != null)
                    Bedrolls.Add(wb);
            }

            int mysteriouscount = reader.ReadInt();
            for (int y = 0; y < mysteriouscount; y++)
            {
                MysteriousTunnel mt = reader.ReadItem() as MysteriousTunnel;

                if (mt != null)
                    MysteriousTunnels.Add(mt);
            }

            if (version == 0)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(5), map =>
                    {
                        EnchantedHotItem.SpawnChests(map);
                        Console.WriteLine("Hot Item chests spawned for {0}.", Map);
                    }, Map);
            }
        }

        public class BedrollEntry
        {
            private readonly Point3D m_Location;
            private readonly Type m_Type;

            public BedrollEntry(Point3D location, Type type)
            {
                m_Location = location;
                m_Type = type;
            }

            public Point3D Location => m_Location;
            public Type Type => m_Type;
        }
    }
}
