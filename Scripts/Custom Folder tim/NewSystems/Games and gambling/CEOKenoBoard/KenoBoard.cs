#undef PROFILE  // Leave this OFF unless you want to create a new theme and you
                // understand Keno and how this program works.
// Undefine if you are not using RunUo 2.0
#define RUNUO2RC1
using System;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;

/*
Package Name: CEOKenoBoard
Author: CEO
Version: 1.0b
Public Release: 01/26/06
Purpose: Las Vegas style Keno!

Description: This package allows you to add casino keno boards to your shard. You can choose
from 5 different themes that all give different main payout tables. Special "Casino Club" cards are given out to big jackpot winners allowing you to configure
wins over 500k. Each board's property will show latest big winner along with other interesting things.

			]add KenoBoard
*/

namespace Server.Items
{
    [DynamicFliping]
    [Flipable(0x1E5E, 0x1E5F)]
    public class KenoBoard : Item
    {
        public enum PayMethod { Standard, QuickPick, BottomHalf, TopHalf, RightHalf, LeftHalf, Odd, Even, Edges, Kool20, EZBucks, BancoSpecial, Millionare10 }
        public enum ThemeType { GoldCoast, Luxor, Stratosphere, TreasureIsland, WhiteDiamond }
        public enum VerboseType { Low, Medium, High }

        readonly ArrayList PlayerList = new ArrayList();

        private bool m_Active = false;
        private ThemeType m_Theme = ThemeType.TreasureIsland;
        private int m_ErrorCode;
        private int m_OrigHue;
        private bool m_AnnounceBigWins = true;
        private Mobile m_SecurityCamMobile = null; // Set to a mobile to "watch" people playing
        private VerboseType m_SecurityChatter = VerboseType.Low;
        private int m_GameSpeed;
        private const int MAXWIN = 2000000;
        private const int MAXUSERS = 50; // Max number of people that can be on one Keno game
        private int m_TotalPlayers = 0;

        //Stats and Totals
        //private int m_Won = 0;
        private bool m_ResetTotals = false;
        private ulong m_TotalCollected = 0;
        private ulong m_TotalWon = 0;
        //private long m_TotalNetProfit = 0;
        private ulong m_TotalSpins = 0;
        private readonly ulong[] m_HitStats = new ulong[21];
#if PROFILE
        private bool m_Profile = true;
#else
        private bool m_Profile = false;
#endif
        private bool m_ShowPlayerInfo;

        //Idle Info
        //private TimeSpan m_TimeOut;
        private readonly TimeSpan m_IdleTimeout = TimeSpan.FromMinutes(5); // How long can a person be standing at the machine not playing?
        private bool m_Throttle = true; // Force a delay between gumps to lesson bandwidth and macro effects
        private bool m_TimerActivated = false;
        private InternalTimer m_IdleTimer;
        private double m_ThrottleSeconds = 0.5;

        //Last Win info
        private Mobile m_LastWonBy = null;
        private DateTime m_LastWonByDate = DateTime.Now;
        private int m_LastWonAmount = 0;

        //ATM Stuff
        private int m_CreditCashOut = 250000;
        private int m_CreditATMLimit = 100000;
        private int m_CreditATMIncrements = 10000;

        //Issue a special "card" for big winners that could be used to get into special places
        private bool m_MembershipCard = true;
        private bool m_CardClubOnly = false; //Only club members can play this Keno Board

        #region Property
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return m_Active; }
            set
            {
                if (!m_Active && value)
                {
                    if (m_OrigHue != -1)
                    {
                        Hue = m_OrigHue;
                        m_OrigHue = -1;
                    }
                    Effects.SendLocationEffect(new Point3D(X, Y + 1, Z), Map, 0x373A, 15, Hue - 1, 0);
                    Effects.SendLocationEffect(new Point3D(X + 1, Y, Z), Map, 0x373A, 15, Hue - 1, 0);
                    Effects.SendLocationEffect(new Point3D(X, Y, Z - 1), Map, 0x373A, 15, Hue - 1, 0);
                    Effects.PlaySound(new Point3D(X, Y, Z), Map, 1481);
                    PublicOverheadMessage(0, (Hue == 907 ? 0 : Hue), false, "CEOKeno online!");
                    string text = String.Format("{0} online!", Name);
                    MessagePlayers(text);
                }
                else if (m_Active && !value)
                {
                    m_OrigHue = Hue;
                    Hue = 1001;
                    PublicOverheadMessage(0, Hue, false, "CEOKeno offline.");
                    string text = String.Format("{0} offline.", Name);
                    MessagePlayers(text);
                }
                m_Active = value;
                InvalidateProperties();
            }
        }

#if PROFILE
        [CommandProperty(AccessLevel.GameMaster)]
