using Server.Engines.Points;
using Server.Engines.Quests;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Server.Engines.ResortAndCasino
{
    public enum GameStage
    {
        Betting,
        Rolling,
        Results,
        Error
    }

    public class BaseDiceGame
    {
        public PlayerMobile Player { get; set; }
        public CasinoDealer Dealer { get; set; }

        public int CurrentBet { get; set; }
        public int BettingOn { get; set; }

        public List<int> Roll { get; set; }
        public GameStage Stage { get; set; }

        public bool Winner { get; set; }

        public BaseDiceGame(PlayerMobile player, CasinoDealer dealer)
        {
            Player = player;
            Dealer = dealer;
        }

        public virtual void BeginRollDice()
        {
            Stage = GameStage.Rolling;
            SendGump();

            Timer.DelayCall(TimeSpan.FromSeconds(3), RollDice);
        }

        public virtual void RollDice(int numofdice)
        {
            List<int> dice = new List<int>();

            for (int i = 0; i < numofdice; i++)
            {
                dice.Add(Utility.RandomMinMax(1, 6));
            }

            Roll = dice;
        }

        public virtual int GetRoll(int index)
        {
            if (Roll == null || index > Roll.Count)
                return 0;

            return Roll[index];
        }

        public virtual int GetMatches()
        {
            if (Roll == null)
                return 0;

            return Roll.Where(i => i == BettingOn).Count();
        }

        public virtual int GetTotal()
        {
            if (Roll == null)
                return 0;

            int total = 0;
            Roll.ForEach(i => total += i);

            return total;
        }

        public void RollDice()
        {
            OnDiceRolled();
            Timer.DelayCall(TimeSpan.FromSeconds(1), SendGump);
        }

        public virtual void OnDiceRolled()
        {
            Stage = GameStage.Results;
        }

        public virtual void SendGump()
        {
        }

        public virtual void Remove()
        {
            if (Dealer != null)
                Dealer.RemoveGame(Player, this);
        }

        public virtual void OnWin()
        {
            if (Player != null)
            {
                GettingEvenQuest q = QuestHelper.GetQuest(Player, typeof(GettingEvenQuest)) as GettingEvenQuest;

                if (q != null)
                    q.Update(GetType());
            }
        }

        public virtual void Reset()
        {
            Winner = false;
            Roll = null;
            CurrentBet = 0;
            BettingOn = -1;
            Stage = GameStage.Betting;

            Dealer.AddGame(Player, this);
            SendGump();
        }
    }

    public class ChucklesLuck : BaseDiceGame
    {
        public ChucklesLuck(PlayerMobile player, CasinoDealer dealer) : base(player, dealer)
        {
        }

        public override void BeginRollDice()
        {
            base.BeginRollDice();

            Player.PrivateOverheadMessage(MessageType.Regular, 0x35, 1153375, string.Format("{0}\t{1}", CurrentBet.ToString(), BettingOn.ToString()), Player.NetState); // *bets ~1_AMT~ chips on ~2_PROP~*
        }

        public override void OnDiceRolled()
        {
            base.OnDiceRolled();

            RollDice(3);

            int matches = GetMatches();

            Dealer.PrivateOverheadMessage(MessageType.Regular, 0x35, 1153391, string.Format("{0}\t{1}\t{2}", Roll[0], Roll[1], Roll[2]), Player.NetState); // *rolls the dice; they land on ~1_FIRST~ ~2_SECOND~ ~3_THIRD~*

            if (matches == 0)
            {
                Dealer.PrivateOverheadMessage(MessageType.Regular, 0x35, 1153376, string.Format("{0}\t{1}", Player.Name, CurrentBet.ToString(CultureInfo.GetCultureInfo("en-US"))), Player.NetState); // *rakes in ~1_NAME~'s ~2_VAL~-chip bet*
            }
            else
            {
                int winnings = CurrentBet * matches;
                PointsSystem.CasinoData.AwardPoints(Player, winnings);

                Winner = true;
                OnWin();
                Dealer.PrivateOverheadMessage(MessageType.Regular, 0x35, 1153377, string.Format("{0}\t{1}", Player.Name, winnings.ToString(CultureInfo.GetCultureInfo("en-US"))), Player.NetState); // *pays out ~2_VAL~ chips to ~1_NAME~*
            }
        }

        public override void SendGump()
        {
            ChucklesLuckGump g = Player.FindGump(typeof(ChucklesLuckGump)) as ChucklesLuckGump;

            if (g != null)
                g.Refresh();
            else
            {
                Player.SendGump(new ChucklesLuckGump(Player, this));
            }
        }
    }

    public enum HighMiddleLowType
    {
        High = 1,
        Middle,
        Low,
        Outside
    }


    public class HiMiddleLow : BaseDiceGame
    {
        public bool ThreeOfAKind { get; set; }
        public HighMiddleLowType BetType => (HighMiddleLowType)BettingOn;

        public HiMiddleLow(PlayerMobile player, CasinoDealer dealer)
            : base(player, dealer)
        {
        }

        public override void BeginRollDice()
        {
            base.BeginRollDice();

            Player.PrivateOverheadMessage(MessageType.Regular, 0x35, 1153375, string.Format("{0}\t{1}", CurrentBet.ToString(), ((HighMiddleLowType)BettingOn).ToString()), Player.NetState); // *bets ~1_AMT~ chips on ~2_PROP~*
        }

        public override void OnDiceRolled()
        {
            base.OnDiceRolled();

            RollDice(3);

            if (Roll[0] == Roll[1] && Roll[0] == Roll[2])
            {
                ThreeOfAKind = true;
            }
            else
            {
                int total = Roll[0] + Roll[1] + Roll[2];
                int winnings = 0;

                Dealer.PrivateOverheadMessage(MessageType.Regular, 0x35, 1153391, string.Format("{0}\t{1}\t{2}", Roll[0], Roll[1], Roll[2]), Player.NetState); // *rolls the dice; they land on ~1_FIRST~ ~2_SECOND~ ~3_THIRD~*

                switch (BetType)
                {
                    default:
                    case HighMiddleLowType.High:
                        if (WinsHi(total)) winnings = CurrentBet * 2; break;
                    case HighMiddleLowType.Middle:
                        if (WinsMiddle(total)) winnings = CurrentBet * 2; break;
                    case HighMiddleLowType.Low:
                        if (WinsLow(total)) winnings = CurrentBet * 2; break;
                    case HighMiddleLowType.Outside:
                        if (WinsOutside(total)) winnings = CurrentBet * 5; break;
                }

                if (winnings > 0)
                {
                    PointsSystem.CasinoData.AwardPoints(Player, winnings);
                    Winner = true;
                    OnWin();

                    Dealer.PrivateOverheadMessage(MessageType.Regular, 0x35, 1153377, string.Format("{0}\t{1}", Player.Name, winnings.ToString(CultureInfo.GetCultureInfo("en-US"))), Player.NetState); // *pays out ~2_VAL~ chips to ~1_NAME~*
                }
                else
                {
                    Dealer.PrivateOverheadMessage(MessageType.Regular, 0x35, 1153376, string.Format("{0}\t{1}", Player.Name, CurrentBet.ToString(CultureInfo.GetCultureInfo("en-US"))), Player.NetState); // *rakes in ~1_NAME~'s ~2_VAL~-chip bet*
                }
            }
        }

        public override void Reset()
        {
            ThreeOfAKind = false;

            base.Reset();
        }

        public override void SendGump()
        {
            HiMiddleLowGump g = Player.FindGump(typeof(HiMiddleLowGump)) as HiMiddleLowGump;

            if (g != null)
                g.Refresh();
            else
            {
                Player.SendGump(new HiMiddleLowGump(Player, this));
            }
        }

        public bool WinsHi(int total)
        {
            return total >= 11;
        }

        public bool WinsMiddle(int total)
        {
            return total >= 9 && total <= 12;
        }

        public bool WinsLow(int total)
        {
            return total <= 10;
        }

        public bool WinsOutside(int total)
        {
            return (total >= 4 && total <= 6) || (total >= 15 && total <= 17);
        }
    }

    public class DiceRider : BaseDiceGame
    {
        public int RollNumber { get; set; }

        public int Bet1 { get; set; }
        public int Bet2 { get; set; }
        public int Bet3 { get; set; }
        public int WinningTotal { get; set; }

        public int TotalBet => Bet1 + Bet2 + Bet3;

        private Timer _RollTimer;

        public DiceRider(PlayerMobile player, CasinoDealer dealer)
            : base(player, dealer)
        {
            RollNumber = 1;
        }

        public override void BeginRollDice()
        {
            base.BeginRollDice();

            if (_RollTimer != null)
            {
                _RollTimer.Stop();
                _RollTimer = null;
            }

            if (RollNumber == 1)
                Player.PrivateOverheadMessage(MessageType.Regular, 0x35, 1153631, (CurrentBet / 3).ToString(CultureInfo.GetCultureInfo("en-US")), Player.NetState); // *bets ~1_AMT~ chips on ~2_PROP~*
        }

        public override void OnDiceRolled()
        {
            if (RollNumber == 1)
            {
                RollDice(3);
                Dealer.PrivateOverheadMessage(MessageType.Regular, 0x35, 1153391, string.Format("{0}\t{1}\t{2}", GetRoll(0), GetRoll(1), GetRoll(2)), Player.NetState); // *rolls the dice; they land on ~1_FIRST~ ~2_SECOND~ ~3_THIRD~*
            }
            else
            {
                Roll.Add(Utility.RandomMinMax(1, 6));

                Dealer.PrivateOverheadMessage(MessageType.Regular, 0x35, RollNumber == 2 ? 1153633 : 1153634, RollNumber == 2 ? GetRoll(3).ToString() : GetRoll(4).ToString(), Player.NetState); // *rolls the fourth die: ~1_DIE4~*
            }

            if (RollNumber == 3)
            {
                int winnings = 0;

                if (IsFiveOfAKind())
                    winnings = TotalBet * 80;
                else if (IsFourOfAKind())
                    winnings = TotalBet * 3;
                else if (IsStraight())
                    winnings = TotalBet * 2;
                else if (IsFullHouse())
                    winnings = (int)(TotalBet * 1.5);
                else if (IsThreeOfAKind())
                    winnings = TotalBet;

                if (winnings > 0)
                {
                    Winner = true;
                    OnWin();

                    WinningTotal = winnings;
                    PointsSystem.CasinoData.AwardPoints(Player, winnings);

                    Dealer.PrivateOverheadMessage(MessageType.Regular, 0x35, 1153377, string.Format("{0}\t{1}", Player.Name, winnings.ToString(CultureInfo.GetCultureInfo("en-US"))), Player.NetState); // *pays out ~2_VAL~ chips to ~1_NAME~*
                }
                else
                {
                    Dealer.PrivateOverheadMessage(MessageType.Regular, 0x35, 1153376, string.Format("{0}\t{1}", Player.Name, TotalBet.ToString(CultureInfo.GetCultureInfo("en-US"))), Player.NetState); // *rakes in ~1_NAME~'s ~2_VAL~-chip bet*
                }
            }
            else
            {
                _RollTimer = Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                    {
                        Stage = GameStage.Rolling;
                        BeginRollDice();
                    });
            }

            Stage = GameStage.Results;
            RollNumber++;
        }

        public override void Reset()
        {
            Bet1 = 0;
            Bet2 = 0;
            Bet3 = 0;

            WinningTotal = 0;
            RollNumber = 1;

            if (_RollTimer != null)
            {
                _RollTimer.Stop();
                _RollTimer = null;
            }

            base.Reset();
        }

        public override void SendGump()
        {
            DiceRiderGump g = Player.FindGump(typeof(DiceRiderGump)) as DiceRiderGump;

            if (g != null)
                g.Refresh();
            else
            {
                Player.SendGump(new DiceRiderGump(Player, this));
            }
        }

        public bool IsFiveOfAKind()
        {
            return Roll != null && Roll.Count == 5 && Roll[0] == Roll[1] && Roll[0] == Roll[2] && Roll[0] == Roll[3] && Roll[0] == Roll[4];
        }

        public bool IsFourOfAKind()
        {
            if (Roll == null || Roll.Count < 4)
                return false;

            int[] roll = Roll.ToArray();
            Array.Sort(roll);

            return (roll[0] == roll[1] && roll[0] == roll[2] && roll[0] == roll[3])
                || (roll[1] == roll[2] && roll[1] == roll[3] && roll[1] == roll[4]);
        }

        public bool IsStraight()
        {
            if (Roll == null || Roll.Count < 5)
                return false;

            int[] roll = Roll.ToArray();
            Array.Sort(roll);

            return (roll[0] == 1 && roll[1] == 2 && roll[2] == 3 && roll[3] == 4 && roll[4] == 5) ||
                   (roll[0] == 2 && roll[1] == 3 && roll[2] == 4 && roll[3] == 5 && roll[4] == 6);
        }

        public bool IsFullHouse()
        {
            if (Roll == null || Roll.Count < 5)
                return false;

            int[] roll = Roll.ToArray();
            Array.Sort(roll);

            return (roll[0] == roll[1] && roll[0] == roll[2] && roll[3] == roll[4]) ||
                   (roll[0] == roll[1] && roll[2] == roll[3] && roll[2] == roll[4]);
        }

        public bool IsThreeOfAKind()
        {
            if (Roll == null || Roll.Count < 3)
                return false;

            int[] roll = Roll.ToArray();
            Array.Sort(roll);

            return (roll[0] == roll[1] && roll[0] == roll[2])
                  || (roll[1] == roll[2] && roll[1] == roll[3])
                  || (roll[2] == roll[3] && roll[2] == roll[4]);
        }
    }
}
