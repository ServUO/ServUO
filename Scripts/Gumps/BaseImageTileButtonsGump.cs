using System;
using System.Collections;
using Server.Network;

namespace Server.Gumps
{
    public class ImageTileButtonInfo
    {
        private int m_ItemID;
        private int m_Hue;
        private int m_LocalizedTooltip;
        private TextDefinition m_Label;
        public ImageTileButtonInfo(int itemID, int hue, TextDefinition label, int localizedTooltip)
        {
            this.m_Hue = hue;
            this.m_ItemID = itemID;
            this.m_Label = label;
            this.m_LocalizedTooltip = localizedTooltip;
        }

        public ImageTileButtonInfo(int itemID, int hue, TextDefinition label)
            : this(itemID, hue, label, -1)
        {
        }

        public virtual int ItemID
        {
            get
            {
                return this.m_ItemID;
            }
            set
            {
                this.m_ItemID = value;
            }
        }
        public virtual int Hue
        {
            get
            {
                return this.m_Hue;
            }
            set
            {
                this.m_Hue = value;
            }
        }
        public virtual int LocalizedTooltip
        {
            get
            {
                return this.m_LocalizedTooltip;
            }
            set
            {
                this.m_LocalizedTooltip = value;
            }
        }
        public virtual TextDefinition Label
        {
            get
            {
                return this.m_Label;
            }
            set
            {
                this.m_Label = value;
            }
        }
    }

    public class BaseImageTileButtonsGump : Gump
    {
        private readonly ImageTileButtonInfo[] m_Buttons;
        public BaseImageTileButtonsGump(TextDefinition header, ArrayList buttons)
            : this(header, (ImageTileButtonInfo[])buttons.ToArray(typeof(ImageTileButtonInfo)))
        {
        }

        public BaseImageTileButtonsGump(TextDefinition header, ImageTileButtonInfo[] buttons)
            : base(10, 10)//Coords are 0, o on OSI, intentional difference
        {
            this.m_Buttons = buttons;
            this.AddPage(0);

            int x = this.XItems * 250;
            int y = this.YItems * 64;

            this.AddBackground(0, 0, x + 20, y + 84, 0x13BE);
            this.AddImageTiled(10, 10, x, 20, 0xA40);
            this.AddImageTiled(10, 40, x, y + 4, 0xA40);
            this.AddImageTiled(10, y + 54, x, 20, 0xA40);
            this.AddAlphaRegion(10, 10, x, y + 64);

            this.AddButton(10, y + 54, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0); //Cancel Button
            this.AddHtmlLocalized(45, y + 56, x - 50, 20, 1060051, 0x7FFF, false, false); // CANCEL
            TextDefinition.AddHtmlText(this, 14, 12, x, 20, header, false, false, 0x7FFF, 0xFFFFFF);

            this.AddPage(1);

            int itemsPerPage = this.XItems * this.YItems;

            for (int i = 0; i < buttons.Length; i++)
            {
                int position = i % itemsPerPage;

                int innerX = (position % this.XItems) * 250 + 14;
                int innerY = (position / this.XItems) * 64 + 44;

                int pageNum = i / itemsPerPage + 1;

                if (position == 0 && i != 0)
                {
                    this.AddButton(x - 100, y + 54, 0xFA5, 0xFA7, 0, GumpButtonType.Page, pageNum);
                    this.AddHtmlLocalized(x - 60, y + 56, 60, 20, 1043353, 0x7FFF, false, false); // Next

                    this.AddPage(pageNum);

                    this.AddButton(x - 200, y + 54, 0xFAE, 0xFB0, 0, GumpButtonType.Page, pageNum - 1);
                    this.AddHtmlLocalized(x - 160, y + 56, 60, 20, 1011393, 0x7FFF, false, false); // Back
                }

                ImageTileButtonInfo b = buttons[i];

                this.AddImageTiledButton(innerX, innerY, 0x918, 0x919, 100 + i, GumpButtonType.Reply, 0, b.ItemID, b.Hue, 15, 10, b.LocalizedTooltip);
                TextDefinition.AddHtmlText(this, innerX + 84, innerY, 250, 60, b.Label, false, false, 0x7FFF, 0xFFFFFF);
            }
        }

        protected ImageTileButtonInfo[] Buttons
        {
            get
            {
                return this.m_Buttons;
            }
        }
        protected virtual int XItems
        {
            get
            {
                return 2;
            }
        }
        protected virtual int YItems
        {
            get
            {
                return 5;
            }
        }
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int adjustedID = info.ButtonID - 100;

            if (adjustedID >= 0 && adjustedID < this.Buttons.Length)
                this.HandleButtonResponse(sender, adjustedID, this.Buttons[adjustedID]);
            else
                this.HandleCancel(sender);
        }

        public virtual void HandleButtonResponse(NetState sender, int adjustedButton, ImageTileButtonInfo buttonInfo)
        {
        }

        public virtual void HandleCancel(NetState sender)
        {
        }
    }
}