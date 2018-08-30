using System; 
using Server; 
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using System.Collections.Generic; 

namespace Server.Engines.LotterySystem 
{
    public class PowerBall : Item
    {
        public static PowerBall PowerBallInstance { get; set; }

        #region Getters/Setters

        private int m_TimeOfDay;                //Time Powerball numbers are Picked in military time
        private int m_TicketCost;               //Price per empty ticket
        private int m_TicketEntryPrice;         //Price per entry, ie picks - 10 Max per ticket
        private int m_PowerBallPrice;           //Price per powerball pick
        private int m_MaxWhiteBalls;            //Determines amount of white balls rolled per game
        private int m_MaxRedBalls;              //Determines amount of Powerballs rolled per game
        private bool m_GuaranteedJackpot;       //Chance to pick a jackpot - minimum 5 games without a winner, chances increasing each jackpotless game
        private int m_GameNumber;               //Current Game Number
        private bool m_HasProcessed;            //Has current game been processed?

        private TimeSpan m_GameDelay;           //Time inbetween Picks
        private TimeSpan m_DeadLine;            //Deadline before Picks you can purchase a ticket

        private static List<PowerBallSatellite> m_SatList = new List<PowerBallSatellite>();
        public static List<PowerBallSatellite> SatList { get { return m_SatList; } }

        private static PowerBallGame m_Instance;    //Convenience accessor for current game instance       
        private static int m_GoldSink;              //Profit - Payouts = Total goldsink
        private static PowerBall m_Game;            //Easily Accessed instance of the powerball item
        private static bool m_Announcement;

