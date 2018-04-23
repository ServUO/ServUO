using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;

/* To give a reward for a jackpot, you can add it to GiveReward(Mobile m) method.
 * RewardChance is the chance that reward is given, on Jackpot. This reward, which 
 * is NOT given by default, would be in addition to the normal jackpot.*/

namespace Server.Engines.LotterySystem
{
    public class PowerBallGame
    {
        public static double RewardChance = 0.5;

        private int m_Profit;          //Total ticket/entry costs per string of games - resets to zero once a jackpot is won
        private int m_NoWins;          //Increases each game there is not jackpot
        private int m_Payout;          //Total payout on any winning tickets
        private bool m_Jackpot;        //Did this game yeild a jackpot?
        private PowerBall m_PowerBall; //Instanced Powerball Item
        private List<PowerBallTicket> m_Tickets = new List<PowerBallTicket>(); //Tickets for current game
        private List<int> m_Picks;     //Picked at the end of the game
        private int m_JackpotWinners;  //Used for jackpot message

        public int Profit { get { return m_Profit; } set { m_Profit = value; } }
        public int Payout { get { return m_Payout; } }
        public int JackPot { get { return m_Profit / 2; } }
        public int NoWins { get { return m_NoWins; } set { m_NoWins = value; } }
        public bool HasJackpot { get { return m_Jackpot; } }
        public PowerBall PowerBall { get { return m_PowerBall; } }
        public List<PowerBallTicket> Tickets { get { return m_Tickets; } }
        public List<int> Picks { get { return m_Picks; } }
        public bool CanBuyTickets { get { return m_PowerBall.NextGame - m_PowerBall.DeadLine > DateTime.Now && m_PowerBall.IsActive; } }
        public int JackpotWinners { get { return m_JackpotWinners; } }

        public PowerBallGame(PowerBall item, int noWins, int profit)
        {
            m_PowerBall = item;

            m_NoWins = noWins;
            m_Profit = profit;
        }

        public PowerBallGame(PowerBall item)
        {
            m_PowerBall = item;

            m_NoWins = 0;
            m_Profit = 0;
        }

        public void ProcessGame()
        {
            m_Payout = 0;
            m_JackpotWinners = 0;

            if (m_PowerBall == null)
                return;;

            if (m_PowerBall.DoJackpot)
                m_Picks = ChooseJackpotPicks();
            
            if (m_Picks == null)
            {
                m_Picks = new List<int>();

                List<int> whiteList = new List<int>();
                int whiteCount = m_PowerBall.MaxWhiteBalls;

                for (int i = 1; i < whiteCount + 1; ++i)
                    whiteList.Add(i);

                int count = 0;
                int pick = 0;
                int powerBall;

                while (++count < 6)
                {
                    pick = whiteList[Utility.Random(whiteList.Count)];
                    m_Picks.Add(pick);
                    whiteList.Remove(pick);
                }

                powerBall = Utility.RandomMinMax(1, m_PowerBall.MaxRedBalls);
                m_Picks.Add(powerBall);
            }

            if (m_PowerBall.GuaranteedJackpot && m_NoWins > 5 && Utility.Random(20) < m_NoWins)
            {
                List<int> list = ChooseJackpotPicks();

                if (list != null)
                {
                    m_Picks.Clear();
                    m_Picks = list;
                }
            }

            new InternalTimer(this);                       
        }

        public void CheckForWinners()
        {

            if (m_Picks == null || m_Tickets == null || m_Tickets.Count == 0)
                return;

            List<TicketEntry> jackpotList = new List<TicketEntry>();
            Dictionary<TicketEntry, int> prizeTable = new Dictionary<TicketEntry, int>();

            for(int i = 0; i < m_Tickets.Count; ++i)
            {
                PowerBallTicket ticket = m_Tickets[i];
                ticket.Checked = true;

                foreach (TicketEntry entry in ticket.Entries)
                {
                    int matches = 0;
                    bool powerball = false;

                    for (int j = 0; j < m_Picks.Count; ++j)
                    {
                        if (j == m_Picks.Count - 1)
                        {
                            if (entry.PowerBall == m_Picks[j])
                                powerball = true;
                        }

                        else if (entry.One == m_Picks[j] || entry.Two == m_Picks[j] || entry.Three == m_Picks[j] || entry.Four == m_Picks[j] || entry.Five == m_Picks[j])
                            matches++;
                    }

                    if (matches == 5 && powerball)
                    {
                        jackpotList.Add(entry);
                        entry.Winner = true;
                        m_Jackpot = true;
                    }

                    if (matches >= 3 && !entry.Winner && !prizeTable.ContainsKey(entry))
                    {
                        prizeTable.Add(entry, matches);
                        entry.Winner = true;
                    }

                    if (powerball && !m_Jackpot)
                    {
                        entry.Ticket.Payout += 500;
                        m_Profit -= 500;
                        m_Payout += 500;

                        entry.Winner = true;
                    }
                }
            }

            DistributeAwards(jackpotList, prizeTable);
        }

