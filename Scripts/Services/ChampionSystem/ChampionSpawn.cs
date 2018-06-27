using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
using System.Linq;
using Server.Spells.Necromancy;

namespace Server.Engines.CannedEvil
{
    public class ChampionSpawn : Item
    {
        public static readonly int MaxStrayDistance = 250;

        private bool m_Active;
        private bool m_RandomizeType;
        private ChampionSpawnType m_Type;
        private List<Mobile> m_Creatures;
        private List<Item> m_RedSkulls;
        private List<Item> m_WhiteSkulls;
        private ChampionPlatform m_Platform;
        private ChampionAltar m_Altar;
        private int m_Kills;
        private Mobile m_Champion;

        //private int m_SpawnRange;
        private Rectangle2D m_SpawnArea;
        private ChampionSpawnRegion m_Region;

        private TimeSpan m_ExpireDelay;
        private DateTime m_ExpireTime;

        private TimeSpan m_RestartDelay;
        private DateTime m_RestartTime;

        private Timer m_Timer, m_RestartTimer;

        private IdolOfTheChampion m_Idol;

        private bool m_HasBeenAdvanced;
        private bool m_ConfinedRoaming;

        private Dictionary<Mobile, int> m_DamageEntries;

		[CommandProperty(AccessLevel.GameMaster)]
		public string GroupName { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public double SpawnMod { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int SpawnRadius { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public double KillsMod { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool AutoRestart { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string SpawnName { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
        public bool ConfinedRoaming
        {
            get
            {
                return m_ConfinedRoaming;
            }
            set
            {
                m_ConfinedRoaming = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasBeenAdvanced
        {
            get
            {
                return m_HasBeenAdvanced;
            }
            set
            {
                m_HasBeenAdvanced = value;
            }
        }

        [Constructable]
        public ChampionSpawn()
            : base(0xBD2)
        {
            Movable = false;
            Visible = false;

            m_Creatures = new List<Mobile>();
            m_RedSkulls = new List<Item>();
            m_WhiteSkulls = new List<Item>();

            m_Platform = new ChampionPlatform(this);
            m_Altar = new ChampionAltar(this);
            m_Idol = new IdolOfTheChampion(this);

            m_ExpireDelay = TimeSpan.FromMinutes(10.0);
            m_RestartDelay = TimeSpan.FromMinutes(10.0);

            m_DamageEntries = new Dictionary<Mobile, int>();
			m_RandomizeType = false;

            SpawnRadius = 35;
            SpawnMod = 1;

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(SetInitialSpawnArea));
        }

        public void SetInitialSpawnArea()
        {
            //Previous default used to be 24;
            SpawnArea = new Rectangle2D(new Point2D(X - SpawnRadius, Y - SpawnRadius),
				new Point2D(X + SpawnRadius, Y + SpawnRadius));
        }

        public void UpdateRegion()
        {
            if (m_Region != null)
                m_Region.Unregister();

            if (!Deleted && Map != Map.Internal)
            {
                m_Region = new ChampionSpawnRegion(this);
                m_Region.Register();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RandomizeType
        {
            get
            {
                return m_RandomizeType;
            }
            set
            {
                m_RandomizeType = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Kills
        {
            get
            {
                return m_Kills;
            }
            set
            {
                m_Kills = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D SpawnArea
        {
            get
            {
                return m_SpawnArea;
            }
            set
            {
                m_SpawnArea = value;
                InvalidateProperties();
                UpdateRegion();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan RestartDelay
        {
            get
            {
                return m_RestartDelay;
            }
            set
            {
                m_RestartDelay = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime RestartTime
        {
            get
            {
                return m_RestartTime;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan ExpireDelay
        {
            get
            {
                return m_ExpireDelay;
            }
            set
            {
                m_ExpireDelay = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ExpireTime
        {
            get
            {
                return m_ExpireTime;
            }
            set
            {
                m_ExpireTime = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ChampionSpawnType Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get
            {
                return m_Active;
            }
            set
            {
                if (value)
                    Start();
                else
                    Stop();

                PrimevalLichPuzzle.Update(this);
				
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Champion
        {
            get
            {
                return m_Champion;
            }
            set
            {
                m_Champion = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Level
        {
            get
            {
                return m_RedSkulls.Count;
            }
            set
            {
                for (int i = m_RedSkulls.Count - 1; i >= value; --i)
                {
                    m_RedSkulls[i].Delete();
                    m_RedSkulls.RemoveAt(i);
                }

                for (int i = m_RedSkulls.Count; i < value; ++i)
                {
                    Item skull = new Item(0x1854);

                    skull.Hue = 0x26;
                    skull.Movable = false;
                    skull.Light = LightType.Circle150;

                    skull.MoveToWorld(GetRedSkullLocation(i), Map);

                    m_RedSkulls.Add(skull);
                }

                InvalidateProperties();
            }
        }

        private void RemoveSkulls()
        {
            if (m_WhiteSkulls != null)
            {
                for (int i = 0; i < m_WhiteSkulls.Count; ++i)
                    m_WhiteSkulls[i].Delete();

                m_WhiteSkulls.Clear();
            }

            if (m_RedSkulls != null)
            {
                for (int i = 0; i < m_RedSkulls.Count; i++)
                    m_RedSkulls[i].Delete();

                m_RedSkulls.Clear();
            }
        }

        public int MaxKills
        {
            get
            {
				int l = Level;
                return ChampionSystem.MaxKillsForLevel(l);
            }
        }

        public bool IsChampionSpawn(Mobile m)
        {
            return m_Creatures.Contains(m);
        }

        public void SetWhiteSkullCount(int val)
        {
            for (int i = m_WhiteSkulls.Count - 1; i >= val; --i)
            {
                m_WhiteSkulls[i].Delete();
                m_WhiteSkulls.RemoveAt(i);
            }

            for (int i = m_WhiteSkulls.Count; i < val; ++i)
            {
                Item skull = new Item(0x1854);

                skull.Movable = false;
                skull.Light = LightType.Circle150;

                skull.MoveToWorld(GetWhiteSkullLocation(i), Map);

                m_WhiteSkulls.Add(skull);

                Effects.PlaySound(skull.Location, skull.Map, 0x29);
                Effects.SendLocationEffect(new Point3D(skull.X + 1, skull.Y + 1, skull.Z), skull.Map, 0x3728, 10);
            }
        }

        public void Start(bool serverLoad = false)
        {
            if (m_Active || Deleted)
                return;

            m_Active = true;
            m_HasBeenAdvanced = false;

            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = new SliceTimer(this);
            m_Timer.Start();

            if (m_RestartTimer != null)
                m_RestartTimer.Stop();

            m_RestartTimer = null;

            if (m_Altar != null)
                m_Altar.Hue = 0;

            PrimevalLichPuzzle.Update(this);

            if (!serverLoad)
            {
                double chance = Utility.RandomDouble();

                if (chance < 0.1)
                    Level = 4;
                else if (chance < 0.25)
                    Level = 3;
                else if (chance < 0.5)
                    Level = 2;
                else if (Utility.RandomBool())
                    Level = 1;
                else
                    Level = 0;

                if (Level > 0)
                    AdvanceLevel();
            }
        }

        public void Stop()
        {
            if (!m_Active || Deleted)
                return;

            m_Active = false;
            m_HasBeenAdvanced = false;

            // We must despawn all the creatures.
            if (m_Creatures != null)
            {
                for (int i = 0; i < m_Creatures.Count; ++i)
                    m_Creatures[i].Delete();

                m_Creatures.Clear();
            }

            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;

            if (m_RestartTimer != null)
                m_RestartTimer.Stop();

            m_RestartTimer = null;

            if (m_Altar != null)
                m_Altar.Hue = 0x455;

            PrimevalLichPuzzle.Update(this);

            RemoveSkulls();
            m_Kills = 0;
        }

        public void BeginRestart(TimeSpan ts)
        {
            if (m_RestartTimer != null)
                m_RestartTimer.Stop();

            m_RestartTime = DateTime.UtcNow + ts;

            m_RestartTimer = new RestartTimer(this, ts);
            m_RestartTimer.Start();
        }

        public void EndRestart()
        {
            if (RandomizeType)
            {
                switch (Utility.Random(5))
                {
                    case 0:
                        Type = ChampionSpawnType.Abyss; break;
                    case 1:
                        Type = ChampionSpawnType.Arachnid; break;
                    case 2:
                        Type = ChampionSpawnType.ColdBlood; break;
                    case 3:
                        Type = ChampionSpawnType.VerminHorde; break;
                    case 4:
                        Type = ChampionSpawnType.UnholyTerror; break;
                }
            }

            m_HasBeenAdvanced = false;

            Start();
        }

        #region Scroll of Transcendence
        private ScrollofTranscendence CreateRandomSoT(bool felucca)
        {
            int level = Utility.RandomMinMax(1, 5);
			
            if (felucca)
                level += 5;

            return ScrollofTranscendence.CreateRandom(level, level);
        }

        #endregion

        public static void GiveScrollTo(Mobile killer, SpecialScroll scroll)
        {
            if (scroll == null || killer == null)	//sanity
                return;

            if (scroll is ScrollofTranscendence)
                killer.SendLocalizedMessage(1094936); // You have received a Scroll of Transcendence!
            else
                killer.SendLocalizedMessage(1049524); // You have received a scroll of power!
			
            if (killer.Alive)
                killer.AddToBackpack(scroll);
            else
            {
                if (killer.Corpse != null && !killer.Corpse.Deleted)
                    killer.Corpse.DropItem(scroll);
                else
                    killer.AddToBackpack(scroll);
            }
			
            // Justice reward
            PlayerMobile pm = (PlayerMobile)killer;
            for (int j = 0; j < pm.JusticeProtectors.Count; ++j)
            {
                Mobile prot = (Mobile)pm.JusticeProtectors[j];
				
                if (prot.Map != killer.Map || prot.Murderer || prot.Criminal || !JusticeVirtue.CheckMapRegion(killer, prot))
                    continue;

                int chance = 0;

                switch ( VirtueHelper.GetLevel(prot, VirtueName.Justice) )
                {
                    case VirtueLevel.Seeker:
                        chance = 60;
                        break;
                    case VirtueLevel.Follower:
                        chance = 80;
                        break;
                    case VirtueLevel.Knight:
                        chance = 100;
                        break;
                }

                if (chance > Utility.Random(100))
                {
                    try
                    {
                        prot.SendLocalizedMessage(1049368); // You have been rewarded for your dedication to Justice!

                        SpecialScroll scrollDupe = Activator.CreateInstance(scroll.GetType()) as SpecialScroll;
					
                        if (scrollDupe != null)
                        {
                            scrollDupe.Skill = scroll.Skill;
                            scrollDupe.Value = scroll.Value;
                            prot.AddToBackpack(scrollDupe);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private DateTime _NextGhostCheck;

        public void OnSlice()
        {
            if (!m_Active || Deleted)
                return;

			int currentRank = Rank;

            if (m_Champion != null)
            {
                if (m_Champion.Deleted)
                {
                    RegisterDamageTo(m_Champion);

                    if (m_Champion is BaseChampion)
                        AwardArtifact(((BaseChampion)m_Champion).GetArtifact());

                    m_DamageEntries.Clear();

                    if (m_Altar != null)
                    {
                        m_Altar.Hue = 0x455;

                        if (!Core.ML || Map == Map.Felucca)
                        {
                            new StarRoomGate(true, m_Altar.Location, m_Altar.Map);
                        }
                    }

                    m_Champion = null;
                    Stop();

					if(AutoRestart)
						BeginRestart(m_RestartDelay);
                }
                else if (m_Champion.Alive && m_Champion.GetDistanceToSqrt(this) > MaxStrayDistance)
                {
                    m_Champion.MoveToWorld(new Point3D(X, Y, Z - 15), Map);
                }
            }
            else
            {
                int kills = m_Kills;

                for (int i = 0; i < m_Creatures.Count; ++i)
                {
                    Mobile m = m_Creatures[i];

                    if (m.Deleted)
                    {
                        if (m.Corpse != null && !m.Corpse.Deleted)
                        {
                            ((Corpse)m.Corpse).BeginDecay(TimeSpan.FromMinutes(1));
                        }
                        m_Creatures.RemoveAt(i);
                        --i;

						int rankOfMob = GetRankFor(m);
						if(rankOfMob == currentRank)
							++m_Kills;

                        Mobile killer = m.FindMostRecentDamager(false);

                        RegisterDamageTo(m);

                        if (killer is BaseCreature)
                            killer = ((BaseCreature)killer).GetMaster();

                        if (killer is PlayerMobile)
                        {
                            #region Scroll of Transcendence
                            if (Core.ML)
                            {
                                if (Map == Map.Felucca)
                                {
                                    if (Utility.RandomDouble() < ChampionSystem.ScrollChance)
                                    {
                                        PlayerMobile pm = (PlayerMobile)killer;

                                        if (Utility.RandomDouble() < ChampionSystem.TranscendenceChance)
                                        {
                                            ScrollofTranscendence SoTF = CreateRandomSoT(true);
                                            GiveScrollTo(pm, (SpecialScroll)SoTF);
                                        }
                                        else
                                        {
                                            PowerScroll PS = PowerScroll.CreateRandomNoCraft(5, 5);
                                            GiveScrollTo(pm, (SpecialScroll)PS);
                                        }
                                    }
                                }

                                if (Map == Map.Ilshenar || Map == Map.Tokuno || Map == Map.Malas)
                                {
                                    if (Utility.RandomDouble() < 0.0015)
                                    {
                                        killer.SendLocalizedMessage(1094936); // You have received a Scroll of Transcendence!
                                        ScrollofTranscendence SoTT = CreateRandomSoT(false);
                                        killer.AddToBackpack(SoTT);
                                    }
                                }
                            }
							#endregion

							int mobSubLevel = rankOfMob + 1;
                            if (mobSubLevel >= 0)
                            {
                                bool gainedPath = false;

                                int pointsToGain = mobSubLevel * 40;

                                if (VirtueHelper.Award(killer, VirtueName.Valor, pointsToGain, ref gainedPath))
                                {
                                    if (gainedPath)
                                        killer.SendLocalizedMessage(1054032); // You have gained a path in Valor!
                                    else
                                        killer.SendLocalizedMessage(1054030); // You have gained in Valor!
                                    //No delay on Valor gains
                                }

                                PlayerMobile.ChampionTitleInfo info = ((PlayerMobile)killer).ChampionTitles;

                                info.Award(m_Type, mobSubLevel);

                                Server.Engines.CityLoyalty.CityLoyaltySystem.OnSpawnCreatureKilled(m as BaseCreature, mobSubLevel);
                            }
                        }
                    }
                }

                // Only really needed once.
                if (m_Kills > kills)
                    InvalidateProperties();

                double n = m_Kills / (double)MaxKills;
                int p = (int)(n * 100);

                if (p >= 90)
                    AdvanceLevel();
                else if (p > 0)
                    SetWhiteSkullCount(p / 20);

                if (DateTime.UtcNow >= m_ExpireTime)
                    Expire();

                Respawn();
            }

            if (m_Timer != null && m_Timer.Running && _NextGhostCheck < DateTime.UtcNow)
            {
                foreach (var ghost in m_Region.GetEnumeratedMobiles().OfType<PlayerMobile>().Where(pm => !pm.Alive && (pm.Corpse == null || pm.Corpse.Deleted)))
                {
                    Map map = ghost.Map;
                    Point3D loc = ExorcismSpell.GetNearestShrine(ghost, ref map);

                    if (loc != Point3D.Zero)
                    {
                        ghost.MoveToWorld(loc, map);
                    }
                    else
                    {
                        ghost.MoveToWorld(new Point3D(989, 520, -50), Map.Malas);
                    }
                }

                _NextGhostCheck = DateTime.UtcNow + TimeSpan.FromMinutes(Utility.RandomMinMax(5, 8));
            }
        }

        public void AdvanceLevel()
        {
            m_ExpireTime = DateTime.UtcNow + m_ExpireDelay;

            if (Level < 16)
            {
                m_Kills = 0;
                ++Level;
                InvalidateProperties();
                SetWhiteSkullCount(0);

                if (m_Altar != null)
                {
                    Effects.PlaySound(m_Altar.Location, m_Altar.Map, 0x29);
                    Effects.SendLocationEffect(new Point3D(m_Altar.X + 1, m_Altar.Y + 1, m_Altar.Z), m_Altar.Map, 0x3728, 10);
                }
            }
            else
            {
                SpawnChampion();
            }
        }

        public void SpawnChampion()
        {
            m_Kills = 0;
            Level = 0;
            InvalidateProperties();
            SetWhiteSkullCount(0);

            try
            {
                m_Champion = Activator.CreateInstance(ChampionSpawnInfo.GetInfo(m_Type).Champion) as Mobile;
            }
            catch
            {
            }

            if (m_Champion != null)
            {
                Point3D p = new Point3D(X, Y, Z - 15);

                m_Champion.MoveToWorld(p, Map);
                ((BaseCreature)m_Champion).Home = p;
            }
        }

        public void Respawn()
        {
            if (!m_Active || Deleted || m_Champion != null)
                return;

			int currentLevel = Level;
			int currentRank = Rank;
			int maxSpawn = (int)((double)MaxKills * 0.5d * SpawnMod);
			if (currentLevel >= 16)
				maxSpawn = Math.Min(maxSpawn, MaxKills - m_Kills);
			if (maxSpawn < 3)
				maxSpawn = 3;

			int spawnRadius = (int)(SpawnRadius * ChampionSystem.SpawnRadiusModForLevel(Level));
			Rectangle2D spawnBounds = new Rectangle2D(new Point2D(X - spawnRadius, Y - spawnRadius),
				new Point2D(X + spawnRadius, Y + spawnRadius));

			int mobCount = 0;
			foreach(Mobile m in m_Creatures)
			{
				if (GetRankFor(m) == currentRank)
					++mobCount;
			}

			while (mobCount <= maxSpawn)
            {
                Mobile m = Spawn();

                if (m == null)
                    return;

                Point3D loc = GetSpawnLocation(spawnBounds, spawnRadius);

                // Allow creatures to turn into Paragons at Ilshenar champions.
                m.OnBeforeSpawn(loc, Map);

                m_Creatures.Add(m);
                m.MoveToWorld(loc, Map);
				++mobCount;

                if (m is BaseCreature)
                {
                    BaseCreature bc = m as BaseCreature;
                    bc.Tamable = false;
                    bc.IsChampionSpawn = true;

                    if (!m_ConfinedRoaming)
                    {
                        bc.Home = Location;
						bc.RangeHome = spawnRadius;
                    }
                    else
                    {
                        bc.Home = bc.Location;

                        Point2D xWall1 = new Point2D(spawnBounds.X, bc.Y);
                        Point2D xWall2 = new Point2D(spawnBounds.X + spawnBounds.Width, bc.Y);
                        Point2D yWall1 = new Point2D(bc.X, spawnBounds.Y);
                        Point2D yWall2 = new Point2D(bc.X, spawnBounds.Y + spawnBounds.Height);

                        double minXDist = Math.Min(bc.GetDistanceToSqrt(xWall1), bc.GetDistanceToSqrt(xWall2));
                        double minYDist = Math.Min(bc.GetDistanceToSqrt(yWall1), bc.GetDistanceToSqrt(yWall2));

                        bc.RangeHome = (int)Math.Min(minXDist, minYDist);
                    }
                }
            }
        }

		public Point3D GetSpawnLocation()
		{
			return GetSpawnLocation(m_SpawnArea, 24);
		}

        public Point3D GetSpawnLocation(Rectangle2D rect, int range)
        {
            Map map = Map;

            if (map == null)
                return Location;

			int cx = Location.X;
			int cy = Location.Y;

            // Try 20 times to find a spawnable location.
            for (int i = 0; i < 20; i++)
            {
				int dx = Utility.Random(range * 2);
				int dy = Utility.Random(range * 2);
				int x = rect.X + dx;
				int y = rect.Y + dy;

				// Make spawn area circular
				//if ((cx - x) * (cx - x) + (cy - y) * (cy - y) > range * range)
				//	continue;

                int z = Map.GetAverageZ(x, y);

                if (Map.CanSpawnMobile(new Point2D(x, y), z))
                    return new Point3D(x, y, z);

                /* try @ platform Z if map z fails */
                else if (Map.CanSpawnMobile(new Point2D(x, y), m_Platform.Location.Z))
                    return new Point3D(x, y, m_Platform.Location.Z);
            }

            return Location;
        }

        public int Rank
        {
			get
			{
				return ChampionSystem.RankForLevel(Level);
			}
        }

        public int GetRankFor(Mobile m)
        {
            Type[][] types = ChampionSpawnInfo.GetInfo(m_Type).SpawnTypes;
            Type t = m.GetType();

            for (int i = 0; i < types.GetLength(0); i++)
            {
                Type[] individualTypes = types[i];

                for (int j = 0; j < individualTypes.Length; j++)
                {
                    if (t == individualTypes[j])
                        return i;
                }
            }

            return -1;
        }

        public Mobile Spawn()
        {
            Type[][] types = ChampionSpawnInfo.GetInfo(m_Type).SpawnTypes;

			int v = Rank;

            if (v >= 0 && v < types.Length)
                return Spawn(types[v]);

            return null;
        }

        public Mobile Spawn(params Type[] types)
        {
            try
            {
                return Activator.CreateInstance(types[Utility.Random(types.Length)]) as Mobile;
            }
            catch
            {
                return null;
            }
        }

        public void Expire()
        {
            m_Kills = 0;

            if (m_WhiteSkulls.Count == 0)
            {
                // They didn't even get 20%, go back a level
                if (Level > 0)
                    --Level;

                InvalidateProperties();
            }
            else
            {
                SetWhiteSkullCount(0);
            }

            m_ExpireTime = DateTime.UtcNow + m_ExpireDelay;
        }

        public Point3D GetRedSkullLocation(int index)
        {
            int x, y;

            if (index < 5)
            {
                x = index - 2;
                y = -2;
            }
            else if (index < 9)
            {
                x = 2;
                y = index - 6;
            }
            else if (index < 13)
            {
                x = 10 - index;
                y = 2;
            }
            else
            {
                x = -2;
                y = 14 - index;
            }

            return new Point3D(X + x, Y + y, Z - 15);
        }

        public Point3D GetWhiteSkullLocation(int index)
        {
            int x, y;

            switch( index )
            {
                default:
                case 0:
                    x = -1;
                    y = -1;
                    break;
                case 1:
                    x = 1;
                    y = -1;
                    break;
                case 2:
                    x = 1;
                    y = 1;
                    break;
                case 3:
                    x = -1;
                    y = 1;
                    break;
            }

            return new Point3D(X + x, Y + y, Z - 15);
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add("champion spawn");
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Active)
            {
                list.Add(1060742); // active
                list.Add(1060658, "Type\t{0}", m_Type); // ~1_val~: ~2_val~
                list.Add(1060659, "Level\t{0}", Level); // ~1_val~: ~2_val~
                list.Add(1060660, "Kills\t{0} of {1} ({2:F1}%)", m_Kills, MaxKills, 100.0 * ((double)m_Kills / MaxKills)); // ~1_val~: ~2_val~
                //list.Add( 1060661, "Spawn Range\t{0}", m_SpawnRange ); // ~1_val~: ~2_val~
            }
            else
            {
                list.Add(1060743); // inactive
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            if (m_Active)
                LabelTo(from, "{0} (Active; Level: {1}; Kills: {2}/{3})", m_Type, Level, m_Kills, MaxKills);
            else
                LabelTo(from, "{0} (Inactive)", m_Type);
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendGump(new PropertiesGump(from, this));
        }

        public override void OnLocationChange(Point3D oldLoc)
        {
            if (Deleted)
                return;

            if (m_Platform != null)
                m_Platform.Location = new Point3D(X, Y, Z - 20);

            if (m_Altar != null)
                m_Altar.Location = new Point3D(X, Y, Z - 15);

            if (m_Idol != null)
                m_Idol.Location = new Point3D(X, Y, Z - 15);

            if (m_RedSkulls != null)
            {
                for (int i = 0; i < m_RedSkulls.Count; ++i)
                    m_RedSkulls[i].Location = GetRedSkullLocation(i);
            }

            if (m_WhiteSkulls != null)
            {
                for (int i = 0; i < m_WhiteSkulls.Count; ++i)
                    m_WhiteSkulls[i].Location = GetWhiteSkullLocation(i);
            }

            m_SpawnArea.X += Location.X - oldLoc.X;
            m_SpawnArea.Y += Location.Y - oldLoc.Y;

            UpdateRegion();
        }

        public override void OnMapChange()
        {
            if (Deleted)
                return;

            if (m_Platform != null)
                m_Platform.Map = Map;

            if (m_Altar != null)
                m_Altar.Map = Map;

            if (m_Idol != null)
                m_Idol.Map = Map;

            if (m_RedSkulls != null)
            {
                for (int i = 0; i < m_RedSkulls.Count; ++i)
                    m_RedSkulls[i].Map = Map;
            }

            if (m_WhiteSkulls != null)
            {
                for (int i = 0; i < m_WhiteSkulls.Count; ++i)
                    m_WhiteSkulls[i].Map = Map;
            }

            UpdateRegion();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Platform != null)
                m_Platform.Delete();

            if (m_Altar != null)
                m_Altar.Delete();

            if (m_Idol != null)
                m_Idol.Delete();

            RemoveSkulls();

            if (m_Creatures != null)
            {
                for (int i = 0; i < m_Creatures.Count; ++i)
                {
                    Mobile mob = m_Creatures[i];

                    if (!mob.Player)
                        mob.Delete();
                }

                m_Creatures.Clear();
            }

            if (m_Champion != null && !m_Champion.Player)
                m_Champion.Delete();

            Stop();

            UpdateRegion();
        }

        public ChampionSpawn(Serial serial)
            : base(serial)
        {
        }

        public virtual void RegisterDamageTo(Mobile m)
        {
            if (m == null)
                return;

            foreach (DamageEntry de in m.DamageEntries)
            {
                if (de.HasExpired)
                    continue;

                Mobile damager = de.Damager;

                Mobile master = damager.GetDamageMaster(m);

                if (master != null)
                    damager = master;

                RegisterDamage(damager, de.DamageGiven);
            }
        }

        public void RegisterDamage(Mobile from, int amount)
        {
            if (from == null || !from.Player)
                return;

            if (m_DamageEntries.ContainsKey(from))
                m_DamageEntries[from] += amount;
            else
                m_DamageEntries.Add(from, amount);
        }

        public void AwardArtifact(Item artifact)
        {
            if (artifact == null)
                return;

            int totalDamage = 0;

            Dictionary<Mobile, int> validEntries = new Dictionary<Mobile, int>();

            foreach (KeyValuePair<Mobile, int> kvp in m_DamageEntries)
            {
                if (IsEligible(kvp.Key, artifact))
                {
                    validEntries.Add(kvp.Key, kvp.Value);
                    totalDamage += kvp.Value;
                }
            }

            int randomDamage = Utility.RandomMinMax(1, totalDamage);

            totalDamage = 0;

            foreach (KeyValuePair<Mobile, int> kvp in validEntries)
            {
                totalDamage += kvp.Value;

                if (totalDamage >= randomDamage)
                {
                    GiveArtifact(kvp.Key, artifact);
                    return;
                }
            }

            artifact.Delete();
        }

        public void GiveArtifact(Mobile to, Item artifact)
        {
            if (to == null || artifact == null)
                return;

			to.PlaySound(0x5B4);

            Container pack = to.Backpack;

            if (pack == null || !pack.TryDropItem(to, artifact, false))
                artifact.Delete();
            else
                to.SendLocalizedMessage(1062317); // For your valor in combating the fallen beast, a special artifact has been bestowed on you.
        }

        public bool IsEligible(Mobile m, Item Artifact)
        {
            return m.Player && m.Alive && m.Region != null && m.Region == m_Region && m.Backpack != null && m.Backpack.CheckHold(m, Artifact, false);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)7); // version

			writer.Write(KillsMod);
			writer.Write(GroupName);

			writer.Write(SpawnName);
			writer.Write(AutoRestart);
			writer.Write(SpawnMod);
			writer.Write(SpawnRadius);

            writer.Write(m_DamageEntries.Count);
            foreach (KeyValuePair<Mobile, int> kvp in m_DamageEntries)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }

            writer.Write(m_ConfinedRoaming);
            writer.WriteItem<IdolOfTheChampion>(m_Idol);
            writer.Write(m_HasBeenAdvanced);
            writer.Write(m_SpawnArea);

            writer.Write(m_RandomizeType);

            // writer.Write( m_SpawnRange );
            writer.Write(m_Kills);

            writer.Write((bool)m_Active);
            writer.Write((int)m_Type);
            writer.Write(m_Creatures, true);
            writer.Write(m_RedSkulls, true);
            writer.Write(m_WhiteSkulls, true);
            writer.WriteItem<ChampionPlatform>(m_Platform);
            writer.WriteItem<ChampionAltar>(m_Altar);
            writer.Write(m_ExpireDelay);
            writer.WriteDeltaTime(m_ExpireTime);
            writer.Write(m_Champion);
            writer.Write(m_RestartDelay);

            writer.Write(m_RestartTimer != null);

            if (m_RestartTimer != null)
                writer.WriteDeltaTime(m_RestartTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            m_DamageEntries = new Dictionary<Mobile, int>();

            int version = reader.ReadInt();

            switch( version )
            {
				case 7:
					KillsMod = reader.ReadDouble();
					GroupName = reader.ReadString();
					goto case 6;
				case 6:
					SpawnName = reader.ReadString();
					AutoRestart = reader.ReadBool();
					SpawnMod = reader.ReadDouble();
					SpawnRadius = reader.ReadInt();
					goto case 5;
                case 5:
                    {
                        int entries = reader.ReadInt();
                        Mobile m;
                        int damage;
                        for (int i = 0; i < entries; ++i)
                        {
                            m = reader.ReadMobile();
                            damage = reader.ReadInt();

                            if (m == null)
                                continue;

                            m_DamageEntries.Add(m, damage);
                        }

                        goto case 4;
                    }
                case 4:
                    {
                        m_ConfinedRoaming = reader.ReadBool();
                        m_Idol = reader.ReadItem<IdolOfTheChampion>();
                        m_HasBeenAdvanced = reader.ReadBool();

                        goto case 3;
                    }
                case 3:
                    {
                        m_SpawnArea = reader.ReadRect2D();

                        goto case 2;
                    }
                case 2:
                    {
                        m_RandomizeType = reader.ReadBool();

                        goto case 1;
                    }
                case 1:
                    {
                        if (version < 3)
                        {
                            int oldRange = reader.ReadInt();

                            m_SpawnArea = new Rectangle2D(new Point2D(X - oldRange, Y - oldRange), new Point2D(X + oldRange, Y + oldRange));
                        }

                        m_Kills = reader.ReadInt();

                        goto case 0;
                    }
                case 0:
                    {
                        if (version < 1)
                            m_SpawnArea = new Rectangle2D(new Point2D(X - 24, Y - 24), new Point2D(X + 24, Y + 24));	//Default was 24

                        bool active = reader.ReadBool();
                        m_Type = (ChampionSpawnType)reader.ReadInt();
                        m_Creatures = reader.ReadStrongMobileList();
                        m_RedSkulls = reader.ReadStrongItemList();
                        m_WhiteSkulls = reader.ReadStrongItemList();
                        m_Platform = reader.ReadItem<ChampionPlatform>();
                        m_Altar = reader.ReadItem<ChampionAltar>();
                        m_ExpireDelay = reader.ReadTimeSpan();
                        m_ExpireTime = reader.ReadDeltaTime();
                        m_Champion = reader.ReadMobile();
                        m_RestartDelay = reader.ReadTimeSpan();

                        if (reader.ReadBool())
                        {
                            m_RestartTime = reader.ReadDeltaTime();
                            BeginRestart(m_RestartTime - DateTime.UtcNow);
                        }

                        if (version < 4)
                        {
                            m_Idol = new IdolOfTheChampion(this);
                            m_Idol.MoveToWorld(new Point3D(X, Y, Z - 15), Map);
                        }

                        if (m_Platform == null || m_Altar == null || m_Idol == null)
                            Delete();
                        else if (active)
                            Start(true);

                        break;
                    }
            }

            foreach (BaseCreature bc in m_Creatures.OfType<BaseCreature>())
            {
                bc.IsChampionSpawn = true;
            }

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(UpdateRegion));
        }

		public void SendGump(Mobile mob)
		{
			mob.SendGump(new ChampionSpawnInfoGump(this));
		}

		private class ChampionSpawnInfoGump : Gump
		{
			private class Damager
			{
				public Mobile Mobile;
				public int Damage;
				public Damager(Mobile mob, int dmg)
				{
					Mobile = mob;
					Damage = dmg;
				}

			}
			private const int gBoarder = 20;
			private const int gRowHeight = 25;
			private const int gFontHue = 0;
			private static readonly int[] gWidths = { 20, 160, 160, 20 };
			private static readonly int[] gTab;
			private static readonly int gWidth;

			static ChampionSpawnInfoGump()
			{
				gWidth = gWidths.Sum();
				int tab = 0;
				gTab = new int[gWidths.Length];
				for (int i = 0; i < gWidths.Length; ++i)
				{
					gTab[i] = tab;
					tab += gWidths[i];
				}
			}

			private ChampionSpawn m_Spawn;

			public ChampionSpawnInfoGump(ChampionSpawn spawn)
				: base(40, 40)
			{
				m_Spawn = spawn;

				AddBackground(0, 0, gWidth, gBoarder * 2 + gRowHeight * (8 + spawn.m_DamageEntries.Count), 0x13BE);

				int top = gBoarder;
				AddLabel(gBoarder, top, gFontHue, "Champion Spawn Info Gump");
				top += gRowHeight;

				AddLabel(gTab[1], top, gFontHue, "Kills");
				AddLabel(gTab[2], top, gFontHue, spawn.Kills.ToString());
				top += gRowHeight;

				AddLabel(gTab[1], top, gFontHue, "Max Kills");
				AddLabel(gTab[2], top, gFontHue, spawn.MaxKills.ToString());
				top += gRowHeight;

				AddLabel(gTab[1], top, gFontHue, "Level");
				AddLabel(gTab[2], top, gFontHue, spawn.Level.ToString());
				top += gRowHeight;

				AddLabel(gTab[1], top, gFontHue, "Rank");
				AddLabel(gTab[2], top, gFontHue, spawn.Rank.ToString());
				top += gRowHeight;

				AddLabel(gTab[1], top, gFontHue, "Active");
				AddLabel(gTab[2], top, gFontHue, spawn.Active.ToString());
				top += gRowHeight;

				AddLabel(gTab[1], top, gFontHue, "Auto Restart");
				AddLabel(gTab[2], top, gFontHue, spawn.AutoRestart.ToString());
				top += gRowHeight;

				List<Damager> damagers = new List<Damager>();
				foreach (Mobile mob in spawn.m_DamageEntries.Keys)
				{
					damagers.Add(new Damager(mob, spawn.m_DamageEntries[mob]));
				}
				damagers = damagers.OrderByDescending(x => x.Damage).ToList<Damager>();

				foreach (Damager damager in damagers)
				{
					AddLabelCropped(gTab[1], top, 100, gRowHeight, gFontHue, damager.Mobile.RawName);
					AddLabelCropped(gTab[2], top, 80, gRowHeight, gFontHue, damager.Damage.ToString());
					top += gRowHeight;
				}

				AddButton(gWidth - (gBoarder + 30), top, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
				AddLabel(gWidth - (gBoarder + 100), top, gFontHue, "Refresh");
			}

			public override void OnResponse(Network.NetState sender, RelayInfo info)
			{
				switch (info.ButtonID)
				{
					case 1:
						m_Spawn.SendGump(sender.Mobile);
						break;
				}
			}
		}
	}

    public class ChampionSpawnRegion : BaseRegion
    {
        public static void Initialize()
        {
            EventSink.Logout += OnLogout;
            EventSink.Login += OnLogin;
        }

        public override bool YoungProtected
        {
            get
            {
                return false;
            }
        }

        private readonly ChampionSpawn m_Spawn;

        public ChampionSpawn ChampionSpawn
        {
            get
            {
                return m_Spawn;
            }
        }

        public ChampionSpawnRegion(ChampionSpawn spawn)
            : base(null, spawn.Map, Region.Find(spawn.Location, spawn.Map), spawn.SpawnArea)
        {
            m_Spawn = spawn;
        }

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            return false;
        }

        public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
        {
            base.AlterLightLevel(m, ref global, ref personal);
            global = Math.Max(global, 1 + m_Spawn.Level);	//This is a guesstimate.  TODO: Verify & get exact values // OSI testing: at 2 red skulls, light = 0x3 ; 1 red = 0x3.; 3 = 8; 9 = 0xD 8 = 0xD 12 = 0x12 10 = 0xD
        }

        public override bool OnMoveInto(Mobile m, Direction d, Point3D newLocation, Point3D oldLocation)
        {
            if (m is PlayerMobile && !m.Alive && (m.Corpse == null || m.Corpse.Deleted) && Map == Map.Felucca)
            {
                return false;
            }

            return base.OnMoveInto(m, d, newLocation, oldLocation);
        }

        public static void OnLogout(LogoutEventArgs e)
        {
            Mobile m = e.Mobile;

            if (m is PlayerMobile && m.Region.IsPartOf<ChampionSpawnRegion>() && m.AccessLevel == AccessLevel.Player && m.Map == Map.Felucca)
            {
                if (m.Alive && m.Backpack != null)
                {
                    var list = new List<Item>(m.Backpack.Items.Where(i => i.LootType == LootType.Cursed));

                    foreach (var item in list)
                    {
                        item.MoveToWorld(m.Location, m.Map);
                    }

                    ColUtility.Free(list);
                }

                Timer.DelayCall(TimeSpan.FromMilliseconds(250), () =>
                {
                    Map map = m.LogoutMap;

                    Point3D loc = ExorcismSpell.GetNearestShrine(m, ref map);

                    if (loc != Point3D.Zero)
                    {
                        m.LogoutLocation = loc;
                        m.LogoutMap = map;
                    }
                    else
                    {
                        m.LogoutLocation = new Point3D(989, 520, -50);
                        m.LogoutMap = Map.Malas;
                    }
                });
            }
        }

        public static void OnLogin(LoginEventArgs e)
        {
            Mobile m = e.Mobile;

            if (m is PlayerMobile && !m.Alive && (m.Corpse == null || m.Corpse.Deleted) && m.Region.IsPartOf<ChampionSpawnRegion>() && m.Map == Map.Felucca)
            {
                Map map = m.Map;
                Point3D loc = ExorcismSpell.GetNearestShrine(m, ref map);

                if (loc != Point3D.Zero)
                {
                    m.MoveToWorld(loc, map);
                }
                else
                {
                    m.MoveToWorld(new Point3D(989, 520, -50), Map.Malas);
                }
            }
        }
    }

    public class IdolOfTheChampion : Item
    {
        private ChampionSpawn m_Spawn;

        public ChampionSpawn Spawn
        {
            get
            {
                return m_Spawn;
            }
        }

        public override string DefaultName
        {
            get
            {
                return "Idol of the Champion";
            }
        }

        public IdolOfTheChampion(ChampionSpawn spawn)
            : base(0x1F18)
        {
            m_Spawn = spawn;
            Movable = false;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Spawn != null)
                m_Spawn.Delete();
        }

        public IdolOfTheChampion(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(m_Spawn);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        m_Spawn = reader.ReadItem() as ChampionSpawn;

                        if (m_Spawn == null)
                            Delete();

                        break;
                    }
            }
        }
    }
}