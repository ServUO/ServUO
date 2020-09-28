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

        public int CurrentPage => m_CurrentPage;
        public int CurrentX => m_CurrentX;
        public int CurrentY => m_CurrentY;
        public virtual int BorderSize => 10;
        public virtual int OffsetSize => 1;
        public virtual int EntryHeight => 20;
        public virtual int OffsetGumpID => 0x0A40;
        public virtual int HeaderGumpID => 0x0E14;
        public virtual int EntryGumpID => 0x0BBC;
        public virtual int BackGumpID => 0x13BE;
        public virtual int TextHue => 0;
        public virtual int TextOffsetX => 2;
        public string Center(string text)
        {
            return string.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return string.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
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
            if (m_Background != null)
                m_Background.Height = m_CurrentY + EntryHeight + OffsetSize + BorderSize;

            if (m_Offset != null)
                m_Offset.Height = m_CurrentY + EntryHeight + OffsetSize - BorderSize;
        }

        public void AddNewPage()
        {
            FinishPage();

            m_CurrentX = BorderSize + OffsetSize;
            m_CurrentY = BorderSize + OffsetSize;

            AddPage(++m_CurrentPage);

            m_Background = new GumpBackground(0, 0, 100, 100, BackGumpID);
            Add(m_Background);

            m_Offset = new GumpImageTiled(BorderSize, BorderSize, 100, 100, OffsetGumpID);
            Add(m_Offset);
        }

        public void AddNewLine()
        {
            m_CurrentY += EntryHeight + OffsetSize;
            m_CurrentX = BorderSize + OffsetSize;
        }

        public void IncreaseX(int width)
        {
            m_CurrentX += width + OffsetSize;

            width = m_CurrentX + BorderSize;

            if (m_Background != null && width > m_Background.Width)
                m_Background.Width = width;

            width = m_CurrentX - BorderSize;

            if (m_Offset != null && width > m_Offset.Width)
                m_Offset.Width = width;
        }

        public void AddEntryLabel(int width, string text)
        {
            AddImageTiled(m_CurrentX, m_CurrentY, width, EntryHeight, EntryGumpID);
            AddLabelCropped(m_CurrentX + TextOffsetX, m_CurrentY, width - TextOffsetX, EntryHeight, TextHue, text);

            IncreaseX(width);
        }

        public void AddEntryHtml(int width, string text)
        {
            AddImageTiled(m_CurrentX, m_CurrentY, width, EntryHeight, EntryGumpID);
            AddHtml(m_CurrentX + TextOffsetX, m_CurrentY, width - TextOffsetX, EntryHeight, text, false, false);

            IncreaseX(width);
        }

        public void AddEntryHeader(int width)
        {
            AddEntryHeader(width, 1);
        }

        public void AddEntryHeader(int width, int spannedEntries)
        {
            AddImageTiled(m_CurrentX, m_CurrentY, width, (EntryHeight * spannedEntries) + (OffsetSize * (spannedEntries - 1)), HeaderGumpID);
            IncreaseX(width);
        }

        public void AddBlankLine()
        {
            if (m_Offset != null)
                AddImageTiled(m_Offset.X, m_CurrentY, m_Offset.Width, EntryHeight, BackGumpID + 4);

            AddNewLine();
        }

        public void AddEntryButton(int width, int normalID, int pressedID, int buttonID, int buttonWidth, int buttonHeight)
        {
            AddEntryButton(width, normalID, pressedID, buttonID, buttonWidth, buttonHeight, 1);
        }

        public void AddEntryButton(int width, int normalID, int pressedID, int buttonID, int buttonWidth, int buttonHeight, int spannedEntries)
        {
            AddImageTiled(m_CurrentX, m_CurrentY, width, (EntryHeight * spannedEntries) + (OffsetSize * (spannedEntries - 1)), HeaderGumpID);
            AddButton(m_CurrentX + ((width - buttonWidth) / 2), m_CurrentY + (((EntryHeight * spannedEntries) + (OffsetSize * (spannedEntries - 1)) - buttonHeight) / 2), normalID, pressedID, buttonID, GumpButtonType.Reply, 0);

            IncreaseX(width);
        }

        public void AddEntryPageButton(int width, int normalID, int pressedID, int page, int buttonWidth, int buttonHeight)
        {
            AddImageTiled(m_CurrentX, m_CurrentY, width, EntryHeight, HeaderGumpID);
            AddButton(m_CurrentX + ((width - buttonWidth) / 2), m_CurrentY + ((EntryHeight - buttonHeight) / 2), normalID, pressedID, 0, GumpButtonType.Page, page);

            IncreaseX(width);
        }

        public void AddEntryText(int width, int entryID, string initialText)
        {
            AddImageTiled(m_CurrentX, m_CurrentY, width, EntryHeight, EntryGumpID);
            AddTextEntry(m_CurrentX + TextOffsetX, m_CurrentY, width - TextOffsetX, EntryHeight, TextHue, entryID, initialText);

            IncreaseX(width);
        }
    }
}