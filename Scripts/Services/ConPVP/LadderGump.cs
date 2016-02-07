using System;
using System.Collections;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.ConPVP
{
    public class LadderItem : Item
    {
        private LadderController m_Ladder;
        [Constructable]
        public LadderItem()
            : base(0x117F)
        {
            this.Movable = false;
        }

        public LadderItem(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public LadderController Ladder
        {
            get
            {
                return this.m_Ladder;
            }
            set
            {
                this.m_Ladder = value;
            }
        }
        public override string DefaultName
        {
            get
            {
                return "1v1 leaderboard";
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);

            writer.Write((Item)this.m_Ladder);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Ladder = reader.ReadItem() as LadderController;
                        break;
                    }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 2))
            {
                Ladder ladder = Server.Engines.ConPVP.Ladder.Instance;

                if (this.m_Ladder != null)
                    ladder = this.m_Ladder.Ladder;

                if (ladder != null)
                {
                    from.CloseGump(typeof(LadderGump));
                    from.SendGump(new LadderGump(ladder, 0));
                }
            }
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that
        }
    }

    public class LadderGump : Gump
    {
        private readonly Ladder m_Ladder;
        private readonly int m_Page;
        private readonly ArrayList m_List;
        private int m_ColumnX = 12;
        public LadderGump(Ladder ladder)
            : this(ladder, 0)
        {
        }

        public LadderGump(Ladder ladder, int page)
            : base(50, 50)
        {
            this.m_Ladder = ladder;
            this.m_Page = page;

            this.AddPage(0);

            ArrayList list = ladder.ToArrayList();
            this.m_List = list;

            int lc = Math.Min(list.Count, 150);

            int start = page * 15;
            int end = start + 15;

            if (end > lc)
                end = lc;

            int ct = end - start;

            int height = 12 + 20 + (ct * 20) + 23 + 12;

            this.AddBackground(0, 0, 499, height, 0x2436);

            for (int i = start + 1; i < end; i += 2)
                this.AddImageTiled(12, 32 + ((i - start) * 20), 475, 20, 0x2430);

            this.AddAlphaRegion(10, 10, 479, height - 20);

            if (page > 0)
                this.AddButton(446, height - 12 - 2 - 16, 0x15E3, 0x15E7, 1, GumpButtonType.Reply, 0);
            else
                this.AddImage(446, height - 12 - 2 - 16, 0x2626);

            if (((page + 1) * 15) < lc)
                this.AddButton(466, height - 12 - 2 - 16, 0x15E1, 0x15E5, 2, GumpButtonType.Reply, 0);
            else
                this.AddImage(466, height - 12 - 2 - 16, 0x2622);

            this.AddHtml(16, height - 12 - 2 - 18, 400, 20, this.Color(String.Format("Top {3} of {0:N0} duelists, page {1} of {2}", list.Count, page + 1, (lc + 14) / 15, lc), 0xFFC000), false, false);

            this.AddColumnHeader(75, "Rank");
            this.AddColumnHeader(115, "Level");
            this.AddColumnHeader(50, "Guild");
            this.AddColumnHeader(115, "Name");
            this.AddColumnHeader(60, "Wins");
            this.AddColumnHeader(60, "Losses");

            for (int i = start; i < end && i < lc; ++i)
            {
                LadderEntry entry = (LadderEntry)list[i];

                int y = 32 + ((i - start) * 20);
                int x = 12;

                this.AddBorderedText(x, y, 75, this.Center(Rank(i + 1)), 0xFFFFFF, 0);
                x += 75;

                /*AddImage( 20, y + 5, 0x2616, 0x96C );
                AddImage( 22, y + 5, 0x2616, 0x96C );
                AddImage( 20, y + 7, 0x2616, 0x96C );
                AddImage( 22, y + 7, 0x2616, 0x96C );

                AddImage( 21, y + 6, 0x2616, 0x454 );*/

                this.AddImage(x + 3, y + 4, 0x805);

                int xp = entry.Experience;
                int level = Ladder.GetLevel(xp);

                int xpBase, xpAdvance;
                Ladder.GetLevelInfo(level, out xpBase, out xpAdvance);

                int width;

                int xpOffset = xp - xpBase;

                if (xpOffset >= xpAdvance)
                    width = 109; // level 50
                else
                    width = ((109 * xpOffset) + (xpAdvance / 2)) / (xpAdvance - 1);

                //AddImageTiled( 21, y + 6, width, 8, 0x2617 );
                this.AddImageTiled(x + 3, y + 4, width, 11, 0x806);
                this.AddBorderedText(x, y, 115, this.Center(level.ToString()), 0xFFFFFF, 0);
                x += 115;

                Mobile mob = entry.Mobile;

                if (mob.Guild != null)
                    this.AddBorderedText(x, y, 50, this.Center(mob.Guild.Abbreviation), 0xFFFFFF, 0);

                x += 50;

                this.AddBorderedText(x + 5, y, 115 - 5, (mob.Name), 0xFFFFFF, 0);
                x += 115;

                this.AddBorderedText(x, y, 60, this.Center(entry.Wins.ToString()), 0xFFFFFF, 0);
                x += 60;

                this.AddBorderedText(x, y, 60, this.Center(entry.Losses.ToString()), 0xFFFFFF, 0);
                x += 60;
                //AddBorderedText( 292 + 15, y, 115 - 30, String.Format( "{0} <DIV ALIGN=CENTER>/</DIV> <DIV ALIGN=RIGHT>{1}</DIV>", entry.Wins, entry.Losses ), 0xFFC000, 0 );
            }
        }

        public static string Rank(int num)
        {
            string numStr = num.ToString("N0");

            if ((num % 100) > 10 && (num % 100) < 20)
                return numStr + "th";

            switch ( num % 10 )
            {
                case 1:
                    return numStr + "st";
                case 2:
                    return numStr + "nd";
                case 3:
                    return numStr + "rd";
                default:
                    return numStr + "th";
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 1 && this.m_Page > 0)
                from.SendGump(new LadderGump(this.m_Ladder, this.m_Page - 1));
            else if (info.ButtonID == 2 && ((this.m_Page + 1) * 15) < Math.Min(this.m_List.Count, 150))
                from.SendGump(new LadderGump(this.m_Ladder, this.m_Page + 1));
        }

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        private void AddBorderedText(int x, int y, int width, string text, int color, int borderColor)
        {
            /*AddColoredText( x - 1, y, width, text, borderColor );
            AddColoredText( x + 1, y, width, text, borderColor );
            AddColoredText( x, y - 1, width, text, borderColor );
            AddColoredText( x, y + 1, width, text, borderColor );*/
            /*AddColoredText( x - 1, y - 1, width, text, borderColor );
            AddColoredText( x + 1, y + 1, width, text, borderColor );*/
            this.AddColoredText(x, y, width, text, color);
        }

        private void AddColoredText(int x, int y, int width, string text, int color)
        {
            if (color == 0)
                this.AddHtml(x, y, width, 20, text, false, false);
            else
                this.AddHtml(x, y, width, 20, this.Color(text, color), false, false);
        }

        private void AddColumnHeader(int width, string name)
        {
            this.AddBackground(this.m_ColumnX, 12, width, 20, 0x242C);
            this.AddImageTiled(this.m_ColumnX + 2, 14, width - 4, 16, 0x2430);
            this.AddBorderedText(this.m_ColumnX, 13, width, this.Center(name), 0xFFFFFF, 0);

            this.m_ColumnX += width;
        }
    }
}