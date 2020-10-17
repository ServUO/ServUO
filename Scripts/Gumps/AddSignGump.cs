using Server.Commands;
using Server.Network;

namespace Server.Gumps
{
    public class AddSignGump : Gump
    {
        public static SignInfo[] m_Types = new SignInfo[]
        {
            new SignInfo(0xB95),
            new SignInfo(0xB96),
            new SignInfo(0xBA3),
            new SignInfo(0xBA4),
            new SignInfo(0xBA5),
            new SignInfo(0xBA6),
            new SignInfo(0xBA7),
            new SignInfo(0xBA8),
            new SignInfo(0xBA9),
            new SignInfo(0xBAA),
            new SignInfo(0xBAB),
            new SignInfo(0xBAC),
            new SignInfo(0xBAD),
            new SignInfo(0xBAE),
            new SignInfo(0xBAF),
            new SignInfo(0xBB0),
            new SignInfo(0xBB1),
            new SignInfo(0xBB2),
            new SignInfo(0xBB3),
            new SignInfo(0xBB4),
            new SignInfo(0xBB5),
            new SignInfo(0xBB6),
            new SignInfo(0xBB7),
            new SignInfo(0xBB8),
            new SignInfo(0xBB9),
            new SignInfo(0xBBA),
            new SignInfo(0xBBB),
            new SignInfo(0xBBC),
            new SignInfo(0xBBD),
            new SignInfo(0xBBE),
            new SignInfo(0xBBF),
            new SignInfo(0xBC0),
            new SignInfo(0xBC1),
            new SignInfo(0xBC2),
            new SignInfo(0xBC3),
            new SignInfo(0xBC4),
            new SignInfo(0xBC5),
            new SignInfo(0xBC6),
            new SignInfo(0xBC7),
            new SignInfo(0xBC8),
            new SignInfo(0xBC9),
            new SignInfo(0xBCA),
            new SignInfo(0xBCB),
            new SignInfo(0xBCC),
            new SignInfo(0xBCD),
            new SignInfo(0xBCE),
            new SignInfo(0xBCF),
            new SignInfo(0xBD0),
            new SignInfo(0xBD1),
            new SignInfo(0xBD2),
            new SignInfo(0xBD3),
            new SignInfo(0xBD4),
            new SignInfo(0xBD5),
            new SignInfo(0xBD6),
            new SignInfo(0xBD7),
            new SignInfo(0xBD8),
            new SignInfo(0xBD9),
            new SignInfo(0xBDA),
            new SignInfo(0xBDB),
            new SignInfo(0xBDC),
            new SignInfo(0xBDD),
            new SignInfo(0xBDE),
            new SignInfo(0xBDF),
            new SignInfo(0xBE0),
            new SignInfo(0xBE1),
            new SignInfo(0xBE2),
            new SignInfo(0xBE3),
            new SignInfo(0xBE4),
            new SignInfo(0xBE5),
            new SignInfo(0xBE6),
            new SignInfo(0xBE7),
            new SignInfo(0xBE8),
            new SignInfo(0xBE9),
            new SignInfo(0xBEA),
            new SignInfo(0xBEB),
            new SignInfo(0xBEC),
            new SignInfo(0xBED),
            new SignInfo(0xBEE),
            new SignInfo(0xBEF),
            new SignInfo(0xBF0),
            new SignInfo(0xBF1),
            new SignInfo(0xBF2),
            new SignInfo(0xBF3),
            new SignInfo(0xBF4),
            new SignInfo(0xBF5),
            new SignInfo(0xBF6),
            new SignInfo(0xBF7),
            new SignInfo(0xBF8),
            new SignInfo(0xBF9),
            new SignInfo(0xBFA),
            new SignInfo(0xBFB),
            new SignInfo(0xBFC),
            new SignInfo(0xBFD),
            new SignInfo(0xBFE),
            new SignInfo(0xBFF),
            new SignInfo(0xC00),
            new SignInfo(0xC01),
            new SignInfo(0xC02),
            new SignInfo(0xC03),
            new SignInfo(0xC04),
            new SignInfo(0xC05),
            new SignInfo(0xC06),
            new SignInfo(0xC07),
            new SignInfo(0xC08),
            new SignInfo(0xC09),
            new SignInfo(0xC0A),
            new SignInfo(0xC0B),
            new SignInfo(0xC0C),
            new SignInfo(0xC0D),
            new SignInfo(0xC0E),
            new SignInfo(0x1297),
            new SignInfo(0x1298),
            new SignInfo(0x1299),
            new SignInfo(0x129A),
            new SignInfo(0x129B),
            new SignInfo(0x129C),
            new SignInfo(0x129D),
            new SignInfo(0x129E),
            new SignInfo(0x1F28),
            new SignInfo(0x1F29),
            new SignInfo(0x4B20),
            new SignInfo(0x4B21),
            new SignInfo(0x9A0C),
            new SignInfo(0x9A0D),
            new SignInfo(0x9A0E),
            new SignInfo(0x9A0F),
            new SignInfo(0x9A10),
            new SignInfo(0x9A11),
            new SignInfo(0x9A12),
            new SignInfo(0x9A13)
        };

