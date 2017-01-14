using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Commands;
using Server.Commands.Generic;

namespace Server.Gumps
{
	public class XmlSetPoint2DGump : Gump
	{
		private PropertyInfo m_Property;
		private Mobile m_Mobile;
		private object m_Object;
#if (NEWTIMERS)
		private Stack<PropertiesGump.StackEntry> m_Stack;
#else
		private Stack m_Stack;
#endif
		private int m_Page;
		private ArrayList m_List;

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

		private static readonly int CoordWidth = 105;
		private static readonly int EntryWidth = CoordWidth + OffsetSize + CoordWidth;

		private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
		private static readonly int TotalHeight = OffsetSize + (4 * (EntryHeight + OffsetSize));

		private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
		private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;

#if (NEWTIMERS)
		public XmlSetPoint2DGump( PropertyInfo prop, Mobile mobile, object o, Stack<PropertiesGump.StackEntry> stack, int page, ArrayList list ) : base( GumpOffsetX, GumpOffsetY )
#else
		public XmlSetPoint2DGump( PropertyInfo prop, Mobile mobile, object o, Stack stack, int page, ArrayList list ) : base( GumpOffsetX, GumpOffsetY )
#endif
		{
			m_Property = prop;
			m_Mobile = mobile;
			m_Object = o;
			m_Stack = stack;
			m_Page = page;
			m_List = list;

			Point2D p = (Point2D)prop.GetValue( o, null );

			AddPage( 0 );

			AddBackground( 0, 0, BackWidth, BackHeight, BackGumpID );
			AddImageTiled( BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), TotalHeight, OffsetGumpID );

			int x = BorderSize + OffsetSize;
			int y = BorderSize + OffsetSize;

			AddImageTiled( x, y, EntryWidth, EntryHeight, EntryGumpID );
			AddLabelCropped( x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, prop.Name );
			x += EntryWidth + OffsetSize;

			if ( SetGumpID != 0 )
				AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );

			x = BorderSize + OffsetSize;
			y += EntryHeight + OffsetSize;

			AddImageTiled( x, y, EntryWidth, EntryHeight, EntryGumpID );
			AddLabelCropped( x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "Use your location" );
			x += EntryWidth + OffsetSize;

			if ( SetGumpID != 0 )
				AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );
			AddButton( x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 1, GumpButtonType.Reply, 0 );

			x = BorderSize + OffsetSize;
			y += EntryHeight + OffsetSize;

			AddImageTiled( x, y, EntryWidth, EntryHeight, EntryGumpID );
			AddLabelCropped( x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "Target a location" );
			x += EntryWidth + OffsetSize;

			if ( SetGumpID != 0 )
				AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );
			AddButton( x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 2, GumpButtonType.Reply, 0 );

			x = BorderSize + OffsetSize;
			y += EntryHeight + OffsetSize;

			AddImageTiled( x, y, CoordWidth, EntryHeight, EntryGumpID );
			AddLabelCropped( x + TextOffsetX, y, CoordWidth - TextOffsetX, EntryHeight, TextHue, "X:" );
			AddTextEntry( x + 16, y, CoordWidth - 16, EntryHeight, TextHue, 0, p.X.ToString() );
			x += CoordWidth + OffsetSize;

			AddImageTiled( x, y, CoordWidth, EntryHeight, EntryGumpID );
			AddLabelCropped( x + TextOffsetX, y, CoordWidth - TextOffsetX, EntryHeight, TextHue, "Y:" );
			AddTextEntry( x + 16, y, CoordWidth - 16, EntryHeight, TextHue, 1, p.Y.ToString() );
			x += CoordWidth + OffsetSize;

			if ( SetGumpID != 0 )
				AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );
			AddButton( x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 3, GumpButtonType.Reply, 0 );
		}

		private class InternalTarget : Target
		{
			private PropertyInfo m_Property;
			private Mobile m_Mobile;
			private object m_Object;
#if (NEWTIMERS)
			private Stack<PropertiesGump.StackEntry> m_Stack;
#else
			private Stack m_Stack;
#endif
			private int m_Page;
			private ArrayList m_List;

#if (NEWTIMERS)
			public InternalTarget( PropertyInfo prop, Mobile mobile, object o, Stack<PropertiesGump.StackEntry> stack, int page, ArrayList list ) : base( -1, true, TargetFlags.None )
#else
			public InternalTarget( PropertyInfo prop, Mobile mobile, object o, Stack stack, int page, ArrayList list ) : base( -1, true, TargetFlags.None )
#endif
			{
				m_Property = prop;
				m_Mobile = mobile;
				m_Object = o;
				m_Stack = stack;
				m_Page = page;
				m_List = list;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				IPoint3D p = targeted as IPoint3D;

				if ( p != null )
				{
					try
					{
						CommandLogging.LogChangeProperty( m_Mobile, m_Object, m_Property.Name, new Point2D( p ).ToString() );
						m_Property.SetValue( m_Object, new Point2D( p ), null );
					}
					catch
					{
						m_Mobile.SendMessage( "An exception was caught. The property may not have changed." );
					}
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Mobile.SendGump( new XmlPropertiesGump( m_Mobile, m_Object, m_Stack, m_List, m_Page ) );
			}
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Point2D toSet;
			bool shouldSet, shouldSend;

			switch ( info.ButtonID )
			{
				case 1: // Current location
				{
					toSet = new Point2D( m_Mobile.Location );
					shouldSet = true;
					shouldSend = true;

					break;
				}
				case 2: // Pick location
				{
					m_Mobile.Target = new InternalTarget( m_Property, m_Mobile, m_Object, m_Stack, m_Page, m_List );

					toSet = Point2D.Zero;
					shouldSet = false;
					shouldSend = false;

					break;
				}
				case 3: // Use values
				{
					TextRelay x = info.GetTextEntry( 0 );
					TextRelay y = info.GetTextEntry( 1 );

					toSet = new Point2D( x == null ? 0 : Utility.ToInt32( x.Text ), y == null ? 0 : Utility.ToInt32( y.Text ) );
					shouldSet = true;
					shouldSend = true;

					break;
				}
				default:
				{
					toSet = Point2D.Zero;
					shouldSet = false;
					shouldSend = true;

					break;
				}
			}

			if ( shouldSet )
			{
				try
				{
					CommandLogging.LogChangeProperty( m_Mobile, m_Object, m_Property.Name, toSet.ToString() );
					m_Property.SetValue( m_Object, toSet, null );
				}
				catch
				{
					m_Mobile.SendMessage( "An exception was caught. The property may not have changed." );
				}
			}

			if ( shouldSend )
				m_Mobile.SendGump( new XmlPropertiesGump( m_Mobile, m_Object, m_Stack, m_List, m_Page ) );
		}
	}
}
