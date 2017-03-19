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
                return this.m_Active;
            }
            set
            {
                if (value)
                    this.Start();
                else
                    this.Stop();

                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime RestartTime
        {
            get
            {
                return this.m_RestartTime;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Parts Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
                this.InvalidateProperties();
            }
        }

        [Constructable]
        public StorageLocker(Parts type)
            : base(0x285E)
        {
            this.m_Barrels = new List<Item>();

            this.Locked = true;
            this.Hue = 2301;
            this.Movable = false;
            this.m_Type = type;
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
            if (this.m_Active || this.Deleted)
                return;

            this.m_Active = true;

            if (this.m_RestartTimer != null)
                this.m_RestartTimer.Stop();

            this.m_RestartTimer = null;

            int index = Utility.Random(0, 8);
            int randomkey = Utility.Random(-4, 4);
            bool loot = false;
            Item barrel = null;

            for (int k = 0; k < 8; k++)
            {
                int itemx = this.Location.X + WoodenToMetalBarrelCoordinate[k][0];
                int itemy = this.Location.Y + WoodenToMetalBarrelCoordinate[k][1];
                int z = Map.GetAverageZ(itemx, itemy);

                if (index == k)
                {
                    barrel = new WoodenKeyBarrel(Parts.None);
                    this.m_Barrels.Add(barrel);
                }
                else
                {
                    barrel = new WoodenToMetalBarrel(this);
                    this.m_Barrels.Add(barrel);
                }

                barrel.MoveToWorld(new Point3D(itemx, itemy, z), this.Map);
            }

            for (int x = -4; x < 5; x++)
            {
                for (int y = 4; y > -5; y--)
                {
                    if ((x >= -1 && x <= 1) && (y >= -1 && y <= 1))
                        continue;

                    int itemx = this.Location.X + x;
                    int itemy = this.Location.Y + y;
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

                    this.m_Barrels.Add(barrel);

                    barrel.MoveToWorld(new Point3D(itemx, itemy, z), this.Map);
                }
            }
        }

        public void Stop()
        {
            if (!this.m_Active || this.Deleted)
                return;

            this.m_Active = false;

            if (this.m_RestartTimer != null)
                this.m_RestartTimer.Stop();

            this.m_RestartTimer = null;

            if (this.m_Barrels != null)
            {
                for (int i = 0; i < this.m_Barrels.Count; ++i)
                {
                    if (this.m_Barrels[i] != null)
                        this.m_Barrels[i].Delete();
                }

                this.m_Barrels.Clear();
            }

            for (int i = this.Items.Count - 1; i >= 0; --i)
            {
                if (i < this.Items.Count)
                    this.Items[i].Delete();
            }
        }

        public void BeginRestart(TimeSpan ts)
        {
            if (this.m_RestartTimer != null)
                this.m_RestartTimer.Stop();

            this.m_RestartTime = DateTime.UtcNow + ts;

            this.m_RestartTimer = new RestartTimer(this, ts);
            this.m_RestartTimer.Start();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            Stop();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((bool)this.m_Active);
            writer.Write((int)this.m_Type);
            writer.Write(this.m_Barrels, true);

            writer.Write(this.m_RestartTimer != null);

            if (this.m_RestartTimer != null)
                writer.WriteDeltaTime(this.m_RestartTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            this.m_Active = reader.ReadBool();
            this.m_Type = (Parts)reader.ReadInt();
            this.m_Barrels = reader.ReadStrongItemList();

            if (reader.ReadBool())
            {
                this.m_RestartTime = reader.ReadDeltaTime();
            }

            this.BeginRestart(TimeSpan.FromSeconds(10.0));
        }
    }

    public class RestartTimer : Timer
    {
        private readonly StorageLocker m_Storage;
        public RestartTimer(StorageLocker storage, TimeSpan delay)
            : base(delay)
        {
            this.m_Storage = storage;
            this.Priority = TimerPriority.FiveSeconds;
        }

        protected override void OnTick()
        {
            this.m_Storage.Stop();
            this.m_Storage.Start();
        }
    }
}