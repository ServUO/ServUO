using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace CustomsFramework.Systems.VIPSystem
{
    public class VIPStore : Gump
    {
        readonly Mobile mobile;

        public VIPStore(Mobile from)
            : base(150, 150)
        {
            VIPCore core = World.GetCore(typeof(VIPCore)) as VIPCore;
            this.mobile = from;
            PlayerMobile player = from as PlayerMobile;
            VIPModule module = player.GetModule(typeof(VIPModule)) as VIPModule;

            int whiteText = 2100;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            // Theme
            this.AddBackground(0, 0, 750, 452, 9270);
            this.AddImageTiled(251, 44, 5, 340, 2701);
            this.AddImageTiled(508, 44, 5, 340, 2701);
            this.AddImageTiled(17, 41, 716, 4, 2700);
            this.AddImageTiled(17, 178, 716, 4, 2700);
            this.AddImageTiled(17, 384, 716, 4, 2700);
            this.AddBackground(535, 432, 230, 52, 9270);

            this.AddImage(312, 10, 5359, 2213);
            this.AddImage(411, 10, 5359, 2213);
            this.AddLabel(353, 18, whiteText, @"VIP Store");
            this.AddButton(695, 16, 22153, 22155, 1010, GumpButtonType.Reply, 0);
            this.AddButton(718, 16, 22150, 22152, 0, GumpButtonType.Reply, 0);

            this.AddImage(50, 64, 100, 2213);
            this.AddLabel(108, 79, whiteText, @"Gold");
            this.AddButton(89, 103, 4014, 4016, 1001, GumpButtonType.Reply, 0);
            this.AddLabel(125, 104, whiteText, @"Buy!");
            this.AddLabel(69, 120, whiteText, String.Format("{0} Donator Deeds", core.GoldFee));

            this.AddImage(312, 64, 100, 2407);
            this.AddLabel(364, 79, whiteText, @"Silver");
            this.AddButton(349, 103, 4014, 4016, 1002, GumpButtonType.Reply, 0);
            this.AddLabel(385, 104, whiteText, @"Buy!");
            this.AddLabel(330, 120, whiteText, String.Format("{0} Donator Deeds", core.SilverFee));

            this.AddImage(556, 64, 100, 1055);
            this.AddLabel(605, 79, whiteText, @"Bronze");
            this.AddButton(594, 103, 4014, 4016, 1003, GumpButtonType.Reply, 0);
            this.AddLabel(630, 104, whiteText, @"Buy!");
            this.AddLabel(578, 120, whiteText, String.Format("{0} Donator Deeds", core.BronzeFee));

            // Bonuses
            #region Gold
            this.AddLabel(55, 185, whiteText, String.Format("{0} Donator Deeds Each", core.GoldBonusFee));

            this.AddButton(18, 227, 4014, 4016, 1, GumpButtonType.Reply, 0);
            this.AddLabel(58, 230, whiteText, @"Loot Gold From Corpses *");

            this.AddButton(18, 257, 4014, 4016, 2, GumpButtonType.Reply, 0);
            this.AddLabel(58, 260, whiteText, @"Global Bank Commands");

            this.AddButton(18, 287, 4014, 4016, 3, GumpButtonType.Reply, 0);
            this.AddLabel(58, 290, whiteText, @"Smart Grab Bags");

            this.AddButton(18, 317, 4014, 4016, 4, GumpButtonType.Reply, 0);
            this.AddLabel(58, 320, whiteText, @"Free House Commits");

            this.AddButton(18, 347, 4014, 4016, 5, GumpButtonType.Reply, 0);
            this.AddLabel(58, 350, whiteText, @"Unlimited Tools **");
            #endregion
            #region Silver
            this.AddLabel(316, 185, whiteText, String.Format("{0} Donator Deeds Each", core.SilverBonusFee));

            this.AddButton(269, 227, 4014, 4016, 6, GumpButtonType.Reply, 0);
            this.AddLabel(309, 230, whiteText, @"Full LRC At All Times");

            this.AddButton(269, 257, 4014, 4016, 7, GumpButtonType.Reply, 0);
            this.AddLabel(309, 260, whiteText, @"%10 Extra Bank Space");

            this.AddButton(269, 287, 4014, 4016, 8, GumpButtonType.Reply, 0);
            this.AddLabel(309, 290, whiteText, @"Unlimited Life Stones");

            this.AddButton(269, 317, 4014, 4016, 9, GumpButtonType.Reply, 0);
            this.AddLabel(309, 320, whiteText, @"Loot Gold From Ground *");

            this.AddButton(269, 347, 4014, 4016, 10, GumpButtonType.Reply, 0);
            this.AddLabel(309, 350, whiteText, @"Double Resources On All Maps");
            #endregion
            #region Bronze
            this.AddLabel(556, 185, whiteText, String.Format("{0} Donator Deeds Each", core.BronzeBonusFee));

            this.AddLabel(560, 230, whiteText, @"Ressurection Protection");
            this.AddButton(521, 227, 4014, 4016, 11, GumpButtonType.Reply, 0);

            this.AddLabel(560, 260, whiteText, @"Toolbar Access");
            this.AddButton(521, 257, 4014, 4016, 12, GumpButtonType.Reply, 0);

            this.AddLabel(560, 290, whiteText, @"VIP Commands");
            this.AddButton(521, 287, 4014, 4016, 13, GumpButtonType.Reply, 0);

            this.AddLabel(560, 320, whiteText, @"Faster Skill/Stat Gain");
            this.AddButton(521, 317, 4014, 4016, 14, GumpButtonType.Reply, 0);

            this.AddLabel(560, 350, whiteText, @"Free Corpse Return");
            this.AddButton(521, 347, 4014, 4016, 15, GumpButtonType.Reply, 0);
            #endregion

            this.AddLabel(20, 394, whiteText, @"* Bonus used with ledger.");
            this.AddLabel(20, 417, whiteText, @"** Non-crafting Tools Only");

            this.AddLabel(558, 435, whiteText, @"You have 0 Donator Deeds");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            VIPCore core = World.GetCore(typeof(VIPCore)) as VIPCore;

            switch (info.ButtonID)
            {
                default:
                    {
                        goto case 0;
                    }
                case 0:
                    {
                        from.SendMessage("You decide to buy nothing.");
                        break;
                    }
                case 1:
                    {
                        if (core.GetBalance(from) >= core.GoldBonusFee)
                        {
                            Item deed = new VIPFreeHouseDecorationDeed();

                            if (!from.PlaceInBackpack(deed))
                            {
                                deed.Delete();
                                from.SendLocalizedMessage(1078837);
                            }
                            else
                            {
                                from.SendMessage("Thank you for donating and helping make this a better place!");
                                from.SendMessage("To use your deed, simply double click it. Have fun. =D");
                            }
                        }
                        else
                        {
                            this.FailedPurchase(from);
                        }
                        break;
                    }
            }
        }

        private void FailedPurchase(Mobile from)
        {
            from.SendMessage("You don't have enough to purchase that!");
            from.CloseGump(typeof(VIPStore));
            from.SendGump(new VIPStore(from));
        }
    }
}