using System;
using System.Collections;
using System.Reflection;
using Server.Commands;
using Server.HuePickers;
using Server.Network;

namespace Server.Gumps
{
    public class SetGump : Gump
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
        private static readonly int TotalHeight = OffsetSize + (2 * (EntryHeight + OffsetSize));
        private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
        private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;
        private readonly PropertyInfo m_Property;
        private readonly Mobile m_Mobile;
        private readonly object m_Object;
        private readonly Stack m_Stack;
        private readonly int m_Page;
        private readonly ArrayList m_List;
        public SetGump(PropertyInfo prop, Mobile mobile, object o, Stack stack, int page, ArrayList list)
            : base(GumpOffsetX, GumpOffsetY)
        {
            this.m_Property = prop;
            this.m_Mobile = mobile;
            this.m_Object = o;
            this.m_Stack = stack;
            this.m_Page = page;
            this.m_List = list;

            bool canNull = !prop.PropertyType.IsValueType;
            bool canDye = prop.IsDefined(typeof(HueAttribute), false);
            bool isBody = prop.IsDefined(typeof(BodyAttribute), false);

            object val = prop.GetValue(this.m_Object, null);
            string initialText;

            if (val == null)
                initialText = "";
            else if (val is TextDefinition)
                initialText = ((TextDefinition)val).GetValue();
            else
                initialText = val.ToString();

            this.AddPage(0);

            this.AddBackground(0, 0, BackWidth, BackHeight + (canNull ? (EntryHeight + OffsetSize) : 0) + (canDye ? (EntryHeight + OffsetSize) : 0) + (isBody ? (EntryHeight + OffsetSize) : 0), BackGumpID);
            this.AddImageTiled(BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), TotalHeight + (canNull ? (EntryHeight + OffsetSize) : 0) + (canDye ? (EntryHeight + OffsetSize) : 0) + (isBody ? (EntryHeight + OffsetSize) : 0), OffsetGumpID);

            int x = BorderSize + OffsetSize;
            int y = BorderSize + OffsetSize;

            this.AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
            this.AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, prop.Name);
            x += EntryWidth + OffsetSize;

            if (SetGumpID != 0)
                this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

            x = BorderSize + OffsetSize;
            y += EntryHeight + OffsetSize;

            this.AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
            this.AddTextEntry(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, 0, initialText);
            x += EntryWidth + OffsetSize;

            if (SetGumpID != 0)
                this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

            this.AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 1, GumpButtonType.Reply, 0);

            if (canNull)
            {
                x = BorderSize + OffsetSize;
                y += EntryHeight + OffsetSize;

                this.AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
                this.AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "Null");
                x += EntryWidth + OffsetSize;

                if (SetGumpID != 0)
                    this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

                this.AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 2, GumpButtonType.Reply, 0);
            }

            if (canDye)
            {
                x = BorderSize + OffsetSize;
                y += EntryHeight + OffsetSize;

                this.AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
                this.AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "Hue Picker");
                x += EntryWidth + OffsetSize;

                if (SetGumpID != 0)
                    this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

                this.AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 3, GumpButtonType.Reply, 0);
            }

            if (isBody)
            {
                x = BorderSize + OffsetSize;
                y += EntryHeight + OffsetSize;

                this.AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
                this.AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "Body Picker");
                x += EntryWidth + OffsetSize;

                if (SetGumpID != 0)
                    this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

                this.AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 4, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            object toSet;
            bool shouldSet, shouldSend = true;

            switch ( info.ButtonID )
            {
                case 1:
                    {
                        TextRelay text = info.GetTextEntry(0);

                        if (text != null)
                        {
                            try
                            {
                                toSet = PropertiesGump.GetObjectFromString(this.m_Property.PropertyType, text.Text);
                                shouldSet = true;
                            }
                            catch
                            {
                                toSet = null;
                                shouldSet = false;
                                this.m_Mobile.SendMessage("Bad format");
                            }
                        }
                        else
                        {
                            toSet = null;
                            shouldSet = false;
                        }

                        break;
                    }
                case 2: // Null
                    {
                        toSet = null;
                        shouldSet = true;

                        break;
                    }
                case 3: // Hue Picker
                    {
                        toSet = null;
                        shouldSet = false;
                        shouldSend = false;

                        this.m_Mobile.SendHuePicker(new InternalPicker(this.m_Property, this.m_Mobile, this.m_Object, this.m_Stack, this.m_Page, this.m_List));

                        break;
                    }
                case 4: // Body Picker
                    {
                        toSet = null;
                        shouldSet = false;
                        shouldSend = false;

                        this.m_Mobile.SendGump(new SetBodyGump(this.m_Property, this.m_Mobile, this.m_Object, this.m_Stack, this.m_Page, this.m_List));

                        break;
                    }
                default:
                    {
                        toSet = null;
                        shouldSet = false;

                        break;
                    }
            }

            if (shouldSet)
            {
                try
                {
                    CommandLogging.LogChangeProperty(this.m_Mobile, this.m_Object, this.m_Property.Name, toSet == null ? "(null)" : toSet.ToString());
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

        private class InternalPicker : HuePicker
        {
            private readonly PropertyInfo m_Property;
            private readonly Mobile m_Mobile;
            private readonly object m_Object;
            private readonly Stack m_Stack;
            private readonly int m_Page;
            private readonly ArrayList m_List;
            public InternalPicker(PropertyInfo prop, Mobile mobile, object o, Stack stack, int page, ArrayList list)
                : base(((IHued)o).HuedItemID)
            {
                this.m_Property = prop;
                this.m_Mobile = mobile;
                this.m_Object = o;
                this.m_Stack = stack;
                this.m_Page = page;
                this.m_List = list;
            }

            public override void OnResponse(int hue)
            {
                try
                {
                    CommandLogging.LogChangeProperty(this.m_Mobile, this.m_Object, this.m_Property.Name, hue.ToString());
                    this.m_Property.SetValue(this.m_Object, hue, null);
                    PropertiesGump.OnValueChanged(this.m_Object, this.m_Property, this.m_Stack);
                }
                catch
                {
                    this.m_Mobile.SendMessage("An exception was caught. The property may not have changed.");
                }

                this.m_Mobile.SendGump(new PropertiesGump(this.m_Mobile, this.m_Object, this.m_Stack, this.m_List, this.m_Page));
            }
        }
    }
}