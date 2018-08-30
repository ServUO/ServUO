using Server;
using System;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Engines.LotterySystem;
using Server.Gumps;
using Server.Items;

namespace Server.Items
{
    public class SkiesTheLimit : BaseLottoTicket
    {
        private int[] m_WinAmounts = new int[] { 1000, 10000, 50000, 100000, 500000, 1, 2};

        public static readonly int TicketCost = 1000;

        [Constructable]
        public SkiesTheLimit() : this(null, false)
        {
        }

        [Constructable]
        public SkiesTheLimit(Mobile owner, bool quickScratch) : base (owner, TicketType.SkiesTheLimit, quickScratch)
        {
            Name = "skies the limit";
            LootType = LootType.Blessed;
            Hue = 0x8AB;

            this.Type = TicketType.SkiesTheLimit;
        }

        public override void CheckScratches()
        {
            if (Scratch1 > 0 && Scratch2 > 0 && Scratch3 > 0)
            {
                Checked = true;
                if (Scratch1 == 2 || Scratch2 == 2 || Scratch3 == 2)
                    FreeTicket = true;
                else if (Scratch1 == Scratch2 && Scratch1 == Scratch3)
                {

                    int payOut = 0;
                    if (Scratch1 == 2)
                        payOut = 250000;
                    else
                        payOut = Scratch1;

                    if (ScratcherLotto.Stone != null && Scratch1 == 1)
                    {
                        payOut = ScratcherLotto.Stone.SkiesProgressive;
                        ScratcherLotto.Stone.SkiesProgressive = 500000;
                        ScratcherLotto.DoProgressiveMessage(Owner, payOut);
                    }

                    Payout = payOut;
                    DoWin(payOut);

                    if (ScratcherLotto.Stone != null)
                        ScratcherLotto.Stone.GoldSink -= Payout;
                }
            }

            InvalidateProperties();
        }

        public override bool DoScratch(int scratch, Mobile from)
        {
            if (scratch > 3 || scratch < 0 || from == null)
                return false;

            int pick;
            int pickAmount;

            try
            {
                int[] odds = ReturnOdds(from);
                pick = odds[Utility.Random(odds.Length)];
                pickAmount = m_WinAmounts[pick];
            }
            catch
            {
                return false;
            }

            switch (scratch)
            {
                case 1: Scratch1 = pickAmount; break;
                case 2: Scratch2 = pickAmount; break;
                case 3: Scratch3 = pickAmount; break;
                default: return false;
            }

            CheckScratches();
            return true;
        }

        private int[] ReturnOdds(Mobile from)
        {
            if (from != null && from.Luck >= 1800 || (from.Luck > 1200 && Utility.RandomBool()))
                return new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 4, 5, 6 };

            return new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 4, 5, 6 };
        }

        private void DoWin(int amount)
        {
            if (Owner != null)
                Owner.PlaySound(Owner.Female ? 0x337 : 0x449);

            if (amount >= 100000)
            {
                new ScratcherStats(Owner, Payout, this.Type);

                if (ScratcherLotto.Stone != null)
                {
                    ScratcherLotto.Stone.InvalidateProperties();
                    ScratcherLotto.Stone.UpdateSatellites();
                }
            }

            if (Owner != null)
                Owner.SendMessage(42, "It looks like you have a winning ticket!");
        }

        public SkiesTheLimit(Serial serial) : base( serial )
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
 
            writer.Write((int) 0); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}