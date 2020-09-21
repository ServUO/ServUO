using Server.Accounting;
using Server.Engines.Points;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System.Globalization;

namespace Server.Engines.ResortAndCasino
{
    public enum Section
    {
        None,
        Buying,
        Selling,
        Error
    }

    public class PurchaseCasinoChipGump : Gump
    {
        public int Yellow => C32216(0xFFFF00);
        public Section Section { get; set; }
        public int Message { get; set; }
        public int Bought { get; set; }
        public int CashedOut { get; set; }

        public PlayerMobile User { get; set; }

        public PurchaseCasinoChipGump(PlayerMobile pm)
            : base(50, 50)
        {
            User = pm;
            AddGumpLayout();
        }

        public void AddGumpLayout()
        {
            AddBackground(0, 0, 430, 230, 1460);
            AddHtmlLocalized(20, 20, 200, 16, 1153172, Yellow, false, false);

            Account a = User.Account as Account;

            long total = a == null ? 0 : (long)(a.TotalCurrency * Account.CurrencyThreshold);
            int chips = (int)PointsSystem.CasinoData.GetPoints(User);

            switch (Section)
            {
                case Section.None:
                    int y = 50;

                    if (Bought > 0 || CashedOut > 0)
                    {
                        int amount = Bought > 0 ? Bought : CashedOut;
                        AddHtmlLocalized(20, y, 200, 16, 1153188, Yellow, false, false); // Transaction successful:
                        y += 20;
                        AddHtmlLocalized(20, y, 200, 16, Bought > 0 ? 1153189 : 1153190, amount.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Yellow, false, false); // You purchased ~1_AMT~ chips
                        y += 30;
                    }

                    AddHtmlLocalized(20, y, 200, 16, 1153173, Yellow, false, false); // Your Bank Balance:
                    AddHtml(300, y, 150, 16, Color("#FF69B4", total.ToString("N0", CultureInfo.GetCultureInfo("en-US"))), false, false);
                    y += 20;

                    AddHtmlLocalized(20, y, 200, 16, 1153174, Yellow, false, false); // Your Chip Balance:
                    AddHtml(300, y, 150, 16, Color("#FF69B4", chips.ToString("N0", CultureInfo.GetCultureInfo("en-US"))), false, false);
                    y += 30;

                    AddHtmlLocalized(20, y, 200, 16, 1153176, Yellow, false, false); // Buy Chips
                    AddButton(300, y, 4014, 4016, 1, GumpButtonType.Reply, 0);

                    y += 20;

                    AddHtmlLocalized(20, y, 200, 16, 1153175, Yellow, false, false); // Cash Out
                    AddButton(300, y, 4014, 4016, 2, GumpButtonType.Reply, 0);

                    break;
                case Section.Buying:
                    AddHtmlLocalized(20, 50, 180, 16, 1153183, Yellow, false, false); // Each casino chip costs 100gp
                    AddHtmlLocalized(20, 80, 180, 16, 1153186, Yellow, false, false); // Number of chips to buy:

                    AddBackground(215, 80, 200, 20, 9350);
                    AddTextEntry(216, 80, 199, 20, 0, 0, "");
                    AddButton(215, 110, 4005, 4007, 3, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(20, 110, 150, 16, 1153176, Yellow, false, false);
                    break;
                case Section.Selling:
                    AddHtmlLocalized(20, 50, 180, 16, 1153184, Yellow, false, false); // You will receive 100gp for each chip
                    AddHtmlLocalized(20, 80, 180, 16, 1153185, Yellow, false, false); // Number of chips to cash out:
                    AddBackground(215, 80, 200, 20, 9350);
                    AddTextEntry(216, 80, 199, 20, 0, 1, "");
                    AddButton(215, 110, 4005, 4007, 4, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(20, 110, 150, 16, 1153175, Yellow, false, false); // CASH OUT
                    break;
                case Section.Error:
                    AddHtmlLocalized(20, 50, 390, 40, 1153177, Yellow, false, false); // There was a problem completing your transaction:
                    AddHtmlLocalized(20, 90, 390, 150, Message, Yellow, false, false);
                    break;

            }

            if (Section == Section.None)
            {
                AddButton(15, 195, 4005, 4007, 0, GumpButtonType.Reply, 0);
                AddHtml(55, 193, 150, 16, Color("#FFFF00", "CLOSE"), false, false);
            }
            else
            {
                AddButton(15, 195, 4005, 4007, 5, GumpButtonType.Reply, 0);
                AddHtml(55, 193, 150, 16, Color("#FFFF00", "BACK"), false, false);
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 1:
                    Section = Section.Buying;
                    Refresh();
                    break;
                case 2:
                    Section = Section.Selling;
                    Refresh();
                    break;
                case 3:
                    TextRelay tr = info.GetTextEntry(0);

                    if (tr != null)
                    {
                        string text = tr.Text;
                        int num = Utility.ToInt32(text);

                        if (num > 0)
                        {
                            if (Banker.Withdraw(User, num * CasinoData.ChipCost, true))
                            {
                                PointsSystem.CasinoData.AwardPoints(User, num);
                                Bought = num;

                                Section = Section.None;

                                Refresh();
                            }
                            else
                            {
                                Section = Section.Error;
                                Message = 1153178; // Your bank does not have sufficient gold
                                Refresh();
                            }
                        }
                        else
                        {
                            Section = Section.Error;
                            Message = 1153187; // You entered an invalid value
                            Refresh();
                        }
                    }
                    else
                    {
                        Section = Section.Error;
                        Message = 1153187; // You entered an invalid value
                        Refresh();
                    }
                    break;
                case 4:
                    TextRelay tr2 = info.GetTextEntry(1);
                    if (tr2 != null)
                    {
                        string text2 = tr2.Text;
                        int num2 = Utility.ToInt32(text2);

                        if (num2 > 0)
                        {
                            if (num2 <= (int)PointsSystem.CasinoData.GetPoints(User))
                            {
                                Banker.Deposit(User, num2 * CasinoData.ChipCost);
                                PointsSystem.CasinoData.DeductPoints(User, num2, false);
                                User.SendLocalizedMessage(1060397, (num2 * CasinoData.ChipCost).ToString(CultureInfo.GetCultureInfo("en-US"))); // ~1_AMOUNT~ gold has been deposited into your bank box.

                                Section = Section.None;

                                CashedOut = num2;
                                Refresh();
                            }
                            else
                            {
                                Section = Section.Error;
                                Message = 1153180; // You do not have enough casino chips
                                Refresh();
                            }
                        }
                        else
                        {
                            Section = Section.None;
                            Refresh();
                        }
                    }
                    else
                    {
                        Section = Section.Error;
                        Message = 1153187; // You entered an invalid value
                        Refresh();
                    }
                    break;
                case 5:
                    Section = Section.None;
                    Refresh();
                    break;

            }
        }

        public void Refresh()
        {
            Entries.Clear();
            Entries.TrimExcess();
            AddGumpLayout();
            User.CloseGump(GetType());
            User.SendGump(this, false);
        }

        public static int C16232(int c16)
        {
            c16 &= 0x7FFF;

            int r = (((c16 >> 10) & 0x1F) << 3);
            int g = (((c16 >> 05) & 0x1F) << 3);
            int b = (((c16 >> 00) & 0x1F) << 3);

            return (r << 16) | (g << 8) | (b << 0);
        }

        public static int C16216(int c16)
        {
            return c16 & 0x7FFF;
        }

        public static int C32216(int c32)
        {
            c32 &= 0xFFFFFF;

            int r = (((c32 >> 16) & 0xFF) >> 3);
            int g = (((c32 >> 08) & 0xFF) >> 3);
            int b = (((c32 >> 00) & 0xFF) >> 3);

            return (r << 10) | (g << 5) | (b << 0);
        }

        protected string Color(string color, string str)
        {
            return string.Format("<basefont color={0}>{1}", color, str);
        }

        protected string ColorAndCenter(string color, string str)
        {
            return string.Format("<basefont color={0}><center>{1}</center>", color, str);
        }
    }

    public class BaseCasinoGump : Gump
    {
        public virtual int Title => 0;

        public int Yellow => C32216(Yellow32);
        public int Yellow32 => 0xFFFF00;

        public int Width { get; set; }
        public int Height { get; set; }

        public PlayerMobile User { get; set; }
        public BaseDiceGame DiceGame { get; set; }

        public BaseCasinoGump(PlayerMobile pm, int width, int height, BaseDiceGame game)
            : base(50, 50)
        {
            User = pm;
            Width = width;
            Height = height;
            DiceGame = game;

            AddGumpLayout();
        }

        public virtual void AddGumpLayout()
        {
            AddBackground(0, 0, Width, Height, 1460);

            if (Title > 0)
                AddHtmlLocalized(15, 20, Width - 30, 16, 1154645, "#" + Title, Yellow, false, false);

            if (PointsSystem.CasinoData.GetPoints(User) <= 0)
                AddHtmlLocalized(15, 150, Width - 30, 40, 1154645, "#1153378", Yellow, false, false); // You have no chips to bet with. Please visit the Casino Cashier to buy chips.
            else
            {
                switch (DiceGame.Stage)
                {
                    case GameStage.Betting: BuildBetting(); break;
                    case GameStage.Rolling: BuildRolling(); break;
                    case GameStage.Results: BuildResults(); break;
                    case GameStage.Error: BuildError(); break;
                }
            }
        }

        public virtual void BuildBetting()
        {
        }

        public virtual void BuildRolling()
        {
        }

        public virtual void BuildResults()
        {
        }

        public virtual void BuildError()
        {
            AddHtmlLocalized(20, 170, 240, 32, 1153380, C32216(0xFF0000), false, false); // Invalid bet amount entered

            AddButton(15, Height - 35, 4005, 4007, 250, GumpButtonType.Reply, 0);

            AddHtml(55, Height - 32, 150, 16, Color("#FFFF00", "CONTINUE"), false, false);
            DiceGame.Remove();
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 250)
            {
                DiceGame.Reset();
                Refresh();
            }
        }

        protected int RandomDyeID(int face)
        {
            int start = (face - 1) + 19380;

            return Utility.RandomList(start, start + 6, start + 12);
        }

        public void Refresh()
        {
            Entries.Clear();
            Entries.TrimExcess();
            AddGumpLayout();
            User.CloseGump(GetType());
            User.SendGump(this, false);
        }

        public static int C16232(int c16)
        {
            c16 &= 0x7FFF;

            int r = (((c16 >> 10) & 0x1F) << 3);
            int g = (((c16 >> 05) & 0x1F) << 3);
            int b = (((c16 >> 00) & 0x1F) << 3);

            return (r << 16) | (g << 8) | (b << 0);
        }

        public static int C16216(int c16)
        {
            return c16 & 0x7FFF;
        }

        public static int C32216(int c32)
        {
            c32 &= 0xFFFFFF;

            int r = (((c32 >> 16) & 0xFF) >> 3);
            int g = (((c32 >> 08) & 0xFF) >> 3);
            int b = (((c32 >> 00) & 0xFF) >> 3);

            return (r << 10) | (g << 5) | (b << 0);
        }

        protected string Color(string color, string str)
        {
            return string.Format("<basefont color={0}>{1}", color, str);
        }

        protected string ColorAndCenter(string color, string str)
        {
            return string.Format("<basefont color={0}><center>{1}</center>", color, str);
        }
    }

