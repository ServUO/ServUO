using Server.Items;
using Server.Engines.SeasonalEvents;
using Server.Engines.CityLoyalty;
using Server.Mobiles;
using Server.Network;
using Server.Services.TownCryer;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.ArtisanFestival
{
    public enum CycleMethod
    {
        InOrder,
        Reverse,
        Random
    }

    public class ArtisanFestivalEvent : SeasonalEvent
    {
        public static readonly int _DefaultCityGold = 0; // Per EA, default is 1,000,000
        public static readonly int _StageDuration = 4;
        public static readonly int _ClaimDuration = 3;
        public static readonly double[] _TreeGrowthPoints = new double[] { 1000, 2500, 7500, 15000 };

        public static ArtisanFestivalEvent Instance { get; set; }

        private static readonly City[] Cities = new[]
        {
            City.Britain, City.Jhelom, City.Minoc, City.Moonglow, City.NewMagincia, City.SkaraBrae, City.Trinsic, City.Vesper, City.Yew
        };

        private int _Stage = -1;
        private List<City> _CityOrder;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Stage
        {
            get { return _Stage; }
            set
            {
                var old = _Stage;
                _Stage = value;

                if (old != _Stage)
                {
                    ChangeStage(_Stage);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ForceClaimPeriod
        {
            get { return false; }
            set
            {
                if (value)
                {
                    NextStage = DateTime.UtcNow + TimeSpan.FromDays(_StageDuration - _ClaimDuration);
                    OnTick();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ForceNextStage
        {
            get { return false; }
            set
            {
                if (value)
                {
                    NextStage = DateTime.UtcNow;
                    OnTick();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CycleMethod Cycle { get; set; } = CycleMethod.InOrder;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextStage { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public City CurrentCity => Stage > 0 && Stage < Stages ? CityOrder[Stage] : Cities[0];

        [CommandProperty(AccessLevel.GameMaster)]
        public int Stages => Cities.Length;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ClaimPeriod => NextStage > DateTime.MinValue && NextStage - TimeSpan.FromDays(_StageDuration - _ClaimDuration) < DateTime.UtcNow;

        public Timer Timer { get; set; }

        public List<City> CityOrder => _CityOrder ?? (_CityOrder = GetCityOrder());

        [CommandProperty(AccessLevel.GameMaster)]
        public TownTree TownTree { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SantasGiftBag RewardBag { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public FestivalElf Elf { get; set; }

        public Dictionary<PlayerMobile, double> PointTable { get; set; }
        public Dictionary<PlayerMobile, bool> Winners { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public double PointsToNext => TownTree != null && TownTree.Stage < TreeStage.Five ? _TreeGrowthPoints[(int)TownTree.Stage - 1] : -1;

        [CommandProperty(AccessLevel.GameMaster)]
        public double PointsThisStage => PointTable != null ? PointTable.Sum(kvp => kvp.Value) : 0.0;

        public override EventStatus Status => EventStatus.Seasonal;
        public override bool FreezeDuration => true;

        public ArtisanFestivalEvent(EventType type, string name, EventStatus status, int month, int day, int duration)
            : base(type, name, status, month, day, duration)
        {
            Instance = this;
        }

        public override bool IsActive()
        {
            return Stage != -1;
        }

        public override void CheckEnabled()
        {
            if (!IsActive() && base.IsActive())
            {
                Utility.WriteConsoleColor(ConsoleColor.Green, string.Format("Attempting to Enable {0}...", Name));
                Generate();

                if (!IsActive())
                {
                    Utility.WriteConsoleColor(ConsoleColor.Red, "Failed, check city balances. Try reducing _DefaultCityGold to a more reasonable level.");
                }
                else
                {
                    Utility.WriteConsoleColor(ConsoleColor.Green, string.Format("Success! Artisan Festival Active in {0}.", CurrentCity.ToString()));
                }
            }
        }

        private void AddTownCryerSupport()
        {
            if (TownCryerSystem.GreetingsEntries != null)
            {
                var entry = new TownCryerGreetingEntry(1157283, 1157284)
                {
                    PreLoaded = true
                };

                TownCryerSystem.GreetingsEntries.Add(entry);
                // The Artisan Festival
                /* Ho! Ho! Ho-llo!  The Artisan Festival is a limited time event that runs during the month of December!  Santa needs your help making enough toys for all of
                 * Britannia and beyond!  Toys aren't cheap, and neither are elves *smirks* so we need to dip into the City Treasury to fund the festival.  The City Treasury
                 * will need to have at least 1 million gold...what? It's not like it's a platinum or anything *chuckles*...for the festival to start.  Complete and turn-in
                 * Bulk Order Deeds to me in order to get the biggest and best tree we can to celebrate the Holiday Season!  As you can see, right now, we are a little light
                 * on trees so turn in completed Bulk Order Deeds to me in order to make the tree grow with Holiday Magic!  Here's the best part - The more Bulk Order Deeds
                 * that you hand over to me, the more presents Santa will put in the magic toy bag he gives me!  Give me enough Bulk Order Deeds and we'll have a HUGE tree
                 * with ornaments, lights and even a STAR on top!  If we can make that happen Santa will make sure EVERYONE who participates in the festival gets a present,
                 * even if they have been a little naughty!  Each festival only lasts for three days in each city, so make sure to give me those Bulk Order Deeds so we can
                 * see our tree grow!  Did I mention elves like treats and cookies too?  Good luck!*/
            }
        }

        private void RemoveTownCryerSupport()
        {
            if (TownCryerSystem.GreetingsEntries != null)
            {
                var entry = TownCryerSystem.GreetingsEntries.FirstOrDefault(e => e.Title != null && e.Title.Number == 1157283);

                if (entry != null)
                {
                    TownCryerSystem.GreetingsEntries.Remove(entry);
                }
            }
        }

        private void AddCurrentTCMessage()
        {
            GlobalTownCrierEntryList.Instance.AddEntry(new TextDefinition[] { GetTCMessage() }, TimeSpan.FromDays(_ClaimDuration));
        }

        private void RemoveCurrentTCMessage()
        {
            if (GlobalTownCrierEntryList.Instance.Entries != null)
            {
                var entry = GlobalTownCrierEntryList.Instance.Entries.FirstOrDefault(
                    e =>
                    e.Lines.Length > 0 &&
                    e.Lines[0].Number > 0 &&
                    e.Lines[0].Number >= 1157167 &&
                    e.Lines[0].Number <= 1157175);

                if (entry != null)
                {
                    GlobalTownCrierEntryList.Instance.RemoveEntry(entry);
                }
            }
        }

        private void OnTick()
        {
            if (NextStage <= DateTime.UtcNow)
            {
                IncreaseStage(Stage + 1);
            }
            else if (ClaimPeriod && RewardBag == null)
            {
                SetupClaimPeriod();
            }
        }

        private List<City> GetCityOrder()
        {
            var list = new List<City>(Cities);

            switch (Cycle)
            {
                default:
                    break;
                case CycleMethod.Reverse:
                    list.Reverse();
                    break;
                case CycleMethod.Random:
                    ColUtility.Shuffle(list);
                    break;
            }

            return list;
        }

        private void ChangeStage(int newStage)
        {
            if (newStage > -1)
            {
                // New event
                if (newStage >= Stages)
                {
                    Deactivate();
                }
                else
                {
                    BeginNewStage();
                }
            }
        }

        private void IncreaseStage(int newStage)
        {
            if (newStage > 0)
            {
                RemoveCurrentTCMessage();
            }

            if (newStage < Stages)
            {
                var cityInstance = CityLoyaltySystem.GetCityInstance(CityOrder[newStage]);

                if (cityInstance != null && cityInstance.Treasury >= (long)_DefaultCityGold)
                {
                    Stage++;
                    AddCurrentTCMessage();
                }
                else
                {
                    IncreaseStage(++newStage);
                }
            }
            else
            {
                Deactivate();
            }
        }

        private void BeginNewStage()
        {
            GenerateStageDecoration();

            NextStage = DateTime.UtcNow + TimeSpan.FromDays(_StageDuration);

            var num = GetTCMessage();

            foreach (var ns in NetState.Instances)
            {
                if (ns.Mobile != null)
                {
                    ns.Mobile.SendLocalizedMessage(num);
                }
            }
        }

        private int GetTCMessage()
        {
            switch (CurrentCity)
            {
                case City.Jhelom: return 1157167;
                case City.Minoc: return 1157168;
                case City.Moonglow: return 1157169;
                case City.NewMagincia: return 1157170;
                case City.SkaraBrae: return 1157171;
                case City.Trinsic: return 1157172;
                case City.Vesper: return 1157173;
                case City.Yew: return 1157174;
                case City.Britain: return 1157175;
            }

            return -1;
        }

        private void GenerateStageDecoration()
        {
            if (Winners != null)
            {
                Winners.Clear();
            }

            if (PointTable != null)
            {
                PointTable.Clear();
            }

            if (RewardBag != null)
            {
                RewardBag.Delete();
                RewardBag = null;
            }

            var map = Siege.SiegeShard ? Map.Felucca : Map.Trammel;
            int treeType = 1;

            switch (CurrentCity)
            {
                case City.Trinsic: treeType = 2; break;
                case City.Britain:
                case City.NewMagincia: treeType = 3; break;
                case City.Minoc: treeType = 4; break;
            }

            if (TownTree != null)
            {
                TownTree.Delete();
            }

            TownTree = new TownTree(treeType);
            TownTree.MoveToWorld(_CityLocations[Stage], map);

            if (Elf == null)
            {
                Elf = new FestivalElf();
            }

            var p = new Point3D(TownTree.X + Utility.RandomMinMax(-1, 1), TownTree.Y + Utility.RandomMinMax(-1, 1), TownTree.Z);
            Elf.MoveToWorld(p, map);
            Elf.Home = p;
            Elf.RangeHome = 5;
        }

        private void Deactivate()
        {
            EndTimer();
            Remove();
        }

        public void BeginTimer()
        {
            EndTimer();

            Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), OnTick);
            Timer.Start();

            AddTownCryerSupport();
        }

        private void EndTimer()
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;

                RemoveTownCryerSupport();
            }
        }

        private void SetupClaimPeriod()
        {
            var map = Siege.SiegeShard ? Map.Felucca : Map.Trammel;
            Point3D p = TownTree.Location;

            RewardBag = new SantasGiftBag();
            RewardBag.MoveToWorld(new Point3D(p.X, p.Y + 2, p.Z), map);

            if (PointTable.Count > 0)
            {
                Winners = new Dictionary<PlayerMobile, bool>();

                if (TownTree.Stage == TreeStage.Five)
                {
                    foreach (var pm in PointTable.Keys)
                    {
                        Winners[pm] = false;
                    }
                }
                else
                {
                    double perc;

                    switch (TownTree.Stage)
                    {
                        default: perc = 0.1; break;
                        case TreeStage.Three: perc = .25; break;
                        case TreeStage.Four: perc = .5; break;
                    }

                    int count = (int)Math.Max(1, (double)PointTable.Count * perc);

                    for (int i = 0; i < count; i++)
                    {
                        DoRaffle();
                    }
                }
            }

            PointTable.Clear();

            var tree = TownTree;

            if (tree != null)
            {
                for (int i = 0; i < 30; i++)
                {
                    Point3D spawnPoint;

                    var temp = new Point3D(Utility.RandomMinMax(tree.X - 5, tree.X + 5), Utility.RandomMinMax(tree.Y - 5, tree.Y + 5), tree.Z);

                    IPooledEnumerable eable = map.GetItemsInRange(temp, 0);
                    var spawnZ = temp.Z;

                    foreach (var comp in eable.OfType<AddonComponent>().Where(c => c.ItemID >= 0x46A2 && c.ItemID < 0x46A7))
                    {
                        spawnZ = Math.Max(spawnZ, comp.Z + 5);
                    }

                    eable.Free();

                    if (spawnZ != TownTree.Z)
                    {
                        spawnPoint = temp;
                        spawnPoint.Z = spawnZ;
                    }
                    else
                    {
                        Spells.SpellHelper.AdjustField(ref temp, map, 20, false);

                        spawnPoint = temp;
                    }

                    var component = new AddonComponent(Utility.Random(0x46A2, 6))
                    {
                        Hue = Utility.RandomMinMax(1, 500)
                    };

                    tree.AddComponent(component, tree.X - spawnPoint.X, tree.Y - spawnPoint.Y, tree.Z - spawnPoint.Z);
                }
            }
        }

        public void OnBodTurnIn(PlayerMobile pm, FestivalElf elf, double points)
        {
            if (PointTable.ContainsKey(pm))
            {
                PointTable[pm] += points;
            }
            else
            {
                PointTable[pm] = points;
            }

            var toNext = PointsToNext;

            if (toNext == -1)
            {
                return;
            }

            double perc = PointsThisStage / PointsToNext;

            if (perc >= 1.0)
            {
                elf.Say(1157164, string.Format("{0}\t#{1}", pm.Name, CityLoyaltySystem.GetCityLocalization(CurrentCity)), 1150); // ~1_PLAYER~ has advanced the Artisan Festival in the City of ~2_CITY~!

                TownTree.Stage++;
                FireworkShow(elf);
            }
            else
            {
                int loc;

                if (perc < .25)
                {
                    loc = 1159273; // There is quite a ways to go before the tree will grow!
                }
                else if (perc < .50)
                {
                    loc = 1159277; // The tree is about a quarter of the way before it will grow!
                }
                else if (perc < .75)
                {
                    loc = 1159276; // The tree is about half way before it will grow!
                }
                else if (perc < .90)
                {
                    loc = 1159275; // The tree is about a three-quarters of the way before it will grow!	
                }
                else
                {
                    loc = 1159274; // The tree is very close to being ready to grow!
                }

                Timer.DelayCall(() => elf.Say(loc, 1150));
            }
        }

        private void FireworkShow(IEntity e)
        {
            if (e.Map == null)
            {
                return;
            }

            for (int i = 2; i <= 8; i += 2)
            {
                Timer.DelayCall(TimeSpan.FromMilliseconds((i - 2) * 600), o =>
                {
                    Misc.Geometry.Circle2D(e.Location, e.Map, o, (pnt, map) =>
                    {
                        VvV.VvVAltar.LaunchFireworks(pnt, map);
                    });
                }, i);
            }
        }

        private void DoRaffle()
        {
            double totalPoints = 0;
            var validEntries = new Dictionary<PlayerMobile, double>();

            foreach (var kvp in PointTable)
            {
                validEntries[kvp.Key] = kvp.Value;
                totalPoints += kvp.Value;
            }

            double randomPoints = Utility.RandomMinMax(1, totalPoints);

            totalPoints = 0;

            foreach (var kvp in validEntries)
            {
                totalPoints += kvp.Value;

                if (totalPoints >= randomPoints)
                {
                    var pm = kvp.Key;

                    PointTable.Remove(pm);
                    Winners[pm] = false;
                    return;
                }
            }
        }

        protected override void Generate()
        {
            BeginTimer();

            PointTable = new Dictionary<PlayerMobile, double>();

            Running = true;
            IncreaseStage(0);
        }

        protected override void Remove()
        {
            Utility.WriteConsoleColor(ConsoleColor.Green, string.Format("{0} Disabled!", Name));

            EndTimer();
            Stage = -1;
            NextStage = DateTime.MinValue;

            if (Elf != null)
            {
                Elf.Delete();
                Elf = null;
            }

            if (Winners != null)
            {
                Winners.Clear();
                Winners = null;
            }

            if (TownTree != null)
            {
                TownTree.Delete();
                TownTree = null;
            }

            if (RewardBag != null)
            {
                RewardBag.Delete();
                RewardBag = null;
            }

            ColUtility.Free(_CityOrder);
            _CityOrder = null;

            Running = false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(Stage);
            writer.Write((int)Cycle);
            writer.Write(NextStage);

            writer.WriteItem(TownTree);
            writer.WriteItem(RewardBag);
            writer.WriteMobile(Elf);

            if (PointTable != null)
            {
                writer.Write(0);
                writer.Write(PointTable.Count);

                foreach (var kvp in PointTable)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }
            }
            else
            {
                writer.Write(1);
            }

            if (Winners != null)
            {
                writer.Write(0);
                writer.Write(Winners.Count);

                foreach (var kvp in Winners)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }
            }
            else
            {
                writer.Write(1);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = InheritInsertion ? 0 : reader.ReadInt();

            if(v > 0)
            {
                _Stage = reader.ReadInt();
                Cycle = (CycleMethod)reader.ReadInt();
                NextStage = reader.ReadDateTime();

                TownTree = reader.ReadItem<TownTree>();
                RewardBag = reader.ReadItem<SantasGiftBag>();
                Elf = reader.ReadMobile<FestivalElf>();

                var count = 0;

                if (reader.ReadInt() == 0)
                {
                    count = reader.ReadInt();
                    PointTable = new Dictionary<PlayerMobile, double>();

                    for (int i = 0; i < count; i++)
                    {
                        var pm = reader.ReadMobile<PlayerMobile>();
                        var points = reader.ReadDouble();

                        if (pm != null)
                        {
                            PointTable[pm] = points;
                        }
                    }
                }

                if (reader.ReadInt() == 0)
                {
                    count = reader.ReadInt();
                    Winners = new Dictionary<PlayerMobile, bool>();

                    for (int i = 0; i < count; i++)
                    {
                        var pm = reader.ReadMobile<PlayerMobile>();
                        var claimed = reader.ReadBool();

                        if (pm != null)
                        {
                            Winners[pm] = claimed;
                        }
                    }
                }

                if (Running)
                {
                    BeginTimer();
                }
            }
        }

        private static Point3D[] _CityLocations = new Point3D[]
        {
            new Point3D(1628, 1639, 35), // brit
            new Point3D(1454, 3991, 0), // Jhelom
            new Point3D(2436, 492, 15), // Minoc
            new Point3D(4437, 1062, 0), // Moonglow
            new Point3D(3721, 2066, 12), // Magincia
            new Point3D(625, 2231, 0), // Skara Brae
            new Point3D(1902, 2764, 0), // Trinsic
            new Point3D(2982, 819, 0), // Vesper
            new Point3D(550, 966, 0), // Yew
        };
    }
}
