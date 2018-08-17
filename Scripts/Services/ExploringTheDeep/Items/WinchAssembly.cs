using System;
using Server.Mobiles;
using Server.Commands;
using System.Collections.Generic;

namespace Server.Items
{
    public class WinchAssembly : Item
    {
        public static readonly string EntityName = "winchassemply";

        public static void Initialize()
        {
            CommandSystem.Register("GenWinchAssembly", AccessLevel.Administrator, GenWinchAssembly_Command);
            CommandSystem.Register("DelWinchAssembly", AccessLevel.Administrator, DelWinchAssembly_Command);
        }

        [Usage("GenWinchAssembly")]
        private static void GenWinchAssembly_Command(CommandEventArgs e)
        {
            GenWinchAssembly(e.Mobile);
        }

        [Usage("GenWinchAssembly")]
        private static void DelWinchAssembly_Command(CommandEventArgs e)
        {
            DeleteWinchAssembly(e.Mobile);
        }

        public static void GenWinchAssembly(Mobile m)
        {
            DeleteWinchAssembly(m);            

            // Winch 
            WinchAssembly winch = new WinchAssembly();
            WeakEntityCollection.Add(EntityName, winch);

            Hatch hatch = new Hatch();
            WeakEntityCollection.Add(EntityName, hatch);

            WinchAssemblyLever lever = new WinchAssemblyLever(winch, hatch);
            WeakEntityCollection.Add(EntityName, lever);

            lever.MoveToWorld(new Point3D(6310, 1705, 0), Map.Trammel);
            winch.MoveToWorld(new Point3D(6310, 1704, 0), Map.Trammel);
            hatch.MoveToWorld(new Point3D(6303, 1711, 10), Map.Trammel);

            m.SendMessage("Winch Assembly Generation completed!");
        }

        private static void DeleteWinchAssembly(Mobile from)
        {
            WeakEntityCollection.Delete(EntityName);
        }

        public override int LabelNumber { get { return 1154433; } } // Winch Assembly

