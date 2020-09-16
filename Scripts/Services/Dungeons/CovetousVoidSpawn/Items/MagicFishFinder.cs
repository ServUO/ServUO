using Server.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class MagicalFishFinder : Item
    {
        public const int DecayPeriod = 4;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Expires { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastUpdate { get; set; }

        private Timer m_Timer;

        public override int LabelNumber => 1152602;  // Magical Fish Finder

        [Constructable]
        public MagicalFishFinder() : base(5366)
        {
            Hue = 2500;

            Expires = DateTime.UtcNow + TimeSpan.FromHours(DecayPeriod);
            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), CheckDecay);
        }

        public void CheckDecay()
        {
            if (Expires < DateTime.UtcNow)
                Decay();
            else
                InvalidateProperties();
        }

        public void Decay()
        {
            if (RootParent is Mobile)
            {
                Mobile parent = (Mobile)RootParent;

                if (Name == null)
                    parent.SendLocalizedMessage(1072515, "#" + LabelNumber); // The ~1_name~ expired...
                else
                    parent.SendLocalizedMessage(1072515, Name); // The ~1_name~ expired...

                Effects.SendLocationParticles(EffectItem.Create(parent.Location, parent.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(parent.Location, parent.Map, 0x201);
            }
            else
            {
                Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(Location, Map, 0x201);
            }

            Delete();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            int left = 0;

            if (DateTime.UtcNow < Expires)
                left = (int)(Expires - DateTime.UtcNow).TotalSeconds;

            list.Add(1072517, left.ToString()); // Lifespan: ~1_val~ seconds
        }

        public override void Delete()
        {
            base.Delete();

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
                CheckUpdate(m);
        }

        public void CheckUpdate(Mobile m)
        {
            if (Schools.ContainsKey(m.Map))
            {
                SchoolEntry entry = Schools[m.Map].FirstOrDefault(e => m.InRange(e.Location, SchoolRange));

                if (entry != null)
                {
                    m.SendLocalizedMessage(1152647); // Fish are schooling right here!
                    return;
                }

                entry = Schools[m.Map].FirstOrDefault(e => m.InRange(e.Location, MessageRange));

                if (entry != null)
                {
                    m.SendLocalizedMessage(1152638, GetDirectionString(Utility.GetDirection(m, entry.Location))); // The fish finder pulls you to the ~1_DIRECTION~.
                }
                else
                {
                    m.SendLocalizedMessage(1152637); // The fish finder shows you nothing.
                }
            }
            else
            {
                m.SendLocalizedMessage(1152637); // The fish finder shows you nothing.
            }
        }

        public string GetDirectionString(Direction d)
        {
            return string.Format("#{0}", 1152639 + (int)d);
        }

        public static bool HasSchool(Mobile m)
        {
            if (m == null || !m.Alive || m.Backpack == null)
                return false;

            if (m.Backpack.FindItemByType<MagicalFishFinder>() != null && Schools.ContainsKey(m.Map))
            {
                SchoolEntry entry = Schools[m.Map].FirstOrDefault(e => m.InRange(e.Location, SchoolRange));

                if (entry != null)
                {
                    entry.OnFish();
                    return true;
                }
            }

            return false;
        }

        public MagicalFishFinder(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Expires);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Expires = reader.ReadDateTime();

            if (Expires < DateTime.UtcNow)
                Decay();
            else
                m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), CheckDecay);
        }

        public static int MessageRange = 100;
        public static int SchoolRange = 20;

        public static Dictionary<Map, List<SchoolEntry>> Schools { get; set; }

        public static void Initialize()
        {
            Schools = new Dictionary<Map, List<SchoolEntry>>();

            Schools[Map.Trammel] = new List<SchoolEntry>();
            Schools[Map.Felucca] = new List<SchoolEntry>();
            Schools[Map.Ilshenar] = new List<SchoolEntry>();
            Schools[Map.Tokuno] = new List<SchoolEntry>();

            foreach (KeyValuePair<Map, List<SchoolEntry>> kvp in Schools)
            {
                int amount = 150;

                if (kvp.Key == Map.Ilshenar || kvp.Key == Map.Tokuno)
                    amount = 50;

                for (int i = 0; i < amount; i++)
                {
                    Point3D p;
                    int failsafe = 0;

                    do
                    {
                        p = SOS.FindLocation(kvp.Key);
                        failsafe++;
                    }
                    while (p == Point3D.Zero && failsafe < 10);

                    kvp.Value.Add(new SchoolEntry(kvp.Key, new Point2D(p.X, p.Y)));
                }

                if (kvp.Value.Count == 0)
                    Console.WriteLine("Warning: {0} has 0 School entries!", kvp.Key);
            }

            CommandSystem.Register("MoveToSchool", AccessLevel.GameMaster, e =>
                {
                    Mobile m = e.Mobile;

                    if (Schools.ContainsKey(m.Map))
                    {
                        SchoolEntry entry = Schools[m.Map][Utility.Random(Schools[m.Map].Count)];

                        if (entry != null)
                        {
                            m.MoveToWorld(new Point3D(entry.Location.X, entry.Location.Y, m.Map.GetAverageZ(entry.Location.X, entry.Location.Y)), m.Map);
                        }
                        else
                            m.SendMessage("Bad entry");
                    }
                    else
                        m.SendMessage("Bad map");
                });
        }

        public static void ExpireSchool(Map map, SchoolEntry entry)
        {
            if (Schools.ContainsKey(map) && Schools[map].Contains(entry))
            {
                Schools[map].Remove(entry);

                Point3D p;
                int failsafe = 0;

                do
                {
                    p = SOS.FindLocation(map);
                    failsafe++;
                }
                while (p == Point3D.Zero && failsafe < 10);

                Schools[map].Add(new SchoolEntry(map, new Point2D(p.X, p.Y)));
            }
        }

        public class SchoolEntry
        {
            public Point2D Location { get; set; }
            public bool HasFished { get; set; }
            public Map Map { get; set; }

            public SchoolEntry(Map map, Point2D location)
            {
                Map = map;
                Location = location;
            }

            public void OnFish()
            {
                if (!HasFished)
                {
                    HasFished = true;
                    Timer.DelayCall(TimeSpan.FromMinutes(Utility.RandomMinMax(5, 8)), Expire);
                }
            }

            public void Expire()
            {
                ExpireSchool(Map, this);
            }
        }
    }
}