        private void DistributeAwards(List<TicketEntry> jackpot, Dictionary<TicketEntry, int> prize)
        {

            int pot = m_Profit;

            if (jackpot != null && jackpot.Count > 0)
            {
                foreach (TicketEntry entry in jackpot)
                {
                    if (entry.Ticket != null && entry.Ticket.Owner != null)
                    {
                        int amount = JackPot / jackpot.Count;

                        m_Jackpot = true;
                        entry.Ticket.Payout += amount;
                        m_Payout += amount;
                        m_Profit -= amount;
                        PowerBall.AddToArchive(entry.Ticket.Owner, amount);
                        m_JackpotWinners++;
                        m_PowerBall.InvalidateProperties();

                        if (RewardChance > Utility.RandomDouble())
                        {
                            GiveReward(entry.Ticket.Owner);
                        }
                    }
                }
            }

            if (prize != null && prize.Count > 0)
            {
                int award = 0;
                int match3 = 0;
                int match4 = 0;
                int match5 = 0;


                foreach (KeyValuePair<TicketEntry, int> kvp in prize)
                {
                    if (kvp.Value == 3)
                        match3++;
                    else if (kvp.Value == 4)
                        match4++;
                    else
                        match5++;
                }

                foreach (KeyValuePair<TicketEntry, int> kvp in prize)
                {
                    TicketEntry entry = kvp.Key;
                    int matches = kvp.Value;

                    if (matches == 3)
                        award = (pot / 50) / match3; //2% of Pot
                    else if (matches == 4)
                        award = (pot / 20) / match4; //5% of Pot
                    else
                        award = (pot / 10) / match5; //10% of Pot

                    entry.Ticket.Payout += award;
                    m_Payout += award;
                    m_Profit -= award;
                }
            }
        }

        private void GiveReward(Mobile m)
        {
        }

        private List<int> ChooseJackpotPicks()
        {
            if (m_Tickets == null || m_Tickets.Count == 0)
                return null;

            List<TicketEntry> entryList = new List<TicketEntry>();

            foreach (PowerBallTicket ticket in m_Tickets)
            {
                if (ticket.Entries.Count > 0 && ticket.Owner != null)
                {
                    foreach (TicketEntry entry in ticket.Entries)
                    {
                        if (entry.PowerBall > 0)
                            entryList.Add(entry);
                    }
                }
            }

            int random = Utility.Random(entryList.Count);
            for (int j = 0; j < entryList.Count; ++j)
            {
                if (j == random)
                {
                    TicketEntry entry = entryList[j];
                    List<int> list = new List<int>();

                    list.Add(entry.One);
                    list.Add(entry.Two);
                    list.Add(entry.Three);
                    list.Add(entry.Four);
                    list.Add(entry.Five);
                    list.Add(entry.PowerBall);

                    return list;
                }
            }
            return null;
        }

        public void AddTicket(PowerBallTicket ticket)
        {
            if (!m_Tickets.Contains(ticket))
                m_Tickets.Add(ticket);
        }

        public void RemoveTicket(PowerBallTicket ticket)
        {
            if (m_Tickets.Contains(ticket))
                m_Tickets.Remove(ticket);
        }

        public class InternalTimer : Timer
        {
            private PowerBallGame m_Game;
            private int m_Ticks;
            private DateTime m_Start;

            public InternalTimer(PowerBallGame game) : base (TimeSpan.Zero, TimeSpan.FromSeconds(1.0))
            {
                m_Game = game;
                m_Ticks = 0;
                m_Start = DateTime.Now;
                Start();
            }

