using System;
using System.Collections;
using System.Reflection;
using Server.Commands;
using Server.Network;
using Server.Targeting;

namespace Server.Gumps
{
    public class SetPoint3DGump : Gump
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
        private static readonly int CoordWidth = 70;
        private static readonly int EntryWidth = CoordWidth + OffsetSize + CoordWidth + OffsetSize + CoordWidth;
        private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
        private static readonly int TotalHeight = OffsetSize + (4 * (EntryHeight + OffsetSize));
        private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
        private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;
        private readonly PropertyInfo m_Property;
        private readonly Mobile m_Mobile;
        private readonly object m_Object;
        private readonly Stack m_Stack;
        private readonly int m_Page;
        private readonly ArrayList m_List;
        public SetPoint3DGump(PropertyInfo prop, Mobile mobile, object o, Stack stack, int page, ArrayList list)
            : base(GumpOffsetX, GumpOffsetY)
        {
            this.m_Property = prop;
            this.m_Mobile = mobile;
            this.m_Object = o;
            this.m_Stack = stack;
            this.m_Page = page;
            this.m_List = list;

            Point3D p = (Point3D)prop.GetValue(o, null);

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
            this.AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "Use your location");
            x += EntryWidth + OffsetSize;

            if (SetGumpID != 0)
                this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
            this.AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 1, GumpButtonType.Reply, 0);

            x = BorderSize + OffsetSize;
            y += EntryHeight + OffsetSize;

            this.AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
            this.AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "Target a location");
            x += EntryWidth + OffsetSize;

            if (SetGumpID != 0)
                this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
            this.AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 2, GumpButtonType.Reply, 0);

            x = BorderSize + OffsetSize;
            y += EntryHeight + OffsetSize;

            this.AddImageTiled(x, y, CoordWidth, EntryHeight, EntryGumpID);
            this.AddLabelCropped(x + TextOffsetX, y, CoordWidth - TextOffsetX, EntryHeight, TextHue, "X:");
            this.AddTextEntry(x + 16, y, CoordWidth - 16, EntryHeight, TextHue, 0, p.X.ToString());
            x += CoordWidth + OffsetSize;

            this.AddImageTiled(x, y, CoordWidth, EntryHeight, EntryGumpID);
            this.AddLabelCropped(x + TextOffsetX, y, CoordWidth - TextOffsetX, EntryHeight, TextHue, "Y:");
            this.AddTextEntry(x + 16, y, CoordWidth - 16, EntryHeight, TextHue, 1, p.Y.ToString());
            x += CoordWidth + OffsetSize;

            this.AddImageTiled(x, y, CoordWidth, EntryHeight, EntryGumpID);
            this.AddLabelCropped(x + TextOffsetX, y, CoordWidth - TextOffsetX, EntryHeight, TextHue, "Z:");
            this.AddTextEntry(x + 16, y, CoordWidth - 16, EntryHeight, TextHue, 2, p.Z.ToString());
            x += CoordWidth + OffsetSize;

            if (SetGumpID != 0)
                this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
            this.AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 3, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Point3D toSet;
            bool shouldSet, shouldSend;

            switch ( info.ButtonID )
            {
                case 1: // Current location
                    {
                        toSet = this.m_Mobile.Location;
                        shouldSet = true;
                        shouldSend = true;

                        break;
                    }
                case 2: // Pick location
                    {
                        this.m_Mobile.Target = new InternalTarget(this.m_Property, this.m_Mobile, this.m_Object, this.m_Stack, this.m_Page, this.m_List);

                        toSet = Point3D.Zero;
                        shouldSet = false;
                        shouldSend = false;

                        break;
                    }
                case 3: // Use values
                    {
                        TextRelay x = info.GetTextEntry(0);
                        TextRelay y = info.GetTextEntry(1);
                        TextRelay z = info.GetTextEntry(2);

                        toSet = new Point3D(x == null ? 0 : Utility.ToInt32(x.Text), y == null ? 0 : Utility.ToInt32(y.Text), z == null ? 0 : Utility.ToInt32(z.Text));
                        shouldSet = true;
                        shouldSend = true;

                        break;
                    }
                default:
                    {
                        toSet = Point3D.Zero;
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

        private class InternalTarget : Target
        {
            private readonly PropertyInfo m_Property;
            private readonly Mobile m_Mobile;
            private readonly object m_Object;
            private readonly Stack m_Stack;
            private readonly int m_Page;
            private readonly ArrayList m_List;
            public InternalTarget(PropertyInfo prop, Mobile mobile, object o, Stack stack, int page, ArrayList list)
                : base(-1, true, TargetFlags.None)
            {
                this.m_Property = prop;
                this.m_Mobile = mobile;
                this.m_Object = o;
                this.m_Stack = stack;
                this.m_Page = page;
                this.m_List = list;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;

                if (p != null)
                {
                    try
                    {
                        CommandLogging.LogChangeProperty(this.m_Mobile, this.m_Object, this.m_Property.Name, new Point3D(p).ToString());
                        this.m_Property.SetValue(this.m_Object, new Point3D(p), null);
                        PropertiesGump.OnValueChanged(this.m_Object, this.m_Property, this.m_Stack);
                    }
                    catch
                    {
                        this.m_Mobile.SendMessage("An exception was caught. The property may not have changed.");
                    }
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Mobile.SendGump(new PropertiesGump(this.m_Mobile, this.m_Object, this.m_Stack, this.m_List, this.m_Page));
            }
        }
    }
}