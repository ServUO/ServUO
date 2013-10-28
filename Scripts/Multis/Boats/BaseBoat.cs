using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Network;

namespace Server.Multis
{
    public enum BoatOrder
    {
        Move,
        Course,
        Single
    }

    public abstract class BaseBoat : BaseMulti
    {
        private static readonly Rectangle2D[] m_BritWrap = new Rectangle2D[] { new Rectangle2D(16, 16, 5120 - 32, 4096 - 32), new Rectangle2D(5136, 2320, 992, 1760) };
        private static readonly Rectangle2D[] m_IlshWrap = new Rectangle2D[] { new Rectangle2D(16, 16, 2304 - 32, 1600 - 32) };
        private static readonly Rectangle2D[] m_TokunoWrap = new Rectangle2D[] { new Rectangle2D(16, 16, 1448 - 32, 1448 - 32) };
        private static readonly TimeSpan BoatDecayDelay = TimeSpan.FromDays(9.0);
        private static readonly List<BaseBoat> m_Instances = new List<BaseBoat>();
        private static readonly TimeSpan SlowInterval = TimeSpan.FromSeconds(0.75);
        private static readonly TimeSpan FastInterval = TimeSpan.FromSeconds(0.75);
        private static readonly int SlowSpeed = 1;
        private static readonly int FastSpeed = 3;
        private static readonly TimeSpan SlowDriftInterval = TimeSpan.FromSeconds(1.50);
        private static readonly TimeSpan FastDriftInterval = TimeSpan.FromSeconds(0.75);
        private static readonly int SlowDriftSpeed = 1;
        private static readonly int FastDriftSpeed = 1;
        private static readonly Direction Forward = Direction.North;
        private static readonly Direction ForwardLeft = Direction.Up;
        private static readonly Direction ForwardRight = Direction.Right;
        private static readonly Direction Backward = Direction.South;
        private static readonly Direction BackwardLeft = Direction.Left;
        private static readonly Direction BackwardRight = Direction.Down;
        private static readonly Direction Left = Direction.West;
        private static readonly Direction Right = Direction.East;
        private static readonly Direction Port = Left;
        private static readonly Direction Starboard = Right;
        private Hold m_Hold;
        private TillerMan m_TillerMan;
        private Mobile m_Owner;
        private Direction m_Facing;
        private Direction m_Moving;
        private int m_Speed;
        private bool m_Anchored;
        private string m_ShipName;
        private BoatOrder m_Order;
        private MapItem m_MapItem;
        private int m_NextNavPoint;
        private Plank m_PPlank, m_SPlank;
        private DateTime m_DecayTime;
        private Timer m_TurnTimer;
        private Timer m_MoveTimer;
        private bool m_Decaying;
        public BaseBoat()
            : base(0x0)
        {
            this.m_DecayTime = DateTime.UtcNow + BoatDecayDelay;

            this.m_TillerMan = new TillerMan(this);
            this.m_Hold = new Hold(this);

            this.m_PPlank = new Plank(this, PlankSide.Port, 0);
            this.m_SPlank = new Plank(this, PlankSide.Starboard, 0);

            this.m_PPlank.MoveToWorld(new Point3D(this.X + this.PortOffset.X, this.Y + this.PortOffset.Y, this.Z), this.Map);
            this.m_SPlank.MoveToWorld(new Point3D(this.X + this.StarboardOffset.X, this.Y + this.StarboardOffset.Y, this.Z), this.Map);

            this.Facing = Direction.North;

            this.m_NextNavPoint = -1;

            this.Movable = false;

            m_Instances.Add(this);
        }

        public BaseBoat(Serial serial)
            : base(serial)
        {
        }

