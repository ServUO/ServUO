using System;
using System.Collections;
using System.Reflection;
using Server.Commands;
using Server.Network;

namespace Server.Gumps
{
    public class SetTimeSpanGump : Gump
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
        private static readonly int EntryWidth = 212;
        private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
        private static readonly int TotalHeight = OffsetSize + (7 * (EntryHeight + OffsetSize));
        private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
        private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;
        private readonly PropertyInfo m_Property;
        private readonly Mobile m_Mobile;
        private readonly object m_Object;
        private readonly Stack m_Stack;
        private readonly int m_Page;
        private readonly ArrayList m_List;
        public SetTimeSpanGump(PropertyInfo prop, Mobile mobile, object o, Stack stack, int page, ArrayList list)
            : base(GumpOffsetX, GumpOffsetY)
        {
            this.m_Property = prop;
            this.m_Mobile = mobile;
            this.m_Object = o;
            this.m_Stack = stack;
            this.m_Page = page;
            this.m_List = list;

            TimeSpan ts = (TimeSpan)prop.GetValue(o, null);

            this.AddPage(0);

            this.AddBackground(0, 0, BackWidth, BackHeight, BackGumpID);
            this.AddImageTiled(BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), TotalHeight, OffsetGumpID);

            this.AddRect(0, prop.Name, 0, -1);
            this.AddRect(1, ts.ToString(), 0, -1);
            this.AddRect(2, "Zero", 1, -1);
            this.AddRect(3, "From H:M:S", 2, -1);
            this.AddRect(4, "H:", 3, 0);
            this.AddRect(5, "M:", 4, 1);
            this.AddRect(6, "S:", 5, 2);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            TimeSpan toSet;
            bool shouldSet, shouldSend;

            TextRelay h = info.GetTextEntry(0);
            TextRelay m = info.GetTextEntry(1);
            TextRelay s = info.GetTextEntry(2);

            switch ( info.ButtonID )
            {
                case 1: // Zero
                    {
                        toSet = TimeSpan.Zero;
                        shouldSet = true;
                        shouldSend = true;

                        break;
                    }
                case 2: // From H:M:S
                    {
                        bool successfulParse = false;
                        if (h != null && m != null && s != null)
                        {
                            successfulParse = TimeSpan.TryParse(h.Text + ":" + m.Text + ":" + s.Text, out toSet);
                        }
                        else
                        {
                            toSet = TimeSpan.Zero;
                        }

                        shouldSet = shouldSend = successfulParse;

                        break;
                    }
                case 3: // From H
                    {
                        if (h != null)
                        {
                            try
                            {
                                toSet = TimeSpan.FromHours(Utility.ToDouble(h.Text));
                                shouldSet = true;
                                shouldSend = true;

                                break;
                            }
                            catch
                            {
                            }
                        }

                        toSet = TimeSpan.Zero;
                        shouldSet = false;
                        shouldSend = false;

                        break;
                    }
                case 4: // From M
                    {
                        if (m != null)
                        {
                            try
                            {
                                toSet = TimeSpan.FromMinutes(Utility.ToDouble(m.Text));
                                shouldSet = true;
                                shouldSend = true;

                                break;
                            }
                            catch
                            {
                            }
                        }

                        toSet = TimeSpan.Zero;
                        shouldSet = false;
                        shouldSend = false;

                        break;
                    }
                case 5: // From S
                    {
                        if (s != null)
                        {
                            try
                            {
                                toSet = TimeSpan.FromSeconds(Utility.ToDouble(s.Text));
                                shouldSet = true;
                                shouldSend = true;

                                break;
                            }
                            catch
                            {
                            }
                        }

                        toSet = TimeSpan.Zero;
                        shouldSet = false;
                        shouldSend = false;

                        break;
                    }
                default:
                    {
                        toSet = TimeSpan.Zero;
                        shouldSet = false;
                        shouldSend = true;

                        break;
                    }
            }

            if (shouldSet)
            {
                try
                {
                    CommandLogging.LogChangeProperty(this.m_Mobile, this.m_Object, this.m_Property.Name, toSet.ToString());
                    this.m_Property.SetValue(this.m_Object, toSet, null);
                    PropertiesGump.OnValueChanged(this.m_Object, this.m_Property, this.m_Stack);
                }
                catch
                {
                    this.m_Mobile.SendMessage("An exception was caught. The property may not have changed.");
                }
            }

            if (shouldSend)
                this.m_Mobile.SendGump(new PropertiesGump(this.m_Mobile, this.m_Object, this.m_Stack, this.m_List, this.m_Page));
        }

        private void AddRect(int index, string str, int button, int text)
        {
            int x = BorderSize + OffsetSize;
            int y = BorderSize + OffsetSize + (index * (EntryHeight + OffsetSize));

            this.AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
            this.AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, str);

            if (text != -1)
                this.AddTextEntry(x + 16 + TextOffsetX, y, EntryWidth - TextOffsetX - 16, EntryHeight, TextHue, text, "");

            x += EntryWidth + OffsetSize;

            if (SetGumpID != 0)
                this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

            if (button != 0)
                this.AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, button, GumpButtonType.Reply, 0);
        }
    }
}