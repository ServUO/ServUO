using Server.Commands;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public enum Parts
    {     
        None = -1,   
        Flywheel,
        WireSpool,
        PowerCore,
        BearingAssembly,
    };    

    [Furniture]
    [Flipable(0x285D, 0x285E)]
    public class StorageLocker : FillableContainer
    {
        public override int LabelNumber { get { return 1154431; } } // Storage Locker

        private bool m_Active;
        private Parts m_Type;
        private List<Item> m_Barrels;
        private Timer m_RestartTimer;
        private DateTime m_RestartTime;
		
		public List<Item> Barrels
        {
            get { return m_Barrels; }
            set { m_Barrels = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get
            {
                return m_Active;
            }
            set
            {
                if (value)
                    Start();
                else
                    Stop();

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime RestartTime
        {
            get
            {
                return m_RestartTime;
            }
        }

        public override bool IsDecoContainer { get { return false; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Parts Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
                InvalidateProperties();
            }
        }

        [Constructable]
        public StorageLocker(Parts type)
            : base(0x285E)
        {
            m_Barrels = new List<Item>();

            Locked = true;
            Hue = 2301;
            Movable = false;
            m_Type = type;
        }

        public StorageLocker(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            
            list.Add(1154425, String.Format("#{0}", 1154427 + (int)m_Type)); // *You barely make out some words on a rusted nameplate*<BR>REPLACEMENT PARTS: ~1_PART~
        }

        public static int[][] WoodenToMetalBarrelCoordinate =
        {
            new int[] { 0, 1 },
            new int[] { 1, 1 },
            new int[] { 1, 0 },
            new int[] { 1, -1 },
            new int[] { 0, -1 },
            new int[] { -1, -1 },
            new int[] { -1, 0 },
            new int[] { -1, 1 }
        };

        private Parts key;

        public void Start()
        {
            if (m_Active || Deleted)
                return;

            m_Active = true;

            if (m_RestartTimer != null)
                m_RestartTimer.Stop();

            m_RestartTimer = null;

            int index = Utility.Random(0, 8);
            int randomkey = Utility.Random(-4, 4);            
            bool loot = false;
            Item barrel = null;

            for (int k = 0; k < 8; k++)
            {
                int itemx = Location.X + WoodenToMetalBarrelCoordinate[k][0];
                int itemy = Location.Y + WoodenToMetalBarrelCoordinate[k][1];
                int z = Map.GetAverageZ(itemx, itemy);

                if (index == k)
                {
                    barrel = new WoodenKeyBarrel(Parts.None);
                    m_Barrels.Add(barrel);
                }
                else
                {
                    barrel = new WoodenToMetalBarrel(this);
                    m_Barrels.Add(barrel);
                }

                barrel.MoveToWorld(new Point3D(itemx, itemy, z), Map);
            }

            for (int x = -4; x < 5; x++)
            {
                for (int y = 4; y > -5; y--)
                {
                    if ((x >= -1 && x <= 1) && (y >= -1 && y <= 1))
                        continue;

                    int itemx = Location.X + x;
                    int itemy = Location.Y + y;
                    int z = Map.GetAverageZ(itemx, itemy);

                    if (!loot)
                    {
                        if (x == randomkey)
                        {
                            key = m_Type;
                            loot = true;

                            barrel = new WoodenKeyBarrel(key);
                            ((WoodenKeyBarrel)barrel).StorageLocker = this;
                        }
                        else
                        {
                            key = Parts.None;
                            barrel = new WoodenKeyBarrel(key);
                        }
                    }
                    else
                    {
                        key = Parts.None;
                        barrel = new WoodenKeyBarrel(key);
                    }

                    m_Barrels.Add(barrel);                 

                    barrel.MoveToWorld(new Point3D(itemx, itemy, z), Map);
                }
            }            
        }

        public void Stop()
        {
            if (!m_Active || Deleted)
                return;

            m_Active = false;

            if (m_RestartTimer != null)
                m_RestartTimer.Stop();

            m_RestartTimer = null;

            if (m_Barrels != null)
            {                
                for (int i = 0; i < m_Barrels.Count; ++i)
                {
                    if (m_Barrels[i] != null)
                        m_Barrels[i].Delete();
                }

                m_Barrels.Clear();
            }
            
            for (int i = Items.Count - 1; i >= 0; --i)
            {
                if (i < Items.Count)
                    Items[i].Delete();
            }
        }

        public void BeginRestart(TimeSpan ts)
        {
            if (m_RestartTimer != null)
                m_RestartTimer.Stop();

            m_RestartTime = DateTime.UtcNow + ts;

            m_RestartTimer = new RestartTimer(this, ts);
            m_RestartTimer.Start();
        }

        public override void OnDelete()
        {
            Stop();

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((bool)m_Active);
            writer.Write((int)m_Type);            
            writer.Write(m_Barrels, true);

            writer.Write(m_RestartTimer != null);

            if (m_RestartTimer != null)
                writer.WriteDeltaTime(m_RestartTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Active = reader.ReadBool();
            m_Type = (Parts)reader.ReadInt();
            m_Barrels = reader.ReadStrongItemList();

            if (reader.ReadBool())
            {
                m_RestartTime = reader.ReadDeltaTime();
            }
			
			BeginRestart(TimeSpan.FromSeconds(10.0));
        }
    }

    public class RestartTimer : Timer
    {
        private readonly StorageLocker m_Storage;
        public RestartTimer(StorageLocker storage, TimeSpan delay)
            : base(delay)
        {
            m_Storage = storage;
            Priority = TimerPriority.FiveSeconds;
        }

        protected override void OnTick()
        {
            m_Storage.Stop();
            m_Storage.Start();
        }
    }
}
