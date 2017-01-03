using Server;
using System;
using Server.Multis;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Commands;
using Server.Gumps;

namespace Server.Engines.Quests
{
    public class BountyQuestSpawner : Item
    {
        public static void Configure()
        {
            m_ActiveZones = new Dictionary<SpawnZone, List<BaseShipCaptain>>();

            foreach (int i in Enum.GetValues(typeof(SpawnZone)))
                m_ActiveZones.Add((SpawnZone)i, new List<BaseShipCaptain>());
        }

        public static void GenerateShipSpawner()
        {
            if (m_Instance == null)
            {
                m_Instance = new BountyQuestSpawner();
                m_Instance.MoveToWorld(new Point3D(4558, 2347, 0), Map.Trammel);
            }
        }

        private static readonly int[] GoldRange = new int[] { 1000, 10000 };

        private static BountyQuestSpawner m_Instance;
        public static BountyQuestSpawner Instance
        { 
            get 
            {
                return m_Instance;
            } 
        }

        private static Dictionary<Mobile, int> m_Bounties = new Dictionary<Mobile, int>();
        public static Dictionary<Mobile, int> Bounties { get { return m_Bounties; } }

        private static Dictionary<SpawnZone, SpawnDefinition> m_Zones = new Dictionary<SpawnZone, SpawnDefinition>();
        public static Dictionary<SpawnZone, SpawnDefinition> Zones { get { return m_Zones; } }

        private Timer m_Timer;
        public Timer Timer { get { return m_Timer; } }

        private static Dictionary<SpawnZone, List<BaseShipCaptain>> m_ActiveZones;

        private int m_MaxTram;
        private int m_MaxFel;
        private int m_MaxTokuno;
        private TimeSpan m_SpawnTime;
        private bool m_Active;

