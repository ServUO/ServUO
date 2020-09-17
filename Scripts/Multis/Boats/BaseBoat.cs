using Server.ContextMenus;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server.Multis
{
    public enum BoatOrder
    {
        PlayerControlled,
        CourseFull,
        CourseSingle
    }

    public enum DamageLevel
    {
        Pristine = 0,
        Slightly = 1,
        Moderately = 2,
        Heavily = 3,
        Severely = 4
    }

    public abstract class BaseBoat : BaseMulti, IMount
    {
        #region Statics

        private static readonly Rectangle2D[] m_BritWrap = new Rectangle2D[]{ new Rectangle2D( 16, 16, 5120 - 32, 4096 - 32 ), new Rectangle2D( 5136, 2320, 992, 1760 ),
                                                                     new Rectangle2D(6272, 1088, 319, 319)};
        private static readonly Rectangle2D[] m_IlshWrap = new Rectangle2D[] { new Rectangle2D(16, 16, 2304 - 32, 1600 - 32) };
        private static readonly Rectangle2D[] m_TokunoWrap = new Rectangle2D[] { new Rectangle2D(16, 16, 1448 - 32, 1448 - 32) };

        private static readonly Type[] WoodTypes = new Type[] { typeof(Board),  typeof(OakBoard), typeof(AshBoard), typeof(YewBoard), typeof(HeartwoodBoard), typeof(BloodwoodBoard), typeof(FrostwoodBoard),
                                                typeof(Log), typeof(OakLog), typeof(AshLog), typeof(YewLog), typeof(HeartwoodLog), typeof(BloodwoodLog), typeof(FrostwoodLog), };

        private static readonly Type[] ClothTypes = new Type[] { typeof(Cloth), typeof(UncutCloth) };

        private static readonly int SlowSpeed = 1;
        private static readonly int FastSpeed = 1;

        private static readonly double WoodPer = 17;
        private static readonly double ClothPer = 17;

        public static readonly int EmergencyRepairClothCost = 55;
        public static readonly int EmergencyRepairWoodCost = 25;
        public static readonly TimeSpan EmergencyRepairSpan = TimeSpan.FromMinutes(6);

        public static List<BaseBoat> Boats { get; } = new List<BaseBoat>();

        public static BaseBoat FindBoatAt(IEntity entity)
        {
            return FindBoatAt(entity, entity.Map);
        }

        public static BaseBoat FindBoatAt(IPoint2D loc, Map map)
        {
            if (map == null || map == Map.Internal)
                return null;

            Sector sector = map.GetSector(loc);

            for (int i = 0; i < sector.Multis.Count; i++)
            {
                if (sector.Multis[i] is BaseBoat boat && boat.Contains(loc.X, loc.Y))
                    return boat;
            }

            return null;
        }

        public static void Initialize()
        {
            new UpdateAllTimer().Start();
            EventSink.WorldSave += EventSink_WorldSave;
            EventSink.Disconnected += EventSink_Disconnected;
            EventSink.PlayerDeath += EventSink_PlayerDeath;
        }

        public static void UpdateAllComponents()
        {
            List<BaseBoat> toDelete = new List<BaseBoat>();

            for (int i = Boats.Count - 1; i >= 0; --i)
            {
                BaseBoat boat = Boats[i];

                boat.UpdateComponents();

                if (boat.PlayerCount > 0)
                    boat.Refresh();
            }

            foreach (BaseBoat b in toDelete)
                b.Delete();

            toDelete.Clear();
            toDelete.TrimExcess();
        }

        private static void EventSink_WorldSave(WorldSaveEventArgs e)
        {
            new UpdateAllTimer().Start();
        }

        public static void EventSink_Disconnected(DisconnectedEventArgs e)
        {
            ForceRemovePilot(e.Mobile);
        }

        public static void EventSink_PlayerDeath(PlayerDeathEventArgs e)
        {
            ForceRemovePilot(e.Mobile);
        }

        public static void ForceRemovePilot(Mobile m)
        {
            if (m.FindItemOnLayer(Layer.Mount) is BoatMountItem mountItem)
            {
                if (mountItem.Mount is BaseBoat boat)
                {
                    if (boat.Pilot == m)
                    {
                        boat.RemovePilot(m);
                    }
                    else
                    {
                        m.RemoveItem(mountItem);
                        mountItem.Delete();
                    }
                }
                else
                {
                    m.RemoveItem(mountItem);
                    mountItem.Delete();
                }
            }
        }

        public static bool HasBoat(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
                return false;

            return Boats.Any(boat => boat.Owner == from && !boat.Deleted && boat.Map != Map.Internal && !boat.IsRowBoat);
        }

        public static BaseBoat GetBoat(Mobile from)
        {
            return Boats.FirstOrDefault(boat => boat.Owner == from && !boat.Deleted && boat.Map != Map.Internal && !boat.IsRowBoat);
        }

        public static bool IsValidLocation(Point3D p, Map map)
        {
            Rectangle2D[] wrap = GetWrapFor(map);

            for (int i = 0; i < wrap.Length; ++i)
            {
                if (wrap[i].Contains(p))
                    return true;
            }

            return false;
        }

        public static Rectangle2D[] GetWrapFor(Map m)
        {
            if (m == Map.Ilshenar)
                return m_IlshWrap;
            else if (m == Map.Tokuno)
                return m_TokunoWrap;
            else
                return m_BritWrap;
        }

        public static bool IsDriving(Mobile from)
        {
            return Boats.Any(b => b.Pilot == from);
        }

        public static int GetID(int multiID, Direction direction)
        {
            switch (direction)
            {
                default:
                case Direction.North: return multiID;
                case Direction.East: return multiID + 1;
                case Direction.South: return multiID + 2;
                case Direction.West: return multiID + 3;
            }
        }

        #endregion

        public enum DryDockResult { Valid, Dead, NoKey, NotAnchored, Mobiles, Items, Hold, Decaying, NotEnoughGold, Damaged, Addons, Cannon }

        #region Variables

        /*
        * OSI sends the 0xF7 packet instead, holding 0xF3 packets
        * for every entity on the boat. Though, the regular 0xF3
        * packets are still being sent as well as entities come
        * into sight. Do we really need it?
        */
        private Packet m_ContainerPacket;

        public virtual int ZSurface => 0;
        public virtual int RuneOffset => 0;

        private int m_ClientSpeed;
        private Timer m_TurnTimer;
        private string m_ShipName;

        public DamageLevel m_DamageTaken;

        public int m_Hits;
        private Direction m_Facing;

        private Timer m_MoveTimer;
        private DateTime m_DecayTime;
        private bool m_Decaying;

        #endregion

        #region Properties

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Anchored { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BoatCourse BoatCourse { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseDockedBoat BoatItem { get; set; }

        public virtual int DamageValue
        {
            get
            {
                switch (m_DamageTaken)
                {
                    default:
                    case DamageLevel.Pristine:
                    case DamageLevel.Slightly: return 0;
                    case DamageLevel.Moderately:
                    case DamageLevel.Heavily: return 1;
                    case DamageLevel.Severely: return 2;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DamageLevel DamageTaken
        {
            get { return m_DamageTaken; }
            set
            {
                DamageLevel oldDamage = m_DamageTaken;

                m_DamageTaken = value;

                if (m_DamageTaken != oldDamage)
                {
                    if (this is BaseGalleon)
                    {
                        BaseGalleon galleon = this as BaseGalleon;

                        galleon.InvalidateGalleon();

                        if (galleon.GalleonPilot != null)
                        {
                            galleon.GalleonPilot.InvalidateProperties();

                            if (m_DamageTaken == DamageLevel.Severely)
                                galleon.GalleonPilot.Say(1116687); // Arr, we be scuttled!
                        }
                    }
                    else
                    {
                        if (TillerMan != null)
                        {
                            if (TillerMan is Mobile)
                                ((Mobile)TillerMan).InvalidateProperties();
                            else if (TillerMan is Item)
                                ((Item)TillerMan).InvalidateProperties();

                            if (m_DamageTaken == DamageLevel.Severely)
                                TillerManSay(1116687); // Arr, we be scuttled!
                        }
                    }
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Durability => m_Hits / (double)MaxHits * 100.0;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DoesDecay { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Facing { get { return m_Facing; } set { SetFacing(value); } }

        public IEnumerable<Item> ItemsOnBoard => GetEntitiesOnBoard().OfType<Item>();

        public IEnumerable<Mobile> MobilesOnBoard => GetEntitiesOnBoard().OfType<Mobile>();

        public override bool HandlesOnSpeech => true;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Hits { get { return m_Hits; } set { m_Hits = value; ComputeDamage(); InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Hold Hold { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsMoving => (m_MoveTimer != null);

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsTurning => m_TurnTimer != null;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPiloted => Pilot != null;

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Moving { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public MapItem MapItem { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int NextNavPoint { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BoatOrder Order { get; set; }

        public int PlayerCount => MobilesOnBoard.Where(m => m is PlayerMobile).Count();

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Pilot { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Plank PPlank { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Plank SPlank { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string ShipName
        {
            get { return m_ShipName; }
            set
            {
                m_ShipName = value;

                if (TillerMan != null)
                    InvalidateTillerManProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Container SecureContainer { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime TimeOfDecay
        {
            get { return m_DecayTime; }
            set
            {
                m_DecayTime = value;

                if (TillerMan != null && TillerMan is Mobile)
                {
                    ((Mobile)TillerMan).InvalidateProperties();
                }
                else if (TillerMan != null && TillerMan is Item)
                {
                    ((Item)TillerMan).InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public object TillerMan { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BoatMountItem VirtualMount { get; private set; }


        #region Movement Offset
        private Direction Forward => Facing;
        private Direction ForwardLeft => (Facing - 1) & Direction.Mask;
        private Direction ForwardRight => (Facing + 1) & Direction.Mask;
        private Direction Backward => (Facing - 4) & Direction.Mask;
        private Direction BackwardLeft => (Facing - 3) & Direction.Mask;
        private Direction BackwardRight => (Facing + 3) & Direction.Mask;
        private Direction Left => (Facing - 2) & Direction.Mask;
        private Direction Right => (Facing + 2) & Direction.Mask;
        #endregion

        #region Speed

        /*
        * Intervals:
        *       drift forward
        * fast | 0.25|   0.25
        * slow | 0.50|   0.50
        *
        * Speed:
        *       drift forward
        * fast |  0x4|    0x4
        * slow |  0x3|    0x3
        *
        * Tiles (per interval):
        *       drift forward
        * fast |    1|      1
        * slow |    1|      1
        *
        * 'walking' in piloting mode has a 1s interval, speed 0x2
        */

        [CommandProperty(AccessLevel.GameMaster)]
        public int Speed { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FastInterval { get; set; } = 250;

        [CommandProperty(AccessLevel.GameMaster)]
        public int NormalInterval { get; set; } = 500;

        [CommandProperty(AccessLevel.GameMaster)]
        public int SlowInterval { get; set; } = 1000;

        public TimeSpan FastInt => TimeSpan.FromMilliseconds(FastInterval);
        public TimeSpan NormalInt => TimeSpan.FromMilliseconds(NormalInterval);
        public TimeSpan SlowInt => TimeSpan.FromMilliseconds(SlowInterval);

        public TimeSpan FastDriftInterval => NormalInt;
        public TimeSpan SlowDriftInterval => SlowInt;

        #endregion

        #endregion

        #region Virtual Properties

        public virtual int Status
        {
            get
            {
                if (!DoesDecay)
                    return 1043010; // This structure is like new.

                if (m_DecayTime <= DateTime.UtcNow)
                    return 1043015; // This structure is in danger of collapsing.

                TimeSpan decaySpan = m_DecayTime - DateTime.UtcNow;
                int percentWorn = 1000 - (int)((decaySpan.Ticks * 1000) / BoatDecayDelay.Ticks);

                if (percentWorn >= 923) // 92.3% worn
                    return 1043015; // This structure is in danger of collapsing

                if (percentWorn >= 711) // 71.1% worn
                    return 1043014; // This structure is greatly worn.

                if (percentWorn >= 500) // 50.0% worn
                    return 1043013; // This structure is fairly worn.

                if (percentWorn >= 289) // 28.9% worn
                    return 1043012; // This structure is somewhat worn.

                if (percentWorn >= 77) // 7.7% worn
                    return 1043011; // This structure is slightly worn.

                // else 
                return 1043010; // This structure is like new.
            }
        }

        public virtual int NorthID => 0;
        public virtual int EastID => 0;
        public virtual int SouthID => 0;
        public virtual int WestID => 0;

        public virtual int HoldDistance => 0;
        public virtual int TillerManDistance => 0;
        public virtual Point2D StarboardOffset => Point2D.Zero;
        public virtual Point2D PortOffset => Point2D.Zero;
        public virtual Point3D MarkOffset => Point3D.Zero;
        public virtual BaseDockedBoat DockedBoat => null;

        public virtual bool IsClassicBoat => true;
        public virtual bool IsRowBoat => false;
        public virtual bool CanLinkToLighthouse => true;
        public virtual int MaxHits => 25000;
        public virtual double TurnDelay => 0.5;
        public virtual double ScuttleLevel => 25.0;
        public virtual TimeSpan BoatDecayDelay => TimeSpan.FromDays(13);

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Scuttled => !IsUnderEmergencyRepairs() && Durability < ScuttleLevel;

        #endregion

        #region Constructors
        public BaseBoat(Direction direction)
            : this(direction, false)
        {
        }

        public BaseBoat(Direction direction, bool isClassic)
            : base(0x0)
        {
            if (IsRowBoat)
                Timer.DelayCall(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5), RowBoat_Tick_Callback);

            m_DecayTime = DateTime.UtcNow + BoatDecayDelay;
            DoesDecay = true;
            Facing = direction;
            Layer = Layer.Mount;
            Anchored = false;

            m_Hits = MaxHits;
            m_DamageTaken = DamageLevel.Pristine;

            if (isClassic)
            {
                TillerMan = new TillerMan(this);
                PPlank = new Plank(this, PlankSide.Port, 0);
                SPlank = new Plank(this, PlankSide.Starboard, 0);

                PPlank.MoveToWorld(new Point3D(X + PortOffset.X, Y + PortOffset.Y, Z), Map);
                SPlank.MoveToWorld(new Point3D(X + StarboardOffset.X, Y + StarboardOffset.Y, Z), Map);

                Hold = new Hold(this);

                UpdateComponents();
            }

            NextNavPoint = -1;
            Movable = false;
            Boats.Add(this);
        }

        public BaseBoat(Serial serial)
            : base(serial)
        {
        }
        #endregion

        #region IMount Members
        public Mobile Rider { get { return Pilot; } set { Pilot = value; } }

        public void OnRiderDamaged(Mobile from, ref int amount, bool willKill)
        {
        }
        #endregion

        #region Overrides
        public override bool AllowsRelativeDrop => true;
        public override bool ForceShowProperties => true;

        public override bool Contains(int x, int y)
        {
            if (base.Contains(x, y))
                return true;

            if (TillerMan != null)
            {
                if (TillerMan is Mobile)
                {
                    if (x == ((Mobile)TillerMan).X && y == ((Mobile)TillerMan).Y)
                        return true;
                }
                else if (TillerMan is Item)
                {
                    if (x == ((Item)TillerMan).X && y == ((Item)TillerMan).Y)
                        return true;
                }
            }

            if (Hold != null && x == Hold.X && y == Hold.Y)
                return true;

            if (PPlank != null && x == PPlank.X && y == PPlank.Y)
                return true;

            if (SPlank != null && x == SPlank.X && y == SPlank.Y)
                return true;

            return false;
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (CheckDecay())
                return;

            Mobile from = e.Mobile;

            if (CanCommand(from) && Contains(from) && Pilot == null)
            {
                for (int i = 0; i < e.Keywords.Length; ++i)
                {
                    if (e.Handled)
                        break;

                    int keyword = e.Keywords[i];

                    if ((keyword >= 0x42 && keyword <= 0x6B) || keyword == 0xF)
                    {
                        switch (keyword)
                        {
                            case 0x42: SetName(e); break;
                            case 0x43: RemoveName(e.Mobile); break;
                            case 0x44: GiveName(e.Mobile); break;
                            case 0x45: Order = BoatOrder.PlayerControlled; StartMove(Forward, true); break;
                            case 0x46: Order = BoatOrder.PlayerControlled; StartMove(Backward, true); break;
                            case 0x47: Order = BoatOrder.PlayerControlled; StartMove(Left, true); break;
                            case 0x48: Order = BoatOrder.PlayerControlled; StartMove(Right, true); break;
                            case 0x4B: Order = BoatOrder.PlayerControlled; StartMove(ForwardLeft, true); break;
                            case 0x4C: Order = BoatOrder.PlayerControlled; StartMove(ForwardRight, true); break;
                            case 0x4D: Order = BoatOrder.PlayerControlled; StartMove(BackwardLeft, true); break;
                            case 0x4E: Order = BoatOrder.PlayerControlled; StartMove(BackwardRight, true); break;
                            case 0x4F: Order = BoatOrder.PlayerControlled; StopMove(true); break;
                            case 0x50: Order = BoatOrder.PlayerControlled; StartMove(Left, false); break;
                            case 0x51: Order = BoatOrder.PlayerControlled; StartMove(Right, false); break;
                            case 0x52: Order = BoatOrder.PlayerControlled; StartMove(Forward, false); break;
                            case 0x53: Order = BoatOrder.PlayerControlled; StartMove(Backward, false); break;
                            case 0x54: Order = BoatOrder.PlayerControlled; StartMove(ForwardLeft, false); break;
                            case 0x55: Order = BoatOrder.PlayerControlled; StartMove(ForwardRight, false); break;
                            case 0x56: Order = BoatOrder.PlayerControlled; StartMove(BackwardRight, false); break;
                            case 0x57: Order = BoatOrder.PlayerControlled; StartMove(BackwardLeft, false); break;
                            case 0x58: Order = BoatOrder.PlayerControlled; OneMove(Left); break;
                            case 0x59: Order = BoatOrder.PlayerControlled; OneMove(Right); break;
                            case 0x5A: Order = BoatOrder.PlayerControlled; OneMove(Forward); break;
                            case 0x5B: Order = BoatOrder.PlayerControlled; OneMove(Backward); break;
                            case 0x5C: Order = BoatOrder.PlayerControlled; OneMove(ForwardLeft); break;
                            case 0x5D: Order = BoatOrder.PlayerControlled; OneMove(ForwardRight); break;
                            case 0x5E: Order = BoatOrder.PlayerControlled; OneMove(BackwardRight); break;
                            case 0x5F: Order = BoatOrder.PlayerControlled; OneMove(BackwardLeft); break;
                            case 0x49:
                            case 0x65: Order = BoatOrder.PlayerControlled; StartTurn(2, true); break; // turn right
                            case 0x4A:
                            case 0x66: Order = BoatOrder.PlayerControlled; StartTurn(-2, true); break; // turn left
                            case 0x67: Order = BoatOrder.PlayerControlled; StartTurn(-4, true); break; // turn around, come about
                            case 0x68: Order = BoatOrder.PlayerControlled; StartMove(Forward, true); break;
                            case 0x69: Order = BoatOrder.PlayerControlled; StopMove(true); break;
                            case 0x6A: break; // Lower Anchor
                            case 0x6B: break; // Raise Anchor
                            case 0x60: GiveNavPoint(); break; // nav
                            case 0x61: NextNavPoint = 0; StartCourse(false, true); break; // start
                            case 0x62: StartCourse(false, true); break; // continue
                            case 0x63: StartCourse(e.Speech, false, true); break; // goto*
                            case 0x64: StartCourse(e.Speech, true, true); break; // single*
                            case 0xF: TryTrack(from, e.Speech); break;
                        }

                        e.Handled = true;
                        break;
                    }
                }
            }
        }

        public override void OnAfterDelete()
        {
            if (TillerMan != null)
            {
                if (TillerMan is Mobile)
                    ((Mobile)TillerMan).Delete();

                else if (TillerMan is Item)
                    ((Item)TillerMan).Delete();
            }

            if (Hold != null)
                Hold.Delete();

            if (PPlank != null)
                PPlank.Delete();

            if (SPlank != null)
                SPlank.Delete();

            if (m_TurnTimer != null)
                m_TurnTimer.Stop();

            if (m_MoveTimer != null)
                m_MoveTimer.Stop();

            #region High Seas
            if (Owner is BaseShipCaptain)
                ((BaseShipCaptain)Owner).OnShipDelete();

            if (Pilot != null)
                RemovePilot(Pilot);

            if (VirtualMount != null)
                VirtualMount.Delete();
            #endregion

            if (SecureContainer != null)
                SecureContainer.Delete();

            if (BoatItem != null && !BoatItem.Deleted && BoatItem.Map == Map.Internal)
                BoatItem.Delete();

            Boats.Remove(this);

            base.OnAfterDelete();
        }

        public override void OnLocationChange(Point3D old)
        {
            if (TillerMan != null)
            {
                if (TillerMan is Mobile && (Math.Abs(X - old.X) > 1 || Math.Abs(Y - old.Y) > 1))
                    ((Mobile)TillerMan).Location = new Point3D(X + (((Mobile)TillerMan).X - old.X), Y + (((Mobile)TillerMan).Y - old.Y), Z + (((Mobile)TillerMan).Z - old.Z));
                else if (TillerMan is Item)
                    ((Item)TillerMan).Location = new Point3D(X + (((Item)TillerMan).X - old.X), Y + (((Item)TillerMan).Y - old.Y), Z + (((Item)TillerMan).Z - old.Z));
            }

            if (Hold != null)
                Hold.Location = new Point3D(X + (Hold.X - old.X), Y + (Hold.Y - old.Y), Z + (Hold.Z - old.Z));

            if (PPlank != null)
                PPlank.Location = new Point3D(X + (PPlank.X - old.X), Y + (PPlank.Y - old.Y), Z + (PPlank.Z - old.Z));

            if (SPlank != null)
                SPlank.Location = new Point3D(X + (SPlank.X - old.X), Y + (SPlank.Y - old.Y), Z + (SPlank.Z - old.Z));

            #region High Seas
            Region oldReg = Region.Find(old, Map);
            Region newReg = Region.Find(Location, Map);

            if (oldReg != newReg && oldReg is CorgulRegion)
                Timer.DelayCall(TimeSpan.FromSeconds(0.5), new TimerStateCallback(CheckExit), oldReg);
            else if (oldReg != newReg && newReg is CorgulWarpRegion)
                Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback(CheckEnter), newReg);
            #endregion
        }

        public override void OnMapChange()
        {
            if (TillerMan != null)
            {
                if (TillerMan is Mobile)
                    ((Mobile)TillerMan).Map = Map;
                else if (TillerMan is Item)
                    ((Item)TillerMan).Map = Map;
            }

            if (Hold != null)
                Hold.Map = Map;

            if (PPlank != null)
                PPlank.Map = Map;

            if (SPlank != null)
                SPlank.Map = Map;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            int health = Hits * 100 / MaxHits;

            if (health >= 75)
            {
                list.Add(1158886, health.ToString());
            }
            else if (health >= 50)
            {
                list.Add(1158887, health.ToString());
            }
            else if (health >= 25)
            {
                list.Add(1158888, health.ToString());
            }
            else if (health >= 0)
            {
                list.Add(1158889, health.ToString());
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(5);

            writer.Write(m_Hits);
            writer.Write((int)m_DamageTaken);

            if (BoatCourse != null)
            {
                writer.Write(1);
                BoatCourse.Serialize(writer);
            }
            else
                writer.Write(0);

            writer.Write(BoatItem);

            writer.Write(VirtualMount);
            writer.Write(DoesDecay);

            // version 3
            writer.Write(MapItem);
            writer.Write(NextNavPoint);

            writer.Write((int)m_Facing);

            writer.WriteDeltaTime(m_DecayTime);

            writer.Write(Owner);
            writer.Write(PPlank);
            writer.Write(SPlank);

            if (TillerMan is Mobile)
                writer.Write(TillerMan as Mobile);
            else if (TillerMan is Item)
                writer.Write(TillerMan as Item);

            writer.Write(Hold);
            writer.Write(Anchored);
            writer.Write(m_ShipName);

            CheckDecay();
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 5:
                    {
                        m_Hits = reader.ReadInt();
                        m_DamageTaken = (DamageLevel)reader.ReadInt();
                        goto case 4;
                    }
                case 4:
                    {
                        if (reader.ReadInt() == 1)
                        {
                            BoatCourse = new BoatCourse(reader)
                            {
                                Boat = this,
                                Map = Map
                            };
                        }

                        BoatItem = reader.ReadItem() as BaseDockedBoat;
                        VirtualMount = reader.ReadItem() as BoatMountItem;
                        DoesDecay = reader.ReadBool();
                        goto case 3;
                    }
                case 3:
                    {
                        MapItem = (MapItem)reader.ReadItem();
                        NextNavPoint = reader.ReadInt();

                        goto case 2;
                    }
                case 2:
                    {
                        m_Facing = (Direction)reader.ReadInt();

                        goto case 1;
                    }
                case 1:
                    {
                        m_DecayTime = reader.ReadDeltaTime();

                        goto case 0;
                    }
                case 0:
                    {
                        if (version < 3)
                            NextNavPoint = -1;

                        if (version < 2)
                        {
                            if (ItemID == NorthID)
                                m_Facing = Direction.North;
                            else if (ItemID == SouthID)
                                m_Facing = Direction.South;
                            else if (ItemID == EastID)
                                m_Facing = Direction.East;
                            else if (ItemID == WestID)
                                m_Facing = Direction.West;
                        }

                        Owner = reader.ReadMobile();
                        PPlank = reader.ReadItem() as Plank;
                        SPlank = reader.ReadItem() as Plank;

                        if (!IsClassicBoat && !IsRowBoat)
                            TillerMan = reader.ReadMobile() as object;
                        else
                            TillerMan = reader.ReadItem() as object;

                        Hold = reader.ReadItem() as Hold;
                        Anchored = reader.ReadBool();
                        m_ShipName = reader.ReadString();

                        Anchored = false; // No more anchors[High Seas]

                        break;
                    }
            }

            Boats.Add(this);

            if (version == 4)
            {
                Timer.DelayCall(() => Hits = MaxHits);
            }

            if (IsRowBoat)
                Timer.DelayCall(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5), RowBoat_Tick_Callback);
        }

        #endregion

        #region Virtual

        public virtual bool CheckAddon(Item item)
        {
            return false;
        }

        public virtual bool CheckItem(int itemID, Item item, Point3D p)
        {
            return Contains(item) ||
                item is BaseMulti ||
                item.ItemID > TileData.MaxItemValue ||
                !item.Visible ||
                item is Corpse ||
                IsComponentItem(item) ||
                item is EffectItem;
        }

        public virtual bool CanCommand(Mobile m)
        {
            return true;
        }

        public virtual bool CanMoveOver(IEntity entity)
        {
            if (entity is Corpse corpse)
            {
                if (corpse.Owner == null || corpse.Owner is BaseCreature)
                {
                    return true;
                }

                return false;
            }

            return entity is Blood;
        }

        protected virtual bool CheckOnBoard(IEntity e)
        {
            if (e is Item && ((Item)e).IsVirtualItem)
                return false;

            return true;
        }

        /// <summary>
        /// Checks for multi components that aren't 'contained' in the multi itself, ie horizontle masts for the orcish galleon
        /// </summary>
        /// <param name="newPnt">Where the boat is projected to be</param>
        /// <param name="itemID">boat's projectied itemID</param>
        /// <param name="x">object to check x</param>
        /// <param name="y">object to check y</param>
        /// <param name="height">object z + itemdata height</param>
        public virtual bool ExemptOverheadComponent(Point3D newPnt, int itemID, int x, int y, int height)
        {
            return false;
        }

        public virtual IEnumerable<IEntity> GetComponents()
        {
            yield break;
        }

        public virtual Point3D GetMarkedLocation()
        {
            Point3D p = new Point3D(X + MarkOffset.X, Y + MarkOffset.Y, Z + MarkOffset.Z);

            return Rotate(p, (int)m_Facing / 2);
        }

        public virtual bool IsEnemy(BaseBoat boat)
        {
            if (Map != null && Map.Rules == MapRules.FeluccaRules)
                return true;

            Mobile thisOwner = Owner;
            Mobile themOwner = boat.Owner;

            if (thisOwner == null || themOwner == null)
                return true;

            if (thisOwner is PlayerMobile && themOwner is PlayerMobile && Map != Map.Felucca)
                return false;

            return thisOwner.CanBeHarmful(themOwner, false);
        }

        public virtual bool IsEnemy(Mobile from)
        {
            if (Map != null && Map.Rules == MapRules.FeluccaRules)
                return true;

            Mobile thisOwner = Owner;

            if (thisOwner == null || from == null || thisOwner is BaseCreature || from is BaseCreature)
                return true;

            return from.CanBeHarmful(thisOwner, false);
        }

        public virtual bool IsOwner(Mobile from)
        {
            if (from == null)
                return false;

            if (from.AccessLevel > AccessLevel.Player || from == Owner)
                return true;

            if (Owner == null)
                return false;

            Accounting.Account acct1 = from.Account as Accounting.Account;
            Accounting.Account acct2 = Owner.Account as Accounting.Account;

            return acct1 != null && acct2 != null && acct1 == acct2;
        }

        public virtual bool IsComponentItem(IEntity item)
        {
            if (item == null)
                return false;

            if (item == this || (TillerMan is Item && item == (Item)TillerMan) || item == SPlank || item == PPlank || item == Hold)
                return true;
            return false;
        }

        public virtual bool IsExcludedTile(StaticTile tile)
        {
            return false;
        }

        public virtual bool IsExcludedTile(StaticTile[] tile)
        {
            return false;
        }

        public virtual IEnumerable<IEntity> GetEntitiesOnBoard()
        {
            Map map = Map;

            if (map == null || map == Map.Internal)
                yield break;

            MultiComponentList mcl = Components;
            IPooledEnumerable eable = map.GetObjectsInBounds(new Rectangle2D(X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height));

            foreach (IEntity ent in eable)
            {
                if (Contains(ent) && CheckOnBoard(ent))
                {
                    yield return ent;
                }
            }

            eable.Free();
        }

        public virtual bool HasAccess(Mobile from)
        {
            return true;
        }

        public virtual void OnTakenDamage(int damage)
        {
            OnTakenDamage(null, damage);
        }

        public virtual void OnTakenDamage(Mobile damager, int damage)
        {
            Hits -= damage;

            if (damager != null)
                SendDamagePacket(damager, damage);

            if (Hits < 0)
                Hits = 0;

            if (Hits > MaxHits)
                Hits = MaxHits;
        }

        public virtual void OnMousePilotCommand(Mobile from, Direction d, int rawSpeed)
        {
            Order = BoatOrder.PlayerControlled;

            int actualDir = (m_Facing - d) & 0x7;
            TimeSpan interval = GetMovementInterval(rawSpeed, out int clientSpeed);

            if (rawSpeed > 1 && actualDir > 1 && actualDir != 7)
            {
                Direction turnDirection = (int)d % 2 != 0 ? d - 1 : d;
                StartTurn((int)turnDirection, false);
            }

            if (!StartMove(d, 1, clientSpeed, interval, false, false))
                StopMove(false);
        }

        public virtual void SendDamagePacket(Mobile from, int amount)
        {
            if (amount == 0)
                return;

            NetState theirState = (from == null ? null : from.NetState);

            if (theirState == null && from != null)
            {
                Mobile master = from.GetDamageMaster(null);

                if (master != null)
                {
                    theirState = master.NetState;
                }
            }

            if (theirState != null)
            {
                theirState.Send(new DamagePacket(this, amount));
            }
        }

        public virtual void SetFacingComponents(Direction newDirection, Direction oldDirection, bool ignoreLastFacing)
        {
        }

        public virtual void OnPlacement(Mobile from)
        {
        }

        public virtual void OnAfterPlacement(bool initial)
        {
        }

        public virtual void UpdateComponents()
        {
            if (PPlank != null)
            {
                PPlank.MoveToWorld(GetRotatedLocation(PortOffset.X, PortOffset.Y), Map);
                PPlank.SetFacing(m_Facing);
            }

            if (SPlank != null)
            {
                SPlank.MoveToWorld(GetRotatedLocation(StarboardOffset.X, StarboardOffset.Y), Map);
                SPlank.SetFacing(m_Facing);
            }

            int xOffset = 0, yOffset = 0;
            Movement.Movement.Offset(m_Facing, ref xOffset, ref yOffset);

            if (TillerMan != null)
            {
                if (TillerMan is TillerMan tillerman)
                {
                    tillerman.Location = new Point3D(X + (xOffset * TillerManDistance) + (m_Facing == Direction.North ? 1 : 0), Y + (yOffset * TillerManDistance), tillerman.Z);
                    tillerman.SetFacing(m_Facing);
                    tillerman.InvalidateProperties();
                }
            }

            if (Hold != null)
            {
                Hold.Location = new Point3D(X + (xOffset * HoldDistance), Y + (yOffset * HoldDistance), Hold.Z);
                Hold.SetFacing(m_Facing);
            }
        }

        #endregion

        #region BoatCourses

        public void GiveNavPoint()
        {
            if (TillerMan == null || CheckDecay())
                return;

            if (NextNavPoint < 0)
                TillerManSay(1042882); // I have no current nav point.
            else
                TillerManSay(1042883, (NextNavPoint + 1).ToString()); // My current destination navpoint is nav ~1_NAV_POINT_NUM~.
        }

        public void AssociateMap(MapItem map)
        {
            if (CheckDecay())
                return;

            if (map is BlankMap)
            {
                if (TillerMan != null)
                    TillerManSay(502575); // Ar, that is not a map, tis but a blank piece of paper!
            }
            else if (map.Pins.Count == 0)
            {
                if (TillerMan != null)
                    TillerManSay(502576); // Arrrr, this map has no course on it!
            }
            else
            {
                StopMove(false);

                MapItem = map;
                NextNavPoint = 0;

                BoatCourse = new BoatCourse(this, map);

                if (TillerMan != null)
                    TillerManSay(502577); // A map!
            }
        }

        public bool StartCourse(string navPoint, bool single, bool message)
        {
            int number = -1;

            int start = -1;
            for (int i = 0; i < navPoint.Length; i++)
            {
                if (char.IsDigit(navPoint[i]))
                {
                    start = i;
                    break;
                }
            }

            if (start != -1)
            {
                string sNumber = navPoint.Substring(start);

                if (!int.TryParse(sNumber, out number))
                    number = -1;

                if (number != -1)
                {
                    number--;

                    if (BoatCourse == null || number < 0 || number >= BoatCourse.Waypoints.Count)
                    {
                        number = -1;
                    }
                }
            }

            if (number == -1)
            {
                if (message && TillerMan != null)
                    TillerManSay(1042551); // I don't see that navpoint, sir.

                return false;
            }

            NextNavPoint = number;
            return StartCourse(single, message);
        }

        public bool StartCourse(bool single, bool message)
        {
            if (CheckDecay())
                return false;

            if (BoatCourse == null)
            {
                if (message && TillerMan != null)
                    TillerManSay(502513); // I have seen no map, sir.

                return false;
            }
            else if (Map != BoatCourse.Map)
            {
                if (message && TillerMan != null)
                    TillerManSay(502514); // The map is too far away from me, sir.

                return false;
            }
            else if ((Map != Map.Trammel && Map != Map.Felucca && Map != Map.Tokuno) || NextNavPoint < 0 || NextNavPoint >= BoatCourse.Waypoints.Count)
            {
                if (message && TillerMan != null)
                    TillerManSay(1042551); // I don't see that navpoint, sir.

                return false;
            }

            Speed = Owner is BaseShipCaptain ? SlowSpeed : FastSpeed;
            Order = single ? BoatOrder.CourseSingle : BoatOrder.CourseFull;

            if (m_MoveTimer != null)
                m_MoveTimer.Stop();

            m_MoveTimer = new MoveTimer(this, FastInt, false);
            m_MoveTimer.Start();

            if (message && TillerMan != null)
                TillerManSay(501429); // Aye aye sir.

            return true;
        }

        #endregion

        #region Docking

        public virtual DryDockResult CheckDryDock(Mobile from, Mobile dockmaster)
        {
            if (CheckDecay())
                return DryDockResult.Decaying;

            if (!from.Alive)
                return DryDockResult.Dead;

            if (DamageTaken != DamageLevel.Pristine)
                return DryDockResult.Damaged;

            Container pack = from.Backpack;

            if (IsClassicBoat)
            {
                if ((SPlank == null || !Key.ContainsKey(pack, SPlank.KeyValue)) && (PPlank == null || !Key.ContainsKey(pack, PPlank.KeyValue)))
                    return DryDockResult.NoKey;

                //if (!m_Anchored)
                //    return DryDockResult.NotAnchored;
            }

            if (Hold != null && Hold.Items.Count > 0)
                return DryDockResult.Hold;

            Map map = Map;

            if (map == null || map == Map.Internal)
                return DryDockResult.Items;

            DryDockResult res = DryDockResult.Valid;

            foreach (IEntity o in GetEntitiesOnBoard())
            {
                if (o == this || IsComponentItem(o) || o is EffectItem || o == TillerMan)
                    continue;

                if (o is Item && Contains((Item)o))
                {
                    if (o is BaseAddon)
                        res = DryDockResult.Addons;
                    else
                        res = DryDockResult.Items;

                    break;
                }
                else if (o is Mobile && Contains((Mobile)o) && ((Mobile)o).AccessLevel == AccessLevel.Player)
                {
                    res = DryDockResult.Mobiles;
                    break;
                }
            }

            return res;
        }

        public virtual void OnDryDock(Mobile from)
        {
            LighthouseAddon addon = LighthouseAddon.GetLighthouse(Owner);

            if (addon != null && Owner != null && Owner.NetState != null)
            {
                Owner.SendLocalizedMessage(1154594); // You have unlinked your ship from your lighthouse.
            }
        }

        public void BeginDryDock(Mobile from)
        {
            BeginDryDock(from, null);
        }

        public void BeginDryDock(Mobile from, Mobile dockmaster)
        {
            if (CheckDecay())
                return;

            DryDockResult result = CheckDryDock(from, dockmaster);

            if (result == DryDockResult.Dead)
                from.SendLocalizedMessage(502493); // You appear to be dead.
            else if (result == DryDockResult.NoKey)
                from.SendLocalizedMessage(502494); // You must have a key to the ship to dock the boat.
            else if (result == DryDockResult.Mobiles)
                from.SendLocalizedMessage(502495); // You cannot dock the ship with beings on board!
            else if (result == DryDockResult.Items || result == DryDockResult.Addons)
                from.SendLocalizedMessage(502496); // You cannot dock the ship with a cluttered deck.
            else if (result == DryDockResult.Hold)
                from.SendLocalizedMessage(502497); // Make sure your hold is empty, and try again!
            else if (result == DryDockResult.NotEnoughGold)
                from.SendLocalizedMessage(1116506, DockMaster.DryDockAmount.ToString()); //The price is ~1_price~ and I will accept nothing less!
            else if (result == DryDockResult.Damaged)
                from.SendLocalizedMessage(1116324); // The ship must be fully repaired before it can be docked!
            else if (result == DryDockResult.Cannon)
                from.SendLocalizedMessage(1116323);  // You cannot dock the ship with loaded weapons on deck!
            else if (result == DryDockResult.Valid)
                from.SendGump(new ConfirmDryDockGump(from, this, dockmaster));
        }

        public void EndDryDock(Mobile from, Mobile dockmaster)
        {
            if (Deleted || CheckDecay())
                return;

            DryDockResult result = CheckDryDock(from, dockmaster);

            if (result == DryDockResult.Dead)
                from.SendLocalizedMessage(502493); // You appear to be dead.
            else if (result == DryDockResult.NoKey)
                from.SendLocalizedMessage(502494); // You must have a key to the ship to dock the boat.
            //else if ( result == DryDockResult.NotAnchored )
            //	from.SendLocalizedMessage( 1010570 ); // You must lower the anchor to dock the boat.
            else if (result == DryDockResult.Mobiles)
                from.SendLocalizedMessage(502495); // You cannot dock the ship with beings on board!
            else if (result == DryDockResult.Items || result == DryDockResult.Addons)
                from.SendLocalizedMessage(502496); // You cannot dock the ship with a cluttered deck.
            else if (result == DryDockResult.Hold)
                from.SendLocalizedMessage(502497); // Make sure your hold is empty, and try again!
            else if (result == DryDockResult.NotEnoughGold)
                from.SendLocalizedMessage(1116506, DockMaster.DryDockAmount.ToString()); // The price is ~1_price~ and I will accept nothing less!
            else if (result == DryDockResult.Damaged)
                from.SendLocalizedMessage(1116324); // The ship must be fully repaired before it can be docked!
            else if (result == DryDockResult.Cannon)
                from.SendLocalizedMessage(1116323); // You cannot dock the ship with loaded weapons on deck!

            if (result != DryDockResult.Valid)
                return;

            BaseDockedBoat boat = BoatItem;

            if (boat == null || boat.Deleted)
                boat = DockedBoat;

            if (boat == null)
                return;

            if (IsRowBoat)
            {
                Delete();
            }
            else
            {
                boat.BoatItem = this;

                if (IsClassicBoat)
                    RemoveKeys(from);

                from.AddToBackpack(boat);

                Refresh();
                Internalize();

                OnDryDock(from);
            }
        }

        #endregion

        #region Decay

        public virtual void OnSink()
        {
            m_Decaying = false;

            if (CanLinkToLighthouse)
            {
                LighthouseAddon addon = LighthouseAddon.GetLighthouse(Owner);

                if (addon != null)
                {
                    BaseHouse house = BaseHouse.FindHouseAt(addon);

                    if (house != null)
                    {
                        addon.DockBoat(this, house);
                        return;
                    }
                }
            }

            Delete();
        }

        public bool CheckDecay()
        {
            if (Map == Map.Internal)
                return false;

            if (PlayerCount > 0)
                return false;

            if (m_Decaying)
                return true;

            if (DoesDecay && !IsMoving && DateTime.UtcNow >= m_DecayTime)
            {
                new DecayTimer(this).Start();

                m_Decaying = true;

                return true;
            }

            return false;
        }

        public void ForceDecay()
        {
            new DecayTimer(this).Start();
            m_Decaying = true;
        }

        private class DecayTimer : Timer
        {
            private readonly BaseBoat m_Boat;
            private int m_Count;

            public DecayTimer(BaseBoat boat)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(5.0))
            {
                m_Boat = boat;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Count == 5)
                {
                    m_Boat.OnSink();
                    Stop();
                }
                else
                {
                    //m_Boat.Location = new Point3D(m_Boat.X, m_Boat.Y, m_Boat.Z - 1);
                    m_Boat.Teleport(0, 0, -1);

                    if (m_Boat.TillerMan != null)
                        m_Boat.TillerManSay(1007168 + m_Count);

                    ++m_Count;
                }
            }
        }

        #endregion

        #region Keys

        public uint CreateKeys(Mobile m)
        {
            uint value = Key.RandomValue();

            Key packKey = new Key(KeyType.Gold, value, this);
            Key bankKey = new Key(KeyType.Gold, value, this);

            packKey.MaxRange = 10;
            bankKey.MaxRange = 10;

            packKey.Name = "a ship key";
            bankKey.Name = "a ship key";

            BankBox box = m.BankBox;

            if (!box.TryDropItem(m, bankKey, false))
                bankKey.Delete();
            else
                m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502484); // A ship's key is now in my safety deposit box.

            if (m.AddToBackpack(packKey))
                m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502485); // A ship's key is now in my backpack.
            else
                m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502483); // A ship's key is now at my feet.

            return value;
        }

        public bool CheckKey(uint keyValue)
        {
            if (SPlank != null && SPlank.KeyValue == keyValue)
                return true;

            if (PPlank != null && PPlank.KeyValue == keyValue)
                return true;

            return false;
        }

        public void RemoveKeys(Mobile m)
        {
            uint keyValue = 0;

            if (PPlank != null)
                keyValue = PPlank.KeyValue;

            if (keyValue == 0 && SPlank != null)
                keyValue = SPlank.KeyValue;

            Key.RemoveKeys(m, keyValue);
        }

        #endregion

        #region Repairs
        private EmergencyRepairDamageTimer m_EmergencyRepairTimer;

        public bool IsUnderEmergencyRepairs()
        {
            return m_EmergencyRepairTimer != null;
        }

        public TimeSpan GetEndEmergencyRepairs()
        {
            if (m_EmergencyRepairTimer != null && m_EmergencyRepairTimer.EndRepairs > DateTime.UtcNow)
                return m_EmergencyRepairTimer.EndRepairs - DateTime.UtcNow;

            return TimeSpan.Zero;
        }

        public bool TryEmergencyRepair(Mobile from)
        {
            if (from == null || from.Backpack == null)
                return false;

            int clothNeeded = EmergencyRepairClothCost;
            int woodNeeded = EmergencyRepairWoodCost;
            Container pack = from.Backpack;
            Container hold = this is BaseGalleon ? ((BaseGalleon)this).GalleonHold : null;
            TimeSpan ts = EmergencyRepairSpan;

            int wood1 = pack.GetAmount(typeof(Board));
            int wood2 = pack.GetAmount(typeof(Log));
            int wood3 = 0; int wood4 = 0;

            int cloth1 = pack.GetAmount(typeof(Cloth));
            int cloth2 = pack.GetAmount(typeof(UncutCloth));
            int cloth3 = 0; int cloth4 = 0;

            if (hold != null)
            {
                wood3 = hold.GetAmount(typeof(Board));
                wood4 = hold.GetAmount(typeof(Log));
                cloth3 = hold.GetAmount(typeof(Cloth));
                cloth4 = hold.GetAmount(typeof(UncutCloth));
            }

            int totalWood = wood1 + wood2 + wood3 + wood4;
            int totalCloth = cloth1 + cloth2 + cloth3 + cloth4;

            if (totalWood >= woodNeeded && totalCloth >= clothNeeded)
            {
                int toConsume = 0;

                if (woodNeeded > 0 && wood1 > 0)
                {
                    toConsume = Math.Min(woodNeeded, wood1);
                    pack.ConsumeTotal(typeof(Board), toConsume);
                    woodNeeded -= toConsume;
                }
                if (woodNeeded > 0 && wood2 > 0)
                {
                    toConsume = Math.Min(woodNeeded, wood2);
                    pack.ConsumeTotal(typeof(Log), toConsume);
                    woodNeeded -= toConsume;
                }
                if (hold != null && woodNeeded > 0 && wood3 > 0)
                {
                    toConsume = Math.Min(woodNeeded, wood3);
                    hold.ConsumeTotal(typeof(Board), toConsume);
                    woodNeeded -= toConsume;
                }
                if (hold != null && woodNeeded > 0 && wood4 > 0)
                {
                    toConsume = Math.Min(woodNeeded, wood4);
                    hold.ConsumeTotal(typeof(Log), toConsume);
                }
                if (clothNeeded > 0 && cloth1 > 0)
                {
                    toConsume = Math.Min(clothNeeded, cloth1);
                    pack.ConsumeTotal(typeof(Cloth), toConsume);
                    clothNeeded -= toConsume;
                }
                if (clothNeeded > 0 && cloth2 > 0)
                {
                    toConsume = Math.Min(clothNeeded, cloth2);
                    pack.ConsumeTotal(typeof(UncutCloth), toConsume);
                    clothNeeded -= toConsume;
                }
                if (hold != null && clothNeeded > 0 && cloth3 > 0)
                {
                    toConsume = Math.Min(clothNeeded, cloth3);
                    hold.ConsumeTotal(typeof(Cloth), toConsume);
                    clothNeeded -= toConsume;
                }
                if (hold != null && clothNeeded > 0 && cloth4 > 0)
                {
                    toConsume = Math.Min(clothNeeded, cloth4);
                    hold.ConsumeTotal(typeof(UncutCloth), toConsume);
                }

                from.SendLocalizedMessage(1116592, ts.TotalMinutes.ToString()); // Your ship is underway with emergency repairs holding for an estimated ~1_TIME~ more minutes.                
                m_EmergencyRepairTimer = new EmergencyRepairDamageTimer(this, ts);

                return true;
            }
            return false;
        }

        public void EndEmergencyRepairEffects()
        {
            m_EmergencyRepairTimer = null;

            SendMessageToAllOnBoard(1116765);  // The emergency repairs have given out!
        }

        public void TryRepairs(Mobile from)
        {
            if (from == null || from.Backpack == null)
                return;

            Container pack = from.Backpack;
            Container hold = this is BaseGalleon ? ((BaseGalleon)this).GalleonHold : null;
            Container secure = SecureContainer;

            double wood = 0;
            double cloth = 0;

            for (int i = 0; i < WoodTypes.Length; i++)
            {
                Type type = WoodTypes[i];
                if (pack != null) wood += pack.GetAmount(type);
                if (hold != null) wood += hold.GetAmount(type);
                if (secure != null) wood += secure.GetAmount(type);
            }

            for (int i = 0; i < ClothTypes.Length; i++)
            {
                Type type = ClothTypes[i];
                if (pack != null) cloth += pack.GetAmount(type);
                if (hold != null) cloth += hold.GetAmount(type);
                if (secure != null) cloth += secure.GetAmount(type);
            }

            double durability = (Hits / (double)MaxHits) * 100;

            //Now, how much do they need for 100% repair
            double woodNeeded = WoodPer * (100.0 - durability);
            double clothNeeded = ClothPer * (100.0 - durability);

            //Apply skill bonus
            woodNeeded -= (from.Skills[SkillName.Carpentry].Value / 200.0) * woodNeeded;
            clothNeeded -= (from.Skills[SkillName.Tailoring].Value / 200.0) * clothNeeded;

            //get 10% of needed repairs
            double minWood = woodNeeded / 10;
            double minCloth = clothNeeded / 10;

            if (wood < minWood || cloth < minCloth)
            {
                from.SendLocalizedMessage(1116593, string.Format("{0}\t{1}", ((int)minCloth).ToString(), ((int)minWood).ToString())); //You need a minimum of ~1_CLOTH~ yards of cloth and ~2_WOOD~ pieces of lumber to effect repairs to this ship.
                return;
            }

            double percWood, percCloth, woodUsed, clothUsed;

            if (wood >= woodNeeded)
            {
                woodUsed = woodNeeded;
                percWood = 100;
            }
            else
            {
                woodUsed = wood;
                percWood = wood / woodNeeded * 100;
            }

            if (cloth >= clothNeeded)
            {
                clothUsed = clothNeeded;
                percCloth = 100;
            }
            else
            {
                clothUsed = cloth;
                percCloth = cloth / clothNeeded * 100;
            }

            if (clothUsed > woodUsed)
            {
                clothUsed = woodUsed;
                percCloth = percWood;
            }
            else if (woodUsed > clothUsed)
            {
                woodUsed = clothUsed;
                percWood = percCloth;
            }

            double totalPerc = (percWood + percCloth) / 2;

            double toConsume = 0;
            double woodTemp = woodUsed;
            double clothTemp = clothUsed;

            #region Consume
            for (int i = 0; i < WoodTypes.Length; i++)
            {
                Type type = WoodTypes[i];

                if (woodUsed <= 0)
                    break;

                if (woodUsed > 0 && pack.GetAmount(type) > 0)
                {
                    toConsume = Math.Min(woodUsed, pack.GetAmount(type));
                    pack.ConsumeTotal(type, (int)toConsume);
                    woodUsed -= toConsume;
                }
                if (hold != null && woodUsed > 0 && hold.GetAmount(type) > 0)
                {
                    toConsume = Math.Min(woodUsed, hold.GetAmount(type));
                    hold.ConsumeTotal(type, (int)toConsume);
                    woodUsed -= toConsume;
                }
                if (secure != null && woodUsed > 0 && secure.GetAmount(type) > 0)
                {
                    toConsume = Math.Min(woodUsed, secure.GetAmount(type));
                    secure.ConsumeTotal(type, (int)toConsume);
                    woodUsed -= toConsume;
                }
            }

            for (int i = 0; i < ClothTypes.Length; i++)
            {
                Type type = ClothTypes[i];

                if (clothUsed <= 0)
                    break;

                if (clothUsed > 0 && pack.GetAmount(type) > 0)
                {
                    toConsume = Math.Min(clothUsed, pack.GetAmount(type));
                    pack.ConsumeTotal(type, (int)toConsume);
                    clothUsed -= toConsume;
                }
                if (hold != null && clothUsed > 0 && hold.GetAmount(type) > 0)
                {
                    toConsume = Math.Min(clothUsed, hold.GetAmount(type));
                    hold.ConsumeTotal(type, (int)toConsume);
                    clothUsed -= toConsume;
                }
                if (secure != null && clothUsed > 0 && secure.GetAmount(type) > 0)
                {
                    toConsume = Math.Min(clothUsed, secure.GetAmount(type));
                    secure.ConsumeTotal(type, (int)toConsume);
                    clothUsed -= toConsume;
                }
            }
            #endregion

            m_Hits += (int)((MaxHits - m_Hits) * (totalPerc / 100));

            if (m_Hits > MaxHits)
                m_Hits = MaxHits;

            ComputeDamage();

            if (totalPerc > 100)
                totalPerc = 100;

            if (m_EmergencyRepairTimer != null)
            {
                m_EmergencyRepairTimer.Stop();
                m_EmergencyRepairTimer = null;
            }

            string args = string.Format("{0}\t{1}\t{2}", ((int)clothTemp).ToString(), ((int)woodTemp).ToString(), ((int)Durability).ToString());
            from.SendLocalizedMessage(1116598, args); //You effect permanent repairs using ~1_CLOTH~ yards of cloth and ~2_WOOD~ pieces of lumber. The ship is now ~3_DMGPCT~% repaired.
        }

        private class EmergencyRepairDamageTimer : Timer
        {
            private readonly BaseBoat m_Boat;

            public DateTime EndRepairs { get; }

            public EmergencyRepairDamageTimer(BaseBoat boat, TimeSpan duration)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                m_Boat = boat;
                EndRepairs = DateTime.UtcNow + duration;
                Start();
            }

            protected override void OnTick()
            {
                if (m_Boat == null)
                {
                    Stop();
                    return;
                }

                if (EndRepairs < DateTime.UtcNow)
                {
                    m_Boat.EndEmergencyRepairEffects();
                    Stop();
                }
            }
        }
        #endregion

        #region Naming

        public void SetName(SpeechEventArgs e)
        {
            if (CheckDecay())
                return;

            if (e.Mobile.AccessLevel < AccessLevel.GameMaster && e.Mobile != Owner)
            {
                if (TillerMan != null)
                    TillerManSay(1042880); // Arr! Only the owner of the ship may change its name!

                return;
            }
            else if (!e.Mobile.Alive)
            {
                if (TillerMan != null)
                    TillerManSay(502582); // You appear to be dead.

                return;
            }

            if (e.Speech.Length > 8)
            {
                string newName = e.Speech.Substring(8).Trim();

                if (newName.Length == 0)
                    newName = null;

                Rename(newName);
            }
        }

        public void Rename(string newName)
        {
            if (CheckDecay())
                return;

            if (newName != null && newName.Length > 40)
                newName = newName.Substring(0, 40);

            if (m_ShipName == newName)
            {
                if (TillerMan != null)
                    TillerManSay(502531); // Yes, sir.

                return;
            }

            ShipName = newName;

            if (TillerMan != null && m_ShipName != null)
            {
                TillerManSay(1042885, m_ShipName); // This ship is now called the ~1_NEW_SHIP_NAME~.
            }
            else if (TillerMan != null)
            {
                TillerManSay(502534); // This ship now has no name.
            }
        }

        public void RemoveName(Mobile m)
        {
            if (CheckDecay())
                return;

            if (m.AccessLevel < AccessLevel.GameMaster && m != Owner)
            {
                if (TillerMan != null)
                    TillerManSay(1042880); // Arr! Only the owner of the ship may change its name!

                return;
            }
            else if (!m.Alive)
            {
                if (TillerMan != null)
                    TillerManSay(502582); // You appear to be dead.

                return;
            }

            if (m_ShipName == null)
            {
                if (TillerMan != null)
                    TillerManSay(502526); // Ar, this ship has no name.

                return;
            }

            ShipName = null;

            if (TillerMan != null)
                TillerManSay(502534); // This ship now has no name.
        }

        public void GiveName(Mobile m)
        {
            if (TillerMan == null || CheckDecay())
                return;

            if (m_ShipName == null)
                TillerManSay(502526); // Ar, this ship has no name.
            else
                TillerManSay(1042881, m_ShipName); // This is the ~1_BOAT_NAME~.
        }

        public void BeginRename(Mobile from)
        {
            if (CheckDecay())
                return;

            if (from.AccessLevel < AccessLevel.GameMaster && from != Owner)
            {
                if (TillerMan != null)
                    TillerManSay(Utility.Random(1042876, 4)); // Arr, don't do that! | Arr, leave me alone! | Arr, watch what thour'rt doing, matey! | Arr! Do that again and I’ll throw ye overhead!

                return;
            }

            if (TillerMan != null)
                TillerManSay(502580); // What dost thou wish to name thy ship?

            from.Prompt = new RenameBoatPrompt(this);
        }

        public void EndRename(Mobile from, string newName)
        {
            if (Deleted || CheckDecay())
                return;

            if (from.AccessLevel < AccessLevel.GameMaster && from != Owner)
            {
                if (TillerMan != null)
                    TillerManSay(1042880); // Arr! Only the owner of the ship may change its name!

                return;
            }
            else if (!from.Alive)
            {
                if (TillerMan != null)
                    TillerManSay(502582); // You appear to be dead.

                return;
            }

            newName = newName.Trim();

            if (!Guilds.BaseGuildGump.CheckProfanity(newName))
            {
                from.SendMessage("That name is unacceptable.");
                return;
            }

            if (newName.Length == 0)
                newName = null;

            Rename(newName);
        }

        public void InvalidateTillerManProperties()
        {
            if (TillerMan is Mobile)
                ((Mobile)TillerMan).InvalidateProperties();
            else if (TillerMan is Item)
                ((Item)TillerMan).InvalidateProperties();
        }

        #endregion

        #region Movement

        public virtual bool SetFacing(Direction facing)
        {
            if (Parent != null || Map == null || CheckDecay())
                return false;

            if (Map != Map.Internal)
            {
                switch (facing)
                {
                    default:
                    case Direction.North: if (!CanFit(Location, Map, NorthID)) return false; break;
                    case Direction.East: if (!CanFit(Location, Map, EastID)) return false; break;
                    case Direction.South: if (!CanFit(Location, Map, SouthID)) return false; break;
                    case Direction.West: if (!CanFit(Location, Map, WestID)) return false; break;
                }
            }

            Map.OnLeave(this);

            Direction old = m_Facing;
            List<IEntity> toMove = new List<IEntity>();

            if (TillerMan is GalleonPilot)
                ((GalleonPilot)TillerMan).SetFacing(facing);
            else if (TillerMan is TillerMan)
                ((TillerMan)TillerMan).SetFacing(facing);

            if (Hold != null)
                Hold.SetFacing(facing);

            if (PPlank != null)
            {
                PPlank.SetFacing(facing);
                toMove.Add(PPlank);
            }

            if (SPlank != null)
            {
                SPlank.SetFacing(facing);
                toMove.Add(SPlank);
            }

            MultiComponentList mcl = Components;

            IPooledEnumerable eable = Map.GetObjectsInBounds(new Rectangle2D(X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height));

            foreach (IEntity o in eable)
            {
                if (o is Item item)
                {
                    if (CanMoveOver(item) || item is BaseAddon || (item is AddonComponent && ((AddonComponent)item).Addon != null))
                        continue;

                    if (item != this && Contains(item) && item.Visible && item.Z >= Z && !(item is TillerMan || item is Hold || item is Plank || item is RudderHandle))
                        toMove.Add(o);
                }
                else if (o is Mobile && Contains((Mobile)o))
                {
                    toMove.Add(o);
                    ((Mobile)o).Direction = (Direction)((int)((Mobile)o).Direction - (int)old + (int)facing);
                }
            }

            foreach (IEntity comp in GetComponents().Where(comp => !toMove.Contains(comp)))
            {
                toMove.Add(comp);
            }

            eable.Free();

            m_Facing = facing;
            int xOffset = 0, yOffset = 0;
            Movement.Movement.Offset(facing, ref xOffset, ref yOffset);

            if (TillerMan is Item)
                ((Item)TillerMan).Location = new Point3D(X + (xOffset * TillerManDistance) + (facing == Direction.North ? 1 : 0), Y + (yOffset * TillerManDistance), ((Item)TillerMan).Z);

            if (Hold != null)
                Hold.Location = new Point3D(X + (xOffset * HoldDistance), Y + (yOffset * HoldDistance), Hold.Z);

            int count = m_Facing - old & 0x7;
            count /= 2;

            foreach (IEntity e in toMove.Where(e => e != null))
            {
                e.Location = Rotate(e.Location, count);
            }

            switch (facing)
            {
                default:
                case Direction.North: ItemID = NorthID; break;
                case Direction.East: ItemID = EastID; break;
                case Direction.South: ItemID = SouthID; break;
                case Direction.West: ItemID = WestID; break;
            }

            SetFacingComponents(m_Facing, old, false);

            Map.OnEnter(this);

            ColUtility.Free(toMove);

            return true;
        }

        public virtual TimeSpan GetMovementInterval(int speed, out int clientSpeed)
        {
            switch (speed)
            {
                default: clientSpeed = 0x0; return TimeSpan.Zero;
                case 1: clientSpeed = 0x2; return SlowInt;
                case 2: clientSpeed = 0x4; return FastInt;
            }
        }

        public virtual TimeSpan GetMovementInterval(bool fast, out int clientSpeed)
        {
            if (fast)
            {
                clientSpeed = 0x4;
                return FastInt;
            }
            else
            {
                clientSpeed = 0x3;
                return NormalInt;
            }
        }

        public bool CanFit(Point3D p, Map map, int itemID)
        {
            if (map == null || map == Map.Internal || Deleted || CheckDecay())
                return false;

            MultiComponentList newComponents = MultiData.GetComponents(itemID);

            for (int x = 0; x < newComponents.Width; ++x)
            {
                for (int y = 0; y < newComponents.Height; ++y)
                {
                    int tx = p.X + newComponents.Min.X + x;
                    int ty = p.Y + newComponents.Min.Y + y;

                    if (newComponents.Tiles[x][y].Length == 0 || Contains(tx, ty) || IsExcludedTile(newComponents.Tiles[x][y]))
                        continue;

                    LandTile landTile = map.Tiles.GetLandTile(tx, ty);
                    StaticTile[] tiles = map.Tiles.GetStaticTiles(tx, ty, true);

                    bool hasWater = false;
                    int dif = Math.Abs(landTile.Z - p.Z);

                    if (dif >= 0 && dif <= 1 && ((landTile.ID >= 168 && landTile.ID <= 171) || (landTile.ID >= 310 && landTile.ID <= 311)))
                        hasWater = true;

                    for (int i = 0; i < tiles.Length; ++i)
                    {
                        StaticTile tile = tiles[i];

                        if (IsExcludedTile(tile))
                            continue;

                        bool isWater = tile.ID >= 0x1796 && tile.ID <= 0x17B2;

                        if (tile.Z == p.Z && isWater)
                        {
                            hasWater = true;
                        }
                        else if (tile.Z >= p.Z && !isWater)
                        {
                            if (Owner is BaseShipCaptain && !Owner.Deleted && Order == BoatOrder.CourseFull)
                            {
                                ((BaseShipCaptain)Owner).CheckBlock(tile, new Point3D(tx, ty, tile.Z));
                            }

                            return false;
                        }
                    }

                    if (!hasWater)
                        return false;
                }
            }

            IPooledEnumerable eable = map.GetObjectsInBounds(new Rectangle2D(p.X + newComponents.Min.X, p.Y + newComponents.Min.Y, newComponents.Width, newComponents.Height));

            foreach (IEntity e in eable)
            {
                int x = e.X - p.X + newComponents.Min.X;
                int y = e.Y - p.Y + newComponents.Min.Y;

                // No multi tiles on that point -or- mast/sail tiles
                if (x >= 0 && x < newComponents.Width && y >= 0 && y < newComponents.Height)
                {
                    if (newComponents.Tiles[x][y].Length == 0 || IsExcludedTile(newComponents.Tiles[x][y]))
                        continue;
                }

                if (e is Item)
                {
                    Item item = e as Item;

                    if ((item is BaseAddon || item is AddonComponent) && CheckAddon(item))
                        continue;

                    // Special item, we're good
                    if (CheckItem(itemID, item, p) || CanMoveOver(item) || item.Z < p.Z || ExemptOverheadComponent(p, itemID, item.X, item.Y, item.Z + item.ItemData.Height))
                        continue;
                }
                else if (e is Mobile)
                {
                    Mobile mobile = e as Mobile;

                    if (Contains(mobile) || ExemptOverheadComponent(p, itemID, mobile.X, mobile.Y, mobile.Z + 10))
                        continue;
                }

                if (Owner is BaseShipCaptain && !Owner.Deleted && Order == BoatOrder.CourseFull)
                {
                    ((BaseShipCaptain)Owner).CheckBlock(e, e.Location);
                }

                eable.Free();
                return false;
            }

            eable.Free();
            return true;
        }

        public Direction GetMovementFor(int x, int y, out int maxSpeed)
        {
            int dx = x - X;
            int dy = y - Y;

            int adx = Math.Abs(dx);
            int ady = Math.Abs(dy);

            Direction dir = Utility.GetDirection(this, new Point2D(x, y));
            int iDir = (int)dir;

            // Compute the maximum distance we can travel without going too far away
            if (iDir % 2 == 0) // North, East, South and West
                maxSpeed = Math.Abs(adx - ady);
            else // Right, Down, Left and Up
                maxSpeed = Math.Min(adx, ady);

            return (Direction)iDir;
        }

        public bool DoMovement(bool message, bool single)
        {
            Direction dir;
            int speed, clientSpeed;

            if (Order == BoatOrder.PlayerControlled)
            {
                dir = Moving;
                speed = Speed;
                clientSpeed = m_ClientSpeed;
            }
            else if (BoatCourse == null)
            {
                if (message && TillerMan != null)
                    TillerManSay(502513); // I have seen no map, sir.

                return false;
            }
            else if (Map != BoatCourse.Map)
            {
                if (message && TillerMan != null)
                    TillerManSay(502514); // The map is too far away from me, sir.

                return false;
            }
            else if ((Map != Map.Trammel && Map != Map.Felucca && Map != Map.Tokuno) || NextNavPoint < 0 || NextNavPoint >= BoatCourse.Waypoints.Count || Region.Find(Location, Map).IsPartOf<CorgulRegion>())
            {
                if (message && TillerMan != null)
                    TillerManSay(1042551); // I don't see that navpoint, sir.

                return false;
            }
            else
            {
                Point2D dest = BoatCourse.Waypoints[NextNavPoint];

                dir = GetMovementFor(dest.X, dest.Y, out int maxSpeed);

                if (maxSpeed == 0)
                {
                    if (message && Order == BoatOrder.CourseSingle && TillerMan != null)
                        TillerManSay(1042874, (NextNavPoint + 1).ToString()); // We have arrived at nav point ~1_POINT_NUM~ , sir.

                    if (NextNavPoint + 1 < BoatCourse.Waypoints.Count)
                    {
                        NextNavPoint++;

                        if (Order == BoatOrder.CourseFull)
                        {
                            if (message && TillerMan != null)
                                TillerManSay(1042875, (NextNavPoint + 1).ToString()); // Heading to nav point ~1_POINT_NUM~, sir.

                            return true;
                        }

                        return false;
                    }
                    else
                    {
                        NextNavPoint = -1;

                        if (message && Order == BoatOrder.CourseFull && TillerMan != null)
                            TillerManSay(502515); // The course is completed, sir.

                        if (Owner is BaseShipCaptain)
                            Engines.Quests.BountyQuestSpawner.ResetNavPoints(this);

                        return false;
                    }
                }

                // TODO: Throw this logic in a new Turn method overload, not this methods job to do it
                int turn;
                int offset = ((int)Facing) - ((int)dir);
                if (Math.Abs(offset) > 4)
                    turn = offset < 0 ? (8 - Math.Abs(offset)) * (-1) : 8 - offset;
                else
                    turn = -offset;

                switch (turn)
                {
                    case -2:
                        return Turn(-2, true, true, (Facing - 2) & Direction.Mask, true);
                    case -3:
                        return Turn(-2, true, true, (Facing - 3) & Direction.Mask, true);
                    case 2:
                        return Turn(2, true, true, (Facing + 2) & Direction.Mask, true);
                    case 3:
                        return Turn(2, true, true, (Facing + 3) & Direction.Mask, true);
                    case 4:
                    case -4:
                        return Turn(4, true, true, (Facing + 4) & Direction.Mask, true);
                    default:
                        break;
                }

                speed = Math.Min(Speed, maxSpeed);
                clientSpeed = 0x4;
            }

            return Move(dir, speed, clientSpeed, true);
        }

        public bool Move(Direction dir, int speed, int clientSpeed, bool message)
        {
            Map map = Map;

            if (map == null || Deleted || CheckDecay() || Scuttled)
                return false;

            int rx = 0, ry = 0;
            Direction d;

            //TODO: Clean this up
            if (Pilot == null)
            {
                d = dir;
                Movement.Movement.Offset(dir, ref rx, ref ry);
            }
            else
            {
                d = dir;
                Movement.Movement.Offset(d, ref rx, ref ry);
            }

            for (int i = 1; i <= speed; ++i)
            {
                if (!CanFit(new Point3D(X + (i * rx), Y + (i * ry), Z), Map, ItemID))
                {
                    if (i == 1)
                    {
                        if (message && TillerMan != null)
                            TillerManSay(501424); // Ar, we've stopped sir.

                        return false;
                    }

                    speed = i - 1;
                    break;
                }
            }

            int xOffset = speed * rx;
            int yOffset = speed * ry;

            int newX = X + xOffset;
            int newY = Y + yOffset;

            Rectangle2D[] wrap = GetWrapFor(map);

            for (int i = 0; i < wrap.Length; ++i)
            {
                Rectangle2D rect = wrap[i];

                if (rect.Contains(new Point2D(X, Y)) && !rect.Contains(new Point2D(newX, newY)))
                {
                    if (newX < rect.X)
                        newX = rect.X + rect.Width - 1;
                    else if (newX >= rect.X + rect.Width)
                        newX = rect.X;

                    if (newY < rect.Y)
                        newY = rect.Y + rect.Height - 1;
                    else if (newY >= rect.Y + rect.Height)
                        newY = rect.Y;

                    for (int j = 1; j <= speed; ++j)
                    {
                        if (!CanFit(new Point3D(newX + (j * rx), newY + (j * ry), Z), Map, ItemID))
                        {
                            if (message && TillerMan != null)
                                TillerManSay(501424); // Ar, we've stopped sir.

                            return false;
                        }
                    }

                    xOffset = newX - X;
                    yOffset = newY - Y;
                }
            }

            if (Math.Abs(xOffset) > 1 || Math.Abs(yOffset) > 1)
            {
                Teleport(xOffset, yOffset, 0);
            }
            else
            {
                IEnumerable<IEntity> toMove = GetEntitiesOnBoard();

                // entities/boat block move packet
                NoMoveHS = true;

                foreach (IEntity e in toMove)
                {
                    e.NoMoveHS = true;
                }

                // packet created
                MoveBoatHS smooth = new MoveBoatHS(this, d, clientSpeed, xOffset, yOffset);
                smooth.SetStatic();

                IPooledEnumerable eable = Map.GetClientsInRange(Location, Core.GlobalUpdateRange);

                // packets sent
                foreach (NetState ns in eable)
                {
                    Mobile m = ns.Mobile;

                    if (m == null || !m.CanSee(this))
                        continue;

                    if (m.InRange(Location, GetUpdateRange(m)))
                        ns.Send(smooth);
                }

                // entities move/restores packet
                foreach (IEntity ent in toMove.Where(e => !IsComponentItem(e) && !CanMoveOver(e)))
                {
                    ent.Location = new Point3D(ent.X + xOffset, ent.Y + yOffset, ent.Z);
                }

                Location = new Point3D(X + xOffset, Y + yOffset, Z);

                NoMoveHS = false;

                foreach (IEntity e in toMove)
                    e.NoMoveHS = false;

                SendContainerPacket();

                eable.Free();
                smooth.Release();
            }

            return true;
        }

        public bool OneMove(Direction dir)
        {
            if (CheckDecay())
                return false;

            if (Scuttled)
            {
                if (TillerMan != null)
                    TillerManSay(1116687);  // Arr, we be scuttled!

                return false;
            }

            // For now, need to clean out and refactor how this info is used here
            TimeSpan interval = FastInt;
            int speed = FastSpeed;

            if (StartMove(dir, speed, 0x1, interval, true, true))
            {
                if (TillerMan != null)
                    TillerManSay(501429); // Aye aye sir.

                return true;
            }

            return false;
        }

        public Point3D Rotate(Point3D p, int count)
        {
            int rx = p.X - Location.X;
            int ry = p.Y - Location.Y;

            for (int i = 0; i < count; ++i)
            {
                int temp = rx;
                rx = -ry;
                ry = temp;
            }

            return new Point3D(Location.X + rx, Location.Y + ry, p.Z);
        }

        public bool StartMove(Direction dir, bool fast)
        {
            if (CheckDecay())
                return false;

            if (Scuttled)
            {
                if (TillerMan != null)
                    TillerManSay(1116687);  //Arr, we be scuttled!

                return false;
            }

            int speed = fast ? FastSpeed : SlowSpeed;
            TimeSpan interval = GetMovementInterval(fast, out int clientSpeed);
            if (StartMove(dir, speed, clientSpeed, interval, false, true))
            {
                if (TillerMan != null && Order == BoatOrder.PlayerControlled)
                    TillerManSay(501429); // Aye aye sir.

                return true;
            }

            return false;
        }

        public bool StartMove(Direction dir, int speed, int clientSpeed, TimeSpan interval, bool single, bool message)
        {
            if (Pilot != null)
                Pilot.RevealingAction();

            if (CheckDecay() || Scuttled || clientSpeed == 0x0)
                return false;

            Moving = dir;

            if (IsRowBoat)
            {
                Speed = 1;
                m_ClientSpeed = 0x2;
                interval = SlowDriftInterval;
            }
            else
            {
                if (IsUnderEmergencyRepairs() || DamageTaken >= DamageLevel.Moderately)
                {
                    Speed = 1;
                    m_ClientSpeed = 0x2;
                    interval = SlowDriftInterval;
                }
                else
                {
                    Speed = speed;
                    m_ClientSpeed = clientSpeed;
                }
            }

            if (m_MoveTimer != null)
                m_MoveTimer.Stop();

            m_MoveTimer = new MoveTimer(this, interval, single);
            m_MoveTimer.Start();

            return true;
        }

        public bool StopMove(bool message)
        {
            if (CheckDecay())
                return false;

            if (m_MoveTimer == null)
            {
                if (message && TillerMan != null)
                    TillerManSay(501443); // Er, the ship is not moving sir.

                return false;
            }

            Moving = Direction.North;
            Speed = 0;
            m_ClientSpeed = 0;
            m_MoveTimer.Stop();
            m_MoveTimer = null;

            if (message && TillerMan != null)
                TillerManSay(501429); // Aye aye sir.

            return true;
        }

        public bool StartTurn(int offset, bool message)
        {
            if (CheckDecay())
                return false;

            if (Scuttled)
            {
                if (TillerMan != null)
                    TillerManSay(1116687);  //Arr, we be scuttled!

                return false;
            }

            bool resume = false;
            bool fast = false;
            Direction resumeDir = Direction.North;

            if (m_MoveTimer != null)
            {
                if (Order == BoatOrder.PlayerControlled)
                {
                    resume = true;
                    resumeDir = (Direction)(((int)Moving) + offset) & Direction.Mask;
                    fast = m_ClientSpeed == 0x4;
                }

                m_MoveTimer.Stop();
                m_MoveTimer = null;
            }

            if (m_TurnTimer != null)
            {
                m_TurnTimer.Stop();
            }

            m_TurnTimer = new TurnTimer(this, offset, resume, resumeDir, fast);
            m_TurnTimer.Start();

            if (message && TillerMan != null)
                TillerManSay(501429); // Aye aye sir.

            return true;
        }

        public bool Turn(int offset, bool message)
        {
            return Turn(offset, message, false, Direction.North, false);
        }

        public bool Turn(int offset, bool message, bool resume, Direction resumeDir, bool fast)
        {
            if (m_TurnTimer != null)
            {
                m_TurnTimer.Stop();
                m_TurnTimer = null;
            }

            if (CheckDecay())
                return false;

            Direction d = IsPiloted ? (Direction)offset : (Direction)(((int)m_Facing + offset) & 0x7);
            bool success = false;

            if (SetFacing(d))
            {
                success = true;
            }
            else if (message)
            {
                TillerManSay(501423); // Ar, can't turn sir.
            }

            if (resume && !IsPiloted)
            {
                StartMove(resumeDir, fast);
            }

            return success;
        }

        public void Teleport(int xOffset, int yOffset, int zOffset)
        {
            foreach (IEntity ent in GetEntitiesOnBoard().Where(e => !IsComponentItem(e) && !CanMoveOver(e) && e != TillerMan))
            {
                ent.Location = new Point3D(ent.X + xOffset, ent.Y + yOffset, ent.Z + zOffset);
            }

            Location = new Point3D(X + xOffset, Y + yOffset, Z + zOffset);
        }

        private class MoveTimer : Timer
        {
            private readonly BaseBoat m_Boat;
            private readonly bool m_SingleMove;

            public MoveTimer(BaseBoat boat, TimeSpan interval, bool single)
                : base(interval, interval, single ? 1 : 0)
            {
                m_Boat = boat;
                m_SingleMove = single;
                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                if (!m_Boat.DoMovement(true, m_SingleMove))
                    m_Boat.StopMove(false);

                if (m_SingleMove)
                    m_Boat.StopMove(false);
            }
        }

        private class TurnTimer : Timer
        {
            private readonly BaseBoat m_Boat;
            private readonly int m_Offset;

            private readonly bool m_Resume;
            private readonly Direction m_ResumeDirection;
            private readonly bool m_Fast;

            public TurnTimer(BaseBoat boat, int offset, bool resume, Direction resumeDir, bool fast)
                : base(TimeSpan.FromSeconds(0.5))
            {
                m_Boat = boat;
                m_Offset = offset;

                m_Resume = resume;
                m_ResumeDirection = resumeDir;
                m_Fast = fast;

                Priority = TimerPriority.TenMS;
            }

            protected override void OnTick()
            {
                if (!m_Boat.Deleted)
                    m_Boat.Turn(m_Offset, true, m_Resume, m_ResumeDirection, m_Fast);
            }
        }

        #endregion

        #region Packets

        public virtual void SendContainerPacket()
        {
            if (NoMoveHS || Map == null)
                return;

            IPooledEnumerable eable = Map.GetClientsInRange(Location, Core.GlobalRadarRange);

            ReleaseContainerPacket();

            foreach (NetState state in eable)
            {
                if (state.Mobile == null || state.Mobile.InUpdateRange(Location))
                    continue;

                state.Send(RemovePacket);

                foreach (Item item in ItemsOnBoard)
                {
                    state.Send(item.RemovePacket);
                }

                state.Send(GetPacketContainer(GetEntitiesOnBoard()));
            }

            eable.Free();
        }

        public Packet ContainerPacket
        {
            get { return m_ContainerPacket; }
            set
            {
                m_ContainerPacket = value;

                if (m_ContainerPacket != null)
                    m_ContainerPacket.SetStatic();
            }
        }

        protected Packet GetPacketContainer(IEnumerable<IEntity> entities)
        {
            if (ContainerPacket == null)
            {
                ContainerPacket = new PacketContainer(entities);
            }

            return ContainerPacket;
        }

        private void ReleaseContainerPacket()
        {
            Packet.Release(ref m_ContainerPacket);
        }

        public sealed class MoveBoatHS : Packet
        {
            public MoveBoatHS(BaseBoat boat, Direction d, int speed, int xOffset, int yOffset)
                : base(0xF6)
            {
                EnsureCapacity(18);

                m_Stream.Write(boat.Serial);
                m_Stream.Write((byte)speed);
                m_Stream.Write((byte)d);
                m_Stream.Write((byte)boat.Facing);
                m_Stream.Write((short)(boat.X + xOffset));
                m_Stream.Write((short)(boat.Y + yOffset));
                m_Stream.Write((short)boat.Z);

                long cp = m_Stream.Seek(0, SeekOrigin.Current);
                short length = 0;

                m_Stream.Write(length);

                foreach (IEntity ent in boat.GetEntitiesOnBoard().Where(e => e != boat))
                {
                    m_Stream.Write(ent.Serial);
                    m_Stream.Write((short)(ent.X + xOffset));
                    m_Stream.Write((short)(ent.Y + yOffset));
                    m_Stream.Write((short)ent.Z);

                    ++length;
                }

                m_Stream.Seek(cp, SeekOrigin.Begin);
                m_Stream.Write(length);

                length *= 10;

                m_Stream.Seek(1, SeekOrigin.Begin);
                m_Stream.Write(length);
            }
        }

        public class PacketContainer : Packet
        {
            public PacketContainer(IEnumerable<IEntity> entities)
                : base(0xF7)
            {
                EnsureCapacity(5);
                short c = 0;
                m_Stream.Write(c);

                foreach (IEntity entity in entities)
                {
                    int itemID = 0;
                    short amount = 0x01;
                    short hue = 0x00;
                    byte cmd = 0x0, light = 0x0, flags = 0x0;

                    if (entity is BaseMulti)
                    {
                        BaseMulti multi = entity as BaseMulti;

                        cmd = 0x02;
                        itemID = multi.ItemID;
                        itemID &= 0x7FFF;
                        hue = (short)multi.Hue;
                        amount = (short)multi.Amount;
                    }
                    else if (entity is Item)
                    {
                        Item item = entity as Item;

                        cmd = (byte)(!item.Movable && item is IDamageable ? 0x03 : 0x00);
                        itemID = item.ItemID;
                        itemID &= 0xFFFF;
                        hue = (short)item.Hue;
                        amount = (short)item.Amount;
                        light = (byte)item.Light;
                        flags = (byte)item.GetPacketFlags();
                    }
                    else if (entity is Mobile)
                    {
                        Mobile mobile = entity as Mobile;

                        cmd = 0x01;
                        itemID = mobile.BodyValue;
                        hue = (short)mobile.Hue;
                        flags = (byte)mobile.GetPacketFlags();
                    }

                    m_Stream.Write((byte)0xF3);

                    m_Stream.Write((short)0x1);
                    m_Stream.Write(cmd);
                    m_Stream.Write(entity.Serial);
                    m_Stream.Write((ushort)itemID);
                    m_Stream.Write((byte)0);

                    m_Stream.Write(amount);
                    m_Stream.Write(amount);

                    m_Stream.Write((short)(entity.X & 0x7FFF));
                    m_Stream.Write((short)(entity.Y & 0x3FFF));
                    m_Stream.Write((sbyte)entity.Z);

                    m_Stream.Write(light);
                    m_Stream.Write(hue);
                    m_Stream.Write(flags);

                    m_Stream.Write((short)0x00); // ??

                    ++c;
                }

                m_Stream.Seek(3, SeekOrigin.Begin);
                m_Stream.Write(c);
            }
        }

        #endregion
         
        public void CheckExit(object o)
        {
            if (o is CorgulRegion)
                ((CorgulRegion)o).CheckExit(this);
        }

        public void CheckEnter(object o)
        {
            if (o is CorgulWarpRegion)
                ((CorgulWarpRegion)o).CheckEnter(this);
        }

        private void ComputeDamage()
        {
            if (Durability >= 100)
                DamageTaken = DamageLevel.Pristine;
            else if (Durability >= 75.0)
                DamageTaken = DamageLevel.Slightly;
            else if (Durability >= 50.0)
                DamageTaken = DamageLevel.Moderately;
            else if (Durability >= 25.0)
                DamageTaken = DamageLevel.Heavily;
            else
                DamageTaken = DamageLevel.Severely;
        }

        public void LockPilot(Mobile pilot)
        {
            Pilot = pilot;

            if (VirtualMount == null || VirtualMount.Deleted)
                VirtualMount = new BoatMountItem(this);

            pilot.AddItem(VirtualMount);
            pilot.Direction = m_Facing;
            pilot.Delta(MobileDelta.Direction | MobileDelta.Properties);

            SendContainerPacket();
            pilot.SendLocalizedMessage(1116727); // You are now piloting this vessel.

            Refresh(pilot);

            GetEntitiesOnBoard().OfType<PlayerMobile>().Where(x => x != pilot).ToList().ForEach(y =>
            {
                y.SendLocalizedMessage(1149664, pilot.Name); // ~1_NAME~ has assumed control of the ship.
            });

            if (IsMoving)
                StopMove(false);
        }

        public void RemovePilot(Mobile from)
        {
            Pilot.RemoveItem(VirtualMount);
            VirtualMount.Internalize();

            if (IsMoving)
                StopMove(false);

            Pilot.SendLocalizedMessage(1149592); // You are no longer piloting this vessel.

            Refresh(from);

            GetEntitiesOnBoard().OfType<PlayerMobile>().Where(x => x != Pilot).ToList().ForEach(y =>
            {
                y.SendLocalizedMessage(1149668, Pilot.Name); // ~1_NAME~ has relinquished control of the ship.
            });

            Pilot = null;
        }

        public void RowBoat_Tick_Callback()
        {
            if (!MobilesOnBoard.Any())
                Delete();
        }

        public Point3D GetRotatedLocation(int x, int y)
        {
            Point3D p = new Point3D(X + x, Y + y, Z);

            return Rotate(p, (int)m_Facing / 2);
        }

        public void Refresh(Mobile from = null)
        {
            if (from != null && Owner != null && from.Account != Owner.Account)
            {
                return;
            }

            if (from != null && Status > 1043010)
            {
                from.SendLocalizedMessage(1043294); // Your ship's age and contents have been refreshed.
            }

            m_DecayTime = DateTime.UtcNow + BoatDecayDelay;

            if (TillerMan != null)
            {
                if (TillerMan is Mobile)
                    ((Mobile)TillerMan).InvalidateProperties();
                else if (TillerMan is Item)
                    ((Item)TillerMan).InvalidateProperties();
            }
        }

        public void SendMessageToAllOnBoard(object message)
        {
            foreach (Mobile m in MobilesOnBoard.OfType<PlayerMobile>().Where(pm => pm.NetState != null))
            {
                if (message is int)
                    m.SendLocalizedMessage((int)message);
                else if (message is string)
                    m.SendMessage((string)message);
            }
        }

        public void TillerManSay(object message)
        {
            if (message is int)
            {
                if (TillerMan is Mobile)
                    ((Mobile)TillerMan).Say((int)message);
                else if (TillerMan is TillerMan)
                    ((TillerMan)TillerMan).Say((int)message);
            }
            else if (message is string)
            {
                if (TillerMan is Mobile)
                    ((Mobile)TillerMan).Say((string)message);
                else if (TillerMan is TillerMan)
                    ((TillerMan)TillerMan).Say(1060658, (string)message);
            }
        }

        public void TillerManSay(object message, string arg)
        {
            if (message is int)
            {
                if (TillerMan is Mobile)
                    ((Mobile)TillerMan).Say((int)message, arg);
                else if (TillerMan is TillerMan)
                    ((TillerMan)TillerMan).Say((int)message, arg);
            }
            else if (message is string)
            {
                if (TillerMan is Mobile)
                    ((Mobile)TillerMan).Say((string)message);
                else if (TillerMan is TillerMan)
                    ((TillerMan)TillerMan).Say(1060658, (string)message);
            }
        }

        public void TryTrack(Mobile from, string speech)
        {
            if (speech.ToLower().IndexOf("start") >= 0)
                BoatTrackingArrow.StartTracking(from);
            else if (speech.ToLower().IndexOf("stop") >= 0)
                BoatTrackingArrow.StopTracking(from);
        }

        public class UpdateAllTimer : Timer
        {
            public UpdateAllTimer()
                : base(TimeSpan.FromSeconds(1.0))
            {
            }

            protected override void OnTick()
            {
                UpdateAllComponents();
            }
        }
    }

    [PropertyObject]
    public class BoatCourse
    {
        private readonly List<Point2D> m_Waypoints = new List<Point2D>();
        public List<Point2D> Waypoints => m_Waypoints;

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseBoat Boat { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map Map { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int NumWaypoints => m_Waypoints.Count;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool GivenMap { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point2D SetWaypoint
        {
            get { return Point2D.Zero; }
            set
            {
                AddWaypoint(value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool GetMap
        {
            get { return false; }
            set
            {
                if (value)
                {
                    if (Boat != null)
                    {
                        MapItem mapItem = new MapItem(Map);

                        if (Map == Map.Tokuno)
                            mapItem.SetDisplay(5, 5, 1448 - 32, 1448 - 10, 400, 400);
                        else
                            mapItem.SetDisplay(5, 5, 5120 - 32, 4096 - 10, 400, 400);

                        for (int i = 0; i < m_Waypoints.Count; i++)
                            mapItem.AddWorldPin(m_Waypoints[i].X, m_Waypoints[i].Y);

                        if (Boat is BaseGalleon)
                        {
                            ((BaseGalleon)Boat).GalleonHold.DropItem(mapItem);
                        }
                        else
                            Boat.Hold.DropItem(mapItem);
                    }
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point2D CurrentWaypoint
        {
            get
            {
                if (Boat != null && Boat.NextNavPoint >= 0 && Boat.NextNavPoint < m_Waypoints.Count)
                    return m_Waypoints[Boat.NextNavPoint];

                return Point2D.Zero;
            }
        }

        public BoatCourse(BaseBoat boat)
        {
            Boat = boat;
            Map = boat.Map;
            GivenMap = false;
        }

        public BoatCourse(BaseBoat boat, MapItem item)
        {
            Boat = boat;
            Map = boat.Map;
            GivenMap = false;

            for (int i = 0; i < item.Pins.Count; i++)
            {
                item.ConvertToWorld(item.Pins[i].X, item.Pins[i].Y, out int x, out int y);
                m_Waypoints.Add(new Point2D(x, y));
            }
        }

        public BoatCourse(BaseBoat boat, List<Point2D> list)
        {
            Boat = boat;
            Map = boat.Map;
            GivenMap = false;

            m_Waypoints = list;
        }

        public void AddWaypoint(Point2D p)
        {
            m_Waypoints.Add(p);
        }

        public void RemoveWaypoint(int index)
        {
            if (index >= 0 && index < m_Waypoints.Count)
                m_Waypoints.RemoveAt(index);
        }

        public override bool Equals(object obj)
        {
            if (obj is BoatCourse)
            {
                BoatCourse course = (BoatCourse)obj;

                if (Map == course.Map && Boat == course.Boat && course.Waypoints.Count == m_Waypoints.Count)
                {
                    for (int i = 0; i < m_Waypoints.Count; i++)
                    {
                        if (m_Waypoints[i] != course.Waypoints[i])
                            return false;
                    }

                    return true;
                }
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public BoatCourse(GenericReader reader)
        {
            int version = reader.ReadInt();
            GivenMap = reader.ReadBool();

            int c = reader.ReadInt();

            for (int i = 0; i < c; i++)
            {
                m_Waypoints.Add(reader.ReadPoint2D());
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);
            writer.Write(GivenMap);

            writer.Write(m_Waypoints.Count);

            foreach (Point2D p in m_Waypoints)
            {
                writer.Write(p);
            }
        }
    }

    public class DryDockEntry : ContextMenuEntry
    {
        private BaseBoat Boat { get; set; }
        private Mobile From { get; set; }

        public DryDockEntry(BaseBoat boat, Mobile from)
            : base(1116520, 12)
        {
            From = from;
            Boat = boat;

            Enabled = Boat != null && Boat.IsOwner(from);
        }

        public override void OnClick()
        {
            if (Boat != null && !Boat.Contains(From) && Boat.IsOwner(From))
                Boat.BeginDryDock(From);
        }
    }

    public class RenameShipEntry : ContextMenuEntry
    {
        private BaseBoat Boat { get; set; }
        private Mobile From { get; set; }

        public RenameShipEntry(BaseBoat boat, Mobile from)
            : base(1111680, 3)
        {
            Boat = boat;
            From = from;

            Enabled = boat != null && boat.IsOwner(from);
        }

        public override void OnClick()
        {
            if (Boat != null && Boat.IsOwner(From))
                Boat.BeginRename(From);
        }
    }
}
