using System;

namespace Server.Gumps
{
    public abstract class BaseGridGump : Gump
    {
        public const int ArrowLeftID1 = 0x15E3;
        public const int ArrowLeftID2 = 0x15E7;
        public const int ArrowLeftWidth = 16;
        public const int ArrowLeftHeight = 16;
        public const int ArrowRightID1 = 0x15E1;
        public const int ArrowRightID2 = 0x15E5;
        public const int ArrowRightWidth = 16;
        public const int ArrowRightHeight = 16;
        protected GumpBackground m_Background;
        protected GumpImageTiled m_Offset;
        private int m_CurrentX, m_CurrentY;
        private int m_CurrentPage;
        public BaseGridGump(int x, int y)
            : base(x, y)
        {
        }

        public int CurrentPage
        {
            get
            {
                return this.m_CurrentPage;
            }
        }
        public int CurrentX
        {
            get
            {
                return this.m_CurrentX;
            }
        }
        public int CurrentY
        {
            get
            {
                return this.m_CurrentY;
            }
        }
        public virtual int BorderSize
        {
            get
            {
                return 10;
            }
        }
        public virtual int OffsetSize
        {
            get
            {
                return 1;
            }
        }
        public virtual int EntryHeight
        {
            get
            {
                return 20;
            }
        }
        public virtual int OffsetGumpID
        {
            get
            {
                return 0x0A40;
            }
        }
        public virtual int HeaderGumpID
        {
            get
            {
                return 0x0E14;
            }
        }
        public virtual int EntryGumpID
        {
            get
            {
                return 0x0BBC;
            }
        }
        public virtual int BackGumpID
        {
            get
            {
                return 0x13BE;
            }
        }
        public virtual int TextHue
        {
            get
            {
                return 0;
            }
        }
        public virtual int TextOffsetX
        {
            get
            {
                return 2;
            }
        }
        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        public int GetButtonID(int typeCount, int type, int index)
        {
            return 1 + (index * typeCount) + type;
        }

        public bool SplitButtonID(int buttonID, int typeCount, out int type, out int index)
        {
            if (buttonID < 1)
            {
                type = 0;
                index = 0;
                return false;
            }

            buttonID -= 1;

            type = buttonID % typeCount;
            index = buttonID / typeCount;

            return true;
        }

        public void FinishPage()
        {
            if (this.m_Background != null)
                this.m_Background.Height = this.m_CurrentY + this.EntryHeight + this.OffsetSize + this.BorderSize;

            if (this.m_Offset != null)
                this.m_Offset.Height = this.m_CurrentY + this.EntryHeight + this.OffsetSize - this.BorderSize;
        }

        public void AddNewPage()
        {
            this.FinishPage();

            this.m_CurrentX = this.BorderSize + this.OffsetSize;
            this.m_CurrentY = this.BorderSize + this.OffsetSize;

            this.AddPage(++this.m_CurrentPage);

            this.m_Background = new GumpBackground(0, 0, 100, 100, this.BackGumpID);
            this.Add(this.m_Background);

            this.m_Offset = new GumpImageTiled(this.BorderSize, this.BorderSize, 100, 100, this.OffsetGumpID);
            this.Add(this.m_Offset);
        }

        public void AddNewLine()
        {
            this.m_CurrentY += this.EntryHeight + this.OffsetSize;
            this.m_CurrentX = this.BorderSize + this.OffsetSize;
        }

        public void IncreaseX(int width)
        {
            this.m_CurrentX += width + this.OffsetSize;

            width = this.m_CurrentX + this.BorderSize;

            if (this.m_Background != null && width > this.m_Background.Width)
                this.m_Background.Width = width;

            width = this.m_CurrentX - this.BorderSize;

            if (this.m_Offset != null && width > this.m_Offset.Width)
                this.m_Offset.Width = width;
        }

        public void AddEntryLabel(int width, string text)
        {
            this.AddImageTiled(this.m_CurrentX, this.m_CurrentY, width, this.EntryHeight, this.EntryGumpID);
            this.AddLabelCropped(this.m_CurrentX + this.TextOffsetX, this.m_CurrentY, width - this.TextOffsetX, this.EntryHeight, this.TextHue, text);

            this.IncreaseX(width);
        }

        public void AddEntryHtml(int width, string text)
        {
            this.AddImageTiled(this.m_CurrentX, this.m_CurrentY, width, this.EntryHeight, this.EntryGumpID);
            this.AddHtml(this.m_CurrentX + this.TextOffsetX, this.m_CurrentY, width - this.TextOffsetX, this.EntryHeight, text, false, false);

            this.IncreaseX(width);
        }

        public void AddEntryHeader(int width)
        {
            this.AddEntryHeader(width, 1);
        }

        public void AddEntryHeader(int width, int spannedEntries)
        {
            this.AddImageTiled(this.m_CurrentX, this.m_CurrentY, width, (this.EntryHeight * spannedEntries) + (this.OffsetSize * (spannedEntries - 1)), this.HeaderGumpID);
            this.IncreaseX(width);
        }

        public void AddBlankLine()
        {
            if (this.m_Offset != null)
                this.AddImageTiled(this.m_Offset.X, this.m_CurrentY, this.m_Offset.Width, this.EntryHeight, this.BackGumpID + 4);

            this.AddNewLine();
        }

        public void AddEntryButton(int width, int normalID, int pressedID, int buttonID, int buttonWidth, int buttonHeight)
        {
            this.AddEntryButton(width, normalID, pressedID, buttonID, buttonWidth, buttonHeight, 1);
        }

        public void AddEntryButton(int width, int normalID, int pressedID, int buttonID, int buttonWidth, int buttonHeight, int spannedEntries)
        {
            this.AddImageTiled(this.m_CurrentX, this.m_CurrentY, width, (this.EntryHeight * spannedEntries) + (this.OffsetSize * (spannedEntries - 1)), this.HeaderGumpID);
            this.AddButton(this.m_CurrentX + ((width - buttonWidth) / 2), this.m_CurrentY + (((this.EntryHeight * spannedEntries) + (this.OffsetSize * (spannedEntries - 1)) - buttonHeight) / 2), normalID, pressedID, buttonID, GumpButtonType.Reply, 0);

            this.IncreaseX(width);
        }

        public void AddEntryPageButton(int width, int normalID, int pressedID, int page, int buttonWidth, int buttonHeight)
        {
            this.AddImageTiled(this.m_CurrentX, this.m_CurrentY, width, this.EntryHeight, this.HeaderGumpID);
            this.AddButton(this.m_CurrentX + ((width - buttonWidth) / 2), this.m_CurrentY + ((this.EntryHeight - buttonHeight) / 2), normalID, pressedID, 0, GumpButtonType.Page, page);

            this.IncreaseX(width);
        }

        public void AddEntryText(int width, int entryID, string initialText)
        {
            this.AddImageTiled(this.m_CurrentX, this.m_CurrentY, width, this.EntryHeight, this.EntryGumpID);
            this.AddTextEntry(this.m_CurrentX + this.TextOffsetX, this.m_CurrentY, width - this.TextOffsetX, this.EntryHeight, this.TextHue, entryID, initialText);

            this.IncreaseX(width);
        }
    }
}