using System;
using Server.Commands;
using Server.Network;

namespace Server.Gumps
{
    public class GMToolbarGump : Gump
    {
        private const int LabelColor32 = 0xFFFFFF;
        private const int LabelHue    = 0x480;

        private const int GumpWidth   = 620;
        private const int Padding     = 10;
        private const int HeaderH     = 25;
        private const int CellW       = 100;
        private const int CellH       = 25;
        private const int Cols        = 6;
        private const int Rows        = 3;
        private const int GridStartY  = Padding + HeaderH + 5;

        private static readonly int ExpandedH  = GridStartY + (Rows * CellH) + Padding;
        private static readonly int CollapsedH = Padding + HeaderH + Padding;

        private readonly bool m_Collapsed;

        public static void Initialize()
        {
            CommandSystem.Register("GMToolbar", AccessLevel.GameMaster, GMToolbar_OnCommand);
        }

        [Usage("GMToolbar")]
        [Description("Opens the GM toolbar.")]
        private static void GMToolbar_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new GMToolbarGump(e.Mobile, false));
        }

        public GMToolbarGump(Mobile from, bool collapsed)
            : base(50, 50)
        {
            from.CloseGump(typeof(GMToolbarGump));

            m_Collapsed = collapsed;

            AddPage(0);

            int totalH = collapsed ? CollapsedH : ExpandedH;

            // Main background
            AddBackground(0, 0, GumpWidth, totalH, 5054);

            // Header bar
            AddImageTiled(Padding, Padding, GumpWidth - Padding * 2, HeaderH, 2624);
            AddAlphaRegion(Padding, Padding, GumpWidth - Padding * 2, HeaderH);

            // Title
            AddHtml(Padding + 5, Padding + 5, 200, 20,
                Color("GM TOOLBAR", LabelColor32), false, false);

            // Collapse / Expand toggle button
            int toggleX = GumpWidth - Padding - 60;
            AddButton(toggleX, Padding + 4, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtml(toggleX + 22, Padding + 5, 40, 20,
                Color(collapsed ? "[+]" : "[-]", LabelColor32), false, false);

            if (!collapsed)
                BuildGrid();
        }

        private void BuildGrid()
        {
            // Unified black alpha backing for all cells
            AddImageTiled(Padding, GridStartY, GumpWidth - Padding * 2, Rows * CellH, 2624);
            AddAlphaRegion(Padding, GridStartY, GumpWidth - Padding * 2, Rows * CellH);

            // Row 0 — Core tools
            AddCell(0, 0, 2,  "Admin");
            AddCell(1, 0, 3,  "Who");
            AddCell(2, 0, 4,  "Props");
            AddCell(3, 0, 5,  "Move");
            AddCell(4, 0, 6,  "Dupe");
            AddCell(5, 0, 7,  "DupeInBag");

            // Row 1 — Wipe + location
            AddCell(0, 1, 8,  "Wipe");
            AddCell(1, 1, 9,  "WipeItems");
            AddCell(2, 1, 10, "WipeNPCs");
            AddCell(3, 1, 11, "WipeMultis");
            AddCell(4, 1, 12, "Go");
            AddCell(5, 1, 13, "Where");

            // Row 2 — Teleport group
            AddCell(0, 2, 14, "Teleport");
            AddCell(1, 2, 15, "Stuck");
            AddCell(2, 2, 16, "GetFollowers");
            AddCell(3, 2, 17, "Bank");
            AddCell(4, 2, 18, "Save");
            AddCell(5, 2, 19, "BCast");
        }

        private void AddCell(int col, int row, int buttonID, string label)
        {
            int x = Padding + col * CellW;
            int y = GridStartY + row * CellH;

            AddButton(x + 3, y + 4, 4005, 4007, buttonID, GumpButtonType.Reply, 0);
            AddHtml(x + 25, y + 5, CellW - 28, 20,
                Color(label, LabelColor32), false, false);
        }

        private string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null || !from.Alive)
                return;

            string prefix = CommandSystem.Prefix;

            switch (info.ButtonID)
            {
                case 0: return; // Close

                case 1: // Toggle collapse/expand
                    from.SendGump(new GMToolbarGump(from, !m_Collapsed));
                    return;

                // Row 0 — Core tools
                case 2:  CommandSystem.Handle(from, prefix + "Admin");        break;
                case 3:  CommandSystem.Handle(from, prefix + "Who");          break;
                case 4:  CommandSystem.Handle(from, prefix + "Props");        break;
                case 5:  CommandSystem.Handle(from, prefix + "Move");         break;
                case 6:  CommandSystem.Handle(from, prefix + "Dupe");         break;
                case 7:  CommandSystem.Handle(from, prefix + "DupeInBag");    break;

                // Row 1 — Wipe + location
                case 8:  CommandSystem.Handle(from, prefix + "Wipe");         break;
                case 9:  CommandSystem.Handle(from, prefix + "WipeItems");    break;
                case 10: CommandSystem.Handle(from, prefix + "WipeNPCs");     break;
                case 11: CommandSystem.Handle(from, prefix + "WipeMultis");   break;
                case 12: CommandSystem.Handle(from, prefix + "Go");           break;
                case 13: CommandSystem.Handle(from, prefix + "Where");        break;

                // Row 2 — Teleport group
                case 14: CommandSystem.Handle(from, prefix + "Teleport");     break;
                case 15: CommandSystem.Handle(from, prefix + "Stuck");        break;
                case 16: CommandSystem.Handle(from, prefix + "GetFollowers"); break;
                case 17: CommandSystem.Handle(from, prefix + "Bank");         break;
                case 18: CommandSystem.Handle(from, prefix + "Save");         break;
                case 19: CommandSystem.Handle(from, prefix + "BCast");        break;
            }
        }
    }
}
