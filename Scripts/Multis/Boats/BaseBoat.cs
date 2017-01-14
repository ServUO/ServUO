using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Movement;
using Server.Network;
using Server.Mobiles;
using Server.Regions;
using System.Linq;

namespace Server.Multis
{
    public enum BoatOrder
    {
        Move,
        Course,
        Single
    }

    public abstract class BaseBoat : BaseMulti, IMount
    {
        private static Rectangle2D[] m_BritWrap = new Rectangle2D[]{ new Rectangle2D( 16, 16, 5120 - 32, 4096 - 32 ), new Rectangle2D( 5136, 2320, 992, 1760 ),
                                                                     new Rectangle2D(6272, 1088, 319, 319)};
        private static Rectangle2D[] m_IlshWrap = new Rectangle2D[] { new Rectangle2D(16, 16, 2304 - 32, 1600 - 32) };
        private static Rectangle2D[] m_TokunoWrap = new Rectangle2D[] { new Rectangle2D(16, 16, 1448 - 32, 1448 - 32) };

        //private static TimeSpan BoatDecayDelay = TimeSpan.FromDays( 9.0 );

        public static BaseBoat FindBoatAt(IPoint2D loc, Map map)
        {
            if (map == null || map == Map.Internal)
                return null;

            Sector sector = map.GetSector(loc);

            for (int i = 0; i < sector.Multis.Count; i++)
            {
                BaseBoat boat = sector.Multis[i] as BaseBoat;

                if (boat != null && boat.Contains(loc.X, loc.Y))
                    return boat;
            }

            return null;
        }

        private Container m_SecureContainer;

        private Hold m_Hold;
        private object m_TillerMan;
        private Mobile m_Owner;
        private Mobile m_Pilot;

        private Direction m_Facing;

        private Direction m_Moving;
        private int m_Speed;
        private int m_ClientSpeed;

        private bool m_Anchored;
        private string m_ShipName;

        private BoatOrder m_Order;

        private MapItem m_MapItem;
        private int m_NextNavPoint;
        private BoatCourse m_BoatCourse;

        private Plank m_PPlank, m_SPlank;

        private DateTime m_DecayTime;

        private Timer m_TurnTimer;
        private Timer m_MoveTimer;

        private bool m_Decay;

        private BoatMountItem m_VirtualMount;
        private BaseDockedBoat m_DockedBoat;