        private bool m_flywheel;
        private bool m_wirespool;
        private bool m_bearingassembly;
        private bool m_powercore;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool FlyWheel
        {
            get { return m_flywheel; }
            set
            {
                m_flywheel = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool WireSpool
        {
            get { return m_wirespool; }
            set
            {
                m_wirespool = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool BearingAssembly
        {
            get { return m_bearingassembly; }
            set
            {
                m_bearingassembly = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PowerCore
        {
            get { return m_powercore; }
            set
            {
                m_powercore = value;
                InvalidateProperties();
            }
        }

        [Constructable]
        public WinchAssembly() : base(0x280E)
        {
            this.Movable = false;
            this.Hue = 2101;
        }

        public WinchAssembly(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(FlyWheel ? 1154448 : 1154432);
            list.Add(WireSpool ? 1154449 : 1154434);
            list.Add(PowerCore ? 1154450 : 1154435);
            list.Add(BearingAssembly ? 1154451 : 1154436);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is BearingAssembly && !m_bearingassembly)
            {
                dropped.Delete();
                BearingAssembly = true;
            }
            else if (dropped is FlyWheel && !m_flywheel)
            {
                dropped.Delete();
                FlyWheel = true;
            }
            else if (dropped is PowerCore && !m_powercore)
            {
                dropped.Delete();
                PowerCore = true;
            }
            else if (dropped is WireSpool && !m_wirespool)
            {
                dropped.Delete();
                WireSpool = true;
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(m_flywheel);
            writer.Write(m_wirespool);
            writer.Write(m_bearingassembly);
            writer.Write(m_powercore);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_flywheel = reader.ReadBool();
            m_wirespool = reader.ReadBool();
            m_bearingassembly = reader.ReadBool();
            m_powercore = reader.ReadBool();
        }
    }

    public class WinchAssemblyLever : Item
    {
        private WinchAssembly m_WinchAssembly;
        private Hatch m_hatch;

        [CommandProperty(AccessLevel.GameMaster)]
        public WinchAssembly Winch
        {
            get { return m_WinchAssembly; }
            set { m_WinchAssembly = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Hatch Hatch
        {
            get { return m_hatch; }
            set { m_hatch = value; }
        }

        [Constructable]
        public WinchAssemblyLever(WinchAssembly winch, Hatch hatch)
            : base(0x108E)
        {
            this.Movable = false;
            this.m_WinchAssembly = winch;
            this.m_hatch = hatch;
        }

        public WinchAssemblyLever(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_WinchAssembly == null || m_hatch == null)
                return;

            if (m_WinchAssembly.BearingAssembly && m_WinchAssembly.FlyWheel && m_WinchAssembly.WireSpool && m_WinchAssembly.PowerCore)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0), 3, new TimerStateCallback(m_hatch.DoDownEffect), new object[] { m_hatch.Location, 0, from });

                Mobile creature = Shadowlord.Spawn(new Point3D(6417, 1649, 0), Map.Trammel);
              
                m_WinchAssembly.BearingAssembly = false;
                m_WinchAssembly.FlyWheel = false;
                m_WinchAssembly.WireSpool = false;
                m_WinchAssembly.PowerCore = false;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(m_WinchAssembly);
            writer.Write(m_hatch);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_WinchAssembly = reader.ReadItem() as WinchAssembly;
            m_hatch = reader.ReadItem() as Hatch;
        }
    }

    public class Hatch : BaseAddon
    {
        private Timer m_Timer;

        [Constructable]
        public Hatch()
            : base()
        {
            this.AddComponent(new HatchTile(this), 2, 7, 0);
            this.AddComponent(new HatchTile(this), 2, 6, 0);
            this.AddComponent(new HatchTile(this), 2, 5, 0);
            this.AddComponent(new HatchTile(this), 2, 4, 0);
            this.AddComponent(new HatchTile(this), 2, 3, 0);
            this.AddComponent(new HatchTile(this), 2, 2, 0);
            this.AddComponent(new HatchTile(this), 3, 7, 0);
            this.AddComponent(new HatchTile(this), 3, 6, 0);
            this.AddComponent(new HatchTile(this), 3, 5, 0);
            this.AddComponent(new HatchTile(this), 3, 4, 0);
            this.AddComponent(new HatchTile(this), 3, 3, 0);
            this.AddComponent(new HatchTile(this), 3, 2, 0);
            this.AddComponent(new HatchTile(this), 4, 7, 0);
            this.AddComponent(new HatchTile(this), 4, 6, 0);
            this.AddComponent(new HatchTile(this), 4, 5, 0);
            this.AddComponent(new HatchTile(this), 4, 4, 0);
            this.AddComponent(new HatchTile(this), 4, 3, 0);
            this.AddComponent(new HatchTile(this), 4, 2, 0);
            this.AddComponent(new HatchTile(this), 5, 7, 0);
            this.AddComponent(new HatchTile(this), 5, 6, 0);
            this.AddComponent(new HatchTile(this), 5, 5, 0);
            this.AddComponent(new HatchTile(this), 5, 4, 0);
            this.AddComponent(new HatchTile(this), 5, 3, 0);
            this.AddComponent(new HatchTile(this), 5, 2, 0);
            this.AddComponent(new HatchTile(this), 2, -1, 0);
            this.AddComponent(new HatchTile(this), 2, -2, 0);
            this.AddComponent(new HatchTile(this), 2, -3, 0);
            this.AddComponent(new HatchTile(this), 2, -4, 0);
            this.AddComponent(new HatchTile(this), 2, -5, 0);
            this.AddComponent(new HatchTile(this), 2, -6, 0);
            this.AddComponent(new HatchTile(this), 3, -1, 0);
            this.AddComponent(new HatchTile(this), 3, -2, 0);
            this.AddComponent(new HatchTile(this), 3, -3, 0);
            this.AddComponent(new HatchTile(this), 3, -4, 0);
            this.AddComponent(new HatchTile(this), 3, -5, 0);
            this.AddComponent(new HatchTile(this), 3, -6, 0);
            this.AddComponent(new HatchTile(this), 4, -1, 0);
            this.AddComponent(new HatchTile(this), 4, -2, 0);
            this.AddComponent(new HatchTile(this), 4, -3, 0);
            this.AddComponent(new HatchTile(this), 4, -4, 0);
            this.AddComponent(new HatchTile(this), 4, -5, 0);
            this.AddComponent(new HatchTile(this), 4, -6, 0);
            this.AddComponent(new HatchTile(this), 5, -1, 0);
            this.AddComponent(new HatchTile(this), 5, -2, 0);
            this.AddComponent(new HatchTile(this), 5, -3, 0);
            this.AddComponent(new HatchTile(this), 5, -4, 0);
            this.AddComponent(new HatchTile(this), 5, -5, 0);
            this.AddComponent(new HatchTile(this), 5, -6, 0);
            this.AddComponent(new HatchTile(this), -4, 7, 0);
            this.AddComponent(new HatchTile(this), -4, 6, 0);
            this.AddComponent(new HatchTile(this), -4, 5, 0);
            this.AddComponent(new HatchTile(this), -4, 4, 0);
            this.AddComponent(new HatchTile(this), -4, 3, 0);
            this.AddComponent(new HatchTile(this), -4, 2, 0);
            this.AddComponent(new HatchTile(this), -3, 7, 0);
            this.AddComponent(new HatchTile(this), -3, 6, 0);
            this.AddComponent(new HatchTile(this), -3, 5, 0);
            this.AddComponent(new HatchTile(this), -3, 4, 0);
            this.AddComponent(new HatchTile(this), -3, 3, 0);
            this.AddComponent(new HatchTile(this), -3, 2, 0);
            this.AddComponent(new HatchTile(this), -2, 7, 0);
            this.AddComponent(new HatchTile(this), -2, 6, 0);
            this.AddComponent(new HatchTile(this), -2, 5, 0);
            this.AddComponent(new HatchTile(this), -2, 4, 0);
            this.AddComponent(new HatchTile(this), -2, 3, 0);
            this.AddComponent(new HatchTile(this), -2, 2, 0);
            this.AddComponent(new HatchTile(this), -1, 7, 0);
            this.AddComponent(new HatchTile(this), -1, 6, 0);
            this.AddComponent(new HatchTile(this), -1, 5, 0);
            this.AddComponent(new HatchTile(this), -1, 4, 0);
            this.AddComponent(new HatchTile(this), -1, 3, 0);
            this.AddComponent(new HatchTile(this), -1, 2, 0);
            this.AddComponent(new HatchTile(this), -4, -1, 0);
            this.AddComponent(new HatchTile(this), -4, -2, 0);
            this.AddComponent(new HatchTile(this), -4, -3, 0);
            this.AddComponent(new HatchTile(this), -4, -4, 0);
            this.AddComponent(new HatchTile(this), -4, -5, 0);
            this.AddComponent(new HatchTile(this), -4, -6, 0);
            this.AddComponent(new HatchTile(this), -3, -1, 0);
            this.AddComponent(new HatchTile(this), -3, -2, 0);
            this.AddComponent(new HatchTile(this), -3, -3, 0);
            this.AddComponent(new HatchTile(this), -3, -4, 0);
            this.AddComponent(new HatchTile(this), -3, -5, 0);
            this.AddComponent(new HatchTile(this), -3, -6, 0);
            this.AddComponent(new HatchTile(this), -2, -1, 0);
            this.AddComponent(new HatchTile(this), -2, -2, 0);
            this.AddComponent(new HatchTile(this), -2, -3, 0);
            this.AddComponent(new HatchTile(this), -2, -4, 0);
            this.AddComponent(new HatchTile(this), -2, -5, 0);
            this.AddComponent(new HatchTile(this), -2, -6, 0);
            this.AddComponent(new HatchTile(this), -1, -1, 0);
            this.AddComponent(new HatchTile(this), -1, -2, 0);
            this.AddComponent(new HatchTile(this), -1, -3, 0);
            this.AddComponent(new HatchTile(this), -1, -4, 0);
            this.AddComponent(new HatchTile(this), -1, -5, 0);
            this.AddComponent(new HatchTile(this), -1, -6, 0);
        }

        public Hatch(Serial serial) : base(serial)
        {
        }

        public void DoDownEffect(object state)
        {
            if (this.Deleted)
                return;

            object[] states = (object[])state;

            Point3D p = (Point3D)states[0];
                        
            for (int i = 0; i < 3; ++i)
            {
                int x, y;

                switch (Utility.Random(8))
                {
                    default:
                    case 0:
                        x = -1;
                        y = -1;
                        break;
                    case 1:
                        x = -1;
                        y = 0;
                        break;
                    case 2:
                        x = -1;
                        y = +1;
                        break;
                    case 3:
                        x = 0;
                        y = -1;
                        break;
                    case 4:
                        x = 0;
                        y = +1;
                        break;
                    case 5:
                        x = +1;
                        y = -1;
                        break;
                    case 6:
                        x = +1;
                        y = 0;
                        break;
                    case 7:
                        x = +1;
                        y = +1;
                        break;
                }

                Effects.SendLocationEffect(new Point3D(p.X + x, p.Y + y, p.Z), this.Map, 0x36CB, 16, 4, 1362, 0);

                
                this.Z -= 1;

                if (this.Z == 1)
                {
                    this.Hue = 1;
                    this.m_Timer = new InternalTimer(this);
                    this.m_Timer.Start();
                }
            }
        }

        public override void OnAfterDelete()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;

            base.OnAfterDelete();
        }

        public class InternalTimer : Timer
        {
            public Hatch m_hatch;

            public InternalTimer(Hatch hatch) : base(TimeSpan.FromMinutes(30.0))
            {
                Priority = TimerPriority.OneSecond;
                m_hatch = hatch;
            }

            protected override void OnTick()
            {
                this.m_hatch.Z = 10;
                this.m_hatch.Hue = 2969;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            this.m_Timer = new InternalTimer(this);
            this.m_Timer.Start();
        }
    }

    public class HatchTile : AddonComponent
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active { get { return (this.Z == 1); } }

        public HatchTile(Hatch hatch)
            : base(0x07CD)
        {
            this.Hue = 2969;
        }

        public HatchTile(Serial serial)
            : base(serial)
        {
        }

        public override bool HandlesOnMovement { get { return true; } }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (Active && m.Player && Utility.InRange(this.Location, m.Location, 0) && !Utility.InRange(this.Location, oldLocation, 0))
            {
                m.MoveToWorld(new Point3D(6415, 1647, 0), Map.Trammel);
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
    }
}