        [CommandProperty(AccessLevel.GameMaster)]
        public int aFelMoonglowCount { get { return m_ActiveZones[SpawnZone.FelMoonglow].Count; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int aTramMoonglowCount { get { return m_ActiveZones[SpawnZone.TramMoonglow].Count; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int aTramJhelomCount { get { return m_ActiveZones[SpawnZone.TramJhelom].Count; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int aFelJhelomCount { get { return m_ActiveZones[SpawnZone.FelJhelom].Count; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int aTokunoPirateCount { get { return m_ActiveZones[SpawnZone.TokunoPirate].Count; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int aTramMerch1Count { get { return m_ActiveZones[SpawnZone.TramMerch1].Count; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int aFelMerch1Count { get { return m_ActiveZones[SpawnZone.FelMerch1].Count; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int aTramMerch2Count { get { return m_ActiveZones[SpawnZone.TramMerch2].Count; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int aFelMerch2Count { get { return m_ActiveZones[SpawnZone.FelMerch2].Count; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int aTokunoMerchCount { get { return m_ActiveZones[SpawnZone.TokunoMerch].Count; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxTram { get { return m_MaxTram; } set { m_MaxTram = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxFel { get { return m_MaxFel; } set { m_MaxFel = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxTokuno { get { return m_MaxTokuno; } set { m_MaxTokuno = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan SpawnTime 
        { 
            get { return m_SpawnTime; } 
            set 
            {
                m_SpawnTime = value;

                if (m_Timer != null)
                    m_Timer.Stop();

                m_Timer = Timer.DelayCall(m_SpawnTime, m_SpawnTime, new TimerCallback(OnTick));

            } 
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int bMerchantCount 
        { 
            get 
            {
                int count = 0;

                foreach (List<BaseShipCaptain> list in m_ActiveZones.Values) {
                    foreach(BaseShipCaptain capt in list) {
                        if (capt is MerchantCaptain)
                            count++;
                    }
                }

                return count; 
            } 
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int bPirateCount { get { return m_Bounties.Count; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return m_Active; }
            set 
            {
                if (m_Active != value)
                {
                    if (value)
                    {
                        if (m_Timer != null)
                        {
                            m_Timer.Stop();
                            m_Timer = null;
                        }
                        
                        m_Timer = Timer.DelayCall(m_SpawnTime, m_SpawnTime, new TimerCallback(OnTick));
                    }
                    else
                    {
                        if (m_Timer != null)
                            m_Timer.Stop();

                        m_Timer = null;

                        RemoveSpawns();
                    }
                }

                m_Active = value; 
            }
        }

        [Constructable]
        public BountyQuestSpawner() : base(0xED4)
        {
            if (m_Instance != null && !m_Instance.Deleted)
            {
                Active = false;
                Delete();
            }

            Name = "Pirate/Merchant Spawner";
            Visible = false;
            Movable = false;
            m_SpawnTime = TimeSpan.FromMinutes(15);

            m_MaxTram = 2;
            m_MaxFel = 2;
            m_MaxTokuno = 1;

            Active = true;
            m_Instance = this;
        }

        public override void Delete()
        {
            base.Delete();

            Active = false;
        }

        public static void AddZone(SpawnDefinition def)
        {
            if (!m_Zones.ContainsKey(def.Zone))
                m_Zones.Add(def.Zone, def);
        }

        public void HandleDeath(BaseShipCaptain captain)
        {
            if (captain is PirateCaptain)
                RemoveBounty(captain);

            else if (captain is MerchantCaptain)
                RemoveMerchant(captain);
        }

        public void RemoveBounty(BaseShipCaptain pirate)
        {
            SpawnZone zone = pirate.Zone;

            if (m_ActiveZones[zone].Contains(pirate))
                m_ActiveZones[zone].Remove(pirate);

            if (m_Bounties.ContainsKey(pirate))
                m_Bounties.Remove(pirate);
        }

        public void RemoveMerchant(BaseShipCaptain merchant)
        {
            SpawnZone zone = merchant.Zone;

            if (m_ActiveZones[zone].Contains(merchant))
                m_ActiveZones[zone].Remove(merchant);
        }

        public void OnTick()
        {
            if (m_Active)
                SpawnRandom();
        }

        public void SpawnRandom()
        {
            foreach (int i in Enum.GetValues(typeof(SpawnZone)))
            {
                SpawnZone zone = (SpawnZone)i;

                switch (zone)
                {
                    case SpawnZone.TramJhelom:
                        if (m_ActiveZones[zone].Count < m_MaxTram)
                            SpawnPirateAndGalleon(zone, Map.Trammel);
                        break;
                    case SpawnZone.FelJhelom:
                        if (m_ActiveZones[zone].Count < m_MaxFel)
                            SpawnPirateAndGalleon(zone, Map.Felucca);
                        break;
                    case SpawnZone.TramMoonglow:
                        if (m_ActiveZones[zone].Count < m_MaxTram)
                            SpawnPirateAndGalleon(zone, Map.Trammel);
                        break;
                    case SpawnZone.FelMoonglow:
                        if (m_ActiveZones[zone].Count < m_MaxFel)
                            SpawnPirateAndGalleon(zone, Map.Felucca);
                        break;
                    case SpawnZone.TokunoPirate:
                        if (m_ActiveZones[zone].Count < m_MaxTokuno)
                            SpawnPirateAndGalleon(zone, Map.Tokuno);
                        break;
                    case SpawnZone.TramMerch1:
                        if (m_ActiveZones[zone].Count < m_MaxTram)
                            SpawnMerchantAndGalleon(zone, Map.Trammel);
                        break;
                    case SpawnZone.TramMerch2:
                        if (m_ActiveZones[zone].Count < m_MaxTram)
                            SpawnMerchantAndGalleon(zone, Map.Trammel);
                        break;
                    case SpawnZone.FelMerch1:
                        if (m_ActiveZones[zone].Count < m_MaxFel)
                            SpawnMerchantAndGalleon(zone, Map.Felucca);
                        break;
                    case SpawnZone.FelMerch2:
                        if (m_ActiveZones[zone].Count < m_MaxFel)
                            SpawnMerchantAndGalleon(zone, Map.Felucca);
                        break;
                    case SpawnZone.TokunoMerch:
                        if (m_ActiveZones[zone].Count < m_MaxTokuno)
                            SpawnMerchantAndGalleon(zone, Map.Tokuno);
                        break;
                }
            }
        }

        public void RemoveSpawns()
        {
            List<BaseShipCaptain> ToRemove = new List<BaseShipCaptain>();

            foreach (List<BaseShipCaptain> list in m_ActiveZones.Values)
            {
                foreach (BaseShipCaptain capt in list)
                    ToRemove.Add(capt);
            }

            foreach (BaseShipCaptain cap in ToRemove)
                cap.TryDecayGalleon(cap.Galleon);
        }

        public void SpawnPirateAndGalleon(SpawnZone zone, Map map)
        {
            SpawnDefinition def = m_Zones[zone];

            if (map != Map.Internal && map != null)
            {
                Rectangle2D rec = def.SpawnRegion;
                OrcishGalleon gal = new OrcishGalleon(Direction.North);
                PirateCaptain pirate = new PirateCaptain(gal);
                pirate.Zone = zone;
                gal.Owner = pirate;
                Point3D p = Point3D.Zero;
                bool spawned = false;
                for(int i = 0; i < 25; i++)
                {
                    int x = Utility.Random(rec.X, rec.Width);
                    int y = Utility.Random(rec.Y, rec.Height);
                    p = new Point3D(x, y, -5);

                    if (gal.CanFit(p, map, gal.ItemID))
                    {
                        spawned = true;
                        break;
                    }
                }

                if (!spawned)
                {
                    gal.Delete();
                    pirate.Delete();
                    return;
                }

                int gold = Utility.RandomMinMax(GoldRange[0], GoldRange[1]);
                gal.MoveToWorld(p, map);
                gal.AutoAddCannons(pirate);
                pirate.MoveToWorld(new Point3D(gal.X, gal.Y - 1, gal.ZSurface), map);

                int crewCount = Utility.RandomMinMax(3, 5);

                for (int i = 0; i < crewCount; i++)
                {
                    Mobile crew = new PirateCrew();

                    if (i == 0)
                        crew.Title = "the orc captain";

                    pirate.AddToCrew(crew);
                    crew.MoveToWorld(new Point3D(gal.X + Utility.RandomList(-1, 1), gal.Y + Utility.RandomList(-1, 0, 1), gal.ZSurface), map);
                }

                Point2D[] course = def.GetRandomWaypoints();
                gal.BoatCourse = new BoatCourse(gal, new List<Point2D>(def.GetRandomWaypoints()));

                gal.NextNavPoint = 0;
                gal.StartCourse(false, false);

                FillHold(gal);

                m_Bounties.Add(pirate, gold);
                m_ActiveZones[zone].Add(pirate);
            }
        }

        public void SpawnMerchantAndGalleon(SpawnZone zone, Map map)
        {
            SpawnDefinition def = m_Zones[zone];

            if (map != Map.Internal && map != null)
            {
                Rectangle2D rec = def.SpawnRegion;
                bool garg = Utility.RandomBool();
                BaseGalleon gal;
                Point3D p = Point3D.Zero;
                bool spawned = false;

                if (garg)
                    gal = new GargishGalleon(Direction.North);
                else
                    gal = new TokunoGalleon(Direction.North);

                MerchantCaptain captain = new MerchantCaptain(gal);

                for(int i = 0; i < 25; i++)
                {
                    int x = Utility.Random(rec.X, rec.Width);
                    int y = Utility.Random(rec.Y, rec.Height);
                    p = new Point3D(x, y, -5);

                    if (gal.CanFit(p, map, gal.ItemID))
                    {
                        spawned = true;
                        break;
                    }
                }

                if (!spawned)
                {
                    gal.Delete();
                    captain.Delete();
                    return;
                }

                gal.Owner = captain;
                captain.Zone = zone;
                gal.MoveToWorld(p, map);
                gal.AutoAddCannons(captain);
                captain.MoveToWorld(new Point3D(gal.X, gal.Y - 1, gal.ZSurface), map);

                int crewCount = Utility.RandomMinMax(3, 5);

                for (int i = 0; i < crewCount; i++)
                {
                    Mobile crew = new MerchantCrew();
                    captain.AddToCrew(crew);
                    crew.MoveToWorld(new Point3D(gal.X + Utility.RandomList(-1, 1), gal.Y + Utility.RandomList(-1, 0, 1), gal.ZSurface), map);
                }

                Point2D[] course = def.GetRandomWaypoints();
                gal.BoatCourse = new BoatCourse(gal, new List<Point2D>(def.GetRandomWaypoints()));

                gal.NextNavPoint = 0;
                gal.StartCourse(false, false);

                FillHold(gal);

                m_ActiveZones[zone].Add(captain);
            }
        }

        public static void ResetNavPoints(BaseBoat boat)
        {
            boat.NextNavPoint = 0;
            boat.StartCourse(false, false);
        }

        public static void FillHold(BaseGalleon galleon)
        {
            if (galleon == null)
                return;

            Container hold = galleon.GalleonHold;

            if (hold != null)
            {
                int cnt = Utility.RandomMinMax(7, 14);

                for (int i = 0; i < cnt; i++)
                {
                    Item item = RunicReforging.GenerateRandomItem(galleon);

                    /*if (item is BaseWeapon)
                        BaseRunicTool.ApplyAttributesTo((BaseWeapon)item, false, 0, Utility.RandomMinMax(1, 6), 10, 100);
                    else if (item is BaseArmor)
                        BaseRunicTool.ApplyAttributesTo((BaseArmor)item, false, 0, Utility.RandomMinMax(1, 6), 10, 100);
                    else if (item is BaseHat)
                        BaseRunicTool.ApplyAttributesTo((BaseHat)item, false, 0, Utility.RandomMinMax(1, 6), 10, 100);
                    else if (item is BaseJewel)
                        BaseRunicTool.ApplyAttributesTo((BaseJewel)item, false, 0, Utility.RandomMinMax(1, 6), 10, 100);
                    else
                        item.Delete();*/

                    if (item != null)
                        hold.DropItem(item);
                }

                hold.DropItem(new Swab());
                hold.DropItem(new Ramrod());
                hold.DropItem(new Matches(Utility.RandomMinMax(25, 30)));
                hold.DropItem(new HeavyCannonball(Utility.RandomMinMax(7, 10)));
                hold.DropItem(new LightCannonball(Utility.RandomMinMax(7, 10)));
                hold.DropItem(new HeavyGrapeshot(Utility.RandomMinMax(7, 10)));
                hold.DropItem(new LightGrapeshot(Utility.RandomMinMax(7, 10)));
                hold.DropItem(new HeavyPowderCharge(Utility.RandomMinMax(7, 10)));
                hold.DropItem(new LightPowderCharge(Utility.RandomMinMax(7, 10)));
                hold.DropItem(new Fusecord(Utility.RandomMinMax(7, 10)));

                if (.10 >= Utility.RandomDouble())
                    hold.DropItem(new SmugglersCache());

                if (.10 >= Utility.RandomDouble())
                {
                    FishSteak steaks = new FishSteak(5);
                    switch (Utility.Random(5))
                    {
                        case 0:
                            steaks.Name = "Spiced Salmon";
                            steaks.Hue = 1759;
                            break;
                        case 1:
                            steaks.Name = "Dried Tuna";
                            steaks.Hue = 2108;
                            break;
                        case 2:
                            steaks.Name = "Salted Snapper";
                            steaks.Hue = 1864;
                            break;
                        case 3:
                            steaks.Name = "Salted Herring";
                            steaks.Hue = 2302;
                            break;
                        case 4:
                            steaks.Name = "Spiced Salmon";
                            steaks.Hue = 1637;
                            break;
                    }

                    hold.DropItem(steaks);
                }

                /*if (0.10 > Utility.RandomDouble())
                {
                    Item item = null;
                    switch (Utility.Random(6))
                    {
                        case 0: item = new LightScatterShot(); break;
                        case 1: item = new HeavyScatterShot(); break;
                        case 2: item = new LightFragShot(); break;
                        case 3: item = new HeavyFragShot(); break;
                        case 4: item = new LightHotShot(); break;
                        case 5: item = new HeavyHotShot(); break;
                    }

                    if (item != null)
                    {
                        item.Amount = Utility.RandomMinMax(2, 10);
                        hold.DropItem(item);
                    }
                }*/

                hold.DropItem(new Gold(Utility.RandomMinMax(5000, 25000)));

                if (0.50 > Utility.RandomDouble())
                {
                    switch (Utility.Random(4))
                    {
                        case 0:
                        case 1:
                        case 2: hold.DropItem(new IronWire(Utility.RandomMinMax(1, 5))); break;
                        case 3:
                        case 4:
                        case 5: hold.DropItem(new CopperWire(Utility.RandomMinMax(1, 5))); break;
                        case 6:
                        case 7: hold.DropItem(new SilverWire(Utility.RandomMinMax(1, 5))); break;
                        case 8: hold.DropItem(new GoldWire(Utility.RandomMinMax(1, 5))); break;
                    }
                }

                switch (Utility.Random(8))
                {
                    case 0:
                        if (Utility.RandomBool())
                            hold.DropItem(new IronOre(Utility.RandomMinMax(40, 50)));
                        else
                            hold.DropItem(new IronIngot(Utility.RandomMinMax(40, 50)));
                        break;
                    case 1:
                        if (Utility.RandomBool())
                            hold.DropItem(new DullCopperOre(Utility.RandomMinMax(40, 50)));
                        else
                            hold.DropItem(new DullCopperIngot(Utility.RandomMinMax(40, 50)));
                        break;
                    case 2:
                        if (Utility.RandomBool())
                            hold.DropItem(new ShadowIronOre(Utility.RandomMinMax(40, 50)));
                        else
                            hold.DropItem(new ShadowIronIngot(Utility.RandomMinMax(40, 50)));
                        break;
                    case 3:
                        if (Utility.RandomBool())
                            hold.DropItem(new CopperOre(Utility.RandomMinMax(40, 50)));
                        else
                            hold.DropItem(new CopperIngot(Utility.RandomMinMax(40, 50)));
                        break;
                    case 4:
                        if (Utility.RandomBool())
                            hold.DropItem(new BronzeOre(Utility.RandomMinMax(40, 50)));
                        else
                            hold.DropItem(new BronzeIngot(Utility.RandomMinMax(40, 50)));
                        break;
                    case 5:
                        if (Utility.RandomBool())
                            hold.DropItem(new AgapiteOre(Utility.RandomMinMax(40, 50)));
                        else
                            hold.DropItem(new AgapiteIngot(Utility.RandomMinMax(40, 50)));
                        break;
                    case 6:
                        if (Utility.RandomBool())
                            hold.DropItem(new VeriteOre(Utility.RandomMinMax(40, 50)));
                        else
                            hold.DropItem(new VeriteIngot(Utility.RandomMinMax(40, 50)));
                        break;
                    case 7:
                        if (Utility.RandomBool())
                            hold.DropItem(new ValoriteOre(Utility.RandomMinMax(40, 50)));
                        else
                            hold.DropItem(new ValoriteIngot(Utility.RandomMinMax(40, 50)));
                        break;
                }

                switch (Utility.Random(5))
                {
                    case 0:
                        if (Utility.RandomBool())
                            hold.DropItem(new Board(Utility.RandomMinMax(40, 50)));
                        else
                            hold.DropItem(new Log(Utility.RandomMinMax(40, 50)));
                        break;
                    case 1:
                        if (Utility.RandomBool())
                            hold.DropItem(new OakBoard(Utility.RandomMinMax(40, 50)));
                        else
                            hold.DropItem(new OakLog(Utility.RandomMinMax(40, 50)));
                        break;
                    case 2:
                        if (Utility.RandomBool())
                            hold.DropItem(new AshBoard(Utility.RandomMinMax(40, 50)));
                        else
                            hold.DropItem(new AshLog(Utility.RandomMinMax(40, 50)));
                        break;
                    case 3:
                        if (Utility.RandomBool())
                            hold.DropItem(new YewBoard(Utility.RandomMinMax(40, 50)));
                        else
                            hold.DropItem(new YewLog(Utility.RandomMinMax(40, 50)));
                        break;
                    case 4:
                        if (Utility.RandomBool())
                            hold.DropItem(new BloodwoodBoard(Utility.RandomMinMax(40, 50)));
                        else
                            hold.DropItem(new BloodwoodLog(Utility.RandomMinMax(40, 50)));
                        break;
                }

                switch (Utility.Random(4))
                {
                    case 0:
                        if (Utility.RandomBool())
                            hold.DropItem(new Leather(Utility.RandomMinMax(40, 50)));
                        else
                            hold.DropItem(new Hides(Utility.RandomMinMax(40, 50)));
                        break;
                    case 1:
                        if (Utility.RandomBool())
                            hold.DropItem(new SpinedLeather(Utility.RandomMinMax(40, 50)));
                        else
                            hold.DropItem(new SpinedHides(Utility.RandomMinMax(40, 50)));
                        break;
                    case 2:
                        if (Utility.RandomBool())
                            hold.DropItem(new HornedLeather(Utility.RandomMinMax(40, 50)));
                        else
                            hold.DropItem(new HornedHides(Utility.RandomMinMax(40, 50)));
                        break;
                    case 3:
                        if (Utility.RandomBool())
                            hold.DropItem(new BarbedLeather(Utility.RandomMinMax(40, 50)));
                        else
                            hold.DropItem(new BarbedHides(Utility.RandomMinMax(40, 50)));
                        break;
                }

                switch (Utility.Random(4))
                {
                    case 0: hold.DropItem(new HeavyCannonball(Utility.RandomMinMax(5, 10))); break;
                    case 1: hold.DropItem(new LightCannonball(Utility.RandomMinMax(5, 10))); break;
                    case 2: hold.DropItem(new HeavyGrapeshot(Utility.RandomMinMax(5, 10))); break;
                    case 3: hold.DropItem(new LightGrapeshot(Utility.RandomMinMax(5, 10))); break;
                }


                //Rares
                if (0.8 > Utility.RandomDouble())
                {
                    if (Utility.RandomBool())
                        hold.DropItem(new HeavyShipCannonDeed());
                    else
                        hold.DropItem(new LightShipCannonDeed());
                }

                if (0.025 > Utility.RandomDouble())
                {
                    if(Utility.RandomBool())
                        hold.DropItem(new WhiteClothDyeTub());
                    else
                        hold.DropItem(PermanentBoatPaint.DropRandom());
                }

                RefinementComponent.Roll(hold, 3, 0.25);
            }
        }

        public bool IsObjective(Mobile from)
        {
            return m_Bounties.ContainsKey(from);
        }

        public BountyQuestSpawner(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_MaxFel);
            writer.Write(m_MaxTram);
            writer.Write(m_MaxTokuno);
            writer.Write(m_SpawnTime);
            writer.Write(m_Active);

            writer.Write(m_ActiveZones.Count);
            foreach (KeyValuePair<SpawnZone, List<BaseShipCaptain>> kvp in m_ActiveZones)
            {
                writer.Write((int)kvp.Key);
                writer.Write(kvp.Value.Count);
                foreach (BaseShipCaptain capt in kvp.Value)
                    writer.Write(capt as Mobile);
            }

            writer.Write(m_Bounties.Count);
            foreach (KeyValuePair<Mobile, int> kvp in m_Bounties)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_MaxFel = reader.ReadInt();
            m_MaxTram = reader.ReadInt();
            m_MaxTokuno = reader.ReadInt();
            m_SpawnTime = reader.ReadTimeSpan();
            m_Active = reader.ReadBool();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                SpawnZone zone = (SpawnZone)reader.ReadInt();
                int c = reader.ReadInt();
                for (int j = 0; j < c; j++)
                {
                    BaseShipCaptain capt = reader.ReadMobile() as BaseShipCaptain;
                    if (capt != null && !capt.Deleted && capt.Alive)
                    {
                        m_ActiveZones[zone].Add(capt);
                    }
                }
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Mobile mob = reader.ReadMobile();
                int amt = reader.ReadInt();

                if (mob != null && !mob.Deleted)
                    m_Bounties.Add(mob, amt);
            }

            m_Instance = this;

            if (m_Active)
                m_Timer = Timer.DelayCall(m_SpawnTime, m_SpawnTime, new TimerCallback(OnTick));
        }

        public override void OnDoubleClick(Mobile from)
        {
            if(from.AccessLevel > AccessLevel.Player)
                from.SendGump(new PropertiesGump(from, this));
        }

        #region Command
        public static void GetRoutes_OnCommand(CommandEventArgs e)
        {
            MapItem mapitem;

            for (int i = 0; i < SpawnDefinition.PirateTramFelCoursesJhelom.Length; i++)
            {
                mapitem = new MapItem();
                mapitem.SetDisplay(5, 5, 5120 - 32, 4096 - 10, 400, 400);

                for (int j = 0; j < SpawnDefinition.PirateTramFelCoursesJhelom[i].Length; j++)
                {
                    Point2D pnt = SpawnDefinition.PirateTramFelCoursesJhelom[i][j];
                    mapitem.AddWorldPin(pnt.X, pnt.Y);
                }

                mapitem.Name = String.Format("Pirate - Jhelom {0}", i + 1);
                e.Mobile.AddToBackpack(mapitem);
            }

            for (int i = 0; i < SpawnDefinition.PirateTramFelCoursesMoonglow.Length; i++)
            {
                mapitem = new MapItem();
                mapitem.SetDisplay(5, 5, 5120 - 32, 4096 - 10, 400, 400);

                for (int j = 0; j < SpawnDefinition.PirateTramFelCoursesJhelom[i].Length; j++)
                {
                    Point2D pnt = SpawnDefinition.PirateTramFelCoursesJhelom[i][j];
                    mapitem.AddWorldPin(pnt.X, pnt.Y);
                }

                mapitem.Name = String.Format("Pirate - Moonglow {0}", i + 1);
                e.Mobile.AddToBackpack(mapitem);
            }

            for (int i = 0; i < SpawnDefinition.PirateTokunoCourses.Length; i++)
            {
                mapitem = new MapItem();
                mapitem.SetDisplay(5, 5, 1448 - 32, 1448 - 10, 400, 400);

                for (int j = 0; j < SpawnDefinition.PirateTokunoCourses[i].Length; j++)
                {
                    Point2D pnt = SpawnDefinition.PirateTokunoCourses[i][j];
                    mapitem.AddWorldPin(pnt.X, pnt.Y);
                }

                mapitem.Name = String.Format("Pirate - tokuno {0}", i + 1);
                e.Mobile.AddToBackpack(mapitem);
            }

            for (int i = 0; i < SpawnDefinition.MerchantTokunoCourses.Length; i++)
            {
                mapitem = new MapItem();
                mapitem.SetDisplay(5, 5, 1448 - 32, 1448 - 10, 400, 400);

                for (int j = 0; j < SpawnDefinition.PirateTokunoCourses[i].Length; j++)
                {
                    Point2D pnt = SpawnDefinition.PirateTokunoCourses[i][j];
                    mapitem.AddWorldPin(pnt.X, pnt.Y);
                }

                mapitem.Name = String.Format("Merchant - tokuno {0}", i + 1);
                e.Mobile.AddToBackpack(mapitem);
            }
            for (int i = 0; i < SpawnDefinition.MerchantTramFelCourses1.Length; i++)
            {
                mapitem = new MapItem();
                mapitem.SetDisplay(5, 5, 5120 - 32, 4096 - 10, 400, 400);

                for (int j = 0; j < SpawnDefinition.MerchantTramFelCourses1[i].Length; j++)
                {
                    Point2D pnt = SpawnDefinition.MerchantTramFelCourses1[i][j];
                    mapitem.AddWorldPin(pnt.X, pnt.Y);
                }

                mapitem.Name = String.Format("Merchant - tram/fel(a) {0}", i + 1);
                e.Mobile.AddToBackpack(mapitem);
            }
            for (int i = 0; i < SpawnDefinition.MerchantTramFelCourses2.Length; i++)
            {
                mapitem = new MapItem();
                mapitem.SetDisplay(5, 5, 5120 - 32, 4096 - 10, 400, 400);

                for (int j = 0; j < SpawnDefinition.MerchantTramFelCourses2[i].Length; j++)
                {
                    Point2D pnt = SpawnDefinition.MerchantTramFelCourses2[i][j];
                    mapitem.AddWorldPin(pnt.X, pnt.Y);
                }

                mapitem.Name = String.Format("Merchant - tram/fel(b) {0}", i + 1);
                e.Mobile.AddToBackpack(mapitem);
            }

        }
        #endregion
    }

    public enum SpawnZone
    {
        //Pirate
        TramJhelom,
        FelJhelom,
        TramMoonglow,
        FelMoonglow,
        TokunoPirate,

        //merchants
        TramMerch1, 
        TramMerch2,
        FelMerch1,
        FelMerch2,
        TokunoMerch
    }

    public class SpawnDefinition
    {
        private Rectangle2D m_SpawnRegion;
        private Point2D[][] m_Waypoints;
        private SpawnZone m_Zone;
        private Map m_Map;

        public Rectangle2D SpawnRegion { get { return m_SpawnRegion; } }
        public Point2D[][] Waypoints { get { return m_Waypoints; } }
        public SpawnZone Zone { get { return m_Zone; } }
        public Map Map { get { return m_Map;}}

        public SpawnDefinition(Rectangle2D spawnreg, Point2D[][] waypoints, SpawnZone type, Map map)
        {
            m_SpawnRegion = spawnreg;
            m_Waypoints = waypoints;
            m_Zone = type;
            m_Map = map;

            BountyQuestSpawner.AddZone(this);
        }

        public Point2D[] GetRandomWaypoints()
        {
            return m_Waypoints[Utility.Random(m_Waypoints.Length)];
        }

        //Defines teh definitions!
        public static void Configure()
        {
            new SpawnDefinition(new Rectangle2D(1500, 3600, 180, 400), m_PirateTramFelCoursesJhelom,   SpawnZone.TramJhelom,   Map.Trammel);
            new SpawnDefinition(new Rectangle2D(1500, 3600, 180, 400), m_PirateTramFelCoursesJhelom,   SpawnZone.FelJhelom,    Map.Felucca);
            new SpawnDefinition(new Rectangle2D(4570, 630, 400, 100),  m_PirateTramFelCoursesMoonglow, SpawnZone.TramMoonglow, Map.Trammel);
            new SpawnDefinition(new Rectangle2D(4570, 630, 400, 100),  m_PirateTramFelCoursesMoonglow, SpawnZone.FelMoonglow,  Map.Felucca);
            new SpawnDefinition(new Rectangle2D(1022, 1182, 350, 200), m_PirateTokunoCourses,          SpawnZone.TokunoPirate, Map.Tokuno);

            new SpawnDefinition(new Rectangle2D(1780, 1650, 300, 200), m_MerchantTramFelCourses1, SpawnZone.TramMerch1, Map.Trammel);
            new SpawnDefinition(new Rectangle2D(1780, 1650, 300, 200), m_MerchantTramFelCourses1, SpawnZone.FelMerch1, Map.Felucca);
            new SpawnDefinition(new Rectangle2D(3780, 2300, 100, 200), m_MerchantTramFelCourses2, SpawnZone.TramMerch2, Map.Trammel);
            new SpawnDefinition(new Rectangle2D(3780, 2300, 100, 200), m_MerchantTramFelCourses2, SpawnZone.FelMerch2, Map.Felucca);
            new SpawnDefinition(new Rectangle2D(425, 1335, 160, 80),   m_MerchantTokunoCourses, SpawnZone.TokunoMerch, Map.Tokuno);
        }

        public static Point2D[][] PirateTramFelCoursesJhelom { get { return m_PirateTramFelCoursesJhelom; } }
        private static Point2D[][] m_PirateTramFelCoursesJhelom = new Point2D[][]
        {
	        new Point2D[]{ new Point2D(1598, 3861), new Point2D(1520, 3470), new Point2D(1418, 3314), new Point2D(1159, 3277), new Point2D(1320, 3508), new Point2D(1527, 3584) },
	        new Point2D[]{ new Point2D(1947, 3536), new Point2D(2023, 4016), new Point2D(1795, 3855), new Point2D(1613, 3887) },
	        new Point2D[]{ new Point2D(2135, 4070), new Point2D(2802, 4070), new Point2D(2620, 3761), new Point2D(1725, 3794), },
	        new Point2D[]{ new Point2D(2154, 3775), new Point2D(2378, 3652), new Point2D(2388, 3812), new Point2D(1696, 3797), },
	        new Point2D[]{ new Point2D(1599, 3933), new Point2D(1299, 3953), new Point2D(971, 3799), new Point2D(813, 3326), new Point2D(1247, 3296), new Point2D(1655, 3890) },
	        new Point2D[]{ new Point2D(1694, 3735), new Point2D(1960, 3678), new Point2D(1808, 3966), },
	        new Point2D[]{ new Point2D(1799, 4070), new Point2D(1810, 3727), new Point2D(2150, 3727), new Point2D(1691, 3916), },
        };

        public static Point2D[][] PirateTramFelCoursesMoonglow { get { return m_PirateTramFelCoursesMoonglow; } }
        private static Point2D[][] m_PirateTramFelCoursesMoonglow = new Point2D[][]
         {
            new Point2D[]{ new Point2D(4415, 792), new Point2D(3927, 900), new Point2D(4069, 1630), new Point2D(4799, 1670), new Point2D(4861, 1061), new Point2D(4533, 589) },
	        new Point2D[]{ new Point2D(4265, 145), new Point2D(5015, 153), new Point2D(5001, 669), new Point2D(4950, 720), new Point2D(4573, 663) },
	        new Point2D[]{ new Point2D(4265, 145), new Point2D(3709, 107), new Point2D(3703, 773), new Point2D(4131, 913), new Point2D(4533, 713) },
	        new Point2D[]{ new Point2D(5043, 155), new Point2D(4447, 231), new Point2D(4531, 609) },
        };

        public static Point2D[][] PirateTokunoCourses { get { return m_PirateTokunoCourses; } }
        private static Point2D[][] m_PirateTokunoCourses = new Point2D[][]
        {
	        new Point2D[]{ new Point2D(1324, 1178), new Point2D(1358, 1334), new Point2D(1032, 1358), new Point2D(1070, 1240) },
	        new Point2D[]{ new Point2D(1370, 1074), new Point2D(1422, 962), new Point2D(1416, 620), new Point2D(1422, 1310) },
	        new Point2D[]{ new Point2D(1032, 1104), new Point2D(982, 1078), new Point2D(942, 914), new Point2D(942, 1086), new Point2D(982, 1078), new Point2D(1134, 1202) },
	        new Point2D[]{ new Point2D(1320, 1378), new Point2D(1050, 1204), new Point2D(1356, 1088), new Point2D(1244, 1300) },
        };

        public static Point2D[][] MerchantTokunoCourses { get { return m_MerchantTokunoCourses; } }
        private static Point2D[][] m_MerchantTokunoCourses = new Point2D[][]
        {
	        new Point2D[]{ new Point2D(460, 1408), new Point2D(878, 1408), new Point2D(500, 1408) },
	        new Point2D[]{ new Point2D(460, 1408), new Point2D(460, 768), new Point2D(460, 1350) },
        };

        public static Point2D[][] MerchantTramFelCourses1 { get { return m_MerchantTramFelCourses1; } }
        private static Point2D[][] m_MerchantTramFelCourses1 = new Point2D[][]
        {
	        new Point2D[]{ new Point2D(2420, 1708), new Point2D(2420, 1400), new Point2D(3516, 1400), new Point2D(3516, 1696), new Point2D(2500, 1696) },
	        new Point2D[]{ new Point2D(2420, 1708), new Point2D(2420, 2760), new Point2D(3164, 2760), new Point2D(3164, 1696), new Point2D(2500, 1696) },
        };

        public static Point2D[][] MerchantTramFelCourses2 { get { return m_MerchantTramFelCourses2; } }
        private static Point2D[][] m_MerchantTramFelCourses2 = new Point2D[][]
        {
	        new Point2D[]{ new Point2D(4129, 2367), new Point2D(4129, 1891), new Point2D(4773, 1891), new Point2D(4773, 2639), new Point2D(4129, 2639), new Point2D(4129, 2351) },
	        new Point2D[]{ new Point2D(4000, 2415), new Point2D(4000, 3515), new Point2D(3141, 3515), new Point2D(3141, 3043), new Point2D(4093, 3043), new Point2D(4093, 2371) },
        };
    }
}