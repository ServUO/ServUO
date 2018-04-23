using Server;
using System;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Engines.LotterySystem;
using Server.Gumps;
using Server.Items;

namespace Server.Items
{
    public class CrazedCrafting : BaseLottoTicket
    {
        private int[] m_WinAmounts = new int[] { 1000, 2500, 7500, 25000, 150000, 2 };
        private int[] m_WildCards = new int[] { 0x15AF, 0x15B3, 0x15B7, 0x15CB, 0x15CD };

        public int[] WildCards { get { return m_WildCards; } }

        public static readonly int TicketCost = 1000;

        [Constructable]
        public CrazedCrafting() : this(null, false)
        {
        }

        [Constructable]
        public CrazedCrafting(Mobile owner, bool quickScratch) : base (owner, TicketType.CrazedCrafting, quickScratch)
        {
            Name = "a crazed crafting ticket";
            LootType = LootType.Blessed;
            Hue = 0x972;
        }

        public override bool DoScratch(int scratch, Mobile from)
        {
            if (scratch > 3 || scratch < 0 || from == null)
                return false;

            int pick;
            int pickAmount;

            try
            {
                if (.08 > Utility.RandomDouble())
                    pickAmount = m_WildCards[Utility.Random(m_WildCards.Length)];
                else
                {
                    int[] odds = ReturnOdds(from);
                    pick = odds[Utility.Random(odds.Length)];
                    pickAmount = m_WinAmounts[pick];
                }
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
            if (CraftingSkill(from) > 120 || (CraftingSkill(from) > 100 && Utility.RandomBool()))
                return new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 5 };

            return new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 5 };
        }

        public override void CheckScratches()
        {
            /*Multipler =  3 same wildcards = 10x
                           2 same wildcards = 2x*/
 
            if (Scratch1 > 0 && Scratch2 > 0 && Scratch3 > 0)
            {
                Checked = true;

                bool wild1 = false;
                bool wild2 = false;
                bool wild3 = false;

                foreach(int num in m_WildCards)
                {
                    if (Scratch1 == num)
                        wild1 = true;
                    if (Scratch2 == num)
                        wild2 = true;
                    if (Scratch3 == num)
                        wild3 = true;
                }

                if (Scratch1 == 2 || Scratch2 == 2 || Scratch3 == 2)
                    FreeTicket = true;
                else if ((Scratch1 == Scratch2 && Scratch2 == Scratch3) || (wild1 && Scratch2 == Scratch3) || (wild2 && Scratch1 == Scratch3)
                    || (wild3 && Scratch1 == Scratch2) || (wild1 && wild2) || (wild2 && wild3) || (wild1 && wild3))
                {
                    int payOut = 0;

                    if (!wild1)
                    {
                        payOut = Scratch1;
                        if (wild2 && wild3 && Scratch2 == Scratch3)
                            payOut *= 2;
                    }
                    else if (!wild2)
                    {
                        payOut = Scratch2;
                        if (wild1 && wild3 && Scratch1 == Scratch3)
                            payOut *= 2;
                    }
                    else if (!wild3)
                    {
                        payOut = Scratch3;
                        if (wild1 && wild2 && Scratch1 == Scratch2)
                            payOut *= 2;
                    }
                    else
                    {
                        payOut = 150000;

                        if (Scratch1 == Scratch2 && Scratch2 == Scratch3)
                            payOut *= 10;
                        else if (Scratch1 == Scratch2 || Scratch2 == Scratch3 || Scratch1 == Scratch3)
                            payOut *= 2;

                    }

                    Payout = payOut;
                    DoWin(Payout);

                    if (ScratcherLotto.Stone != null)
                        ScratcherLotto.Stone.GoldSink -= Payout;
                }
            }

            InvalidateProperties();
        }

        private void DoWin(int amount)
        {
            if (Owner != null)
                Owner.PlaySound(Owner.Female ? 0x337 : 0x449);

            if (amount >= 100000) //Jackpot
            {
                new ScratcherStats(Owner, amount, this.Type);

                if (ScratcherLotto.Stone != null)
                {
                    ScratcherLotto.Stone.InvalidateProperties();
                    ScratcherLotto.Stone.UpdateSatellites();
                }
            }

            if (Owner != null)
                Owner.SendMessage(42, "It looks like you have a winning ticket!");
        }

        private double CraftingSkill(Mobile from)
        {
            double topSkill = from.Skills[SkillName.Alchemy].Value;

            if (from.Skills[SkillName.Fletching].Value > topSkill)
                topSkill = from.Skills[SkillName.Fletching].Value;
            if (from.Skills[SkillName.Blacksmith].Value > topSkill)
                topSkill = from.Skills[SkillName.Blacksmith].Value;
            if (from.Skills[SkillName.Tailoring].Value > topSkill)
                topSkill = from.Skills[SkillName.Tailoring].Value;
            if (from.Skills[SkillName.Inscribe].Value > topSkill)
                topSkill = from.Skills[SkillName.Inscribe].Value;
            if (from.Skills[SkillName.Carpentry].Value > topSkill)
                topSkill = from.Skills[SkillName.Carpentry].Value;
            if (from.Skills[SkillName.Tinkering].Value > topSkill)
                topSkill = from.Skills[SkillName.Tinkering].Value; 

            return topSkill;
        }

        public CrazedCrafting(Serial serial)
            : base(serial)
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