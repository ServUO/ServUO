using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.LotterySystem
{
    public class PowerBallStats
    {
        public static readonly int MaxStats = 10; //How many in the list to retain

        private Mobile m_JackpotWinner;
        private int m_Payout;
        private int m_JackpotAmount;
        private int m_GameNumber;
        private DateTime m_JackpotTime;
        private List<int> m_Picks = new List<int>();
        private bool m_JackpotEntry;

        public Mobile JackpotWinner { get { return m_JackpotWinner; } }
        public int Payout { get { return m_Payout; } }
        public int JackpotAmount { get { return m_JackpotAmount; } }
        public int GameNumber { get { return m_GameNumber; } }
        public DateTime JackpotTime { get { return m_JackpotTime; } }
        public List<int> Picks { get { return m_Picks; } }
        public bool IsJackpotEntry { get { return m_JackpotEntry; } }

        private static List<PowerBallStats> m_PicksStats = new List<PowerBallStats>();
        public static List<PowerBallStats> PicksStats { get { return m_PicksStats; } }

        private static List<PowerBallStats> m_JackpotStats = new List<PowerBallStats>();
        public static List<PowerBallStats> JackpotStats { get { return m_JackpotStats; } }

        public PowerBallStats(Mobile winner, int amount, int gameNumber)
        {
            m_JackpotWinner = winner;
            m_JackpotAmount = amount;
            m_GameNumber = gameNumber;
            m_JackpotTime = DateTime.Now;
            m_JackpotEntry = true;

            m_JackpotStats.Add(this);
            TrimList(m_JackpotStats); //Keeps the list at a minimum of 10

            if (PowerBall.Game != null)
                PowerBall.Game.UpdateSatellites();
        }

        public PowerBallStats(List<int> list, int gameNumber, int payOut)
        {
            m_Picks = list;
            m_GameNumber = gameNumber;
            m_JackpotEntry = false;
            m_Payout = payOut;

            m_PicksStats.Add(this);
            TrimList(m_PicksStats); //Keeps the list at a minimum of 10
        }

        public void TrimList(List<PowerBallStats> list)
        {
            if (list == null || list.Count <= MaxStats || PowerBall.Game == null)
                return;

            for (int i = 0; i < list.Count; ++i)
            {
                PowerBallStats stat = list[i];

                if (stat.IsJackpotEntry && i < list.Count - MaxStats && m_JackpotStats.Contains(stat))
                    m_JackpotStats.Remove(stat);
                else if (stat.GameNumber < PowerBall.Game.GameNumber - MaxStats && m_PicksStats.Contains(stat))
                    m_PicksStats.Remove(stat);
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0);	//version

            writer.Write(m_JackpotEntry);
            writer.Write(m_GameNumber);

            if (m_JackpotEntry)
            {
                writer.Write(m_JackpotWinner);
                writer.Write(m_JackpotAmount);
                writer.Write(m_JackpotTime);
            }
            else
            {
                writer.Write(m_Payout);
                writer.Write(m_Picks.Count);

                for (int i = 0; i < m_Picks.Count; ++i)
                {
                    writer.Write(m_Picks[i]);
                }
            }
        }

        public PowerBallStats(GenericReader reader)
        {
            int version = reader.ReadInt();

            m_JackpotEntry = reader.ReadBool();
            m_GameNumber = reader.ReadInt();

            if (m_JackpotEntry)
            {
                m_JackpotWinner = reader.ReadMobile();
                m_JackpotAmount = reader.ReadInt();
                m_JackpotTime = reader.ReadDateTime();

                m_JackpotStats.Add(this);
            }
            else
            {
                m_Payout = reader.ReadInt();

                int count = reader.ReadInt();

                for (int i = 0; i < count; ++i)
                {
                    int pick = reader.ReadInt();
                    m_Picks.Add(pick);
                }

                m_PicksStats.Add(this);
            }


        }
    }
}