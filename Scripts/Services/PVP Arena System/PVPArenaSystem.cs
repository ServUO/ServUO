using Server.Commands;
using Server.Engines.Points;
using Server.Gumps;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;

//TODO: Party: 1152064 You cannot invite other players in an arena to your party!
namespace Server.Engines.ArenaSystem
{
    public class PVPArenaSystem : PointsSystem
    {
        public static PVPArenaSystem Instance { get; set; }
        public static bool Enabled => true;
        public static bool BlockSameIP => true;

        public override PointsType Loyalty => PointsType.PVPArena;
        public override TextDefinition Name => m_Name;
        public override bool AutoAdd => true;
        public override double MaxPoints => double.MaxValue;

        public override bool ShowOnLoyaltyGump => false;
        private readonly TextDefinition m_Name = new TextDefinition("Arena Stats");

        public static List<PVPArena> Arenas { get; set; }
        public static List<string> BlockedArenas { get; set; }

        public static bool SystemInitialized { get; set; }

        public PVPArenaSystem()
        {
            Instance = this;

            if (Enabled)
            {
                CommandSystem.Register("ResetArenaStats", AccessLevel.Administrator, ResetStats_OnTarget);
                CommandSystem.Register("ArenaSetup", AccessLevel.Administrator, ArenaSetup);
            }
        }

        public void OnTick()
        {
            Arenas.ForEach(a => a.OnTick());
        }

        public List<ArenaDuel> GetBookedDuels()
        {
            List<ArenaDuel> booked = new List<ArenaDuel>();

            foreach (PVPArena arena in Arenas.Where(a => a.BookedDuels.Count > 0))
            {
                booked.AddRange(arena.BookedDuels);
            }

            return booked;
        }

        public ArenaDuel GetBookedDuel(PlayerMobile pm)
        {
            foreach (PVPArena arena in Arenas.Where(a => a.BookedDuels.Count > 0))
            {
                foreach (ArenaDuel duel in arena.BookedDuels.Where(d => d.IsParticipant(pm)))
                {
                    return duel;
                }
            }

            return null;
        }

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
        }

        public override TextDefinition GetTitle(PlayerMobile from)
        {
            return new TextDefinition("Arena Stats");
        }

        public override PointsEntry GetSystemEntry(PlayerMobile pm)
        {
            return new PlayerStatsEntry(pm);
        }

        public void Register(PVPArena arena)
        {
            if (Arenas == null)
                Arenas = new List<PVPArena>();

            if (!Arenas.Contains(arena))
                Arenas.Add(arena);
        }

        public void Unregister(PVPArena arena)
        {
            if (Arenas != null)
            {
                Arenas.Remove(arena);
            }

            arena.Unregister();
        }

        public void AddBlockedArena(PVPArena arena)
        {
            if (BlockedArenas == null)
            {
                BlockedArenas = new List<string>();
            }

            if (!IsBlocked(arena.Definition))
            {
                Utility.WriteConsoleColor(ConsoleColor.Green, "Adding blocked EA PVP Arena: {0}", arena.Definition.Name);
                BlockedArenas.Add(arena.Definition.Name);
                Unregister(arena);
            }
        }