    public class ChucklesLuckGump : BaseCasinoGump
    {
        public override int Title => 1153368;  // CHUCKLES' LUCK

        public ChucklesLuck Game => DiceGame as ChucklesLuck;

        private readonly int _DiceHue = 1931;

        public ChucklesLuckGump(PlayerMobile pm, ChucklesLuck game)
            : base(pm, 280, 330, game)
        {
        }

        public override void AddGumpLayout()
        {
            base.AddGumpLayout();

            AddHtmlLocalized(15, 50, Width - 30, 80, 1154645, "#1153369", Yellow, false, false); // Place a bet on any number. The dealer will roll three dice. Win back your bet times the number of times your lucky number comes up!
        }

        public override void BuildBetting()
        {
            int chips = (int)PointsSystem.CasinoData.GetPoints(User);

            AddHtmlLocalized(20, 140, 240, 16, 1153370, chips.ToString(CultureInfo.GetCultureInfo("en-US")), Yellow, false, false); // You have ~1_VAL~ chips

            AddHtmlLocalized(20, 170, 240, 40, 1153372, Yellow, false, false); // Enter your bet below and click the die showing your lucky number!
            AddHtmlLocalized(20, 230, 240, 16, 1153371, Yellow, false, false); // Amount to bet:

            AddBackground(140, 230, 120, 20, 9350);
            AddTextEntry(142, 230, 120, 20, 0, 0, "");

            for (int i = 0; i < 6; i++)
            {
                AddButton(25 + (i * 40), 260, 1450 + i, 1450 + i, i + 1, GumpButtonType.Reply, 0);
            }

            AddButton(15, 295, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtml(55, 293, 150, 16, Color("#FFFF00", "CLOSE"), false, false);
        }

        public override void BuildRolling()
        {
            AddHtmlLocalized(0, 140, 138, 16, 1114514, "#1153383", Yellow, false, false); // Amount of Bet:
            AddHtml(142, 140, 150, 16, Color("#FFFF00", Game.CurrentBet.ToString(CultureInfo.GetCultureInfo("en-US"))), false, false);

            AddHtmlLocalized(0, 160, 138, 16, 1114514, "#1153382", Yellow, false, false);  // Betting On:
            AddHtml(142, 160, 150, 16, Color("#FFFF00", Game.BettingOn.ToString()), false, false);

            AddHtmlLocalized(15, 190, Width - 30, 40, 1153381, Yellow, false, false);  // The dealer prepares to roll the dice...
        }

        public override void BuildResults()
        {
            Game.Remove();

            AddHtmlLocalized(0, 140, 138, 16, 1114514, "#1153383", Yellow, false, false); // Amount of Bet:
            AddHtml(142, 140, 150, 16, Color("#FFFF00", Game.CurrentBet.ToString(CultureInfo.GetCultureInfo("en-US"))), false, false);

            AddHtmlLocalized(0, 160, 138, 16, 1114514, "#1153382", Yellow, false, false);  // Betting On:
            AddHtml(142, 160, 150, 16, Color("#FFFF00", Game.BettingOn.ToString()), false, false);

            AddHtmlLocalized(15, 190, 138, 16, 1153388, Yellow, false, false);  // The dealer rolls:

            AddItem(90, 210, RandomDyeID(Game.GetRoll(0)), _DiceHue);
            AddItem(130, 210, RandomDyeID(Game.GetRoll(1)), _DiceHue);
            AddItem(170, 210, RandomDyeID(Game.GetRoll(2)), _DiceHue);

            if (!Game.Winner)
                AddHtmlLocalized(20, 250, 240, 32, 1153385, Game.CurrentBet.ToString(CultureInfo.GetCultureInfo("en-US")), Yellow, false, false); // The dice did not match your number. You lose your bet of ~1_AMT~.
            else
            {
                int matches = Game.GetMatches();
                int win = Game.CurrentBet * matches;

                AddHtmlLocalized(20, 250, 240, 32, 1153384, string.Format("{0}\t{1}", matches.ToString(), win.ToString(CultureInfo.GetCultureInfo("en-US"))), Yellow, false, false); // The dice matched your number ~1_COUNT~ times. You win ~2_AMT~ chips!
            }

            AddHtml(55, 293, 150, 16, Color("#FFFF00", Game.Winner ? "COLLECT" : "CONTINUE"), false, false);
            AddButton(15, 295, 4005, 4007, 7, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            base.OnResponse(state, info);

            switch (info.ButtonID)
            {
                case 0: break;
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    TextRelay tr = info.GetTextEntry(0);

                    if (tr != null)
                    {
                        string text = tr.Text;
                        int bet = Utility.ToInt32(text);
                        int chips = (int)PointsSystem.CasinoData.GetPoints(User);

                        if (bet > 0 && bet <= chips)
                        {
                            PointsSystem.CasinoData.DeductPoints(User, bet, false);

                            Game.CurrentBet = bet;
                            Game.BettingOn = info.ButtonID;

                            Game.BeginRollDice();
                        }
                        else
                        {
                            Game.Stage = GameStage.Error;
                            Refresh();
                        }
                    }

                    break;
                case 7:
                    Game.Reset();
                    Refresh();
                    break;
            }
        }
    }

    public class HiMiddleLowGump : BaseCasinoGump
    {
        public override int Title => 1153392;  // HI-MIDDLE-LO

        public HiMiddleLow Game => DiceGame as HiMiddleLow;
        private readonly int _DiceHue = 1928;

        public HiMiddleLowGump(PlayerMobile pm, HiMiddleLow game)
            : base(pm, 380, 380, game)
        {
        }

        public override void AddGumpLayout()
        {
            base.AddGumpLayout();

            AddHtmlLocalized(15, 50, Width - 30, 90, 1153393, Yellow, false, true);
            /*Place a bet on Low, Middle, High, or Outside. The dealer rolls 3 dice. The house always wins on 3-of-a-kind. 
             * High Bets win on a total of 11 or more. Low Bets win on a total of 10 or less. Middle Bets win on a total of 
             * 9, 10, 11, or 12. Bets for Low, Middle, and High pay even money. Outside Bets win on totals of 4, 5, 6, 15, 16, or 17 and pay 5:1.*/
        }

        public override void BuildBetting()
        {
            int chips = (int)PointsSystem.CasinoData.GetPoints(User);

            AddHtmlLocalized(15, 145, 340, 16, 1153370, chips.ToString(CultureInfo.GetCultureInfo("en-US")), Yellow, false, false); // You have ~1_VAL~ chips
            AddHtmlLocalized(15, 175, 340, 40, 1153412, Yellow, false, false); // Enter your wager here and select which way you wish to bet:

            AddHtmlLocalized(15, 220, 300, 16, 1153371, Yellow, false, false); // Amount to bet:

            AddBackground(200, 220, 160, 20, 9350);
            AddTextEntry(202, 220, 150, 20, 0, 0, "");

            AddHtmlLocalized(55, 250, 150, 16, 1153394, Yellow, false, false); // High
            AddButton(20, 250, 4005, 4007, 1, GumpButtonType.Reply, 0);

            AddHtmlLocalized(215, 250, 150, 16, 1153395, Yellow, false, false); // Middle
            AddButton(180, 250, 4005, 4007, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(55, 272, 150, 16, 1153396, Yellow, false, false); // Low
            AddButton(20, 272, 4005, 4007, 3, GumpButtonType.Reply, 0);

            AddHtmlLocalized(215, 272, 150, 16, 1153397, Yellow, false, false); // Outside
            AddButton(180, 272, 4005, 4007, 4, GumpButtonType.Reply, 0);

            AddButton(15, 345, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtml(55, 347, 150, 16, Color("#FFFF00", "CLOSE"), false, false);
        }

        public override void BuildRolling()
        {
            AddHtmlLocalized(0, 145, 188, 16, 1114514, "#1153383", Yellow, false, false); // Amount of Bet:
            AddHtml(192, 145, 150, 16, Color("#FFFF00", Game.CurrentBet.ToString(CultureInfo.GetCultureInfo("en-US"))), false, false);

            AddHtmlLocalized(0, 165, 188, 16, 1114514, "#1153382", Yellow, false, false);  // Betting On:
            AddHtml(192, 165, 150, 16, Color("#FFFF00", ((HighMiddleLowType)Game.BettingOn).ToString()), false, false);

            AddHtmlLocalized(15, 195, Width - 30, 40, 1153381, Yellow, false, false);  // The dealer prepares to roll the dice...
        }

        public override void BuildResults()
        {
            Game.Remove();

            AddHtmlLocalized(15, 145, Width - 30, 16, 1153388, Yellow, false, false);  // The dealer rolls:

            AddItem(140, 165, RandomDyeID(Game.GetRoll(0)), _DiceHue);
            AddItem(180, 165, RandomDyeID(Game.GetRoll(1)), _DiceHue);
            AddItem(220, 165, RandomDyeID(Game.GetRoll(2)), _DiceHue);

            int total = Game.GetTotal();

            AddHtmlLocalized(0, 205, Width / 2, 16, 1154645, Game.WinsHi(total) ? "#1153400" : "#1153399", Yellow, false, false); // Hi wins/lose
            AddHtmlLocalized(190, 205, Width / 2, 16, 1154645, Game.WinsLow(total) ? "#1153404" : "#1153403", Yellow, false, false); // Low wins/lose
            AddHtmlLocalized(00, 225, Width / 2, 16, 1154645, Game.WinsMiddle(total) ? "#1153402" : "#1153401", Yellow, false, false); // middle wins/lose
            AddHtmlLocalized(190, 225, Width / 2, 16, 1154645, Game.WinsOutside(total) ? "#1153406" : "#1153405", Yellow, false, false); // outside wins/lose

            AddHtmlLocalized(0, 245, 188, 16, 1114514, "#1153383", Yellow, false, false); // Amount of Bet:
            AddHtml(192, 245, 188, 16, Color("#FFFF00", Game.CurrentBet.ToString(CultureInfo.GetCultureInfo("en-US"))), false, false);

            AddHtmlLocalized(0, 265, 188, 16, 1114514, "#1153382", Yellow, false, false);  // Betting On:
            AddHtml(192, 265, 188, 16, Color("#FFFF00", ((HighMiddleLowType)Game.BettingOn).ToString()), false, false);

            if (!Game.Winner)
            {
                int y = 290;

                if (Game.ThreeOfAKind)
                {
                    AddHtmlLocalized(20, 295, 340, 16, 1153398, Yellow, false, false);
                    y += 30;
                }

                AddHtmlLocalized(20, y, 345, 16, 1153413, Game.CurrentBet.ToString(CultureInfo.GetCultureInfo("en-US")), Yellow, false, false); // You lost ~1_AMT~ chips. Better luck next time!
            }
            else
            {
                int winnings = Game.BettingOn == 4 ? Game.CurrentBet * 6 : Game.CurrentBet * 2;
                AddHtmlLocalized(20, 295, 340, 16, 1153414, winnings.ToString(CultureInfo.GetCultureInfo("en-US")), Yellow, false, false); // You won ~1_AMT~ chips!
            }

            AddHtml(55, 347, 150, 16, Color("#FFFF00", Game.Winner ? "COLLECT" : "CONTINUE"), false, false);
            AddButton(15, 345, 4005, 4007, 5, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            base.OnResponse(state, info);

            switch (info.ButtonID)
            {
                case 0: break;
                case 1:
                case 2:
                case 3:
                case 4:
                    TextRelay tr = info.GetTextEntry(0);

                    if (tr != null)
                    {
                        string text = tr.Text;
                        int bet = Utility.ToInt32(text);
                        int chips = (int)PointsSystem.CasinoData.GetPoints(User);

                        if (bet > 0 && bet <= chips)
                        {
                            PointsSystem.CasinoData.DeductPoints(User, bet, false);

                            Game.CurrentBet = bet;
                            Game.BettingOn = info.ButtonID;

                            Game.BeginRollDice();
                        }
                        else
                        {
                            Game.Stage = GameStage.Error;
                            Refresh();
                        }
                    }
                    break;
                case 5:
                    Game.Reset();
                    Refresh();
                    break;
            }
        }
    }

    public class DiceRiderGump : BaseCasinoGump
    {
        public override int Title => 1153613;  // DICE RIDER

        public DiceRider Game => DiceGame as DiceRider;

        private int[] _DiceID = new int[5];
        private readonly int _DiceHue = 1930;

        public DiceRiderGump(PlayerMobile pm, DiceRider game)
            : base(pm, 530, 430, game)
        {
        }

        public override void AddGumpLayout()
        {
            base.AddGumpLayout();

            if (PointsSystem.CasinoData.GetPoints(User) <= 0)
                return;

            AddHtmlLocalized(15, 50, Width - 30, 90, 1154645, "#1153614", Yellow, false, true);
            /*HOW TO PLAY<br><br>Place three equal-size bets. Enter the amount you want a single bet to be. Your initial bet will be 3x 
             * that amount. The dealer will then roll three dice. You may then choose to pull back one of your bets, or to "let it ride".
             * The dealer rolls a fourth die. You may then choose to pull back a second bet. Finally, the dealer rolls a fifth die.
             * <br><br>Your payout depends on the results of your roll:<br>Five of a kind pays 80-to-1<br>Four of a kind pays 3-to-1<br>
             * A straight pays 2-to-1<br>A full house pays 3-to-2<br>Three of a Kind pays 1-to-1<br><br>NOTE: Fractional win amounts for
             * 3:2 payouts will be rounded down!*/

            AddHtmlLocalized(15, 140, Width - 30, 16, 1154645, "#1153628", Yellow, false, false); // PAY TABLE

            AddHtmlLocalized(0, 160, 160, 16, 1114514, "#1153615", Yellow, false, false); // FIVE OF A KIND
            AddHtmlLocalized(405, 160, 100, 16, 1153627, "80\t1", Yellow, false, false);

            for (int i = 0; i < 6; i++)
                AddImage(170 + (i * 40), 160, 1455);

            AddHtmlLocalized(0, 187, 160, 16, 1114514, "#1153617", Yellow, false, false); // FIVE OF A KIND
            AddHtmlLocalized(405, 187, 100, 16, 1153627, "3\t1", Yellow, false, false);

            for (int i = 0; i < 6; i++)
                AddImage(170 + (i * 40), 187, i == 5 ? 1453 : 1454);

            AddHtmlLocalized(0, 214, 160, 16, 1114514, "#1153619", Yellow, false, false); // STRAIGHT
            AddHtmlLocalized(405, 214, 100, 16, 1153627, "3\t1", Yellow, false, false);

            for (int i = 0; i < 6; i++)
                AddImage(170 + (i * 40), 214, 1455 - i);

            AddHtmlLocalized(0, 241, 160, 16, 1114514, "#1153619", Yellow, false, false); // STRAIGHT
            AddHtmlLocalized(405, 241, 100, 16, 1153627, "2\t1", Yellow, false, false);

            for (int i = 0; i < 6; i++)
                AddImage(170 + (i * 40), 241, 1455 - i);

            AddHtmlLocalized(0, 268, 160, 16, 1114514, "#1153621", Yellow, false, false); // FULL HOUSE
            AddHtmlLocalized(405, 268, 100, 16, 1153627, "3\t2", Yellow, false, false);

            for (int i = 0; i < 6; i++)
                AddImage(170 + (i * 40), 268, i < 3 ? 1453 : 1452);

            AddHtmlLocalized(0, 295, 160, 16, 1114514, "#1153623", Yellow, false, false); // THREE OF A KIND
            AddHtmlLocalized(405, 295, 100, 16, 1153627, "1\t1", Yellow, false, false);

            for (int i = 0; i < 6; i++)
                AddImage(170 + (i * 40), 295, i < 3 ? 1452 : i == 3 ? 1453 : 1455);
        }

        public override void BuildBetting()
        {
            int chips = (int)PointsSystem.CasinoData.GetPoints(User);

            AddHtmlLocalized(15, 325, 160, 16, 1153370, chips.ToString(CultureInfo.GetCultureInfo("en-US")), Yellow, false, false); // You have ~1_VAL~ chips
            AddHtmlLocalized(15, 350, Width - 30, 32, 1153629, Yellow, false, false); // Enter your bet amount below and click the button to play. You will place 3 bets of that amount but you can pull two of them back!

            AddHtmlLocalized(170, Height - 35, 150, 16, 1153371, Yellow, false, false); // Amount to bet:
            AddBackground(270, Height - 35, 200, 20, 9350);
            AddTextEntry(272, Height - 35, 198, 20, 0, 0, "");

            AddButton(485, Height - 35, 4005, 4007, 1, GumpButtonType.Reply, 0);

            AddButton(15, Height - 35, 4005, 4007, 100, GumpButtonType.Reply, 0);
            AddHtml(55, Height - 35, 150, 16, Color("#FFFF00", "CLOSE"), false, false);
        }

        public override void BuildRolling()
        {
            AddHtmlLocalized(120, 325, 150, 16, 1153383, Yellow, false, false); // Amount of Bet:

            AddHtml(275, 325, 100, 16, Color("#FFFF00", Game.Bet1.ToString(CultureInfo.GetCultureInfo("en-US"))), false, false);
            AddHtml(325, 325, 100, 16, Color("#FFFF00", Game.Bet2.ToString(CultureInfo.GetCultureInfo("en-US"))), false, false);
            AddHtml(375, 325, 100, 16, Color("#FFFF00", Game.Bet3.ToString(CultureInfo.GetCultureInfo("en-US"))), false, false);

            AddHtmlLocalized(100, 345, 100, 16, 1153635, Yellow, false, false); // Your Dice:

            if (Game.Roll != null)
            {
                for (int i = 0; i < Game.Roll.Count; i++)
                {
                    if (_DiceID[i] == 0)
                        _DiceID[i] = RandomDyeID(Game.GetRoll(i));

                    AddItem(180 + (i * 45), 345, _DiceID[i], _DiceHue);
                }
            }

            AddHtmlLocalized(15, 385, Width - 30, 40, 1153381, Yellow, false, false); //  The dealer prepares to roll the dice...
        }

        public override void BuildResults()
        {
            AddHtmlLocalized(120, 325, 150, 16, 1153383, Yellow, false, false); // Amount of Bet:

            AddHtml(275, 325, 100, 16, Color("#FFFF00", Game.Bet1.ToString(CultureInfo.GetCultureInfo("en-US"))), false, false);
            AddHtml(325, 325, 100, 16, Color("#FFFF00", Game.Bet2.ToString(CultureInfo.GetCultureInfo("en-US"))), false, false);
            AddHtml(375, 325, 100, 16, Color("#FFFF00", Game.Bet3.ToString(CultureInfo.GetCultureInfo("en-US"))), false, false);

            AddHtmlLocalized(100, 345, 100, 16, 1153635, Yellow, false, false); // Your Dice:

            if (Game.Roll != null)
            {
                for (int i = 0; i < Game.Roll.Count; i++)
                {
                    if (_DiceID[i] == 0)
                        _DiceID[i] = RandomDyeID(Game.GetRoll(i));

                    AddItem(180 + (i * 45), 345, _DiceID[i], _DiceHue);
                }
            }

            if (Game.RollNumber <= 3)
            {
                AddHtmlLocalized(165, 385, 150, 16, 1153636, Yellow, false, false); // PULL BET
                AddButton(125, 385, 4005, 4007, 2, GumpButtonType.Reply, 0);

                AddHtmlLocalized(315, 385, 150, 16, 1153637, Yellow, false, false); // LET IT RIDE
                AddButton(275, 385, 4005, 4007, 3, GumpButtonType.Reply, 0);
            }
            else
            {
                Game.Remove();

                _DiceID = new int[5];

                int totalbet = Game.Bet1 + Game.Bet2 + Game.Bet3;

                if (!Game.Winner)
                {
                    AddHtmlLocalized(15, 375, Width - 30, 16, 1153639, totalbet.ToString(CultureInfo.GetCultureInfo("en-US")), Yellow, false, false); // You did not make a winning hand. You lost ~1_AMT~.
                }
                else
                {
                    AddHtmlLocalized(15, 375, Width - 30, 16, 1153640, string.Format("{0}\t#{1}", Game.WinningTotal, WinningHand()), Yellow, false, false); // You won ~1_AMT~ for making a ~2_HAND_NAME~!
                }

                AddHtml(55, Height - 35, 150, 16, Color("#FFFF00", Game.Winner ? "COLLECT" : "CONTINUE"), false, false);
                AddButton(15, Height - 35, 4005, 4007, 4, GumpButtonType.Reply, 0);
            }
        }

        public override void BuildError()
        {
            AddHtmlLocalized(20, Height - 62, 240, 32, 1153380, C32216(0xFF0000), false, false); // Invalid bet amount entered

            AddButton(15, Height - 35, 4005, 4007, 250, GumpButtonType.Reply, 0);

            AddHtml(55, Height - 32, 150, 16, Color("#FFFF00", "CONTINUE"), false, false);
            DiceGame.Remove();
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: break;
                case 1:
                    TextRelay tr = info.GetTextEntry(0);

                    if (tr != null)
                    {
                        string text = tr.Text;
                        int bet = Utility.ToInt32(text) * 3;

                        if (bet > 0 && bet <= (int)PointsSystem.CasinoData.GetPoints(User))
                        {
                            PointsSystem.CasinoData.DeductPoints(User, bet, false);

                            Game.CurrentBet = bet;
                            Game.Bet1 = bet / 3;
                            Game.Bet2 = bet / 3;
                            Game.Bet3 = bet / 3;

                            Game.BeginRollDice();
                        }
                        else
                        {
                            Game.Stage = GameStage.Error;
                            Refresh();
                        }
                    }
                    break;
                case 2:
                    if (Game.RollNumber == 2)
                    {
                        PointsSystem.CasinoData.AwardPoints(User, Game.Bet2);
                        Game.Bet2 = 0;
                    }
                    else if (Game.RollNumber == 3)
                    {
                        PointsSystem.CasinoData.AwardPoints(User, Game.Bet3);
                        Game.Bet3 = 0;
                    }

                    Game.Stage = GameStage.Rolling;
                    Game.BeginRollDice();
                    break;
                case 3:
                    Game.Stage = GameStage.Rolling;
                    Game.BeginRollDice();
                    break;
                case 4:
                    Game.Reset();
                    Refresh();
                    break;
                case 250:
                    DiceGame.Reset();
                    Refresh();
                    break;
            }
        }

        private int WinningHand()
        {
            if (Game.IsFiveOfAKind())
                return 1153616;

            if (Game.IsFourOfAKind())
                return 1153618;

            if (Game.IsStraight())
                return 1153620;

            if (Game.IsFullHouse())
                return 1153622;

            if (Game.IsThreeOfAKind())
                return 1153624;

            return 1153626;
        }
    }
}