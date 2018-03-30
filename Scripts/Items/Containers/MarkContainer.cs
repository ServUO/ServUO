using System;
using Server.Commands;

namespace Server.Items
{
    public class MarkContainer : LockableContainer
    {
        private bool m_AutoLock;
        private InternalTimer m_RelockTimer;
        private Map m_TargetMap;
        private Point3D m_Target;
        private string m_Description;
        [Constructable]
        public MarkContainer()
            : this(false)
        {
        }

        [Constructable]
        public MarkContainer(bool bone)
            : this(bone, false)
        {
        }

        [Constructable]
        public MarkContainer(bool bone, bool locked)
            : base(bone ? 0xECA : 0xE79)
        {
            this.Movable = false;

            if (bone)
                this.Hue = 1102;

            this.m_AutoLock = locked;
            this.Locked = locked;

            if (locked)
                this.LockLevel = -255;
        }

        public MarkContainer(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AutoLock
        {
            get
            {
                return this.m_AutoLock;
            }
            set
            {
                this.m_AutoLock = value;

                if (!this.m_AutoLock)
                    this.StopTimer();
                else if (!this.Locked && this.m_RelockTimer == null)
                    this.m_RelockTimer = new InternalTimer(this);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Map TargetMap
        {
            get
            {
                return this.m_TargetMap;
            }
            set
            {
                this.m_TargetMap = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Target
        {
            get
            {
                return this.m_Target;
            }
            set
            {
                this.m_Target = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Bone
        {
            get
            {
                return this.ItemID == 0xECA;
            }
            set
            {
                this.ItemID = value ? 0xECA : 0xE79;
                this.Hue = value ? 1102 : 0;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Description
        {
            get
            {
                return this.m_Description;
            }
            set
            {
                this.m_Description = value;
            }
        }
        public override bool IsDecoContainer
        {
            get
            {
                return false;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public override bool Locked
        {
            get
            {
                return base.Locked;
            }
            set
            {
                base.Locked = value;

                if (this.m_AutoLock)
                {
                    this.StopTimer();

                    if (!this.Locked)
                        this.m_RelockTimer = new InternalTimer(this);
                }
            }
        }
        public static void Initialize()
        {
            CommandSystem.Register("SecretLocGen", AccessLevel.Administrator, new CommandEventHandler(SecretLocGen_OnCommand));
			CommandSystem.Register("SecretLocDelete", AccessLevel.Administrator, new CommandEventHandler(SecretLocDelete_OnCommand));
		}

		[Usage("SecretLocDelete")]
		[Description("Deletes mark containers to Malas secret locations.")]
		public static void SecretLocDelete_OnCommand(CommandEventArgs e)
		{
			WeakEntityCollection.Delete("malas");
		}

		[Usage("SecretLocGen")]
        [Description("Generates mark containers to Malas secret locations.")]
        public static void SecretLocGen_OnCommand(CommandEventArgs e)
        {
            CreateMalasPassage(951, 546, -70, 1006, 994, -70, false, false);
            CreateMalasPassage(914, 192, -79, 1019, 1062, -70, false, false);
            CreateMalasPassage(1614, 143, -90, 1214, 1313, -90, false, false);
            CreateMalasPassage(2176, 324, -90, 1554, 172, -90, false, false);
            CreateMalasPassage(864, 812, -90, 1061, 1161, -70, false, false);
            CreateMalasPassage(1051, 1434, -85, 1076, 1244, -70, false, true);
            CreateMalasPassage(1326, 523, -87, 1201, 1554, -70, false, false);
            CreateMalasPassage(424, 189, -1, 2333, 1501, -90, true, false);
            CreateMalasPassage(1313, 1115, -85, 1183, 462, -45, false, false);

            e.Mobile.SendMessage("Secret mark containers have been created.");

            Server.Engines.GenerateForgottenPyramid.Generate(e.Mobile);
        }

        public void StopTimer()
        {
            if (this.m_RelockTimer != null)
                this.m_RelockTimer.Stop();

            this.m_RelockTimer = null;
        }

        public void Mark(RecallRune rune)
        {
            if (this.TargetMap != null)
            {
                rune.Marked = true;
                rune.TargetMap = this.m_TargetMap;
                rune.Target = this.m_Target;
                rune.Description = this.m_Description;
                rune.House = null;
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            RecallRune rune = dropped as RecallRune;

            if (rune != null && base.OnDragDrop(from, dropped))
            {
                this.Mark(rune);

                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool OnDragDropInto(Mobile from, Item dropped, Point3D p)
        {
            RecallRune rune = dropped as RecallRune;

            if (rune != null && base.OnDragDropInto(from, dropped, p))
            {
                this.Mark(rune);

                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_AutoLock);

            if (!this.Locked && this.m_AutoLock)
                writer.WriteDeltaTime(this.m_RelockTimer.RelockTime);

            writer.Write(this.m_TargetMap);
            writer.Write(this.m_Target);
            writer.Write(this.m_Description);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_AutoLock = reader.ReadBool();

            if (!this.Locked && this.m_AutoLock)
                this.m_RelockTimer = new InternalTimer(this, reader.ReadDeltaTime() - DateTime.UtcNow);

            this.m_TargetMap = reader.ReadMap();
            this.m_Target = reader.ReadPoint3D();
            this.m_Description = reader.ReadString();
        }

        private static bool FindMarkContainer(Point3D p, Map map)
        {
            IPooledEnumerable eable = map.GetItemsInRange(p, 0);

            foreach (Item item in eable)
            {
                if (item.Z == p.Z && item is MarkContainer)
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }

        private static void CreateMalasPassage(int x, int y, int z, int xTarget, int yTarget, int zTarget, bool bone, bool locked)
        {
            Point3D location = new Point3D(x, y, z);

            if (FindMarkContainer(location, Map.Malas))
                return;

            MarkContainer cont = new MarkContainer(bone, locked);
			WeakEntityCollection.Add("malas", cont);
            cont.TargetMap = Map.Malas;
            cont.Target = new Point3D(xTarget, yTarget, zTarget);
            cont.Description = "strange location";

            cont.MoveToWorld(location, Map.Malas);
        }

        private class InternalTimer : Timer
        {
            private readonly MarkContainer m_Container;
            private readonly DateTime m_RelockTime;
            public InternalTimer(MarkContainer container)
                : this(container, TimeSpan.FromMinutes(5.0))
            {
            }

            public InternalTimer(MarkContainer container, TimeSpan delay)
                : base(delay)
            {
                this.m_Container = container;
                this.m_RelockTime = DateTime.UtcNow + delay;

                this.Start();
            }

            public MarkContainer Container
            {
                get
                {
                    return this.m_Container;
                }
            }
            public DateTime RelockTime
            {
                get
                {
                    return this.m_RelockTime;
                }
            }
            protected override void OnTick()
            {
                this.m_Container.Locked = true;
                this.m_Container.LockLevel = -255;
            }
        }
    }
}