        [CommandProperty(AccessLevel.GameMaster)]
        public Hold Hold { get { return m_Hold; } set { m_Hold = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public object TillerMan { get { return m_TillerMan; } set { m_TillerMan = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Plank PPlank { get { return m_PPlank; } set { m_PPlank = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Plank SPlank { get { return m_SPlank; } set { m_SPlank = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get { return m_Owner; } set { m_Owner = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Pilot { get { return m_Pilot; } set { m_Pilot = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Facing { get { return m_Facing; } set { SetFacing(value); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Moving { get { return m_Moving; } set { m_Moving = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsMoving { get { return (m_MoveTimer != null); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsTurning { get { return (m_TurnTimer != null); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPiloted { get { return (m_Pilot != null); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Speed { get { return m_Speed; } set { m_Speed = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Anchored { get { return m_Anchored; } set { m_Anchored = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string ShipName { get { return m_ShipName; } set { m_ShipName = value; if (m_TillerMan != null) InvalidateTillerManProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public BoatOrder Order { get { return m_Order; } set { m_Order = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public MapItem MapItem { get { return m_MapItem; } set { m_MapItem = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int NextNavPoint { get { return m_NextNavPoint; } set { m_NextNavPoint = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public BoatCourse BoatCourse { get { return m_BoatCourse; } set { m_BoatCourse = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DoesDecay { get { return m_Decay; } set { m_Decay = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public BoatMountItem VirtualMount { get { return m_VirtualMount; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseDockedBoat BoatItem { get { return m_DockedBoat; } set { m_DockedBoat = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Container SecureContainer { get { return m_SecureContainer; } set { m_SecureContainer = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime TimeOfDecay
        {
            get
            {
                return m_DecayTime;
            }
            set
            {
                m_DecayTime = value;
                if (m_TillerMan != null && m_TillerMan is Mobile)
                    ((Mobile)m_TillerMan).InvalidateProperties();
                else if (m_TillerMan != null && m_TillerMan is Item)
                    ((Item)m_TillerMan).InvalidateProperties();
            }
        }

        public int Status
        {
            get
            {
                if (!m_Decay)
                    return 1043010; // This structure is like new.

                if(m_DecayTime <= DateTime.UtcNow)
                    return 1043015; // This structure is in danger of collapsing.

                TimeSpan decaySpan = m_DecayTime - DateTime.UtcNow;

                if(decaySpan > TimeSpan.FromDays(6.0))
                    return 1043010; // This structure is like new.

                if(decaySpan > TimeSpan.FromDays(5.0))
                    return 1043011; // This structure is slightly worn.

                if(decaySpan > TimeSpan.FromDays(4.0))
                    return 1043012; // This structure is somewhat worn.

                if(decaySpan > TimeSpan.FromDays(3.0))
                    return 1043013; // This structure is fairly worn.

                 if(decaySpan > TimeSpan.FromDays(1.5))
                     return 1043014; // This structure is greatly worn.

                 return 1043015; // This structure is in danger of collapsing.*/
            }
        }

        public virtual int NorthID { get { return 0; } }
        public virtual int EastID { get { return 0; } }
        public virtual int SouthID { get { return 0; } }
        public virtual int WestID { get { return 0; } }

        public virtual int HoldDistance { get { return 0; } }
        public virtual int TillerManDistance { get { return 0; } }
        public virtual Point2D StarboardOffset { get { return Point2D.Zero; } }
        public virtual Point2D PortOffset { get { return Point2D.Zero; } }
        public virtual Point3D MarkOffset { get { return Point3D.Zero; } }

        #region High Seas
        public virtual bool IsClassicBoat { get { return true; } }
        public virtual double TurnDelay { get { return 0.5; } }
        public virtual bool Scuttled { get { return false; } }
        public virtual TimeSpan BoatDecayDelay { get { return TimeSpan.FromDays(9); } }
        public virtual bool CanLinkToLighthouse { get { return true; } }

        #region IMount Members
        public Mobile Rider { get { return m_Pilot; } set { m_Pilot = value; } }

        public void OnRiderDamaged(int amount, Mobile from, bool willKill)
        {
        }
        #endregion
        #endregion

        public virtual BaseDockedBoat DockedBoat { get { return null; } }

        private static List<BaseBoat> m_Instances = new List<BaseBoat>();

        public static List<BaseBoat> Boats { get { return m_Instances; } }

        public BaseBoat(Direction direction)
            : this(direction, false)
        {
        }

        public BaseBoat(Direction direction, bool isClassic)
            : base(0x0)
        {
            m_DecayTime = DateTime.UtcNow + BoatDecayDelay;
            m_Decay = true;
            Facing = direction;
            Layer = Layer.Mount;
            m_Anchored = false;
            m_VirtualMount = new BoatMountItem(this);

            if (isClassic)
            {
                m_TillerMan = new TillerMan(this);
                m_PPlank = new Plank(this, PlankSide.Port, 0);
                m_SPlank = new Plank(this, PlankSide.Starboard, 0);

                m_PPlank.MoveToWorld(new Point3D(X + PortOffset.X, Y + PortOffset.Y, Z), Map);
                m_SPlank.MoveToWorld(new Point3D(X + StarboardOffset.X, Y + StarboardOffset.Y, Z), Map);

                m_Hold = new Hold(this);

                UpdateComponents();
            }

            m_NextNavPoint = -1;
            Movable = false;
            m_Instances.Add(this);
        }

        public BaseBoat(Serial serial)
            : base(serial)
        {
        }

        public Point3D GetRotatedLocation(int x, int y)
        {
            Point3D p = new Point3D(X + x, Y + y, Z);

            return Rotate(p, (int)m_Facing / 2);
        }

        public virtual void UpdateComponents()
        {
            if (m_PPlank != null)
            {
                m_PPlank.MoveToWorld(GetRotatedLocation(PortOffset.X, PortOffset.Y), Map);
                m_PPlank.SetFacing(m_Facing);
            }

            if (m_SPlank != null)
            {
                m_SPlank.MoveToWorld(GetRotatedLocation(StarboardOffset.X, StarboardOffset.Y), Map);
                m_SPlank.SetFacing(m_Facing);
            }

            int xOffset = 0, yOffset = 0;
            Movement.Movement.Offset(m_Facing, ref xOffset, ref yOffset);

            if (m_TillerMan != null)
            {
                if (m_TillerMan is Item)
                {
                    TillerMan tillerman = (TillerMan)m_TillerMan;
                    tillerman.Location = new Point3D(X + (xOffset * TillerManDistance) + (m_Facing == Direction.North ? 1 : 0), Y + (yOffset * TillerManDistance), tillerman.Z);
                    tillerman.SetFacing(m_Facing);
                    tillerman.InvalidateProperties();
                }
            }

            if (m_Hold != null)
            {
                m_Hold.Location = new Point3D(X + (xOffset * HoldDistance), Y + (yOffset * HoldDistance), m_Hold.Z);
                m_Hold.SetFacing(m_Facing);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)4);

            if (m_BoatCourse != null)
            {
                writer.Write((int)1);
                m_BoatCourse.Serialize(writer);
            }
            else
                writer.Write((int)0);

            writer.Write(m_DockedBoat);

            writer.Write(m_VirtualMount);
            writer.Write(m_Decay);

            // version 3
            writer.Write((Item)m_MapItem);
            writer.Write((int)m_NextNavPoint);

            writer.Write((int)m_Facing);

            writer.WriteDeltaTime(m_DecayTime);

            writer.Write(m_Owner);
            writer.Write(m_PPlank);
            writer.Write(m_SPlank);

            if (m_TillerMan is Mobile)
                writer.Write(m_TillerMan as Mobile);
            else if (m_TillerMan is Item)
                writer.Write(m_TillerMan as Item);

            writer.Write(m_Hold);
            writer.Write(m_Anchored);
            writer.Write(m_ShipName);

            CheckDecay();
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 4:
                    {
                        if (reader.ReadInt() == 1)
                        {
                            m_BoatCourse = new BoatCourse(reader);
                            m_BoatCourse.Boat = this;
                            m_BoatCourse.Map = this.Map;
                        }

                        m_DockedBoat = reader.ReadItem() as BaseDockedBoat;
                        m_VirtualMount = reader.ReadItem() as BoatMountItem;
                        m_Decay = reader.ReadBool();
                        goto case 3;
                    }
                case 3:
                    {
                        m_MapItem = (MapItem)reader.ReadItem();
                        m_NextNavPoint = reader.ReadInt();

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
                            m_NextNavPoint = -1;

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

                        m_Owner = reader.ReadMobile();
                        m_PPlank = reader.ReadItem() as Plank;
                        m_SPlank = reader.ReadItem() as Plank;

                        if (!IsClassicBoat && !(this is RowBoat))
                            m_TillerMan = reader.ReadMobile() as object;
                        else
                            m_TillerMan = reader.ReadItem() as object;

                        m_Hold = reader.ReadItem() as Hold;
                        m_Anchored = reader.ReadBool();
                        m_ShipName = reader.ReadString();

                        m_Anchored = false; //No more anchors[High Seas]

                        if (version < 1)
                            Refresh();

                        break;
                    }
            }

            m_Instances.Add(this);

            if (m_VirtualMount == null)
                m_VirtualMount = new BoatMountItem(this);

            if (version == 6)
            {
                if (m_MapItem != null)
                    Timer.DelayCall(TimeSpan.FromSeconds(10), delegate
                    {
                        BoatCourse = new BoatCourse(this, m_MapItem);
                    });
            }
        }

        public void RemoveKeys(Mobile m)
        {
            uint keyValue = 0;

            if (m_PPlank != null)
                keyValue = m_PPlank.KeyValue;

            if (keyValue == 0 && m_SPlank != null)
                keyValue = m_SPlank.KeyValue;

            Key.RemoveKeys(m, keyValue);
        }

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

        public override bool AllowsRelativeDrop
        {
            get { return true; }
        }

        public override void OnAfterDelete()
        {
            if (m_TillerMan != null)
            {
                if (m_TillerMan is Mobile)
                    ((Mobile)m_TillerMan).Delete();

                else if (m_TillerMan is Item)
                    ((Item)m_TillerMan).Delete();
            }

            if (m_Hold != null)
                m_Hold.Delete();

            if (m_PPlank != null)
                m_PPlank.Delete();

            if (m_SPlank != null)
                m_SPlank.Delete();

            if (m_TurnTimer != null)
                m_TurnTimer.Stop();

            if (m_MoveTimer != null)
                m_MoveTimer.Stop();

            #region High Seas
            if (Owner is BaseShipCaptain)
                ((BaseShipCaptain)Owner).OnShipDelete();

            if (m_Pilot != null)
                RemovePilot(m_Pilot);

            if (m_VirtualMount != null)
                m_VirtualMount.Delete();
            #endregion

            if (m_SecureContainer != null)
                m_SecureContainer.Delete();

            if (m_DockedBoat != null && !m_DockedBoat.Deleted && m_DockedBoat.Map == Map.Internal)
                m_DockedBoat.Delete();

            m_Instances.Remove(this);

            base.OnAfterDelete();
        }

        public override void OnLocationChange(Point3D old)
        {

            if (m_TillerMan != null)
            {
                if (m_TillerMan is Mobile && (Math.Abs(X - old.X) > 1 || Math.Abs(Y - old.Y) > 1))
                    ((Mobile)m_TillerMan).Location = new Point3D(X + (((Mobile)m_TillerMan).X - old.X), Y + (((Mobile)m_TillerMan).Y - old.Y), Z + (((Mobile)m_TillerMan).Z - old.Z));
                else if (m_TillerMan is Item)
                    ((Item)m_TillerMan).Location = new Point3D(X + (((Item)m_TillerMan).X - old.X), Y + (((Item)m_TillerMan).Y - old.Y), Z + (((Item)m_TillerMan).Z - old.Z));
            }

            if (m_Hold != null)
                m_Hold.Location = new Point3D(X + (m_Hold.X - old.X), Y + (m_Hold.Y - old.Y), Z + (m_Hold.Z - old.Z));

            if (m_PPlank != null)
                m_PPlank.Location = new Point3D(X + (m_PPlank.X - old.X), Y + (m_PPlank.Y - old.Y), Z + (m_PPlank.Z - old.Z));

            if (m_SPlank != null)
                m_SPlank.Location = new Point3D(X + (m_SPlank.X - old.X), Y + (m_SPlank.Y - old.Y), Z + (m_SPlank.Z - old.Z));

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
            if (m_TillerMan != null)
            {
                if (m_TillerMan is Mobile)
                    ((Mobile)m_TillerMan).Map = Map;
                else if (m_TillerMan is Item)
                    ((Item)m_TillerMan).Map = Map;
            }

            if (m_Hold != null)
                m_Hold.Map = Map;

            if (m_PPlank != null)
                m_PPlank.Map = Map;

            if (m_SPlank != null)
                m_SPlank.Map = Map;
        }

        public virtual bool CanCommand(Mobile m)
        {
            return true;
        }

        public virtual Point3D GetMarkedLocation()
        {
            Point3D p = new Point3D(X + MarkOffset.X, Y + MarkOffset.Y, Z + MarkOffset.Z);

            return Rotate(p, (int)m_Facing / 2);
        }

        public virtual bool IsOwner(Mobile from)
        {
            if (m_Owner == null || from == null)
                return false;

            if (from == m_Owner || from.AccessLevel > AccessLevel.Player)
                return true;

            Server.Accounting.Account acct1 = from.Account as Server.Accounting.Account;
            Server.Accounting.Account acct2 = m_Owner.Account as Server.Accounting.Account;

            return acct1 != null && acct2 != null && acct1 == acct2;
        }

        public bool CheckKey(uint keyValue)
        {
            if (m_SPlank != null && m_SPlank.KeyValue == keyValue)
                return true;

            if (m_PPlank != null && m_PPlank.KeyValue == keyValue)
                return true;

            return false;
        }

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

        // Legacy clients will continue to see the old-style movement, but speeds are adjusted
        private static bool NewBoatMovement = true;

        private static int SlowSpeed = 1;
        private static int FastSpeed = NewBoatMovement ? 1 : 3;
        private static int SlowDriftSpeed = 1;
        private static int FastDriftSpeed = 1;

        public static TimeSpan FastInterval = TimeSpan.FromSeconds(.25);
        public static TimeSpan NormalInterval = TimeSpan.FromSeconds(.50);
        public static TimeSpan SlowInterval = TimeSpan.FromSeconds(1.0);

        public static TimeSpan FastDriftInterval = NormalInterval;
        public static TimeSpan SlowDriftInterval = SlowInterval;

        private static Direction Forward = Direction.North;
        private static Direction ForwardLeft = Direction.Up;
        private static Direction ForwardRight = Direction.Right;
        private static Direction Backward = Direction.South;
        private static Direction BackwardLeft = Direction.Left;
        private static Direction BackwardRight = Direction.Down;
        private static Direction Left = Direction.West;
        private static Direction Right = Direction.East;
        private static Direction Port = Left;
        private static Direction Starboard = Right;

        private bool m_Decaying;

        public void Refresh()
        {
            m_DecayTime = DateTime.UtcNow + BoatDecayDelay;

            if (m_TillerMan != null)
            {
                if (m_TillerMan is Mobile)
                    ((Mobile)m_TillerMan).InvalidateProperties();
                else if (m_TillerMan is Item)
                    ((Item)m_TillerMan).InvalidateProperties();
            }
        }

        public bool StartMove(Direction dir, bool fast)
        {
            if (CheckDecay())
                return false;

            if (Scuttled)
            {
                if (m_TillerMan != null)
                    TillerManSay(1116687);  //Arr, we be scuttled!

                return false;
            }

            int clientSpeed = 0x4;
            bool drift = (dir != Forward && dir != ForwardLeft && dir != ForwardRight);
            int speed = (fast ? (drift ? FastDriftSpeed : FastSpeed) : (drift ? SlowDriftSpeed : SlowSpeed));
            TimeSpan interval = GetMovementInterval(fast, drift, out clientSpeed);

            if (StartMove(dir, speed, clientSpeed, interval, false, true))
            {
                if (m_TillerMan != null)
                    TillerManSay(501429); // Aye aye sir.

                return true;
            }

            return false;
        }

        public bool OneMove(Direction dir)
        {
            if (CheckDecay())
                return false;

            if (Scuttled)
            {
                if (m_TillerMan != null)
                    TillerManSay(1116687);  //Arr, we be scuttled!

                return false;
            }

            bool drift = (dir != Forward);
            TimeSpan interval = drift ? NormalInterval : FastInterval;
            int speed = drift ? FastDriftSpeed : FastSpeed;

            if (StartMove(dir, speed, 0x1, interval, true, true))
            {
                if (m_TillerMan != null)
                    TillerManSay(501429); // Aye aye sir.

                return true;
            }

            return false;
        }

        public void BeginRename(Mobile from)
        {
            if (CheckDecay())
                return;

            if (from.AccessLevel < AccessLevel.GameMaster && from != m_Owner)
            {
                if (m_TillerMan != null)
                    TillerManSay(Utility.Random(1042876, 4)); // Arr, don't do that! | Arr, leave me alone! | Arr, watch what thour'rt doing, matey! | Arr! Do that again and I’ll throw ye overhead!

                return;
            }

            if (m_TillerMan != null)
                TillerManSay(502580); // What dost thou wish to name thy ship?

            from.Prompt = new RenameBoatPrompt(this);
        }

        public void EndRename(Mobile from, string newName)
        {
            if (Deleted || CheckDecay())
                return;

            if (from.AccessLevel < AccessLevel.GameMaster && from != m_Owner)
            {
                if (m_TillerMan != null)
                    TillerManSay(1042880); // Arr! Only the owner of the ship may change its name!

                return;
            }
            else if (!from.Alive)
            {
                if (m_TillerMan != null)
                    TillerManSay(502582); // You appear to be dead.

                return;
            }

            newName = newName.Trim();

            if (!Server.Guilds.BaseGuildGump.CheckProfanity(newName))
            {
                from.SendMessage("That name is unacceptable.");
                return;
            }

            if (newName.Length == 0)
                newName = null;

            Rename(newName);
        }

        public enum DryDockResult { Valid, Dead, NoKey, NotAnchored, Mobiles, Items, Hold, Decaying, NotEnoughGold, Damaged, Addons, Cannon }

        public virtual DryDockResult CheckDryDock(Mobile from, Mobile dockmaster)
        {
            if (CheckDecay())
                return DryDockResult.Decaying;

            if (!from.Alive)
                return DryDockResult.Dead;

            Container pack = from.Backpack;

            if (IsClassicBoat)
            {
                if ((m_SPlank == null || !Key.ContainsKey(pack, m_SPlank.KeyValue)) && (m_PPlank == null || !Key.ContainsKey(pack, m_PPlank.KeyValue)))
                    return DryDockResult.NoKey;

                //if (!m_Anchored)
                //    return DryDockResult.NotAnchored;
            }

            if (m_Hold != null && m_Hold.Items.Count > 0)
                return DryDockResult.Hold;

            Map map = Map;

            if (map == null || map == Map.Internal)
                return DryDockResult.Items;

            List<ISpawnable> list = GetMovingEntities();
            DryDockResult res = DryDockResult.Valid;

            foreach (ISpawnable o in list)
            {
                if (o == this || IsComponentItem(o) || o is EffectItem || o == m_TillerMan)
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

            list.Clear();
            list.TrimExcess();
            return res;
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
                from.SendLocalizedMessage(1116323);  //You cannot dock the ship with loaded weapons on deck!
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
                from.SendLocalizedMessage(1116506, DockMaster.DryDockAmount.ToString()); //The price is ~1_price~ and I will accept nothing less!
            else if (result == DryDockResult.Damaged)
                from.SendLocalizedMessage(1116324); // The ship must be fully repaired before it can be docked!
            else if (result == DryDockResult.Cannon)
                from.SendLocalizedMessage(1116323);  //You cannot dock the ship with loaded weapons on deck!

            if (result != DryDockResult.Valid)
                return;

            BaseDockedBoat boat = BoatItem;

            if (boat == null || boat.Deleted)
                boat = DockedBoat;

            if (boat == null)
                return;

            boat.BoatItem = this;

            if (IsClassicBoat)
                RemoveKeys(from);

            from.AddToBackpack(boat);

            this.Refresh();
            this.Internalize();

            OnDryDock(from);
        }

        public virtual void OnDryDock(Mobile from)
        {
            var addon = LighthouseAddon.GetLighthouse(Owner);

            if (addon != null && Owner != null && Owner.NetState != null)
            {
                Owner.SendLocalizedMessage(1154594); // You have unlinked your ship from your lighthouse.
            }
        }

        public void SetName(SpeechEventArgs e)
        {
            if (CheckDecay())
                return;

            if (e.Mobile.AccessLevel < AccessLevel.GameMaster && e.Mobile != m_Owner)
            {
                if (m_TillerMan != null)
                    TillerManSay(1042880); // Arr! Only the owner of the ship may change its name!

                return;
            }
            else if (!e.Mobile.Alive)
            {
                if (m_TillerMan != null)
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
                if (m_TillerMan != null)
                    TillerManSay(502531); // Yes, sir.

                return;
            }

            ShipName = newName;

            if (m_TillerMan != null && m_ShipName != null)
                TillerManSay(1042885, m_ShipName); // This ship is now called the ~1_NEW_SHIP_NAME~.
            else if (m_TillerMan != null)
                TillerManSay(502534); // This ship now has no name.
        }

        public void RemoveName(Mobile m)
        {
            if (CheckDecay())
                return;

            if (m.AccessLevel < AccessLevel.GameMaster && m != m_Owner)
            {
                if (m_TillerMan != null)
                    TillerManSay(1042880); // Arr! Only the owner of the ship may change its name!

                return;
            }
            else if (!m.Alive)
            {
                if (m_TillerMan != null)
                    TillerManSay(502582); // You appear to be dead.

                return;
            }

            if (m_ShipName == null)
            {
                if (m_TillerMan != null)
                    TillerManSay(502526); // Ar, this ship has no name.

                return;
            }

            ShipName = null;

            if (m_TillerMan != null)
                TillerManSay(502534); // This ship now has no name.
        }

        public void GiveName(Mobile m)
        {
            if (m_TillerMan == null || CheckDecay())
                return;

            if (m_ShipName == null)
                TillerManSay(502526); // Ar, this ship has no name.
            else
                TillerManSay(1042881, m_ShipName); // This is the ~1_BOAT_NAME~.
        }

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
                NextNavPoint = -1;

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
                if (Char.IsDigit(navPoint[i]))
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

                    if (m_BoatCourse == null || number < 0 || number >= m_BoatCourse.Waypoints.Count)
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

            if (m_BoatCourse == null)
            {
                if (message && TillerMan != null)
                    TillerManSay(502513); // I have seen no map, sir.

                return false;
            }
            else if (this.Map != m_BoatCourse.Map)
            {
                if (message && TillerMan != null)
                    TillerManSay(502514); // The map is too far away from me, sir.

                return false;
            }
            else if ((this.Map != Map.Trammel && this.Map != Map.Felucca && this.Map != Map.Tokuno) || NextNavPoint < 0 || NextNavPoint >= m_BoatCourse.Waypoints.Count)
            {
                if (message && TillerMan != null)
                    TillerManSay(1042551); // I don't see that navpoint, sir.

                return false;
            }

            Speed = Owner is BaseShipCaptain ? SlowSpeed : FastSpeed;
            Order = single ? BoatOrder.Single : BoatOrder.Course;

            if (m_MoveTimer != null)
                m_MoveTimer.Stop();

            m_MoveTimer = new MoveTimer(this, FastInterval, false);
            m_MoveTimer.Start();

            if (message && TillerMan != null)
                TillerManSay(501429); // Aye aye sir.

            return true;
        }

        public override bool HandlesOnSpeech { get { return true; } }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (CheckDecay())
                return;

            Mobile from = e.Mobile;

            if (CanCommand(from) && Contains(from) && m_Pilot == null)
            {
                for (int i = 0; i < e.Keywords.Length; ++i)
                {
                    if (e.Handled)
                        break;

                    int keyword = e.Keywords[i];

                    if ((keyword >= 0x42 && keyword <= 0x6B) || keyword == 0x28 || keyword == 0xF)  // TG - added 0x28
                    {
                        switch (keyword)
                        {
                            case 0x42: SetName(e); break;
                            case 0x43: RemoveName(e.Mobile); break;
                            case 0x44: GiveName(e.Mobile); break;
                            case 0x45: StartMove(Forward, true); break;
                            case 0x46: StartMove(Backward, true); break;
                            case 0x47: StartMove(Left, true); break;
                            case 0x48: StartMove(Right, true); break;
                            case 0x4B: StartMove(ForwardLeft, true); break;
                            case 0x4C: StartMove(ForwardRight, true); break;
                            case 0x4D: StartMove(BackwardLeft, true); break;
                            case 0x4E: StartMove(BackwardRight, true); break;
                            case 0x4F: StopMove(true); break;
                            case 0x50: StartMove(Left, false); break;
                            case 0x51: StartMove(Right, false); break;
                            case 0x52: StartMove(Forward, false); break;
                            case 0x53: StartMove(Backward, false); break;
                            case 0x54: StartMove(ForwardLeft, false); break;
                            case 0x55: StartMove(ForwardRight, false); break;
                            case 0x56: StartMove(BackwardRight, false); break;
                            case 0x57: StartMove(BackwardLeft, false); break;
                            case 0x58: OneMove(Left); break;
                            case 0x59: OneMove(Right); break;
                            case 0x5A: OneMove(Forward); break;
                            case 0x5B: OneMove(Backward); break;
                            case 0x5C: OneMove(ForwardLeft); break;
                            case 0x5D: OneMove(ForwardRight); break;
                            case 0x5E: OneMove(BackwardRight); break;
                            case 0x5F: OneMove(BackwardLeft); break;
                            case 0x49:
                            case 0x65: StartTurn(2, true); break; // turn right
                            case 0x4A:
                            case 0x66: StartTurn(-2, true); break; // turn left
                            case 0x67: StartTurn(-4, true); break; // turn around, come about
                            case 0x68: StartMove(Forward, true); break;
                            case 0x69: StopMove(true); break;
                            //case 0x6A: LowerAnchor( true ); break;
                            //case 0x6B: RaiseAnchor( true ); break;
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

        public void TryTrack(Mobile from, string speech)
        {
            if (speech.ToLower().IndexOf("start") >= 0)
                BoatTrackingArrow.StartTracking(from);
            else if (speech.ToLower().IndexOf("stop") >= 0)
                BoatTrackingArrow.StopTracking(from);
        }

        public virtual void AddSecureContainer(Mobile from)
        {
            from.SendMessage("You can only place a container on a galleon!");
        }

        public bool StartTurn(int offset, bool message)
        {
            if (CheckDecay())
                return false;

            if (Scuttled)
            {
                if (m_TillerMan != null)
                    TillerManSay(1116687);  //Arr, we be scuttled!

                return false;
            }

            if (m_MoveTimer != null && this.Order != BoatOrder.Move)
            {
                m_MoveTimer.Stop();
                m_MoveTimer = null;
            }

            if (m_TurnTimer != null)
                m_TurnTimer.Stop();

            m_TurnTimer = new TurnTimer(this, offset);
            m_TurnTimer.Start();

            if (message && TillerMan != null)
                TillerManSay(501429); // Aye aye sir.

            return true;
        }

        public bool Turn(int offset, bool message)
        {
            if (m_TurnTimer != null)
            {
                m_TurnTimer.Stop();
                m_TurnTimer = null;
            }

            if (CheckDecay())
                return false;

            Direction d = IsPiloted ? (Direction)offset : (Direction)(((int)m_Facing + offset) & 0x7);

            if (SetFacing(d))
            {
                return true;
            }
            else
            {
                if (message)
                    TillerManSay(501423); // Ar, can't turn sir.

                return false;
            }
        }

        private class TurnTimer : Timer
        {
            private BaseBoat m_Boat;
            private int m_Offset;

            public TurnTimer(BaseBoat boat, int offset)
                : base(TimeSpan.FromSeconds(0.5))
            {
                m_Boat = boat;
                m_Offset = offset;

                Priority = TimerPriority.TenMS;
            }

            protected override void OnTick()
            {
                if (!m_Boat.Deleted)
                    m_Boat.Turn(m_Offset, true);
            }
        }

        public bool StartMove(Direction dir, int speed, int clientSpeed, TimeSpan interval, bool single, bool message)
        {
            if (CheckDecay() || Scuttled || clientSpeed == 0x0)
                return false;

            m_Moving = dir;
            m_Speed = speed;
            m_ClientSpeed = clientSpeed;
            m_Order = BoatOrder.Move;

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
                if (message && m_TillerMan != null)
                    TillerManSay(501443); // Er, the ship is not moving sir.

                return false;
            }

            m_Moving = Direction.North;
            m_Speed = 0;
            m_ClientSpeed = 0;
            m_MoveTimer.Stop();
            m_MoveTimer = null;

            if (message && m_TillerMan != null)
                TillerManSay(501429); // Aye aye sir.

            return true;
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

                    //if (tiles.Length > 0 && IsExcludedTile(tiles) && !Contains(tiles[0].X, tiles[0].Y))
                    //    continue;

                    bool hasWater = false;

                    if (landTile.Z == p.Z && ((landTile.ID >= 168 && landTile.ID <= 171) || (landTile.ID >= 310 && landTile.ID <= 311)))
                        hasWater = true;

                    int z = p.Z;

                    //int landZ = 0, landAvg = 0, landTop = 0;

                    //map.GetAverageZ( tx, ty, ref landZ, ref landAvg, ref landTop );

                    //if ( !landTile.Ignored && top > landZ && landTop > z )
                    //	return false;

                    for (int i = 0; i < tiles.Length; ++i)
                    {
                        StaticTile tile = tiles[i];

                        if (IsExcludedTile(tile))
                            continue;

                        bool isWater = (tile.ID >= 0x1796 && tile.ID <= 0x17B2);

                        if (tile.Z == p.Z && isWater)
                            hasWater = true;
                        else if (tile.Z >= p.Z && !isWater)
                            return false;
                    }

                    if (!hasWater)
                        return false;
                }
            }

            IPooledEnumerable eable = map.GetItemsInBounds(new Rectangle2D(p.X + newComponents.Min.X, p.Y + newComponents.Min.Y, newComponents.Width, newComponents.Height));

            foreach (Item item in eable)
            {
                if (CheckItem(itemID, item, p) || CanMoveOver(item) || item.Z < p.Z || ExemptOverheadComponent(p, itemID, item.X, item.Y, item.Z + item.ItemData.Height))
                    continue;

                int x = item.X - p.X + newComponents.Min.X;
                int y = item.Y - p.Y + newComponents.Min.Y;

                if (x >= 0 && x < newComponents.Width && y >= 0 && y < newComponents.Height && newComponents.Tiles[x][y].Length == 0)
                    continue;

                eable.Free();
                return false;
            }

            eable.Free();

            IPooledEnumerable mobiles = map.GetMobilesInBounds(new Rectangle2D(p.X + newComponents.Min.X, p.Y + newComponents.Min.Y, newComponents.Width, newComponents.Height));

            foreach (Mobile mobile in mobiles)
            {
                if (mobile is BaseSeaChampion)
                {
                    mobiles.Free();
                    return false;
                }
            }

            mobiles.Free();

            return true;
        }

        public virtual bool CheckItem(int itemID, Item item, Point3D p)
        {
            return Contains(item) || item is BaseMulti || item.ItemID > TileData.MaxItemValue || !item.Visible || /*[s7] Fix Corpses Block Boats*/  item is Corpse || IsComponentItem((ISpawnable)item) || item is EffectItem;
        }

        public virtual bool CanMoveOver(Item item)
        {
            return item is Blood;
        }

        public virtual bool IsExcludedTile(StaticTile tile)
        {
            return false;
        }

        public virtual bool IsExcludedTile(StaticTile[] tile)
        {
            return false;
        }

        public virtual void OnPlacement(Mobile from)
        {
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

        public override bool Contains(int x, int y)
        {
            if (base.Contains(x, y))
                return true;

            if (m_TillerMan != null)
            {
                if (m_TillerMan is Mobile)
                {
                    if (x == ((Mobile)m_TillerMan).X && y == ((Mobile)m_TillerMan).Y)
                        return true;
                }
                else if (m_TillerMan is Item)
                {
                    if (x == ((Item)m_TillerMan).X && y == ((Item)m_TillerMan).Y)
                        return true;
                }
            }
            if (m_Hold != null && x == m_Hold.X && y == m_Hold.Y)
                return true;

            if (m_PPlank != null && x == m_PPlank.X && y == m_PPlank.Y)
                return true;

            if (m_SPlank != null && x == m_SPlank.X && y == m_SPlank.Y)
                return true;

            return false;
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

        public Direction GetMovementFor(int x, int y, out int maxSpeed)
        {
            int dx = x - this.X;
            int dy = y - this.Y;

            int adx = Math.Abs(dx);
            int ady = Math.Abs(dy);

            Direction dir = Utility.GetDirection(this, new Point2D(x, y));
            int iDir = (int)dir;

            // Compute the maximum distance we can travel without going too far away
            if (iDir % 2 == 0) // North, East, South and West
                maxSpeed = Math.Abs(adx - ady);
            else // Right, Down, Left and Up
                maxSpeed = Math.Min(adx, ady);

            return (Direction)((iDir - (int)Facing) & 0x7);
        }

        public bool DoMovement(bool message, bool single)
        {
            Direction dir;
            int speed, clientSpeed;

            if (this.Order == BoatOrder.Move)
            {
                dir = this.Moving;
                speed = this.Speed;
                clientSpeed = m_ClientSpeed;
            }
            else if (m_BoatCourse == null)
            {
                if (message && TillerMan != null)
                    TillerManSay(502513); // I have seen no map, sir.

                return false;
            }
            else if (this.Map != m_BoatCourse.Map)
            {
                if (message && TillerMan != null)
                    TillerManSay(502514); // The map is too far away from me, sir.

                return false;
            }
            else if ((this.Map != Map.Trammel && this.Map != Map.Felucca && this.Map != Map.Tokuno) || NextNavPoint < 0 || NextNavPoint >= m_BoatCourse.Waypoints.Count)
            {
                if (message && TillerMan != null)
                    TillerManSay(1042551); // I don't see that navpoint, sir.

                return false;
            }
            else
            {
                Point2D dest = m_BoatCourse.Waypoints[NextNavPoint];

                int maxSpeed;
                dir = GetMovementFor(dest.X, dest.Y, out maxSpeed);

                if (maxSpeed == 0)
                {
                    if (message && this.Order == BoatOrder.Single && TillerMan != null)
                        TillerManSay(1042874, (NextNavPoint + 1).ToString()); // We have arrived at nav point ~1_POINT_NUM~ , sir.

                    if (NextNavPoint + 1 < m_BoatCourse.Waypoints.Count)
                    {
                        NextNavPoint++;

                        if (this.Order == BoatOrder.Course)
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

                        if (message && this.Order == BoatOrder.Course && TillerMan != null)
                            TillerManSay(502515); // The course is completed, sir.

                        if (Owner is BaseShipCaptain)
                            Server.Engines.Quests.BountyQuestSpawner.ResetNavPoints(this);

                        return false;
                    }
                }

                if (dir == Left || dir == BackwardLeft || dir == Backward)
                    return Turn(-2, true);
                else if (dir == Right || dir == BackwardRight)
                    return Turn(2, true);

                speed = Math.Min(this.Speed, maxSpeed);
                clientSpeed = 0x4;
            }

            return Move(dir, speed, clientSpeed, true);
        }

        private object m_Lock = new object();

        public bool Move(Direction dir, int speed, int clientSpeed, bool message)
        {
            Map map = Map;

            if (map == null || Deleted || CheckDecay() || Scuttled)
                return false;

            int rx = 0, ry = 0;
            Direction d;

            if (m_Pilot == null)
            {
                d = (Direction)(((int)m_Facing + (int)dir) & 0x7);
                Movement.Movement.Offset((Direction)(((int)m_Facing + (int)dir) & 0x7), ref rx, ref ry);
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
                        if (message && m_TillerMan != null)
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
                            if (message && m_TillerMan != null)
                                TillerManSay(501424); // Ar, we've stopped sir.

                            return false;
                        }
                    }

                    xOffset = newX - X;
                    yOffset = newY - Y;
                }
            }

            if (!NewBoatMovement || Math.Abs(xOffset) > 1 || Math.Abs(yOffset) > 1)
            {
                Teleport(xOffset, yOffset, 0);
            }
            else
            {
                List<ISpawnable> toMove = GetMovingEntities();

                if (m_TillerMan != null && m_TillerMan is ISpawnable && !toMove.Contains((ISpawnable)m_TillerMan))
                    toMove.Add((ISpawnable)m_TillerMan);

                MoveBoatHS smooth = new MoveBoatHS(this, d, clientSpeed, toMove, xOffset, yOffset);
                smooth.SetStatic();

                // Packet must be sent before actual locations are changed
                IPooledEnumerable eable = Map.GetClientsInRange(Location, GetMaxUpdateRange());
                foreach (NetState ns in eable)
                {
                    Mobile m = ns.Mobile;

                    if (m == null || !m.CanSee(this))
                        continue;

                    if (m.InRange(Location, GetUpdateRange(m)))
                        ns.Send(smooth);
                }

                eable.Free();
                smooth.Release();

                foreach (ISpawnable e in toMove)
                {
                    if (e is Item)
                    {
                        Item item = (Item)e;

                        item.NoMoveHS = true;

                        if (!IsComponentItem((ISpawnable)item) && (!CanMoveOver(item)))
                            item.Location = new Point3D(item.X + xOffset, item.Y + yOffset, item.Z);
                    }
                    else if (e is Mobile)
                    {
                        Mobile m = (Mobile)e;

                        m.NoMoveHS = true;

                        if (!IsComponentItem((ISpawnable)m) && !(m is BaseSeaChampion))
                            m.Location = new Point3D(m.X + xOffset, m.Y + yOffset, m.Z);
                    }
                }

                NoMoveHS = true;
                Location = new Point3D(X + xOffset, Y + yOffset, Z);

                foreach (ISpawnable e in toMove)
                {
                    if (e is Item)
                        ((Item)e).NoMoveHS = false;
                    else if (e is Mobile)
                        ((Mobile)e).NoMoveHS = false;
                }

                NoMoveHS = false;
                toMove.Clear();
                toMove.TrimExcess();
            }

            return true;
        }

        public void Teleport(int xOffset, int yOffset, int zOffset)
        {
            List<ISpawnable> toMove = GetMovingEntities();

            Location = new Point3D(X + xOffset, Y + yOffset, Z + zOffset);

            foreach (ISpawnable e in toMove)
            {
                if (e is Item)
                {
                    Item item = (Item)e;

                    if (IsComponentItem((ISpawnable)item) || (CanMoveOver(item)))
                        continue;

                    item.Location = new Point3D(item.X + xOffset, item.Y + yOffset, item.Z + zOffset);
                }
                else if (e is Mobile)
                {
                    Mobile m = (Mobile)e;

                    if (IsComponentItem((ISpawnable)m) || m is BaseSeaChampion || m == m_TillerMan)
                        continue;

                    m.Location = new Point3D(m.X + xOffset, m.Y + yOffset, m.Z + zOffset);
                }
            }

            toMove.Clear();
            toMove.TrimExcess();
        }

        public virtual bool SetFacing(Direction facing)
        {
            if (Parent != null || this.Map == null)
                return false;

            if (CheckDecay())
                return false;

            if (Map != Map.Internal)
            {
                switch (facing)
                {
                    case Direction.North: if (!CanFit(Location, Map, NorthID)) return false; break;
                    case Direction.East: if (!CanFit(Location, Map, EastID)) return false; break;
                    case Direction.South: if (!CanFit(Location, Map, SouthID)) return false; break;
                    case Direction.West: if (!CanFit(Location, Map, WestID)) return false; break;
                }
            }

            this.Map.OnLeave(this);

            Direction old = m_Facing;
            List<object> toMove = new List<object>();

            if (m_TillerMan is GalleonPilot)
                ((GalleonPilot)m_TillerMan).SetFacing(facing);
            else if (m_TillerMan is TillerMan)
                ((TillerMan)m_TillerMan).SetFacing(facing);

            if (m_Hold != null)
                m_Hold.SetFacing(facing);

            if (m_PPlank != null)
                m_PPlank.SetFacing(facing);

            if (m_SPlank != null)
                m_SPlank.SetFacing(facing);

            MultiComponentList mcl = Components;

            if (m_PPlank != null)
                toMove.Add(m_PPlank);

            if (m_SPlank != null)
                toMove.Add(m_SPlank);

            IPooledEnumerable eable = Map.GetObjectsInBounds(new Rectangle2D(X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height));

            foreach (object o in eable)
            {
                if (o is Item)
                {
                    Item item = (Item)o;

                    if (CanMoveOver(item))
                        continue;

                    if (item != this && Contains(item) && item.Visible && item.Z >= Z && !(item is TillerMan || item is Hold || item is Plank || item is BaseAddon || item is RudderHandle))
                        toMove.Add(o);
                }
                else if (o is Mobile && Contains((Mobile)o))
                {
                    toMove.Add(o);
                    ((Mobile)o).Direction = (Direction)((int)((Mobile)o).Direction - (int)old + (int)facing);
                }
            }

            List<Item> components = GetComponents();
            if (components != null)
            {
                foreach (Item item in components)
                {
                    if (!toMove.Contains(item))
                        toMove.Add(item as object);
                }
            }

            eable.Free();

            m_Facing = facing;
            int xOffset = 0, yOffset = 0;
            Movement.Movement.Offset(facing, ref xOffset, ref yOffset);

            if (m_TillerMan != null && m_TillerMan is Item)
                ((Item)m_TillerMan).Location = new Point3D(X + (xOffset * TillerManDistance) + (facing == Direction.North ? 1 : 0), Y + (yOffset * TillerManDistance), ((Item)m_TillerMan).Z);

            if (m_Hold != null)
                m_Hold.Location = new Point3D(X + (xOffset * HoldDistance), Y + (yOffset * HoldDistance), m_Hold.Z);

            int count = (int)(m_Facing - old) & 0x7;
            count /= 2;

            foreach (object o in toMove)
            {
                if (o is Item)
                    ((Item)o).Location = Rotate(((Item)o).Location, count);
                else if (o is Mobile)
                    ((Mobile)o).Location = Rotate(((Mobile)o).Location, count);
            }

            switch (facing)
            {
                case Direction.North: ItemID = NorthID; break;
                case Direction.East: ItemID = EastID; break;
                case Direction.South: ItemID = SouthID; break;
                case Direction.West: ItemID = WestID; break;
            }

            this.Map.OnEnter(this);
            SetFacingComponents(m_Facing, old, false);

            return true;
        }

        private class MoveTimer : Timer
        {
            private BaseBoat m_Boat;
            private bool m_SingleMove;

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

        public static void UpdateAllComponents()
        {
            List<BaseBoat> toDelete = new List<BaseBoat>();

            for (int i = m_Instances.Count - 1; i >= 0; --i)
            {
                BaseBoat boat = m_Instances[i];

                boat.UpdateComponents();
                List<Mobile> list = boat.GetMobilesOnBoard();

                foreach (Mobile m in list)
                {
                    if (m is PlayerMobile)
                    {
                        boat.Refresh();
                        break;
                    }
                }

                list.Clear();
                list.TrimExcess();
            }

            foreach (BaseBoat b in toDelete)
                b.Delete();

            toDelete.Clear();
            toDelete.TrimExcess();
        }

        public void InvalidateTillerManProperties()
        {
            if (TillerMan is Mobile)
                ((Mobile)TillerMan).InvalidateProperties();
            else if (TillerMan is Item)
                ((Item)TillerMan).InvalidateProperties();
        }

        public static void Initialize()
        {
            new UpdateAllTimer().Start();
            EventSink.WorldSave += new WorldSaveEventHandler(EventSink_WorldSave);
            EventSink.Disconnected += new DisconnectedEventHandler(EventSink_Disconnected);
            EventSink.PlayerDeath += new PlayerDeathEventHandler(EventSink_PlayerDeath);
        }

        private static void EventSink_WorldSave(WorldSaveEventArgs e)
        {
            new UpdateAllTimer().Start();
        }

        #region High Seas
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

        public void LockPilot(Mobile pilot)
        {
            m_Pilot = pilot;
            pilot.Direction = m_Facing;
            pilot.AddItem(m_VirtualMount);

            pilot.SendLocalizedMessage(1116727); //You are now piloting this vessel.

            if (IsMoving)
                StopMove(false);
        }

        public void RemovePilot(Mobile from)
        {
            if (m_Pilot != from)
                return;

            m_Pilot.RemoveItem(m_VirtualMount);
            m_VirtualMount.Internalize();

            m_Pilot = null;

            if (IsMoving)
                StopMove(false);

            from.SendLocalizedMessage(1149592); //You are no longer piloting this vessel.
        }

        public static bool IsDriving(Mobile from)
        {
            foreach (BaseBoat boat in m_Instances)
            {
                if (boat.Pilot == from)
                    return true;
            }

            return false;
        }

        public static BaseBoat GetPiloting(Mobile from)
        {
            BaseBoat boat = FindBoatAt(from, from.Map);

            if (boat != null && boat.Pilot == from)
                return boat;

            return null;
        }

        public virtual void OnMousePilotCommand(Mobile from, Direction d, int rawSpeed)
        {
            int clientSpeed = 0;
            int actualDir = (m_Facing - d) & 0x7;
            TimeSpan interval = GetMovementInterval(rawSpeed, out clientSpeed);

            if (rawSpeed > 1 && actualDir > 1 && actualDir != 7)
            {
                Direction turnDirection = (int)d % 2 != 0 ? d - 1 : d;
                StartTurn((int)turnDirection, false);
            }

            if (!StartMove(d, 1, clientSpeed, interval, false, false))
                StopMove(false);
        }

        public static void EventSink_Disconnected(DisconnectedEventArgs e)
        {
            BaseBoat boat = GetPiloting(e.Mobile);

            if (boat != null)
                boat.RemovePilot(e.Mobile);
        }

        public static void EventSink_PlayerDeath(PlayerDeathEventArgs e)
        {
            BaseBoat boat = GetPiloting(e.Mobile);

            if (boat != null)
                boat.RemovePilot(e.Mobile);
        }

        public void SendMessageToAllOnBoard(object message)
        {
            List<Mobile> list = GetMobilesOnBoard();

            foreach(Mobile m in list.Where(mobile => mobile is PlayerMobile && ((PlayerMobile)mobile).NetState != null))
            {
                if (message is int)
                    m.SendLocalizedMessage((int)message);
                else if (message is string)
                    m.SendMessage((string)message);
            }

            list.Clear();
            list.TrimExcess();
        }

        public virtual void SetFacingComponents(Direction newDirection, Direction oldDirection, bool ignoreLastFacing)
        {
        }

        public virtual List<Item> GetComponents()
        {
            return null;
        }

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

        public virtual bool IsComponentItem(ISpawnable item)
        {
            if (item == null)
                return false;

            if (item == this || (m_TillerMan is Item && item == (Item)m_TillerMan) || item == m_SPlank || item == m_PPlank || item == m_Hold)
                return true;
            return false;
        }

        public List<ISpawnable> GetMovingEntities()
        {
            List<ISpawnable> list = new List<ISpawnable>();

            Map map = Map;

            if (map == null || map == Map.Internal)
                return list;

            MultiComponentList mcl = Components;

            IPooledEnumerable eable = map.GetObjectsInBounds(new Rectangle2D(X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height));
            foreach (object o in eable)
            {
                if (o is Item)
                {
                    Item item = (Item)o;

                    if (Contains(item) && item.Visible && item.Z >= Z)
                        list.Add(item);
                }
                else if (o is Mobile)
                {
                    Mobile m = (Mobile)o;

                    if (Contains(m))
                        list.Add(m);
                }
            }
            eable.Free();
            return list;
        }

        public List<Item> GetItemsOnBoard()
        {
            List<Item> list = new List<Item>();
            List<ISpawnable> spawnables = GetObjectsOnBoard();

            foreach(ISpawnable s in spawnables.Where(spawnable => spawnable is Item))
            {
                list.Add(s as Item);
            }

            spawnables.Clear();
            spawnables.TrimExcess();

            return list;
        }

        public List<ISpawnable> GetObjectsOnBoard()
        {
            List<ISpawnable> list = new List<ISpawnable>();

            if (this.Map == null || this.Map == Map.Internal)
                return list;

            MultiComponentList mcl = Components;
            IPooledEnumerable eable = this.Map.GetObjectsInBounds(new Rectangle2D(X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height));

            foreach (object o in eable)
            {
                if (o != this && o is ISpawnable && !list.Contains((ISpawnable)o))
                    list.Add((ISpawnable)o);
            }
            eable.Free();

            return list;
        }

        public List<Mobile> GetMobilesOnBoard()
        {
            List<Mobile> list = new List<Mobile>();
            List<ISpawnable> spawnables = GetObjectsOnBoard();

            foreach (ISpawnable s in spawnables.Where(spawnable => spawnable is Mobile && this.Contains(spawnable.X, spawnable.Z)))
            {
                list.Add(s as Mobile);
            }

            spawnables.Clear();
            spawnables.TrimExcess();

            return list;
        }

        public void TillerManSay(object message)
        {
            if (message is int)
            {
                if (m_TillerMan is Mobile)
                    ((Mobile)m_TillerMan).Say((int)message);
                else if (m_TillerMan is Server.Items.TillerMan)
                    ((Server.Items.TillerMan)m_TillerMan).Say((int)message);
            }
            else if (message is string)
            {
                if (m_TillerMan is Mobile)
                    ((Mobile)m_TillerMan).Say((string)message);
                else if (m_TillerMan is Server.Items.TillerMan)
                    ((Server.Items.TillerMan)m_TillerMan).Say(1060658, (string)message);
            }
        }

        public void TillerManSay(object message, string arg)
        {
            if (message is int)
            {
                if (m_TillerMan is Mobile)
                    ((Mobile)m_TillerMan).Say((int)message, arg);
                else if (m_TillerMan is Server.Items.TillerMan)
                    ((Server.Items.TillerMan)m_TillerMan).Say((int)message, arg);
            }
            else if (message is string)
            {
                if (m_TillerMan is Mobile)
                    ((Mobile)m_TillerMan).Say((string)message);
                else if (m_TillerMan is Server.Items.TillerMan)
                    ((Server.Items.TillerMan)m_TillerMan).Say(1060658, (string)message);
            }
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

        public void ForceDecay()
        {
            new DecayTimer(this).Start();
            m_Decaying = true;
        }

        private class DecayTimer : Timer
        {
            private BaseBoat m_Boat;
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

        public bool CheckDecay()
        {
            if (this.Map == Map.Internal)
                return false;

            List<Mobile> list = GetMobilesOnBoard();

            foreach (Mobile m in list)
            {
                if (m is PlayerMobile)
                {
                    list.Clear();
                    list.TrimExcess();
                    return false;
                }
            }

            list.Clear();
            list.TrimExcess();

            if (m_Decaying)
                return true;

            if (m_Decay && !IsMoving && DateTime.UtcNow >= m_DecayTime)
            {
                new DecayTimer(this).Start();

                m_Decaying = true;

                return true;
            }

            return false;
        }

        public virtual void OnSink()
        {
            m_Decaying = false;

            if (CanLinkToLighthouse)
            {
                var addon = LighthouseAddon.GetLighthouse(Owner);

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

        /*
		 * OSI sends the 0xF7 packet instead, holding 0xF3 packets
		 * for every entity on the boat. Though, the regular 0xF3
		 * packets are still being sent as well as entities come
		 * into sight. Do we really need it?

        public virtual Packet SendDisplayBoatPacket(NetState state)
        {
            if (NewBoatMovement && state.HighSeas)
                return new DisplayBoatHS(state.Mobile, this);
            else
                return base.GetWorldPacketFor(state);
        }*/

        private Packet m_ContainerPacket;

        private object _PacketLock = new object();

        public override Packet WorldPacketHS
        {
            get
            {
                if (NewBoatMovement)
                {
                    lock (_PacketLock)
                    {
                        if (m_ContainerPacket == null)
                        {
                            m_ContainerPacket = new DisplayBoatHS(this);
                            m_ContainerPacket.SetStatic();
                        }
                    }

                    return m_ContainerPacket;
                }

                return base.WorldPacketHS;
            }
        }

        public override void ReleaseWorldPackets()
        {
            if (NewBoatMovement)
                Packet.Release(ref m_ContainerPacket);

            base.ReleaseWorldPackets(); 
        }

        public sealed class MoveBoatHS : Packet
        {
            public MoveBoatHS(BaseBoat boat, Direction d, int speed, List<ISpawnable> ents, int xOffset, int yOffset)
                : base(0xF6)
            {
                EnsureCapacity(3 + 15 + ents.Count * 10);

                m_Stream.Write((int)boat.Serial);
                m_Stream.Write((byte)speed);
                m_Stream.Write((byte)d);
                m_Stream.Write((byte)boat.Facing);
                m_Stream.Write((short)(boat.X + xOffset));
                m_Stream.Write((short)(boat.Y + yOffset));
                m_Stream.Write((short)boat.Z);
                m_Stream.Write((short)0); // count placeholder

                int count = 0;

                foreach (ISpawnable ent in ents)
                {
                    m_Stream.Write((int)ent.Serial);
                    m_Stream.Write((short)(ent.X + xOffset));
                    m_Stream.Write((short)(ent.Y + yOffset));
                    m_Stream.Write((short)ent.Z);
                    ++count;
                }

                m_Stream.Seek(16, System.IO.SeekOrigin.Begin);
                m_Stream.Write((short)count);
            }
        }

        public sealed class DisplayBoatHS : Packet
        {
            public DisplayBoatHS(BaseBoat boat) : base(0xF7)
            {
                List<ISpawnable> ents = boat.GetMovingEntities();

                EnsureCapacity(5 + ents.Count * 26);
                m_Stream.Write((short)ents.Count);

                new WorldItemHS(boat, m_Stream);

                for (int i = 0; i < ents.Count; i++)
                {
                    if (ents[i] is Item)
                        new WorldItemHS((Item)ents[i], m_Stream);
                    else if (ents[i] is Mobile)
                        new WorldItemHS((Mobile)ents[i], m_Stream);
                }
            }
        }

        public virtual TimeSpan GetMovementInterval(int speed, out int clientSpeed)
        {
            switch (speed)
            {
                default: clientSpeed = 0x0; return TimeSpan.Zero;
                case 1: clientSpeed = 0x2; return SlowInterval;
                case 2: clientSpeed = 0x4; return FastInterval;
            }
        }

        public virtual TimeSpan GetMovementInterval(bool fast, bool drifting, out int clientSpeed)
        {
            if (fast)
            {
                if (drifting)
                {
                    clientSpeed = 0x3;
                    return NormalInterval;
                }

                clientSpeed = 0x4;
                return FastInterval;
            }
            else
            {
                if (drifting)
                {
                    clientSpeed = 0x2;
                    return SlowInterval;
                }

                clientSpeed = 0x3;
                return NormalInterval;
            }
        }
        #endregion

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
        private List<Point2D> m_Waypoints = new List<Point2D>();
        public List<Point2D> Waypoints { get { return m_Waypoints; } }

        private BaseBoat m_Boat;
        private Map m_Map;
        private bool m_GivenMap;

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseBoat Boat { get { return m_Boat; } set { m_Boat = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map Map { get { return m_Map; } set { m_Map = value ; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int NumWaypoints { get { return m_Waypoints.Count; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool GivenMap { get { return m_GivenMap; } set { m_GivenMap = value; } }

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
                    if (m_Boat != null)
                    {
                        MapItem mapItem = new MapItem(m_Map);

                        if (m_Map == Map.Tokuno)
                            mapItem.SetDisplay(5, 5, 1448 - 32, 1448 - 10, 400, 400);
                        else
                            mapItem.SetDisplay(5, 5, 5120 - 32, 4096 - 10, 400, 400);

                        for (int i = 0; i < m_Waypoints.Count; i++)
                            mapItem.AddWorldPin(m_Waypoints[i].X, m_Waypoints[i].Y);

                        if (m_Boat is BaseGalleon)
                        {
                            ((BaseGalleon)m_Boat).GalleonHold.DropItem(mapItem);
                        }
                        else
                            m_Boat.Hold.DropItem(mapItem);
                    }
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point2D CurrentWaypoint
        {
            get
            {
                if(m_Boat != null && m_Boat.NextNavPoint >= 0 && m_Boat.NextNavPoint < m_Waypoints.Count)
                    return m_Waypoints[m_Boat.NextNavPoint];

                return Point2D.Zero;
            }
        }

        public BoatCourse(BaseBoat boat)
        {
            m_Boat = boat;
            m_Map = boat.Map;
            m_GivenMap = false;
        }

        public BoatCourse(BaseBoat boat, MapItem item)
        {
            m_Boat = boat;
            m_Map = boat.Map;
            m_GivenMap = false;

            for (int i = 0; i < item.Pins.Count; i++)
            {
                int x, y;
                item.ConvertToWorld(item.Pins[i].X, item.Pins[i].Y, out x, out y);
                m_Waypoints.Add(new Point2D(x, y));
            }
        }

        public BoatCourse(BaseBoat boat, List<Point2D> list)
        {
            m_Boat = boat;
            m_Map = boat.Map;
            m_GivenMap = false;

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

                if (m_Map == course.Map && m_Boat == course.Boat && course.Waypoints.Count == m_Waypoints.Count)
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

            m_GivenMap = reader.ReadBool();

            int c = reader.ReadInt();
            for (int i = 0; i < c; i++)
                m_Waypoints.Add(reader.ReadPoint2D());
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0);

            writer.Write(m_GivenMap);

            writer.Write(m_Waypoints.Count);
            foreach (Point2D p in m_Waypoints)
                writer.Write(p);
        }
    }
}
