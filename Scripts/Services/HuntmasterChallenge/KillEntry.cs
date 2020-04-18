using System;

namespace Server.Engines.HuntsmasterChallenge
{
    [PropertyObject]
    public class HuntingKillEntry : IComparable
    {
        private readonly Mobile m_Owner;
        private readonly int m_Measurement;
        private readonly int m_KillIndex;
        private readonly DateTime m_DateKilled;
        private readonly string m_Location;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner => m_Owner;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Measurement => m_Measurement;

        [CommandProperty(AccessLevel.GameMaster)]
        public int KillIndex => m_KillIndex;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DateKilled => m_DateKilled;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Location => m_Location;

        public HuntingKillEntry(Mobile owner, int measurement, DateTime DateKilled, int killindex, string location)
        {
            m_Owner = owner;
            m_Measurement = measurement;
            m_DateKilled = DateKilled;
            m_KillIndex = killindex;
            m_Location = location;
        }

        public int CompareTo(object o)
        {
            if (((HuntingKillEntry)o).KillIndex == m_KillIndex || m_KillIndex < 0 || m_KillIndex >= HuntingTrophyInfo.Infos.Count)
                return ((HuntingKillEntry)o).Measurement - m_Measurement;

            HuntingTrophyInfo info1 = HuntingTrophyInfo.Infos[((HuntingKillEntry)o).KillIndex];
            HuntingTrophyInfo info2 = HuntingTrophyInfo.Infos[m_KillIndex];

            double perc1 = (double)((HuntingKillEntry)o).Measurement / info1.MaxMeasurement;
            double perc2 = (double)m_Measurement / info2.MaxMeasurement;

            return (int)((perc1 * 100) - (perc2 * 100));
        }

        public HuntingKillEntry(GenericReader reader)
        {
            int v = reader.ReadInt();

            m_Owner = reader.ReadMobile();
            m_Measurement = reader.ReadInt();
            m_DateKilled = reader.ReadDateTime();
            m_KillIndex = reader.ReadInt();
            m_Location = reader.ReadString();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(m_Owner);
            writer.Write(m_Measurement);
            writer.Write(m_DateKilled);
            writer.Write(m_KillIndex);
            writer.Write(m_Location);
        }
    }
}