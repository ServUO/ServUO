using Server.Engines.CityLoyalty;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.Blackthorn
{
    public enum InvasionType
    {
        Dragons,
        Undead,
        Elemental,
        Daemon,
        Orc,
        Wildlings,
        Frost,
        Arachnid,
        Fey,
        Nature
    }

    public class InvasionController : Item
    {
        public static bool Enabled = true;
        public static int WaveCountMin = 8;
        public static int WaveCountMax = 10;
        public static int MaxWaves = 2;

        [CommandProperty(AccessLevel.Administrator)]
        public static InvasionController TramInstance { get; set; }

        [CommandProperty(AccessLevel.Administrator)]
        public static InvasionController FelInstance { get; set; }

        [CommandProperty(AccessLevel.Administrator)]
        public bool ForceRespawn
        {
            get
            {
                return false;
            }
            set
            {
                if (!value)
                    return;

                RemoveSpawn();
                OnEndInvasion();
                Cleanup();
                BeginInvasion();
            }
        }

        public static Dictionary<City, InvasionDefinition> Defs;

        [CommandProperty(AccessLevel.GameMaster)]
        public City CurrentInvasion { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public InvasionType InvasionType { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public InvasionBeacon Beacon { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpawnCount
        {
            get
            {
                if (Spawn == null || Spawn.Count == 0)
                    return 0;

                int count = 0;

                foreach (KeyValuePair<BaseCreature, List<BaseCreature>> kvp in Spawn)
                {
                    if (kvp.Key.Alive)
                        count++;

                    count += kvp.Value.Where(bc => bc.Alive).Count();
                }

                return count;
            }
        }

        public Dictionary<BaseCreature, List<BaseCreature>> Spawn { get; set; }

        public List<Rectangle2D> SpawnZones { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CurrentWave { get; private set; }

        public bool BeaconVulnerable => Beacon != null && (Spawn == null || Spawn.Count == 0);

        public InvasionController(Map map) : base(3796)
        {
            Movable = false;
            Visible = false;

            Spawn = new Dictionary<BaseCreature, List<BaseCreature>>();
            SpawnZones = new List<Rectangle2D>();

            if (Enabled)
                Timer.DelayCall(TimeSpan.FromSeconds(10), BeginInvasion);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.GameMaster)
            {
                from.SendGump(new Gumps.PropertiesGump(from, this));
            }
        }

        private readonly Type[][] _SpawnTable =
        {
            new Type[] { typeof(Dragon), typeof(Drake), typeof(GiantSerpent), typeof(Reptalon), typeof(Hydra) },
            new Type[] { typeof(Lich), typeof(Wraith), typeof(Mummy), typeof(Zombie), typeof(SkeletalKnight), typeof(BoneKnight) },
            new Type[] { typeof(MudElemental), typeof(MoltenEarthElemental), typeof(DiseasedBloodElemental), typeof(GreaterAirElemental),
                         typeof(GreaterBloodElemental), typeof(GreaterEarthElemental), typeof(GreaterWaterElemental), typeof(ShameGreaterPoisonElemental),
                         typeof(StoneElemental) },
            new Type[] { typeof(Daemon), typeof(Succubus), typeof(Imp), typeof(ChaosDaemon), typeof(BoneDemon) },
            new Type[] { typeof(Orc), typeof(OrcBomber), typeof(OrcishMage), typeof(OrcishLord) },
            new Type[] { typeof(SwampTentacle), typeof(PlagueBeast), typeof(Bogling), typeof(FeralTreefellow) },
            new Type[] { typeof(IceElemental), typeof(SnowElemental), typeof(IceFiend), typeof(FrostTroll), typeof(IceSerpent) },
            new Type[] { typeof(DreadSpider), typeof(GiantBlackWidow), typeof(Scorpion), typeof(TerathanWarrior), typeof(WolfSpider) },
            new Type[] { typeof(Satyr), typeof(Centaur), typeof(CuSidhe), typeof(Wisp), typeof(MLDryad) },
            new Type[] { typeof(DireWolf), typeof(GiantRat), typeof(Troglodyte), typeof(RagingGrizzlyBear), typeof(GreaterMongbat) },
        };

        public void BeginInvasion()
        {
            if (!Enabled)
                return;

            RemoveSpawn();

            CurrentWave = 1;
            InvasionType newType;
            City newCity;

            do
            {
                newType = (InvasionType)Utility.Random(10);
            }
            while (newType == InvasionType);

            do
            {
                newCity = (City)Utility.Random(9);
            }
            while (newCity == CurrentInvasion);

            CurrentInvasion = newCity;
            InvasionType = newType;
            SpawnZones = Defs[CurrentInvasion].SpawnRecs.ToList();

            Beacon = new InvasionBeacon(this);
            Beacon.MoveToWorld(Defs[CurrentInvasion].BeaconLoc, Map);

            // Shuffle zones
            for (int i = 0; i < 8; i++)
            {
                Rectangle2D rec = SpawnZones[Utility.Random(SpawnZones.Count)];
                SpawnZones.Remove(rec);
                SpawnZones.Insert(0, rec);
            }

            SpawnWave();
        }

        public void SpawnWave()
        {
            List<Rectangle2D> zones = new List<Rectangle2D>(SpawnZones);

            for (int j = 0; j < 2; j++)
            {
                Rectangle2D spawnrec = zones[Utility.Random(zones.Count)];
                zones.Remove(spawnrec);

                int count = Utility.RandomMinMax(WaveCountMin, WaveCountMax);
                List<BaseCreature> list = new List<BaseCreature>();

                for (int i = 0; i < count; i++)
                {
                    BaseCreature bc = Activator.CreateInstance(_SpawnTable[(int)InvasionType][Utility.Random(_SpawnTable[(int)InvasionType].Length)]) as BaseCreature;

                    bc.Kills = 100;

                    if (bc.FightMode == FightMode.Evil)
                    {
                        bc.FightMode = FightMode.Aggressor;
                    }

                    if (SpawnMobile(bc, spawnrec))
                    {
                        list.Add(bc);
                    }
                    else
                    {
                        bc.Delete();
                    }
                }

                for (int i = 0; i < 3; i++)
                {
                    Invader invader = new Invader(InvasionType);

                    if (SpawnMobile(invader, spawnrec))
                    {
                        list.Add(invader);
                    }
                    else
                        invader.Delete();
                }

                InvaderCaptain capt = new InvaderCaptain(InvasionType)
                {
                    Blessed = true
                };

                if (SpawnMobile(capt, spawnrec) || SpawnMobile(capt, new Rectangle2D(Defs[CurrentInvasion].BeaconLoc.X - 10, Defs[CurrentInvasion].BeaconLoc.Y - 10, 20, 20)))
                {
                    Spawn[capt] = list;
                }
            }
        }

        private bool SpawnMobile(BaseCreature bc, Rectangle2D spawnrec)
        {
            if (Map == null)
                return false;

            if (bc != null)
            {
                for (int i = 0; i < 25; i++)
                {
                    Point3D p = Map.GetRandomSpawnPoint(spawnrec);
                    bool exempt = false;

                    if (spawnrec.X == 6444 && spawnrec.Y == 2446)
                    {
                        exempt = true;
                        p.Z = -2;
                    }

                    if (exempt || Map.CanFit(p.X, p.Y, p.Z, 16, false, false, true))
                    {
                        bc.MoveToWorld(p, Map);
                        bc.Home = Defs[CurrentInvasion].BeaconLoc;
                        bc.SeeksHome = true;
                        bc.RangeHome = Utility.RandomMinMax(5, 10);
                        bc.CanSwim = false;
                        bc.Tamable = false;

                        return true;
                    }
                }
            }

            return false;
        }

        public void OnDeath(BaseCreature bc)
        {
            if (bc == null || bc.Controlled || bc.Summoned)
                return;

            if (Spawn.ContainsKey(bc))
            {
                Spawn.Remove(bc);

                bool wavecomplete = true;
                foreach (KeyValuePair<BaseCreature, List<BaseCreature>> kvp in Spawn)
                {
                    if (kvp.Key != null && kvp.Key.Alive)
                    {
                        wavecomplete = false;
                        break;
                    }
                }

                if (wavecomplete)
                    CompleteWave();
            }
            else
            {
                foreach (KeyValuePair<BaseCreature, List<BaseCreature>> kvp in Spawn)
                {
                    if (kvp.Value.Contains(bc))
                        kvp.Value.Remove(bc);

                    int count = kvp.Value.Where(b => b != null && b.Alive).Count();

                    if (count == 0 && kvp.Key.Alive)
                    {
                        kvp.Key.Blessed = false;
                        kvp.Key.Delta(MobileDelta.Noto);
                    }
                }
            }
        }

        public void CleanupSpawn()
        {
            if (Spawn == null)
                return;

            List<BaseCreature> list = null;

            foreach (KeyValuePair<BaseCreature, List<BaseCreature>> kvp in Spawn)
            {
                if (kvp.Value != null)
                {
                    list = new List<BaseCreature>(kvp.Value);

                    foreach (BaseCreature b in list.Where(bc => bc == null || !bc.Alive || bc.Deleted))
                        kvp.Value.Remove(b);
                }

                if (list != null && list.Count > 0)
                {
                    list.Clear();
                    list.TrimExcess();
                }
            }
        }

        private void CompleteWave()
        {
            if (CurrentWave == MaxWaves)
            {
                DoMessage();
            }
            else
            {
                DoMessage();
                CurrentWave++;
            }
        }

        private void DoMessage()
        {
            if (Map == null)
                return;

            IPooledEnumerable eable = Map.GetMobilesInRange(Beacon.Location, 20);

            foreach (Mobile m in eable)
            {
                if (m != null && m.NetState != null)
                    m.PrivateOverheadMessage(Network.MessageType.Regular, 1154, 1154550, m.NetState); // *A sound roars in the distance...Minax's Beacon is vulnerable to attack!!*
            }

            eable.Free();
        }

        public void OnBeaconDestroyed()
        {
            OnEndInvasion();

            Timer.DelayCall(TimeSpan.FromMinutes(2), () =>
                {
                    BeginInvasion();
                });
        }

        public void OnEndInvasion()
        {
            if (Beacon != null)
            {
                List<Mobile> rights = Beacon.GetLootingRights();

                if (rights != null)
                {
                    foreach (Mobile damager in rights.Where(mob => mob.InRange(Beacon.Location, 12)))
                    {
                        if (0.15 < Utility.RandomDouble())
                            continue;

                        Item i = CreateItem(damager);

                        if (i != null)
                        {
                            damager.PlaySound(0x5B4);
                            damager.SendLocalizedMessage(1154554); // You recover an artifact bearing the crest of Minax from the rubble.

                            if (!damager.PlaceInBackpack(i))
                            {
                                if (damager.BankBox != null && damager.BankBox.TryDropItem(damager, i, false))
                                    damager.SendLocalizedMessage(1079730); // The item has been placed into your bank box.
                                else
                                {
                                    damager.SendLocalizedMessage(1072523); // You find an artifact, but your backpack and bank are too full to hold it.
                                    i.MoveToWorld(damager.Location, damager.Map);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static Item CreateItem(Mobile damager)
        {
            Item i = Loot.RandomArmorOrShieldOrWeaponOrJewelry(LootPackEntry.IsInTokuno(damager), LootPackEntry.IsMondain(damager), LootPackEntry.IsStygian(damager));

            if (i != null)
            {
                RunicReforging.GenerateRandomItem(i, damager, Utility.RandomMinMax(700, 800), damager is PlayerMobile ? ((PlayerMobile)damager).RealLuck : 0, ReforgedPrefix.None, ReforgedSuffix.Minax);
            }

            return i;
        }

        public void Cleanup()
        {
            if (Beacon != null && !Beacon.Deleted)
            {
                Beacon.Delete();
            }
        }

        public void RemoveSpawn()
        {
            if (Spawn == null)
                return;

            Dictionary<BaseCreature, List<BaseCreature>> copy = Spawn;
            Spawn = new Dictionary<BaseCreature, List<BaseCreature>>();

            foreach (KeyValuePair<BaseCreature, List<BaseCreature>> kvp in copy)
            {
                foreach (BaseCreature bc in kvp.Value.Where(b => b.Alive))
                    bc.Kill();

                if (kvp.Key.Alive)
                {
                    kvp.Key.Blessed = false;
                    kvp.Key.Kill();
                }
            }

            copy.Clear();
        }

        public InvasionController(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)CurrentInvasion);
            writer.Write((int)InvasionType);
            writer.Write(Beacon);
            writer.Write(CurrentWave);

            writer.Write(SpawnZones == null ? 0 : SpawnZones.Count);
            SpawnZones.ForEach(rec => writer.Write(rec));

            writer.Write(Spawn == null ? 0 : Spawn.Count);
            foreach (KeyValuePair<BaseCreature, List<BaseCreature>> kvp in Spawn)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value.Count);
                kvp.Value.ForEach(bc => writer.Write(bc));
            }

            Timer.DelayCall(TimeSpan.FromSeconds(30), CleanupSpawn);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Spawn = new Dictionary<BaseCreature, List<BaseCreature>>();
            SpawnZones = new List<Rectangle2D>();

            if (Map == Map.Trammel)
                TramInstance = this;

            if (Map == Map.Felucca)
                FelInstance = this;

            CurrentInvasion = (City)reader.ReadInt();
            InvasionType = (InvasionType)reader.ReadInt();
            Beacon = reader.ReadItem() as InvasionBeacon;
            CurrentWave = reader.ReadInt();

            if (Beacon != null)
                Beacon.Controller = this;

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                SpawnZones.Add(reader.ReadRect2D());
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                BaseCreature captain = reader.ReadMobile() as BaseCreature;
                int c = reader.ReadInt();

                List<BaseCreature> list = new List<BaseCreature>();

                for (int j = 0; j < c; j++)
                {
                    BaseCreature spawn = reader.ReadMobile() as BaseCreature;

                    if (spawn != null)
                    {
                        list.Add(spawn);
                    }
                }

                if (captain != null)
                    Spawn[captain] = list;
                else
                {
                    list.Clear();
                }
            }

            Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
            {
                if (Beacon == null || Beacon.Destroyed)
                {
                    Timer.DelayCall(TimeSpan.FromMinutes(2), () =>
                    {
                        Cleanup();
                        BeginInvasion();
                    });
                }
            });
        }

        public static void Initialize()
        {
            if (TramInstance == null)
            {
                TramInstance = new InvasionController(Map.Trammel);
                TramInstance.MoveToWorld(new Point3D(6359, 2570, 0), Map.Trammel);
            }

            if (FelInstance == null)
            {
                TramInstance = new InvasionController(Map.Felucca);
                TramInstance.MoveToWorld(new Point3D(6359, 2570, 0), Map.Felucca);
            }

            Defs = new Dictionary<City, InvasionDefinition>();

            Defs[City.Moonglow] = new InvasionDefinition(
                new Rectangle2D[]
                {
                    new Rectangle2D(6314, 2571, 10, 5),
                    new Rectangle2D(6288, 2535, 8, 15),
                    new Rectangle2D(6322, 2527, 8, 8),
                    new Rectangle2D(6302, 2524, 10, 5),
                },
                new Point3D(6317, 2555, 0));

            Defs[City.Britain] = new InvasionDefinition(
                new Rectangle2D[]
                {
                    new Rectangle2D(6296, 2464, 7, 7),
                    new Rectangle2D(6332, 2473, 8, 10),
                    new Rectangle2D(6320, 2508, 3, 8),
                    new Rectangle2D(6287, 2494, 8, 8),
                },
                new Point3D(6316, 2477, 11));

            Defs[City.Jhelom] = new InvasionDefinition(
                new Rectangle2D[]
                {
                    new Rectangle2D(6450, 2465, 10, 8),
                    new Rectangle2D(6418, 2497, 15, 5),
                    new Rectangle2D(6417, 2469, 5, 10),
                    new Rectangle2D(6432, 2507, 10, 5),
                },
                new Point3D(6448, 2492, 5));

            Defs[City.Yew] = new InvasionDefinition(
                new Rectangle2D[]
                {
                    new Rectangle2D(6314, 2397, 12, 5),
                    new Rectangle2D(6317, 2440, 10, 10),
                    new Rectangle2D(6286, 2432, 8, 8),
                    new Rectangle2D(6289, 2405, 5, 5),
                },
                new Point3D(6305, 2423, 0));

            Defs[City.Minoc] = new InvasionDefinition(
                new Rectangle2D[]
                {
                    new Rectangle2D(6309, 2339, 10, 5),
                    new Rectangle2D(6290, 2367, 5, 10),
                    new Rectangle2D(6304, 2378, 10, 5),
                    new Rectangle2D(6323, 2344, 5, 10)
                },
                new Point3D(6307, 2362, 15));

            Defs[City.Trinsic] = new InvasionDefinition(
                new Rectangle2D[]
                {
                    new Rectangle2D(6356, 2371, 10, 10),
                    new Rectangle2D(6354, 2344, 5, 10),
                    new Rectangle2D(6366, 2344, 5, 7),
                    new Rectangle2D(6386, 2355, 8, 8),
                },
                new Point3D(6402, 2368, 25));

            Defs[City.SkaraBrae] = new InvasionDefinition(
                new Rectangle2D[]
                {
                    new Rectangle2D(6434, 2330, 10, 5),
                    new Rectangle2D(6456, 2342, 5, 10),
                    new Rectangle2D(6458, 2368, 15, 6),
                    new Rectangle2D(6440, 2384, 10, 3),
                    new Rectangle2D(6412, 2360, 12, 12),
                },
                new Point3D(6442, 2351, 0));

            Defs[City.NewMagincia] = new InvasionDefinition(
                new Rectangle2D[]
                {
                    new Rectangle2D(6426, 2397, 10, 5),
                    new Rectangle2D(6444, 2446, 10, 5),
                    new Rectangle2D(6436, 2395, 5, 8),
                    new Rectangle2D(6419, 2446, 10, 5),
                },
                new Point3D(6440, 2419, 26));

            Defs[City.Vesper] = new InvasionDefinition(
                new Rectangle2D[]
                {
                    new Rectangle2D(6428, 2534, 10, 5),
                    new Rectangle2D(6458, 2534, 5, 10),
                    new Rectangle2D(6460, 2551, 5, 10),
                    new Rectangle2D(6433, 2561, 6, 6),
                },
                new Point3D(6444, 2553, 0));
        }
    }
}
