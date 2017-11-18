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
        public static TimeSpan StartTime = TimeSpan.FromSeconds(5);

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
        public ArenaTeam TeamOrder { get { return Teams[0]; } }
        public ArenaTeam TeamChaos { get { return Teams[1]; } }

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

            Teams.Add(new ArenaTeam());
            Teams.Add(new ArenaTeam());

            Complete = false;
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
            return GetParticipants().ToList();
        }

        public bool InArena(Mobile m)
        {
            if (m == null)
                return false;

            return Region.Find(m.Location, m.Map) == Arena.Definition.Region;
        }

        public PlayerStatsEntry GetStats(PlayerMobile check)
        {
            var kvp = GetParticipants().FirstOrDefault(kvp => kvp.Key == check);

            if (kvp != null)
                return kvp.Value;

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

        }

        public void RegisterPlayer(PlayerMobile pm)
        {
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
            Rectangle2D rec = Arena.Definition.RegionBounds[Utility.Random(Arena.Definition.RegionBounds.Length)];
            rec = new Rectangle2D(rec.X + 2, rec.Y + 2, rec.Width - 4, rec.Height - 4);

            Point3D p = rec.GetRandomSpawnPoint(map);

            //TODO: Handle pets? for now they'll have to use mounts or pet ball summon?
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

            Gate = new ArenaGate(this);
            Gate.MoveToWorld(Arena.Definition.GateLocation, Arena.Definition.Map);

            PVPArenaSystem.SendParticipantMessage(this, 1115874); // The arena gate has opened near the arena stone. <br>You've ninety seconds to use the gate or you'll be removed from this duel.

            EntryDeadline = DateTime.UtcNow + EntryTime;
            PVPArenaSystem.SendParticipantMessage(this, 1115875);
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
                    foreach (var pm in present.Players.Keys)
                    {
                        PVPArenaSystem.SendMessage(pm, 1116476); // Your duel is about to begin but the opposing team has not came to the arena. This session will be aborted and you will be ejected from the arena.
                    }
                }

                CancelDuel();
                ColUtility.Free(present);

                return;
            }

            ColUtility.Free(present);

            Timer.DelayCall(StartTime, () =>
                {
                    DoStartEffects();

                    foreach (var m in GetParticipants(true))
                    {
                        m.SendSpeedControl(SpeedControlType.Disable);
                    }
                });
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

            if (EntryFee > EntryFee.None)
            {
                foreach (var part in GetParticipants())
                {
                    Banker.Depost(part.Key, (int)EntryFee, true);
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

            SendResults();

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
                    PVPArenaSystem.SendMessage(pm, team.Count > 0 ? 1116489 : 1116488); // You have lost the duel... : Your team has lost the duel...
                }
            }

            if (Ranked)
            {
                RecordStats(winner);
            }

            PVPArenaSystem.SendParticipantMessage(this, 1115965); // All participants will be ejected from this arena in 40 seconds.
        }

        public void RemovePlayers()
        {
            foreach (var part in GetParticipants(true))
            {
                RemovePlayer(part.Key);
            }

            PVPArenaSystem.SendParticipantMessage(this, 1115973); // Thank you for your participation! Please return to the arena stone for additional dueling opportunities!

            Arena.OnDuelEnd();
        }

        public void RemovePlayer(PlayerMobile pm)
        {
            Rectangle2D rec = new Rectangle2D(Arena.Stone.X - 2, Arena.Stone.Y - 5, 5, 11);
            Map map = Arena.Definition.Map;

            Point3D p = Arena.Stone.Location;

            while (p == Arena.Stone.Location)
            {
                p = rec.GetRandomSpawnPoint(map);
            }

            pm.Key.MoveToWorld(p, map);
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

                if (Ranked && killer != null && killer != victim) // if not ranked, does it keep track of this?
                {
                    PlayerStatsEntry victimEntry = GetStats((PlayerMobile)victim);
                    PlayerStatsEntry killerEntry = GetStats((PlayerMobile)killer);

                    if (victimEntry != null)
                        victimEntry.Deaths++;

                    if (killerEntry != null)
                        killerEntry.Kills++;
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

        public void SendResults()
        {
            foreach (var pm in GetParticipants().Keys.Where(p => Region.Find(p.Location, p.Map) == Arena.Region))
            {
                BaseGump.SendGump(new DuelResultsGump(pm, this));
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
    }
}