using Server;
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using System.Linq;

namespace Server.Engines.ArenaSystem
{
    public class ArenaTeam
    {
        public Dictionary<PlayerMobile, PlayerStatsEntry> Players { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Count { get { return Players == null ? 0 : Players.Count; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Unoccupied { get { return Count == 0; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile PlayerZero { get; set; }

        public ArenaTeam()
        {
        }

        public ArenaTeam(PlayerMobile pm)
        {
            AddParticipant(pm);
        }

        public void AddParticipant(PlayerMobile pm)
        {
            if (Players == null)
            {
                Players = new Dictionary<PlayerMobile, PlayerStatsEntry>();
                PlayerZero = pm;
            }

            Players[pm] = PVPArenaSystem.Instance.GetPlayerEntry<PlayerStatsEntry>(pm);
        }

        public bool RemoveParticipant(PlayerMobile pm)
        {
            if (Players == null)
                return false;

            if (Players.ContainsKey(pm))
            {
                Players.Remove(pm);
                return true;
            }

            return false;
        }

        public bool CheckTeamDead()
        {
            return Players.Keys.FirstOrDefault(pm => pm.Alive) == null;
        }

        public bool Contains(PlayerMobile pm)
        {
            return Players.Contains(pm);
        }
    }
}