            protected override void OnTick()
            {
                if (m_Game == null || m_Game.PowerBall == null || m_Game.Picks == null || m_Game.Picks.Count < 6)
                {
                    Stop();
                    return;
                }

                if (m_Start > DateTime.Now)
                    return;

                if (m_Ticks <= 5)
                {
                    string text;
                    int num = 0;
                    try
                    {
                        num = m_Game.Picks[m_Ticks];
                    }
                    catch
                    {
                        this.Stop();
                        Console.WriteLine("Error with PowerBallGame Timer");
                        return;
                    }

                    if (m_Ticks == 0)
                        text = "The first pick is... ";
                    else if (m_Ticks < 5)
                        text = "The next pick is... ";
                    else
                        text = "And the powerball is... ";

                    if (m_Game != null && m_Game.PowerBall != null)
                    {
                        m_Game.PowerBall.PublicOverheadMessage(0, m_Ticks < 5 ? 2041 : 0x21, false, text);
                        Timer.DelayCall(TimeSpan.FromSeconds(2), new TimerStateCallback(DoDelayMessage), new object[] { m_Game.PowerBall, num, m_Ticks });
                    }

                    foreach (PowerBallSatellite sat in PowerBall.SatList)
                    {
                        sat.PublicOverheadMessage(0, m_Ticks < 5 ? 2041 : 0x21, false, text);
                        Timer.DelayCall(TimeSpan.FromSeconds(2), new TimerStateCallback(DoDelayMessage), new object[] { sat, num, m_Ticks });
                    }

                    m_Ticks++;
                    m_Start = DateTime.Now + TimeSpan.FromSeconds(8 + Utility.Random(10));
                }
                else  //Time to tally picks and start up a new game!
                {
                    m_Game.CheckForWinners();
                    PowerBall.AddToArchive(m_Game.Picks, m_Game.Payout);  //Adds pickslist to Archive for stats gump

                    if (!m_Game.HasJackpot)                               //Still no jackpot eh?
                    {
                        m_Game.NoWins++;
                        Timer.DelayCall(TimeSpan.FromSeconds(2), new TimerStateCallback(DoMessage), m_Game);
                    }
                    else
                    {
                        if (PowerBall.Announcement)
                            Timer.DelayCall(TimeSpan.FromSeconds(2), new TimerStateCallback(DoJackpotMessage), m_Game.JackpotWinners);

                        PowerBall.GoldSink += PowerBall.Instance.Profit;
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(10), new TimerStateCallback(DoWinnersMessage), m_Game);
                    m_Game.PowerBall.NewGame(m_Game.HasJackpot);

                    Stop();
                }
            }

            public static void DoDelayMessage(object state)
            {
                object[] o = (object[])state;
                Item item = o[0] as Item;
                int num = (int)o[1];
                int tick = (int)o[2];

                item.PublicOverheadMessage(0, tick < 5 ? 2041 : 0x21, false, num.ToString());
            }

            #region Messages
            public static void DoMessage(object state)
            {
                if (state is PowerBallGame)
                {
                    PowerBallGame powerball = (PowerBallGame)state;
                    List<Mobile> mobList = new List<Mobile>();

                    if (powerball == null)
                        return;

                    foreach (PowerBallTicket ticket in powerball.Tickets)
                    {
                        if (ticket.Owner != null && !mobList.Contains(ticket.Owner))
                            mobList.Add(ticket.Owner);
                    }

                    if (mobList.Count > 0)
                    {
                        for (int i = 0; i < mobList.Count; ++i)
                        {
                            mobList[i].SendMessage("New Powerball numbers have been chosen so check your tickets!");
                        }
                    }
                }
            }

            public static void DoJackpotMessage(object state)
            {
                if (state is int && (int)state > 0)
                {
                    int amount = (int)state;
                    foreach (NetState netState in NetState.Instances)
                    {
                        Mobile m = netState.Mobile;
                        if (m != null)
                        {
                            m.PlaySound(1460);
                            m.SendMessage(33, "There {0} {1} winning jackpot {2} in powerball!", amount > 1 ? "are" : "is", amount, amount > 1 ? "tickets" : "ticket");
                        }
                    }
                }
            }

            public static void DoWinnersMessage(object state)
            {
                if (state is PowerBallGame)
                {
                    PowerBallGame powerball = (PowerBallGame)state;
                    List<Mobile> winList = new List<Mobile>();

                    if (powerball == null)
                        return;

                    foreach (PowerBallTicket ticket in powerball.Tickets)
                    {
                        if (ticket.Owner != null && ticket.Owner is PlayerMobile && ticket.Owner.NetState != null)
                        {
                            foreach (TicketEntry entry in ticket.Entries)
                            {
                                if (entry.Winner && !winList.Contains(ticket.Owner))
                                    winList.Add(ticket.Owner);
                            }
                        }
                    }

                    foreach (Mobile mob in winList)
                    {
                        int sound;
                        if (mob.Female)
                        {
                            sound = Utility.RandomList(0x30C, 0x30F, 0x313, 0x31A, 0x31D, 0x32B, 0x330, 0x337);
                            mob.PlaySound(sound);
                        }
                        else
                        {
                            sound = Utility.RandomList(0x41A, 0x41B, 0x41E, 0x422, 0x42A, 0x42D, 0x431, 0x429);
                            mob.PlaySound(sound);
                        }

                        mob.SendMessage("It looks like you have a winning ticket!");
                    }
                }
            }
            #endregion
        }
            
    }
}