        public AddSignGump()
            : base(50, 40)
        {
            AddPage(0);

            AddBlueBack(570, 175);

            int pages = m_Types.Length / 20 + 1;
            for (int i = 0; i < m_Types.Length; ++i)
            {
                int page = i / 20 + 1;
                int xpos = (i / 2) % 10;
                int ypos = i % 2;

                if (xpos == 0 && ypos == 0)
                {
                    AddPage(page);
                    AddHtmlLocalized(30, 20, 60, 20, 1042971, string.Format("{0}", page), 0x7FFF, false, false); // #

                    AddHtmlLocalized(30, 45, 60, 20, 1043353, 0x7FFF, false, false); // Next
                    if (page < pages)
                        AddButton(30, 60, 0xFA5, 0xFA7, 0, GumpButtonType.Page, page + 1);
                    else
                        AddButton(30, 60, 0xFA5, 0xFA7, 0, GumpButtonType.Page, 1);

                    AddHtmlLocalized(30, 85, 60, 20, 1011393, 0x7FFF, false, false); // Back
                    if (page > 1)
                        AddButton(30, 100, 0xFAE, 0xFB0, 0, GumpButtonType.Page, page - 1);
                    else
                        AddButton(30, 100, 0xFAE, 0xFB0, 0, GumpButtonType.Page, pages);
                }

                if (m_Types[i].m_BaseID == 0)
                    continue;

                int x = (xpos + 1) * 50;
                int y = (ypos * 75);
                AddButton(30 + x, 20 + y, 0x2624, 0x2625, i + 1, GumpButtonType.Reply, m_Types[i].m_BaseID);
                AddItem(15 + x, 40 + y, m_Types[i].m_BaseID);
            }
        }

        public static void Initialize()
        {
            CommandSystem.Register("AddSign", AccessLevel.GameMaster, AddSign_OnCommand);
        }

        [Usage("AddSign")]
        [Description("Displays a menu from which you can interactively add signs.")]
        public static void AddSign_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new AddSignGump());
        }

        public void AddBlueBack(int width, int height)
        {
            AddBackground(0, 0, width - 00, height - 00, 0xE10);
            AddBackground(8, 5, width - 16, height - 11, 0x053);
            AddImageTiled(15, 14, width - 29, height - 29, 0xE14);
            AddAlphaRegion(15, 14, width - 29, height - 29);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            int button = info.ButtonID - 1;

            if (button < 0)
                return;

            CommandSystem.Handle(from, string.Format("{0}Add {1} {2}", CommandSystem.Prefix, " Sign ", m_Types[button].m_BaseID));
            from.SendGump(new AddSignGump());
        }
    }

    public class SignInfo
    {
        public int m_BaseID;
        public SignInfo(int baseID)
        {
            m_BaseID = baseID;
        }
    }
}
