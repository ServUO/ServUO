using Server.Commands;
using Server.Mobiles;
using System;

namespace Server.Items
{
    public class ArisenController : Item
    {
        private static readonly int SpawnHour = 0; // Midnight
        private static readonly int DespawnHour = 4; // 4 AM

        public class ArisenEntry
        {
            private readonly Map m_Map;
            private Point3D m_Location;
            private readonly string m_Creature;
            private readonly int m_Amount;
            private readonly int m_HomeRange;
            private readonly int m_SpawnRange;
            private TimeSpan m_MinDelay;
            private TimeSpan m_MaxDelay;

            public Map Map => m_Map;
            public Point3D Location => m_Location;
            public string Creature => m_Creature;
            public int Amount => m_Amount;
            public int HomeRange => m_HomeRange;
            public int SpawnRange => m_SpawnRange;
            public TimeSpan MinDelay => m_MinDelay;
            public TimeSpan MaxDelay => m_MaxDelay;

            public ArisenEntry(Map map, Point3D location, string creature, int amount, int homeRange, int spawnRange, TimeSpan minDelay, TimeSpan maxDelay)
            {
                m_Map = map;
                m_Location = location;
                m_Creature = creature;
                m_Amount = amount;
                m_HomeRange = homeRange;
                m_SpawnRange = spawnRange;
                m_MinDelay = minDelay;
                m_MaxDelay = maxDelay;
            }

            public XmlSpawner CreateSpawner()
            {
                XmlSpawner spawner = new XmlSpawner(m_Amount, (int)m_MinDelay.TotalSeconds, (int)m_MaxDelay.TotalSeconds, 0, 20, 10, m_Creature);

                spawner.MoveToWorld(Location, Map);

                return spawner;
            }
        }

        private static readonly ArisenEntry[] m_Entries = new ArisenEntry[]
            {
                new ArisenEntry( Map.TerMur, new Point3D( 996, 3862, -42 ), "EffeteUndeadGargoyle", 5, 20, 15, TimeSpan.FromSeconds( 15.0 ), TimeSpan.FromSeconds( 30.0 ) ),
                new ArisenEntry( Map.TerMur, new Point3D( 996, 3863, -42 ), "EffetePutridGargoyle", 5, 20, 15, TimeSpan.FromSeconds( 30.0 ), TimeSpan.FromSeconds( 60.0 ) ),
                new ArisenEntry( Map.TerMur, new Point3D( 996, 3864, -42 ), "GargoyleShade",        2, 15, 10, TimeSpan.FromSeconds( 60.0 ), TimeSpan.FromSeconds( 90.0 ) ),

                new ArisenEntry( Map.TerMur, new Point3D( 996, 3892, -42 ), "EffeteUndeadGargoyle", 5, 20, 15, TimeSpan.FromSeconds( 15.0 ), TimeSpan.FromSeconds( 30.0 ) ),
                new ArisenEntry( Map.TerMur, new Point3D( 996, 3893, -42 ), "EffetePutridGargoyle", 5, 20, 15, TimeSpan.FromSeconds( 30.0 ), TimeSpan.FromSeconds( 60.0 ) ),

                new ArisenEntry( Map.TerMur, new Point3D( 996, 3917, -42 ), "EffeteUndeadGargoyle", 5, 20, 15, TimeSpan.FromSeconds( 15.0 ), TimeSpan.FromSeconds( 30.0 ) ),
                new ArisenEntry( Map.TerMur, new Point3D( 996, 3918, -42 ), "EffetePutridGargoyle", 5, 20, 15, TimeSpan.FromSeconds( 30.0 ), TimeSpan.FromSeconds( 60.0 ) ),

                new ArisenEntry( Map.TerMur, new Point3D( 996, 3951, -42 ), "EffeteUndeadGargoyle", 5, 20, 15, TimeSpan.FromSeconds( 15.0 ), TimeSpan.FromSeconds( 30.0 ) ),
                new ArisenEntry( Map.TerMur, new Point3D( 996, 3952, -42 ), "EffetePutridGargoyle", 5, 20, 15, TimeSpan.FromSeconds( 30.0 ), TimeSpan.FromSeconds( 60.0 ) ),
                new ArisenEntry( Map.TerMur, new Point3D( 997, 3951, -42 ), "GargoyleShade",        2, 15, 10, TimeSpan.FromSeconds( 60.0 ), TimeSpan.FromSeconds( 90.0 ) ),
                new ArisenEntry( Map.TerMur, new Point3D( 997, 3951, -42 ), "PutridUndeadGargoyle", 1, 10,  5, TimeSpan.FromMinutes( 5.0 ),  TimeSpan.FromMinutes( 10.0 ) )
            };

