using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using System.Collections.Generic;
using Server.Network;
using Server.Guilds;
using System.Linq;
using Server.Engines.Points;
using Server.Engines.CityLoyalty;
using Server.SkillHandlers;
using Server.Multis;
using Server.Regions;

namespace Server.Engines.VvV
{
    public enum UpdateType
    {
        Kill,
        Assist,
        Steal,
        TurnInVice,
        TurnInVirtue, 
        Disarm
    }

    [PropertyObject]
    public class VvVBattle
    {
        public static readonly int Duration = 20;
        public static readonly int Cooldown = 5;
        public static readonly int Announcement = 2;
        public static readonly int KillCooldownDuration = 5;
        public static readonly int MaxTraps = 20;
        public static readonly int MaxTurrets = 10;

        //Battle Scoring
        public static readonly double ScoreToWin = 10000;
        public static readonly double OccupyPoints = 300;
        public static readonly double AltarPoints = 1000;
        public static readonly double KillPoints = 600;
        public static readonly double TurnInPoints = 500;

        public static readonly int AltarSilver = 100;
        public static readonly int TurnInSilver = 100;
        public static readonly int KillSilver = 50;
        public static readonly int WinSilver = 100;
        public static readonly int DisarmSilver = 50;

        [CommandProperty(AccessLevel.GameMaster)]
        public ViceVsVirtueSystem System { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime StartTime { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime CooldownEnds { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastOccupationCheck { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextSigilSpawn { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextAnnouncement { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextAltarActivate { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextManaSpike { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ManaSpikeEndEffects { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public VvVCity City { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public VvVSigil Sigil { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool OnGoing { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Region Region
        {
            get
            {
                CityInfo info = CityInfo.Infos[City];

                if (info != null)
                {
                    return info.Region;
                }

                return null;
            }

        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ForceStart
        {
            get { return false; }
            set
            {
                if (!OnGoing && value)
                    Begin();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ForceEnd
        {
            get { return false; }
            set
            {
                if (OnGoing && value)
                    EndBattle();
            }
        }

        public Dictionary<Guild, VvVGuildBattleStats> GuildStats { get; set; }
        public Dictionary<Mobile, DateTime> KillCooldown { get; set; }
        public List<Guild> Participants { get; set; }
        public List<string> Messages { get; set; }

        public List<VvVAltar> Altars { get; set; }
        public int AltarIndex { get; set; }

        public List<VvVTrap> Traps { get; set; }
        public List<CannonTurret> Turrets { get; set; }
        public List<Mobile> Warned { get; set; }

        public VvVPriest VicePriest { get; private set; }
        public VvVPriest VirtuePriest { get; private set; }

        public Timer Timer { get; private set; }

        public int TrapCount { get { return Traps.Where(t => !t.Deleted).Count(); } }
        public int TurretCount { get { return Turrets.Where(t => !t.Deleted).Count(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool InCooldown
        {
            get { return CooldownEnds > DateTime.UtcNow; }
        }

        public VvVBattle(ViceVsVirtueSystem sys)
        {
            System = sys;

            Participants = new List<Guild>();
            GuildStats = new Dictionary<Guild, VvVGuildBattleStats>();
            KillCooldown = new Dictionary<Mobile, DateTime>();
            Messages = new List<string>();
            Altars = new List<VvVAltar>();
            Traps = new List<VvVTrap>();
            Warned = new List<Mobile>();
            Turrets = new List<CannonTurret>();
        }

        public void Begin()
        {
            VvVCity newCity = City;
            OnGoing = true;

            do
            {
                newCity = (VvVCity)Utility.Random(8);
            }
            while (newCity == City);

            City = newCity;

            BeginTimer();

            StartTime = DateTime.UtcNow;
            NextSigilSpawn = DateTime.UtcNow + TimeSpan.FromMinutes(Utility.RandomMinMax(1, 3));

            AltarIndex = 0;
            SpawnAltars();
            SpawnPriests(false);

            if (Region is GuardedRegion)
            {
                GuardedRegion.Disable((GuardedRegion)Region);
            }

            NextAltarActivate = DateTime.UtcNow + TimeSpan.FromMinutes(1);

            System.SendVvVMessage(1154721, String.Format("#{0}", ViceVsVirtueSystem.GetCityLocalization(City).ToString())); 
            // A Battle between Vice and Virtue is active! To Arms! The City of ~1_CITY~ is besieged!
        }

        public void SpawnAltars()
        {
            foreach (Point3D p in CityInfo.Infos[City].AltarLocs)
            {
                VvVAltar altar = new VvVAltar(this);
                altar.MoveToWorld(p, Map.Felucca);

                Altars.Add(altar);
            }
        }

        public void SpawnPriests(bool movetoworld = true)
        {
            if(VicePriest == null || VicePriest.Deleted)
                VicePriest = new VvVPriest(VvVType.Vice, this);

            if(VirtuePriest == null || VirtuePriest.Deleted)
                VirtuePriest = new VvVPriest(VvVType.Virtue, this);

            if (movetoworld)
            {
                Point3D p;

                do
                {
                    p = CityInfo.Infos[City].PriestLocation.GetRandomSpawnPoint(Map.Felucca);
                }
                while (!Map.Felucca.CanSpawnMobile(p));

                VicePriest.MoveToWorld(p, Map.Felucca);

                do
                {
                    p = CityInfo.Infos[City].PriestLocation.GetRandomSpawnPoint(Map.Felucca);
                }
                while (!Map.Felucca.CanSpawnMobile(p));

                VirtuePriest.MoveToWorld(p, Map.Felucca);
            }
        }

        public void BeginTimer()
        {
            EndTimer();

            Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnTick);
            Timer.Start();
        }

        public void EndTimer()
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }
        }

        public void OnTick()
        {
            if (!OnGoing)
                return;

            CheckParticipation();
            UpdateAllGumps(false);

            if (StartTime + TimeSpan.FromMinutes(Duration) < DateTime.UtcNow)
            {
                EndBattle();
                return;
            }

            if (LastOccupationCheck + TimeSpan.FromMinutes(1) < DateTime.UtcNow)
            {
                CheckOccupation();

                LastOccupationCheck = DateTime.UtcNow;
            }

            if (NextAltarActivate != DateTime.MinValue && NextAltarActivate < DateTime.UtcNow)
            {
                ActivateAltar();
                NextAltarActivate = DateTime.MinValue;
            }

            if (NextSigilSpawn != DateTime.MinValue && NextSigilSpawn < DateTime.UtcNow)
            {
                if (Sigil == null || Sigil.Deleted)
                {
                    SpawnSigil();
                }
            }

            ActivateArrows();

            List<Mobile> list = KillCooldown.Keys.Where(mob => KillCooldown[mob] < DateTime.UtcNow).ToList();

            foreach (Mobile m in list)
            {
                KillCooldown.Remove(m);
            }

            if (OnGoing)
            {
                Turrets.ForEach(t => { t.Scan(); });
            }

            list.Clear();
            list.TrimExcess();
        }

        public void CheckParticipation()
        {
            if (Region == null)
                return;

            foreach (PlayerMobile pm in this.Region.GetEnumeratedMobiles().OfType<PlayerMobile>())
            {
                bool vvv = ViceVsVirtueSystem.IsVvV(pm);

                if (!vvv && !Warned.Contains(pm))
                {
                    pm.SendGump(new BattleWarningGump(pm));
                    Warned.Add(pm);
                    return;
                }
                else if (pm.Alive && !pm.Hidden && BaseBoat.FindBoatAt(pm.Location, pm.Map) == null && BaseHouse.FindHouseAt(pm) == null)
                {
                    Guild g = pm.Guild as Guild;

                    if (pm.Alive && !pm.Hidden && g != null && !Participants.Contains(g) && vvv)
                    {
                        Participants.Add(g);
                    }
                }
            }
        }

        public void EndBattle()
        {
            EndTimer();

            if (Region is GuardedRegion)
            {
                ((GuardedRegion)Region).Disabled = false;
            }

            CooldownEnds = DateTime.UtcNow + TimeSpan.FromMinutes(Cooldown);
            ViceVsVirtueSystem.Instance.OnBattleEnd();

            foreach (VvVAltar altar in Altars)
            {
                if(!altar.Deleted)
                    altar.Delete();
            }

            foreach (VvVTrap trap in Traps)
            {
                if(!trap.Deleted)
                    trap.Delete();
            }

            foreach (CannonTurret turret in Turrets)
            {
                if (!turret.Deleted)
                    turret.Delete();
            }

            if (VicePriest != null)
            {
                VicePriest.Delete();
                VicePriest = null;
            }

            if (VirtuePriest != null)
            {
                VirtuePriest.Delete();
                VirtuePriest = null;
            }

            if (Sigil != null)
            {
                Sigil.Delete();
                Sigil = null;
            }

            Guild leader = GetLeader();

            foreach (Mobile m in this.Region.GetEnumeratedMobiles())
            {
                Guild g = m.Guild as Guild;

                if (leader != null && (g == leader || leader.IsAlly(g)))
                {
                    System.AwardPoints(m, WinSilver + (OppositionCount(g) * 50), message: false);
                }
            }

            SendBattleStatsGump();

            System.SendVvVMessage(1154722); // A VvV battle has just concluded. The next battle will begin in less than five minutes!

            Altars.Clear();
            GuildStats.Clear();
            KillCooldown.Clear();
            Participants.Clear();
            Participants.TrimExcess();
            Messages.Clear();
            Messages.TrimExcess();
            Traps.Clear();
            Traps.TrimExcess();
            Warned.Clear();
            Warned.TrimExcess();
            Turrets.Clear();
            Turrets.TrimExcess();

            if (Region is GuardedRegion)
            {
                ((GuardedRegion)Region).Disabled = false;
            }

            OnGoing = false;

            NextSigilSpawn = DateTime.MinValue;
            LastOccupationCheck = DateTime.MinValue;
            NextAnnouncement = DateTime.MinValue;
            StartTime = DateTime.MinValue;
            NextAltarActivate = DateTime.MinValue;
            ManaSpikeEndEffects = DateTime.MinValue;
            NextManaSpike = DateTime.MinValue;
        }

        public void SpawnSigil()
        {
            Point3D p = CityInfo.Infos[City].SigilLocs[Utility.Random(CityInfo.Infos[City].SigilLocs.Length)];
            Sigil = new VvVSigil(this, p);

            Sigil.MoveToWorld(p, Map.Felucca);

            UpdateAllGumps();
        }

        public void ActivateAltar()
        {
            if (AltarIndex == 2)
                AltarIndex = 0;
            else
                AltarIndex++;

            Altars[AltarIndex].Activate();
            ActivateArrows();

            SendStatusMessage("Fight for the altar!", true);
        }

        public void CheckArrow(PlayerMobile pm)
        {
            if (pm.NetState == null)
                return;

            foreach (VvVAltar altar in Altars)
            {
                if (altar.IsActive)
                {
                    pm.QuestArrow = new AltarArrow(pm, altar);
                    break;
                }
            }
        }

        public void ActivateArrows()
        {
            foreach (PlayerMobile pm in this.Region.GetEnumeratedMobiles().OfType<PlayerMobile>())
            {
                if (pm.NetState != null && pm.QuestArrow == null)
                {
                    foreach (VvVAltar altar in Altars)
                    {
                        if (altar.IsActive)
                        {
                            pm.QuestArrow = new AltarArrow(pm, altar);
                            break;
                        }
                    }
                }
            }
        }

        public VvVGuildBattleStats GetGuildStats(Guild g)
        {
            if (g == null)
                return null;

            if (!GuildStats.ContainsKey(g))
                GuildStats[g] = new VvVGuildBattleStats(g);

            return GuildStats[g];
        }

        public void Update(Mobile m, UpdateType type)
        {
            VvVPlayerEntry entry = System.GetPlayerEntry<VvVPlayerEntry>(m);

            if (entry != null)
            {
                Update(null, entry, type);
            }
        }

        public void Update(VvVPlayerEntry victim, VvVPlayerEntry killer, UpdateType type)
        {
            VvVGuildBattleStats killerStats = GetGuildStats(killer.Guild);
            VvVGuildBattleStats victimStats = victim == null ? null : GetGuildStats(victim.Guild);

            switch (type)
            {
                case UpdateType.Kill:
                    if (killer != null) killer.Kills++;
                    if (victim != null) victim.Deaths++;
                    if (killerStats != null) killerStats.Kills++;
                    if (victimStats != null) victimStats.Deaths++;

                    if (victim != null && victim.Player != null)
                    {
                        if (!KillCooldown.ContainsKey(victim.Player) || KillCooldown[victim.Player] < DateTime.UtcNow)
                        {
                            if (killerStats != null) 
                                killerStats.Points += KillPoints;

                            SendStatusMessage(String.Format("{0} has killed {1}!", killer.Player.Name, victim.Player.Name));
                            killerStats.Silver += KillSilver + (OppositionCount(killer.Guild) * 50);
                            KillCooldown[victim.Player] = DateTime.UtcNow + TimeSpan.FromMinutes(KillCooldownDuration);
                        }
                    }

                    break;
                case UpdateType.Assist:
                    if(killer != null) 
                        killer.Assists++;

                    if (killerStats != null) 
                        killerStats.Assists++;
                    break;
                case UpdateType.Steal:
                    if (killerStats != null)
                    {
                        killer.StolenSigils++;
                        killerStats.Stolen++;
                        SendStatusMessage(String.Format("{0} has stolen the sigil!", killer.Player.Name));
                    }
                    break;
                case UpdateType.TurnInVice:
                    if (killerStats != null) 
                    {
                        killer.ReturnedSigils++;
                        killerStats.ViceReturned++; 
                        killerStats.Points += TurnInPoints;
                        killerStats.Silver += TurnInSilver + (OppositionCount(killer.Guild) * 50);
                        SendStatusMessage(String.Format("{0} has returned the sigil!", killer.Player.Name));
                        NextSigilSpawn = DateTime.UtcNow + TimeSpan.FromMinutes(1);
                    }
                    RemovePriests();
                    break;
                case UpdateType.TurnInVirtue:
                    if (killerStats != null)
                    {
                        killer.ReturnedSigils++;
                        killerStats.VirtueReturned++; 
                        killerStats.Points += TurnInPoints;
                        killerStats.Silver += TurnInSilver + (OppositionCount(killer.Guild) * 50);
                        SendStatusMessage(String.Format("{0} has returned the sigil!", killer.Player.Name));
                        NextSigilSpawn = DateTime.UtcNow + TimeSpan.FromMinutes(1);
                    }
                    RemovePriests();
                    break;
                case UpdateType.Disarm:
                    if (killerStats != null)
                    {
                        killerStats.Disarmed++;
                        killer.DisarmedTraps++;
                        killerStats.Silver += DisarmSilver + (OppositionCount(killer.Guild) * 50);
                        SendStatusMessage(String.Format("{0} has disarmed a trap!", killer.Player.Name));
                    }
                    break;
            }

            CheckScore();
        }

        public void RemovePriests()
        {
            Timer.DelayCall(TimeSpan.FromSeconds(4), () =>
                {
                    if (VicePriest != null)
                        VicePriest.Internalize();

                    if (VirtuePriest != null)
                        VirtuePriest.Internalize();
                });
        }

        public void OccupyAltar(Guild g)
        {
            VvVGuildBattleStats killerStats = GetGuildStats(g);

            foreach (KeyValuePair<Guild, VvVGuildBattleStats> kvp in GuildStats)
            {
                Guild guild = kvp.Key;

                if (guild == g || guild.IsAlly(g))
                {
                    VvVGuildBattleStats stats = GetGuildStats(guild);

                    if (stats != null)
                    {
                        stats.Points += AltarPoints;
                        stats.Silver += AltarSilver + (OppositionCount(g) * 50);
                    }
                }
            }

            SendStatusMessage(String.Format("{0} claimed the altar!", g.Abbreviation));

            foreach (PlayerMobile pm in Region.GetEnumeratedMobiles().OfType<PlayerMobile>())
            {
                if (pm.QuestArrow != null)
                    pm.QuestArrow = null;
            }

            CheckScore();
            NextAltarActivate = DateTime.UtcNow + TimeSpan.FromMinutes(2);
        }

        public void CheckOccupation()
        {
            if (!OnGoing)
                return;

            if (Participants.Count == 1)
            {
                Guild g = Participants[0];

                if (!GuildStats.ContainsKey(g))
                    GuildStats[g] = new VvVGuildBattleStats(g);

                GuildStats[g].Points += OccupyPoints;
                UpdateAllGumps();
                CheckScore();

                if (OnGoing && NextAnnouncement < DateTime.UtcNow)
                {
                    System.SendVvVMessage(1154957, g.Name); // ~1_NAME~ is occupying the city!
                    NextAnnouncement = DateTime.UtcNow + TimeSpan.FromMinutes(Announcement);
                }
            }
            else // Is this a bug?  Verified on EA this is how it behaves
            {
                if (NextAnnouncement < DateTime.UtcNow)
                {
                    System.SendVvVMessage(1154958); // The City is unoccupied! Slay opposing forces to claim the city for your guild!
                    NextAnnouncement = DateTime.UtcNow + TimeSpan.FromMinutes(Announcement);
                }
            }
        }

        public void CheckScore()
        {
            int score;
            Guild leader = GetLeader(out score);

            if (score >= ScoreToWin)
            {
                EndBattle();
                return;
            }

            UpdateAllGumps();
        }

        public Guild GetLeader()
        {
            int score;
            return GetLeader(out score);
        }

        /// <summary>
        /// Gets real time leader of battle, out parameter of their score
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public Guild GetLeader(out int score)
        {
            Guild leader = null;
            score = 0;

            foreach (Guild g in Participants)
            {
                if(!GuildStats.ContainsKey(g))
                    GuildStats[g] = new VvVGuildBattleStats(g);

                if (leader == null || GuildStats[g].Points > score)
                {
                    leader = g;
                    score = (int)GuildStats[g].Points;
                }
            }

            return leader;
        }

        /// <summary>
        /// Gets all allied participating guilds
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public List<Guild> GetAlliance(Guild g)
        {
            if (g == null)
                return null;

            List<Guild> alliance = new List<Guild>();

            foreach (Guild gu in Participants.Where(guild => guild == g || g.IsAlly(guild)))
            {
                alliance.Add(gu);
            }

            return alliance;
        }

        /// <summary>
        /// Indicates whether a guild has an alliance
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public bool HasAlliance(Guild g)
        {
            foreach (Guild guild in GuildStats.Keys)
            {
                if (g.IsAlly(guild))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns enemy count, by alliance
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public int OppositionCount(Guild g)
        {
            List<Guild> exempt = new List<Guild>();
            int count = 0;

            foreach (Guild G in Participants.Where(gui => gui != g && !gui.IsAlly(g)))
            {
                if (exempt.Contains(G))
                    continue;

                count++;

                if (G.Alliance != null)
                {
                    foreach (Guild guil in G.Alliance.Members.Where(guil => !exempt.Contains(guil)))
                        exempt.Add(guil);
                }
            }

            exempt.Clear();
            exempt.TrimExcess();

            return count;
        }

        /// <summary>
        /// Friendly guild count
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public int FriendlyCount(Guild g)
        {
            List<Guild> guilds = Participants;
            int count = 0;

            foreach (Guild gu in Participants.Where(gui => gui != g && gui.IsAlly(g)))
            {
                count++;
            }

            guilds.Clear();
            guilds.TrimExcess();

            return count;
        }

        public bool IsInActiveBattle(Mobile one, Mobile two)
        {
            return IsInActiveBattle(one) && IsInActiveBattle(two);
        }

        public bool IsInActiveBattle(Mobile m)
        {
            if (!OnGoing)
                return false;

            Region r = Region.Find(m.Location, m.Map);

            return r == this.Region;
        }

        public void OnEnterRegion(Mobile m)
        {
            if (m is PlayerMobile && OnGoing)
            {
                if (m.HasGump(typeof(VvVBattleStatusGump)))
                    m.CloseGump(typeof(VvVBattleStatusGump));

                m.SendGump(new VvVBattleStatusGump((PlayerMobile)m, this));
            }
        }

        public void CheckGump(Mobile m)
        {
            if (m is PlayerMobile && OnGoing)
            {
                m.SendGump(new VvVBattleStatusGump((PlayerMobile)m, this));
            }
        }

        public void UpdateAllGumps(bool recompile = true)
        {
            if (Region == null)
                return;

            foreach (PlayerMobile m in this.Region.GetEnumeratedMobiles().OfType<PlayerMobile>())
            {
                if (!ViceVsVirtueSystem.IsVvV(m) || m.NetState == null)
                    continue;

                VvVBattleStatusGump g = m.FindGump(typeof(VvVBattleStatusGump)) as VvVBattleStatusGump;

                if (g == null)
                {
                    m.SendGump(new VvVBattleStatusGump(m, this));
                }
                else
                {
                    g.Refresh(recompile);
                }
            }
        }

        public void SendBattleStatsGump()
        {
            if (Region == null)
                return;

            foreach (PlayerMobile m in this.Region.GetEnumeratedMobiles().OfType<PlayerMobile>())
            {
                if (ViceVsVirtueSystem.IsVvV(m))
                {
                    m.CloseGump(typeof(VvVBattleStatusGump));
                    m.SendGump(new BattleStatsGump((PlayerMobile)m, this));
                }
            }
        }

        public void SendStatusMessage(string message, bool sendgumps = false)
        {
            Messages.Add(message);

            if(sendgumps)
                UpdateAllGumps();
        }

        public void AddCannonTurret(CannonTurret turret)
        {
            if (!Turrets.Contains(turret))
                Turrets.Add(turret);
        }

        public VvVBattle(GenericReader reader, ViceVsVirtueSystem system)
        {
            int version = reader.ReadInt();
            System = system;

            Altars = new List<VvVAltar>();
            GuildStats = new Dictionary<Guild, VvVGuildBattleStats>();
            KillCooldown = new Dictionary<Mobile, DateTime>();
            Participants = new List<Guild>();
            Messages = new List<string>();
            Traps = new List<VvVTrap>();
            Warned = new List<Mobile>();
            Turrets = new List<CannonTurret>();

            OnGoing = reader.ReadBool();

            if (reader.ReadInt() == 0)
            {
                StartTime = reader.ReadDateTime();
                CooldownEnds = reader.ReadDateTime();
                LastOccupationCheck = reader.ReadDateTime();
                NextSigilSpawn = reader.ReadDateTime();
                NextAnnouncement = reader.ReadDateTime();
                NextAltarActivate = reader.ReadDateTime();
                City = (VvVCity)reader.ReadInt();
                Sigil = reader.ReadItem() as VvVSigil;
                VicePriest = reader.ReadMobile() as VvVPriest;
                VirtuePriest = reader.ReadMobile() as VvVPriest;

                if (Sigil != null)
                    Sigil.Battle = this;

                if (VicePriest != null)
                    VicePriest.Battle = this;

                if (VirtuePriest != null)
                    VirtuePriest.Battle = this;

                int count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    VvVAltar altar = reader.ReadItem() as VvVAltar;
                    if (altar != null)
                    {
                        altar.Battle = this;
                        Altars.Add(altar);
                    }
                }

                count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    Guild g = reader.ReadGuild() as Guild;
                    VvVGuildBattleStats stats = new VvVGuildBattleStats(reader, g);

                    if (g != null)
                        GuildStats[g] = stats;
                }

                count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    VvVTrap t = reader.ReadItem() as VvVTrap;

                    if (t != null)
                        Traps.Add(t);
                }

                Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
                    {
                        if (Region is GuardedRegion)
                        {
                            GuardedRegion.Disable((GuardedRegion)Region);
                        }

                        BeginTimer();

                        ActivateArrows();
                    });
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(OnGoing);

            if (OnGoing)
            {
                writer.Write(0);
                writer.Write(StartTime);
                writer.Write(CooldownEnds);
                writer.Write(LastOccupationCheck);
                writer.Write(NextSigilSpawn);
                writer.Write(NextAnnouncement);
                writer.Write(NextAltarActivate);
                writer.Write((int)City);
                writer.Write(Sigil);
                writer.Write(VicePriest);
                writer.Write(VirtuePriest);

                writer.Write(Altars.Count);
                Altars.ForEach(altar => writer.Write(altar));

                writer.Write(GuildStats.Count);
                foreach (KeyValuePair<Guild, VvVGuildBattleStats> kvp in GuildStats)
                {
                    writer.Write(kvp.Key);
                    kvp.Value.Serialize(writer);
                }

                writer.Write(Traps.Count);
                Traps.ForEach(t => writer.Write(t));
            }
            else
                writer.Write(1);
            
        }
    }
}