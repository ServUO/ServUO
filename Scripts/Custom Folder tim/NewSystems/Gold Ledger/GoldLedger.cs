/**************************************
*Script Name: Alpha Gold Ledger       *
*Author: Joeku AKA Demortris          *
*Mods By:Callandor2k & Prplbeast      *
*For use with RunUO 2.0.0             * 
*Client Tested with: 5.0.6b           *
*Version: 1.7                         *
*Initial Release: 11/12/07            *
**************************************/

using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Gumps;
using Server.Misc;
using Server.Network;

namespace Server.Items
{
    public class GoldLedger : Item
    {
        public const double GoldWeight = 0.0066;
	//  public const double GoldWeight = 0.02;      // original
        public const bool GoldAutoLootAvailable = true;
        public const bool GoldSweeperAvailable = true;
        public double d_WeightScale = 0.0;
        public bool b_open = false;
        private int i_Gold;
        private int i_Owner;
        private bool b_GoldAutoLoot = true;
        private bool b_GoldSweeper = true;
        public int i_Selection = 0;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Gold
        {
            get { return i_Gold; }
            set
            {
                if (value > 999999999)
                    i_Gold = 999999999;
                else if (value < 0)
                    i_Gold = 0;
                else
                    i_Gold = value;
                this.AppendWeight();
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.Administrator)]
        public int Owner { get { return i_Owner; } set { i_Owner = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool GoldAutoLoot { get { return b_GoldAutoLoot; } set { b_GoldAutoLoot = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool GoldSweeper { get { return b_GoldSweeper; } set { b_GoldSweeper = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double WeightScale { get { return d_WeightScale; } set { d_WeightScale = value; this.AppendWeight(); } }

        public override bool ForceShowProperties { get { return ObjectPropertyList.Enabled; } }

        [Constructable]
        public GoldLedger()
            : base(7714)
        {
            Name = "a gold ledger";
            Hue = 2125;
            Weight = 1;
            LootType = LootType.Blessed;
        }
        [Constructable]
        public GoldLedger(int value)
            : base(7714)
        {
            Name = "a gold ledger";
            Hue = 2125;
            Gold = value;
            Weight = 1;
            LootType = LootType.Blessed;
        }

        public override void UpdateTotals()
        {
            base.UpdateTotals();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                if (i_Owner == 0)
                {
                    i_Owner = from.Serial;
                    Name = from.Name + "'s Gold Ledger";
                    from.SendGump(new GoldLedgerGump(this));
                    this.b_open = true;
                }
                if (from.Serial == i_Owner)
                {
                    Name = from.Name + "'s Gold Ledger";
                    from.CloseGump(typeof(GoldLedgerGump));
                    from.SendGump(new GoldLedgerGump(this));
                    from.SendMessage(2125, "You open your gold ledger.");
                    this.b_open = true;
                }
                else if (from.AccessLevel >= AccessLevel.GameMaster)
                {
                    from.SendMessage(1173, "Select a new owner for this Gold ledger.");
                    BeginSetOwner(from);
                }
                else
                {
                    from.PlaySound(1074); //play no!! sound
                    from.SendMessage(1173, "This book is not yours and you cannot seem to write your name in it!");
                }
            }
            else
                from.SendMessage(1173, "The Gold ledger must be in your backpack to be used.");
        }

        public void BeginSetOwner(Mobile from)
        {
            from.Target = new SetOwnerTarget(this);
        }

        public class SetOwnerTarget : Target
        {
            private GoldLedger m_TL;

            public SetOwnerTarget(GoldLedger tl)
                : base(18, false, TargetFlags.None)
            {
                m_TL = tl;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_TL.Deleted)
                    return;

                m_TL.EndSetOwner(from, targeted);
            }
        }

        public void EndSetOwner(Mobile from, object obj)
        {
            if (obj is PlayerMobile)
            {
                PlayerMobile m = obj as PlayerMobile;
                if (m.Alive)
                {
                    if (!(this.Deleted))
                    {
                        if (m.Name != null)
                        {
                            this.Owner = m.Serial;
                            this.Name = m.Name + "'s Gold Ledger";
                            from.SendMessage(1173, "You set the new owner as: {0}", m.Name);
                            m.SendMessage(1173, "You became the owner of a new Gold ledger book.");
                        }
                        else
                            from.SendMessage(1173, "Your target doesn't have a name.");
                    }
                    else
                        from.SendMessage(1173, "The ledger was deleted before you selected your target.");
                }
                else
                    from.SendMessage(1173, "Your target is dead, please choose a target that is alive.");
            }
            else
                from.SendMessage(1173, "Only players can be the owners of Gold ledger.");
        }

        public void BeginAddGold(Mobile from)
        {
            if (this.Gold >= 999999999)
            {
                from.SendGump(new GoldLedgerGump(this));
                from.SendMessage(2125, "You cannot deposit any more gold into this gold ledger.");
                return;
            }
            from.Target = new AddGoldTarget(this);
            from.SendMessage(2125, "What do you wish to deposit? (ESC to cancel)");
        }

        public class AddGoldTarget : Target
        {
            private GoldLedger m_TL;

            public AddGoldTarget(GoldLedger tl)
                : base(18, false, TargetFlags.None)
            {
                m_TL = tl;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_TL.Deleted)
                    return;
                m_TL.EndAddGold(from, targeted);
            }
        }

        public void EndAddGold(Mobile from, object obj)
        {
            from.CloseGump(typeof(GoldLedgerGump));

            if (obj is Item)
            {
                Item item = obj as Item;
                double maxWeight = (WeightOverloading.GetMaxWeight(from));
                int golda = 0;

                if (!this.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                    return;
                }
                if (!item.IsAccessibleTo(from))
                {
                    from.SendGump(new GoldLedgerGump(this));
                    from.SendLocalizedMessage(3000270); // That is not accessible.
                    return;
                }

                if (obj is Gold)
                {
                    Gold gold = obj as Gold;

                    golda = gold.Amount;
                    if ((gold.Amount + this.Gold) > 999999999)
                        golda = (999999999 - this.Gold);
                    double maxgold = golda;
                    if (this.d_WeightScale > 0 && gold.RootParent != from)
                        maxgold = ((maxWeight - ((double)Mobile.BodyWeight + (double)from.TotalWeight)) / this.d_WeightScale);
                    if (golda > maxgold)
                        golda = (int)maxgold;
                    if (golda < gold.Amount)
                        gold.Amount -= golda;
                    else
                        gold.Delete();

                    this.Gold += golda;

                    from.SendGump(new GoldLedgerGump(this));
                    from.SendMessage(2125, String.Format("You deposit {0} gold into your gold ledger.", golda.ToString("#,0")));
                    from.PlaySound(0x249);
                }
                else if (obj is BankCheck)
                {
                    BankCheck check = obj as BankCheck;

                    golda = check.Worth;
                    if ((check.Worth + this.Gold) > 999999999)
                        golda = (999999999 - this.Gold);
                    double maxgold = golda;
                    if (this.d_WeightScale > 0)
                        maxgold = ((maxWeight - ((double)Mobile.BodyWeight + (double)from.TotalWeight)) / this.d_WeightScale);
                    if (golda > maxgold)
                        golda = (int)maxgold;
                    if (golda < check.Worth)
                        check.Worth -= golda;
                    else
                        check.Delete();

                    this.Gold += golda;

                    from.SendGump(new GoldLedgerGump(this));
                    from.SendMessage(2125, String.Format("You deposit a bank check worth {0} gold into your gold ledger.", golda.ToString("#,0")));
                    from.PlaySound(0x249);
                }
                else if (obj is GoldLedger && obj != this)
                {
                    GoldLedger gledger = obj as GoldLedger;

                    golda = gledger.Gold;
                    if ((gledger.Gold + this.Gold) > 999999999)
                        golda = (999999999 - this.Gold);
                    double maxgold = golda;
                    if (this.d_WeightScale > 0)
                        maxgold = ((maxWeight - ((double)Mobile.BodyWeight + (double)from.TotalWeight)) / this.d_WeightScale);
                    if (golda > maxgold)
                        golda = (int)maxgold;
                    gledger.Gold -= golda;

                    this.Gold += golda;

                    from.SendGump(new GoldLedgerGump(this));
                    from.SendMessage(2125, String.Format("You transfer {0} gold into your gold ledger.", golda.ToString("#,0")));
                    from.PlaySound(0x249);
                }
                else
                {
                    from.SendGump(new GoldLedgerGump(this));
                    from.SendMessage(2125, "You can only deposit gold or bank checks into your gold ledger.");
                }
            }
            else
            {
                from.SendGump(new GoldLedgerGump(this));
                from.SendMessage(2125, "You can only deposit gold or bank checks into your gold ledger.");
            }

            from.SendMessage(2125, "Do you want to deposit anything else? (ESC to cancel)");
            from.Target = new AddGoldTarget(this);
        }

        public GoldLedger(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            string weight = "" + (int)this.Weight;

            base.AddNameProperties(list);

            list.Add(1060738, this.Gold.ToString("#,0") + " Gold"); //value: ~1_val~
            if (this.GoldSweeper && GoldLedger.GoldSweeperAvailable)
                list.Add(1060661, "Gold Sweeper\tEnabled");
            else if (!this.GoldSweeper && GoldLedger.GoldSweeperAvailable)
                list.Add(1060661, "Gold Sweeper\tDisabled");
            if (this.GoldAutoLoot && GoldLedger.GoldAutoLootAvailable)
                list.Add(1060660, "Gold Looting\tAutomatic");
            else if (!this.GoldAutoLoot && GoldLedger.GoldAutoLootAvailable)
                list.Add(1060660, "Gold Looting\tManual");
            if (this.d_WeightScale > 0)
            {
                if (this.Weight < 2 && this.Weight >= 1)
                    list.Add(1072788, weight);	//Weight: ~1_WEIGHT~ stone
                else if (this.Weight >= 2)
                    list.Add(1072789, weight);	//Weight: ~1_WEIGHT~ stones
            }
        }

        public void AppendWeight()
        {
            double newweight = (this.Gold * this.d_WeightScale);
            this.Weight = newweight + 1;
        }

        public override bool OnDroppedToMobile(Mobile from, Mobile target)
        {
            Item item = target.Backpack.FindItemByType(typeof(GoldLedger));

            if (item != null)
            {
                if (target == from)
                    from.SendMessage(2125, "You can only carry one gold ledger!");
                else
                    from.SendMessage(2125, "That player can only carry one gold ledger!");
                return false;
            }

            return true;
        }

        public override bool OnDroppedInto(Mobile from, Container target, Point3D p)
        {
            if (target == from.Backpack)
            {
                Item item = from.Backpack.FindItemByType(typeof(GoldLedger));

                if (item != null)
                {
                    from.SendMessage(2125, "You can only carry one gold ledger!");
                    return false;
                }
            }

            else if (target.IsChildOf(from.Backpack))
            {
                Item item = from.Backpack.FindItemByType(typeof(GoldLedger));

                if (item != null)
                {
                    from.SendMessage(2125, "You can only carry one gold ledger!");
                    return false;
                }
            }

            return target.OnDragDropInto(from, this, p);
        }

        public override bool OnDroppedOnto(Mobile from, Item target)
        {
            if (target == from.Backpack)
            {
                Item item = from.Backpack.FindItemByType(typeof(GoldLedger));

                if (item != null)
                {
                    from.SendMessage(2125, "You can only carry one gold ledger!");
                    return false;
                }
            }

            else if (target.IsChildOf(from.Backpack))
            {
                Item item = from.Backpack.FindItemByType(typeof(GoldLedger));

                if (item != null)
                {
                    from.SendMessage(2125, "You can only carry one gold ledger!");
                    return false;
                }
            }

            return target.OnDragDrop(from, this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write(i_Owner);

            writer.Write(i_Gold);
            writer.Write(b_GoldAutoLoot);
            writer.Write(b_GoldSweeper);
            writer.Write(d_WeightScale);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        i_Owner = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        i_Gold = reader.ReadInt();
                        b_GoldAutoLoot = reader.ReadBool();
                        b_GoldSweeper = reader.ReadBool();
                        d_WeightScale = reader.ReadDouble();
                        break;
                    }
            }
        }
    }

    public class GoldLedgerGump : Gump
    {
        public GoldLedger Ledger;

        public GoldLedgerGump(GoldLedger ledger)
            : base(225, 100)
        {
            Resizable = false;

            Ledger = ledger;
            int val = Ledger.Gold;
            if (val > 5000)
                val = 5000;

            bool bool1 = true;
            bool bool2 = false;
            if (Ledger.i_Selection == 1)
            {
                bool1 = false;
                bool2 = true;
            }

            AddPage(0);
            AddBackground(50, 50, 400, 365, 3600);
            AddLabel(217, 78, 2124, @"Gold Ledger");
            AddImage(138, 68, 4037, 2124);
            AddImage(328, 70, 4037, 2124);
            AddImage(295, 80, 57, 2101);
            AddImage(175, 80, 59, 2101);
            AddButton(395, 66, 22153, 22155, 8, GumpButtonType.Reply, 0);
            AddButton(417, 68, 3, 4, 0, GumpButtonType.Reply, 0);

            AddBackground(65, 115, 185, 85, 9270);
            AddLabel(107, 130, 2124, @"Account Balance");
            AddBackground(103, 157, 109, 25, 9350);
            AddLabel(112, 160, 0, Ledger.Gold.ToString("#,0"));

            AddBackground(250, 115, 185, 85, 9270);
            AddLabel(291, 130, 2124, @"Deposit Currency");
            AddButton(289, 154, 10800, 10820, 5, GumpButtonType.Reply, 0);

            AddBackground(65, 200, 185, 115, 9270);
            AddLabel(132, 215, 2124, @"Currency");
            AddItem(97, 242, 3823, 0);
            AddLabel(108, 242, 2101, @"Gold");
            AddLabel(154, 243, 2101, @"or");
            AddItem(174, 240, 5360, 52);
            AddLabel(188, 237, 2101, @"Bank");
            AddLabel(185, 249, 2101, @"Check");
            AddRadio(114, 275, 210, 211, bool1, 2);
            AddRadio(182, 275, 210, 211, bool2, 3);

            AddBackground(250, 200, 185, 115, 9270);
            AddLabel(284, 215, 2124, @"Withdraw Currency");
            AddBackground(288, 242, 109, 30, 2620);
            AddTextEntry(295, 247, 95, 20, 2101, 4, val.ToString());
            AddLabel(290, 277, 2101, @"Finalize");
            AddButton(343, 276, 2128, 2129, 1, GumpButtonType.Reply, 0);

            if (GoldLedger.GoldSweeperAvailable)
            {
                AddBackground(65, 315, 185, 85, 9270);
                AddLabel(121, 330, 2124, @"Gold Sweeper");
                AddLabel(90, 357, 2101, @"Sweeper Enabled");
                AddButton(199, 354, (Ledger.GoldSweeper ? 9723 : 9720), (Ledger.GoldSweeper ? 9720 : 9723), 6, GumpButtonType.Reply, 0);
            }

            if (GoldLedger.GoldAutoLootAvailable)
            {
                AddBackground(250, 315, 185, 85, 9270);
                AddLabel(297, 330, 2124, @"Gold Auto-Loot");
                AddLabel(275, 357, 2101, @"Gold Looting");
                AddButton(357, 357, (Ledger.GoldAutoLoot ? 2113 : 2116), (Ledger.GoldAutoLoot ? 2116 : 2113), 7, GumpButtonType.Reply, 0);
            }

            AddImage(0, -7, 10400, 2124);
            AddImage(0, 167, 10401, 2124);
            AddImage(0, 345, 10402, 2124);
            AddImage(418, -7, 10410, 2124);
            AddImage(418, 167, 10411, 2124);
            AddImage(418, 345, 10412, 2124);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (!Ledger.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            switch (info.ButtonID)
            {
                case 0:	//Close
                    {
                        int[] switches = info.Switches;

                        for (int i = 0; i < switches.Length; i++)
                        {
                            if (switches[i] == 2)
                                Ledger.i_Selection = 0;
                            if (switches[i] == 3)
                                Ledger.i_Selection = 1;
                        }

                        Ledger.b_open = false;
                        from.SendMessage(2125, "You close your gold ledger.");
                        break;
                    }

                case 1:	//Withdraw Currency
                    {
                        string WithdrawString = info.GetTextEntry(4).Text.Replace(",", "");

                        if (WithdrawString != null)
                        {
                            int WithdrawAmount = 0;

                            try
                            {
                                WithdrawAmount = Convert.ToInt32(WithdrawString, 10);
                            }
                            catch
                            {
                                //from.SendGump(new GoldLedgerGump(Ledger));  // Commented out to keep from getting multiple gumps.
                                from.SendMessage(2125, "You can't withdraw letters, silly! Only numbers!");
                            }

                            int[] switches = info.Switches;

                            for (int i = 0; i < switches.Length; i++)
                            {
                                if (switches[i] == 2)
                                    Ledger.i_Selection = 0;
                                if (switches[i] == 3)
                                    Ledger.i_Selection = 1;
                            }

                            if (WithdrawAmount < 0)
                            {
                                from.SendGump(new GoldLedgerGump(Ledger));
                                from.SendMessage(2125, "You can't withdraw negative gold, silly!");
                                return;
                            }

                            if (WithdrawAmount == 0)
                            {
                                from.SendGump(new GoldLedgerGump(Ledger));
                                return;
                            }

                            if (WithdrawAmount > Ledger.Gold)
                                WithdrawAmount = Ledger.Gold;

                            if (Ledger.i_Selection == 0)
                            {
                                double maxWeight = (WeightOverloading.GetMaxWeight(from));
                                double newledgerweight = ((Ledger.Gold - WithdrawAmount) * Ledger.d_WeightScale);
                                double curWeight = ((Mobile.BodyWeight + from.TotalWeight) - (Ledger.Weight - newledgerweight));

                                double maxGold = ((maxWeight - (Mobile.BodyWeight + from.TotalWeight)) / (((GoldLedger.GoldWeight > Ledger.d_WeightScale) ? GoldLedger.GoldWeight : Ledger.d_WeightScale) - ((GoldLedger.GoldWeight > Ledger.d_WeightScale) ? Ledger.d_WeightScale : GoldLedger.GoldWeight)));

                                if ((int)maxGold < 1)
                                {
                                    from.SendGump(new GoldLedgerGump(Ledger));
                                    from.SendMessage(2125, "You can't carry that many stones.");
                                }
                                else
                                {
                                    if (WithdrawAmount > (int)maxGold)
                                    {
                                        WithdrawAmount = (int)maxGold;
                                        from.SendMessage(2125, "You can only withdraw {0} stones' worth of gold.", (int)Math.Ceiling((int)maxGold * GoldLedger.GoldWeight));
                                    }

                                    Ledger.Gold -= WithdrawAmount;
                                    from.SendGump(new GoldLedgerGump(Ledger));
                                    Ledger.AppendWeight();

                                    int toAdd = WithdrawAmount;

                                    Gold gold;
                                    int sixtyk = 60000;

                                    while (toAdd > sixtyk)
                                    {
                                        gold = new Gold(sixtyk);

                                        toAdd -= sixtyk;
                                        from.Backpack.AddItem(gold);
                                        from.SendMessage(2125, "You withdraw {0} gold from your gold ledger.", sixtyk.ToString("#,0"));
                                    }

                                    if (toAdd > 0)
                                    {
                                        gold = new Gold(toAdd);

                                        from.Backpack.AddItem(gold);
                                        from.SendMessage(2125, "You withdraw {0} gold from your gold ledger.", toAdd.ToString("#,0"));
                                    }

                                    from.PlaySound(0x2E6);
                                }
                            }
                            else if (Ledger.i_Selection == 1)
                            {
                                BankCheck check = new BankCheck(WithdrawAmount);

                                Ledger.Gold -= WithdrawAmount;
                                Ledger.AppendWeight();
                                from.Backpack.AddItem(check);
                                from.SendGump(new GoldLedgerGump(Ledger));
                                from.PlaySound(0x42);
                                from.SendMessage(2125, "You withdraw a bank check worth {0} gold from your gold ledger.", WithdrawAmount.ToString("#,0"));
                            }
                        }

                        break;
                    }

                case 5:	//Deposit Currency
                    {
                        int[] switches = info.Switches;

                        for (int i = 0; i < switches.Length; i++)
                        {
                            if (switches[i] == 2)
                                Ledger.i_Selection = 0;
                            if (switches[i] == 3)
                                Ledger.i_Selection = 1;
                        }

                        from.SendGump(new GoldLedgerGump(Ledger));
                        Ledger.BeginAddGold(from);

                        break;
                    }

                case 6:	//Gold Sweeper
                    {
                        int[] switches = info.Switches;

                        for (int i = 0; i < switches.Length; i++)
                        {
                            if (switches[i] == 2)
                                Ledger.i_Selection = 0;
                            if (switches[i] == 3)
                                Ledger.i_Selection = 1;
                        }

                        if (Ledger.GoldSweeper)
                        {
                            Ledger.GoldSweeper = false;
                            from.SendGump(new GoldLedgerGump(Ledger));
                            from.SendMessage(2125, "Gold Sweeper: Disabled");
                        }
                        else
                        {
                            Ledger.GoldSweeper = true;
                            from.SendGump(new GoldLedgerGump(Ledger));
                            from.SendMessage(2125, "Gold Sweeper: Enabled");
                        }

                        break;
                    }

                case 7:	//Auto-Loot
                    {
                        int[] switches = info.Switches;

                        for (int i = 0; i < switches.Length; i++)
                        {
                            if (switches[i] == 2)
                                Ledger.i_Selection = 0;
                            if (switches[i] == 3)
                                Ledger.i_Selection = 1;
                        }

                        if (Ledger.GoldAutoLoot)
                        {
                            Ledger.GoldAutoLoot = false;
                            from.SendGump(new GoldLedgerGump(Ledger));
                            from.SendMessage(2125, "Gold Looting: Manual");
                        }
                        else
                        {
                            Ledger.GoldAutoLoot = true;
                            from.SendGump(new GoldLedgerGump(Ledger));
                            from.SendMessage(2125, "Gold Looting: Automatic");
                        }

                        break;
                    }

                case 8: //Help
                    {
                        from.SendGump(new GoldLedgerGump(Ledger));
                        from.CloseGump(typeof(GoldLedgerHelp));
                        from.SendGump(new GoldLedgerHelp());

                        break;
                    }
            }
        }
    }

    public class GoldLedgerHelp : Gump
    {
        public string HelpString1 = "<center>Welcome to the Gold Ledger Help menu!</center><br><u>Information</u><br>*Script Name: Alpha Gold Ledger<br>*Author: Joeku AKA Demortris<br>*Version: 1.6<br>*Client Tested with: 5.0.2a<br>*Revision Date: 06/04/06<br>*Public Release: 01/30/06<br>*Purpose: Makes carrying gold more convenient, as well as decreasing item count<br>*Download Link: <A HREF=\"http://www.runuo.com/forums/showthread.php?t=67525\">Click Here</A><br>*Special thanks to Doomsday Dragon<br><p><u>Overview</u><br>This Gold Ledger can hold up to 999,999,999 gold. With a user-friendly menu and design, this script is very easy-to-use. You can only hold one gold ledger in your pack at a time. You can withdraw/deposit both gold and bank checks, and it has advanced functions (if you withdraw more than 60,000 gold, it splits it into seperate piles; it will detect how much the ledger will weigh after the withdrawal and how much the added gold/bank check will weigh to calculate whether or not to let you withdraw, etc.) There are also two special functions: Gold Sweeper and Gold Auto-Loot (these can be disabled by the shard administrator.) Details for these features are covered below.<br><p><u>Account Balance</u><br>This sub-window displays how much gold you have in your gold ledger. You cannot withdraw more gold than you have in your account.<br><p><u>Deposit Currency</u><br>This sub-window gives you access to deposit gold/bank checks into your gold ledger. Click the big blue button, and target a pile of gold, a bank check, or a gold ledger that you can access. If done correctly, this will deposit the currency into your gold ledger's account. If you can't hold what you're trying to deposit, it will deposit as much as you can hold.<br><p><u>Currency</u><br>This sub-window lets you select what form of currency that";
        public string HelpString2 = " you want to withdraw gold in; if you select Gold, you will withdraw gold from your gold ledger when you click the withdraw button. If you select Bank Check, you will withdraw a bank check from your gold ledger when you click the withdraw button.<br><p><u>Withdraw Currency</u><br>This sub-window lets you withdraw currency from your gold ledger. In the entry window, type in how much you want to withdraw out of your account (note that you can't withdraw letters, only numbers!) After you have entered how much you want to withdraw, click the 'Okay' button next to 'Finalize'. This will withdraw currency from your account, in whatever form that you specified in the Currency window. If you enter a number higher than your account balance, it will attempt to withdraw the entire account. If you try to withdraw something that you will not be able to hold, it will withdraw as much as it can.<br><p><u>Gold Sweeper</u><br>In this sub-window, you can specify whether or not you want to enable the gold sweeper. If this window doesn't exist, it means that the shard administrator overrode the default settings and disabled the gold sweeper. If you have the gold sweeper enabled, the gold ledger will attempt to deposit any gold that you walk over into its account. The gold ledger will not sweep gold for you if it would put you over your weight limit. If you have it disabled, it will do nothing.<br><p><u>Gold Auto-Loot</u><br>In this sub-window, you can specify what you want gold looting to be: manual or automatic. If this window doesn't exist, it means that the shard administrator overrode the default settings and disabled the gold auto-loot. If it is set to manual, the gold ledger will not do anything. If it is set to automatic, the gold ledger will automatically loot gold from any monster that you ";
        public string HelpString3 = "have looting rights to that dies within 12 paces of you. The gold ledger will not loot gold for you if it would put you over your weight limit.<br><p><u>Other</u><br>-Weight: The weight of the gold ledger is calculated by how much gold it is holding. The default setting is for the gold to weigh 1/100 of what it normally does if it is inside the ledger.<br>-Abilities: The gold ledger's abilities (gold sweeper and gold auto-loot) will only work if the gold ledger is in your pack.<br>-Contact information: You can e-mail the creator of this script at demortris@adelphia.net.";

        public GoldLedgerHelp()
            : base(300, 160)
        {
            Resizable = false;

            AddPage(0);
            AddBackground(50, 50, 400, 365, 3600);
            AddLabel(197, 78, 2124, @"Gold Ledger Help");
            AddButton(417, 68, 3, 4, 0, GumpButtonType.Reply, 0);
            AddHtml(100, 120, 300, 250, HelpString1 + HelpString2 + HelpString3, true, true);
        }
    }

    public class GiveGold
    {
        public static void GoldTransfer(Mobile m, Container c, BaseCreature creature)
        {
            if (creature.IsBonded)
                return;

            if (m.Map != creature.Map || !m.InRange(creature, 12))
                return;

            string nametitle = null;
            if (creature.Title != null)
                nametitle = creature.Name + " " + creature.Title;

            else
                nametitle = creature.Name;

            Item item = m.Backpack.FindItemByType(typeof(GoldLedger));
            GoldLedger ledger = item as GoldLedger;

            if (ledger == null)
                return;

            if (!ledger.GoldAutoLoot || !m.Alive || !GoldLedger.GoldAutoLootAvailable)
                return;

            TransferGold(m, c, ledger, nametitle);
        }

        public static void TransferGold(Mobile m, Container c, GoldLedger ledger, string nametitle)
        {
            Item[] items = c.FindItemsByType(typeof(Gold));

            foreach (Gold ngold in items)
            {
                Gold gold = ngold as Gold;

                if (gold != null)
                {
                    if (ledger.Gold < 999999999)
                    {
                        double maxWeight = (WeightOverloading.GetMaxWeight(m));
                        if ((Mobile.BodyWeight + m.TotalWeight) < (maxWeight))
                        {
                            int golda = gold.Amount;
                            if ((gold.Amount + ledger.Gold) > 999999999)
                                golda = (999999999 - ledger.Gold);
                            double maxgold = golda;
                            if (ledger.d_WeightScale > 0)
                                maxgold = ((maxWeight - ((double)Mobile.BodyWeight + (double)m.TotalWeight)) / ledger.d_WeightScale);
                            if (golda > maxgold)
                                golda = (int)maxgold;
                            int GoldID = 0;
                            if (golda == 1)
                                GoldID = gold.ItemID;
                            else if (golda > 1 && golda < 6)
                                GoldID = gold.ItemID + 1;
                            else if (golda >= 6)
                                GoldID = gold.ItemID + 2;
                            if (golda < gold.Amount)
                                gold.Amount -= golda;
                            else
                                gold.Delete();
                            ledger.Gold += golda;
                            if (ledger.b_open && golda > 0)
                            {
                                m.CloseGump(typeof(GoldLedgerGump));
                                m.SendGump(new GoldLedgerGump(ledger));
                            }

                            if (golda > 4999)
                            {
                                m.SendMessage(2125, "You loot {0} gold from {1} and deposit it into your gold ledger.", golda.ToString("#,0"), nametitle);
                                Effects.SendMovingEffect(c, m, GoldID, 5, 50, true, false);
                                m.PlaySound(0x2E6);
                            }
                        }
                    }
                }
            }
        }

        public static void GoldSweep(Mobile m, Gold gold)
        {
            Item item = m.Backpack.FindItemByType(typeof(GoldLedger));

            GoldLedger ledger = item as GoldLedger;

            if (ledger == null)
                return;

            if (!ledger.GoldSweeper || !GoldLedger.GoldSweeperAvailable || !gold.Movable)
                return;

            if (gold != null)
            {
                if (ledger.Gold < 999999999)
                {
                    double maxWeight = (WeightOverloading.GetMaxWeight(m));
                    if ((Mobile.BodyWeight + m.TotalWeight) < (maxWeight))
                    {
                        int golda = gold.Amount;
                        if ((gold.Amount + ledger.Gold) > 999999999)
                            golda = (999999999 - ledger.Gold);
                        double maxgold = golda;
                        if (ledger.d_WeightScale > 0)
                            maxgold = ((maxWeight - ((double)Mobile.BodyWeight + (double)m.TotalWeight)) / ledger.d_WeightScale);
                        if (golda > maxgold)
                            golda = (int)maxgold;
                        int GoldID = 0;
                        if (golda == 1)
                            GoldID = gold.ItemID;
                        else if (golda > 1 && golda < 6)
                            GoldID = gold.ItemID + 1;
                        else if (golda >= 6)
                            GoldID = gold.ItemID + 2;
                        if (golda < gold.Amount)
                            gold.Amount -= golda;
                        else
                            gold.Delete();
                        ledger.Gold += golda;
                        if (ledger.b_open && golda > 0)
                        {
                            m.CloseGump(typeof(GoldLedgerGump));
                            m.SendGump(new GoldLedgerGump(ledger));
                        }

                        if (golda > 4999)
                        {
                            m.SendMessage(2125, "You deposit {0} gold into your gold ledger.", golda.ToString("#,0"));
                            if (!m.Mounted)
                                m.Animate(32, 5, 1, true, false, 0);
                            Effects.SendMovingEffect(gold, m, GoldID, 5, 50, true, false);
                            m.PlaySound(0x2E6);
                        }
                    }
                }
            }
        }
    }
}