using System;
using Server.Commands;

namespace Server.Items
{
    public class SHTeleComponent : AddonComponent
    {
        private bool m_Active;
        private SHTeleComponent m_TeleDest;
        private Point3D m_TeleOffset;
        [Constructable]
        public SHTeleComponent()
            : this(0x1775)
        {
        }

        [Constructable]
        public SHTeleComponent(int itemID)
            : this(itemID, new Point3D(0, 0, 0))
        {
        }

        [Constructable]
        public SHTeleComponent(int itemID, Point3D offset)
            : base(itemID)
        {
            this.Movable = false;
            this.Hue = 1;

            this.m_Active = true;
            this.m_TeleOffset = offset;
        }

        public SHTeleComponent(Serial serial)
            : base(serial)
        {
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
                this.m_Active = value;

                SHTeleporter sourceAddon = this.Addon as SHTeleporter;

                if (sourceAddon != null)
                    sourceAddon.ChangeActive(value);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D TeleOffset
        {
            get
            {
                return this.m_TeleOffset;
            }
            set
            {
                this.m_TeleOffset = value;
            }
        }
        [CommandProperty(AccessLevel.Counselor)]
        public Point3D TelePoint
        {
            get
            {
                return new Point3D(this.Location.X + this.TeleOffset.X, this.Location.Y + this.TeleOffset.Y, this.Location.Z + this.TeleOffset.Z);
            }
            set
            {
                this.m_TeleOffset = new Point3D(value.X - this.Location.X, value.Y - this.Location.Y, value.Z - this.Location.Z);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public SHTeleComponent TeleDest
        {
            get
            {
                return this.m_TeleDest;
            }
            set
            {
                this.m_TeleDest = value;

                SHTeleporter sourceAddon = this.Addon as SHTeleporter;

                if (sourceAddon != null)
                    sourceAddon.ChangeDest(value);
            }
        }
        public override string DefaultName
        {
            get
            {
                return "a hole";
            }
        }
        public override void OnDoubleClick(Mobile m)
        {
            if (!this.m_Active || this.m_TeleDest == null || this.m_TeleDest.Deleted || this.m_TeleDest.Map == Map.Internal)
                return;

            if (Server.Engines.CityLoyalty.CityTradeSystem.HasTrade(m))
            {
                m.SendLocalizedMessage(1151733); // You cannot do that while carrying a Trade Order.
                return;
            }

            if (m.InRange(this, 3))
            {
                Map map = this.m_TeleDest.Map;
                Point3D p = this.m_TeleDest.TelePoint;

                Server.Mobiles.BaseCreature.TeleportPets(m, p, map);

                m.MoveToWorld(p, map);
            }
            else
            {
                m.SendLocalizedMessage(1019045); // I can't reach that.
            }
        }

        public override void OnDoubleClickDead(Mobile m)
        {
            this.OnDoubleClick(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Active);
            writer.Write(this.m_TeleDest);
            writer.Write(this.m_TeleOffset);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Active = reader.ReadBool();
            this.m_TeleDest = reader.ReadItem() as SHTeleComponent;
            this.m_TeleOffset = reader.ReadPoint3D();
        }
    }

    public class SHTeleporter : BaseAddon
    {
        private bool m_External;
        private SHTeleComponent m_UpTele;
        private SHTeleComponent m_RightTele;
        private SHTeleComponent m_DownTele;
        private SHTeleComponent m_LeftTele;
        private bool m_Changing;
        [Constructable]
        public SHTeleporter()
            : this(true)
        {
        }

        [Constructable]
        public SHTeleporter(bool external)
        {
            this.m_Changing = false;
            this.m_External = external;

            if (external)
            {
                this.AddComponent(new AddonComponent(0x549), -1, -1, 0);
                this.AddComponent(new AddonComponent(0x54D), 0, -1, 0);
                this.AddComponent(new AddonComponent(0x54E), 1, -1, 0);
                this.AddComponent(new AddonComponent(0x548), 2, -1, 0);
                this.AddComponent(new AddonComponent(0x54B), -1, 0, 0);
                this.AddComponent(new AddonComponent(0x53B), 0, 0, 0);
                this.AddComponent(new AddonComponent(0x53B), 1, 0, 0);
                this.AddComponent(new AddonComponent(0x544), 2, 0, 0);
                this.AddComponent(new AddonComponent(0x54C), -1, 1, 0);
                this.AddComponent(new AddonComponent(0x53B), 0, 1, 0);
                this.AddComponent(new AddonComponent(0x53B), 1, 1, 0);
                this.AddComponent(new AddonComponent(0x545), 2, 1, 0);
                this.AddComponent(new AddonComponent(0x547), -1, 2, 0);
                this.AddComponent(new AddonComponent(0x541), 0, 2, 0);
                this.AddComponent(new AddonComponent(0x543), 1, 2, 0);
                this.AddComponent(new AddonComponent(0x540), 2, 2, 0);
            }

            Point3D upOS = external ? new Point3D(-1, 0, 0) : new Point3D(-2, -1, 0);
            this.m_UpTele = new SHTeleComponent(external ? 0x1775 : 0x495, upOS);
            this.AddComponent(this.m_UpTele, 0, 0, 0);

            Point3D rightOS = external ? new Point3D(-2, 0, 0) : new Point3D(2, -1, 0);
            this.m_RightTele = new SHTeleComponent(external ? 0x1775 : 0x495, rightOS);
            this.AddComponent(this.m_RightTele, 1, 0, 0);

            Point3D downOS = external ? new Point3D(-2, -1, 0) : new Point3D(2, 2, 0);
            this.m_DownTele = new SHTeleComponent(external ? 0x1776 : 0x495, downOS);
            this.AddComponent(this.m_DownTele, 1, 1, 0);

            Point3D leftOS = external ? new Point3D(-1, -1, 0) : new Point3D(-1, 2, 0);
            this.m_LeftTele = new SHTeleComponent(external ? 0x1775 : 0x495, leftOS);
            this.AddComponent(this.m_LeftTele, 0, 1, 0);
        }

        public SHTeleporter(Serial serial)
            : base(serial)
        {
            this.m_Changing = false;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool External
        {
            get
            {
                return this.m_External;
            }
        }
        public SHTeleComponent UpTele
        {
            get
            {
                return this.m_UpTele;
            }
        }
        public SHTeleComponent RightTele
        {
            get
            {
                return this.m_RightTele;
            }
        }
        public SHTeleComponent DownTele
        {
            get
            {
                return this.m_DownTele;
            }
        }
        public SHTeleComponent LeftTele
        {
            get
            {
                return this.m_LeftTele;
            }
        }
        public override bool ShareHue
        {
            get
            {
                return false;
            }
        }
        public static void Initialize()
        {
            CommandSystem.Register("SHTelGen", AccessLevel.Administrator, new CommandEventHandler(SHTelGen_OnCommand));
        }

        [Usage("SHTelGen")]
        [Description("Generates solen hives teleporters.")]
        public static void SHTelGen_OnCommand(CommandEventArgs e)
        {
            World.Broadcast(0x35, true, "Solen hives teleporters are being generated, please wait.");

            DateTime startTime = DateTime.UtcNow;

            int count = new SHTeleporterCreator().CreateSHTeleporters();

            DateTime endTime = DateTime.UtcNow;

            World.Broadcast(0x35, true, "{0} solen hives teleporters have been created. The entire process took {1:F1} seconds.", count, (endTime - startTime).TotalSeconds);
        }

        public void ChangeActive(bool active)
        {
            if (this.m_Changing)
                return;

            this.m_Changing = true;

            this.m_UpTele.Active = active;
            this.m_RightTele.Active = active;
            this.m_DownTele.Active = active;
            this.m_LeftTele.Active = active;

            this.m_Changing = false;
        }

        public void ChangeDest(SHTeleComponent dest)
        {
            if (this.m_Changing)
                return;

            this.m_Changing = true;

            if (dest == null || !(dest.Addon is SHTeleporter))
            {
                this.m_UpTele.TeleDest = dest;
                this.m_RightTele.TeleDest = dest;
                this.m_DownTele.TeleDest = dest;
                this.m_LeftTele.TeleDest = dest;
            }
            else
            {
                SHTeleporter destAddon = (SHTeleporter)dest.Addon;

                this.m_UpTele.TeleDest = destAddon.UpTele;
                this.m_RightTele.TeleDest = destAddon.RightTele;
                this.m_DownTele.TeleDest = destAddon.DownTele;
                this.m_LeftTele.TeleDest = destAddon.LeftTele;
            }

            this.m_Changing = false;
        }

        public void ChangeDest(SHTeleporter destAddon)
        {
            if (this.m_Changing)
                return;

            this.m_Changing = true;

            if (destAddon != null)
            {
                this.m_UpTele.TeleDest = destAddon.UpTele;
                this.m_RightTele.TeleDest = destAddon.RightTele;
                this.m_DownTele.TeleDest = destAddon.DownTele;
                this.m_LeftTele.TeleDest = destAddon.LeftTele;
            }
            else
            {
                this.m_UpTele.TeleDest = null;
                this.m_RightTele.TeleDest = null;
                this.m_DownTele.TeleDest = null;
                this.m_LeftTele.TeleDest = null;
            }

            this.m_Changing = false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_External);

            writer.Write(this.m_UpTele);
            writer.Write(this.m_RightTele);
            writer.Write(this.m_DownTele);
            writer.Write(this.m_LeftTele);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_External = reader.ReadBool();

            this.m_UpTele = (SHTeleComponent)reader.ReadItem();
            this.m_RightTele = (SHTeleComponent)reader.ReadItem();
            this.m_DownTele = (SHTeleComponent)reader.ReadItem();
            this.m_LeftTele = (SHTeleComponent)reader.ReadItem();
        }

        public class SHTeleporterCreator
        {
            private int m_Count;
            public SHTeleporterCreator()
            {
                this.m_Count = 0;
            }

            public static SHTeleporter FindSHTeleporter(Map map, Point3D p)
            {
                IPooledEnumerable eable = map.GetItemsInRange(p, 0);

                foreach (Item item in eable)
                {
                    if (item is SHTeleporter && item.Z == p.Z)
                    {
                        eable.Free();
                        return (SHTeleporter)item;
                    }
                }

                eable.Free();
                return null;
            }

            public static void Link(SHTeleporter tele1, SHTeleporter tele2)
            {
                tele1.ChangeDest(tele2);
                tele2.ChangeDest(tele1);
            }

            public SHTeleporter AddSHT(Map map, bool ext, int x, int y, int z)
            {
                Point3D p = new Point3D(x, y, z);
                SHTeleporter tele = FindSHTeleporter(map, p);

                if (tele == null)
                {
                    tele = new SHTeleporter(ext);
                    tele.MoveToWorld(p, map);

                    this.m_Count++;
                }

                return tele;
            }

            public void AddSHTCouple(Map map, bool ext1, int x1, int y1, int z1, bool ext2, int x2, int y2, int z2)
            {
                SHTeleporter tele1 = this.AddSHT(map, ext1, x1, y1, z1);
                SHTeleporter tele2 = this.AddSHT(map, ext2, x2, y2, z2);

                Link(tele1, tele2);
            }

            public void AddSHTCouple(bool ext1, int x1, int y1, int z1, bool ext2, int x2, int y2, int z2)
            {
                this.AddSHTCouple(Map.Trammel, ext1, x1, y1, z1, ext2, x2, y2, z2);
                this.AddSHTCouple(Map.Felucca, ext1, x1, y1, z1, ext2, x2, y2, z2);
            }

            public int CreateSHTeleporters()
            {
                SHTeleporter tele1, tele2;

                this.AddSHTCouple(true, 2608, 763, 0, false, 5918, 1794, 0);
                this.AddSHTCouple(false, 5897, 1877, 0, false, 5871, 1867, 0);
                this.AddSHTCouple(false, 5852, 1848, 0, false, 5771, 1867, 0);

                tele1 = this.AddSHT(Map.Trammel, false, 5747, 1895, 0);
                tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = this.AddSHT(Map.Trammel, false, 5658, 1898, 0);
                Link(tele1, tele2);

                tele1 = this.AddSHT(Map.Felucca, false, 5747, 1895, 0);
                tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = this.AddSHT(Map.Felucca, false, 5658, 1898, 0);
                Link(tele1, tele2);

                this.AddSHTCouple(false, 5727, 1894, 0, false, 5756, 1794, 0);
                this.AddSHTCouple(false, 5784, 1929, 0, false, 5700, 1929, 0);

                tele1 = this.AddSHT(Map.Trammel, false, 5711, 1952, 0);
                tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = this.AddSHT(Map.Trammel, false, 5657, 1954, 0);
                Link(tele1, tele2);

                tele1 = this.AddSHT(Map.Felucca, false, 5711, 1952, 0);
                tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = this.AddSHT(Map.Felucca, false, 5657, 1954, 0);
                Link(tele1, tele2);

                tele1 = this.AddSHT(Map.Trammel, false, 5655, 2018, 0);
                tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = this.AddSHT(Map.Trammel, true, 1690, 2789, 0);
                Link(tele1, tele2);

                tele1 = this.AddSHT(Map.Felucca, false, 5655, 2018, 0);
                tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = this.AddSHT(Map.Felucca, true, 1690, 2789, 0);
                Link(tele1, tele2);

                this.AddSHTCouple(false, 5809, 1905, 0, false, 5876, 1891, 0);

                tele1 = this.AddSHT(Map.Trammel, false, 5814, 2015, 0);
                tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = this.AddSHT(Map.Trammel, false, 5913, 1893, 0);
                Link(tele1, tele2);

                tele1 = this.AddSHT(Map.Felucca, false, 5814, 2015, 0);
                tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = this.AddSHT(Map.Felucca, false, 5913, 1893, 0);
                Link(tele1, tele2);

                this.AddSHTCouple(false, 5919, 2021, 0, true, 1724, 814, 0);

                tele1 = this.AddSHT(Map.Trammel, false, 5654, 1791, 0);
                tele2 = this.AddSHT(Map.Trammel, true, 730, 1451, 0);
                Link(tele1, tele2);
                this.AddSHT(Map.Trammel, false, 5734, 1859, 0).ChangeDest(tele2);

                tele1 = this.AddSHT(Map.Felucca, false, 5654, 1791, 0);
                tele2 = this.AddSHT(Map.Felucca, true, 730, 1451, 0);
                Link(tele1, tele2);
                this.AddSHT(Map.Felucca, false, 5734, 1859, 0).ChangeDest(tele2);

                return this.m_Count;
            }
        }
    }
}