using System;
using Server.Commands;
using Server.Network;

namespace Server.Gumps
{
    public class GMToolbarGump : Gump
    {
        private const int LabelColor32    = 0xFFFFFF;
        private const int DisabledColor32 = 0x808080;
        private const int LabelHue        = 0x480;

        private const int GumpWidth    = 620;
        private const int CollapsedW   = 175;
        private const int Padding      = 10;
        private const int HeaderH      = 25;
        private const int CellW        = 100;
        private const int CellH        = 25;
        private const int Cols         = 6;
        private const int Rows         = 3;
        private const int GridStartY   = Padding + HeaderH + 5;

        private static readonly int ExpandedH  = GridStartY + (Rows * CellH) + Padding;
        private static readonly int CollapsedH = Padding + HeaderH + Padding;

        private readonly bool   m_Collapsed;
        private readonly Mobile m_From;

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

            m_From      = from;
            m_Collapsed = collapsed;

            AddPage(0);

            int totalW = collapsed ? CollapsedW : GumpWidth;
            int totalH = collapsed ? CollapsedH : ExpandedH;

            // Main background
            AddBackground(0, 0, totalW, totalH, 5054);

            // Header bar
            AddImageTiled(Padding, Padding, totalW - Padding * 2, HeaderH, 2624);
            AddAlphaRegion(Padding, Padding, totalW - Padding * 2, HeaderH);

            // Title
            AddHtml(Padding + 5, Padding + 5, 95, 20,
                Color("GM TOOLBAR", LabelColor32), false, false);

            // Collapse / Expand toggle button — hug title when collapsed, right-align when expanded
            int toggleX = collapsed ? (Padding + 5 + 95 + 5) : (GumpWidth - Padding - 55);
            AddButton(toggleX, Padding + 4, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtml(toggleX + 35, Padding + 5, 35, 20,
                Color(collapsed ? "[+]" : "[-]", LabelColor32), false, false);

            if (!collapsed)
                BuildGrid(from);
        }

        private void BuildGrid(Mobile from)
        {
            // Unified black alpha backing for all cells
            AddImageTiled(Padding, GridStartY, GumpWidth - Padding * 2, Rows * CellH, 2624);
            AddAlphaRegion(Padding, GridStartY, GumpWidth - Padding * 2, Rows * CellH);

            // Row 0 — Core tools
            AddCell(from, 0, 0, 2,  "Admin",        AccessLevel.Administrator);
            AddCell(from, 1, 0, 3,  "Who",          AccessLevel.Counselor);
            AddCell(from, 2, 0, 4,  "Props",        AccessLevel.Counselor);
            AddCell(from, 3, 0, 5,  "Move",         AccessLevel.GameMaster);
            AddCell(from, 4, 0, 6,  "Dupe",         AccessLevel.GameMaster);
            AddCell(from, 5, 0, 7,  "DupeInBag",    AccessLevel.GameMaster);

            // Row 1 — Wipe + location
            AddCell(from, 0, 1, 8,  "Wipe",         AccessLevel.GameMaster);
            AddCell(from, 1, 1, 9,  "WipeItems",    AccessLevel.GameMaster);
            AddCell(from, 2, 1, 10, "WipeNPCs",     AccessLevel.GameMaster);
            AddCell(from, 3, 1, 11, "WipeMultis",   AccessLevel.GameMaster);
            AddCell(from, 4, 1, 12, "Go",           AccessLevel.Counselor);
            AddCell(from, 5, 1, 13, "Where",        AccessLevel.Counselor);

            // Row 2 — Teleport group
            AddCell(from, 0, 2, 14, "Teleport",     AccessLevel.Counselor);
            AddCell(from, 1, 2, 15, "Stuck",        AccessLevel.Counselor);
            AddCell(from, 2, 2, 16, "GetFollowers", AccessLevel.GameMaster);
            AddCell(from, 3, 2, 17, "Bank",         AccessLevel.GameMaster);
            AddCell(from, 4, 2, 18, "Save",         AccessLevel.Administrator);
            AddCell(from, 5, 2, 19, "Kill",         AccessLevel.GameMaster);
        }

        private void AddCell(Mobile from, int col, int row, int buttonID, string label, AccessLevel required)
        {
            int x = Padding + col * CellW;
            int y = GridStartY + row * CellH;

            if (from.AccessLevel >= required)
            {
                AddButton(x + 3, y + 4, 4005, 4007, buttonID, GumpButtonType.Reply, 0);
                AddHtml(x + 38, y + 5, CellW - 41, 20,
                    Color(label, LabelColor32), false, false);
            }
            else
            {
                // Render label greyed out with no button
                AddHtml(x + 38, y + 5, CellW - 41, 20,
                    Color(label, DisabledColor32), false, false);
            }
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
                case 2:  if (from.AccessLevel >= AccessLevel.Administrator) CommandSystem.Handle(from, prefix + "Admin");        break;
                case 3:  if (from.AccessLevel >= AccessLevel.Counselor)     CommandSystem.Handle(from, prefix + "Who");          break;
                case 4:  if (from.AccessLevel >= AccessLevel.Counselor)     CommandSystem.Handle(from, prefix + "Props");        break;
                case 5:  if (from.AccessLevel >= AccessLevel.GameMaster)    CommandSystem.Handle(from, prefix + "Move");         break;
                case 6:  if (from.AccessLevel >= AccessLevel.GameMaster)    CommandSystem.Handle(from, prefix + "Dupe");         break;
                case 7:  if (from.AccessLevel >= AccessLevel.GameMaster)    CommandSystem.Handle(from, prefix + "DupeInBag");    break;

                // Row 1 — Wipe + location
                case 8:  if (from.AccessLevel >= AccessLevel.GameMaster)    CommandSystem.Handle(from, prefix + "Wipe");         break;
                case 9:  if (from.AccessLevel >= AccessLevel.GameMaster)    CommandSystem.Handle(from, prefix + "WipeItems");    break;
                case 10: if (from.AccessLevel >= AccessLevel.GameMaster)    CommandSystem.Handle(from, prefix + "WipeNPCs");     break;
                case 11: if (from.AccessLevel >= AccessLevel.GameMaster)    CommandSystem.Handle(from, prefix + "WipeMultis");   break;
                case 12: if (from.AccessLevel >= AccessLevel.Counselor)     CommandSystem.Handle(from, prefix + "Go");           break;
                case 13: if (from.AccessLevel >= AccessLevel.Counselor)     CommandSystem.Handle(from, prefix + "Where");        break;

                // Row 2 — Teleport group
                case 14: if (from.AccessLevel >= AccessLevel.Counselor)     CommandSystem.Handle(from, prefix + "Multi Tele");   break;
                case 15: if (from.AccessLevel >= AccessLevel.Counselor)     CommandSystem.Handle(from, prefix + "Stuck");        break;
                case 16: if (from.AccessLevel >= AccessLevel.GameMaster)    CommandSystem.Handle(from, prefix + "GetFollowers"); break;
                case 17: if (from.AccessLevel >= AccessLevel.GameMaster)    CommandSystem.Handle(from, prefix + "Bank");         break;
                case 18: if (from.AccessLevel >= AccessLevel.Administrator) CommandSystem.Handle(from, prefix + "Save");         break;
                case 19: if (from.AccessLevel >= AccessLevel.GameMaster)    CommandSystem.Handle(from, prefix + "Kill");         break;
            }

            from.SendGump(new GMToolbarGump(from, m_Collapsed));
        }
    }
}