        [CommandProperty(AccessLevel.GameMaster)]
        public int TimeOfDay { get { return m_TimeOfDay; } set { m_TimeOfDay = value; m_NextGame = GetNextGameTime(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TicketCost { get { return m_TicketCost; } set { m_TicketCost = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TicketEntryPrice { get { return m_TicketEntryPrice; } set { m_TicketEntryPrice = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PowerBallPrice { get { return m_PowerBallPrice; } set { m_PowerBallPrice = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxWhiteBalls
        {
            get { return m_MaxWhiteBalls; }
            set { m_MaxWhiteBalls = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxRedBalls
        {
            get { return m_MaxRedBalls; }
            set { m_MaxRedBalls = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool GuaranteedJackpot { get { return m_GuaranteedJackpot; } set { m_GuaranteedJackpot = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan GameDelay { get { return m_GameDelay; } set { m_GameDelay = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan DeadLine { get { return m_DeadLine; } set { m_DeadLine = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int NoWins { get { if (m_Instance != null) return m_Instance.NoWins; return 0; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int GameNumber { get { return m_GameNumber; } set { m_GameNumber = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasProcessed { get { return m_HasProcessed; } set { m_HasProcessed = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public static bool Announcement { get { return m_Announcement; } set { m_Announcement = value; } }

        public static PowerBallGame Instance { get { return m_Instance; } set { m_Instance = value; } }
        //These 2 are backwards, but oh well!
        public static PowerBall Game { get { return m_Game; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public static int GoldSink { get { return m_GoldSink; } set { m_GoldSink = value; } }

        private DateTime m_NextGame;
        private bool m_Active;
        private bool m_DoJackpot;
        private Timer m_Timer;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastGame { get { return m_NextGame - GameDelay; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextGame { get { return m_NextGame; } set { m_NextGame = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool DoJackpot { get { return m_DoJackpot; } set { m_DoJackpot = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsActive
        {
            get { return m_Active; }
            set
            {
                m_Active = value;

                if (m_Active)
                {
                    if (m_Instance == null)
                        m_Instance = new PowerBallGame(this);

                    if (m_Timer != null)
                    {
                        m_Timer.Stop();
                        m_Timer = null;
                    }

                    m_NextGame = GetNextGameTime();
                    m_Timer = new PowerBallTimer(this);
                    m_Timer.Start();
                }
                else
                {
                    if (m_Timer != null)
                    {
                        m_Timer.Stop();
                        m_Timer = null;
                    }
                }
                UpdateSatellites();
                InvalidateProperties();
            }
        }
        #endregion

        [Constructable]
        public PowerBall()
            : base(0xED4)
        {
            if (PowerBallInstance != null)
            {
                Console.WriteLine("You can only have one shard PowerBall Item.");
                Delete();
                return;
            }

            Hue = 592;
            Name = "Let's Play Powerball!";
            m_Game = this;
            m_Instance = new PowerBallGame(this);
            m_Timer = new PowerBallTimer(this);
            m_Timer.Start();
            m_Active = true;
            Movable = false;
            m_HasProcessed = false;

            /*Modify below for custom values.  See readme file for info*/

            m_TicketCost = 100;
            m_TicketEntryPrice = 1000;
            m_PowerBallPrice = 500;
            m_MaxWhiteBalls = 20;
            m_MaxRedBalls = 8;
            m_GuaranteedJackpot = true;
            m_Announcement = true;
            m_GameNumber = 1;

            m_GameDelay = TimeSpan.FromHours(24);
            m_DeadLine = TimeSpan.FromMinutes(30);

            m_TimeOfDay = 18;
            m_NextGame = GetNextGameTime();

            PowerBallInstance = this;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!m_Active && from.AccessLevel == AccessLevel.Player)
                from.SendMessage("Powerball is currenlty inactive at this time.");
            else if (from.InRange(Location, 3))
            {
                if (from.HasGump(typeof(PowerBallStatsGump)))
                    from.CloseGump(typeof(PowerBallStatsGump));

                from.SendGump(new PowerBallStatsGump(this, from));
            }
            else if (from.AccessLevel > AccessLevel.Player)
                from.SendGump(new PropertiesGump(from, this));
            else
                from.SendLocalizedMessage(500446); // That is too far away.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!m_Active)
                list.Add(1060658, "Status\tOffline");
            else
                list.Add(1060658, "Status\tActive");

            if (PowerBallStats.JackpotStats.Count > 0)
            {
                int index = PowerBallStats.JackpotStats.Count - 1; //Most Recent Jackpot!

                string name = PowerBallStats.JackpotStats[index].JackpotWinner != null ? PowerBallStats.JackpotStats[index].JackpotWinner.Name : "unknown";
                list.Add(1060659, "Last Jackpot\t{0}", name);
                list.Add(1060660, "Date\t{0}", PowerBallStats.JackpotStats[index].JackpotTime);
                list.Add(1060661, "Amount\t{0}", PowerBallStats.JackpotStats[index].JackpotAmount);
            }

        }

        public DateTime GetNextGameTime()
        {
            DateTime now = DateTime.Now;
            DateTime nDT;

            if (now.Hour >= m_TimeOfDay && now.Second >= 0)
            {
                DateTime check = new DateTime(now.Year, now.Month, now.Day, m_TimeOfDay, 1, 0) + m_GameDelay;
                if (check < now)
                    nDT = now + m_GameDelay;
                else
                    nDT = check;
            }
            else
                nDT = new DateTime(now.Year, now.Month, now.Day, m_TimeOfDay, 0, 0);

            return nDT;
        }

        public void UpdateSatellites()
        {
            foreach (PowerBallSatellite sat in m_SatList)
            {
                if (sat != null && !sat.Deleted)
                    sat.InvalidateProperties();
            }
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            for (int i = 0; i < m_SatList.Count; ++i)
            {
                if (m_SatList[i] != null && !m_SatList[i].Deleted)
                    m_SatList[i].Delete();
            }

            base.OnAfterDelete();
        }

        public PowerBall(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); //Version

            writer.Write(m_TimeOfDay);
            writer.Write(m_TicketCost);
            writer.Write(m_TicketEntryPrice);
            writer.Write(m_PowerBallPrice);
            writer.Write(m_MaxWhiteBalls);
            writer.Write(m_MaxRedBalls);
            writer.Write(m_GuaranteedJackpot);
            writer.Write(m_GoldSink);
            writer.Write(m_NextGame);
            writer.Write(m_Active);
            writer.Write(m_GameNumber);
            writer.Write(m_GameDelay);
            writer.Write(m_DeadLine);
            writer.Write(m_Announcement);
            //writer.Write(m_HasProcessed);

            if (m_Instance == null)
                m_Instance = new PowerBallGame(this);

            writer.Write(m_Instance.Profit);
            writer.Write(m_Instance.NoWins);

            writer.Write(PowerBallStats.JackpotStats.Count);
            for (int i = 0; i < PowerBallStats.JackpotStats.Count; ++i)
            {
                PowerBallStats.JackpotStats[i].Serialize(writer);
            }
            writer.Write(PowerBallStats.PicksStats.Count);
            for (int i = 0; i < PowerBallStats.PicksStats.Count; ++i)
            {
                PowerBallStats.PicksStats[i].Serialize(writer);
            }

            InvalidateProperties();
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            m_Game = this;

            int version = reader.ReadInt();

            m_TimeOfDay = reader.ReadInt();
            m_TicketCost = reader.ReadInt();
            m_TicketEntryPrice = reader.ReadInt();
            m_PowerBallPrice = reader.ReadInt();
            m_MaxWhiteBalls = reader.ReadInt();
            m_MaxRedBalls = reader.ReadInt();
            m_GuaranteedJackpot = reader.ReadBool();
            m_GoldSink = reader.ReadInt();
            m_NextGame = reader.ReadDateTime();
            m_Active = reader.ReadBool();
            m_GameNumber = reader.ReadInt();
            m_GameDelay = reader.ReadTimeSpan();
            m_DeadLine = reader.ReadTimeSpan();
            m_Announcement = reader.ReadBool();

            if (version == 0)
                m_HasProcessed = reader.ReadBool();
            else
                m_HasProcessed = false;

            if (m_Active)
            {
                m_Timer = new PowerBallTimer(this);
                m_Timer.Start();
            }

            if (m_Instance == null)
                m_Instance = new PowerBallGame(this);

            m_Instance.Profit = reader.ReadInt();
            m_Instance.NoWins = reader.ReadInt();

            int jackpotCount = reader.ReadInt();
            for (int i = 0; i < jackpotCount; i++)
            {
                new PowerBallStats(reader);
            }
            int picksCount = reader.ReadInt();
            for (int i = 0; i < picksCount; i++)
            {
                new PowerBallStats(reader);
            }

            PowerBallInstance = this;
        }

        public static void AddToArchive(List<int> numbers, int payOut)
        {
            if (numbers == null || PowerBall.Game == null)
                return;

            new PowerBallStats(numbers, PowerBall.Game.GameNumber, payOut);
        }

        public static void AddToArchive(Mobile mob, int amount) //Jackpot entry
        {
            if (mob == null || PowerBall.Game == null)
                return;

            new PowerBallStats(mob, amount, PowerBall.Game.GameNumber);
            PowerBall.Game.UpdateSatellites();
        }

        public void AddToSatList(PowerBallSatellite satellite)
        {
            if (m_SatList != null && !m_SatList.Contains(satellite))
                m_SatList.Add(satellite);
        }

        public void RemoveFromSatList(PowerBallSatellite satellite)
        {
            if (m_SatList != null && m_SatList.Contains(satellite))
                m_SatList.Remove(satellite);
        }

        public void NewGame(bool hasjackpot)
        {
            if (hasjackpot || m_Instance == null)
                m_Instance = new PowerBallGame(this);
            else
            {
                int noWins = m_Instance.NoWins;
                int profit = m_Instance.Profit;

                m_Instance = new PowerBallGame(this, noWins, profit); //No jackpot? Sure! carry over how many non-wins and total profit
            }

            if (m_DoJackpot)
                m_DoJackpot = false;

            m_GameNumber++;
            m_NextGame = GetNextGameTime();
            m_HasProcessed = false;
        }
    }

    public class PowerBallTimer : Timer
    { 
        
        private PowerBall m_PowerBall;

        public PowerBallTimer(PowerBall pb) : base (TimeSpan.Zero, TimeSpan.FromSeconds(5)) 
        {
            m_PowerBall = pb;
            Priority = TimerPriority.FiveSeconds;
        }

        protected override void OnTick()
        {
            if (PowerBall.Instance == null || PowerBall.Game == null || m_PowerBall == null)
            {
                Stop();
                Console.WriteLine("There is a null error with Powerball.");
                return;
            }

            if (m_PowerBall.NextGame < DateTime.Now && !m_PowerBall.HasProcessed)
            {
                m_PowerBall.HasProcessed = true;
                PowerBall.Instance.ProcessGame();
            }
            else if (m_PowerBall.NextGame - TimeSpan.FromSeconds(30) < DateTime.Now)
            {
                int dur = 1000;

                foreach (PowerBallSatellite sat in PowerBall.SatList)
                {
                    if (sat != null && !sat.Deleted)
                    {
                        Effects.SendLocationEffect(new Point3D(sat.X, sat.Y + 1, sat.Z + 3), sat.Map, 0x373A, dur, 0, 0);
                        Effects.SendLocationEffect(new Point3D(sat.X + 1, sat.Y, sat.Z + 3), sat.Map, 0x373A, dur, 0, 0);
                        Effects.SendLocationEffect(new Point3D(sat.X, sat.Y, sat.Z + 2), sat.Map, 0x373A, dur, 0, 0);
                    }
                }

                Effects.SendLocationEffect(new Point3D(m_PowerBall.X, m_PowerBall.Y + 1, m_PowerBall.Z), m_PowerBall.Map, 0x373A, dur, 0, 0);
                Effects.SendLocationEffect(new Point3D(m_PowerBall.X + 1, m_PowerBall.Y, m_PowerBall.Z), m_PowerBall.Map, 0x373A, dur, 0, 0);
                Effects.SendLocationEffect(new Point3D(m_PowerBall.X, m_PowerBall.Y, m_PowerBall.Z - 1), m_PowerBall.Map, 0x373A, dur, 0, 0);
            }
        }
    }  
}