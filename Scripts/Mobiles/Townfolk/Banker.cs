#region Header
// **********
// ServUO - Banker.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Accounting;
using Server.ContextMenus;
using Server.Items;
using Server.Network;

using Acc = Server.Accounting.Account;
#endregion

namespace Server.Mobiles
{
    public class Banker : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();

        [Constructable]
        public Banker()
            : base("the banker")
        { }

        public Banker(Serial serial)
            : base(serial)
        { }

        public override NpcGuild NpcGuild { get { return NpcGuild.MerchantsGuild; } }

        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        public static int GetBalance(Mobile m)
        {
            double balance = 0;

			if (AccountGold.Enabled && m.Account != null)
            {
                int goldStub;
                m.Account.GetGoldBalance(out goldStub, out balance);

                if (balance > Int32.MaxValue)
                {
                    return Int32.MaxValue;
                }
            }

            Container bank = m.FindBankNoCreate();

            if (bank != null)
            {
                var gold = bank.FindItemsByType<Gold>();
                var checks = bank.FindItemsByType<BankCheck>();

                balance += gold.Aggregate(0.0, (c, t) => c + t.Amount);
                balance += checks.Aggregate(0.0, (c, t) => c + t.Worth);
            }

            return (int)Math.Max(0, Math.Min(Int32.MaxValue, balance));
        }

        public static int GetBalance(Mobile m, out Item[] gold, out Item[] checks)
        {
            double balance = 0;

			if (AccountGold.Enabled && m.Account != null)
            {
                int goldStub;
                m.Account.GetGoldBalance(out goldStub, out balance);

                if (balance > Int32.MaxValue)
                {
                    gold = checks = new Item[0];
                    return Int32.MaxValue;
                }
            }

            Container bank = m.FindBankNoCreate();

            if (bank != null)
            {
                gold = bank.FindItemsByType(typeof(Gold));
                checks = bank.FindItemsByType(typeof(BankCheck));

                balance += gold.OfType<Gold>().Aggregate(0.0, (c, t) => c + t.Amount);
                balance += checks.OfType<BankCheck>().Aggregate(0.0, (c, t) => c + t.Worth);
            }
            else
            {
                gold = checks = new Item[0];
            }

            return (int)Math.Max(0, Math.Min(Int32.MaxValue, balance));
        }

        public static bool Withdraw(Mobile from, int amount)
        {
            // If for whatever reason the TOL checks fail, we should still try old methods for withdrawing currency.
			if (AccountGold.Enabled && from.Account != null && from.Account.WithdrawGold(amount))
            {
                return true;
            }

            Item[] gold, checks;
            var balance = GetBalance(from, out gold, out checks);

            if (balance < amount)
            {
                return false;
            }

            for (var i = 0; amount > 0 && i < gold.Length; ++i)
            {
                if (gold[i].Amount <= amount)
                {
                    amount -= gold[i].Amount;
                    gold[i].Delete();
                }
                else
                {
                    gold[i].Amount -= amount;
                    amount = 0;
                }
            }

            for (var i = 0; amount > 0 && i < checks.Length; ++i)
            {
                var check = (BankCheck)checks[i];

                if (check.Worth <= amount)
                {
                    amount -= check.Worth;
                    check.Delete();
                }
                else
                {
                    check.Worth -= amount;
                    amount = 0;
                }
            }

            return true;
        }

        public static bool Deposit(Mobile from, int amount)
        {
            // If for whatever reason the TOL checks fail, we should still try old methods for depositing currency.
			if (AccountGold.Enabled && from.Account != null && from.Account.DepositGold(amount))
            {
                return true;
            }

            var box = from.FindBankNoCreate();

            if (box == null)
            {
                return false;
            }

            var items = new List<Item>();

            while (amount > 0)
            {
                Item item;
                if (amount < 5000)
                {
                    item = new Gold(amount);
                    amount = 0;
                }
                else if (amount <= 1000000)
                {
                    item = new BankCheck(amount);
                    amount = 0;
                }
                else
                {
                    item = new BankCheck(1000000);
                    amount -= 1000000;
                }

                if (box.TryDropItem(from, item, false))
                {
                    items.Add(item);
                }
                else
                {
                    item.Delete();
                    foreach (var curItem in items)
                    {
                        curItem.Delete();
                    }

                    return false;
                }
            }

            return true;
        }

        public static int DepositUpTo(Mobile from, int amount)
        {
            // If for whatever reason the TOL checks fail, we should still try old methods for depositing currency.
			if (AccountGold.Enabled && from.Account != null && from.Account.DepositGold(amount))
            {
                return amount;
            }

            var box = from.FindBankNoCreate();

            if (box == null)
            {
                return 0;
            }

            var amountLeft = amount;
            while (amountLeft > 0)
            {
                Item item;
                int amountGiven;

                if (amountLeft < 5000)
                {
                    item = new Gold(amountLeft);
                    amountGiven = amountLeft;
                }
                else if (amountLeft <= 1000000)
                {
                    item = new BankCheck(amountLeft);
                    amountGiven = amountLeft;
                }
                else
                {
                    item = new BankCheck(1000000);
                    amountGiven = 1000000;
                }

                if (box.TryDropItem(from, item, false))
                {
                    amountLeft -= amountGiven;
                }
                else
                {
                    item.Delete();
                    break;
                }
            }

            return amount - amountLeft;
        }

