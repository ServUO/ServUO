using Server;
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using System.Linq;

namespace Server.Engines.ArenaSystem
{
    [PropertyObject]
    public class PVPArena
    {
        public static TimeSpan PendingDuelExpirationTime = TimeSpan.FromMinutes(10);
        public static TimeSpan BookedDuelBegin = TimeSpan.FromSeconds(10);
        public static int StartRank = 10000;

        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaManager Manager { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaStone Stone { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaExitBanner Banner1 { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaExitBanner Banner2 { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaDefinition Definition { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool InUse { get { return CurrentDuel != null; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaDuel CurrentDuel { get; set; }

        public ArenaRegion Region { get; set; }

        public Dictionary<ArenaDuel, DateTime> PendingDuels { get; set; }
        public List<ArenaDuel> BookedDuels { get; set; }
        public List<Item> Blockers { get; set; }

        public List<ArenaStats> TeamRankings { get; set; }
        public List<ArenaStats> SurvivalRankings { get; set; }

        public PVPArena(ArenaDefinition definition)
        {
            Definition = definition;

            PendingDuels = new Dictionary<ArenaDuel, DateTime>();
            BookedDuels = new List<ArenaDuel>();
            Blockers = new List<Item>();

            TeamRankings = new List<ArenaStats>();
            SurvivalRankings = new List<ArenaStats>();
        }

        public override string ToString()
        {
            return "...";
        }

        public void ConfigureArena()
        {
            if (Manager == null)
            {
                Manager = new ArenaManager(this);
                Manager.MoveToWorld(Definition.ManagerLocation, Definition.Map);
            }

            if (Stone == null)
            {
                Stone = new ArenaStone(this);
                Stone.MoveToWorld(Definition.StoneLocation, Definition.Map);
            }

            if (Banner1 == null)
            {
                Banner1 = new ArenaExitBanner(Definition.BannerID1, this);
                Banner1.MoveToWorld(Definition.BannerLocation1, Definition.Map);
            }

            if (Banner2 == null)
            {
                Banner2 = new ArenaExitBanner(Definition.BannerID2, this);
                Banner2.MoveToWorld(Definition.BannerLocation2, Definition.Map);
            }

            if (Region == null)
            {
                Region = new ArenaRegion(this);
                Region.Register();
            }
        }

        public void Unregister()
        {
            if (Region != null)
            {
                Region.Unregister();
                Region = null;
            }
        }

        private List<ArenaDuel> _Remove = new List<ArenaDuel>();

        public void OnTick()
        {
            if (CurrentDuel != null)
            {
                CurrentDuel.OnTick();
            }

            foreach (var kvp in PendingDuels)
            {
                if (kvp.Value < DateTime.UtcNow)
                {
                    _Remove.Add(kvp.Key);
                }
            }

            if (_Remove.Count > 0)
            {
                foreach (var duel in _Remove)
                {
                    if (PendingDuels.ContainsKey(duel))
                        PendingDuels.Remove(duel);
                }

                _Remove.Clear();
            }
        }

        public void AddPendingDuel(ArenaDuel duel)
        {
            if (!PendingDuels.ContainsKey(duel))
            {
                PendingDuels[duel] = DateTime.UtcNow + PendingDuelExpirationTime;
            }
        }

        public void RemovePendingDuel(ArenaDuel duel, bool cancel = false)
        {
            if(PendingDuels.ContainsKey(duel))
            {
                PendingDuels.Remove(duel);

                if (cancel)
                {
                    PVPArenaSystem.SendParticipantMessage(duel, 1115947); // The session owner has canceled the duel.
                }
            }
        }

        public ArenaDuel GetPendingDuel(Mobile m)
        {
            return PendingDuels.Keys.FirstOrDefault(d => d.Host == m);
        }

        public List<ArenaDuel> GetPendingPublic()
        {
            return PendingDuels.Keys.Where(d => d.RoomType == RoomType.Public).ToList();
        }

        public void TryBeginDuel(ArenaDuel duel)
        {
            if (PendingDuels.ContainsKey(duel))
            {
                PendingDuels.Remove(duel);
            }

            if (BookedDuels.Count == 0)
            {
                CurrentDuel = duel;
                duel.DoPreDuel();
            }
            else
            {
                BookedDuels.Add(duel);
                PVPArenaSystem.SendParticipantMessage(duel, 1115960); // There are currently no open arenas. Your duel session has been added to the booking queue.
            }
        }

        public void OnDuelEnd(ArenaDuel duel)
        {
            CurrentDuel = null;

            foreach (var corpse in Region.GetEnumeratedItems().OfType<Corpse>())
            {
                if (corpse.Owner != null && corpse.Owner.InRange(corpse.Location, 30))
                {
                    corpse.MoveToWorld(corpse.Owner.Location, corpse.Owner.Map);
                }
                else
                {
                    corpse.MoveToWorld(GetRandomRemovalLocation(), Definition.Map);
                }
            }

            if (BookedDuels.Count > 0)
            {
                var newDuel = BookedDuels[0];
                CurrentDuel = newDuel;

                PVPArenaSystem.SendParticipantMessage(newDuel, 1153141); // Your session has been booked. Please wait a few moments to start the fight.

                Timer.DelayCall(BookedDuelBegin, () =>
                    {
                        newDuel.DoPreDuel();
                    });
            }
        }

        public void RemovePlayer(Mobile m)
        {
            Map map = Definition.Map;
            Point3D p = GetRandomRemovalLocation();

            m.MoveToWorld(p, map);

            // lets remove pets, too
            if (m is PlayerMobile && ((PlayerMobile)m).AllFollowers.Count > 0)
            {
                foreach (var mob in ((PlayerMobile)m).AllFollowers.Where(pet => pet.Region.IsPartOf<ArenaRegion>()))
                {
                    mob.MoveToWorld(p, map);
                }
            }
        }

        private Point3D GetRandomRemovalLocation()
        {
            Rectangle2D rec = new Rectangle2D(Stone.X - 2, Stone.Y - 5, 5, 11);
            Point3D p = Stone.Location;

            while (p == Stone.Location)
            {
                p = rec.GetRandomSpawnPoint(Definition.Map);
            }

            return p;
        }

        public void RecordRankings(ArenaDuel duel, ArenaTeam winners)
        {
            List<ArenaStats> rankings;

            if (duel.BattleMode == BattleMode.Team)
                rankings = TeamRankings;
            else
                rankings = SurvivalRankings;

            foreach (var part in duel.GetParticipants())
            {
                var pm = part.Key;
                ArenaStats stats = rankings.FirstOrDefault(r => r.Owner == pm);

                if (stats == null)
                {
                    stats = new ArenaStats(pm);
                    rankings.Add(stats);
                }

                var team = duel.GetTeam(pm);

                if (team != winners)
                {
                    stats.Ranking -= 33;
                }
                else
                {
                    stats.Ranking += 33;
                }
            }

            rankings.Sort();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(SurvivalRankings.Count);
            foreach (var ranking in SurvivalRankings)
            {
                ranking.Serialize(writer);
            }

            writer.Write(TeamRankings.Count);
            foreach (var ranking in TeamRankings)
            {
                ranking.Serialize(writer);
            }

            writer.Write(Blockers.Count);
            foreach (var blocker in Blockers)
            {
                writer.Write(blocker);
            }

            writer.Write(Stone);
            writer.Write(Manager);
            writer.Write(Banner1);
            writer.Write(Banner2);
        }

        public void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                ArenaStats stats = new ArenaStats(reader);

                if (stats.Owner != null)
                    SurvivalRankings.Add(stats);
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                ArenaStats stats = new ArenaStats(reader);

                if (stats.Owner != null)
                    TeamRankings.Add(stats);
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Item blocker = reader.ReadItem();

                if (blocker != null)
                    Blockers.Add(blocker);
            }

            Stone = reader.ReadItem() as ArenaStone;
            Manager = reader.ReadMobile() as ArenaManager;
            Banner1 = reader.ReadItem() as ArenaExitBanner;
            Banner2 = reader.ReadItem() as ArenaExitBanner;

            if (Stone != null)
                Stone.Arena = this;
            
            if (Manager != null)
                Manager.Arena = this;

            if (Banner1 != null)
                Banner1.Arena = this;

            if (Banner2 != null)
                Banner2.Arena = this;
        }
    }

    public class ArenaStats : IComparable<ArenaStats>
    {
        public PlayerMobile Owner { get; set; }
        public int Ranking { get; set; }

        public ArenaStats(PlayerMobile pm)
        {
            Owner = pm;
            Ranking = 10000;
        }

        public int CompareTo(ArenaStats stats)
        {
            if(Ranking > stats.Ranking)
                return -1;

            if(Ranking < stats.Ranking)
                return 1;

            return 0;
        }

        public ArenaStats(GenericReader reader)
        {
            int version = reader.ReadInt();

            Owner = reader.ReadMobile() as PlayerMobile;
            Ranking = reader.ReadInt();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Owner);
            writer.Write(Ranking);
        }
    }
}