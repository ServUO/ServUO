using Server.Items;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Gumps
{
    public class SpringDecorTokenGump : Gump
    {
        private readonly SpringDecorToken m_Token;
        private readonly Mobile m_User;

        public SpringDecorTokenGump(SpringDecorToken token, Mobile from)
            : base(60, 36)
        {
            m_Token = token;
            m_User = from;

            AddPage(0);

            AddBackground(0, 0, 520, 404, 0x13BE);
            AddImageTiled(10, 10, 500, 20, 0xA40);
            AddImageTiled(10, 40, 500, 324, 0xA40);
            AddImageTiled(10, 374, 500, 20, 0xA40);
            AddAlphaRegion(10, 10, 500, 384);
            AddButton(10, 374, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 376, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
            AddHtmlLocalized(14, 12, 500, 20, 1075576, 0x7FFF, false, false); // Choose your item from the following pages

            AddPage(1);

            AddImageTiledButton(14, 44, 0x918, 0x919, 0x64, GumpButtonType.Reply, 0, 0x194B, 0x0, 18, 15);
            AddTooltip(1075562);
            AddHtmlLocalized(98, 44, 170, 60, 1075492, 0x7FFF, false, false); // Surveyor's Scope

            AddImageTiledButton(264, 44, 0x918, 0x919, 0x65, GumpButtonType.Reply, 0, 0x1950, 0x0, -4, -7);
            AddTooltip(1075573);
            AddHtmlLocalized(348, 44, 170, 60, 1075498, 0x7FFF, false, false); // Mongbat Dartboard

            AddImageTiledButton(14, 108, 0x918, 0x919, 0x66, GumpButtonType.Reply, 0, 0x1947, 0x0, 18, -9);
            AddTooltip(1075564);
            AddHtmlLocalized(98, 108, 170, 60, 1075494, 0x7FFF, false, false); // Blessed Statue

            AddImageTiledButton(264, 108, 0x918, 0x919, 0x67, GumpButtonType.Reply, 0, 0x1945, 0x0, 18, -24);
            AddTooltip(1075565);
            AddHtmlLocalized(348, 108, 170, 60, 1075495, 0x7FFF, false, false); // Carved Wooden Screen

            AddImageTiledButton(14, 172, 0x918, 0x919, 0x68, GumpButtonType.Reply, 0, 0x1944, 0x0, 18, 11);
            AddTooltip(1075566);
            AddHtmlLocalized(98, 172, 170, 60, 1075496, 0x7FFF, false, false); // Throw Pillow

            AddImageTiledButton(264, 172, 0x918, 0x919, 0x69, GumpButtonType.Reply, 0, 0x194E, 0x0, 18, -8);
            AddTooltip(1075568);
            AddHtmlLocalized(348, 172, 170, 60, 1075501, 0x7FFF, false, false); // Dragon Brazier

            AddImageTiledButton(14, 236, 0x918, 0x919, 0x6A, GumpButtonType.Reply, 0, 0x14EB, 0x1E3, 18, 11);
            AddTooltip(1075571);
            AddHtmlLocalized(98, 236, 170, 60, 1075500, 0x7FFF, false, false); // Navigator's World Map

            AddImageTiledButton(264, 236, 0x918, 0x919, 0x6B, GumpButtonType.Reply, 0, 0x194F, 0x0, 18, 9);
            AddTooltip(1075563);
            AddHtmlLocalized(348, 236, 170, 60, 1075493, 0x7FFF, false, false); // Basket of Herbs

            AddImageTiledButton(14, 300, 0x918, 0x919, 0x6C, GumpButtonType.Reply, 0, 0x1956, 0x2BC, 18, 14);
            AddTooltip(1075572);
            AddHtmlLocalized(98, 300, 170, 60, 1075497, 0x7FFF, false, false); // Shochu

            AddImageTiledButton(264, 300, 0x918, 0x919, 0x6D, GumpButtonType.Reply, 0, 0x1057, 0x1E3, 18, 16);
            AddTooltip(1075567);
            AddHtmlLocalized(348, 300, 170, 60, 1075499, 0x7FFF, false, false); // Mariner's Brass Sextant

            AddButton(400, 374, 0xFA5, 0xFA7, 0, GumpButtonType.Page, 2);
            AddHtmlLocalized(440, 376, 60, 20, 1043353, 0x7FFF, false, false); // Next

            AddPage(2);

            AddButton(300, 374, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 1);
            AddHtmlLocalized(340, 376, 60, 20, 1011393, 0x7FFF, false, false); // Back

            AddImageTiledButton(14, 44, 0x918, 0x919, 0x6E, GumpButtonType.Reply, 0, 0xE7D, 0x4A9, 18, 13);
            AddTooltip(1075570);
            AddHtmlLocalized(98, 44, 170, 60, 1075503, 0x7FFF, false, false); // Heartwood Chest

            AddImageTiledButton(264, 44, 0x918, 0x919, 0x6F, GumpButtonType.Reply, 0, 0x281A, 0x4A8, 7, -8);
            AddTooltip(1075569);
            AddHtmlLocalized(348, 44, 170, 60, 1075502, 0x7FFF, false, false); // Low Yew Table
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Token == null || m_Token.Deleted || info.ButtonID == 0 ||
                m_User == null || m_User.Deleted)
                return;

            if (!m_Token.IsChildOf(m_User.Backpack))
            {
                sender.Mobile.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
                return;
            }

            List<Type> types = new List<Type>();
            int cliloc = 0;

            switch (info.ButtonID)
            {
                case 0x64:
                    types.Add(typeof(SurveyorsScope));
                    cliloc = 1075492;
                    break;
                case 0x65:
                    types.Add(typeof(MongbatDartboard));
                    cliloc = 1075498;
                    break;
                case 0x66:
                    types.Add(typeof(FelineBlessedStatue));
                    cliloc = 1075494;
                    break;
                case 0x67:
                    types.Add(typeof(CarvedWoodenScreen));
                    cliloc = 1075495;
                    break;
                case 0x68:
                    types.Add(typeof(ThrowPillow));
                    cliloc = 1075496;
                    break;
                case 0x69:
                    types.Add(typeof(DragonBrazier));
                    cliloc = 1075501;
                    break;
                case 0x6A:
                    types.Add(typeof(NavigatorsWorldMap));
                    cliloc = 1075500;
                    break;
                case 0x6B:
                    types.Add(typeof(BasketOfHerbs));
                    cliloc = 1075493;
                    break;
                case 0x6C:
                    types.Add(typeof(Shochu));
                    cliloc = 1075497;
                    break;
                case 0x6D:
                    types.Add(typeof(MarinersBrassSextant));
                    cliloc = 1075499;
                    break;
                case 0x6E:
                    types.Add(typeof(HeartwoodChest));
                    cliloc = 1075503;
                    break;
                case 0x6F:
                    types.Add(typeof(LowYewTable));
                    cliloc = 1075502;
                    break;
            }

            if (types.Count > 0 && cliloc > 0)
            {
                sender.Mobile.CloseGump(typeof(ConfirmSpringDecorGump));
                sender.Mobile.SendGump(new ConfirmSpringDecorGump(m_Token, types.ToArray(), cliloc, m_User));
            }
            else
                sender.Mobile.SendLocalizedMessage(501311); // This option is currently disabled, while we evaluate it for game balance.
        }
    }

    public class ConfirmSpringDecorGump : Gump
    {
        private readonly SpringDecorToken m_Token;
        private readonly Type[] m_Selected;
        private readonly Mobile m_User;

        public ConfirmSpringDecorGump(SpringDecorToken token, Type[] selected, int cliloc, Mobile from)
            : base(100, 100)
        {
            m_Token = token;
            m_Selected = selected;
            m_User = from;

            AddPage(0);

            AddBackground(0, 0, 291, 99, 0x13BE);
            AddImageTiled(5, 6, 280, 20, 0xA40);
            AddHtmlLocalized(9, 8, 280, 20, 1070972, 0x7FFF, false, false); // Click "OKAY" to redeem the following:
            AddImageTiled(5, 31, 280, 40, 0xA40);
            AddHtmlLocalized(9, 35, 272, 40, cliloc, 0x7FFF, false, false);
            AddButton(180, 73, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(215, 75, 100, 20, 1011036, 0x7FFF, false, false); // OKAY
            AddButton(5, 73, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, 75, 100, 20, 1060051, 0x7FFF, false, false); // CANCEL
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Token == null || m_Token.Deleted ||
                m_User == null || m_User.Deleted)
                return;

            if (!m_Token.IsChildOf(m_User.Backpack))
            {
                sender.Mobile.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
                return;
            }

            switch (info.ButtonID)
            {
                case 1:

                    Item item = null;

                    foreach (Type type in m_Selected)
                    {
                        item = Loot.Construct(type);

                        if (item != null)
                        {
                            m_Token.Delete();
                            sender.Mobile.AddToBackpack(item);
                        }
                    }

                    break;
                case 0:
                    sender.Mobile.SendGump(new SpringDecorTokenGump(m_Token, m_User));
                    break;
            }
        }
    }
}
