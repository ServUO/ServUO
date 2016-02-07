using System;
using System.Collections;
using System.Reflection;
using Server.Commands;
using Server.Commands.Generic;
using Server.Network;
using Server.Prompts;

namespace Server.Gumps
{
    public class SetObjectGump : Gump
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
        private static readonly int TotalHeight = OffsetSize + (5 * (EntryHeight + OffsetSize));
        private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
        private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;
        private readonly PropertyInfo m_Property;
        private readonly Mobile m_Mobile;
        private readonly object m_Object;
        private readonly Stack m_Stack;
        private readonly Type m_Type;
        private readonly int m_Page;
        private readonly ArrayList m_List;
        public SetObjectGump(PropertyInfo prop, Mobile mobile, object o, Stack stack, Type type, int page, ArrayList list)
            : base(GumpOffsetX, GumpOffsetY)
        {
            this.m_Property = prop;
            this.m_Mobile = mobile;
            this.m_Object = o;
            this.m_Stack = stack;
            this.m_Type = type;
            this.m_Page = page;
            this.m_List = list;

            string initialText = PropertiesGump.ValueToString(o, prop);

            this.AddPage(0);

            this.AddBackground(0, 0, BackWidth, BackHeight, BackGumpID);
            this.AddImageTiled(BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), TotalHeight, OffsetGumpID);

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
            this.AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, initialText);
            x += EntryWidth + OffsetSize;

            if (SetGumpID != 0)
                this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

            this.AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 1, GumpButtonType.Reply, 0);

            x = BorderSize + OffsetSize;
            y += EntryHeight + OffsetSize;

            this.AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
            this.AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "Change by Serial");
            x += EntryWidth + OffsetSize;

            if (SetGumpID != 0)
                this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

            this.AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 2, GumpButtonType.Reply, 0);

            x = BorderSize + OffsetSize;
            y += EntryHeight + OffsetSize;

            this.AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
            this.AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "Nullify");
            x += EntryWidth + OffsetSize;

            if (SetGumpID != 0)
                this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

            this.AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 3, GumpButtonType.Reply, 0);

            x = BorderSize + OffsetSize;
            y += EntryHeight + OffsetSize;

            this.AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
            this.AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "View Properties");
            x += EntryWidth + OffsetSize;

            if (SetGumpID != 0)
                this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

            this.AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 4, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            object toSet;
            bool shouldSet, shouldSend = true;
            object viewProps = null;

            switch ( info.ButtonID )
            {
                case 0: // closed
                    {
                        this.m_Mobile.SendGump(new PropertiesGump(this.m_Mobile, this.m_Object, this.m_Stack, this.m_List, this.m_Page));

                        toSet = null;
                        shouldSet = false;
                        shouldSend = false;

                        break;
                    }
                case 1: // Change by Target
                    {
                        this.m_Mobile.Target = new SetObjectTarget(this.m_Property, this.m_Mobile, this.m_Object, this.m_Stack, this.m_Type, this.m_Page, this.m_List);
                        toSet = null;
                        shouldSet = false;
                        shouldSend = false;
                        break;
                    }
                case 2: // Change by Serial
                    {
                        toSet = null;
                        shouldSet = false;
                        shouldSend = false;

                        this.m_Mobile.SendMessage("Enter the serial you wish to find:");
                        this.m_Mobile.Prompt = new InternalPrompt(this.m_Property, this.m_Mobile, this.m_Object, this.m_Stack, this.m_Type, this.m_Page, this.m_List);

                        break;
                    }
                case 3: // Nullify
                    {
                        toSet = null;
                        shouldSet = true;

                        break;
                    }
                case 4: // View Properties
                    {
                        toSet = null;
                        shouldSet = false;

                        object obj = this.m_Property.GetValue(this.m_Object, null);

                        if (obj == null)
                            this.m_Mobile.SendMessage("The property is null and so you cannot view its properties.");
                        else if (!BaseCommand.IsAccessible(this.m_Mobile, obj))
                            this.m_Mobile.SendMessage("You may not view their properties.");
                        else
                            viewProps = obj;

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
                this.m_Mobile.SendGump(new SetObjectGump(this.m_Property, this.m_Mobile, this.m_Object, this.m_Stack, this.m_Type, this.m_Page, this.m_List));

            if (viewProps != null)
                this.m_Mobile.SendGump(new PropertiesGump(this.m_Mobile, viewProps));
        }

        private class InternalPrompt : Prompt
        {
            private readonly PropertyInfo m_Property;
            private readonly Mobile m_Mobile;
            private readonly object m_Object;
            private readonly Stack m_Stack;
            private readonly Type m_Type;
            private readonly int m_Page;
            private readonly ArrayList m_List;
            public InternalPrompt(PropertyInfo prop, Mobile mobile, object o, Stack stack, Type type, int page, ArrayList list)
            {
                this.m_Property = prop;
                this.m_Mobile = mobile;
                this.m_Object = o;
                this.m_Stack = stack;
                this.m_Type = type;
                this.m_Page = page;
                this.m_List = list;
            }

            public override void OnCancel(Mobile from)
            {
                this.m_Mobile.SendGump(new SetObjectGump(this.m_Property, this.m_Mobile, this.m_Object, this.m_Stack, this.m_Type, this.m_Page, this.m_List));
            }

            public override void OnResponse(Mobile from, string text)
            {
                object toSet;
                bool shouldSet;

                try
                {
                    int serial = Utility.ToInt32(text);

                    toSet = World.FindEntity(serial);

                    if (toSet == null)
                    {
                        shouldSet = false;
                        this.m_Mobile.SendMessage("No object with that serial was found.");
                    }
                    else if (!this.m_Type.IsAssignableFrom(toSet.GetType()))
                    {
                        toSet = null;
                        shouldSet = false;
                        this.m_Mobile.SendMessage("The object with that serial could not be assigned to a property of type : {0}", this.m_Type.Name);
                    }
                    else
                    {
                        shouldSet = true;
                    }
                }
                catch
                {
                    toSet = null;
                    shouldSet = false;
                    this.m_Mobile.SendMessage("Bad format");
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

                this.m_Mobile.SendGump(new SetObjectGump(this.m_Property, this.m_Mobile, this.m_Object, this.m_Stack, this.m_Type, this.m_Page, this.m_List));
            }
        }
    }
}