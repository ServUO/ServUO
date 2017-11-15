using Server;
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Points;

namespace Server.Engines.ArenaSystem
{
    public class PVPArenaStats : PointsSystem
    {
        public static bool Enabled = Config.Get("PVPArena.Enabled", true);

        public override PointsType Loyalty { get { return PointsType.PVPArenaStats; } }
        public override TextDefinition Name { get { return m_Name; } }
        public override bool AutoAdd { get { return true; } }
        public override double MaxPoints { get { return double.MaxValue; } }

        public override bool ShowOnLoyaltyGump { get { return false; } }
        private TextDefinition m_Name = new TextDefinition("Arena Stats");

        public static List<PVPArena> Arenas { get; set; }

        public PVPArenaStats()
        {
            if (Enabled)
            {
                InitializeArenas();
            }
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

            arena.Register();
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

        public static void SendMessage(Mobile from, string message, int hue)
        {

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
            Register(new PVPArena(ArenaDefinition.LostLandsTrammel));
            Register(new PVPArena(ArenaDefinition.LostLandsFelucca));
            Register(new PVPArena(ArenaDefinition.HavenTrammel));
            Register(new PVPArena(ArenaDefinition.HavenFelucca));
        }
    }

    public class PlayerStatsEntry : PointsEntry
    {
        public int Kills { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }

        public PlayerStatsEntry(PlayerMobile pm)
            : base(pm)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Kills);
            writer.Write(Losses);
            writer.Write(Draws);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Kills = reader.ReadInt();
            Losses = reader.ReadInt();
            Draws = reader.ReadInt();
        }
    }
}