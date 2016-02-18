using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Commands;
using Server.Commands.Generic;

namespace Server.Gumps
{
	public class XmlSetTimeSpanGump : Gump
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

		private static readonly int EntryWidth = 212;

		private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
		private static readonly int TotalHeight = OffsetSize + (7 * (EntryHeight + OffsetSize));

		private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
		private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;

#if (NEWTIMERS)
		public XmlSetTimeSpanGump( PropertyInfo prop, Mobile mobile, object o, Stack<PropertiesGump.StackEntry> stack, int page, ArrayList list ) : base( GumpOffsetX, GumpOffsetY )
#else
		public XmlSetTimeSpanGump( PropertyInfo prop, Mobile mobile, object o, Stack stack, int page, ArrayList list ) : base( GumpOffsetX, GumpOffsetY )
#endif
		{
			m_Property = prop;
			m_Mobile = mobile;
			m_Object = o;
			m_Stack = stack;
			m_Page = page;
			m_List = list;

			TimeSpan ts = (TimeSpan)prop.GetValue( o, null );

			AddPage( 0 );

			AddBackground( 0, 0, BackWidth, BackHeight, BackGumpID );
			AddImageTiled( BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), TotalHeight, OffsetGumpID );

			AddRect( 0, prop.Name, 0, -1 );
			AddRect( 1, ts.ToString(), 0, -1 );
			AddRect( 2, "Zero", 1, -1 );
			AddRect( 3, "From H:M:S", 2, -1 );
			AddRect( 4, "H:", 3, 0 );
			AddRect( 5, "M:", 4, 1 );
			AddRect( 6, "S:", 5, 2 );
		}

		private void AddRect( int index, string str, int button, int text )
		{
			int x = BorderSize + OffsetSize;
			int y = BorderSize + OffsetSize + (index * (EntryHeight + OffsetSize));

			AddImageTiled( x, y, EntryWidth, EntryHeight, EntryGumpID );
			AddLabelCropped( x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, str );

			if ( text != -1 )
				AddTextEntry( x + 16 + TextOffsetX, y, EntryWidth - TextOffsetX - 16, EntryHeight, TextHue, text, "" );

			x += EntryWidth + OffsetSize;

			if ( SetGumpID != 0 )
				AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );

			if ( button != 0 )
				AddButton( x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, button, GumpButtonType.Reply, 0 );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			TimeSpan toSet;
			bool shouldSet, shouldSend;

			TextRelay h = info.GetTextEntry( 0 );
			TextRelay m = info.GetTextEntry( 1 );
			TextRelay s = info.GetTextEntry( 2 );

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
					if ( h != null && m != null && s != null )
					{
						try
						{
							toSet = TimeSpan.Parse( h.Text + ":" + m.Text + ":" + s.Text );
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
				case 3: // From H
				{
					if ( h != null )
					{
						try
						{
							toSet = TimeSpan.FromHours( Utility.ToDouble( h.Text ) );
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
					if ( m != null )
					{
						try
						{
							toSet = TimeSpan.FromMinutes( Utility.ToDouble( m.Text ) );
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
					if ( s != null )
					{
						try
						{
							toSet = TimeSpan.FromSeconds( Utility.ToDouble( s.Text ) );
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
