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

namespace Server.Engines.VvV
{
    public class VvVGuildStats
    {
        public Guild Guild { get; set; }
        public int Score { get; set; }
        public int Kills { get; set; }
        public int ReturnedSigils { get; set; }

        public VvVGuildStats(Guild g)
        {
            Guild = g;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Score);
            writer.Write(Kills);
            writer.Write(ReturnedSigils);
        }

        public VvVGuildStats(Guild g, GenericReader reader)
        {
            int version = reader.ReadInt();

            Guild = g;

            Score = reader.ReadInt();
            Kills = reader.ReadInt();
            ReturnedSigils = reader.ReadInt();
        }
    }

    public class VvVGuildBattleStats : IComparable<VvVGuildBattleStats>
    {
        public Guild Guild { get; set; }
        public double Points { get; set; }

        public int Silver { get; set; }
        public int Kills { get; set; }
        public int Assists { get; set; }
        public int Deaths { get; set; }
        public int Stolen { get; set; }

        public int ReturnedSigils { get { return ViceReturned + VirtueReturned; } }

        public int ViceReturned { get; set; }
        public int VirtueReturned { get; set; }

        public int Disarmed { get; set; }

        public VvVGuildBattleStats(Guild g)
        {
            Guild = g;
        }

        public int CompareTo(VvVGuildBattleStats stats)
        {
            if (Points > stats.Points)
                return 1;

            if (Points == stats.Points)
                return 0;

            return -1;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Points);

            writer.Write(Silver);
            writer.Write(Kills);
            writer.Write(Assists);
            writer.Write(Deaths);
            writer.Write(Stolen);
            writer.Write(ViceReturned);
            writer.Write(VirtueReturned);
            writer.Write(Disarmed);
        }

        public VvVGuildBattleStats(GenericReader reader, Guild g)
        {
            int version = reader.ReadInt();

            Guild = g;
            Points = reader.ReadDouble();

            Silver = reader.ReadInt();
            Kills = reader.ReadInt();
            Assists = reader.ReadInt();
            Deaths = reader.ReadInt();
            Stolen = reader.ReadInt();
            ViceReturned = reader.ReadInt();
            VirtueReturned = reader.ReadInt();
            Disarmed = reader.ReadInt();
        }
    }
}