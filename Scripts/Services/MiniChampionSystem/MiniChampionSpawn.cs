using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using System.Collections.Generic;
using Server.Commands;
using System.Linq;

namespace Server.Engines.CannedEvil
{
    public class MiniChamp : Item
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenMiniChamp", AccessLevel.Administrator, new CommandEventHandler(GenStoneRuins_OnCommand));
            CommandSystem.Register("GenMiniChampDelete", AccessLevel.Administrator, new CommandEventHandler(GenMiniChampDelete_OnCommand));
            CommandSystem.Register("ClearSpawners", AccessLevel.Administrator, new CommandEventHandler(ClearSpawners_OnCommand));
        }

        [Usage("ClearSpawners")]
        [Description("Resets all previous mini champ spawners.")]
        public static void ClearSpawners_OnCommand(CommandEventArgs e)
        {
            ClearSpawners(e.Mobile);
        }

        [Usage("GenMiniChampDelete")]
        [Description("Removes all Mini Champ.")]
        public static void GenMiniChampDelete_OnCommand(CommandEventArgs e)
        {
            DeleteAllMiniChamp(e.Mobile);
        }

        [Usage("GenMiniChamp")]
        [Description("MiniChampion Generator")]
        public static void GenStoneRuins_OnCommand(CommandEventArgs e)
        {
            DeleteAllMiniChamp(e.Mobile);

            Map map = Map.TerMur;

            MiniChamp CrimsonVeins = new MiniChamp();
            CrimsonVeins.Type = MiniChampType.CrimsonVeins;
            CrimsonVeins.MoveToWorld(new Point3D(974, 161, -10), map);
            CrimsonVeins.SpawnArea = new Rectangle2D(new Point2D(950, 137), new Point2D(998, 185));
            CrimsonVeins.Active = true;

            MiniChamp AbyssalLair = new MiniChamp();
            AbyssalLair.Type = MiniChampType.AbyssalLair;
            AbyssalLair.MoveToWorld(new Point3D(987, 328, 11), map);
            AbyssalLair.SpawnArea = new Rectangle2D(new Point2D(963, 304), new Point2D(1011, 352));
            AbyssalLair.Active = true;

            MiniChamp DiscardedCavernClanRibbon = new MiniChamp();
            DiscardedCavernClanRibbon.Type = MiniChampType.DiscardedCavernClanRibbon;
            DiscardedCavernClanRibbon.MoveToWorld(new Point3D(915, 501, -11), map);
            DiscardedCavernClanRibbon.SpawnArea = new Rectangle2D(new Point2D(891, 477), new Point2D(939, 525));
            DiscardedCavernClanRibbon.Active = true;

            MiniChamp DiscardedCavernClanScratch = new MiniChamp();
            DiscardedCavernClanScratch.Type = MiniChampType.DiscardedCavernClanScratch;
            DiscardedCavernClanScratch.MoveToWorld(new Point3D(950, 552, -13), map);
            DiscardedCavernClanScratch.SpawnArea = new Rectangle2D(new Point2D(926, 528), new Point2D(974, 576));
            DiscardedCavernClanScratch.Active = true;

            MiniChamp DiscardedCavernClanChitter = new MiniChamp();
            DiscardedCavernClanChitter.Type = MiniChampType.DiscardedCavernClanChitter;
            DiscardedCavernClanChitter.MoveToWorld(new Point3D(980, 491, -11), map);
            DiscardedCavernClanChitter.SpawnArea = new Rectangle2D(new Point2D(956, 467), new Point2D(1007, 515));
            DiscardedCavernClanChitter.Active = true;

            MiniChamp EnslavedGoblins = new MiniChamp();
            EnslavedGoblins.Type = MiniChampType.EnslavedGoblins;
            EnslavedGoblins.MoveToWorld(new Point3D(578, 799, -45), map);
            EnslavedGoblins.SpawnArea = new Rectangle2D(new Point2D(552, 799), new Point2D(589, 849));
            EnslavedGoblins.Active = true;

            MiniChamp FairyDragonLair = new MiniChamp();
            FairyDragonLair.Type = MiniChampType.FairyDragonLair;
            FairyDragonLair.MoveToWorld(new Point3D(887, 273, 4), map);
            FairyDragonLair.SpawnArea = new Rectangle2D(new Point2D(863, 249), new Point2D(914, 298));
            FairyDragonLair.Active = true;

            MiniChamp FireTemple = new MiniChamp();
            FireTemple.Type = MiniChampType.FireTemple;
            FireTemple.MoveToWorld(new Point3D(546, 760, -91), map);
            FireTemple.SpawnArea = new Rectangle2D(new Point2D(522, 736), new Point2D(570, 784));
            FireTemple.Active = true;

            MiniChamp LandsoftheLich = new MiniChamp();
            LandsoftheLich.Type = MiniChampType.LandsoftheLich;
            LandsoftheLich.MoveToWorld(new Point3D(530, 658, 9), map);
            LandsoftheLich.SpawnArea = new Rectangle2D(new Point2D(506, 634), new Point2D(554, 682));
            LandsoftheLich.Active = true;

            MiniChamp LavaCaldera = new MiniChamp();
            LavaCaldera.Type = MiniChampType.LavaCaldera;
            LavaCaldera.MoveToWorld(new Point3D(578, 900, -72), map);
            LavaCaldera.SpawnArea = new Rectangle2D(new Point2D(554, 876), new Point2D(607, 926));
            LavaCaldera.Active = true;

            MiniChamp PassageofTears = new MiniChamp();
            PassageofTears.Type = MiniChampType.PassageofTears;
            PassageofTears.MoveToWorld(new Point3D(684, 579, -14), map);
            PassageofTears.SpawnArea = new Rectangle2D(new Point2D(660, 555), new Point2D(708, 603));
            PassageofTears.Active = true;

            MiniChamp SecretGarden = new MiniChamp();
            SecretGarden.Type = MiniChampType.SecretGarden;
            SecretGarden.MoveToWorld(new Point3D(434, 701, 29), map);
            SecretGarden.SpawnArea = new Rectangle2D(new Point2D(410, 677), new Point2D(458, 725));
            SecretGarden.Active = true;

            MiniChamp SkeletalDragon = new MiniChamp();
            SkeletalDragon.Type = MiniChampType.SkeletalDragon;
            SkeletalDragon.MoveToWorld(new Point3D(677, 824, -108), map);
            SkeletalDragon.SpawnArea = new Rectangle2D(new Point2D(653, 800), new Point2D(701, 848));
            SkeletalDragon.Active = true;

            MiniChamp MeraktusTheTormented = new MiniChamp();
            MeraktusTheTormented.Type = MiniChampType.MeraktusTheTormented;
            MeraktusTheTormented.MoveToWorld(new Point3D(395, 1913, 12), Map.Malas);
            MeraktusTheTormented.SpawnArea = new Rectangle2D(new Point2D(371, 1889), new Point2D(410, 1940));
            MeraktusTheTormented.Active = true;

            ClearSpawners(e.Mobile);

            e.Mobile.SendMessage("Created Mini Champion Spawns.");
        }

        private static void DeleteAllMiniChamp(Mobile from)
        {
            List<Item> list = new List<Item>();

            foreach (MiniChamp item in _Spawners)
            {
                list.Add(item);
            }

            foreach (Item item in list)
                item.Delete();

            if (list.Count > 0)
                from.SendMessage("{0} minichamps removed.", list.Count);

            list.Clear();
            list.TrimExcess();
        }

        private static void ClearSpawners(Mobile from)
        {
            int reset = 0;

            _Spawners.ForEach(sp =>
            {
                if (sp.Map != null)
                {
                    IPooledEnumerable eable = sp.Map.GetItemsInRange(sp.Location, 30);

                    foreach (Item item in eable)
                    {
                        if (item is XmlSpawner && ((XmlSpawner)item).SequentialSpawn > -1)
                        {
                            reset++;
                            ((XmlSpawner)item).DoReset = true;
                        }
                    }
                }
            });

            from.SendMessage("{0} mini-champ spawners reset.", reset);
        }

        private bool m_Active;
        private bool m_RandomizeType;
        private MiniChampType m_Type;
        private List<Mobile> m_Creatures;
        private int m_Kills;
        private int Spwnd;
        private int m_Level;
        private int spawnn;

        private IdolOfTheMiniChamp m_Idol;
        private MiniChampionAltar m_Altar;

        private Mobile m_Champion;
        private Mobile m_Champion2;

        private Rectangle2D m_SpawnArea;
        private MiniChampRegion m_Region;

        private TimeSpan m_ExpireDelay;
        private DateTime m_ExpireTime;

        private TimeSpan m_RestartDelay;
        private DateTime m_RestartTime;

        private Timer m_Timer, m_RestartTimer;

        private bool m_HasBeenAdvanced;
        private bool m_ConfinedRoaming;

        private Dictionary<Mobile, int> m_DamageEntries;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ConfinedRoaming
        {
            get { return this.m_ConfinedRoaming; }
            set { this.m_ConfinedRoaming = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasBeenAdvanced
        {
            get { return this.m_HasBeenAdvanced; }
            set { this.m_HasBeenAdvanced = value; }
        }

        private static List<MiniChamp> _Spawners = new List<MiniChamp>();
        public static List<MiniChamp> Spawners { get { return _Spawners; } }

        [Constructable]
        public MiniChamp()
            : base(0x1AED)
        {
            this.Movable = false;
            this.Visible = false;

            this.m_Altar = new MiniChampionAltar(this);
            this.m_Idol = new IdolOfTheMiniChamp(this);

            this.m_Creatures = new List<Mobile>();
            this.m_ExpireDelay = TimeSpan.FromMinutes(10.0);
            this.m_RestartDelay = TimeSpan.FromMinutes(10.0);
            this.m_DamageEntries = new Dictionary<Mobile, int>();

            _Spawners.Add(this);

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(SetInitialSpawnArea));
        }

        public void SetInitialSpawnArea()
        {
            //Previous default used to be 24;
            this.SpawnArea = new Rectangle2D(new Point2D(X - 24, Y - 24), new Point2D(X + 24, Y + 24));
        }

        public void UpdateRegion()
        {
            if (this.m_Region != null)
                this.m_Region.Unregister();

            if (!this.Deleted && this.Map != Map.Internal)
            {
                this.m_Region = new MiniChampRegion(this);
                this.m_Region.Register();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RandomizeType
        {
            get { return this.m_RandomizeType; }
            set { this.m_RandomizeType = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Kills
        {
            get { return this.m_Kills; }
            set { this.m_Kills = value; InvalidateProperties(); }
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
                InvalidateProperties();
                UpdateRegion();
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
        public TimeSpan ExpireTime
        {
            get
            {
                TimeSpan ts = this.m_ExpireTime - DateTime.Now;

                if (ts < TimeSpan.Zero)
                    ts = TimeSpan.Zero;

                return ts;
            }
            set
            {
                try
                {
                    this.m_ExpireTime = DateTime.Now + value;
                }
                catch
                { }

            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public MiniChampType Type
        {
            get { return this.m_Type; }
            set { this.m_Type = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return this.m_Active; }
            set { if (value) Start(); else Stop(); InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Champion
        {
            get { return this.m_Champion; }
            set { this.m_Champion = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Champion2
        {
            get { return this.m_Champion2; }
            set { this.m_Champion2 = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Level
        {
            get
            {
                return this.m_Level;
            }
            set
            {
                this.m_Level = value;
                InvalidateProperties();
            }
        }

        public int MaxKills
        {
            get { return 20; }
        }

        public bool IsMiniChamp(Mobile m)
        {
            return this.m_Creatures.Contains(m);
        }

        public void Start()
        {
            if (this.m_Active || Deleted)
                return;

            this.m_Active = true;
            this.m_HasBeenAdvanced = false;

            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = new MCSliceTimer(this);
            this.m_Timer.Start();

            this.m_RestartTimer = null;

        }

        public void Stop()
        {
            if (!this.m_Active || Deleted)
                return;

            this.m_Active = false;
            this.m_HasBeenAdvanced = false;

            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;

            if (this.m_RestartTimer != null)
                this.m_RestartTimer.Stop();

            this.m_RestartTimer = null;
        }

        public void BeginRestart(TimeSpan ts)
        {
            if (this.m_RestartTimer != null)
                this.m_RestartTimer.Stop();

            this.m_RestartTime = DateTime.Now + ts;

            this.m_RestartTimer = new MCRestartTimer(this, ts);
            this.m_RestartTimer.Start();
        }

        public void EndRestart()
        {
            this.m_HasBeenAdvanced = false;

            Start();
        }

        public void OnSlice()
        {
            if (!this.m_Active || Deleted)
                return;

            if (this.ExpireTime == TimeSpan.Zero && Kills != 0)
                this.Expire();

            int ChampsOut = 0;

            if (m_Champion != null)
                ChampsOut += 1;
            if (m_Champion2 != null)
                ChampsOut += 1;

            if (ChampsOut > 0)
            {
                if (this.m_Type != MiniChampType.EnslavedGoblins)
                {
                    if (this.m_Champion != null)
                    {
                        if (this.m_Champion.Deleted)
                        {
                            this.RegisterDamageTo(this.m_Champion);

                            if (this.m_Champion is BaseChampion && m_Type == MiniChampType.MeraktusTheTormented)
                                this.AwardArtifact(((BaseChampion)this.m_Champion).GetArtifact());

                            this.m_DamageEntries.Clear();
                            this.m_Champion = null;

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

                            this.Stop();
                            this.BeginRestart(this.m_RestartDelay);
                        }
                    }
                }
                else
                {
                    if ((this.m_Champion != null && this.m_Champion.Deleted) && (this.m_Champion2 != null && this.m_Champion2.Deleted))
                    {
                        this.m_DamageEntries.Clear();
                        this.m_Champion = null;
                        this.m_Champion2 = null;

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

                        this.Stop();
                        this.BeginRestart(m_RestartDelay);
                    }
                }
            }
            else
            {
                Kills = this.m_Kills;

                for (int i = 0; i < this.m_Creatures.Count; ++i)
                {
                    Mobile m = this.m_Creatures[i];

                    if (m.Deleted)
                    {
                        this.m_Creatures.RemoveAt(i);
                        --i;

                        Mobile killer = m.FindMostRecentDamager(false);

                        bool WiD = WhatIsDead(m);

                        if (WiD == true)
                        {
                            this.m_Kills += 1;

                            if (this.m_Type == MiniChampType.MeraktusTheTormented)
                                MinotaurShouts();

                            if (this.m_Kills <= 10)
                                Spwnd -= 1;
                        }

                        RegisterDamageTo(m);

                        if (killer is BaseCreature)
                            killer = ((BaseCreature)killer).GetMaster();

                        ExpireTime = ExpireDelay;
                    }
                }
            }

            if (this.m_Type == MiniChampType.MeraktusTheTormented)
            {
                if (this.m_Kills >= 20 && m_Level == 0)
                    AdvanceLevel();
                else if (this.m_Kills >= 30 && m_Level == 1)
                    AdvanceLevel();
                else if (this.m_Kills >= 40 && m_Level == 2)
                    AdvanceLevel();
            }
            else
            {
                if (this.m_Kills >= 20)
                    AdvanceLevel();
            }

            Respawn();
        }

        private void MinotaurShouts()
        {
            int cliloc = 0;

            if (this.m_Level == 2 && this.m_Kills == 39)
            {
                cliloc = 1073369;   // A minotaur captain bellows, "Meraktus has arrived!"                
            }
            else if (this.m_Level == 2 && this.m_Kills == 1)
            {
                cliloc = 1073368;   // A minotaur captain barks an order to his fellow scouts, "Aid the others! Kill the invaders!"               
            }
            else if (m_Level == 1 && this.m_Kills == 1)
            {
                cliloc = 1073367;   // A minotaur scout barks an order to his fellow minotaurs, "Invaders are among us! Crush them!"        
            }
            else if (m_Level == 0 && this.m_Kills == 1)
            {
                cliloc = 1073370;   // A minotaur bellows, "I see invaders! Must warn the others!"          
            }
            else
            {
                return;
            }

            Map map = Map.Malas;
            ArrayList list = new ArrayList();
            Point3D loc1 = new Point3D(388, 1920, 0);

            foreach (Mobile m in map.GetMobilesInRange(loc1, 50))
            {
                if (m is PlayerMobile)
                    m.SendLocalizedMessage(cliloc, "", 0x22);
            }
        }

        public bool WhatIsDead(Mobile Mon)
        {
            if (this.m_Type == MiniChampType.EnslavedGoblins)
            {
                if (this.m_Level == 0)
                {
                    if (Mon is EnslavedGrayGoblin || Mon is EnslavedGreenGoblin)
                        return true;
                }
                else if (this.m_Level == 1)
                {
                    if (Mon is EnslavedGoblinScout || Mon is EnslavedGoblinKeeper)
                        return true;
                }
                else if (this.m_Level == 2)
                {
                    if (Mon is EnslavedGoblinMage || Mon is EnslavedGreenGoblinAlchemist)
                        return true;
                }
            }
            else if (this.m_Type == MiniChampType.CrimsonVeins)
            {
                if (this.m_Level == 0)
                {
                    if (Mon is LavaSnake || Mon is LavaLizard || Mon is FireAnt)
                        return true;
                }
                else if (this.m_Level == 1)
                {
                    if (Mon is Efreet || Mon is FireGargoyle)
                        return true;
                }
                else if (this.m_Level == 2)
                {
                    if (Mon is LavaElemental || Mon is FireDaemon)
                        return true;
                }
            }
            else if (this.m_Type == MiniChampType.FairyDragonLair)
            {
                if (this.m_Level == 0)
                {
                    if (Mon is FairyDragon)
                        return true;
                }
                else if (this.m_Level == 1)
                {
                    if (Mon is Wyvern)
                        return true;
                }
                else if (this.m_Level == 2)
                {
                    if (Mon is ForgottenServant)
                        return true;
                }
            }
            else if (this.m_Type == MiniChampType.AbyssalLair)
            {
                if (this.m_Level == 0)
                {
                    if (Mon is GreaterMongbat || Mon is Imp)
                        return true;
                }
                else if (this.m_Level == 1)
                {
                    if (Mon is Daemon)
                        return true;
                }
                else if (this.m_Level == 2)
                {
                    if (Mon is PitFiend)
                        return true;
                }
            }
            else if (this.m_Type == MiniChampType.DiscardedCavernClanRibbon)
            {
                if (this.m_Level == 0)
                {
                    if (Mon is ClanRibbonPlagueRat || Mon is ClanRS)
                        return true;
                }
                else if (this.m_Level == 1)
                {
                    if (Mon is ClanRibbonPlagueRat || Mon is ClanRC)
                        return true;
                }
            }
            else if (this.m_Type == MiniChampType.DiscardedCavernClanScratch)
            {
                if (this.m_Level == 0)
                {
                    if (Mon is ClanSSW || Mon is ClanSS)
                        return true;
                }
                else if (this.m_Level == 1)
                {
                    if (Mon is ClanSSW || Mon is ClanSH)
                        return true;
                }
            }
            else if (this.m_Type == MiniChampType.DiscardedCavernClanChitter)
            {
                if (this.m_Level == 0)
                {
                    if (Mon is ClockworkScorpion || Mon is ClanCA)
                        return true;
                }
                else if (this.m_Level == 1)
                {
                    if (Mon is ClockworkScorpion || Mon is ClanCT)
                        return true;
                }
            }
            else if (this.m_Type == MiniChampType.PassageofTears)
            {
                if (this.m_Level == 0)
                {
                    if (Mon is CorrosiveSlime || Mon is AcidSlug)
                        return true;
                }
                else if (this.m_Level == 1)
                {
                    if (Mon is AcidElemental)
                        return true;
                }
                else if (this.m_Level == 2)
                {
                    if (Mon is InterredGrizzle)
                        return true;
                }
            }
            else if (this.m_Type == MiniChampType.LandsoftheLich)
            {
                if (this.m_Level == 0)
                {
                    if (Mon is Wraith || Mon is Spectre || Mon is Shade || Mon is Skeleton || Mon is Zombie)
                        return true;
                }
                else if (this.m_Level == 1)
                {
                    if (Mon is BoneMagi || Mon is SkeletalMage || Mon is BoneKnight || Mon is SkeletalKnight || Mon is WailingBanshee)
                        return true;
                }
                else if (this.m_Level == 2)
                {
                    if (Mon is SkeletalLich || Mon is RottingCorpse)
                        return true;
                }
            }
            else if (this.m_Type == MiniChampType.SecretGarden)
            {
                if (this.m_Level == 0)
                {
                    if (Mon is Pixie)
                        return true;
                }
                else if (this.m_Level == 1)
                {
                    if (Mon is Wisp)
                        return true;
                }
                else if (this.m_Level == 2)
                {
                    if (Mon is DarkWisp)
                        return true;
                }
            }
            else if (this.m_Type == MiniChampType.FireTemple)
            {
                if (this.m_Level == 0)
                {
                    if (Mon is LavaSnake || Mon is LavaLizard || Mon is FireAnt)
                        return true;
                }
                else if (this.m_Level == 1)
                {
                    if (Mon is LavaSerpent || Mon is HellCat || Mon is HellHound)
                        return true;
                }
                else if (this.m_Level == 2)
                {
                    if (Mon is FireDaemon || Mon is LavaElemental)
                        return true;
                }
            }
            else if (this.m_Type == MiniChampType.SkeletalDragon)
            {
                if (this.m_Level == 0)
                {
                    if (Mon is PatchworkSkeleton || Mon is Skeleton)
                        return true;
                }
                else if (this.m_Level == 1)
                {
                    if (Mon is BoneKnight || Mon is SkeletalKnight)
                        return true;
                }
                else if (this.m_Level == 2)
                {
                    if (Mon is BoneMagi || Mon is SkeletalMage)
                        return true;
                }
                else if (this.m_Level == 3)
                {
                    if (Mon is SkeletalLich)
                        return true;
                }
            }
            else if (this.m_Type == MiniChampType.LavaCaldera)
            {
                if (this.m_Level == 0)
                {
                    if (Mon is LavaSnake || Mon is LavaLizard || Mon is FireAnt)
                        return true;
                }
                else if (this.m_Level == 1)
                {
                    if (Mon is LavaSerpent || Mon is HellCat || Mon is HellHound)
                        return true;
                }
                else if (this.m_Level == 2)
                {
                    if (Mon is FireDaemon || Mon is LavaElemental)
                        return true;
                }
            }
            else if (this.m_Type == MiniChampType.MeraktusTheTormented)
            {
                if (this.m_Level == 0)
                {
                    if (Mon is Minotaur)
                        return true;
                }
                else if (this.m_Level == 1)
                {
                    if (Mon is MinotaurScout)
                        return true;
                }
                else if (this.m_Level == 2)
                {
                    if (Mon is MinotaurCaptain)
                        return true;
                }
            }
            return false;
        }

        public void AdvanceLevel()
        {
            /*m_ExpireTime = DateTime.Now + m_ExpireDelay;*/

            if (this.m_Level < 3 && (m_Type == MiniChampType.SkeletalDragon))
            {
                this.m_Kills = 0;
                Spwnd = 0;
                this.m_Level += 1;
            }
            else if (this.m_Level < 2 && (this.m_Type == MiniChampType.SecretGarden || this.m_Type == MiniChampType.FairyDragonLair || this.m_Type == MiniChampType.CrimsonVeins || this.m_Type == MiniChampType.AbyssalLair || this.m_Type == MiniChampType.PassageofTears || this.m_Type == MiniChampType.LavaCaldera || this.m_Type == MiniChampType.EnslavedGoblins || this.m_Type == MiniChampType.LandsoftheLich || this.m_Type == MiniChampType.FireTemple || this.m_Type == MiniChampType.MeraktusTheTormented))
            {
                this.m_Kills = 0;
                Spwnd = 0;
                this.m_Level += 1;
            }
            else if (m_Level < 1 && (m_Type == MiniChampType.DiscardedCavernClanScratch || m_Type == MiniChampType.DiscardedCavernClanRibbon || m_Type == MiniChampType.DiscardedCavernClanChitter))
            {
                this.m_Kills = 0;
                Spwnd = 0;
                this.m_Level += 1;
            }
            else
            {
                SpawnChampion();
            }
        }

        public void Expire()
        {
            this.m_Kills = 0;
            Spwnd = 0;
            this.m_Level = 0;
            InvalidateProperties();

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

            this.Stop();
            this.BeginRestart(m_RestartDelay);
        }

        public void SpawnChampion()
        {
            this.m_Kills = 0;
            Spwnd = 0;
            this.m_Level = 0;
            InvalidateProperties();

            try
            {
                if (this.m_Type == MiniChampType.EnslavedGoblins)
                {
                    this.m_Champion = new GrayGoblinMageRenowned() as Mobile;
                    this.m_Champion2 = new GreenGoblinAlchemistRenowned() as Mobile;
                }
                else
                {
                    this.m_Champion = Activator.CreateInstance(MiniChampInfo.GetInfo(this.m_Type).Champion) as Mobile;
                }
            }
            catch { }

            if (this.m_Type == MiniChampType.MeraktusTheTormented)
            {
                if (this.m_Champion != null)
                    this.m_Champion.MoveToWorld(new Point3D(383, 1932, 10), Map.Malas);
            }
            else
            {
                if (this.m_Champion != null)
                    this.m_Champion.MoveToWorld(new Point3D(X - 1, Y - 1, Z), Map);
                if (this.m_Champion2 != null)
                    this.m_Champion2.MoveToWorld(new Point3D(X + 1, Y + 1, Z), Map);
            }
        }

        public void Respawn()
        {
            if (!this.m_Active || Deleted || this.m_Champion != null)
                return;

            int level = this.Level;

            if (this.m_Type == MiniChampType.MeraktusTheTormented)
            {
                if (this.m_Level == 0)
                    spawnn = 20;
                else if (this.m_Level == 1)
                    spawnn = 30;
                else if (this.m_Level == 2)
                    spawnn = 40;
            }
            else
            {
                spawnn = 20;
            }

            while (Spwnd < spawnn)
            {
                Mobile m = Spawn();

                if (m == null)
                    return;

                Point3D loc = GetSpawnLocation();

                this.m_Creatures.Add(m);
                m.MoveToWorld(loc, Map);

                Spwnd += 1;

                if (m is BaseCreature)
                {
                    BaseCreature bc = m as BaseCreature;
                    bc.Tamable = false;

                    if (!this.m_ConfinedRoaming)
                    {
                        bc.Home = loc;
                        bc.RangeHome = 10;
                    }
                    else
                    {
                        bc.Home = bc.Location;

                        Point2D xWall1 = new Point2D(this.m_SpawnArea.X, bc.Y);
                        Point2D xWall2 = new Point2D(this.m_SpawnArea.X + this.m_SpawnArea.Width, bc.Y);
                        Point2D yWall1 = new Point2D(bc.X, this.m_SpawnArea.Y);
                        Point2D yWall2 = new Point2D(bc.X, this.m_SpawnArea.Y + this.m_SpawnArea.Height);

                        double minXDist = Math.Min(bc.GetDistanceToSqrt(xWall1), bc.GetDistanceToSqrt(xWall2));
                        double minYDist = Math.Min(bc.GetDistanceToSqrt(yWall1), bc.GetDistanceToSqrt(yWall2));

                        bc.RangeHome = (int)Math.Min(minXDist, minYDist);
                    }
                }
            }
        }

        public Point3D GetSpawnLocation()
        {
            Map map = Map;

            if (map == null)
                return Location;

            // Try 20 times to find a spawnable location.
            for (int i = 0; i < 20; i++)
            {
                int x = Utility.Random(m_SpawnArea.X, m_SpawnArea.Width);
                int y = Utility.Random(m_SpawnArea.Y, m_SpawnArea.Height);

                int z = Map.GetAverageZ(x, y);

                if (Map.CanSpawnMobile(new Point2D(x, y), z))
                    return new Point3D(x, y, z);
            }

            return Location;
        }

        public Mobile Spawn()
        {
            Type[][] types = MiniChampInfo.GetInfo(this.m_Type).SpawnTypes;

            int v = this.Level;

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

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add("mini champion spawn");
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_Active)
            {
                list.Add(1060742); // active
                list.Add(1060658, "Type\t{0}", this.m_Type); // ~1_val~: ~2_val~
                list.Add(1060659, "Level\t{0}", Level); // ~1_val~: ~2_val~

                if (this.m_Type == MiniChampType.MeraktusTheTormented)
                {
                    if (this.m_Level == 0)
                        list.Add(1060660, "Kills\t{0} of {1} ({2:F1}%)", this.m_Kills, MaxKills, 100.0 * ((double)this.m_Kills / MaxKills)); // ~1_val~: ~2_val~
                    else if (this.m_Level == 1)
                        list.Add(1060660, "Kills\t{0} of {1} ({2:F1}%)", this.m_Kills, 30, 100.0 * ((double)this.m_Kills / 30)); // ~1_val~: ~2_val~
                    else if (this.m_Level == 2)
                        list.Add(1060660, "Kills\t{0} of {1} ({2:F1}%)", this.m_Kills, 40, 100.0 * ((double)this.m_Kills / 40)); // ~1_val~: ~2_val~
                }
                else
                {
                    list.Add(1060660, "Kills\t{0} of {1} ({2:F1}%)", this.m_Kills, MaxKills, 100.0 * ((double)this.m_Kills / MaxKills)); // ~1_val~: ~2_val~	
                }

            }
            else
            {
                list.Add(1060743); // inactive
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            if (m_Active)
                LabelTo(from, "{0} (Active; Level: {1}; Kills: {2}/{3})", this.m_Type, this.Level, this.m_Kills, MaxKills);
            else
                LabelTo(from, "{0} (Inactive)", this.m_Type);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
                from.SendGump(new PropertiesGump(from, this));
        }

        public override void OnLocationChange(Point3D oldLoc)
        {
            if (Deleted)
                return;

            if (this.m_Altar != null)
                this.m_Altar.Location = new Point3D(X, Y, Z);

            if (this.m_Idol != null)
                this.m_Idol.Location = new Point3D(X, Y, Z);

            this.m_SpawnArea.X += Location.X - oldLoc.X;
            this.m_SpawnArea.Y += Location.Y - oldLoc.Y;

            UpdateRegion();
        }

        public override void OnMapChange()
        {
            if (Deleted)
                return;

            if (this.m_Altar != null)
                this.m_Altar.Map = Map;

            if (this.m_Idol != null)
                this.m_Idol.Map = Map;

            UpdateRegion();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

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

            if (this.m_Altar != null)
                this.m_Altar.Delete();

            if (this.m_Idol != null)
                this.m_Idol.Delete();

            if (this.m_Champion != null && !this.m_Champion.Player)
                this.m_Champion.Delete();
            if (this.m_Champion2 != null && !this.m_Champion2.Player)
                this.m_Champion2.Delete();

            _Spawners.Remove(this);

            Stop();

            UpdateRegion();
        }

        public MiniChamp(Serial serial)
            : base(serial)
        {
        }

        public virtual void RegisterDamageTo(Mobile m)
        {
            if (m == null)
                return;

            foreach (DamageEntry de in m.DamageEntries)
            {
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

                if (totalDamage > randomDamage)
                {
                    GiveArtifact(kvp.Key, artifact);
                    break;
                }
            }
        }

        public void GiveArtifact(Mobile to, Item artifact)
        {
            if (to == null || artifact == null)
                return;

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

            writer.Write(this.m_ExpireDelay);
            writer.WriteDeltaTime(this.m_ExpireTime);

            writer.Write(this.m_Champion2);

            writer.Write(this.m_DamageEntries.Count);
            foreach (KeyValuePair<Mobile, int> kvp in m_DamageEntries)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }

            writer.Write(this.m_ConfinedRoaming);
            writer.Write(this.m_HasBeenAdvanced);
            writer.Write(this.m_SpawnArea);
            writer.Write(this.m_RandomizeType);
            writer.Write(this.m_Kills);
            writer.Write(Spwnd);

            writer.Write((bool)this.m_Active);
            writer.Write((int)this.m_Type);
            writer.Write(this.m_Creatures, true);
            writer.Write(this.m_Champion);
            writer.Write(this.m_RestartDelay);

            writer.Write(this.m_RestartTimer != null);

            if (this.m_RestartTimer != null)
                writer.WriteDeltaTime(this.m_RestartTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            m_DamageEntries = new Dictionary<Mobile, int>();

            int version = reader.ReadInt();

            switch (version)
            {
                case 7:
                    {
                        this.m_ExpireDelay = reader.ReadTimeSpan();
                        this.m_ExpireTime = reader.ReadDeltaTime();

                        goto case 6;
                    }
                case 6:
                    {
                        this.m_Champion2 = reader.ReadMobile();

                        goto case 5;
                    }
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
                        this.m_Kills = reader.ReadInt();
                        Spwnd = reader.ReadInt();

                        goto case 0;
                    }
                case 0:
                    {
                        bool active = reader.ReadBool();
                        this.m_Type = (MiniChampType)reader.ReadInt();
                        this.m_Creatures = reader.ReadStrongMobileList();
                        this.m_Champion = reader.ReadMobile();
                        this.m_RestartDelay = reader.ReadTimeSpan();

                        if (reader.ReadBool())
                        {
                            this.m_RestartTime = reader.ReadDeltaTime();
                            BeginRestart(this.m_RestartTime - DateTime.Now);
                        }

                        if (active)
                            this.Start();

                        break;
                    }
            }

            _Spawners.Add(this);

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(UpdateRegion));
        }
    }

    public class MiniChampRegion : BaseRegion
    {
        public override bool YoungProtected
        {
            get
            {
                return false;
            }
        }

        private readonly MiniChamp m_Spawn;

        public MiniChamp MiniChamp
        {
            get
            {
                return this.m_Spawn;
            }
        }

        public MiniChampRegion(MiniChamp spawn)
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

    public class IdolOfTheMiniChamp : Item
    {
        private MiniChamp m_Spawn;

        public MiniChamp Spawn { get { return m_Spawn; } }

        public override string DefaultName
        {
            get 
            {
                if (Spawn != null)
                {
                    MiniChampInfo info = MiniChampInfo.GetInfo(Spawn.Type);

                    if (info != null)
                        return info.Name;
                }

                return "Idol of the Champion";
            }
        }

        public IdolOfTheMiniChamp(MiniChamp spawn) : base(0x1F18)
        {
            this.m_Spawn = spawn;
            Movable = false;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_Spawn != null)
                this.m_Spawn.Delete();
        }

        public IdolOfTheMiniChamp(Serial serial) : base(serial)
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

            switch (version)
            {
                case 0:
                    {
                        this.m_Spawn = reader.ReadItem() as MiniChamp;

                        if (this.m_Spawn == null)
                            Delete();

                        break;
                    }
            }
        }
    }
}