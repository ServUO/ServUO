using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Engines.ConPVP
{
    public class ArenaController : Item
    {
        private Arena m_Arena;
        private bool m_IsPrivate;

        [CommandProperty(AccessLevel.GameMaster)]
        public Arena Arena
        {
            get
            {
                return this.m_Arena;
            }
            set
            {
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPrivate
        {
            get
            {
                return this.m_IsPrivate;
            }
            set
            {
                this.m_IsPrivate = value;
            }
        }

        public override string DefaultName
        {
            get
            {
                return "arena controller";
            }
        }

        [Constructable]
        public ArenaController()
            : base(0x1B7A)
        {
            this.Visible = false;
            this.Movable = false;

            this.m_Arena = new Arena();

            m_Instances.Add(this);
        }

        public override void OnDelete()
        {
            base.OnDelete();

            m_Instances.Remove(this);
            this.m_Arena.Delete();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                from.SendGump(new Gumps.PropertiesGump(from, this.m_Arena));
        }

        public ArenaController(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);

            writer.Write((bool)this.m_IsPrivate);

            this.m_Arena.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_IsPrivate = reader.ReadBool();

                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Arena = new Arena(reader);
                        break;
                    }
            }

            m_Instances.Add(this);
        }

        private static List<ArenaController> m_Instances = new List<ArenaController>();

        public static List<ArenaController> Instances
        {
            get
            {
                return m_Instances;
            }
            set
            {
                m_Instances = value;
            }
        }
    }

    [PropertyObject]
    public class ArenaStartPoints
    {
        private readonly Point3D[] m_Points;

        public Point3D[] Points
        {
            get
            {
                return this.m_Points;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D EdgeWest
        {
            get
            {
                return this.m_Points[0];
            }
            set
            {
                this.m_Points[0] = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D EdgeEast
        {
            get
            {
                return this.m_Points[1];
            }
            set
            {
                this.m_Points[1] = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D EdgeNorth
        {
            get
            {
                return this.m_Points[2];
            }
            set
            {
                this.m_Points[2] = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D EdgeSouth
        {
            get
            {
                return this.m_Points[3];
            }
            set
            {
                this.m_Points[3] = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D CornerNW
        {
            get
            {
                return this.m_Points[4];
            }
            set
            {
                this.m_Points[4] = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D CornerSE
        {
            get
            {
                return this.m_Points[5];
            }
            set
            {
                this.m_Points[5] = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D CornerSW
        {
            get
            {
                return this.m_Points[6];
            }
            set
            {
                this.m_Points[6] = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D CornerNE
        {
            get
            {
                return this.m_Points[7];
            }
            set
            {
                this.m_Points[7] = value;
            }
        }

        public override string ToString()
        {
            return "...";
        }

        public ArenaStartPoints()
            : this(new Point3D[8])
        {
        }

        public ArenaStartPoints(Point3D[] points)
        {
            this.m_Points = points;
        }

        public ArenaStartPoints(GenericReader reader)
        {
            this.m_Points = new Point3D[reader.ReadEncodedInt()];

            for (int i = 0; i < this.m_Points.Length; ++i)
                this.m_Points[i] = reader.ReadPoint3D();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)this.m_Points.Length);

            for (int i = 0; i < this.m_Points.Length; ++i)
                writer.Write((Point3D)this.m_Points[i]);
        }
    }

    [PropertyObject]
    public class Arena : IComparable
    {
        private Map m_Facet;
        private Rectangle2D m_Bounds;
        private Rectangle2D m_Zone;
        private Point3D m_Outside;
        private Point3D m_Wall;
        private Point3D m_GateIn;
        private Point3D m_GateOut;
        private readonly ArenaStartPoints m_Points;
        private bool m_Active;
        private string m_Name;

        private bool m_IsGuarded;

        private Item m_Teleporter;

        private readonly List<Mobile> m_Players;

        private TournamentController m_Tournament;
        private Mobile m_Announcer;

        private LadderController m_Ladder;

        [CommandProperty(AccessLevel.GameMaster)]
        public LadderController Ladder
        {
            get
            {
                return this.m_Ladder;
            }
            set
            {
                this.m_Ladder = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsGuarded
        {
            get
            {
                return this.m_IsGuarded;
            }
            set
            {
                this.m_IsGuarded = value;

                if (this.m_Region != null)
                    this.m_Region.Disabled = !this.m_IsGuarded;
            }
        }

        public Ladder AcquireLadder()
        {
            if (this.m_Ladder != null)
                return this.m_Ladder.Ladder;

            return Server.Engines.ConPVP.Ladder.Instance;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TournamentController Tournament
        {
            get
            {
                return this.m_Tournament;
            }
            set
            {
                if (this.m_Tournament != null)
                    this.m_Tournament.Tournament.Arenas.Remove(this);

                this.m_Tournament = value;

                if (this.m_Tournament != null)
                    this.m_Tournament.Tournament.Arenas.Add(this);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Announcer
        {
            get
            {
                return this.m_Announcer;
            }
            set
            {
                this.m_Announcer = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
                if (this.m_Active)
                    m_Arenas.Sort();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map Facet
        {
            get
            {
                return this.m_Facet;
            }
            set
            {
                this.m_Facet = value;

                if (this.m_Teleporter != null)
                    this.m_Teleporter.Map = value;

                if (this.m_Region != null)
                    this.m_Region.Unregister();

                if (this.m_Zone.Start != Point2D.Zero && this.m_Zone.End != Point2D.Zero && this.m_Facet != null)
                    this.m_Region = new SafeZone(this.m_Zone, this.m_Outside, this.m_Facet, this.m_IsGuarded);
                else
                    this.m_Region = null;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D Bounds
        {
            get
            {
                return this.m_Bounds;
            }
            set
            {
                this.m_Bounds = value;
            }
        }

        private SafeZone m_Region;

        public int Spectators
        {
            get
            {
                if (this.m_Region == null)
                    return 0;

                int specs = this.m_Region.GetPlayerCount() - this.m_Players.Count;

                if (specs < 0)
                    specs = 0;

                return specs;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D Zone
        {
            get
            {
                return this.m_Zone;
            }
            set
            {
                this.m_Zone = value;

                if (this.m_Zone.Start != Point2D.Zero && this.m_Zone.End != Point2D.Zero && this.m_Facet != null)
                {
                    if (this.m_Region != null)
                        this.m_Region.Unregister();

                    this.m_Region = new SafeZone(this.m_Zone, this.m_Outside, this.m_Facet, this.m_IsGuarded);
                }
                else
                {
                    if (this.m_Region != null)
                        this.m_Region.Unregister();

                    this.m_Region = null;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Outside
        {
            get
            {
                return this.m_Outside;
            }
            set
            {
                this.m_Outside = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D GateIn
        {
            get
            {
                return this.m_GateIn;
            }
            set
            {
                this.m_GateIn = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D GateOut
        {
            get
            {
                return this.m_GateOut;
            }
            set
            {
                this.m_GateOut = value;
                if (this.m_Teleporter != null)
                    this.m_Teleporter.Location = this.m_GateOut;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Wall
        {
            get
            {
                return this.m_Wall;
            }
            set
            {
                this.m_Wall = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsOccupied
        {
            get
            {
                return (this.m_Players.Count > 0);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaStartPoints Points
        {
            get
            {
                return this.m_Points;
            }
            set
            {
            }
        }

        public Item Teleporter
        {
            get
            {
                return this.m_Teleporter;
            }
            set
            {
                this.m_Teleporter = value;
            }
        }

        public List<Mobile> Players
        {
            get
            {
                return this.m_Players;
            }
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
                if (this.m_Active == value)
                    return;

                this.m_Active = value;

                if (this.m_Active)
                {
                    m_Arenas.Add(this);
                    m_Arenas.Sort();
                }
                else
                {
                    m_Arenas.Remove(this);
                }
            }
        }

        public void Delete()
        {
            this.Active = false;

            if (this.m_Region != null)
                this.m_Region.Unregister();

            this.m_Region = null;
        }

        public override string ToString()
        {
            return "...";
        }

        public Point3D GetBaseStartPoint(int index)
        {
            if (index < 0)
                index = 0;

            return this.m_Points.Points[index % this.m_Points.Points.Length];
        }

        #region Offsets & Rotation
        private static readonly Point2D[] m_EdgeOffsets = new Point2D[]
        {
            /*
            *        /\
            *       /\/\
            *      /\/\/\
            *      \/\/\/
            *       \/\/\
            *        \/\/
            */
            new Point2D(0, 0),
            new Point2D(0, -1),
            new Point2D(0, +1),
            new Point2D(1, 0),
            new Point2D(1, -1),
            new Point2D(1, +1),
            new Point2D(2, 0),
            new Point2D(2, -1),
            new Point2D(2, +1),
            new Point2D(3, 0)
        };

        // nw corner
        private static readonly Point2D[] m_CornerOffsets = new Point2D[]
        {
            /*
            *         /\
            *        /\/\
            *       /\/\/\
            *      /\/\/\/\
            *      \/\/\/\/
            */
            new Point2D(0, 0),
            new Point2D(0, 1),
            new Point2D(1, 0),
            new Point2D(1, 1),
            new Point2D(0, 2),
            new Point2D(2, 0),
            new Point2D(2, 1),
            new Point2D(1, 2),
            new Point2D(0, 3),
            new Point2D(3, 0)
        };

        private static readonly int[][,] m_Rotate = new int[][,]
        {
            new int[,] { { +1, 0 }, { 0, +1 } }, // west
            new int[,] { { -1, 0 }, { 0, -1 } }, // east
            new int[,] { { 0, +1 }, { +1, 0 } }, // north
            new int[,] { { 0, -1 }, { -1, 0 } }, // south
            new int[,] { { +1, 0 }, { 0, +1 } }, // nw
            new int[,] { { -1, 0 }, { 0, -1 } }, // se
            new int[,] { { 0, +1 }, { +1, 0 } }, // sw
            new int[,] { { 0, -1 }, { -1, 0 } }, // ne
        };
        #endregion

        public void MoveInside(DuelPlayer[] players, int index)
        {
            if (index < 0)
                index = 0;
            else
                index %= this.m_Points.Points.Length;

            Point3D start = this.GetBaseStartPoint(index);

            int offset = 0;

            Point2D[] offsets = (index < 4) ? m_EdgeOffsets : m_CornerOffsets;
            int[,] matrix = m_Rotate[index];

            for (int i = 0; i < players.Length; ++i)
            {
                DuelPlayer pl = players[i];

                if (pl == null)
                    continue;

                Mobile mob = pl.Mobile;

                Point2D p;

                if (offset < offsets.Length)
                    p = offsets[offset++];
                else
                    p = offsets[offsets.Length - 1];

                p.X = (p.X * matrix[0, 0]) + (p.Y * matrix[0, 1]);
                p.Y = (p.X * matrix[1, 0]) + (p.Y * matrix[1, 1]);

                mob.MoveToWorld(new Point3D(start.X + p.X, start.Y + p.Y, start.Z), this.m_Facet);
                mob.Direction = mob.GetDirectionTo(this.m_Wall);

                this.m_Players.Add(mob);
            }
        }

        public Arena()
        {
            this.m_Points = new ArenaStartPoints();
            this.m_Players = new List<Mobile>();
        }

        public Arena(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 7:
                    {
                        this.m_IsGuarded = reader.ReadBool();

                        goto case 6;
                    }
                case 6:
                    {
                        this.m_Ladder = reader.ReadItem() as LadderController;

                        goto case 5;
                    }
                case 5:
                    {
                        this.m_Tournament = reader.ReadItem() as TournamentController;
                        this.m_Announcer = reader.ReadMobile();

                        goto case 4;
                    }
                case 4:
                    {
                        this.m_Name = reader.ReadString();

                        goto case 3;
                    }
                case 3:
                    {
                        this.m_Zone = reader.ReadRect2D();

                        goto case 2;
                    }
                case 2:
                    {
                        this.m_GateIn = reader.ReadPoint3D();
                        this.m_GateOut = reader.ReadPoint3D();
                        this.m_Teleporter = reader.ReadItem();

                        goto case 1;
                    }
                case 1:
                    {
                        this.m_Players = reader.ReadStrongMobileList();

                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Facet = reader.ReadMap();
                        this.m_Bounds = reader.ReadRect2D();
                        this.m_Outside = reader.ReadPoint3D();
                        this.m_Wall = reader.ReadPoint3D();

                        if (version == 0)
                        {
                            reader.ReadBool();
                            this.m_Players = new List<Mobile>();
                        }

                        this.m_Active = reader.ReadBool();
                        this.m_Points = new ArenaStartPoints(reader);

                        if (this.m_Active)
                        {
                            m_Arenas.Add(this);
                            m_Arenas.Sort();
                        }

                        break;
                    }
            }

            if (this.m_Zone.Start != Point2D.Zero && this.m_Zone.End != Point2D.Zero && this.m_Facet != null)
                this.m_Region = new SafeZone(this.m_Zone, this.m_Outside, this.m_Facet, this.m_IsGuarded);

            if (this.IsOccupied)
                Timer.DelayCall(TimeSpan.FromSeconds(2.0), new TimerCallback(Evict));

            if (this.m_Tournament != null)
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(AttachToTournament_Sandbox));
        }

        private void AttachToTournament_Sandbox()
        {
            if (this.m_Tournament != null)
                this.m_Tournament.Tournament.Arenas.Add(this);
        }

        [CommandProperty(AccessLevel.Administrator, AccessLevel.Administrator)]
        public bool ForceEvict
        {
            get
            {
                return false;
            }
            set
            {
                if (value)
                    this.Evict();
            }
        }

        public void Evict()
        {
            Point3D loc;
            Map facet;

            if (this.m_Facet == null)
            {
                loc = new Point3D(2715, 2165, 0);
                facet = Map.Felucca;
            }
            else
            {
                loc = this.m_Outside;
                facet = this.m_Facet;
            }

            bool hasBounds = (this.m_Bounds.Start != Point2D.Zero && this.m_Bounds.End != Point2D.Zero);

            for (int i = 0; i < this.m_Players.Count; ++i)
            {
                Mobile mob = this.m_Players[i];

                if (mob == null)
                    continue;

                if (mob.Map == Map.Internal)
                {
                    if ((this.m_Facet == null || mob.LogoutMap == this.m_Facet) && (!hasBounds || this.m_Bounds.Contains(mob.LogoutLocation)))
                        mob.LogoutLocation = loc;
                }
                else if ((this.m_Facet == null || mob.Map == this.m_Facet) && (!hasBounds || this.m_Bounds.Contains(mob.Location)))
                {
                    mob.MoveToWorld(loc, facet);
                }

                mob.Combatant = null;
                mob.Frozen = false;
                DuelContext.Debuff(mob);
                DuelContext.CancelSpell(mob);
            }

            if (hasBounds)
            {
                List<Mobile> pets = new List<Mobile>();

                foreach (Mobile mob in facet.GetMobilesInBounds(this.m_Bounds))
                {
                    BaseCreature pet = mob as BaseCreature;

                    if (pet != null && pet.Controlled && pet.ControlMaster != null)
                    {
                        if (this.m_Players.Contains(pet.ControlMaster))
                        {
                            pets.Add(pet);
                        }
                    }
                }

                foreach (Mobile pet in pets)
                {
                    pet.Combatant = null;
                    pet.Frozen = false;

                    pet.MoveToWorld(loc, facet);
                }
            }

            this.m_Players.Clear();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)7);

            writer.Write((bool)this.m_IsGuarded);

            writer.Write((Item)this.m_Ladder);

            writer.Write((Item)this.m_Tournament);
            writer.Write((Mobile)this.m_Announcer);

            writer.Write((string)this.m_Name);

            writer.Write((Rectangle2D)this.m_Zone);

            writer.Write((Point3D)this.m_GateIn);
            writer.Write((Point3D)this.m_GateOut);
            writer.Write((Item)this.m_Teleporter);

            writer.Write(this.m_Players);

            writer.Write((Map)this.m_Facet);
            writer.Write((Rectangle2D)this.m_Bounds);
            writer.Write((Point3D)this.m_Outside);
            writer.Write((Point3D)this.m_Wall);
            writer.Write((bool)this.m_Active);

            this.m_Points.Serialize(writer);
        }

        private static readonly List<Arena> m_Arenas = new List<Arena>();

        public static List<Arena> Arenas
        {
            get
            {
                return m_Arenas;
            }
        }

        public static Arena FindArena(List<Mobile> players)
        {
            Preferences prefs = Preferences.Instance;

            if (prefs == null)
                return FindArena();

            if (m_Arenas.Count == 0)
                return null;

            if (players.Count > 0)
            {
                Mobile first = players[0];

                List<ArenaController> allControllers = ArenaController.Instances;

                for (int i = 0; i < allControllers.Count; ++i)
                {
                    ArenaController controller = allControllers[i];

                    if (controller != null && !controller.Deleted && controller.Arena != null && controller.IsPrivate && controller.Map == first.Map && first.InRange(controller, 24))
                    {
                        Multis.BaseHouse house = Multis.BaseHouse.FindHouseAt(controller);
                        bool allNear = true;

                        for (int j = 0; j < players.Count; ++j)
                        {
                            Mobile check = players[j];
                            bool isNear;

                            if (house == null)
                                isNear = (controller.Map == check.Map && check.InRange(controller, 24));
                            else
                                isNear = (Multis.BaseHouse.FindHouseAt(check) == house);

                            if (!isNear)
                            {
                                allNear = false;
                                break;
                            }
                        }

                        if (allNear)
                            return controller.Arena;
                    }
                }
            }

            List<ArenaEntry> arenas = new List<ArenaEntry>();

            for (int i = 0; i < m_Arenas.Count; ++i)
            {
                Arena arena = m_Arenas[i];

                if (!arena.IsOccupied)
                    arenas.Add(new ArenaEntry(arena));
            }

            if (arenas.Count == 0)
                return m_Arenas[0];

            int tc = 0;

            for (int i = 0; i < arenas.Count; ++i)
            {
                ArenaEntry ae = arenas[i];

                for (int j = 0; j < players.Count; ++j)
                {
                    PreferencesEntry pe = prefs.Find(players[j]);

                    if (pe.Disliked.Contains(ae.m_Arena.Name))
                        ++ae.m_VotesAgainst;
                    else
                        ++ae.m_VotesFor;
                }

                tc += ae.Value;
            }

            int rn = Utility.Random(tc);

            for (int i = 0; i < arenas.Count; ++i)
            {
                ArenaEntry ae = arenas[i];

                if (rn < ae.Value)
                    return ae.m_Arena;

                rn -= ae.Value;
            }

            return arenas[Utility.Random(arenas.Count)].m_Arena;
        }

        private class ArenaEntry
        {
            public readonly Arena m_Arena;
            public int m_VotesFor;
            public int m_VotesAgainst;

            public int Value
            {
                get
                {
                    return this.m_VotesFor;
                    /*if ( m_VotesFor > m_VotesAgainst )
                    return m_VotesFor - m_VotesAgainst;
                    else if ( m_VotesFor > 0 )
                    return 1;
                    else
                    return 0;*/
                }
            }

            public ArenaEntry(Arena arena)
            {
                this.m_Arena = arena;
            }
        }

        public static Arena FindArena()
        {
            if (m_Arenas.Count == 0)
                return null;

            int offset = Utility.Random(m_Arenas.Count);

            for (int i = 0; i < m_Arenas.Count; ++i)
            {
                Arena arena = m_Arenas[(i + offset) % m_Arenas.Count];

                if (!arena.IsOccupied)
                    return arena;
            }

            return m_Arenas[offset];
        }

        public int CompareTo(object obj)
        {
            Arena c = (Arena)obj;

            string a = this.m_Name;
            string b = c.m_Name;

            if (a == null && b == null)
                return 0;
            else if (a == null)
                return -1;
            else if (b == null)
                return +1;

            return a.CompareTo(b);
        }
    }
}