        public void RemoveBlockedArena(ArenaDefinition def)
        {
            if (BlockedArenas == null)
            {
                return;
            }

            BlockedArenas.Remove(def.Name);

            if (Arenas == null || Arenas.Count == 0)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), () => Instance.OnTick());
            }

            PVPArena arena = new PVPArena(def);
            Instance.Register(arena);
            Utility.WriteConsoleColor(ConsoleColor.Green, "Removing blocked EA PVP Arena: {0}", arena.Definition.Name);
            arena.ConfigureArena();
        }

        public bool IsBlocked(ArenaDefinition def)
        {
            return BlockedArenas != null && BlockedArenas.Contains(def.Name);
        }

        public static bool IsEnemy(Mobile source, Mobile target)
        {
            ArenaRegion sourceRegion = Region.Find(source.Location, source.Map) as ArenaRegion;
            ArenaRegion targetRegion = Region.Find(target.Location, target.Map) as ArenaRegion;

            if (sourceRegion != null && sourceRegion.Arena.CurrentDuel != null && sourceRegion == targetRegion)
            {
                return sourceRegion.Arena.CurrentDuel.IsEnemy(source, target);
            }

            return false;
        }

        public static bool IsFriendly(Mobile source, Mobile target)
        {
            ArenaRegion sourceRegion = Region.Find(source.Location, source.Map) as ArenaRegion;
            ArenaRegion targetRegion = Region.Find(target.Location, target.Map) as ArenaRegion;

            if (sourceRegion != null && sourceRegion.Arena.CurrentDuel != null && sourceRegion == targetRegion)
            {
                return sourceRegion.Arena.CurrentDuel.IsFriendly(source, target);
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2);

            writer.Write(BlockedArenas == null ? 0 : BlockedArenas.Count);

            if (BlockedArenas != null)
            {
                for (int i = 0; i < BlockedArenas.Count; i++)
                {
                    writer.Write(BlockedArenas[i]);
                }
            }

            writer.Write(Arenas == null ? 0 : Arenas.Count);

            if (Arenas != null)
            {
                for (int i = 0; i < Arenas.Count; i++)
                {
                    writer.Write(Arenas[i].Definition.Name);
                    Arenas[i].Serialize(writer);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            SystemInitialized = true;

            int version = reader.ReadInt();

            if (version < 2)
            {
                InitializeArenas();
            }

            switch (version)
            {
                case 2:
                case 1:
                    int c = reader.ReadInt();

                    for (int i = 0; i < c; i++)
                    {
                        if (BlockedArenas == null)
                            BlockedArenas = new List<string>();

                        BlockedArenas.Add(reader.ReadString());
                    }
                    goto case 0;
                case 0:
                    int count = reader.ReadInt();

                    for (int i = 0; i < count; i++)
                    {
                        if (Arenas == null)
                            Arenas = new List<PVPArena>();

                        if (version >= 2)
                        {
                            PVPArena arena = new PVPArena(GetDefinition(reader.ReadString()));
                            Register(arena);
                            arena.Deserialize(reader);
                        }
                        else
                        {
                            Arenas[i].Deserialize(reader);
                        }
                    }
                    break;
            }
        }

        private ArenaDefinition GetDefinition(string name)
        {
            ArenaDefinition def = ArenaDefinition.Definitions.FirstOrDefault(d => d.Name == name);

            if (def == null)
            {
                return ArenaDefinition.Definitions[0];
            }

            return def;
        }

        public static void SendMessage(Mobile from, string message, int hue = 0x1F)
        {
            from.SendMessage(hue, message);
        }

        public static void SendMessage(Mobile from, int message, string args = "", int hue = 0x1F)
        {
            from.SendLocalizedMessage(message, args, hue);
        }

        public static void SendParticipantMessage(ArenaDuel duel, int message, bool inRegion = false, string args = "", int hue = 0x1F)
        {
            foreach (KeyValuePair<PlayerMobile, PlayerStatsEntry> part in duel.GetParticipants(inRegion))
            {
                SendMessage(part.Key, message, args, hue);
            }
        }

        public void CheckTitle(PlayerMobile pm)
        {
            PlayerStatsEntry entry = GetPlayerEntry<PlayerStatsEntry>(pm);
            int title = 0;

            switch (entry.TotalDuels)
            {
                case 1: title = 1152068 + (int)ArenaTitle.FledglingGladiator; break;
                case 50: title = 1152068 + (int)ArenaTitle.BuddingGladiator; break;
                case 100: title = 1152068 + (int)ArenaTitle.Gladiator; break;
                case 250: title = 1152068 + (int)ArenaTitle.WellKnownGladiator; break;
                case 500: title = 1152068 + (int)ArenaTitle.VeteranGladiator; break;
            }

            if (title > 0)
            {
                pm.AddRewardTitle(title);
                pm.SendLocalizedMessage(1152067, string.Format("#{0}", title.ToString())); // You have gotten a new subtitle, ~1_VAL~, in reward for your duel!
            }
        }

        public static bool HasSameIP(Mobile m, ArenaDuel duel)
        {
            if (duel == null || m.AccessLevel > AccessLevel.Player)
                return false;

            foreach (KeyValuePair<PlayerMobile, PlayerStatsEntry> kvp in duel.GetParticipants())
            {
                if (IsSameIP(m, kvp.Key))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsSameIP(Mobile one, Mobile two)
        {
            if (one.NetState == null || two.NetState == null || one.AccessLevel > AccessLevel.Player || two.AccessLevel > AccessLevel.Player)
                return false;

            System.Net.IPAddress oneAddress = one.NetState.Address;
            System.Net.IPAddress twoAddress = two.NetState.Address;

            return one.NetState.Address == two.NetState.Address;
        }

        public static void Initialize()
        {
            if (Enabled)
            {
                if (!SystemInitialized)
                {
                    InitializeArenas();
                    SystemInitialized = true;
                }

                if (Arenas != null)
                {
                    foreach (PVPArena arena in Arenas)
                    {
                        arena.ConfigureArena();
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), () => Instance.OnTick());
                }
            }
        }

        public static void InitializeArenas()
        {
            if (CanInitialize(ArenaDefinition.LostLandsTrammel))
            {
                Instance.Register(new PVPArena(ArenaDefinition.LostLandsTrammel));
            }

            if (CanInitialize(ArenaDefinition.LostLandsFelucca))
            {
                Instance.Register(new PVPArena(ArenaDefinition.LostLandsFelucca));
            }

            if (CanInitialize(ArenaDefinition.HavenTrammel))
            {
                Instance.Register(new PVPArena(ArenaDefinition.HavenTrammel));
            }

            if (CanInitialize(ArenaDefinition.HavenFelucca))
            {
                Instance.Register(new PVPArena(ArenaDefinition.HavenFelucca));
            }
        }

        private static bool CanInitialize(ArenaDefinition def)
        {
            return !Instance.IsBlocked(def) && (Arenas == null || !Arenas.Any(arena => arena.Definition == def));
        }

        [Usage("ArenaSetup")]
        [Description("Gives gump for arena setup.")]
        public static void ArenaSetup(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm != null)
            {
                BaseGump.SendGump(new PVPArenaSystemSetupGump(pm));
            }
        }

        [Usage("ResetArenaStats")]
        [Description("Target an arena stone to reset/wipe the stats associated with that arena.")]
        public static void ResetStats_OnTarget(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

            m.BeginTarget(-1, false, Targeting.TargetFlags.None, (fro, targeted) =>
                {
                    if (m is PlayerMobile && targeted is ArenaStone)
                    {
                        ArenaStone stone = (ArenaStone)targeted;

                        if (stone.Arena != null)
                        {
                            PVPArena arena = stone.Arena;

                            BaseGump.SendGump(new GenericConfirmCallbackGump<PVPArena>((PlayerMobile)m,
                                string.Format("Reset {0} Statistics?", arena.Definition.Name),
                                "By selecting yes, you will permanently wipe the stats associated to this arena.",
                                arena,
                                null,
                                (from, a) =>
                                {
                                    ColUtility.Free(a.TeamRankings);
                                    ColUtility.Free(a.SurvivalRankings);
                                    from.SendMessage("Arena stats cleared.");
                                }));
                        }
                    }
                });
        }
    }

    public class PlayerStatsEntry : PointsEntry
    {
        public int SurvivalWins { get; set; }
        public int SurvivalLosses { get; set; }
        public int SurvivalDraws { get; set; }

        public int TeamWins { get; set; }
        public int TeamLosses { get; set; }
        public int TeamDraws { get; set; }

        public int Kills { get; set; }
        public int Deaths { get; set; }

        public bool IgnoreInvites { get; set; }
        public bool OpenStats { get; set; }

        public int TotalDuels => SurvivalWins + SurvivalLosses + SurvivalDraws + TeamWins + TeamLosses + TeamDraws;

        public List<DuelRecord> Record { get; set; }

        public DuelProfile Profile { get; set; }

        public PlayerStatsEntry(PlayerMobile pm)
            : base(pm)
        {
            IgnoreInvites = true;
            OpenStats = true;

            Record = new List<DuelRecord>();
        }

        public void HandleDeath(Mobile m, bool killedBy)
        {
            Record.Add(new DuelRecord(m, killedBy));

            if (Record.Count > 12)
            {
                Record.RemoveAt(12);
            }
        }

        // Rating, seems to start at 10000, then +33 for win, -33 for loss
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            if (Profile != null)
            {
                writer.Write(1);
                Profile.Serialize(writer);
            }
            else
            {
                writer.Write(0);
            }

            writer.Write(Record.Count);
            for (int i = 0; i < Record.Count; i++)
            {
                Record[i].Serialize(writer);
            }

            writer.Write(OpenStats);
            writer.Write(IgnoreInvites);

            writer.Write(SurvivalWins);
            writer.Write(SurvivalLosses);
            writer.Write(SurvivalDraws);

            writer.Write(TeamWins);
            writer.Write(TeamLosses);
            writer.Write(TeamDraws);

            writer.Write(Kills);
            writer.Write(Deaths);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Record = new List<DuelRecord>();

            if (reader.ReadInt() == 1)
            {
                Profile = new DuelProfile(reader);
            }

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Record.Add(new DuelRecord(reader));
            }

            OpenStats = reader.ReadBool();
            IgnoreInvites = reader.ReadBool();

            SurvivalWins = reader.ReadInt();
            SurvivalLosses = reader.ReadInt();
            SurvivalDraws = reader.ReadInt();

            TeamWins = reader.ReadInt();
            TeamLosses = reader.ReadInt();
            TeamDraws = reader.ReadInt();

            Kills = reader.ReadInt();
            Deaths = reader.ReadInt();
        }

        public class DuelRecord
        {
            public Mobile Opponent { get; set; }
            public bool KilledBy { get; set; }
            public DateTime DuelDate { get; set; }

            public DuelRecord(Mobile opponent, bool killedBy)
            {
                Opponent = opponent;
                KilledBy = killedBy;
                DuelDate = DateTime.Now;
            }

            public DuelRecord(GenericReader reader)
            {
                int version = reader.ReadInt();

                Opponent = reader.ReadMobile();
                KilledBy = reader.ReadBool();
                DuelDate = reader.ReadDateTime();
            }

            public void Serialize(GenericWriter writer)
            {
                writer.Write(0);

                writer.Write(Opponent);
                writer.Write(KilledBy);
                writer.Write(DuelDate);
            }
        }

        public class DuelProfile
        {
            public int Entries { get; set; }
            public RoomType RoomType { get; set; }
            public BattleMode BattleMode { get; set; }
            public bool Ranked { get; set; }
            public TimeLimit TimeLimit { get; set; }
            public EntryFee EntryFee { get; set; }
            public int PetSlots { get; set; }
            public bool RidingFlyingAllowed { get; set; }
            public bool RangedWeaponsAllowed { get; set; }
            public bool SummonSpellsAllowed { get; set; }
            public bool FieldSpellsAllowed { get; set; }
            public PotionRules PotionRules { get; set; }

            public DuelProfile(ArenaDuel duel)
            {
                Entries = duel.Entries;
                RoomType = duel.RoomType;
                BattleMode = duel.BattleMode;
                Ranked = duel.Ranked;
                TimeLimit = duel.TimeLimit;
                EntryFee = duel.EntryFee;
                PetSlots = duel.PetSlots;
                RidingFlyingAllowed = duel.RidingFlyingAllowed;
                RangedWeaponsAllowed = duel.RangedWeaponsAllowed;
                SummonSpellsAllowed = duel.SummonSpellsAllowed;
                FieldSpellsAllowed = duel.FieldSpellsAllowed;
                PotionRules = duel.PotionRules;
            }

            public DuelProfile(GenericReader reader)
            {
                int version = reader.ReadInt();

                Entries = reader.ReadInt();
                RoomType = (RoomType)reader.ReadInt();
                BattleMode = (BattleMode)reader.ReadInt();
                Ranked = reader.ReadBool();
                TimeLimit = (TimeLimit)reader.ReadInt();
                EntryFee = (EntryFee)reader.ReadInt();
                PetSlots = reader.ReadInt();
                RidingFlyingAllowed = reader.ReadBool();
                RangedWeaponsAllowed = reader.ReadBool();
                SummonSpellsAllowed = reader.ReadBool();
                FieldSpellsAllowed = reader.ReadBool();
                PotionRules = (PotionRules)reader.ReadInt();
            }

            public void Serialize(GenericWriter writer)
            {
                writer.Write(0);

                writer.Write(Entries);
                writer.Write((int)RoomType);
                writer.Write((int)BattleMode);
                writer.Write(Ranked);
                writer.Write((int)TimeLimit);
                writer.Write((int)EntryFee);
                writer.Write(PetSlots);
                writer.Write(RidingFlyingAllowed);
                writer.Write(RangedWeaponsAllowed);
                writer.Write(SummonSpellsAllowed);
                writer.Write(FieldSpellsAllowed);
                writer.Write((int)PotionRules);
            }
        }
    }
}

namespace Server.Mobiles
{
    public enum ArenaTitle
    {
        FledglingGladiator = 0,
        BuddingGladiator = 1,
        Gladiator = 3,
        WellKnownGladiator = 4,
        VeteranGladiator = 5
    }
}
