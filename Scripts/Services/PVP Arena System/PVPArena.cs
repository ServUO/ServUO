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

        public Dictionary<ArenaDuel, TimeSpan> PendingDuels { get; set; }
        public List<ArenaDuel> BookedDuels { get; set; }
        public Dictionary<PlayerMobile, int> StatsTable { get; set; }

        public PVPArena(ArenaDefinition definition)
        {
            Definition = definition;

            PendingDuels = new Dictionary<ArenaDuel, TimeSpan>();
            BookedDuels = new List<ArenaDuel>();
            StatsTable = new Dictionary<PlayerMobile, int>();
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

        public void OnTick()
        {
            if (CurrentDuel != null)
            {
                CurrentDuel.OnTick();
            }

            PendingDuels.Keys.ForEach(d =>
                {
                    if (PendingDuels[d] < DateTime.UtcNow)
                    {
                        PendingDuels.Remove(d);
                    }
                });
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
            return PendingDuels.FirstOrDefault(d => d.Host == m);
        }

        public List<ArenaDuel> GetPendingPublic()
        {
            return PendingDuels.Where(d => d.RoomType == RoomType.Public).ToList();
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
            }
        }

        public void OnDuelEnd(ArenaDuel duel)
        {
            CurrentDuel = null;

            if (BookedDuels.Count > 0)
            {
                var newDuel = CurrentDuel[0];
                CurrentDuel = newDuel;
                newDuel.DoPreDuel();
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(StatsTable.Count);
            foreach (var kvp in StatsTable)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
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
                PlayerMobile pm = reader.ReadMobile() as PlayerMobile;
                int score = reader.ReadInt();

                if (pm != null)
                    StatsTable[pm] = score;
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
}