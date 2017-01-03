using Server;
using System;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Multis;
using Server.Regions;
using Server.Targeting;
using Server.Items;

namespace Server.Mobiles
{
    [PropertyObject]
    public class CharydbisSpawner
    {
        public static void GenerateCharydbisSpawner()
        {
            if (m_SpawnInstance == null)
            {
                m_SpawnInstance = new CharydbisSpawner();
            }
        }

        public static readonly TimeSpan KillDelay = TimeSpan.FromHours(7);
        public static readonly TimeSpan NoSpawnDelay = TimeSpan.FromHours(24);

        private Charydbis m_Charydbis;
        private Timer m_Timer;
        private DateTime m_NextSpawn;
        private DateTime m_LastAttempt;
        private bool m_IsSummoned;
        private bool m_HasSpawned;
        private Map m_Map;
        private Rectangle2D m_CurrentLocation;
        private Rectangle2D m_LastLocation;
        private bool m_Active;

        [CommandProperty(AccessLevel.GameMaster)]
        public Charydbis Charydbis { get { return m_Charydbis; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextSpawn { get { return m_NextSpawn; } set { m_NextSpawn = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastAttempt { get { return m_LastAttempt; } set { m_LastAttempt = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsSummoned { get { return m_IsSummoned; } set { m_IsSummoned = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasSpawned { get { return m_HasSpawned; } set { m_HasSpawned = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map Map { get { return m_Map; } set { m_Map = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D CurrentLocation { get { return m_CurrentLocation; } set { m_CurrentLocation = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D LastLocation { get { return m_LastLocation; } set { m_LastLocation = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get
            {
                return m_Active;
            }
            set
            {
                Reset();
                m_NextSpawn = DateTime.UtcNow;
                m_Active = value;
            }
        }

        private static CharydbisSpawner m_SpawnInstance;

        public static CharydbisSpawner SpawnInstance
        {
            get
            {
                return m_SpawnInstance;
            }
            set
            {
                m_SpawnInstance = value;
            }
        }

        public CharydbisSpawner()
        {
            m_Charydbis = null;
            m_NextSpawn = DateTime.UtcNow;
            m_IsSummoned = false;
            m_HasSpawned = false;

            Active = true;
        }

        public bool TrySpawnCharybdis(Mobile from)
        {
            if (!m_Active)
            {
                from.SendLocalizedMessage(1150198); //The spyglass goes dark, it has failed to find what you seek.
                return false;
            }

            else if (m_IsSummoned)
            {
                if (m_Charydbis != null && m_Charydbis.Alive)
                {
                    Point3D pnt = new Point3D(m_CurrentLocation.X + 5, m_CurrentLocation.Y + 5, -5);
                    from.SendMessage(String.Format("The location you seek is: {0} in {1}", GetSextantLocation(pnt), m_Map));
                }
                else if (m_HasSpawned && (m_Charydbis == null || !m_Charydbis.Alive))
                {
                    from.SendMessage("The creature you seek has already been slain.");
                }
                else
                    from.SendLocalizedMessage(1150198); //The spyglass goes dark, it has failed to find what you seek.
                return false;
            }
            else if (DateTime.UtcNow < m_NextSpawn)
            {
                from.SendLocalizedMessage(1150198); //The spyglass goes dark, it has failed to find what you seek.
                return false;
            }

            Map map = from.Map;

            if (map != Map.Felucca && map != Map.Trammel)
            {
                from.SendMessage("You can only summon Charydbis in Felucca or Trammel.");
                return false;
            }

            m_Map = map;

            from.SendLocalizedMessage(1150190); //You peer into the spyglass, images swirl in your mind as the magic device searches.
            m_NextSpawn = DateTime.UtcNow + NoSpawnDelay;
            m_IsSummoned = true;
            Point3D p = SOS.FindLocation(map);
            from.SendMessage(String.Format("The location you seek is: {0} in {1}", GetSextantLocation(p), m_Map));
            m_CurrentLocation = new Rectangle2D(p.X - 5, p.Y - 5, 10, 10);
            m_LastLocation = m_CurrentLocation;
            m_Timer = new InternalTimer(this, NoSpawnDelay);
            m_Timer.Start();
            m_LastAttempt = DateTime.UtcNow;
            return true;
        }

        public string GetSextantLocation(Point3D pnt)
        {
            int xLong = 0, yLat = 0;
            int xMins = 0, yMins = 0;
            bool xEast = false, ySouth = false;

            if (Sextant.Format(pnt, m_Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
                return String.Format("{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");

            return pnt.ToString();
        }

        public void SpawnCharydbis(Mobile from, Point3D pnt, Map map, BaseBoat boat)
        {
            Effects.PlaySound(pnt, map, 0x668);

            m_Charydbis = new Charydbis(from);
            from.SendMessage("It seems as though you've snagged your hook.");

            Timer.DelayCall(TimeSpan.FromSeconds(11), new TimerStateCallback(DoTeleportEffect), new object[] { pnt, map });
            Timer.DelayCall(TimeSpan.FromSeconds(12), new TimerStateCallback(DoDelayedSpawn), new object[] { from, pnt, map, boat });
        }

        public void DoTeleportEffect(object o)
        {
            object[] ojs = (object[])o;
            Point3D pnt = (Point3D)ojs[0];
            Map map = (Map)ojs[1];
            m_Charydbis.DoTeleportEffects(new Point3D(pnt.X, pnt.Y, map.GetAverageZ(pnt.X, pnt.Y)), map);
        }

        public void DoDelayedSpawn(object o)
        {
            object[] ojs = (object[])o;
            Mobile from = (Mobile)ojs[0];
            Point3D pnt = (Point3D)ojs[1];
            Map map = (Map)ojs[2];
            BaseBoat boat = (BaseBoat)ojs[3];

            int x = pnt.X, y = pnt.Y;

            m_Charydbis.MoveToWorld(new Point3D(x, y, map.GetAverageZ(pnt.X, pnt.Y)), map);
            m_Charydbis.Combatant = from;

            from.SendMessage("THATS NO FISH!");

            if (boat != null)
            {
                pnt = boat.Location;
            }

            for (int i = 0; i < 8; i++)
            {
                x = pnt.X; y = pnt.Y;
                if (TrySpawnMobile(ref x, ref y, map))
                {
                    GiantTentacle tent = new GiantTentacle(m_Charydbis);
                    m_Charydbis.AddTentacle(tent);
                    tent.MoveToWorld(new Point3D(x, y, -5), map);
                }
            }
        }

        public bool TrySpawnMobile(ref int x, ref int y, Map map)
        {
            for (int i = 0; map != null && i < 25; ++i)
            {
                int tx = x - 10 + Utility.Random(21);
                int ty = y - 10 + Utility.Random(21);

                LandTile t = map.Tiles.GetLandTile(tx, ty);

                if (t.Z == -5 && ((t.ID >= 0xA8 && t.ID <= 0xAB) || (t.ID >= 0x136 && t.ID <= 0x137)) && !Spells.SpellHelper.CheckMulti(new Point3D(tx, ty, -5), map))
                {
                    x = tx;
                    y = ty;

                    return true;
                }
            }

            return false;
        }

        public void OnCharybdisKilled()
        {
            DateTime timeout = m_LastAttempt + KillDelay;

            if (m_LastAttempt + KillDelay < DateTime.UtcNow)
            {
                m_Charydbis = null;
                Reset();
            }
            else
            {
                TimeSpan ts = timeout - DateTime.UtcNow;
                m_NextSpawn = DateTime.UtcNow + ts;
                m_Timer = new InternalTimer(this, ts);
                m_Timer.Start();
            }
        }

        public void Reset()
        {
            if (m_Charydbis != null && m_Charydbis.Alive)
            {
                IPooledEnumerable eable = m_Charydbis.GetMobilesInRange(12);
                foreach (Mobile mob in eable)
                {
                    if (mob is PlayerMobile)
                        mob.SendMessage("Charydbis sinks to the depths of the ocean from which it came from...You have taken too long!");
                }
                eable.Free();

                for (int x = m_Charydbis.X - 1; x <= m_Charydbis.X + 1; x++)
                {
                    for (int y = m_Charydbis.Y - 1; y <= m_Charydbis.Y + 1; y++)
                    {
                        int splash = Utility.RandomList(0x352D, 0x5675);

                        Effects.SendLocationEffect(new Point3D(x, y, m_Charydbis.Z), m_Charydbis.Map, splash, 16, 4);
                        Effects.PlaySound(new Point3D(x, y, m_Charydbis.Z), m_Charydbis.Map, 0x364);
                    }
                }

                m_Charydbis.Delete();
            }

            m_Charydbis = null;
            m_IsSummoned = false;
            m_HasSpawned = false;
            m_CurrentLocation = new Rectangle2D(0, 0, 0, 0);
            m_NextSpawn = DateTime.UtcNow;

            if (m_Timer != null)
                m_Timer.Stop();
        }

        private class InternalTimer : Timer
        {
            private CharydbisSpawner m_Info;

            public InternalTimer(CharydbisSpawner info, TimeSpan ts)
                : base(ts)
            {
                m_Info = info;
            }

            protected override void OnTick()
            {
                m_Info.Reset();
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0);

            writer.Write(m_HasSpawned);
            writer.Write(m_LastAttempt);
            writer.Write(m_Active);

            writer.Write(m_Charydbis);
            writer.Write(m_NextSpawn);
            writer.Write(m_IsSummoned);
            writer.Write(m_Map);
            writer.Write(m_CurrentLocation);
            writer.Write(m_LastLocation);
        }

        public void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            m_HasSpawned = reader.ReadBool();
            m_LastAttempt = reader.ReadDateTime();
            m_Active = reader.ReadBool();

            m_Charydbis = reader.ReadMobile() as Charydbis;
            m_NextSpawn = reader.ReadDateTime();
            m_IsSummoned = reader.ReadBool();
            m_Map = reader.ReadMap();
            m_CurrentLocation = reader.ReadRect2D();
            m_LastLocation = reader.ReadRect2D();

            if (m_NextSpawn > DateTime.UtcNow)
                m_Timer = new InternalTimer(this, m_NextSpawn - DateTime.UtcNow);
            else
                Reset();
        }
    }
}
