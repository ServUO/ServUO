using Server.Commands;
using Server.Mobiles;
using Server.Gumps;

using System;
using System.Linq;

namespace Server.Items
{
    public class WinchAssembly : PeerlessAltar
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

            Hatch hatch = new Hatch(winch);
            WeakEntityCollection.Add(EntityName, hatch);

            WinchAssemblyLever lever = new WinchAssemblyLever(winch, hatch);
            WeakEntityCollection.Add(EntityName, lever);

            lever.MoveToWorld(new Point3D(6310, 1705, 0), Map.Trammel);
            winch.MoveToWorld(new Point3D(6310, 1704, 0), Map.Trammel);
            hatch.MoveToWorld(new Point3D(6303, 1711, 10), Map.Trammel);

            var tele = new ExitTeleporter(winch);
            tele.MoveToWorld(new Point3D(6400, 1656, 0), Map.Trammel);
            WeakEntityCollection.Add(EntityName, tele);

            if (m != null)
            {
                m.SendMessage("Winch Assembly Generation completed!");
            }
            else
            {
                Console.WriteLine("Winch Assembly Generation completed!");
            }
        }

        private static void DeleteWinchAssembly(Mobile from)
        {
            WeakEntityCollection.Delete(EntityName);

            var ladder = Map.Trammel.FindItem<ShipLadder>(new Point3D(6400, 1656, 0), 0);

            if (ladder != null)
            {
                ladder.Delete();
            }
        }

        public override int LabelNumber => 1154433;  // Winch Assembly
        public override bool ForceShowProperties => true;

        public override Type[] Keys => new Type[] { typeof(BearingAssembly), typeof(FlyWheel), typeof(PowerCore), typeof(WireSpool) };
        public override int KeyCount => 0;
        public override MasterKey MasterKey => null;

        public override BasePeerless Boss => new Shadowlord();
        public override Rectangle2D[] BossBounds => new Rectangle2D[] { new Rectangle2D(6399, 1631, 38, 38) };

        [CommandProperty(AccessLevel.GameMaster)]
        public Hatch Hatch { get; set; }

        [Constructable]
        public WinchAssembly()
            : base(0x280E)
        {
            Hue = 2101;

            BossLocation = new Point3D(6417, 1649, 0);
            TeleportDest = new Point3D(6401, 1665, 0);
            ExitDest = new Point3D(6296, 1715, 0);
        }

        public WinchAssembly(Serial serial)
            : base(serial)
        {
        }

        public override void StopTimers()
        {
            base.StopTimers();

            if (Hatch != null)
            {
                Hatch.Reset();  
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(KeyValidation != null && KeyValidation.Any(x => x.Key == typeof(FlyWheel) && x.Active) ? 1154448 : 1154432);
            list.Add(KeyValidation != null && KeyValidation.Any(x => x.Key == typeof(WireSpool) && x.Active) ? 1154449 : 1154434);
            list.Add(KeyValidation != null && KeyValidation.Any(x => x.Key == typeof(PowerCore) && x.Active) ? 1154450 : 1154435);
            list.Add(KeyValidation != null && KeyValidation.Any(x => x.Key == typeof(BearingAssembly) && x.Active) ? 1154451 : 1154436);
        }

        public void Activate(Mobile from)
        {
            base.ActivateEncounter(from);
        }

        public override void ActivateEncounter(Mobile from)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.WriteItem(Hatch);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Hatch = reader.ReadItem<Hatch>();

            if (Hatch != null)
            {
                Hatch.Winch = this;
            }
        }
    }

    public class WinchAssemblyLever : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public WinchAssembly Winch { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Hatch Hatch { get; set; }

        [Constructable]
        public WinchAssemblyLever(WinchAssembly winch, Hatch hatch)
            : base(0x108E)
        {
            Movable = false;
            Winch = winch;
            Hatch = hatch;
        }

        public WinchAssemblyLever(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Winch == null || Hatch == null || Winch.Owner == null || !Winch.KeysValidated())
                return;

            if (Winch.Peerless != null && Winch.Peerless.CheckAlive())
            {
                from.SendLocalizedMessage(1075213); // The master of this realm has already been summoned and is engaged in combat.  Your opportunity will come after he has squashed the current batch of intruders!
            }
            else if (!Winch.CheckParty(from))
            {
                from.SendLocalizedMessage(1072683, Winch.Owner.Name); // ~1_NAME~ has already activated the Prism, please wait...
            }
            else
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0), 3, new TimerStateCallback(Hatch.DoDownEffect), new object[] { Hatch.Location, 0, from });

                Winch.Activate(from);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version

            writer.Write(Winch);
            writer.Write(Hatch);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Winch = reader.ReadItem() as WinchAssembly;
            Hatch = reader.ReadItem() as Hatch;

            if (version == 0 && Winch != null)
            {
                Winch.Hatch = Hatch;
            }
        }
    }

    public class Hatch : BaseAddon
    {
        private Timer m_Timer;

        [CommandProperty(AccessLevel.GameMaster)]
        public WinchAssembly Winch { get; set; }

        [Constructable]
        public Hatch(WinchAssembly winch)
            : base()
        {
            Winch = winch;
            winch.Hatch = this;

            AddComponent(new HatchTile(this), 2, 7, 0);
            AddComponent(new HatchTile(this), 2, 6, 0);
            AddComponent(new HatchTile(this), 2, 5, 0);
            AddComponent(new HatchTile(this), 2, 4, 0);
            AddComponent(new HatchTile(this), 2, 3, 0);
            AddComponent(new HatchTile(this), 2, 2, 0);
            AddComponent(new HatchTile(this), 3, 7, 0);
            AddComponent(new HatchTile(this), 3, 6, 0);
            AddComponent(new HatchTile(this), 3, 5, 0);
            AddComponent(new HatchTile(this), 3, 4, 0);
            AddComponent(new HatchTile(this), 3, 3, 0);
            AddComponent(new HatchTile(this), 3, 2, 0);
            AddComponent(new HatchTile(this), 4, 7, 0);
            AddComponent(new HatchTile(this), 4, 6, 0);
            AddComponent(new HatchTile(this), 4, 5, 0);
            AddComponent(new HatchTile(this), 4, 4, 0);
            AddComponent(new HatchTile(this), 4, 3, 0);
            AddComponent(new HatchTile(this), 4, 2, 0);
            AddComponent(new HatchTile(this), 5, 7, 0);
            AddComponent(new HatchTile(this), 5, 6, 0);
            AddComponent(new HatchTile(this), 5, 5, 0);
            AddComponent(new HatchTile(this), 5, 4, 0);
            AddComponent(new HatchTile(this), 5, 3, 0);
            AddComponent(new HatchTile(this), 5, 2, 0);
            AddComponent(new HatchTile(this), 2, -1, 0);
            AddComponent(new HatchTile(this), 2, -2, 0);
            AddComponent(new HatchTile(this), 2, -3, 0);
            AddComponent(new HatchTile(this), 2, -4, 0);
            AddComponent(new HatchTile(this), 2, -5, 0);
            AddComponent(new HatchTile(this), 2, -6, 0);
            AddComponent(new HatchTile(this), 3, -1, 0);
            AddComponent(new HatchTile(this), 3, -2, 0);
            AddComponent(new HatchTile(this), 3, -3, 0);
            AddComponent(new HatchTile(this), 3, -4, 0);
            AddComponent(new HatchTile(this), 3, -5, 0);
            AddComponent(new HatchTile(this), 3, -6, 0);
            AddComponent(new HatchTile(this), 4, -1, 0);
            AddComponent(new HatchTile(this), 4, -2, 0);
            AddComponent(new HatchTile(this), 4, -3, 0);
            AddComponent(new HatchTile(this), 4, -4, 0);
            AddComponent(new HatchTile(this), 4, -5, 0);
            AddComponent(new HatchTile(this), 4, -6, 0);
            AddComponent(new HatchTile(this), 5, -1, 0);
            AddComponent(new HatchTile(this), 5, -2, 0);
            AddComponent(new HatchTile(this), 5, -3, 0);
            AddComponent(new HatchTile(this), 5, -4, 0);
            AddComponent(new HatchTile(this), 5, -5, 0);
            AddComponent(new HatchTile(this), 5, -6, 0);
            AddComponent(new HatchTile(this), -4, 7, 0);
            AddComponent(new HatchTile(this), -4, 6, 0);
            AddComponent(new HatchTile(this), -4, 5, 0);
            AddComponent(new HatchTile(this), -4, 4, 0);
            AddComponent(new HatchTile(this), -4, 3, 0);
            AddComponent(new HatchTile(this), -4, 2, 0);
            AddComponent(new HatchTile(this), -3, 7, 0);
            AddComponent(new HatchTile(this), -3, 6, 0);
            AddComponent(new HatchTile(this), -3, 5, 0);
            AddComponent(new HatchTile(this), -3, 4, 0);
            AddComponent(new HatchTile(this), -3, 3, 0);
            AddComponent(new HatchTile(this), -3, 2, 0);
            AddComponent(new HatchTile(this), -2, 7, 0);
            AddComponent(new HatchTile(this), -2, 6, 0);
            AddComponent(new HatchTile(this), -2, 5, 0);
            AddComponent(new HatchTile(this), -2, 4, 0);
            AddComponent(new HatchTile(this), -2, 3, 0);
            AddComponent(new HatchTile(this), -2, 2, 0);
            AddComponent(new HatchTile(this), -1, 7, 0);
            AddComponent(new HatchTile(this), -1, 6, 0);
            AddComponent(new HatchTile(this), -1, 5, 0);
            AddComponent(new HatchTile(this), -1, 4, 0);
            AddComponent(new HatchTile(this), -1, 3, 0);
            AddComponent(new HatchTile(this), -1, 2, 0);
            AddComponent(new HatchTile(this), -4, -1, 0);
            AddComponent(new HatchTile(this), -4, -2, 0);
            AddComponent(new HatchTile(this), -4, -3, 0);
            AddComponent(new HatchTile(this), -4, -4, 0);
            AddComponent(new HatchTile(this), -4, -5, 0);
            AddComponent(new HatchTile(this), -4, -6, 0);
            AddComponent(new HatchTile(this), -3, -1, 0);
            AddComponent(new HatchTile(this), -3, -2, 0);
            AddComponent(new HatchTile(this), -3, -3, 0);
            AddComponent(new HatchTile(this), -3, -4, 0);
            AddComponent(new HatchTile(this), -3, -5, 0);
            AddComponent(new HatchTile(this), -3, -6, 0);
            AddComponent(new HatchTile(this), -2, -1, 0);
            AddComponent(new HatchTile(this), -2, -2, 0);
            AddComponent(new HatchTile(this), -2, -3, 0);
            AddComponent(new HatchTile(this), -2, -4, 0);
            AddComponent(new HatchTile(this), -2, -5, 0);
            AddComponent(new HatchTile(this), -2, -6, 0);
            AddComponent(new HatchTile(this), -1, -1, 0);
            AddComponent(new HatchTile(this), -1, -2, 0);
            AddComponent(new HatchTile(this), -1, -3, 0);
            AddComponent(new HatchTile(this), -1, -4, 0);
            AddComponent(new HatchTile(this), -1, -5, 0);
            AddComponent(new HatchTile(this), -1, -6, 0);
        }

        public Hatch(Serial serial) : base(serial)
        {
        }

        public void DoDownEffect(object state)
        {
            if (Deleted)
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

                Effects.SendLocationEffect(new Point3D(p.X + x, p.Y + y, p.Z), Map, 0x36CB, 16, 4, 1362, 0);


                Z -= 1;

                if (Z == 1)
                {
                    Hue = 1;
                }
            }
        }

        public void Reset()
        {
            Z = 10;
            Hue = 2969;
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;

            base.OnAfterDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                Timer.DelayCall(() =>
                {
                    WinchAssembly.GenWinchAssembly(null);
                });
            }
        }
    }

    public class HatchTile : AddonComponent
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active => (Z == 1);

        public HatchTile(Hatch hatch)
            : base(0x07CD)
        {
            Hue = 2969;
        }

        public HatchTile(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (Active && Addon is Hatch hatch)
            {
                var winch = hatch.Winch;

                if (winch.CheckParty(m))
                {
                    m.CloseGump(typeof(ConfirmEntranceGump));
                    m.SendGump(new ConfirmEntranceGump(winch, m));
                }
            }

            return false;
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

    public class ExitTeleporter : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public PeerlessAltar Altar { get; set; }

        public override int LabelNumber => 1022201;

        public ExitTeleporter(PeerlessAltar altar)
            : base(2209)
        {
            Altar = altar;
            Movable = false;
        }

        public ExitTeleporter(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 3))
            {
                OnMoveOver(from);
            }
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.Alive)
            {
                m.CloseGump(typeof(ConfirmExitGump));
                m.SendGump(new ConfirmExitGump(Altar));
            }
            else if (Altar != null)
            {
                Altar.Exit(m);
                return false;
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.WriteItem(Altar);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Altar = reader.ReadItem<PeerlessAltar>();
        }
    }
}
