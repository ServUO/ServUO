using Server.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server.Engines.Astronomy
{
    public enum TimeCoordinate
    {
        FiveToEight,
        NineToEleven,
        Midnight,
        OneToFour,
        Day
    }

    public static class AstronomySystem
    {
        public static bool Enabled = true;
        public static string FilePath = Path.Combine("Saves/Misc", "Astronomy.bin");

        public static readonly int MaxConstellations = 1000;
        public static readonly int MaxRA = 24; // zero based 0 - 24, 1 per increment
        public static readonly double MaxDEC = 90; // zero based 0 - 90, 0.2 per increment

        // testing only. THis needs to be deleted prior to going live.
        //public static TimeCoordinate ForceTimeCoordinate { get { return PersonalTelescope.ForceTimeCoordinate; } }

        public static int LoadedConstellations { get; set; }
        public static List<ConstellationInfo> Constellations { get; set; }
        public static List<Tuple<int, int>> InterstellarObjects { get; set; }

        public static List<int> DiscoveredConstellations { get; set; }

        public static void Configure()
        {
            if (Enabled)
            {
                EventSink.WorldSave += OnSave;
                EventSink.WorldLoad += OnLoad;

                Constellations = new List<ConstellationInfo>();
                InterstellarObjects = new List<Tuple<int, int>>();
                DiscoveredConstellations = new List<int>();

                // comets
                for (int i = 0x68D; i <= 0x693; i++)
                {
                    InterstellarObjects.Add(new Tuple<int, int>(i, 1158514));
                }

                // felucca
                for (int i = 0x69F; i <= 0x6A6; i++)
                {
                    InterstellarObjects.Add(new Tuple<int, int>(i, 1158734));
                }

                // trammel
                for (int i = 0x6A7; i <= 0x6AE; i++)
                {
                    InterstellarObjects.Add(new Tuple<int, int>(i, 1158735));
                }

                // galaxy
                for (int i = 0x6AF; i <= 0x6BC; i++)
                {
                    InterstellarObjects.Add(new Tuple<int, int>(i, 1158736));
                }

                // planet
                for (int i = 0x6BD; i <= 0x6CD; i++)
                {
                    InterstellarObjects.Add(new Tuple<int, int>(i, 1158737));
                }
            }
        }

        public static void Initialize()
        {
            if (Enabled && LoadedConstellations < MaxConstellations)
            {
                CreateConstellations(MaxConstellations - LoadedConstellations);
            }
        }

        private static void CreateConstellations(int amount)
        {
            TimeCoordinate next = TimeCoordinate.FiveToEight;

            if (LoadedConstellations > 0)
            {
                if (Constellations.Where(c => c.TimeCoordinate == TimeCoordinate.FiveToEight).Count() > Constellations.Where(c => c.TimeCoordinate == TimeCoordinate.NineToEleven).Count())
                    next = TimeCoordinate.NineToEleven;
                else if (Constellations.Where(c => c.TimeCoordinate == TimeCoordinate.NineToEleven).Count() > Constellations.Where(c => c.TimeCoordinate == TimeCoordinate.Midnight).Count())
                    next = TimeCoordinate.Midnight;
                else if (Constellations.Where(c => c.TimeCoordinate == TimeCoordinate.Midnight).Count() > Constellations.Where(c => c.TimeCoordinate == TimeCoordinate.OneToFour).Count())
                    next = TimeCoordinate.OneToFour;
            }

            for (int i = 0; i < amount; i++)
            {
                int ra = 0;
                double dec = 0.0;

                do
                {
                    ra = Utility.RandomMinMax(0, MaxRA);
                    dec = Utility.RandomMinMax(0, (int)MaxDEC) + Utility.RandomList(.2, .4, .6, .8, .0);
                }
                while (CheckExists(next, ra, dec));

                ConstellationInfo info = new ConstellationInfo(next, ra, dec, ConstellationInfo.RandomStarPositions());
                Constellations.Add(info);

                info.Identifier = Constellations.Count - 1;

                switch (next)
                {
                    case TimeCoordinate.FiveToEight: next = TimeCoordinate.NineToEleven; break;
                    case TimeCoordinate.NineToEleven: next = TimeCoordinate.Midnight; break;
                    case TimeCoordinate.Midnight: next = TimeCoordinate.OneToFour; break;
                    case TimeCoordinate.OneToFour: next = TimeCoordinate.FiveToEight; break;
                }
            }
        }

        public static void ResetConstellations()
        {
            ColUtility.Free(Constellations);
            LoadedConstellations = 0;

            CreateConstellations(MaxConstellations);
            Console.WriteLine("Reset Constellations!");
        }

        public static ConstellationInfo GetConstellation(int id)
        {
            return Constellations.FirstOrDefault(info => info.Identifier == id);
        }

        public static ConstellationInfo GetConstellation(TimeCoordinate p, int ra, double dec)
        {
            return Constellations.FirstOrDefault(c => c.TimeCoordinate == p && c.CoordRA == ra && c.CoordDEC == dec);
        }

        private static bool CheckExists(TimeCoordinate p, int ra, double dec)
        {
            return Constellations.Any(c => c.TimeCoordinate == p && c.CoordRA == ra && c.CoordDEC == dec);
        }

        public static bool CheckNameExists(string name)
        {
            return Constellations.Any(c => !string.IsNullOrEmpty(c.Name) && c.Name.ToLower() == name.ToLower());
        }

        public static TimeCoordinate GetTimeCoordinate(IEntity e)
        {
            int minutes, hours, totalMinutes;

            Clock.GetTime(e.Map, e.X, e.Y, out hours, out minutes, out totalMinutes);

            if (hours >= 17 && hours < 21)
            {
                return TimeCoordinate.FiveToEight;
            }

            if (hours >= 21 && hours < 24)
            {
                return TimeCoordinate.NineToEleven;
            }

            if ((hours >= 24 && hours < 1) || hours == 0)
            {
                return TimeCoordinate.Midnight;
            }

            if (hours >= 1 && hours <= 4)
            {
                return TimeCoordinate.OneToFour;
            }

            return TimeCoordinate.Day;
        }

        public static int RandomSkyImage(Mobile m)
        {
            return RandomSkyImage(GetTimeCoordinate(m));
        }

        public static int RandomSkyImage(TimeCoordinate TimeCoordinate)
        {
            switch (TimeCoordinate)
            {
                default: return 0x67E;
                case TimeCoordinate.FiveToEight: return 0x67F;
                case TimeCoordinate.NineToEleven: return Utility.RandomMinMax(0x680, 0x682);
                case TimeCoordinate.Midnight: return 0x686;
                case TimeCoordinate.OneToFour: return Utility.RandomMinMax(0x683, 0x685);
            }
        }

        public static Tuple<int, int> GetRandomInterstellarObject()
        {
            return InterstellarObjects[Utility.Random(InterstellarObjects.Count)];
        }

        public static int TimeCoordinateLocalization(TimeCoordinate TimeCoordinate)
        {
            switch (TimeCoordinate)
            {
                default:
                case TimeCoordinate.FiveToEight: return 1158506;
                case TimeCoordinate.NineToEleven: return 1158507;
                case TimeCoordinate.Midnight: return 1158508;
                case TimeCoordinate.OneToFour: return 1158509;
            }
        }

        public static void AddDiscovery(ConstellationInfo info)
        {
            if (!DiscoveredConstellations.Contains(info.Identifier))
            {
                DiscoveredConstellations.Add(info.Identifier);
            }
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(0);

                    writer.Write(Constellations.Count);

                    foreach (ConstellationInfo info in Constellations)
                    {
                        info.Serialize(writer);
                    }
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    reader.ReadInt();

                    LoadedConstellations = reader.ReadInt();

                    for (int i = 0; i < LoadedConstellations; i++)
                    {
                        Constellations.Add(new ConstellationInfo(reader));
                    }
                });
        }
    }
}
