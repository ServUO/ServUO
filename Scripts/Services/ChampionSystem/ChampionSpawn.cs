using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
using System.Linq;

namespace Server.Engines.CannedEvil
{
    public class ChampionSpawn : Item
    {
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
                return this.m_ConfinedRoaming;
            }
            set
            {
                this.m_ConfinedRoaming = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasBeenAdvanced
        {
            get
            {
                return this.m_HasBeenAdvanced;
            }
            set
            {
                this.m_HasBeenAdvanced = value;
            }
        }

        [Constructable]
        public ChampionSpawn()
            : base(0xBD2)
        {
            this.Movable = false;
            this.Visible = false;

            this.m_Creatures = new List<Mobile>();
            this.m_RedSkulls = new List<Item>();
            this.m_WhiteSkulls = new List<Item>();

            this.m_Platform = new ChampionPlatform(this);
            this.m_Altar = new ChampionAltar(this);
            this.m_Idol = new IdolOfTheChampion(this);

            this.m_ExpireDelay = TimeSpan.FromMinutes(10.0);
            this.m_RestartDelay = TimeSpan.FromMinutes(10.0);

            this.m_DamageEntries = new Dictionary<Mobile, int>();
			this.m_RandomizeType = false;

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(SetInitialSpawnArea));
        }

        public void SetInitialSpawnArea()
        {
            //Previous default used to be 24;
            this.SpawnArea = new Rectangle2D(new Point2D(this.X - SpawnRadius, this.Y - SpawnRadius),
				new Point2D(this.X + SpawnRadius, this.Y + SpawnRadius));
        }