        public static void Deposit(Container cont, int amount)
        {
            while (amount > 0)
            {
                Item item;

                if (amount < 5000)
                {
                    item = new Gold(amount);
                    amount = 0;
                }
                else if (amount <= 1000000)
                {
                    item = new BankCheck(amount);
                    amount = 0;
                }
                else
                {
                    item = new BankCheck(1000000);
                    amount -= 1000000;
                }

                cont.DropItem(item);
            }
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBBanker());
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.InRange(Location, 12))
            {
                return true;
            }

            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (!e.Handled && e.Mobile.InRange(Location, 12))
            {
                foreach (var keyword in e.Keywords)
                {
                    switch (keyword)
                    {
                        case 0x0000: // *withdraw*
                            {
                                e.Handled = true;

                                if (e.Mobile.Criminal)
                                {
                                    Say(500389); // I will not do business with a criminal!
                                    break;
                                }

                                var split = e.Speech.Split(' ');

                                if (split.Length >= 2)
                                {
                                    int amount;

                                    var pack = e.Mobile.Backpack;

                                    if (!int.TryParse(split[1], out amount))
                                    {
                                        break;
                                    }

                                    if ((!Core.ML && amount > 5000) || (Core.ML && amount > 60000))
                                    {
                                        Say(500381); // Thou canst not withdraw so much at one time!
                                    }
                                    else if (pack == null || pack.Deleted || !(pack.TotalWeight < pack.MaxWeight) || !(pack.TotalItems < pack.MaxItems))
                                    {
                                        Say(1048147); // Your backpack can't hold anything else.
                                    }
                                    else if (amount > 0)
                                    {
                                        var box = e.Mobile.FindBankNoCreate();

                                        if (box == null || !Withdraw(e.Mobile, amount))
                                        {
                                            Say(500384);
                                            // Ah, art thou trying to fool me? Thou hast not so much gold!
                                        }
                                        else
                                        {
                                            //acnt.TotalGold = -amount;
                                            pack.DropItem(new Gold(amount));

                                            Say(1010005); // Thou hast withdrawn gold from thy account.
                                        }
                                    }
                                }

                                break;
                            }
                        case 0x0001: // *balance*
                            {
                                e.Handled = true;

                                if (e.Mobile.Criminal)
                                {
                                    Say(500389); // I will not do business with a criminal!
                                    break;
                                }

								if (AccountGold.Enabled && e.Mobile.Account != null)
                                {
                                    Say(1155855, String.Format("{0:#,0}\t{1:#,0}", e.Mobile.Account.TotalPlat, e.Mobile.Account.TotalGold));
                                    // Thy current bank balance is ~1_AMOUNT~ platinum and ~2_AMOUNT~ gold.
                                }
                                else
                                {
                                    Say(1042759, GetBalance(e.Mobile).ToString("#,0")); // Thy current bank balance is ~1_AMOUNT~ gold.
                                }
                            }
                            break;
                        case 0x0002: // *bank*
                            {
                                e.Handled = true;

                                if (e.Mobile.Criminal)
                                {
                                    Say(500378); // Thou art a criminal and cannot access thy bank box.
                                    break;
                                }

                                e.Mobile.BankBox.Open();
                            }
                            break;
                        case 0x0003: // *check*
                            {
                                e.Handled = true;

                                if (e.Mobile.Criminal)
                                {
                                    Say(500389); // I will not do business with a criminal!
                                    break;
                                }

								if (AccountGold.Enabled && e.Mobile.Account != null)
                                {
                                    Say("We no longer offer a checking service.");
                                    break;
                                }

                                var split = e.Speech.Split(' ');

                                if (split.Length >= 2)
                                {
                                    int amount;

                                    if (!int.TryParse(split[1], out amount))
                                    {
                                        break;
                                    }

                                    if (amount < 5000)
                                    {
                                        Say(1010006); // We cannot create checks for such a paltry amount of gold!
                                    }
                                    else if (amount > 1000000)
                                    {
                                        Say(1010007); // Our policies prevent us from creating checks worth that much!
                                    }
                                    else
                                    {
                                        var check = new BankCheck(amount);

                                        var box = e.Mobile.BankBox;

                                        if (!box.TryDropItem(e.Mobile, check, false))
                                        {
                                            Say(500386); // There's not enough room in your bankbox for the check!
                                            check.Delete();
                                        }
                                        else if (!box.ConsumeTotal(typeof(Gold), amount))
                                        {
                                            Say(500384); // Ah, art thou trying to fool me? Thou hast not so much gold!
                                            check.Delete();
                                        }
                                        else
                                        {
                                            Say(1042673, AffixType.Append, amount.ToString("#,0"), "");
                                            // Into your bank box I have placed a check in the amount of:
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            base.OnSpeech(e);
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive)
            {
                list.Add(new OpenBankEntry(from, this));
            }

            base.AddCustomContextEntries(from, list);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }
}