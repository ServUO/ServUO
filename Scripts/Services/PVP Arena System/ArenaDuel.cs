using Server;
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using System.Linq;
using Server.Network;
using Server.Gumps;

namespace Server.Engines.ArenaSystem
{
    public enum RoomType
    {
        Public,
        Private
    }

    public enum BattleMode
    {
        Survival,
        Team
    }

    public enum TimeLimit
    {
        FiveMinutes = 5,
        TenMinutes = 10,
        FifteenMinutes = 15,
        TwentyMinutes = 20,
        ThirtyMinutes = 30
    }

    public enum EntryFee
    {
        Zero,
        OneThousand = 1000,
        FiveThousand = 5000,
        TenThousand = 10000,
        TwentyFiveThousand = 25000,
        FiftyThousand = 50000
    }

    public enum PotionRules
    {
        All,
        None, 
        NoHealing
    }

    [PropertyObject]
    public class ArenaDuel
    {
        public const int MaxEntries = 10;
        public const int MaxPetSlots = 5;

        public static TimeSpan KickTime = TimeSpan.FromSeconds(40);
        public static TimeSpan EntryTime = TimeSpan.FromSeconds(90);
        public static TimeSpan StartTimeDuration = TimeSpan.FromSeconds(5);

        public PVPArena Arena { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Entries { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public RoomType RoomType { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BattleMode BattleMode { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Ranked { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeLimit TimeLimit { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public EntryFee EntryFee { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PetSlots { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RidingFlyingAllowed { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RangedWeaponsAllowed { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool SummonSpellsAllowed { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool FieldSpellsAllowed { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public PotionRules PotionRules { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile Host { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Pot { get; set; }

        public List<ArenaTeam> Teams { get; set; }
        public List<PlayerMobile> Banned { get; set; }
        public Dictionary<string, string> KillRecord { get; set; }

        public ArenaGate Gate { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasBegun { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Complete { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool InPreFight { get { return EntryDeadline != DateTime.MinValue; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime StartTime { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime EntryDeadline { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime EndTime { get { return StartTime + TimeSpan.FromMinutes((int)TimeLimit); } }

        // used in team mode
        public ArenaTeam TeamOrder { get { return Teams != null && Teams.Count > 0 ? Teams[0] : null; } }
        public ArenaTeam TeamChaos { get { return Teams != null && Teams.Count > 1 ?Teams[1] : null; } }

        public int ParticipantCount
        {
            get { return Teams.Sum(t => t.Count); }
        }

        public ArenaDuel(PVPArena arena, PlayerMobile host)
        {
            Host = host;

            Entries = 2;
            RoomType = RoomType.Public;
            Arena = arena;
            BattleMode = BattleMode.Team;
            Ranked = true;
            TimeLimit = TimeLimit.TenMinutes;
            EntryFee = EntryFee.Zero;
            PetSlots = 5;
            RidingFlyingAllowed = true;
            RangedWeaponsAllowed = true;
            SummonSpellsAllowed = true;
            FieldSpellsAllowed = true;
            PotionRules = PotionRules.All;

            Teams = new List<ArenaTeam>();
            KillRecord = new Dictionary<string, string>();
            Banned = new List<PlayerMobile>();

            Teams.Add(new ArenaTeam(host));
            Teams.Add(new ArenaTeam());

            Complete = false;
        }

        public override string ToString()
        {
            return "...";
        }

        public void SetDuel()
        {
            PVPArenaSystem.SendMessage(Host, 1115800); // You have created a new duel session.

            Teams[0].AddParticipant(Host);
        }

        public IEnumerable<KeyValuePair<PlayerMobile, PlayerStatsEntry>> GetParticipants(bool inArena = false)
        {
            foreach (var team in Teams)
            {
                foreach (var player in team.Players.Where(p => !inArena || InArena(p.Key)))
                {
                    yield return player;
                }
            }
        }

        public List<PlayerMobile> ParticipantList()
        {
            List<PlayerMobile> list = new List<PlayerMobile>();

            foreach (var part in GetParticipants())
            {
                list.Add(part.Key);
            }

            return list;
        }

        public bool InArena(Mobile m)
        {
            if (m == null)
                return false;

            return Region.Find(m.Location, m.Map) == Arena.Region;
        }

        public PlayerStatsEntry GetStats(PlayerMobile check)
        {
            foreach (var kvp in GetParticipants())
            {
                if (kvp.Key == check)
                    return kvp.Value;
            }

            return null;
        }

        public void SwapParticipant(PlayerMobile pm)
        {
            if (HasBegun)
                return;

            if (TeamOrder.RemoveParticipant(pm))
            {
                TeamChaos.AddParticipant(pm);
            }
            else if (TeamChaos.RemoveParticipant(pm))
            {
                TeamOrder.AddParticipant(pm);
            }
        }

        public bool TryAddPlayer(PlayerMobile pm)
        {
            if (ParticipantCount >= MaxEntries)
            {
                pm.SendLocalizedMessage(1115954); // This session is already full.
            }
            else if (InPreFight || HasBegun)
            {
                pm.SendLocalizedMessage(1115953); // This session has already been closed.
            }
            else if (!Arena.PendingDuels.ContainsKey(this))
            {
                pm.SendLocalizedMessage(1115957); // This session has expired. Please create a new session and try again.
            }
            else if (pm.Map != Arena.Definition.Map)
            {
                pm.SendLocalizedMessage(1115959); // You need to return to the facet that you opened the session on to start this duel.
            }
            else
            {
                RegisterPlayer(pm);
                return true;
            }

            return false;
        }

        public void RegisterPlayer(PlayerMobile pm)
        {
            PVPArenaSystem.SendParticipantMessage(this, 1115956); // A new duelist has joined your session!
            PVPArenaSystem.SendMessage(pm, 1115955); // You have joined the session.

            if (BattleMode == BattleMode.Team)
            {
                if (TeamOrder.Count > TeamChaos.Count)
                {
                    TeamChaos.AddParticipant(pm);
                }
                else
                {
                    TeamOrder.AddParticipant(pm);
                }
            }
            else
            {
                if (Teams[Teams.Count - 1].Unoccupied)
                {
                    Teams[Teams.Count - 1].AddParticipant(pm);
                }
                else
                {
                    Teams.Add(new ArenaTeam(pm));
                }
            }
        }

        public void RemovePlayer(PlayerMobile pm, bool ban = false)
        {
            var team = GetTeam(pm);

            if (team != null)
            {
                team.RemoveParticipant(pm);

                if (Teams[1] != team && team.Unoccupied)
                {
                    Teams.Remove(team);
                }

                if (ban)
                {
                    Banned.Add(pm);

                    PVPArenaSystem.SendMessage(Host, 1115951); // You have banned the specified participant.
                    PVPArenaSystem.SendParticipantMessage(this, 1115951); // You have been banned from the session by the host player.
                }
                else
                {
                    PVPArenaSystem.SendMessage(pm, 1115948); // One of the participants has left your duel session.
                    PVPArenaSystem.SendParticipantMessage(this, 1115948); // One of the participants has left your duel session.
                }
            }
        }

        public bool IsParticipant(PlayerMobile pm)
        {
            return GetTeam(pm) != null;
        }

        public bool IsBanned(PlayerMobile pm)
        {
            return Banned.Contains(pm);
        }

        public ArenaTeam GetTeam(PlayerMobile pm)
        {
            return Teams.FirstOrDefault(team => team.Contains(pm));
        }

        public void MoveToArena(PlayerMobile pm)
        {
            Map map = Arena.Definition.Map;
            Rectangle2D rec = _StartPoints[pm];

            Point3D p = rec.GetRandomSpawnPoint(map);

            BaseCreature.TeleportPets(pm, p, map);
            pm.MoveToWorld(p, Arena.Definition.Map);
            pm.SendSpeedControl(SpeedControlType.NoMove);

            if (EntryFee > EntryFee.Zero)
            {
                int fee = (int)EntryFee;

                if (pm.Backpack != null && pm.Backpack.ConsumeTotal(typeof(Gold), fee))
                {
                    pm.SendLocalizedMessage(1149610); // You have paid the entry fee from your backpack.
                }
                else
                {
                    Banker.Withdraw(pm, (int)EntryFee, true);
                    pm.SendLocalizedMessage(1149609); // You have paid the entry fee from your bank account.
                }

                Pot += (int)EntryFee;
            }

            bool allin = true;

            foreach (var team in Teams)
            {
                foreach (var player in team.Players.Keys)
                {
                    if (!InArena(player))
                    {
                        allin = false;
                        break;
                    }
                }
            }

            if (allin)
            {
                BeginDuel();
            }
        }

        public void OnPlayerLeave(PlayerMobile pm)
        {
            if (!Complete)
            {
                List<ArenaTeam> present = GetTeamsPresent();

                if (present.Count == 1)
                {
                    EndDuel(present[0]);
                }
                else if (present.Count == 0)
                {
                    CancelDuel();
                }

                ColUtility.Free(present);
            }
        }

        public void DoPreDuel()
        {
            HasBegun = true;

            AssignStartPoints();

            Gate = new ArenaGate(this);
            Gate.MoveToWorld(Arena.Definition.GateLocation, Arena.Definition.Map);

            PVPArenaSystem.SendParticipantMessage(this, 1115874); // The arena gate has opened near the arena stone. <br>You've ninety seconds to use the gate or you'll be removed from this duel.

            EntryDeadline = DateTime.UtcNow + EntryTime;
            PVPArenaSystem.SendParticipantMessage(this, 1115875);

            PlaceBlockers();
        }

        public void BeginDuel()
        {
            EntryDeadline = DateTime.MinValue;
            StartTime = DateTime.UtcNow;

            if (Gate != null)
            {
                Gate.Delete();
                Gate = null;
            }

            List<ArenaTeam> present = GetTeamsPresent();

            if (present.Count <= 1)
            {
                if (present.Count == 1)
                {
                    PVPArenaSystem.SendParticipantMessage(this, 1116476, true); // Your duel is about to begin but the opposing team has not came to the arena. This session will be aborted and you will be ejected from the arena.
                }

                CancelDuel();
                ColUtility.Free(present);

                return;
            }

            ColUtility.Free(present);

            PVPArenaSystem.SendParticipantMessage(this, 1115964, true); // The duel will start in 5 seconds!

            Timer.DelayCall(StartTimeDuration, () =>
                {
                    RemoveBlockers();
                    DoStartEffects();
                });
        }

        private Dictionary<PlayerMobile, Rectangle2D> _StartPoints;

        public void AssignStartPoints()
        {
            _StartPoints = new Dictionary<PlayerMobile, Rectangle2D>();

            var pm1 = Teams[0].PlayerZero;
            var pm2 = Teams[1].PlayerZero;

            _StartPoints[pm1] = Arena.Definition.StartLocations[0];
            _StartPoints[pm2] = Arena.Definition.StartLocations[1];

            List<PlayerMobile> partList = ParticipantList();

            foreach (var pm in partList.Where(p => p != pm1 && p != pm2))
            {
                _StartPoints[pm] = Arena.Definition.StartLocations[_StartPoints.Count];
            }
        }

        private List<ArenaTeam> GetTeamsPresent()
        {
            List<ArenaTeam> present = new List<ArenaTeam>();

            foreach (var part in GetParticipants(true))
            {
                var team = GetTeam(part.Key);

                if (!present.Contains(team))
                {
                    present.Add(team);
                }
            }

            return present;
        }

        public void PlaceBlockers()
        {
            foreach (var rec in Arena.Definition.EffectAreas)
            {
                for (int x = rec.X; x < rec.X + rec.Width; x++)
                {
                    for (int y = rec.Y; y < rec.Y + rec.Height; y++)
                    {
                        Item blocker = Arena.Blockers.FirstOrDefault(bl => !bl.Deleted && bl.Map == Map.Internal);

                        if (blocker == null)
                        {
                            blocker = new Blocker();
                            Arena.Blockers.Add(blocker);
                        }

                        blocker.MoveToWorld(new Point3D(x, y, Arena.Definition.Map.GetAverageZ(x, y)), Arena.Definition.Map);
                    }
                }
            }
        }

        public void RemoveBlockers()
        {
            foreach (var item in Arena.Blockers)
            {
                item.Internalize();
            }
        }

        public void DoStartEffects()
        {
            foreach (var rec in Arena.Definition.EffectAreas)
            {
                for (int x = rec.X; x < rec.X + rec.Width; x++)
                {
                    for (int y = rec.Y; y < rec.Y + rec.Height; y++)
                    {
                        Effects.SendLocationEffect(new Point3D(x, y, Arena.Definition.Map.GetAverageZ(x, y)), Arena.Definition.Map, 0x3709, 60, 10);
                    }
                }
            }
        }

        public void CancelDuel()
        {
            Timer.DelayCall(TimeSpan.FromSeconds(5), RemovePlayers);
            Complete = true;

            if (EntryFee > EntryFee.Zero)
            {
                foreach (var part in GetParticipants())
                {
                    Banker.Deposit(part.Key, (int)EntryFee, true);
                    PVPArenaSystem.SendMessage(part.Key, 1149606); // The entry fee has been refunded to your bank box.
                }
            }
        }

        public void EndDuel()
        {
            EndDuel(null); // TIE!
        }

        public void EndDuel(ArenaTeam winner)
        {
            Timer.DelayCall(KickTime, RemovePlayers);
            Complete = true;

            SendResults(winner);

            if (winner != null)
            {
                foreach (var pm in winner.Players.Keys)
                {
                    if (Pot > 0)
                    {
                        Banker.Deposit(pm, Pot / winner.Count, true);
                    }

                    PVPArenaSystem.SendMessage(pm, 1115975); // Congratulations! You have won the duel!
                }
            }

            foreach (var team in Teams.Where(t => t != winner))
            {
                foreach (var pm in team.Players.Keys)
                {
                    PVPArenaSystem.SendMessage(pm, team.Count == 1 ? 1116489 : 1116488); // You have lost the duel... : Your team has lost the duel...
                }
            }

            RecordStats(winner);

            if (Ranked)
            {
                Arena.RecordRankings(this, winner);
            }

            PVPArenaSystem.SendParticipantMessage(this, 1115965); // All participants will be ejected from this arena in 40 seconds.
        }

        public void RemovePlayers()
        {
            foreach (var part in GetParticipants(true))
            {
                Arena.RemovePlayer(part.Key);
            }

            PVPArenaSystem.SendParticipantMessage(this, 1115973); // Thank you for your participation! Please return to the arena stone for additional dueling opportunities!
            Arena.OnDuelEnd(this);

            Closeout();
        }

        public void RecordStats(ArenaTeam winner)
        {
            foreach (var kvp in GetParticipants(true))
            {
                var team = Teams.FirstOrDefault(t => t.Contains(kvp.Key));
                var stats = kvp.Value;

                if (winner == null)
                {
                    switch (BattleMode)
                    {
                        case BattleMode.Survival: stats.SurvivalDraws++; break;
                        case BattleMode.Team: stats.TeamDraws++; break;
                    }
                }
                else if (team == winner)
                {
                    switch (BattleMode)
                    {
                        case BattleMode.Survival: stats.SurvivalWins++; break;
                        case BattleMode.Team: stats.TeamWins++; break;
                    }
                }
                else
                {
                    switch (BattleMode)
                    {
                        case BattleMode.Survival: stats.SurvivalLosses++; break;
                        case BattleMode.Team: stats.TeamLosses++; break;
                    }
                }
            }
        }

        public void HandleDeath(Mobile victim)
        {
            Mobile killer = victim.LastKiller;

            if (killer is BaseCreature)
                killer = ((BaseCreature)killer).GetMaster();

            if (victim is PlayerMobile)
            {
                KillRecord[victim.Name] = killer != null ? killer.Name : "Unknown";

                if (killer is PlayerMobile && killer != victim) // if not ranked, does it keep track of this?
                {
                    PlayerStatsEntry victimEntry = GetStats((PlayerMobile)victim);
                    PlayerStatsEntry killerEntry = GetStats((PlayerMobile)killer);

                    if (victimEntry != null)
                    {
                        victimEntry.Deaths++;
                        victimEntry.HandleDeath(killer, true);
                    }

                    if (killerEntry != null)
                    {
                        killerEntry.Kills++;
                        killerEntry.HandleDeath(victim, false);
                    }
                }

                List<ArenaTeam> stillAlive = new List<ArenaTeam>();

                foreach (var team in Teams)
                {
                    if (!team.CheckTeamDead())
                        stillAlive.Add(team);
                }

                if (stillAlive.Count == 1)
                {
                    EndDuel(stillAlive[0]);
                }

                ColUtility.Free(stillAlive);
            }
        }

        public void SendResults(ArenaTeam winner)
        {
            foreach (var part in GetParticipants(true))
            {
                BaseGump.SendGump(new DuelResultsGump(part.Key, this, winner));
            }
        }

        public void OnTick()
        {
            if (HasBegun)
            {
                if (InPreFight && EntryDeadline < DateTime.UtcNow)
                {
                    BeginDuel();
                }
                else if (EndTime < DateTime.UtcNow)
                {
                    EndDuel();
                }
            }
        }

        public void Closeout()
        {
            foreach (var team in Teams)
            {
                team.Players.Clear();
                team.Players = null;
                team.PlayerZero = null;
            }

            ColUtility.Free(Teams);
            ColUtility.Free(Banned);
            KillRecord.Clear();

            Teams = null;
            Banned = null;
            KillRecord = null;
        }
    }
}