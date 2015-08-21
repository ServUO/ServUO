using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.HuePickers;
using Server.Commands;
using Server.Commands.Generic;

namespace Server.Gumps
{
	public class XmlSetGump : Gump
	{
		private PropertyInfo m_Property;
		private Mobile m_Mobile;
		private object m_Object;
#if (NEWTIMERS)
		private Stack<StackEntry> m_Stack;
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

		private static readonly int EntryWidth = 212;

		private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
		private static readonly int TotalHeight = OffsetSize + (2 * (EntryHeight + OffsetSize));

		private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
		private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;

#if (NEWTIMERS)
		public XmlSetGump( PropertyInfo prop, Mobile mobile, object o, Stack<StackEntry> stack, int page, ArrayList list ) : base( GumpOffsetX, GumpOffsetY )
#else
		public XmlSetGump( PropertyInfo prop, Mobile mobile, object o, Stack stack, int page, ArrayList list ) : base( GumpOffsetX, GumpOffsetY )
#endif
		{
			m_Property = prop;
			m_Mobile = mobile;
			m_Object = o;
			m_Stack = stack;
			m_Page = page;
			m_List = list;

			bool canNull = !prop.PropertyType.IsValueType;
			bool canDye = prop.IsDefined( typeof( HueAttribute ), false );
			bool isBody = prop.IsDefined( typeof( BodyAttribute ), false );

            int xextend = 0;
			if(prop.PropertyType == typeof(string))
			{
			     xextend = 300;
			}

			object val = prop.GetValue( m_Object, null );
			string initialText;

			if ( val == null )
				initialText = "";
			else
				initialText = val.ToString();

			AddPage( 0 );

			AddBackground( 0, 0, BackWidth+xextend, BackHeight + (canNull ? (EntryHeight + OffsetSize) : 0) + (canDye ? (EntryHeight + OffsetSize) : 0) + (isBody ? (EntryHeight + OffsetSize) : 0), BackGumpID );
			AddImageTiled( BorderSize, BorderSize, TotalWidth+xextend - (OldStyle ? SetWidth + OffsetSize : 0), TotalHeight + (canNull ? (EntryHeight + OffsetSize) : 0) + (canDye ? (EntryHeight + OffsetSize) : 0) + (isBody ? (EntryHeight + OffsetSize) : 0), OffsetGumpID );

			int x = BorderSize + OffsetSize;
			int y = BorderSize + OffsetSize;

			AddImageTiled( x, y, EntryWidth+xextend, EntryHeight, EntryGumpID );
			AddLabelCropped( x + TextOffsetX, y, EntryWidth+xextend - TextOffsetX, EntryHeight, TextHue, prop.Name );
			x += EntryWidth+xextend + OffsetSize;

			if ( SetGumpID != 0 )
				AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );

			x = BorderSize + OffsetSize;
			y += EntryHeight + OffsetSize;

			AddImageTiled( x, y, EntryWidth+xextend, EntryHeight, EntryGumpID );
			AddTextEntry( x + TextOffsetX, y, EntryWidth+xextend - TextOffsetX, EntryHeight, TextHue, 0, initialText );
			x += EntryWidth+xextend + OffsetSize;

			if ( SetGumpID != 0 )
				AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );

			AddButton( x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 1, GumpButtonType.Reply, 0 );

			if ( canNull )
			{
				x = BorderSize + OffsetSize;
				y += EntryHeight + OffsetSize;

				AddImageTiled( x, y, EntryWidth+xextend, EntryHeight, EntryGumpID );
				AddLabelCropped( x + TextOffsetX, y, EntryWidth+xextend - TextOffsetX, EntryHeight, TextHue, "Null" );
				x += EntryWidth+xextend + OffsetSize;

				if ( SetGumpID != 0 )
					AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );

				AddButton( x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 2, GumpButtonType.Reply, 0 );
			}

			if ( canDye )
			{
				x = BorderSize + OffsetSize;
				y += EntryHeight + OffsetSize;

				AddImageTiled( x, y, EntryWidth+xextend, EntryHeight, EntryGumpID );
				AddLabelCropped( x + TextOffsetX, y, EntryWidth+xextend - TextOffsetX, EntryHeight, TextHue, "Hue Picker" );
				x += EntryWidth+xextend + OffsetSize;

				if ( SetGumpID != 0 )
					AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );

				AddButton( x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 3, GumpButtonType.Reply, 0 );
			}

			if ( isBody )
			{
				x = BorderSize + OffsetSize;
				y += EntryHeight + OffsetSize;

				AddImageTiled( x, y, EntryWidth+xextend, EntryHeight, EntryGumpID );
				AddLabelCropped( x + TextOffsetX, y, EntryWidth+xextend - TextOffsetX, EntryHeight, TextHue, "Body Picker" );
				x += EntryWidth+xextend + OffsetSize;

				if ( SetGumpID != 0 )
					AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );

				AddButton( x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 4, GumpButtonType.Reply, 0 );
			}
		}

		private class InternalPicker : HuePicker
		{
			private PropertyInfo m_Property;
			private Mobile m_Mobile;
			private object m_Object;
#if (NEWTIMERS)
			private Stack<StackEntry> m_Stack;
#else
			private Stack m_Stack;
#endif
			private int m_Page;
			private ArrayList m_List;

#if (NEWTIMERS)
			public InternalPicker( PropertyInfo prop, Mobile mobile, object o, Stack<StackEntry> stack, int page, ArrayList list ) : base( ((IHued)o).HuedItemID )
#else
			public InternalPicker( PropertyInfo prop, Mobile mobile, object o, Stack stack, int page, ArrayList list ) : base( ((IHued)o).HuedItemID )
#endif
			{
				m_Property = prop;
				m_Mobile = mobile;
				m_Object = o;
				m_Stack = stack;
				m_Page = page;
				m_List = list;
			}

			public override void OnResponse( int hue )
			{
				try
				{
					CommandLogging.LogChangeProperty( m_Mobile, m_Object, m_Property.Name, hue.ToString() );
					m_Property.SetValue( m_Object, hue, null );
				}
				catch
				{
					m_Mobile.SendMessage( "An exception was caught. The property may not have changed." );
				}

				m_Mobile.SendGump( new XmlPropertiesGump( m_Mobile, m_Object, m_Stack, m_List, m_Page ) );
			}
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			object toSet;
			bool shouldSet, shouldSend = true;

			switch ( info.ButtonID )
			{
				case 1:
				{
					TextRelay text = info.GetTextEntry( 0 );

					if ( text != null )
					{
						try
						{
							toSet = XmlPropertiesGump.GetObjectFromString( m_Property.PropertyType, text.Text );
							shouldSet = true;
						}
						catch
						{
							toSet = null;
							shouldSet = false;
							m_Mobile.SendMessage( "Bad format" );
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

					m_Mobile.SendHuePicker( new InternalPicker( m_Property, m_Mobile, m_Object, m_Stack, m_Page, m_List ) );

					break;
				}
				case 4: // Body Picker
				{
					toSet = null;
					shouldSet = false;
					shouldSend = false;

					m_Mobile.SendGump( new SetBodyGump( m_Property, m_Mobile, m_Object, m_Stack, m_Page, m_List ) );

					break;
				}
				default:
				{
					toSet = null;
					shouldSet = false;

					break;
				}
			}

			if ( shouldSet )
			{
				try
				{
					CommandLogging.LogChangeProperty( m_Mobile, m_Object, m_Property.Name, toSet==null?"(null)":toSet.ToString() );
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
