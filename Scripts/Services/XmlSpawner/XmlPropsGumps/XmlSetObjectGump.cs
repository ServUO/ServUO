using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Prompts;
using Server.Commands;
using Server.Commands.Generic;

namespace Server.Gumps
{
	public class XmlSetObjectGump : Gump
	{
		private PropertyInfo m_Property;
		private Mobile m_Mobile;
		private object m_Object;
#if (NEWTIMERS)
		private Stack<PropertiesGump.StackEntry> m_Stack;
#else
		private Stack m_Stack;
#endif
		private Type m_Type;
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
		private static readonly int TotalHeight = OffsetSize + (5 * (EntryHeight + OffsetSize));

		private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
		private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;

#if (NEWTIMERS)
		public XmlSetObjectGump( PropertyInfo prop, Mobile mobile, object o, Stack<PropertiesGump.StackEntry> stack, Type type, int page, ArrayList list ) : base( GumpOffsetX, GumpOffsetY )
#else
		public XmlSetObjectGump( PropertyInfo prop, Mobile mobile, object o, Stack stack, Type type, int page, ArrayList list ) : base( GumpOffsetX, GumpOffsetY )
#endif
		{
			m_Property = prop;
			m_Mobile = mobile;
			m_Object = o;
			m_Stack = stack;
			m_Type = type;
			m_Page = page;
			m_List = list;

			string initialText = XmlPropertiesGump.ValueToString( o, prop );

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
			AddLabelCropped( x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, initialText );
			x += EntryWidth + OffsetSize;

			if ( SetGumpID != 0 )
				AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );

			AddButton( x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 1, GumpButtonType.Reply, 0 );

			x = BorderSize + OffsetSize;
			y += EntryHeight + OffsetSize;

			AddImageTiled( x, y, EntryWidth, EntryHeight, EntryGumpID );
			AddLabelCropped( x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "Change by Serial" );
			x += EntryWidth + OffsetSize;

			if ( SetGumpID != 0 )
				AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );

			AddButton( x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 2, GumpButtonType.Reply, 0 );

			x = BorderSize + OffsetSize;
			y += EntryHeight + OffsetSize;

			AddImageTiled( x, y, EntryWidth, EntryHeight, EntryGumpID );
			AddLabelCropped( x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "Nullify" );
			x += EntryWidth + OffsetSize;

			if ( SetGumpID != 0 )
				AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );

			AddButton( x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 3, GumpButtonType.Reply, 0 );

			x = BorderSize + OffsetSize;
			y += EntryHeight + OffsetSize;

			AddImageTiled( x, y, EntryWidth, EntryHeight, EntryGumpID );
			AddLabelCropped( x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "View Properties" );
			x += EntryWidth + OffsetSize;

			if ( SetGumpID != 0 )
				AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );

			AddButton( x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 4, GumpButtonType.Reply, 0 );
		}

		private class InternalPrompt : Prompt
		{
			private PropertyInfo m_Property;
			private Mobile m_Mobile;
			private object m_Object;
#if (NEWTIMERS)
			private Stack<PropertiesGump.StackEntry> m_Stack;
#else
			private Stack m_Stack;
#endif
			private Type m_Type;
			private int m_Page;
			private ArrayList m_List;

#if (NEWTIMERS)
			public InternalPrompt( PropertyInfo prop, Mobile mobile, object o, Stack<PropertiesGump.StackEntry> stack, Type type, int page, ArrayList list )
#else
			public InternalPrompt( PropertyInfo prop, Mobile mobile, object o, Stack stack, Type type, int page, ArrayList list )
#endif
			{
				m_Property = prop;
				m_Mobile = mobile;
				m_Object = o;
				m_Stack = stack;
				m_Type = type;
				m_Page = page;
				m_List = list;
			}

			public override void OnCancel( Mobile from )
			{
				m_Mobile.SendGump( new XmlSetObjectGump( m_Property, m_Mobile, m_Object, m_Stack, m_Type, m_Page, m_List ) );
			}

			public override void OnResponse( Mobile from, string text )
			{
				object toSet;
				bool shouldSet;

				try
				{
					int serial = Utility.ToInt32( text );

					toSet = World.FindEntity( serial );

					if ( toSet == null )
					{
						shouldSet = false;
						m_Mobile.SendMessage( "No object with that serial was found." );
					}
					else if ( !m_Type.IsAssignableFrom( toSet.GetType() ) )
					{
						toSet = null;
						shouldSet = false;
						m_Mobile.SendMessage( "The object with that serial could not be assigned to a property of type : {0}", m_Type.Name );
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
					m_Mobile.SendMessage( "Bad format" );
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

				m_Mobile.SendGump( new XmlSetObjectGump( m_Property, m_Mobile, m_Object, m_Stack, m_Type, m_Page, m_List ) );
			}
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			object toSet;
			bool shouldSet, shouldSend = true;
			object viewProps = null;

			switch ( info.ButtonID )
			{
				case 0: // closed
				{
					m_Mobile.SendGump( new XmlPropertiesGump( m_Mobile, m_Object, m_Stack, m_List, m_Page ) );

					toSet = null;
					shouldSet = false;
					shouldSend = false;

					break;
				}
				case 1: // Change by Target
				{
					m_Mobile.Target = new XmlSetObjectTarget( m_Property, m_Mobile, m_Object, m_Stack, m_Type, m_Page, m_List );
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

					m_Mobile.SendMessage( "Enter the serial you wish to find:" );
					m_Mobile.Prompt = new InternalPrompt( m_Property, m_Mobile, m_Object, m_Stack, m_Type, m_Page, m_List );

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

					object obj = m_Property.GetValue( m_Object, null );

					if ( obj == null )
						m_Mobile.SendMessage( "The property is null and so you cannot view its properties." );
					else if ( !BaseCommand.IsAccessible( m_Mobile, obj ) )
						m_Mobile.SendMessage( "You may not view their properties." );
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
				m_Mobile.SendGump( new XmlSetObjectGump( m_Property, m_Mobile, m_Object, m_Stack, m_Type, m_Page, m_List ) );

			if ( viewProps != null )
				m_Mobile.SendGump( new XmlPropertiesGump( m_Mobile, viewProps ) );
		}
	}
}