        public void UpdateRegion()
        {
            if (this.m_Region != null)
                this.m_Region.Unregister();

            if (!this.Deleted && this.Map != Map.Internal)
            {
                this.m_Region = new ChampionSpawnRegion(this);
                this.m_Region.Register();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RandomizeType
        {
            get
            {
                return this.m_RandomizeType;
            }
            set
            {
                this.m_RandomizeType = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Kills
        {
            get
            {
                return this.m_Kills;
            }
            set
            {
                this.m_Kills = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D SpawnArea
        {
            get
            {
                return this.m_SpawnArea;
            }
            set
            {
                this.m_SpawnArea = value;
                this.InvalidateProperties();
                this.UpdateRegion();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan RestartDelay
        {
            get
            {
                return this.m_RestartDelay;
            }
            set
            {
                this.m_RestartDelay = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime RestartTime
        {
            get
            {
                return this.m_RestartTime;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan ExpireDelay
        {
            get
            {
                return this.m_ExpireDelay;
            }
            set
            {
                this.m_ExpireDelay = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ExpireTime
        {
            get
            {
                return this.m_ExpireTime;
            }
            set
            {
                this.m_ExpireTime = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ChampionSpawnType Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
                this.InvalidateProperties();
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
                if (value)
                    this.Start();
                else
                    this.Stop();

                PrimevalLichPuzzle.Update(this);
				
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Champion
        {
            get
            {
                return this.m_Champion;
            }
            set
            {
                this.m_Champion = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Level
        {
            get
            {
                return this.m_RedSkulls.Count;
            }
            set
            {
                for (int i = this.m_RedSkulls.Count - 1; i >= value; --i)
                {
                    this.m_RedSkulls[i].Delete();
                    this.m_RedSkulls.RemoveAt(i);
                }

                for (int i = this.m_RedSkulls.Count; i < value; ++i)
                {
                    Item skull = new Item(0x1854);

                    skull.Hue = 0x26;
                    skull.Movable = false;
                    skull.Light = LightType.Circle150;

                    skull.MoveToWorld(this.GetRedSkullLocation(i), this.Map);

                    this.m_RedSkulls.Add(skull);
                }

                this.InvalidateProperties();
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
            return this.m_Creatures.Contains(m);
        }

        public void SetWhiteSkullCount(int val)
        {
            for (int i = this.m_WhiteSkulls.Count - 1; i >= val; --i)
            {
                this.m_WhiteSkulls[i].Delete();
                this.m_WhiteSkulls.RemoveAt(i);
            }

            for (int i = this.m_WhiteSkulls.Count; i < val; ++i)
            {
                Item skull = new Item(0x1854);

                skull.Movable = false;
                skull.Light = LightType.Circle150;

                skull.MoveToWorld(this.GetWhiteSkullLocation(i), this.Map);

                this.m_WhiteSkulls.Add(skull);

                Effects.PlaySound(skull.Location, skull.Map, 0x29);
                Effects.SendLocationEffect(new Point3D(skull.X + 1, skull.Y + 1, skull.Z), skull.Map, 0x3728, 10);
            }
        }

        public void Start()
        {
            if (this.m_Active || this.Deleted)
                return;

            this.m_Active = true;
            this.m_HasBeenAdvanced = false;

            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = new SliceTimer(this);
            this.m_Timer.Start();

            if (this.m_RestartTimer != null)
                this.m_RestartTimer.Stop();

            this.m_RestartTimer = null;

            if (this.m_Altar != null)
            {
                if (this.m_Champion != null)
                    this.m_Altar.Hue = 0x26;
                else
                    this.m_Altar.Hue = 0;
            }

            if (this.m_Platform != null)
                this.m_Platform.Hue = 0x452;

            PrimevalLichPuzzle.Update(this);
        }

        public void Stop()
        {
            if (!this.m_Active || this.Deleted)
                return;

            this.m_Active = false;
            this.m_HasBeenAdvanced = false;

            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;

            if (this.m_RestartTimer != null)
                this.m_RestartTimer.Stop();

            this.m_RestartTimer = null;

            if (this.m_Altar != null)
                this.m_Altar.Hue = 0;

            if (this.m_Platform != null)
                this.m_Platform.Hue = 0x497;

            PrimevalLichPuzzle.Update(this);
        }

        public void BeginRestart(TimeSpan ts)
        {
            if (this.m_RestartTimer != null)
                this.m_RestartTimer.Stop();

            this.m_RestartTime = DateTime.UtcNow + ts;

            this.m_RestartTimer = new RestartTimer(this, ts);
            this.m_RestartTimer.Start();
        }

        public void EndRestart()
        {
            if (this.RandomizeType)
            {
                switch (Utility.Random(5))
                {
                    case 0:
                        this.Type = ChampionSpawnType.Abyss; break;
                    case 1:
                        this.Type = ChampionSpawnType.Arachnid; break;
                    case 2:
                        this.Type = ChampionSpawnType.ColdBlood; break;
                    case 3:
                        this.Type = ChampionSpawnType.VerminHorde; break;
                    case 4:
                        this.Type = ChampionSpawnType.UnholyTerror; break;
                }
            }

            this.m_HasBeenAdvanced = false;

            this.Start();
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
				
                if (prot.Map != killer.Map || prot.Kills >= 5 || prot.Criminal || !JusticeVirtue.CheckMapRegion(killer, prot))
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

        public void OnSlice()
        {
            if (!this.m_Active || this.Deleted)
                return;

			int currentRank = Rank;

            if (this.m_Champion != null)
            {
                if (this.m_Champion.Deleted)
                {
                    this.RegisterDamageTo(this.m_Champion);

                    if (this.m_Champion is BaseChampion)
                        this.AwardArtifact(((BaseChampion)this.m_Champion).GetArtifact());

                    this.m_DamageEntries.Clear();

                    if (this.m_Platform != null)
                        this.m_Platform.Hue = 0x497;

                    if (this.m_Altar != null)
                    {
                        this.m_Altar.Hue = 0;

                        if (!Core.ML || this.Map == Map.Felucca)
                        {
                            new StarRoomGate(true, this.m_Altar.Location, this.m_Altar.Map);
                        }
                    }

                    this.m_Champion = null;
                    this.Stop();

					if(AutoRestart)
						this.BeginRestart(this.m_RestartDelay);
                }
            }
            else
            {
                int kills = this.m_Kills;

                for (int i = 0; i < this.m_Creatures.Count; ++i)
                {
                    Mobile m = this.m_Creatures[i];

                    if (m.Deleted)
                    {
                        if (m.Corpse != null && !m.Corpse.Deleted)
                        {
                            ((Corpse)m.Corpse).BeginDecay(TimeSpan.FromMinutes(1));
                        }
                        this.m_Creatures.RemoveAt(i);
                        --i;

						int rankOfMob = GetRankFor(m);
						if(rankOfMob == currentRank)
							++this.m_Kills;

                        Mobile killer = m.FindMostRecentDamager(false);

                        this.RegisterDamageTo(m);

                        if (killer is BaseCreature)
                            killer = ((BaseCreature)killer).GetMaster();

                        if (killer is PlayerMobile)
                        {
                            #region Scroll of Transcendence
                            if (Core.ML)
                            {
                                if (this.Map == Map.Felucca)
                                {
                                    if (Utility.RandomDouble() < ChampionSystem.ScrollChance)
                                    {
                                        PlayerMobile pm = (PlayerMobile)killer;

                                        if (Utility.RandomDouble() < ChampionSystem.TranscendenceChance)
                                        {
                                            ScrollofTranscendence SoTF = this.CreateRandomSoT(true);
                                            GiveScrollTo(pm, (SpecialScroll)SoTF);
                                        }
                                        else
                                        {
                                            PowerScroll PS = PowerScroll.CreateRandomNoCraft(5, 5);
                                            GiveScrollTo(pm, (SpecialScroll)PS);
                                        }
                                    }
                                }

                                if (this.Map == Map.Ilshenar || this.Map == Map.Tokuno || this.Map == Map.Malas)
                                {
                                    if (Utility.RandomDouble() < 0.0015)
                                    {
                                        killer.SendLocalizedMessage(1094936); // You have received a Scroll of Transcendence!
                                        ScrollofTranscendence SoTT = this.CreateRandomSoT(false);
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

                                info.Award(this.m_Type, mobSubLevel);
                            }
                        }
                    }
                }

                // Only really needed once.
                if (this.m_Kills > kills)
                    this.InvalidateProperties();

                double n = this.m_Kills / (double)this.MaxKills;
                int p = (int)(n * 100);

                if (p >= 90)
                    this.AdvanceLevel();
                else if (p > 0)
                    this.SetWhiteSkullCount(p / 20);

                if (DateTime.UtcNow >= this.m_ExpireTime)
                    this.Expire();

                this.Respawn();
            }
        }

        public void AdvanceLevel()
        {
            this.m_ExpireTime = DateTime.UtcNow + this.m_ExpireDelay;

            if (this.Level < 16)
            {
                this.m_Kills = 0;
                ++this.Level;
                this.InvalidateProperties();
                this.SetWhiteSkullCount(0);

                if (this.m_Altar != null)
                {
                    Effects.PlaySound(this.m_Altar.Location, this.m_Altar.Map, 0x29);
                    Effects.SendLocationEffect(new Point3D(this.m_Altar.X + 1, this.m_Altar.Y + 1, this.m_Altar.Z), this.m_Altar.Map, 0x3728, 10);
                }
            }
            else
            {
                this.SpawnChampion();
            }
        }

        public void SpawnChampion()
        {
            if (this.m_Altar != null)
                this.m_Altar.Hue = 0x26;

            if (this.m_Platform != null)
                this.m_Platform.Hue = 0x452;

            this.m_Kills = 0;
            this.Level = 0;
            this.InvalidateProperties();
            this.SetWhiteSkullCount(0);

            try
            {
                this.m_Champion = Activator.CreateInstance(ChampionSpawnInfo.GetInfo(this.m_Type).Champion) as Mobile;
            }
            catch
            {
            }

            if (this.m_Champion != null)
                this.m_Champion.MoveToWorld(new Point3D(this.X, this.Y, this.Z - 15), this.Map);
        }

        public void Respawn()
        {
            if (!this.m_Active || this.Deleted || this.m_Champion != null)
                return;

			int currentLevel = Level;
			int currentRank = Rank;
			int maxSpawn = (int)((double)MaxKills * 0.5d * SpawnMod);
			if (currentLevel >= 16)
				maxSpawn = Math.Min(maxSpawn, MaxKills - m_Kills);
			if (maxSpawn < 3)
				maxSpawn = 3;

			int spawnRadius = (int)(SpawnRadius * ChampionSystem.SpawnRadiusModForLevel(Level));
			Rectangle2D spawnBounds = new Rectangle2D(new Point2D(this.X - spawnRadius, this.Y - spawnRadius),
				new Point2D(this.X + spawnRadius, this.Y + spawnRadius));

			int mobCount = 0;
			foreach(Mobile m in m_Creatures)
			{
				if (GetRankFor(m) == currentRank)
					++mobCount;
			}

			while (mobCount <= maxSpawn)
            {
                Mobile m = this.Spawn();

                if (m == null)
                    return;

                Point3D loc = GetSpawnLocation(spawnBounds, spawnRadius);

                // Allow creatures to turn into Paragons at Ilshenar champions.
                m.OnBeforeSpawn(loc, this.Map);

                this.m_Creatures.Add(m);
                m.MoveToWorld(loc, this.Map);
				++mobCount;

                if (m is BaseCreature)
                {
                    BaseCreature bc = m as BaseCreature;
                    bc.Tamable = false;

                    if (!this.m_ConfinedRoaming)
                    {
                        bc.Home = this.Location;
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
            Map map = this.Map;

            if (map == null)
                return this.Location;

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

                int z = this.Map.GetAverageZ(x, y);

                if (this.Map.CanSpawnMobile(new Point2D(x, y), z))
                    return new Point3D(x, y, z);

                /* try @ platform Z if map z fails */
                else if (this.Map.CanSpawnMobile(new Point2D(x, y), this.m_Platform.Location.Z))
                    return new Point3D(x, y, this.m_Platform.Location.Z);
            }

            return this.Location;
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
            Type[][] types = ChampionSpawnInfo.GetInfo(this.m_Type).SpawnTypes;
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
            Type[][] types = ChampionSpawnInfo.GetInfo(this.m_Type).SpawnTypes;

			int v = Rank;

            if (v >= 0 && v < types.Length)
                return this.Spawn(types[v]);

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
            this.m_Kills = 0;

            if (this.m_WhiteSkulls.Count == 0)
            {
                // They didn't even get 20%, go back a level
                if (this.Level > 0)
                    --this.Level;

                this.InvalidateProperties();
            }
            else
            {
                this.SetWhiteSkullCount(0);
            }

            this.m_ExpireTime = DateTime.UtcNow + this.m_ExpireDelay;
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

            return new Point3D(this.X + x, this.Y + y, this.Z - 15);
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

            return new Point3D(this.X + x, this.Y + y, this.Z - 15);
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add("champion spawn");
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_Active)
            {
                list.Add(1060742); // active
                list.Add(1060658, "Type\t{0}", this.m_Type); // ~1_val~: ~2_val~
                list.Add(1060659, "Level\t{0}", this.Level); // ~1_val~: ~2_val~
                list.Add(1060660, "Kills\t{0} of {1} ({2:F1}%)", this.m_Kills, this.MaxKills, 100.0 * ((double)this.m_Kills / this.MaxKills)); // ~1_val~: ~2_val~
                //list.Add( 1060661, "Spawn Range\t{0}", m_SpawnRange ); // ~1_val~: ~2_val~
            }
            else
            {
                list.Add(1060743); // inactive
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            if (this.m_Active)
                this.LabelTo(from, "{0} (Active; Level: {1}; Kills: {2}/{3})", this.m_Type, this.Level, this.m_Kills, this.MaxKills);
            else
                this.LabelTo(from, "{0} (Inactive)", this.m_Type);
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendGump(new PropertiesGump(from, this));
        }

        public override void OnLocationChange(Point3D oldLoc)
        {
            if (this.Deleted)
                return;

            if (this.m_Platform != null)
                this.m_Platform.Location = new Point3D(this.X, this.Y, this.Z - 20);

            if (this.m_Altar != null)
                this.m_Altar.Location = new Point3D(this.X, this.Y, this.Z - 15);

            if (this.m_Idol != null)
                this.m_Idol.Location = new Point3D(this.X, this.Y, this.Z - 15);

            if (this.m_RedSkulls != null)
            {
                for (int i = 0; i < this.m_RedSkulls.Count; ++i)
                    this.m_RedSkulls[i].Location = this.GetRedSkullLocation(i);
            }

            if (this.m_WhiteSkulls != null)
            {
                for (int i = 0; i < this.m_WhiteSkulls.Count; ++i)
                    this.m_WhiteSkulls[i].Location = this.GetWhiteSkullLocation(i);
            }

            this.m_SpawnArea.X += this.Location.X - oldLoc.X;
            this.m_SpawnArea.Y += this.Location.Y - oldLoc.Y;

            this.UpdateRegion();
        }

        public override void OnMapChange()
        {
            if (this.Deleted)
                return;

            if (this.m_Platform != null)
                this.m_Platform.Map = this.Map;

            if (this.m_Altar != null)
                this.m_Altar.Map = this.Map;

            if (this.m_Idol != null)
                this.m_Idol.Map = this.Map;

            if (this.m_RedSkulls != null)
            {
                for (int i = 0; i < this.m_RedSkulls.Count; ++i)
                    this.m_RedSkulls[i].Map = this.Map;
            }

            if (this.m_WhiteSkulls != null)
            {
                for (int i = 0; i < this.m_WhiteSkulls.Count; ++i)
                    this.m_WhiteSkulls[i].Map = this.Map;
            }

            this.UpdateRegion();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_Platform != null)
                this.m_Platform.Delete();

            if (this.m_Altar != null)
                this.m_Altar.Delete();

            if (this.m_Idol != null)
                this.m_Idol.Delete();

            if (this.m_RedSkulls != null)
            {
                for (int i = 0; i < this.m_RedSkulls.Count; ++i)
                    this.m_RedSkulls[i].Delete();

                this.m_RedSkulls.Clear();
            }

            if (this.m_WhiteSkulls != null)
            {
                for (int i = 0; i < this.m_WhiteSkulls.Count; ++i)
                    this.m_WhiteSkulls[i].Delete();

                this.m_WhiteSkulls.Clear();
            }

            if (this.m_Creatures != null)
            {
                for (int i = 0; i < this.m_Creatures.Count; ++i)
                {
                    Mobile mob = this.m_Creatures[i];

                    if (!mob.Player)
                        mob.Delete();
                }

                this.m_Creatures.Clear();
            }

            if (this.m_Champion != null && !this.m_Champion.Player)
                this.m_Champion.Delete();

            this.Stop();

            this.UpdateRegion();
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

                this.RegisterDamage(damager, de.DamageGiven);
            }
        }

        public void RegisterDamage(Mobile from, int amount)
        {
            if (from == null || !from.Player)
                return;

            if (this.m_DamageEntries.ContainsKey(from))
                this.m_DamageEntries[from] += amount;
            else
                this.m_DamageEntries.Add(from, amount);
        }

        public void AwardArtifact(Item artifact)
        {
            if (artifact == null)
                return;

            int totalDamage = 0;

            Dictionary<Mobile, int> validEntries = new Dictionary<Mobile, int>();

            foreach (KeyValuePair<Mobile, int> kvp in this.m_DamageEntries)
            {
                if (this.IsEligible(kvp.Key, artifact))
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
                    this.GiveArtifact(kvp.Key, artifact);
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
            return m.Player && m.Alive && m.Region != null && m.Region == this.m_Region && m.Backpack != null && m.Backpack.CheckHold(m, Artifact, false);
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

            writer.Write(this.m_DamageEntries.Count);
            foreach (KeyValuePair<Mobile, int> kvp in this.m_DamageEntries)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }

            writer.Write(this.m_ConfinedRoaming);
            writer.WriteItem<IdolOfTheChampion>(this.m_Idol);
            writer.Write(this.m_HasBeenAdvanced);
            writer.Write(this.m_SpawnArea);

            writer.Write(this.m_RandomizeType);

            // writer.Write( m_SpawnRange );
            writer.Write(this.m_Kills);

            writer.Write((bool)this.m_Active);
            writer.Write((int)this.m_Type);
            writer.Write(this.m_Creatures, true);
            writer.Write(this.m_RedSkulls, true);
            writer.Write(this.m_WhiteSkulls, true);
            writer.WriteItem<ChampionPlatform>(this.m_Platform);
            writer.WriteItem<ChampionAltar>(this.m_Altar);
            writer.Write(this.m_ExpireDelay);
            writer.WriteDeltaTime(this.m_ExpireTime);
            writer.Write(this.m_Champion);
            writer.Write(this.m_RestartDelay);

            writer.Write(this.m_RestartTimer != null);

            if (this.m_RestartTimer != null)
                writer.WriteDeltaTime(this.m_RestartTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            this.m_DamageEntries = new Dictionary<Mobile, int>();

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

                            this.m_DamageEntries.Add(m, damage);
                        }

                        goto case 4;
                    }
                case 4:
                    {
                        this.m_ConfinedRoaming = reader.ReadBool();
                        this.m_Idol = reader.ReadItem<IdolOfTheChampion>();
                        this.m_HasBeenAdvanced = reader.ReadBool();

                        goto case 3;
                    }
                case 3:
                    {
                        this.m_SpawnArea = reader.ReadRect2D();

                        goto case 2;
                    }
                case 2:
                    {
                        this.m_RandomizeType = reader.ReadBool();

                        goto case 1;
                    }
                case 1:
                    {
                        if (version < 3)
                        {
                            int oldRange = reader.ReadInt();

                            this.m_SpawnArea = new Rectangle2D(new Point2D(this.X - oldRange, this.Y - oldRange), new Point2D(this.X + oldRange, this.Y + oldRange));
                        }

                        this.m_Kills = reader.ReadInt();

                        goto case 0;
                    }
                case 0:
                    {
                        if (version < 1)
                            this.m_SpawnArea = new Rectangle2D(new Point2D(this.X - 24, this.Y - 24), new Point2D(this.X + 24, this.Y + 24));	//Default was 24

                        bool active = reader.ReadBool();
                        this.m_Type = (ChampionSpawnType)reader.ReadInt();
                        this.m_Creatures = reader.ReadStrongMobileList();
                        this.m_RedSkulls = reader.ReadStrongItemList();
                        this.m_WhiteSkulls = reader.ReadStrongItemList();
                        this.m_Platform = reader.ReadItem<ChampionPlatform>();
                        this.m_Altar = reader.ReadItem<ChampionAltar>();
                        this.m_ExpireDelay = reader.ReadTimeSpan();
                        this.m_ExpireTime = reader.ReadDeltaTime();
                        this.m_Champion = reader.ReadMobile();
                        this.m_RestartDelay = reader.ReadTimeSpan();

                        if (reader.ReadBool())
                        {
                            this.m_RestartTime = reader.ReadDeltaTime();
                            this.BeginRestart(this.m_RestartTime - DateTime.UtcNow);
                        }

                        if (version < 4)
                        {
                            this.m_Idol = new IdolOfTheChampion(this);
                            this.m_Idol.MoveToWorld(new Point3D(this.X, this.Y, this.Z - 15), this.Map);
                        }

                        if (this.m_Platform == null || this.m_Altar == null || this.m_Idol == null)
                            this.Delete();
                        else if (active)
                            this.Start();

                        break;
                    }
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
                return this.m_Spawn;
            }
        }

        public ChampionSpawnRegion(ChampionSpawn spawn)
            : base(null, spawn.Map, Region.Find(spawn.Location, spawn.Map), spawn.SpawnArea)
        {
            this.m_Spawn = spawn;
        }

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            return false;
        }

        public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
        {
            base.AlterLightLevel(m, ref global, ref personal);
            global = Math.Max(global, 1 + this.m_Spawn.Level);	//This is a guesstimate.  TODO: Verify & get exact values // OSI testing: at 2 red skulls, light = 0x3 ; 1 red = 0x3.; 3 = 8; 9 = 0xD 8 = 0xD 12 = 0x12 10 = 0xD
        }
    }

    public class IdolOfTheChampion : Item
    {
        private ChampionSpawn m_Spawn;

        public ChampionSpawn Spawn
        {
            get
            {
                return this.m_Spawn;
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
            this.m_Spawn = spawn;
            this.Movable = false;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_Spawn != null)
                this.m_Spawn.Delete();
        }

        public IdolOfTheChampion(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Spawn);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Spawn = reader.ReadItem() as ChampionSpawn;

                        if (this.m_Spawn == null)
                            this.Delete();

                        break;
                    }
            }
        }
    }
}