#endif
        public bool Profile
        {
            get { return m_Profile; }

            set { m_Profile = value; }
        }


        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowPlayerInfo
        {
            get { return m_ShowPlayerInfo; }
            set
            {
                if (value)
                {
                    GetPlayerInfo();
                }
                m_ShowPlayerInfo = false;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile SecurityCamMob
        {
            get { return m_SecurityCamMobile; }
            set { m_SecurityCamMobile = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public VerboseType SecurityChatter
        {
            get { return m_SecurityChatter; }
            set { m_SecurityChatter = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Throttle
        {
            get { return m_Throttle; }
            set { m_Throttle = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public double ThrottleSeconds
        {
            get { return m_ThrottleSeconds; }
            set
            {
                if (value > 5)
                    value = 5;
                if (value <= 0.25)
                    value = 0.25;
                m_ThrottleSeconds = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ThemeType Theme
        {
            get { return m_Theme; }
            set
            {
                if (m_Theme != value)
                {
                    bool active = m_Active;
                    if (m_Active)
                        InvalidatePlayers("This Keno board has changed themes.");
                    Active = false;
                    SetupTheme(value);
                    string text = String.Format("Theme Change: {0}", Name);
                    SecurityCamera(0, text);
                    if (m_OrigHue == -1)
                        m_OrigHue = Hue;
                    if (active)
                        Active = true;
                    m_Theme = value;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CreditCashOutAt
        {
            get { return m_CreditCashOut; }
            set { m_CreditCashOut = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CreditATMLimit
        {
            get { return m_CreditATMLimit; }
            set { m_CreditATMLimit = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CreditATMIncrements
        {
            get { return m_CreditATMIncrements; }
            set { m_CreditATMIncrements = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ErrorCode
        {
            get { return m_ErrorCode; }
            set { m_ErrorCode = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ResetTotals
        {
            get { return m_ResetTotals; }
            set
            {
                if (value)
                {
                    m_TotalCollected = 0;
                    m_TotalWon = 0;
                    m_TotalSpins = 0;
                    for (int i = 0; i < m_HitStats.Length; i++)
                        m_HitStats[i] = 0;
                }
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public long TotalNetProfit
        {
            get { return (long)(m_TotalCollected - m_TotalWon); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ulong TotalCollected
        {
            get { return m_TotalCollected; }
#if PROFILE
            set { m_TotalCollected = value; }
#endif
        }
        /*public ulong KenoTotalCollected
        {
            get { return m_TotalCollected; }
            set { m_TotalCollected = value; }
        }*/

        [CommandProperty(AccessLevel.GameMaster)]
        public ulong TotalWon
        {
            get { return m_TotalWon; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ulong TotalSpins
        {
            get { return m_TotalSpins; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public float WinningPercentage
        {
            get
            {
                if (m_TotalWon == 0 || m_TotalCollected == 0)
                    return 0;
                if (m_TotalCollected == 0)
                    return 0;
                return ((m_TotalWon / (float)m_TotalCollected) * 100.00f);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile LastWonBy
        {
            get { return m_LastWonBy; }
            set
            {
                m_LastWonBy = value;
                if (m_LastWonBy == null)
                {
                    m_LastWonAmount = 0;
                    m_LastWonByDate = DateTime.Now;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastWonByDate
        {
            get { return m_LastWonByDate; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LastWonAmount
        {
            get { return m_LastWonAmount; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool MembershipCard
        {
            get { return m_MembershipCard; }
            set { m_MembershipCard = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CardClubOnly
        {
            get { return m_CardClubOnly; }
            set { m_CardClubOnly = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats00
        {
            get { return m_HitStats[0]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats01
        {
            get { return m_HitStats[1]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats02
        {
            get { return m_HitStats[2]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats03
        {
            get { return m_HitStats[3]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats04
        {
            get { return m_HitStats[4]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats05
        {
            get { return m_HitStats[5]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats06
        {
            get { return m_HitStats[6]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats07
        {
            get { return m_HitStats[7]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats08
        {
            get { return m_HitStats[8]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats09
        {
            get { return m_HitStats[9]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats10
        {
            get { return m_HitStats[10]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats11
        {
            get { return m_HitStats[11]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats12
        {
            get { return m_HitStats[12]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats13
        {
            get { return m_HitStats[13]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats14
        {
            get { return m_HitStats[14]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats15
        {
            get { return m_HitStats[15]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats16
        {
            get { return m_HitStats[16]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats17
        {
            get { return m_HitStats[17]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats18
        {
            get { return m_HitStats[18]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats19
        {
            get { return m_HitStats[19]; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ulong zHitStats20
        {
            get { return m_HitStats[20]; }
        }

        #endregion
        [Constructable]
        public KenoBoard()
            : base(0x1E5E)
        {
            Light = LightType.Empty;
            Movable = false;
            m_Active = true;
            SetupTheme(ThemeType.TreasureIsland);
            m_OrigHue = Hue;
            m_Active = false;
            Hue = 1001;
            m_GameSpeed = Utility.RandomList(5, 6, 7, 8, 9, 10, 11, 12);
        }

        public byte[] PickNumbers(int count)
        {
            byte[] picks = new byte[count];
            bool[] numbers = new bool[80];
            for (int i = 0; i < count; i++)
            {
                bool selected = false;
                while (!selected)
                {
                    byte number = (byte)Utility.Random(80);
                    if (!numbers[number])
                    {
                        numbers[number] = true;
                        picks[i] = number;
                        selected = true;
                    }
                }
            }
            return picks;
        }

        public void IncrementStats(int numberhit)
        {
            m_HitStats[numberhit]++;
            m_TotalSpins++;
            if (m_TotalSpins % 1000 == 0 && !m_Profile)
                InvalidateProperties();
        }

        public int ProcessPlay(Mobile from, PlayerInfo player, int amount)
        {
            player.OnCredit += amount;
            if (player.Cost * 2 < amount && !m_Profile)
                DoWinSound(from, player.mobile.Female, amount);
            m_TotalWon += (ulong)amount;
            UpdateLastWonBy(from, amount);
            string text = null;
            if (amount > 499999 && !m_Profile)
            {
                if (m_MembershipCard)
                    IssueMembershipCard(from, amount);
                if (m_AnnounceBigWins)
                {
                    text = String.Format("{0} has won {1} gold on {2}!", from.Name, amount, Name);
                    AnnounceWin(from, text);
                }
            }
            text = String.Format("{0} wins {1}.", from.Name, amount);
            SecurityCamera(amount > 5000 ? 0 : 1, text);
            text = String.Format("OnCredit={1}.", player.mobile.Name, player.OnCredit);
            SecurityCamera(player.OnCredit > 10000 ? 1 : 2, text);
            return player.OnCredit;
        }

        public void DoWinSound(Mobile from, bool female, int amount)
        {
            string text = null;
            if (amount > 200)
            {
                text = String.Format("{0} wins {1}!", from.Name, amount);
                PublicOverheadMessage(0, (Hue == 907 ? 0 : Hue), false, text);
            }
            if (amount > 499999)
            {
                for (int i = 0; i < 5; i++)
                    DoFireworks(from);
                from.PlaySound(female ? 824 : 1098);
            }
            else if (amount > 249999)
            {
                DoFireworks(from);
                from.PlaySound(female ? 823 : 1097);
            }
            else if (amount > 149999)
            {
                DoFireworks(from);
                from.PlaySound(female ? 783 : 1054);
            }
            else if (amount > 999)
                from.PlaySound(female ? 794 : 1066);
            else if (amount > 50)
            {
                switch (Utility.Random(7))
                {
                    case 0:
                        from.PlaySound(female ? 794 : 1066);
                        break;
                    case 1:
                        from.PlaySound(female ? 797 : 1069);
                        break;
                    case 2:
                        from.PlaySound(female ? 783 : 1054);
                        break;
                    case 3:
                        from.PlaySound(female ? 823 : 1097);
                        break;
                    default:
                        break;
                }
            }
        }

        private void DoFireworks(Mobile m)
        {
            FireworksWand fwand = new FireworksWand();

            if (fwand != null && !fwand.Deleted)
            {
                try
                {
                    fwand.Parent = m;
                    fwand.BeginLaunch(m, true);
                    fwand.Delete();
                }
                catch { }
            }
        }

        private void AnnounceWin(Mobile from, string text)
        {
            foreach (Server.Network.NetState state in Server.Network.NetState.Instances)
            {
                Mobile m = state.Mobile;
                if (m != null)// && m != from)
                {
                    m.PlaySound(1460);
                    m.SendMessage(text);
                }
            }
        }

        public int OnCredit(Mobile from, PlayerInfo player, int amount)
        {
            if (from == null || player == null || from != player.mobile)
            {
                from.SendMessage("This game needs maintenance.");
                SecurityCamera(0, "This game needs maintenance.");
                Active = false;
                return player.OnCredit;
            }
            player.OnCredit += amount;
            if (amount < 0)
                m_TotalCollected += (ulong)Math.Abs(amount);
            return player.OnCredit;
        }

        public float[] GetPayTable(int method, int Cost, int totalselected)
        {
            //Console.WriteLine("(GetPayTable) method={0} totalselected={1}", method, totalselected);
            float[] paytable = new float[21];
            if (totalselected == 0) return paytable; // Return zeroed table when nothing is selected
            PayMethod paymethod = (PayMethod)method;
            if (paymethod >= PayMethod.BottomHalf && paymethod <= PayMethod.Even)
            {
                // Nevada - Las Vegas - The Orleans.
                paytable = new float[] { 100000, 20000, 2000, 200, 25, 5, 3, 1, 2f / 5f, 0, 0, 0, 2f / 5f, 1, 3, 5, 25, 200, 2000, 20000, 100000 };
                return AdjustedPayTable(paytable, 1000000, 3000000, Cost, totalselected);
            }
            switch (paymethod)
            {
                case PayMethod.Edges:
                    paytable = new float[] { 5000, 300, 60, 6, 2, 3f / 5f, 1f / 5f, 1f / 5f, 1f / 5f, 1f / 5f, 1f / 5f, 3f / 5f, 2, 10, 40, 200, 1000, 5000, 12000, 20000, 50000 };
                    return AdjustedPayTable(paytable, 2000000, 3000000, Cost, totalselected);
                case PayMethod.Kool20:
                    paytable = new float[] { 100, 2, 1, 1, 0, 0, 0, 1, 2, 5, 10, 40, 200, 1000, 2500, 5000, 10000, 40000, 200000, 200000, 200000 };
                    return AdjustedPayTable(paytable, 2500000, 5000000, Cost, totalselected);
                case PayMethod.EZBucks:
                    paytable = new float[] { 100, 2, 1, 1, 0, 0, 0, 0, 1, 2, 5, 10, 40, 200, 1000, 2500, 10000, 20000, 20000, 20000, 20000 };
                    return AdjustedPayTable(paytable, 2000000, 2000000, Cost, totalselected);
                case PayMethod.BancoSpecial:
                    paytable = new float[] { 100, 4, 2, 1, 0, 0, 0, 0, 1, 2, 4, 10, 100, 200, 2000, 200000, 200000, 200000, 200000, 200000, 200000 };
                    return AdjustedPayTable(paytable, 2500000, 10000000, Cost, totalselected);
                case PayMethod.Millionare10:
                    paytable = new float[] { 0, 0, 0, 0, 0, 1, 5, 50, 500, 10000, 1000000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    switch (Cost)
                    {
                        case 5:
                            paytable[10] = 400000;
                            break;
                        case 10:
                            paytable[10] = 300000;
                            break;
                        case 25:
                            paytable[10] = 200000;
                            break;
                        case 50:
                            paytable[10] = 150000;
                            break;
                        case 100:
                            paytable[10] = 100000;
                            break;
                    }
                    return paytable;
                case PayMethod.QuickPick:
                    goto case PayMethod.Standard;
                case PayMethod.Standard:
                    switch (m_Theme)
                    {
                        case ThemeType.GoldCoast:
                            {
                                switch (totalselected)
                                {
                                    case 1:
                                        paytable = new float[] { 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 2:
                                        paytable = new float[] { 0, 0, 12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 3:
                                        paytable = new float[] { 0, 0, 1, 45, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 4:
                                        paytable = new float[] { 0, 0, 1, 2, 160, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 5:
                                        paytable = new float[] { 0, 0, 0, 1, 15, 750, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 6:
                                        paytable = new float[] { 0, 0, 0, 1, 4, 80, 2000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 7:
                                        paytable = new float[] { 0, 0, 0, 0, 1, 15, 400, 10000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 8:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 5, 75, 1500, 50000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 9:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 6, 30, 300, 5000, 50000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 10:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 1, 25, 125, 1000, 10000, 100000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 11:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 2, 25, 650, 10000, 50000, 100000, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 12:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 5, 20, 200, 2000, 7500, 50000, 250000, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 13:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 2, 15, 40, 1000, 5000, 12500, 27500, 250000, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 14:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 2, 10, 40, 300, 750, 7500, 25000, 50000, 250000, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 15:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 0, 5, 25, 150, 1000, 5000, 25000, 100000, 250000, 500000, 0, 0, 0, 0, 0 };
                                        break;
                                }
                                return AdjustedPayTable(paytable, 2000000, 5000000, Cost, totalselected);
                            }
                        case ThemeType.Luxor:
                            {
                                switch (totalselected)
                                {
                                    case 1:
                                        paytable = new float[] { 0, 3.3f, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 2:
                                        paytable = new float[] { 0, 0, 13.2f, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 3:
                                        paytable = new float[] { 0, 0, 1.1f, 46.2f, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 4:
                                        paytable = new float[] { 0, 0, 1.1f, 3.5f, 132, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 5:
                                        paytable = new float[] { 0, 0, 0, 1.1f, 10, 900, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 6:
                                        paytable = new float[] { 0, 0, 0, 1.1f, 4.4f, 98, 1700, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 7:
                                        paytable = new float[] { 0, 0, 0, 0, 2.2f, 22, 400, 8000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 8:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 10, 100, 1700, 22000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 9:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 4.4f, 50, 330, 4400, 27500, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 10:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 2.2f, 22, 148, 1100, 4400, 27500, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 11:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 10, 85, 450, 2500, 14000, 27500, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 12:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 5.5f, 33, 275, 700, 2750, 13750, 27500, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 13:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 1.1f, 17, 90, 850, 4400, 9900, 27500, 50000, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 14:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 1.1f, 11, 47, 330, 850, 8500, 14000, 38500, 50000, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 15:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 0, 11, 28, 110, 330, 3300, 14000, 38500, 50000, 50000, 0, 0, 0, 0, 0 };
                                        break;
                                }
                                return AdjustedPayTable(paytable, 2000000, 5000000, Cost, totalselected);
                            }
                        case ThemeType.Stratosphere:
                            {
                                switch (totalselected)
                                {
                                    case 1:
                                        paytable = new float[] { 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 2:
                                        paytable = new float[] { 0, 0, 12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 3:
                                        paytable = new float[] { 0, 0, 1, 42, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 4:
                                        paytable = new float[] { 0, 0, 1, 3, 124, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 5:
                                        paytable = new float[] { 0, 0, 0, 1, 7, 850, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 6:
                                        paytable = new float[] { 0, 0, 0, 1, 3, 95, 1495, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 7:
                                        paytable = new float[] { 0, 0, 0, 0, 1, 15, 450, 8000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 8:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 8, 100, 1495, 22000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 9:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 3, 41, 350, 4250, 30000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 10:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 2, 20, 140, 850, 5000, 34000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 11:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 8, 75, 400, 2500, 10000, 50000, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 12:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 5, 32, 250, 625, 2500, 13000, 100000, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 13:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 2, 16, 70, 700, 3500, 10000, 30000, 100000, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 14:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 1, 10, 40, 300, 800, 7500, 12500, 35000, 50000, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 15:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 0, 10, 25, 100, 275, 2500, 10000, 30000, 50000, 100000, 0, 0, 0, 0, 0 };
                                        break;
                                }
                                return AdjustedPayTable(paytable, 2000000, 5000000, Cost, totalselected);
                            }
                        case ThemeType.TreasureIsland:
                            {
                                switch (totalselected)
                                {
                                    case 1:
                                        paytable = new float[] { 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 2:
                                        paytable = new float[] { 0, 0, 12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 3:
                                        paytable = new float[] { 0, 0, 1, 42, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 4:
                                        paytable = new float[] { 0, 0, 1, 3, 115, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 5:
                                        paytable = new float[] { 0, 0, 0, 1, 23, 500, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 6:
                                        paytable = new float[] { 0, 0, 0, 1, 3, 88, 1500, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 7:
                                        paytable = new float[] { 0, 0, 0, 0, 1, 20, 400, 6000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 8:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 8, 90, 1500, 20000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 9:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 3, 44, 300, 4000, 25000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 10:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 1, 22, 132, 960, 3800, 40000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 11:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 9, 75, 400, 2250, 12500, 45000, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 12:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 5, 30, 250, 600, 2500, 12500, 50000, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 13:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 1, 15, 80, 750, 4000, 9000, 25000, 50000, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 14:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 1, 10, 42, 300, 750, 7500, 12500, 35000, 50000, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 15:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 0, 0, 10, 25, 120, 300, 2800, 12500, 35000, 50000, 50000, 0, 0, 0, 0, 0 };
                                        break;
                                }
                                return AdjustedPayTable(paytable, 2000000, 5000000, Cost, totalselected);
                            }
                        case ThemeType.WhiteDiamond:
                            {
                                switch (totalselected)
                                {
                                    case 1:
                                        paytable = new float[] { 0, 3.4f, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 2:
                                        paytable = new float[] { 0, .7f, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 3:
                                        paytable = new float[] { 0, .25f, 1.75f, 35, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 4:
                                        paytable = new float[] { 0, .2f, 1, 6, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 5:
                                        paytable = new float[] { 0, 0, .2f, 4, 20, 400, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 6:
                                        paytable = new float[] { 0, 0, 0, 1, 10, 100, 1000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 7:
                                        paytable = new float[] { 0, 0, 0, 0, 0.35f, 50, 350, 5000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 8:
                                        paytable = new float[] { 0, 0, 0, 0.1f, 1.5f, 10, 150, 1000, 10000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 9:
                                        paytable = new float[] { 0, 0, 0, 0, 0.5f, 12.5f, 50, 125, 2500, 12500, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 10:
                                        paytable = new float[] { 0, 0, 0, 0, .22f, 2.2f, 22, 250, 500, 4500, 20000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 11:
                                        paytable = new float[] { 0, 0, 0, 0, 0, 1, 5, 30, 1250, 3000, 12500, 30000, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 12:
                                        paytable = new float[] { 0, 0, 0, 0, 0, .2f, 4, 60, 200, 600, 6000, 20000, 40000, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 13:
                                        paytable = new float[] { 5, 0, 0, 0, 0, 0, 5, 10, 100, 500, 5000, 10000, 25000, 50000, 0, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 14:
                                        paytable = new float[] { 7.5f, 0, 0, 0, 0, .25f, 1, 10, 25, 500, 1000, 10000, 20000, 30000, 50000, 0, 0, 0, 0, 0, 0 };
                                        break;
                                    case 15:
                                        paytable = new float[] { 10, 0, 0, 0, 0, .35f, 1.25f, 5, 35, 100, 350, 5000, 12500, 35000, 50000, 100000, 0, 0, 0, 0, 0 };
                                        break;
                                }
                                return AdjustedPayTable(paytable, 5000000, 10000000, Cost, totalselected);
                            }
                    }
                    break;

                default:
                    for (int i = 0; i < 21; i++)
                        paytable[i] = 0;
                    break;
            }
            for (int i = 0; i < 21; i++)
            {
                if ((Cost * paytable[i]) > MAXWIN)
                    paytable[i] = MAXWIN / Cost;
            }
            return paytable;
        }

        public float[] AdjustedPayTable(float[] paytable, int high, int winhigh, int Cost, int totalselected)
        {
            totalselected = (totalselected > 15) ? 20 : totalselected;
            for (int i = 0; i < 21; i++)
            {
                if (i == 0 || i == totalselected)
                {
                    if ((Cost * paytable[i]) > winhigh)
                        paytable[i] = winhigh / Cost;
                }
                else if ((Cost * paytable[i]) > high)
                    paytable[i] = high / Cost;
            }
            return paytable;
        }

        public void ClearBoard(Mobile from, PlayerInfo player)
        {
            if (from != player.mobile)
            {
                from.SendMessage("This game needs maintenance.");
                SecurityCamera(0, "This game needs maintenance.");
                Active = false;
                return;
            }
            PayMethod PayTable = PayMethod.QuickPick;
            byte[] Selected = new byte[80];
            byte[] MachinePicks = new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 };
            byte TotalSelected = 15;
            byte[] TPicks = PickNumbers(TotalSelected);
            for (int i = 0; i < TotalSelected; i++)
                Selected[TPicks[i]] = 1;
            from.CloseGump(typeof(KenoGump));
            from.SendGump(new KenoGump(this, from, player, false, (int)PayTable, GetPayTable((int)PayTable, player.Cost, TotalSelected), 2, TotalSelected, false, Selected, TotalSelected, MachinePicks));
            player.LastPlayed = DateTime.Now;
        }


        public void DoCashOut(Mobile from, PlayerInfo player)
        {

            if (from != player.mobile)
            {
                from.SendMessage("This game needs maintenance.");
                SecurityCamera(0, "This game needs maintenance.");
                Active = false;
                return;
            }
            int credit = player.OnCredit;
            if (from == null || credit == 0)
                return;
            if (!m_Active && (m_ErrorCode == 9500 || m_ErrorCode == 9501 || m_ErrorCode == 9502)) // Prevent a loop cashing out
                return;
            if (from != player.mobile)
            {
                from.SendMessage("You are no longer playing this game!");
                return;
            }
            if (player.OnCredit < 0) // This should never happen but protect against some kind of overflow and a wild payout
            {
                if (from.AccessLevel >= AccessLevel.GameMaster) // Allow a GM to clear out the invalid amount
                {
                    from.SendMessage("Invalid gold won amount({0}), reset to 0.", player.OnCredit);
                    player.OnCredit = 0;
                }
                from.SendMessage("There's a problem with this machine's gold amount, this game is offline. Page for help.");
                KenoOffline(9502);
                return;
            }
            if (player.OnCredit < 1000)
            {
                try
                {
                    from.AddToBackpack(new Gold(player.OnCredit));
                    from.SendMessage("{0} gold has been added to your pack.", credit);
                }
                catch
                {
                    from.SendMessage("There's a problem returning your gold, this game is offline. Page for help.");
                    KenoOffline(9500);
                    return;
                }
            }
            else
            {
                try
                {
                    from.AddToBackpack(new BankCheck(player.OnCredit));
                    from.SendMessage("A bank check for {0} gold has been placed in your pack.", credit);
                }
                catch
                {
                    from.SendMessage("There's a problem returning your gold, this game is offline. Page for help.");
                    KenoOffline(9501);
                    return;
                }

            }
            player.OnCredit = 0;
            string text = null;
            if (credit >= 10000)
            {
                text = String.Format("{0} is cashing out {1} Gold!", from.Name, credit);
                PublicOverheadMessage(0, (Hue == 907 ? 0 : Hue), false, text);
            }
            text = String.Format("{0} is cashing out {1} Gold!", from.Name, credit);
            SecurityCamera(credit >= 10000 ? 0 : 1, text);
            from.PlaySound(52);
            from.PlaySound(53);
            from.PlaySound(54);
            from.PlaySound(55);
        }

        public bool CashCheck(Mobile m, PlayerInfo player, out int amount)
        {
            amount = 0;
            if (m != player.mobile)
            {
                m.SendMessage("This game needs maintenance.");
                SecurityCamera(0, "This game needs maintenance.");
                Active = false;
                return false;
            }
            if (m == null || m.Backpack == null || player == null)
                return false;
#if RUNUO2RC1
			List<Item> packlist = m.Backpack.Items;
#else
			ArrayList packlist = m.Backpack.Items;
#endif

            for (int i = 0; i < packlist.Count; ++i)
            {
                Item item = packlist[i];
                if (item != null && !item.Deleted && item is BankCheck)
                {
                    amount = ((BankCheck)item).Worth;
                    item.Delete();
                    if (item.Deleted)
                    {
                        string text = null;
                        Effects.PlaySound(new Point3D(X, Y, Z), Map, 501);
                        player.OnCredit += amount;
                        text = String.Format("{0}:Check={1}.", player.mobile.Name, amount);
                        SecurityCamera(amount > 5000 ? 0 : 1, text);
                        text = String.Format("OnCredit={1}.", player.mobile.Name, player.OnCredit);
                        SecurityCamera(player.OnCredit > 10000 ? 1 : 2, text);
                    }
                    else
                    {
                        m.SendMessage("There's a problem trying to cash a check in your backpack, this game is offline. Page for help.");
                        KenoOffline(9503);
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        private void SetupTheme(ThemeType theme)
        {
            switch (theme)
            {
                case ThemeType.GoldCoast:
                    Name = "Gold Coast Keno";
                    if (m_Active)
                        Hue = 2213;
                    else
                        m_OrigHue = 2213;
                    break;
                case ThemeType.Luxor:
                    Name = "Luxor Keno";
                    if (m_Active)
                        Hue = 907;
                    else
                        m_OrigHue = 907;
                    break;
                case ThemeType.Stratosphere:
                    Name = "Stratosphere Keno";
                    if (m_Active)
                        Hue = 1160;
                    else
                        m_OrigHue = 1160;
                    break;
                case ThemeType.TreasureIsland:
                    Name = "Treasure Island Keno";
                    if (m_Active)
                        Hue = 643;
                    else
                        m_OrigHue = 643;
                    break;

                case ThemeType.WhiteDiamond:
                    Name = "White Diamond Keno";
                    if (m_Active)
                        Hue = 1150;
                    else
                        m_OrigHue = 1150;
                    //Hue = 1150;
                    break;

            }
            m_Theme = theme;
        }

        private void KenoOffline(int error)
        {
            InvalidatePlayers("A critical error has forced this game offline, notify a GameMaster please.");
            string text = String.Format("Critical Error: {0}", error);
            SecurityCamera(0, text);
            m_ErrorCode = error;
            Active = false;
        }

        public bool SearchForPlayer(Mobile from, out PlayerInfo player)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                player = (PlayerInfo)PlayerList[i];
                if (player.mobile == from)
                {
                    player.LastPlayed = DateTime.Now;
                    return true;
                }
            }
            player = null;
            return false;
        }

        public PlayerInfo AddNewPlayer(Mobile from)
        {
            if (PlayerList.Count > MAXUSERS)
            {
                from.SendMessage("This game is full, try again later.");
                return null;
            }
            for (int i = 0; i < PlayerList.Count; i++)
            {
                PlayerInfo p = (PlayerInfo)PlayerList[i];
                if (p.mobile == null)
                {
                    p.mobile = from;
                    p.LastPlayed = DateTime.Now;
                    p.OnCredit = 0;
                    p.Cost = 1;
                    m_TotalPlayers++;
                    InvalidateProperties();
                    return p;
                }
            }
            PlayerInfo player = new PlayerInfo();
            player.mobile = from;
            player.LastPlayed = DateTime.Now;
            player.OnCredit = 0;
            InvalidateProperties();
            player.Cost = 1;
            PlayerList.Add(player);
            m_TotalPlayers++;
            InvalidateProperties();
            /*for (int i = 0; i < PlayerList.Count; i++)
            {
                PlayerInfo tplayer = (PlayerInfo) PlayerList[i];
                //Console.WriteLine("(AddNewPlayer) Added/{0}/{1}", i, tplayer.mobile);
            }*/
            return player;
        }

        public bool RemovePlayer(Mobile from, PlayerInfo player)
        {
            if (from != player.mobile)
            {
                from.SendMessage("This game needs maintenance.");
                SecurityCamera(0, "This game needs maintenance.");
                Active = false;
                return false;
            }
            string text = String.Format("Removing: {0}.", player.mobile.Name);
            SecurityCamera(0, text);
            if (player.OnCredit != 0)
                DoCashOut(from, player);
            PlayerList.Remove(player);
            m_TotalPlayers--;
            InvalidateProperties();
            return true;
        }

        private void UpdateLastWonBy(Mobile m, int winamount)
        {
#if PROFILE
            if (m_Profile)
                return;
#endif

            if (m_LastWonBy == null || m_LastWonBy.Deleted)
            {
                m_LastWonBy = m;
                m_LastWonByDate = DateTime.Now;
                m_LastWonAmount = winamount;
            }
            else
            {
                TimeSpan timespan = DateTime.Now - m_LastWonByDate;
                if (m_LastWonAmount <= winamount || TimeSpan.FromDays(30) < timespan)
                {
                    m_LastWonBy = m;
                    m_LastWonByDate = DateTime.Now;
                    m_LastWonAmount = winamount;
                }
            }
            InvalidateProperties();
        }

        public void RemoveIdlePlayers()
        {
            SecurityCamera(1, "Removing Idle Players.");
            for (int i = 0; i < PlayerList.Count; i++)
            {
                PlayerInfo player = (PlayerInfo)PlayerList[i];

                if (player != null && !player.mobile.Deleted && DateTime.Now - player.LastPlayed > m_IdleTimeout)
                {
                    player.mobile.CloseGump(typeof(KenoGump));
                    player.mobile.CloseGump(typeof(KenoPayTableGump));
                    player.mobile.SendMessage("You have been idle too long.");
                    RemovePlayer(player.mobile, player);
                    player.mobile.SendMessage("You quit playing {0}.", Name);
                }
            }
            if (m_TotalPlayers != 0)
                ActivateIdleTimer(TimeSpan.FromMinutes(10));
            else
                m_TimerActivated = false;
        }

        private void InvalidatePlayers(string text)
        {
            SecurityCamera(0, "Removing all Players.");
            for (int i = 0; i < PlayerList.Count; i++)
            {
                PlayerInfo player = (PlayerInfo)PlayerList[i];
                if (player.OnCredit < 0)
                {
                    Console.WriteLine("{0} has an invalidate credit amount for {1}({2}). Set to 0.", Name, player.mobile.Name, player.OnCredit);
                    player.OnCredit = 0;
                }
                if (player.OnCredit != 0)
                    DoCashOut(player.mobile, player);
                player.mobile.CloseGump(typeof(KenoGump));
                player.mobile.CloseGump(typeof(KenoPayTableGump));
                if (text != null)
                    player.mobile.SendMessage(text);
            }
            PlayerList.Clear();
            m_TotalPlayers = 0;
            InvalidateProperties();
        }

        private void MessagePlayers(string text)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                PlayerInfo player = (PlayerInfo)PlayerList[i];
                player.mobile.SendMessage(text);
            }
        }

        private void GetPlayerInfo()
        {
            string text;
            for (int i = 0; i < PlayerList.Count; i++)
            {
                PlayerInfo player = (PlayerInfo)PlayerList[i];
                if (player != null && player.mobile != null && player.mobile.AccessLevel != AccessLevel.Player)
                {
                    for (int j = 0; j < PlayerList.Count; j++)
                    {
                        PlayerInfo tplayer = (PlayerInfo)PlayerList[j];
                        text = String.Format("Name:{0} OnCredit:{1} Cost:{2} LastPlayed:{3}", tplayer.mobile.Name, tplayer.OnCredit, tplayer.Cost, tplayer.LastPlayed);
                        player.mobile.SendMessage(text);
                    }

                }
            }
        }

        private void IssueMembershipCard(Mobile to, int win)
        {
            Item item = null;
            item = new CasinoMembershipCard();
            if (item != null)
            {
                ((CasinoMembershipCard)item).ClubMember = to;
                ((CasinoMembershipCard)item).Game = Name;
                ((CasinoMembershipCard)item).Jackpot = win;
                GiveItem(to, item);
            }
            else
            {
                InvalidatePlayers("A critical error has forced this game offline, notify a GameMaster please.");
                KenoOffline(10011);
            }
        }

        private bool CarryingClubCard(Mobile m)
        {
            if (m.Backpack == null)
                return false;
#if RUNUO2RC1
			List<Item> packlist = m.Backpack.Items;
#else
			ArrayList packlist = m.Backpack.Items;
#endif
            for (int i = 0; i < packlist.Count; ++i)
            {
                Item item = packlist[i];
                if (item != null && !item.Deleted && item is CasinoMembershipCard)
                {
                    if (((CasinoMembershipCard)item).ClubMember == m)
                        return true;
                }
            }
            return false;
        }

        private void GiveItem(Mobile to, Item i)
        {
            if (to == null || i == null)
                return;

            Container pack = to.Backpack;
            string text = null;
            if (pack != null)
            {
                if (pack.TryDropItem(to, i, false))
                {
                    text = String.Format("{0} {1} has been placed in your backpack!", i.Amount > 1 ? "Some" : "A", i.Name);
                }
                else
                {
                    to.BankBox.DropItem(i);
                    text = String.Format("{0} {1} has been placed in your bankbox!", i.Amount > 1 ? "Some" : "A", i.Name);
                }
            }
        }

        public void SecurityCamera(int chatter, string text)
        {
            if (m_SecurityCamMobile == null || m_SecurityCamMobile.Deleted)
                return;
            if (chatter > (int)m_SecurityChatter)
                return;
            if (m_SecurityCamMobile.Player)
                m_SecurityCamMobile.SendMessage(text);
            else
                m_SecurityCamMobile.PublicOverheadMessage(0, (Hue == 907 ? 0 : Hue), false, text);
        }

        public void ProfileTable(KenoBoard Keno, KenoBoard.PlayerInfo player, bool played, int PayTable, float[] PayOutTable, int CPayMethod, byte QuickPickCount, bool PickButtons, byte[] Selected, byte TotalSelected, byte[] MachinePicks)
        {
#if PROFILE
            ResetTotals = true;
            if (TotalSelected == 0)
            {
                player.mobile.SendMessage("You must select some numbers on the Keno card before profiling!");
                return;
            }
            DateTime startdt = DateTime.Now;
            Console.WriteLine("Begin-Date/Time: {0}", startdt);
            Console.WriteLine("Profiling Keno Board. Theme: {0}", m_Theme);
            for (int o = 0; o < 3; o++)
            {
                for (int p = 0; p < 1000000; p++)
                {
                    byte totalhit = 0;
                    OnCredit(player.mobile, player, -player.Cost);
                    MachinePicks = PickNumbers(20);
                    for (int h = 0; h < 20; h++)
                    {
                        if (MachinePicks[h] != 255 && Selected[MachinePicks[h]] == 1)
                            totalhit++;
                    }
                    int won = (int)(player.Cost * PayOutTable[totalhit]);
                    if (won != 0)
                    {
                        ProcessPlay(player.mobile, player, won);
                    }
                    IncrementStats(totalhit);
                }
            }
            DateTime enddt = DateTime.Now;
            TimeSpan elapsed = enddt - startdt;
            Console.WriteLine("Done-Date/Time: {0}  Elapsed: {1}", enddt, elapsed);
#endif
        }


        public override void OnDelete()
        {
            base.OnDelete();
            InvalidatePlayers("Keno machine has been deleted.");
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            if (!m_Active)
            {
                if (m_ErrorCode == 0)
                    list.Add(1060658, "Status\tOffline");
                else
                    list.Add(1060658, "Status\tMaintenance Required({0})", m_ErrorCode);
                return;
            }
            else
            {
                if (m_CardClubOnly)
                    list.Add(1060658, "Status\tAvailable (Captain's Cabin Only)");
                else
                    list.Add(1060658, "Status\tAvailable");
            }
            list.Add(1060659, "Total Players\t{0}", m_TotalPlayers);
            list.Add(1060660, "Game Number\t{0}", (int)(DateTime.Now.TimeOfDay.TotalMinutes / m_GameSpeed) + 1);
            if (m_LastWonBy != null)
            {
                list.Add(1060661, "Last Big Win\t{0}", m_LastWonBy.Name);
                list.Add(1060662, "Date\t{0}", m_LastWonByDate);
                list.Add(1060663, "Amount\t{0}", m_LastWonAmount);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerInfo player;

            if (!m_Active)
            {
                from.SendMessage("This game is offline.");
                return;
            }
            if (m_CardClubOnly && !CarryingClubCard(from))
            {
                from.SendMessage("You must be carrying your Captain's Cabin Membership Card to play {0}.", Name);
                return;
            }
            if (!from.Alive)
            {
                from.SendMessage("Ghosts can not play this game.");
                return;
            }
            if (!from.InRange(GetWorldLocation(), 9) || !from.InLOS(this))
            {
                from.SendMessage("You are too far away from {0} or it is blocked from your location.", Name);
                return;
            }
            if (from.Hidden && from.AccessLevel == AccessLevel.Player) // Don't let someone sit at the KenoBoard and play hidden
            {
                from.Hidden = false;
                from.SendMessage("Playing {0} reveals you!", Name);
            }
            if (SearchForPlayer(from, out player))
            {
                //Console.WriteLine("Player Found");
                ClearBoard(from, player);
                string text = String.Format("Adding(e): {0}.", player.mobile.Name);
                SecurityCamera(0, text);
            }
            else
            {
                player = AddNewPlayer(from);
                if (player != null)
                {
                    ClearBoard(from, player);
                    string text = String.Format("Adding(n): {0}.", player.mobile.Name);
                    SecurityCamera(0, text);
                }
            }
            if (!m_TimerActivated)
                ActivateIdleTimer(TimeSpan.FromSeconds(5));
        }

        public KenoBoard(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version 
            writer.Write(m_Active);
            writer.Write((int)m_Theme);
            writer.Write(m_TotalSpins);
            writer.Write(m_TotalCollected);
            writer.Write(m_TotalWon);
            writer.Write(m_ErrorCode);
            writer.Write(m_OrigHue);
            writer.Write(m_Throttle);
            writer.Write(m_ThrottleSeconds);
            writer.Write(m_CardClubOnly);
            writer.Write(m_MembershipCard);

            writer.Write(m_LastWonBy);
            writer.Write(m_LastWonByDate);
            writer.Write(m_LastWonAmount);

            writer.Write(m_SecurityCamMobile);
            writer.Write((int)m_SecurityChatter);
            writer.Write(PlayerList.Count);
            if (PlayerList.Count > 0)
            {
                for (int i = 0; i < PlayerList.Count; i++)
                {
                    PlayerInfo player = (PlayerInfo)PlayerList[i];
                    writer.Write(player.mobile);
                    writer.Write(player.LastPlayed);
                    writer.Write(player.OnCredit);
                    writer.Write(player.Cost);
                }
            }

            for (int i = 0; i < m_HitStats.Length; i++)
                writer.Write(m_HitStats[i]);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Active = reader.ReadBool();
            m_Theme = (ThemeType)reader.ReadInt();
            m_TotalSpins = reader.ReadULong();
            m_TotalCollected = reader.ReadULong();
            m_TotalWon = reader.ReadULong();
            m_ErrorCode = reader.ReadInt();
            m_OrigHue = reader.ReadInt();
            m_Throttle = reader.ReadBool();
            m_ThrottleSeconds = reader.ReadDouble();
            m_CardClubOnly = reader.ReadBool();
            m_MembershipCard = reader.ReadBool();

            m_LastWonBy = reader.ReadMobile();
            m_LastWonByDate = reader.ReadDateTime();
            m_LastWonAmount = reader.ReadInt();

            m_SecurityCamMobile = reader.ReadMobile();
            m_SecurityChatter = (VerboseType)reader.ReadInt();
            m_TotalPlayers = reader.ReadInt();
            InvalidateProperties();
            if (m_TotalPlayers > 0)
            {
                for (int i = 0; i < m_TotalPlayers; i++)
                {
                    PlayerInfo player = new PlayerInfo();

                    player.mobile = reader.ReadMobile();
                    player.LastPlayed = reader.ReadDateTime();
                    player.OnCredit = reader.ReadInt();
                    player.Cost = reader.ReadInt();
                    PlayerList.Add(player);
                }
            }

            for (int i = 0; i < m_HitStats.Length; i++)
                m_HitStats[i] = reader.ReadULong();

            if (m_OrigHue != -1 && m_Active)
            {
                Hue = m_OrigHue;
                m_OrigHue = -1;
            }
            SetupTheme(m_Theme);
            ActivateIdleTimer(TimeSpan.FromMinutes(5));
            m_GameSpeed = Utility.RandomList(5, 6, 7, 8, 9, 10, 11, 12);
        }

        public class PlayerInfo
        {
            public Mobile mobile;
            public DateTime LastPlayed;
            public int OnCredit;
            public int Cost;

            public void IncCost()
            {
                switch (Cost)
                {
                    case 1: Cost = 5;
                        break;
                    case 5: Cost = 10;
                        break;
                    case 10: Cost = 25;
                        break;
                    case 25: Cost = 50;
                        break;
                    case 50: Cost = 100;
                        break;
                    case 100: Cost = 1;
                        break;
                }
            }

            public void DecCost()
            {
                switch (Cost)
                {
                    case 1: Cost = 100;
                        break;
                    case 5: Cost = 1;
                        break;
                    case 10: Cost = 5;
                        break;
                    case 25: Cost = 10;
                        break;
                    case 50: Cost = 25;
                        break;
                    case 100: Cost = 50;
                        break;
                }
            }
        }

        public void ActivateIdleTimer(TimeSpan delay)
        {

            m_TimerActivated = true;

            if (m_IdleTimer != null)
                m_IdleTimer.Stop();

            m_IdleTimer = new InternalTimer(this, delay);
            m_IdleTimer.Start();
        }

        private class InternalTimer : Timer
        {
            private readonly KenoBoard m_Keno;

            public InternalTimer(KenoBoard Keno, TimeSpan delay)
                : base(delay)
            {
                Priority = TimerPriority.OneMinute;
                m_Keno = Keno;
            }

            protected override void OnTick()
            {
                if (m_Keno != null && !m_Keno.Deleted)
                    m_Keno.RemoveIdlePlayers();
            }
        }

    }

}