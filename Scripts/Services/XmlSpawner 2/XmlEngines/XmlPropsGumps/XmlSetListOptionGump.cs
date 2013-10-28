using System;
using System.Collections;
using System.Reflection;
using Server.Commands;
using Server.Network;

namespace Server.Gumps
{
    public class XmlSetListOptionGump : Gump
    {
        public static readonly bool OldStyle = PropsConfig.OldStyle;
        public static readonly int GumpOffsetX = PropsConfig.GumpOffsetX;
        public static readonly int GumpOffsetY = PropsConfig.GumpOffsetY;
        public static readonly int TextHue = PropsConfig.TextHue;
        public static readonly int TextOffsetX = PropsConfig.TextOffsetX;
        public static readonly int OffsetGumpID = PropsConfig.OffsetGumpID;
        public static readonly int HeaderGumpID = PropsConfig.HeaderGumpID;
        public static readonly int EntryGumpID = PropsConfig.EntryGumpID;
        public static readonly int BackGumpID = PropsConfig.BackGumpID;
        public static readonly int SetGumpID = PropsConfig.SetGumpID;
        public static readonly int SetWidth = PropsConfig.SetWidth;
        public static readonly int SetOffsetX = PropsConfig.SetOffsetX, SetOffsetY = PropsConfig.SetOffsetY;
        public static readonly int SetButtonID1 = PropsConfig.SetButtonID1;
        public static readonly int SetButtonID2 = PropsConfig.SetButtonID2;
        public static readonly int PrevWidth = PropsConfig.PrevWidth;
        public static readonly int PrevOffsetX = PropsConfig.PrevOffsetX, PrevOffsetY = PropsConfig.PrevOffsetY;
        public static readonly int PrevButtonID1 = PropsConfig.PrevButtonID1;
        public static readonly int PrevButtonID2 = PropsConfig.PrevButtonID2;
        public static readonly int NextWidth = PropsConfig.NextWidth;
        public static readonly int NextOffsetX = PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY;
        public static readonly int NextButtonID1 = PropsConfig.NextButtonID1;
        public static readonly int NextButtonID2 = PropsConfig.NextButtonID2;
        public static readonly int OffsetSize = PropsConfig.OffsetSize;
        public static readonly int EntryHeight = PropsConfig.EntryHeight;
        public static readonly int BorderSize = PropsConfig.BorderSize;
        protected PropertyInfo m_Property;
        protected Mobile m_Mobile;
        protected object m_Object;
        protected Stack m_Stack;
        protected int m_Page;
        protected ArrayList m_List;
        protected object[] m_Values;
        private static readonly int EntryWidth = 212;
        private static readonly int EntryCount = 13;
        private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
        private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
        private static readonly bool PrevLabel = OldStyle;
        private static readonly bool NextLabel = OldStyle;
        private static readonly int PrevLabelOffsetX = PrevWidth + 1;
        private static readonly int PrevLabelOffsetY = 0;
        private static readonly int NextLabelOffsetX = -29;
        private static readonly int NextLabelOffsetY = 0;
        public XmlSetListOptionGump(PropertyInfo prop, Mobile mobile, object o, Stack stack, int propspage, ArrayList list, string[] names, object[] values)
            : base(GumpOffsetX, GumpOffsetY)
        {
            this.m_Property = prop;
            this.m_Mobile = mobile;
            this.m_Object = o;
            this.m_Stack = stack;
            this.m_Page = propspage;
            this.m_List = list;

            this.m_Values = values;

            int pages = (names.Length + EntryCount - 1) / EntryCount;
            int index = 0;

            for (int page = 1; page <= pages; ++page)
            {
                this.AddPage(page);

                int start = (page - 1) * EntryCount;
                int count = names.Length - start;

                if (count > EntryCount)
                    count = EntryCount;

                int totalHeight = OffsetSize + ((count + 2) * (EntryHeight + OffsetSize));
                int backHeight = BorderSize + totalHeight + BorderSize;

                this.AddBackground(0, 0, BackWidth, backHeight, BackGumpID);
                this.AddImageTiled(BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), totalHeight, OffsetGumpID);

                int x = BorderSize + OffsetSize;
                int y = BorderSize + OffsetSize;

                int emptyWidth = TotalWidth - PrevWidth - NextWidth - (OffsetSize * 4) - (OldStyle ? SetWidth + OffsetSize : 0);

                this.AddImageTiled(x, y, PrevWidth, EntryHeight, HeaderGumpID);

                if (page > 1)
                {
                    this.AddButton(x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 0, GumpButtonType.Page, page - 1);

                    if (PrevLabel)
                        this.AddLabel(x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous");
                }

                x += PrevWidth + OffsetSize;

                if (!OldStyle)
                    this.AddImageTiled(x - (OldStyle ? OffsetSize : 0), y, emptyWidth + (OldStyle ? OffsetSize * 2 : 0), EntryHeight, HeaderGumpID);

                x += emptyWidth + OffsetSize;

                if (!OldStyle)
                    this.AddImageTiled(x, y, NextWidth, EntryHeight, HeaderGumpID);

                if (page < pages)
                {
                    this.AddButton(x + NextOffsetX, y + NextOffsetY, NextButtonID1, NextButtonID2, 0, GumpButtonType.Page, page + 1);

                    if (NextLabel)
                        this.AddLabel(x + NextLabelOffsetX, y + NextLabelOffsetY, TextHue, "Next");
                }

                this.AddRect(0, prop.Name, 0);

                for (int i = 0; i < count; ++i)
                    this.AddRect(i + 1, names[index], ++index);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int index = info.ButtonID - 1;

            if (index >= 0 && index < this.m_Values.Length)
            {
                try
                {
                    object toSet = this.m_Values[index];
                    CommandLogging.LogChangeProperty(this.m_Mobile, this.m_Object, this.m_Property.Name, (toSet == null ? "(-null-)" : toSet.ToString()));
                    this.m_Property.SetValue(this.m_Object, toSet, null);
                }
                catch
                {
                    this.m_Mobile.SendMessage("An exception was caught. The property may not have changed.");
                }
            }

            this.m_Mobile.SendGump(new XmlPropertiesGump(this.m_Mobile, this.m_Object, this.m_Stack, this.m_List, this.m_Page));
        }

        private void AddRect(int index, string str, int button)
        {
            int x = BorderSize + OffsetSize;
            int y = BorderSize + OffsetSize + ((index + 1) * (EntryHeight + OffsetSize));

            this.AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
            this.AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, str);

            x += EntryWidth + OffsetSize;

            if (SetGumpID != 0)
                this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

            if (button != 0)
                this.AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, button, GumpButtonType.Reply, 0);
        }
    }
}