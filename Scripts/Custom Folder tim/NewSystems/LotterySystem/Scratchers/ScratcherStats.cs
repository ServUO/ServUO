using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.LotterySystem
{
    public class ScratcherStats
    {
        private Mobile m_Winner;
        private TicketType m_Type;
        private int m_Payout;
        private DateTime m_WinTime;

        public Mobile Winner { get { return m_Winner; } }
        public TicketType Type { get { return m_Type; } }
        public int Payout { get { return m_Payout; } }
        public DateTime WinTime { get { return m_WinTime; } } 

        private static List<ScratcherStats> m_Stats = new List<ScratcherStats>();
        public static List<ScratcherStats> Stats { get { return m_Stats; } }

        public ScratcherStats(Mobile winner, int amount, TicketType type) : this(winner, amount, type, DateTime.Now)
        {
        }

        public ScratcherStats(Mobile winner, int amount, TicketType type, DateTime time)
        {
            m_Winner = winner;
            m_Type = type;
            m_Payout = amount;
            m_WinTime = time;

            m_Stats.Add(this);
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0);	//version

            writer.Write(m_Winner);
            writer.Write((int)m_Type);
            writer.Write(m_Payout);
            writer.Write(m_WinTime);
        }

        public ScratcherStats(GenericReader reader)
        {
            int version = reader.ReadInt();

            m_Winner = reader.ReadMobile();
            m_Type = (TicketType)reader.ReadInt();
            m_Payout = reader.ReadInt();
            m_WinTime = reader.ReadDateTime();

            m_Stats.Add(this);
        }
    }
}