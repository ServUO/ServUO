using Server;
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Points;
using System.Linq;
using Server.Commands;
using Server.Gumps;

//TODO: Party: 1152064 You cannot invite other players in an arena to your party!
namespace Server.Engines.ArenaSystem
{
    public class PVPArenaSystem : PointsSystem
    {
        public static PVPArenaSystem Instance { get; set; }
        public static bool Enabled { get { return Core.HS; } }
        public static bool BlockSameIP { get { return true; } }

        public override PointsType Loyalty { get { return PointsType.PVPArena; } }
        public override TextDefinition Name { get { return m_Name; } }
        public override bool AutoAdd { get { return true; } }
        public override double MaxPoints { get { return double.MaxValue; } }

        public override bool ShowOnLoyaltyGump { get { return false; } }
        private TextDefinition m_Name = new TextDefinition("Arena Stats");

        public static List<PVPArena> Arenas { get; set; }

        public PVPArenaSystem()
        {
            Instance = this;

            if (Enabled)
            {
                InitializeArenas();

                CommandSystem.Register("ResetArenaStats", AccessLevel.Administrator, ResetStats_OnTarget);
            }
        }

        public void OnTick()
        {
            Arenas.ForEach(a => a.OnTick());
        }

        public List<ArenaDuel> GetBookedDuels()
        {
            List<ArenaDuel> booked = new List<ArenaDuel>();

            foreach (var arena in Arenas.Where(a => a.BookedDuels.Count > 0))
            {
                booked.AddRange(arena.BookedDuels);
            }

            return booked;
        }

        public ArenaDuel GetBookedDuel(PlayerMobile pm)
        {
            foreach (var arena in Arenas.Where(a => a.BookedDuels.Count > 0))
            {
                foreach (var duel in arena.BookedDuels.Where(d => d.IsParticipant(pm)))
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

        public static bool IsEnemy(Mobile source, Mobile target)
        {
            var sourceRegion = Region.Find(source.Location, source.Map) as ArenaRegion;
            var targetRegion = Region.Find(target.Location, target.Map) as ArenaRegion;

            if (sourceRegion != null && sourceRegion.Arena.CurrentDuel != null && sourceRegion == targetRegion)
            {
                return sourceRegion.Arena.CurrentDuel.IsEnemy(source, target);
            }

            return false;
        }

        public static bool IsFriendly(Mobile source, Mobile target)
        {
            var sourceRegion = Region.Find(source.Location, source.Map) as ArenaRegion;
            var targetRegion = Region.Find(target.Location, target.Map) as ArenaRegion;

            if (sourceRegion != null && sourceRegion.Arena.CurrentDuel != null && sourceRegion == targetRegion)
            {
                return sourceRegion.Arena.CurrentDuel.IsFriendly(source, target);
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
            writer.Write(Arenas == null ? 0 : Arenas.Count);

            if (Arenas != null)
            {
                for (int i = 0; i < Arenas.Count; i++)
                {
                    Arenas[i].Serialize(writer);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                Arenas[i].Deserialize(reader);
            }
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
            foreach (var part in duel.GetParticipants(inRegion))
            {
                SendMessage(part.Key, message, args, hue);
            }
        }

        public void CheckTitle(PlayerMobile pm)
        {
            var entry = GetPlayerEntry<PlayerStatsEntry>(pm);
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
                pm.SendLocalizedMessage(1152067, String.Format("#{0}", title.ToString())); // You have gotten a new subtitle, ~1_VAL~, in reward for your duel!
            }
        }

        public static bool HasSameIP(Mobile m, ArenaDuel duel)
        {
            if (duel == null || m.AccessLevel > AccessLevel.Player)
                return false;

            foreach (var kvp in duel.GetParticipants())
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

            var oneAddress = one.NetState.Address;
            var twoAddress = two.NetState.Address;

            return one.NetState.Address == two.NetState.Address;
        }

        public static void Initialize()
        {
            if (Enabled)
            {
                foreach (var arena in Arenas)
                {
                    arena.ConfigureArena();
                }

                Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), () => Instance.OnTick() );
            }
        }

        public static void InitializeArenas()
        {
            Instance.Register(new PVPArena(ArenaDefinition.LostLandsTrammel));
            Instance.Register(new PVPArena(ArenaDefinition.LostLandsFelucca));
            Instance.Register(new PVPArena(ArenaDefinition.HavenTrammel));
            Instance.Register(new PVPArena(ArenaDefinition.HavenFelucca));
        }

        [Usage("ResetArenaStats")]
        [Description("Target an arena stone to reset/wipe the stats associated with that arena.")]
        public static void ResetStats_OnTarget(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

            m.BeginTarget(-1, false, Server.Targeting.TargetFlags.None, (fro, targeted) =>
                {
                    if (m is PlayerMobile && targeted is ArenaStone)
                    {
                        var stone = (ArenaStone)targeted;

                        if (stone.Arena != null)
                        {
                            var arena = stone.Arena;

                            BaseGump.SendGump(new GenericConfirmCallbackGump<PVPArena>((PlayerMobile)m,
                                String.Format("Reset {0} Statistics?", arena.Definition.Name),
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

        public int TotalDuels { get { return SurvivalWins + SurvivalLosses + SurvivalDraws + TeamWins + TeamLosses + TeamDraws; } }

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
        VeteranGladiator =5
    }
}