using Server;
using System;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Engines.LotterySystem;
using Server.Gumps;
using Server.Items;

namespace Server.Items
{
    public class GoldenTicket : BaseLottoTicket
    {
        private int[] m_WinAmounts = new int[] { 1000, 5000, 25000, 100000, 250000, 1000000, 2 };

        public static readonly int TicketCost = 1000;

        [Constructable]
        public GoldenTicket() :this(null, false)
        {
        }

        [Constructable]
        public GoldenTicket(Mobile owner, bool quickScratch) : base (owner, TicketType.GoldenTicket, quickScratch)
        {
            Name = "a golden ticket";
            LootType = LootType.Blessed;
            Hue = 0x8A5;
        }

        public override void CheckScratches()
        {
            if (Scratch1 > 0 && Scratch2 > 0 && Scratch3 > 0 && !Checked)
            {
                Checked = true;
                if (Scratch1 == 2 || Scratch2 == 2 || Scratch3 == 2)
                    FreeTicket = true;
                else if (Scratch1 == Scratch2 && Scratch1 == Scratch3)
                {
                    if (Scratch1 == 2)
                        Payout = 250000;
                    else
                        Payout = Scratch1;

                    DoWin();

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
                return new int[] {  0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 4, 5, 6 };

            return new int[] {  0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 4, 5, 6 };
        }

        private void DoWin()
        {
            if (Owner != null)
                Owner.PlaySound(Owner.Female ? 0x337 : 0x449);

            if (Payout >= 100000)
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

        public GoldenTicket(Serial serial) : base( serial )
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}