using Server;
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Points;
using System.Linq;

namespace Server.Engines.ArenaSystem
{
    public class PVPArenaSystem : PointsSystem
    {
        public static PVPArenaSystem Instance { get; set; }
        public static bool Enabled = Config.Get("PVPArena.Enabled", true);

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
                Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnTick);
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

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
            //from.SendLocalizedMessage(1153423, ((int)points).ToString()); // You have gained ~1_AMT~ Dungeon Crystal Points of Despise.
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

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
            writer.Write(Arenas.Count);

            for (int i = 0; i < Arenas.Count; i++)
            {
                Arenas[i].Serialize(writer);
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

        public static void Initialize()
        {
            foreach (var arena in Arenas)
            {
                arena.ConfigureArena();
            }
        }

        public static void InitializeArenas()
        {
            Instance.Register(new PVPArena(ArenaDefinition.LostLandsTrammel));
            Instance.Register(new PVPArena(ArenaDefinition.LostLandsFelucca));
            Instance.Register(new PVPArena(ArenaDefinition.HavenTrammel));
            Instance.Register(new PVPArena(ArenaDefinition.HavenFelucca));
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

        public List<DuelRecord> Record { get; set; }

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

            if (Record.Count > 10)
            {
                Record.RemoveAt(10);
            }
        }

        // Rating, seems to start at 10000, then +33 for win, -33 for loss
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Record.Count);
            for (int i = 0; i < Record.Count; i++)
            {
                Record[i].Serialize(writer);
            }

            writer.Write(OpenStats);

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

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Record.Add(new DuelRecord(reader));
            }

            OpenStats = reader.ReadBool();

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
    }
}