        public enum DryDockResult
        {
            Valid,
            Dead,
            NoKey,
            NotAnchored,
            Mobiles,
            Items,
            Hold,
            Decaying
        }
        public static List<BaseBoat> Boats
        {
            get
            {
                return m_Instances;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Hold Hold
        {
            get
            {
                return this.m_Hold;
            }
            set
            {
                this.m_Hold = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TillerMan TillerMan
        {
            get
            {
                return this.m_TillerMan;
            }
            set
            {
                this.m_TillerMan = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Plank PPlank
        {
            get
            {
                return this.m_PPlank;
            }
            set
            {
                this.m_PPlank = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Plank SPlank
        {
            get
            {
                return this.m_SPlank;
            }
            set
            {
                this.m_SPlank = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get
            {
                return this.m_Owner;
            }
            set
            {
                this.m_Owner = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Facing
        {
            get
            {
                return this.m_Facing;
            }
            set
            {
                this.SetFacing(value);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Moving
        {
            get
            {
                return this.m_Moving;
            }
            set
            {
                this.m_Moving = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsMoving
        {
            get
            {
                return (this.m_MoveTimer != null);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Speed
        {
            get
            {
                return this.m_Speed;
            }
            set
            {
                this.m_Speed = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Anchored
        {
            get
            {
                return this.m_Anchored;
            }
            set
            {
                this.m_Anchored = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string ShipName
        {
            get
            {
                return this.m_ShipName;
            }
            set
            {
                this.m_ShipName = value;
                if (this.m_TillerMan != null)
                    this.m_TillerMan.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public BoatOrder Order
        {
            get
            {
                return this.m_Order;
            }
            set
            {
                this.m_Order = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public MapItem MapItem
        {
            get
            {
                return this.m_MapItem;
            }
            set
            {
                this.m_MapItem = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int NextNavPoint
        {
            get
            {
                return this.m_NextNavPoint;
            }
            set
            {
                this.m_NextNavPoint = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime TimeOfDecay
        {
            get
            {
                return this.m_DecayTime;
            }
            set
            {
                this.m_DecayTime = value;
                if (this.m_TillerMan != null)
                    this.m_TillerMan.InvalidateProperties();
            }
        }
        public int Status
        {
            get
            {
                DateTime start = this.TimeOfDecay - BoatDecayDelay;

                if (DateTime.UtcNow - start < TimeSpan.FromHours(1.0))
                    return 1043010; // This structure is like new.

                if (DateTime.UtcNow - start < TimeSpan.FromDays(2.0))
                    return 1043011; // This structure is slightly worn.

                if (DateTime.UtcNow - start < TimeSpan.FromDays(3.0))
                    return 1043012; // This structure is somewhat worn.

                if (DateTime.UtcNow - start < TimeSpan.FromDays(4.0))
                    return 1043013; // This structure is fairly worn.

                if (DateTime.UtcNow - start < TimeSpan.FromDays(5.0))
                    return 1043014; // This structure is greatly worn.

                return 1043015; // This structure is in danger of collapsing.
            }
        }
        public virtual int NorthID
        {
            get
            {
                return 0;
            }
        }
        public virtual int EastID
        {
            get
            {
                return 0;
            }
        }
        public virtual int SouthID
        {
            get
            {
                return 0;
            }
        }
        public virtual int WestID
        {
            get
            {
                return 0;
            }
        }
        public virtual int HoldDistance
        {
            get
            {
                return 0;
            }
        }
        public virtual int TillerManDistance
        {
            get
            {
                return 0;
            }
        }
        public virtual Point2D StarboardOffset
        {
            get
            {
                return Point2D.Zero;
            }
        }
        public virtual Point2D PortOffset
        {
            get
            {
                return Point2D.Zero;
            }
        }
        public virtual Point3D MarkOffset
        {
            get
            {
                return Point3D.Zero;
            }
        }
        public virtual BaseDockedBoat DockedBoat
        {
            get
            {
                return null;
            }
        }
        public override bool HandlesOnSpeech
        {
            get
            {
                return true;
            }
        }
        public static BaseBoat FindBoatAt(IPoint2D loc, Map map)
        {
            Sector sector = map.GetSector(loc);

            for (int i = 0; i < sector.Multis.Count; i++)
            {
                BaseBoat boat = sector.Multis[i] as BaseBoat;

                if (boat != null && boat.Contains(loc.X, loc.Y))
                    return boat;
            }

            return null;
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

        public static void UpdateAllComponents()
        {
            for (int i = m_Instances.Count - 1; i >= 0; --i)
                m_Instances[i].UpdateComponents();
        }

        public static void Initialize()
        {
            new UpdateAllTimer().Start();
            EventSink.WorldSave += new WorldSaveEventHandler(EventSink_WorldSave);
        }

        public Point3D GetRotatedLocation(int x, int y)
        {
            Point3D p = new Point3D(this.X + x, this.Y + y, this.Z);

            return this.Rotate(p, (int)this.m_Facing / 2);
        }

        public void UpdateComponents()
        {
            if (this.m_PPlank != null)
            {
                this.m_PPlank.MoveToWorld(this.GetRotatedLocation(this.PortOffset.X, this.PortOffset.Y), this.Map);
                this.m_PPlank.SetFacing(this.m_Facing);
            }

            if (this.m_SPlank != null)
            {
                this.m_SPlank.MoveToWorld(this.GetRotatedLocation(this.StarboardOffset.X, this.StarboardOffset.Y), this.Map);
                this.m_SPlank.SetFacing(this.m_Facing);
            }

            int xOffset = 0, yOffset = 0;
            Movement.Movement.Offset(this.m_Facing, ref xOffset, ref yOffset);

            if (this.m_TillerMan != null)
            {
                this.m_TillerMan.Location = new Point3D(this.X + (xOffset * this.TillerManDistance) + (this.m_Facing == Direction.North ? 1 : 0), this.Y + (yOffset * this.TillerManDistance), this.m_TillerMan.Z);
                this.m_TillerMan.SetFacing(this.m_Facing);
                this.m_TillerMan.InvalidateProperties();
            }

            if (this.m_Hold != null)
            {
                this.m_Hold.Location = new Point3D(this.X + (xOffset * this.HoldDistance), this.Y + (yOffset * this.HoldDistance), this.m_Hold.Z);
                this.m_Hold.SetFacing(this.m_Facing);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3);

            writer.Write((Item)this.m_MapItem);
            writer.Write((int)this.m_NextNavPoint);

            writer.Write((int)this.m_Facing);

            writer.WriteDeltaTime(this.m_DecayTime);

            writer.Write(this.m_Owner);
            writer.Write(this.m_PPlank);
            writer.Write(this.m_SPlank);
            writer.Write(this.m_TillerMan);
            writer.Write(this.m_Hold);
            writer.Write(this.m_Anchored);
            writer.Write(this.m_ShipName);

            this.CheckDecay();
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 3:
                    {
                        this.m_MapItem = (MapItem)reader.ReadItem();
                        this.m_NextNavPoint = reader.ReadInt();

                        goto case 2;
                    }
                case 2:
                    {
                        this.m_Facing = (Direction)reader.ReadInt();

                        goto case 1;
                    }
                case 1:
                    {
                        this.m_DecayTime = reader.ReadDeltaTime();

                        goto case 0;
                    }
                case 0:
                    {
                        if (version < 3)
                            this.m_NextNavPoint = -1;

                        if (version < 2)
                        {
                            if (this.ItemID == this.NorthID)
                                this.m_Facing = Direction.North;
                            else if (this.ItemID == this.SouthID)
                                this.m_Facing = Direction.South;
                            else if (this.ItemID == this.EastID)
                                this.m_Facing = Direction.East;
                            else if (this.ItemID == this.WestID)
                                this.m_Facing = Direction.West;
                        }

                        this.m_Owner = reader.ReadMobile();
                        this.m_PPlank = reader.ReadItem() as Plank;
                        this.m_SPlank = reader.ReadItem() as Plank;
                        this.m_TillerMan = reader.ReadItem() as TillerMan;
                        this.m_Hold = reader.ReadItem() as Hold;
                        this.m_Anchored = reader.ReadBool();
                        this.m_ShipName = reader.ReadString();

                        if (version < 1)
                            this.Refresh();

                        break;
                    }
            }

            m_Instances.Add(this);
        }

        public void RemoveKeys(Mobile m)
        {
            uint keyValue = 0;

            if (this.m_PPlank != null)
                keyValue = this.m_PPlank.KeyValue;

            if (keyValue == 0 && this.m_SPlank != null)
                keyValue = this.m_SPlank.KeyValue;

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

        public override void OnAfterDelete()
        {
            if (this.m_TillerMan != null)
                this.m_TillerMan.Delete();

            if (this.m_Hold != null)
                this.m_Hold.Delete();

            if (this.m_PPlank != null)
                this.m_PPlank.Delete();

            if (this.m_SPlank != null)
                this.m_SPlank.Delete();

            if (this.m_TurnTimer != null)
                this.m_TurnTimer.Stop();

            if (this.m_MoveTimer != null)
                this.m_MoveTimer.Stop();

            m_Instances.Remove(this);
        }

        public override void OnLocationChange(Point3D old)
        {
            if (this.m_TillerMan != null)
                this.m_TillerMan.Location = new Point3D(this.X + (this.m_TillerMan.X - old.X), this.Y + (this.m_TillerMan.Y - old.Y), this.Z + (this.m_TillerMan.Z - old.Z));

            if (this.m_Hold != null)
                this.m_Hold.Location = new Point3D(this.X + (this.m_Hold.X - old.X), this.Y + (this.m_Hold.Y - old.Y), this.Z + (this.m_Hold.Z - old.Z));

            if (this.m_PPlank != null)
                this.m_PPlank.Location = new Point3D(this.X + (this.m_PPlank.X - old.X), this.Y + (this.m_PPlank.Y - old.Y), this.Z + (this.m_PPlank.Z - old.Z));

            if (this.m_SPlank != null)
                this.m_SPlank.Location = new Point3D(this.X + (this.m_SPlank.X - old.X), this.Y + (this.m_SPlank.Y - old.Y), this.Z + (this.m_SPlank.Z - old.Z));
        }

        public override void OnMapChange()
        {
            if (this.m_TillerMan != null)
                this.m_TillerMan.Map = this.Map;

            if (this.m_Hold != null)
                this.m_Hold.Map = this.Map;

            if (this.m_PPlank != null)
                this.m_PPlank.Map = this.Map;

            if (this.m_SPlank != null)
                this.m_SPlank.Map = this.Map;
        }

        public bool CanCommand(Mobile m)
        {
            return true;
        }

        public Point3D GetMarkedLocation()
        {
            Point3D p = new Point3D(this.X + this.MarkOffset.X, this.Y + this.MarkOffset.Y, this.Z + this.MarkOffset.Z);

            return this.Rotate(p, (int)this.m_Facing / 2);
        }

        public bool CheckKey(uint keyValue)
        {
            if (this.m_SPlank != null && this.m_SPlank.KeyValue == keyValue)
                return true;

            if (this.m_PPlank != null && this.m_PPlank.KeyValue == keyValue)
                return true;

            return false;
        }

        public void Refresh()
        {
            this.m_DecayTime = DateTime.UtcNow + BoatDecayDelay;

            if (this.m_TillerMan != null)
                this.m_TillerMan.InvalidateProperties();
        }

        public bool CheckDecay()
        {
            if (this.m_Decaying)
                return true;

            if (!this.IsMoving && DateTime.UtcNow >= this.m_DecayTime)
            {
                new DecayTimer(this).Start();

                this.m_Decaying = true;

                return true;
            }

            return false;
        }

        public bool LowerAnchor(bool message)
        {
            if (this.CheckDecay())
                return false;

            if (this.m_Anchored)
            {
                if (message && this.m_TillerMan != null)
                    this.m_TillerMan.Say(501445); // Ar, the anchor was already dropped sir.

                return false;
            }

            this.StopMove(false);

            this.m_Anchored = true;

            if (message && this.m_TillerMan != null)
                this.m_TillerMan.Say(501444); // Ar, anchor dropped sir.

            return true;
        }

        public bool RaiseAnchor(bool message)
        {
            if (this.CheckDecay())
                return false;

            if (!this.m_Anchored)
            {
                if (message && this.m_TillerMan != null)
                    this.m_TillerMan.Say(501447); // Ar, the anchor has not been dropped sir.

                return false;
            }

            this.m_Anchored = false;

            if (message && this.m_TillerMan != null)
                this.m_TillerMan.Say(501446); // Ar, anchor raised sir.

            return true;
        }

        public bool StartMove(Direction dir, bool fast)
        {
            if (this.CheckDecay())
                return false;

            bool drift = (dir != Forward && dir != ForwardLeft && dir != ForwardRight);
            TimeSpan interval = (fast ? (drift ? FastDriftInterval : FastInterval) : (drift ? SlowDriftInterval : SlowInterval));
            int speed = (fast ? (drift ? FastDriftSpeed : FastSpeed) : (drift ? SlowDriftSpeed : SlowSpeed));

            if (this.StartMove(dir, speed, interval, false, true))
            {
                if (this.m_TillerMan != null)
                    this.m_TillerMan.Say(501429); // Aye aye sir.

                return true;
            }

            return false;
        }

        public bool OneMove(Direction dir)
        {
            if (this.CheckDecay())
                return false;

            bool drift = (dir != Forward);
            TimeSpan interval = drift ? FastDriftInterval : FastInterval;
            int speed = drift ? FastDriftSpeed : FastSpeed;

            if (this.StartMove(dir, speed, interval, true, true))
            {
                if (this.m_TillerMan != null)
                    this.m_TillerMan.Say(501429); // Aye aye sir.

                return true;
            }

            return false;
        }

        public void BeginRename(Mobile from)
        {
            if (this.CheckDecay())
                return;

            if (from.AccessLevel < AccessLevel.GameMaster && from != this.m_Owner)
            {
                if (this.m_TillerMan != null)
                    this.m_TillerMan.Say(Utility.Random(1042876, 4)); // Arr, don't do that! | Arr, leave me alone! | Arr, watch what thour'rt doing, matey! | Arr! Do that again and I’ll throw ye overhead!

                return;
            }

            if (this.m_TillerMan != null)
                this.m_TillerMan.Say(502580); // What dost thou wish to name thy ship?

            from.Prompt = new RenameBoatPrompt(this);
        }

        public void EndRename(Mobile from, string newName)
        {
            if (this.Deleted || this.CheckDecay())
                return;

            if (from.AccessLevel < AccessLevel.GameMaster && from != this.m_Owner)
            {
                if (this.m_TillerMan != null)
                    this.m_TillerMan.Say(1042880); // Arr! Only the owner of the ship may change its name!

                return;
            }
            else if (!from.Alive)
            {
                if (this.m_TillerMan != null)
                    this.m_TillerMan.Say(502582); // You appear to be dead.

                return;
            }

            newName = newName.Trim();

            if (newName.Length == 0)
                newName = null;

            this.Rename(newName);
        }

        public DryDockResult CheckDryDock(Mobile from)
        {
            if (this.CheckDecay())
                return DryDockResult.Decaying;

            if (!from.Alive)
                return DryDockResult.Dead;

            Container pack = from.Backpack;
            if ((this.m_SPlank == null || !Key.ContainsKey(pack, this.m_SPlank.KeyValue)) && (this.m_PPlank == null || !Key.ContainsKey(pack, this.m_PPlank.KeyValue)))
                return DryDockResult.NoKey;

            if (!this.m_Anchored)
                return DryDockResult.NotAnchored;

            if (this.m_Hold != null && this.m_Hold.Items.Count > 0)
                return DryDockResult.Hold;

            Map map = this.Map;

            if (map == null || map == Map.Internal)
                return DryDockResult.Items;

            MultiComponentList mcl = this.Components;

            IPooledEnumerable eable = map.GetObjectsInBounds(new Rectangle2D(this.X + mcl.Min.X, this.Y + mcl.Min.Y, mcl.Width, mcl.Height));

            foreach (object o in eable)
            {
                if (o == this || o == this.m_Hold || o == this.m_SPlank || o == this.m_PPlank || o == this.m_TillerMan)
                    continue;

                if (o is Item && this.Contains((Item)o))
                {
                    eable.Free();
                    return DryDockResult.Items;
                }
                else if (o is Mobile && this.Contains((Mobile)o))
                {
                    eable.Free();
                    return DryDockResult.Mobiles;
                }
            }

            eable.Free();
            return DryDockResult.Valid;
        }

        public void BeginDryDock(Mobile from)
        {
            if (this.CheckDecay())
                return;

            DryDockResult result = this.CheckDryDock(from);

            if (result == DryDockResult.Dead)
                from.SendLocalizedMessage(502493); // You appear to be dead.
            else if (result == DryDockResult.NoKey)
                from.SendLocalizedMessage(502494); // You must have a key to the ship to dock the boat.
            else if (result == DryDockResult.NotAnchored)
                from.SendLocalizedMessage(1010570); // You must lower the anchor to dock the boat.
            else if (result == DryDockResult.Mobiles)
                from.SendLocalizedMessage(502495); // You cannot dock the ship with beings on board!
            else if (result == DryDockResult.Items)
                from.SendLocalizedMessage(502496); // You cannot dock the ship with a cluttered deck.
            else if (result == DryDockResult.Hold)
                from.SendLocalizedMessage(502497); // Make sure your hold is empty, and try again!
            else if (result == DryDockResult.Valid)
                from.SendGump(new ConfirmDryDockGump(from, this));
        }

        public void EndDryDock(Mobile from)
        {
            if (this.Deleted || this.CheckDecay())
                return;

            DryDockResult result = this.CheckDryDock(from);

            if (result == DryDockResult.Dead)
                from.SendLocalizedMessage(502493); // You appear to be dead.
            else if (result == DryDockResult.NoKey)
                from.SendLocalizedMessage(502494); // You must have a key to the ship to dock the boat.
            else if (result == DryDockResult.NotAnchored)
                from.SendLocalizedMessage(1010570); // You must lower the anchor to dock the boat.
            else if (result == DryDockResult.Mobiles)
                from.SendLocalizedMessage(502495); // You cannot dock the ship with beings on board!
            else if (result == DryDockResult.Items)
                from.SendLocalizedMessage(502496); // You cannot dock the ship with a cluttered deck.
            else if (result == DryDockResult.Hold)
                from.SendLocalizedMessage(502497); // Make sure your hold is empty, and try again!

            if (result != DryDockResult.Valid)
                return;

            BaseDockedBoat boat = this.DockedBoat;

            if (boat == null)
                return;

            this.RemoveKeys(from);

            from.AddToBackpack(boat);
            this.Delete();
        }

        public void SetName(SpeechEventArgs e)
        {
            if (this.CheckDecay())
                return;

            if (e.Mobile.AccessLevel < AccessLevel.GameMaster && e.Mobile != this.m_Owner)
            {
                if (this.m_TillerMan != null)
                    this.m_TillerMan.Say(1042880); // Arr! Only the owner of the ship may change its name!

                return;
            }
            else if (!e.Mobile.Alive)
            {
                if (this.m_TillerMan != null)
                    this.m_TillerMan.Say(502582); // You appear to be dead.

                return;
            }

            if (e.Speech.Length > 8)
            {
                string newName = e.Speech.Substring(8).Trim();

                if (newName.Length == 0)
                    newName = null;

                this.Rename(newName);
            }
        }

        public void Rename(string newName)
        {
            if (this.CheckDecay())
                return;

            if (newName != null && newName.Length > 40)
                newName = newName.Substring(0, 40);

            if (this.m_ShipName == newName)
            {
                if (this.m_TillerMan != null)
                    this.m_TillerMan.Say(502531); // Yes, sir.

                return;
            }

            this.ShipName = newName;

            if (this.m_TillerMan != null && this.m_ShipName != null)
                this.m_TillerMan.Say(1042885, this.m_ShipName); // This ship is now called the ~1_NEW_SHIP_NAME~.
            else if (this.m_TillerMan != null)
                this.m_TillerMan.Say(502534); // This ship now has no name.
        }

        public void RemoveName(Mobile m)
        {
            if (this.CheckDecay())
                return;

            if (m.AccessLevel < AccessLevel.GameMaster && m != this.m_Owner)
            {
                if (this.m_TillerMan != null)
                    this.m_TillerMan.Say(1042880); // Arr! Only the owner of the ship may change its name!

                return;
            }
            else if (!m.Alive)
            {
                if (this.m_TillerMan != null)
                    this.m_TillerMan.Say(502582); // You appear to be dead.

                return;
            }

            if (this.m_ShipName == null)
            {
                if (this.m_TillerMan != null)
                    this.m_TillerMan.Say(502526); // Ar, this ship has no name.

                return;
            }

            this.ShipName = null;

            if (this.m_TillerMan != null)
                this.m_TillerMan.Say(502534); // This ship now has no name.
        }

        public void GiveName(Mobile m)
        {
            if (this.m_TillerMan == null || this.CheckDecay())
                return;

            if (this.m_ShipName == null)
                this.m_TillerMan.Say(502526); // Ar, this ship has no name.
            else
                this.m_TillerMan.Say(1042881, this.m_ShipName); // This is the ~1_BOAT_NAME~.
        }

        public void GiveNavPoint()
        {
            if (this.TillerMan == null || this.CheckDecay())
                return;

            if (this.NextNavPoint < 0)
                this.TillerMan.Say(1042882); // I have no current nav point.
            else
                this.TillerMan.Say(1042883, (this.NextNavPoint + 1).ToString()); // My current destination navpoint is nav ~1_NAV_POINT_NUM~.
        }

        public void AssociateMap(MapItem map)
        {
            if (this.CheckDecay())
                return;

            if (map is BlankMap)
            {
                if (this.TillerMan != null)
                    this.TillerMan.Say(502575); // Ar, that is not a map, tis but a blank piece of paper!
            }
            else if (map.Pins.Count == 0)
            {
                if (this.TillerMan != null)
                    this.TillerMan.Say(502576); // Arrrr, this map has no course on it!
            }
            else
            {
                this.StopMove(false);

                this.MapItem = map;
                this.NextNavPoint = -1;

                if (this.TillerMan != null)
                    this.TillerMan.Say(502577); // A map!
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

                    if (this.MapItem == null || number < 0 || number >= this.MapItem.Pins.Count)
                    {
                        number = -1;
                    }
                }
            }

            if (number == -1)
            {
                if (message && this.TillerMan != null)
                    this.TillerMan.Say(1042551); // I don't see that navpoint, sir.

                return false;
            }

            this.NextNavPoint = number;
            return this.StartCourse(single, message);
        }

        public bool StartCourse(bool single, bool message)
        {
            if (this.CheckDecay())
                return false;

            if (this.Anchored)
            {
                if (message && this.TillerMan != null)
                    this.TillerMan.Say(501419); // Ar, the anchor is down sir!

                return false;
            }
            else if (this.MapItem == null || this.MapItem.Deleted)
            {
                if (message && this.TillerMan != null)
                    this.TillerMan.Say(502513); // I have seen no map, sir.

                return false;
            }
            else if (this.Map != this.MapItem.Map || !this.Contains(this.MapItem.GetWorldLocation()))
            {
                if (message && this.TillerMan != null)
                    this.TillerMan.Say(502514); // The map is too far away from me, sir.

                return false;
            }
            else if ((this.Map != Map.Trammel && this.Map != Map.Felucca) || this.NextNavPoint < 0 || this.NextNavPoint >= this.MapItem.Pins.Count)
            {
                if (message && this.TillerMan != null)
                    this.TillerMan.Say(1042551); // I don't see that navpoint, sir.

                return false;
            }

            this.Speed = FastSpeed;
            this.Order = single ? BoatOrder.Single : BoatOrder.Course;

            if (this.m_MoveTimer != null)
                this.m_MoveTimer.Stop();

            this.m_MoveTimer = new MoveTimer(this, FastInterval, false);
            this.m_MoveTimer.Start();

            if (message && this.TillerMan != null)
                this.TillerMan.Say(501429); // Aye aye sir.

            return true;
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (this.CheckDecay())
                return;

            Mobile from = e.Mobile;

            if (this.CanCommand(from) && this.Contains(from))
            {
                for (int i = 0; i < e.Keywords.Length; ++i)
                {
                    int keyword = e.Keywords[i];

                    if (keyword >= 0x42 && keyword <= 0x6B)
                    {
                        switch ( keyword )
                        {
                            case 0x42:
                                this.SetName(e);
                                break;
                            case 0x43:
                                this.RemoveName(e.Mobile);
                                break;
                            case 0x44:
                                this.GiveName(e.Mobile);
                                break;
                            case 0x45:
                                this.StartMove(Forward, true);
                                break;
                            case 0x46:
                                this.StartMove(Backward, true);
                                break;
                            case 0x47:
                                this.StartMove(Left, true);
                                break;
                            case 0x48:
                                this.StartMove(Right, true);
                                break;
                            case 0x4B:
                                this.StartMove(ForwardLeft, true);
                                break;
                            case 0x4C:
                                this.StartMove(ForwardRight, true);
                                break;
                            case 0x4D:
                                this.StartMove(BackwardLeft, true);
                                break;
                            case 0x4E:
                                this.StartMove(BackwardRight, true);
                                break;
                            case 0x4F:
                                this.StopMove(true);
                                break;
                            case 0x50:
                                this.StartMove(Left, false);
                                break;
                            case 0x51:
                                this.StartMove(Right, false);
                                break;
                            case 0x52:
                                this.StartMove(Forward, false);
                                break;
                            case 0x53:
                                this.StartMove(Backward, false);
                                break;
                            case 0x54:
                                this.StartMove(ForwardLeft, false);
                                break;
                            case 0x55:
                                this.StartMove(ForwardRight, false);
                                break;
                            case 0x56:
                                this.StartMove(BackwardRight, false);
                                break;
                            case 0x57:
                                this.StartMove(BackwardLeft, false);
                                break;
                            case 0x58:
                                this.OneMove(Left);
                                break;
                            case 0x59:
                                this.OneMove(Right);
                                break;
                            case 0x5A:
                                this.OneMove(Forward);
                                break;
                            case 0x5B:
                                this.OneMove(Backward);
                                break;
                            case 0x5C:
                                this.OneMove(ForwardLeft);
                                break;
                            case 0x5D:
                                this.OneMove(ForwardRight);
                                break;
                            case 0x5E:
                                this.OneMove(BackwardRight);
                                break;
                            case 0x5F:
                                this.OneMove(BackwardLeft);
                                break;
                            case 0x49:
                            case 0x65:
                                this.StartTurn(2, true);
                                break; // turn right
                            case 0x4A:
                            case 0x66:
                                this.StartTurn(-2, true);
                                break; // turn left
                            case 0x67:
                                this.StartTurn(-4, true);
                                break; // turn around, come about
                            case 0x68:
                                this.StartMove(Forward, true);
                                break;
                            case 0x69:
                                this.StopMove(true);
                                break;
                            case 0x6A:
                                this.LowerAnchor(true);
                                break;
                            case 0x6B:
                                this.RaiseAnchor(true);
                                break;
                            case 0x60:
                                this.GiveNavPoint();
                                break; // nav
                            case 0x61:
                                this.NextNavPoint = 0;
                                this.StartCourse(false, true);
                                break; // start
                            case 0x62:
                                this.StartCourse(false, true);
                                break; // continue
                            case 0x63:
                                this.StartCourse(e.Speech, false, true);
                                break; // goto*
                            case 0x64:
                                this.StartCourse(e.Speech, true, true);
                                break; // single*
                        }

                        break;
                    }
                }
            }
        }

        public bool StartTurn(int offset, bool message)
        {
            if (this.CheckDecay())
                return false;

            if (this.m_Anchored)
            {
                if (message)
                    this.m_TillerMan.Say(501419); // Ar, the anchor is down sir!

                return false;
            }
            else
            {
                if (this.m_MoveTimer != null && this.Order != BoatOrder.Move)
                {
                    this.m_MoveTimer.Stop();
                    this.m_MoveTimer = null;
                }

                if (this.m_TurnTimer != null)
                    this.m_TurnTimer.Stop();

                this.m_TurnTimer = new TurnTimer(this, offset);
                this.m_TurnTimer.Start();

                if (message && this.TillerMan != null)
                    this.TillerMan.Say(501429); // Aye aye sir.

                return true;
            }
        }

        public bool Turn(int offset, bool message)
        {
            if (this.m_TurnTimer != null)
            {
                this.m_TurnTimer.Stop();
                this.m_TurnTimer = null;
            }

            if (this.CheckDecay())
                return false;

            if (this.m_Anchored)
            {
                if (message)
                    this.m_TillerMan.Say(501419); // Ar, the anchor is down sir!

                return false;
            }
            else if (this.SetFacing((Direction)(((int)this.m_Facing + offset) & 0x7)))
            {
                return true;
            }
            else
            {
                if (message)
                    this.m_TillerMan.Say(501423); // Ar, can't turn sir.

                return false;
            }
        }

        public bool StartMove(Direction dir, int speed, TimeSpan interval, bool single, bool message)
        {
            if (this.CheckDecay())
                return false;

            if (this.m_Anchored)
            {
                if (message && this.m_TillerMan != null)
                    this.m_TillerMan.Say(501419); // Ar, the anchor is down sir!

                return false;
            }

            this.m_Moving = dir;
            this.m_Speed = speed;
            this.m_Order = BoatOrder.Move;

            if (this.m_MoveTimer != null)
                this.m_MoveTimer.Stop();

            this.m_MoveTimer = new MoveTimer(this, interval, single);
            this.m_MoveTimer.Start();

            return true;
        }

        public bool StopMove(bool message)
        {
            if (this.CheckDecay())
                return false;

            if (this.m_MoveTimer == null)
            {
                if (message && this.m_TillerMan != null)
                    this.m_TillerMan.Say(501443); // Er, the ship is not moving sir.

                return false;
            }

            this.m_Moving = Direction.North;
            this.m_Speed = 0;
            this.m_MoveTimer.Stop();
            this.m_MoveTimer = null;

            if (message && this.m_TillerMan != null)
                this.m_TillerMan.Say(501429); // Aye aye sir.

            return true;
        }

        public bool CanFit(Point3D p, Map map, int itemID)
        {
            if (map == null || map == Map.Internal || this.Deleted || this.CheckDecay())
                return false;

            MultiComponentList newComponents = MultiData.GetComponents(itemID);

            for (int x = 0; x < newComponents.Width; ++x)
            {
                for (int y = 0; y < newComponents.Height; ++y)
                {
                    int tx = p.X + newComponents.Min.X + x;
                    int ty = p.Y + newComponents.Min.Y + y;

                    if (newComponents.Tiles[x][y].Length == 0 || this.Contains(tx, ty))
                        continue;

                    LandTile landTile = map.Tiles.GetLandTile(tx, ty);
                    StaticTile[] tiles = map.Tiles.GetStaticTiles(tx, ty, true);

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
                if (item is BaseMulti || item.ItemID > TileData.MaxItemValue || item.Z < p.Z || !item.Visible)
                    continue;

                int x = item.X - p.X + newComponents.Min.X;
                int y = item.Y - p.Y + newComponents.Min.Y;

                if (x >= 0 && x < newComponents.Width && y >= 0 && y < newComponents.Height && newComponents.Tiles[x][y].Length == 0)
                    continue;
                else if (this.Contains(item))
                    continue;

                eable.Free();
                return false;
            }

            eable.Free();

            return true;
        }

        public Point3D Rotate(Point3D p, int count)
        {
            int rx = p.X - this.Location.X;
            int ry = p.Y - this.Location.Y;

            for (int i = 0; i < count; ++i)
            {
                int temp = rx;
                rx = -ry;
                ry = temp;
            }

            return new Point3D(this.Location.X + rx, this.Location.Y + ry, p.Z);
        }

        public override bool Contains(int x, int y)
        {
            if (base.Contains(x, y))
                return true;

            if (this.m_TillerMan != null && x == this.m_TillerMan.X && y == this.m_TillerMan.Y)
                return true;

            if (this.m_Hold != null && x == this.m_Hold.X && y == this.m_Hold.Y)
                return true;

            if (this.m_PPlank != null && x == this.m_PPlank.X && y == this.m_PPlank.Y)
                return true;

            if (this.m_SPlank != null && x == this.m_SPlank.X && y == this.m_SPlank.Y)
                return true;

            return false;
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

            return (Direction)((iDir - (int)this.Facing) & 0x7);
        }

        public bool DoMovement(bool message)
        {
            Direction dir;
            int speed;

            if (this.Order == BoatOrder.Move)
            {
                dir = this.Moving;
                speed = this.Speed;
            }
            else if (this.MapItem == null || this.MapItem.Deleted)
            {
                if (message && this.TillerMan != null)
                    this.TillerMan.Say(502513); // I have seen no map, sir.

                return false;
            }
            else if (this.Map != this.MapItem.Map || !this.Contains(this.MapItem.GetWorldLocation()))
            {
                if (message && this.TillerMan != null)
                    this.TillerMan.Say(502514); // The map is too far away from me, sir.

                return false;
            }
            else if ((this.Map != Map.Trammel && this.Map != Map.Felucca) || this.NextNavPoint < 0 || this.NextNavPoint >= this.MapItem.Pins.Count)
            {
                if (message && this.TillerMan != null)
                    this.TillerMan.Say(1042551); // I don't see that navpoint, sir.

                return false;
            }
            else
            {
                Point2D dest = (Point2D)this.MapItem.Pins[this.NextNavPoint];

                int x, y;
                this.MapItem.ConvertToWorld(dest.X, dest.Y, out x, out y);

                int maxSpeed;
                dir = this.GetMovementFor(x, y, out maxSpeed);

                if (maxSpeed == 0)
                {
                    if (message && this.Order == BoatOrder.Single && this.TillerMan != null)
                        this.TillerMan.Say(1042874, (this.NextNavPoint + 1).ToString()); // We have arrived at nav point ~1_POINT_NUM~ , sir.

                    if (this.NextNavPoint + 1 < this.MapItem.Pins.Count)
                    {
                        this.NextNavPoint++;

                        if (this.Order == BoatOrder.Course)
                        {
                            if (message && this.TillerMan != null)
                                this.TillerMan.Say(1042875, (this.NextNavPoint + 1).ToString()); // Heading to nav point ~1_POINT_NUM~, sir.

                            return true;
                        }

                        return false;
                    }
                    else
                    {
                        this.NextNavPoint = -1;

                        if (message && this.Order == BoatOrder.Course && this.TillerMan != null)
                            this.TillerMan.Say(502515); // The course is completed, sir.

                        return false;
                    }
                }

                if (dir == Left || dir == BackwardLeft || dir == Backward)
                    return this.Turn(-2, true);
                else if (dir == Right || dir == BackwardRight)
                    return this.Turn(2, true);

                speed = Math.Min(this.Speed, maxSpeed);
            }

            return this.Move(dir, speed, true);
        }

        public bool Move(Direction dir, int speed, bool message)
        {
            Map map = this.Map;

            if (map == null || this.Deleted || this.CheckDecay())
                return false;

            if (this.m_Anchored)
            {
                if (message && this.m_TillerMan != null)
                    this.m_TillerMan.Say(501419); // Ar, the anchor is down sir!

                return false;
            }

            int rx = 0, ry = 0;
            Movement.Movement.Offset((Direction)(((int)this.m_Facing + (int)dir) & 0x7), ref rx, ref ry);

            for (int i = 1; i <= speed; ++i)
            {
                if (!this.CanFit(new Point3D(this.X + (i * rx), this.Y + (i * ry), this.Z), this.Map, this.ItemID))
                {
                    if (i == 1)
                    {
                        if (message && this.m_TillerMan != null)
                            this.m_TillerMan.Say(501424); // Ar, we've stopped sir.

                        return false;
                    }

                    speed = i - 1;
                    break;
                }
            }

            int xOffset = speed * rx;
            int yOffset = speed * ry;

            int newX = this.X + xOffset;
            int newY = this.Y + yOffset;

            Rectangle2D[] wrap = GetWrapFor(map);

            for (int i = 0; i < wrap.Length; ++i)
            {
                Rectangle2D rect = wrap[i];

                if (rect.Contains(new Point2D(this.X, this.Y)) && !rect.Contains(new Point2D(newX, newY)))
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
                        if (!this.CanFit(new Point3D(newX + (j * rx), newY + (j * ry), this.Z), this.Map, this.ItemID))
                        {
                            if (message && this.m_TillerMan != null)
                                this.m_TillerMan.Say(501424); // Ar, we've stopped sir.

                            return false;
                        }
                    }

                    xOffset = newX - this.X;
                    yOffset = newY - this.Y;
                }
            }

            this.Teleport(xOffset, yOffset, 0);

            return true;
        }

        public void Teleport(int xOffset, int yOffset, int zOffset)
        {
            MultiComponentList mcl = this.Components;

            ArrayList toMove = new ArrayList();

            IPooledEnumerable eable = this.Map.GetObjectsInBounds(new Rectangle2D(this.X + mcl.Min.X, this.Y + mcl.Min.Y, mcl.Width, mcl.Height));

            foreach (object o in eable)
            {
                if (o != this && !(o is TillerMan || o is Hold || o is Plank))
                    toMove.Add(o);
            }

            eable.Free();

            for (int i = 0; i < toMove.Count; ++i)
            {
                object o = toMove[i];

                if (o is Item)
                {
                    Item item = (Item)o;

                    if (this.Contains(item) && item.Visible && item.Z >= this.Z)
                        item.Location = new Point3D(item.X + xOffset, item.Y + yOffset, item.Z + zOffset);
                }
                else if (o is Mobile)
                {
                    Mobile m = (Mobile)o;

                    if (this.Contains(m))
                        m.Location = new Point3D(m.X + xOffset, m.Y + yOffset, m.Z + zOffset);
                }
            }

            this.Location = new Point3D(this.X + xOffset, this.Y + yOffset, this.Z + zOffset);
        }

        public bool SetFacing(Direction facing)
        {
            if (this.Parent != null || this.Map == null)
                return false;

            if (this.CheckDecay())
                return false;

            if (this.Map != Map.Internal)
            {
                switch ( facing )
                {
                    case Direction.North:
                        if (!this.CanFit(this.Location, this.Map, this.NorthID))
                            return false;
                        break;
                    case Direction.East:
                        if (!this.CanFit(this.Location, this.Map, this.EastID))
                            return false;
                        break;
                    case Direction.South:
                        if (!this.CanFit(this.Location, this.Map, this.SouthID))
                            return false;
                        break;
                    case Direction.West:
                        if (!this.CanFit(this.Location, this.Map, this.WestID))
                            return false;
                        break;
                }
            }

            this.Map.OnLeave(this);

            Direction old = this.m_Facing;

            this.m_Facing = facing;

            if (this.m_TillerMan != null)
                this.m_TillerMan.SetFacing(facing);

            if (this.m_Hold != null)
                this.m_Hold.SetFacing(facing);

            if (this.m_PPlank != null)
                this.m_PPlank.SetFacing(facing);

            if (this.m_SPlank != null)
                this.m_SPlank.SetFacing(facing);

            MultiComponentList mcl = this.Components;

            ArrayList toMove = new ArrayList();

            toMove.Add(this.m_PPlank);
            toMove.Add(this.m_SPlank);

            IPooledEnumerable eable = this.Map.GetObjectsInBounds(new Rectangle2D(this.X + mcl.Min.X, this.Y + mcl.Min.Y, mcl.Width, mcl.Height));

            foreach (object o in eable)
            {
                if (o is Item)
                {
                    Item item = (Item)o;

                    if (item != this && this.Contains(item) && item.Visible && item.Z >= this.Z && !(item is TillerMan || item is Hold || item is Plank))
                        toMove.Add(item);
                }
                else if (o is Mobile && this.Contains((Mobile)o))
                {
                    toMove.Add(o);

                    ((Mobile)o).Direction = (Direction)((int)((Mobile)o).Direction - (int)old + (int)facing);
                }
            }

            eable.Free();

            int xOffset = 0, yOffset = 0;
            Movement.Movement.Offset(facing, ref xOffset, ref yOffset);

            if (this.m_TillerMan != null)
                this.m_TillerMan.Location = new Point3D(this.X + (xOffset * this.TillerManDistance) + (facing == Direction.North ? 1 : 0), this.Y + (yOffset * this.TillerManDistance), this.m_TillerMan.Z);

            if (this.m_Hold != null)
                this.m_Hold.Location = new Point3D(this.X + (xOffset * this.HoldDistance), this.Y + (yOffset * this.HoldDistance), this.m_Hold.Z);

            int count = (int)(this.m_Facing - old) & 0x7;
            count /= 2;

            for (int i = 0; i < toMove.Count; ++i)
            {
                object o = toMove[i];

                if (o is Item)
                    ((Item)o).Location = this.Rotate(((Item)o).Location, count);
                else if (o is Mobile)
                    ((Mobile)o).Location = this.Rotate(((Mobile)o).Location, count);
            }

            switch ( facing )
            {
                case Direction.North:
                    this.ItemID = this.NorthID;
                    break;
                case Direction.East:
                    this.ItemID = this.EastID;
                    break;
                case Direction.South:
                    this.ItemID = this.SouthID;
                    break;
                case Direction.West:
                    this.ItemID = this.WestID;
                    break;
            }

            this.Map.OnEnter(this);

            return true;
        }

        private static void EventSink_WorldSave(WorldSaveEventArgs e)
        {
            new UpdateAllTimer().Start();
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

        private class DecayTimer : Timer
        {
            private readonly BaseBoat m_Boat;
            private int m_Count;
            public DecayTimer(BaseBoat boat)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(5.0))
            {
                this.m_Boat = boat;

                this.Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (this.m_Count == 5)
                {
                    this.m_Boat.Delete();
                    this.Stop();
                }
                else
                {
                    this.m_Boat.Location = new Point3D(this.m_Boat.X, this.m_Boat.Y, this.m_Boat.Z - 1);

                    if (this.m_Boat.TillerMan != null)
                        this.m_Boat.TillerMan.Say(1007168 + this.m_Count);

                    ++this.m_Count;
                }
            }
        }

        private class TurnTimer : Timer
        {
            private readonly BaseBoat m_Boat;
            private readonly int m_Offset;
            public TurnTimer(BaseBoat boat, int offset)
                : base(TimeSpan.FromSeconds(0.5))
            {
                this.m_Boat = boat;
                this.m_Offset = offset;

                this.Priority = TimerPriority.TenMS;
            }

            protected override void OnTick()
            {
                if (!this.m_Boat.Deleted)
                    this.m_Boat.Turn(this.m_Offset, true);
            }
        }

        private class MoveTimer : Timer
        {
            private readonly BaseBoat m_Boat;
            public MoveTimer(BaseBoat boat, TimeSpan interval, bool single)
                : base(interval, interval, single ? 1 : 0)
            {
                this.m_Boat = boat;
                this.Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                if (!this.m_Boat.DoMovement(true))
                    this.m_Boat.StopMove(false);
            }
        }
    }
}