        public static ArisenEntry[] Entries => m_Entries;

        private static ArisenController m_Instance;

        public static ArisenController Instance => m_Instance;

        public static void Initialize()
        {
            CommandSystem.Register("ArisenGenerate", AccessLevel.Owner, ArisenGenerate_OnCommand);
            CommandSystem.Register("ArisenDelete", AccessLevel.Owner, ArisenDelete_OnCommand);
        }

        [Usage("ArisenGenerate")]
        [Description("Generates the Arisen creatures spawner.")]
        private static void ArisenGenerate_OnCommand(CommandEventArgs args)
        {
            Mobile from = args.Mobile;

            if (Create())
                from.SendMessage("Arisen creatures spawner generated.");
            else
                from.SendMessage("Arisen creatures spawner already present.");
        }

        [Usage("ArisenDelete")]
        [Description("Removes the Arisen creatures spawner.")]
        private static void ArisenDelete_OnCommand(CommandEventArgs args)
        {
            Mobile from = args.Mobile;

            if (Remove())
                from.SendMessage("Arisen creatures spawner removed.");
            else
                from.SendMessage("Arisen creatures spawner not present.");
        }

        public static bool Create()
        {
            if (m_Instance != null && !m_Instance.Deleted)
                return false;

            m_Instance = new ArisenController();
            WeakEntityCollection.Add("sa", m_Instance);
            return true;
        }

        public static bool Remove()
        {
            if (m_Instance == null)
                return false;

            m_Instance.Delete();
            m_Instance = null;

            return true;
        }

        private InternalTimer m_SpawnTimer;
        private XmlSpawner[] m_Spawners;
        private bool m_Spawned;
        private bool m_ForceDeactivate;

        [CommandProperty(AccessLevel.Seer)]
        public bool ForceDeactivate
        {
            get { return m_ForceDeactivate; }
            set { m_ForceDeactivate = value; }
        }

        private ArisenController()
            : base(1)
        {
            Name = "Arisen Controller - Internal";
            Movable = false;

            m_Spawners = new XmlSpawner[m_Entries.Length];

            for (int i = 0; i < m_Entries.Length; i++)
            {
                m_Spawners[i] = m_Entries[i].CreateSpawner();
                m_Spawners[i].SmartSpawning = true;
            }

            m_SpawnTimer = new InternalTimer(this);
            m_SpawnTimer.Start();
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if (m_SpawnTimer != null)
            {
                m_SpawnTimer.Stop();
                m_SpawnTimer = null;
            }

            foreach (XmlSpawner spawner in m_Spawners)
                spawner.Delete();

            m_Instance = null;
        }

        public void OnTick()
        {
            // check time
            int hours, minutes;

            Clock.GetTime(Map.TerMur, 997, 3869, out hours, out minutes); // Holy City

            m_Spawned = (hours >= SpawnHour && hours < DespawnHour) && !m_ForceDeactivate;

            foreach (XmlSpawner spawner in m_Spawners)
            {
                if (!m_Spawned)
                {
                    spawner.Reset();
                }
                else
                {
                    if (!spawner.Running)
                    {
                        spawner.Respawn();
                    }
                }
            }
        }

        public class InternalTimer : Timer
        {
            private readonly ArisenController m_Controller;

            public InternalTimer(ArisenController controller)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(5.0))
            {

                m_Controller = controller;
            }

            protected override void OnTick()
            {
                m_Controller.OnTick();
            }
        }

        public ArisenController(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version

            // Version 1
            writer.Write(m_ForceDeactivate);

            // Version 0
            writer.WriteEncodedInt(m_Spawners.Length);

            for (int i = 0; i < m_Spawners.Length; i++)
                writer.WriteItem(m_Spawners[i]);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 1:
                    {
                        m_ForceDeactivate = reader.ReadBool();

                        goto case 0;
                    }
                case 0:
                    {
                        int length = reader.ReadEncodedInt();

                        m_Spawners = new XmlSpawner[length];

                        for (int i = 0; i < length; i++)
                        {
                            XmlSpawner spawner = reader.ReadItem<XmlSpawner>();

                            if (spawner == null)
                            {
                                spawner = m_Entries[i].CreateSpawner();
                                spawner.SmartSpawning = true;
                            }

                            m_Spawners[i] = spawner;
                        }

                        break;
                    }
            }

            m_Instance = this;

            m_SpawnTimer = new InternalTimer(this);
            m_SpawnTimer.Start();
        }
    }
}
