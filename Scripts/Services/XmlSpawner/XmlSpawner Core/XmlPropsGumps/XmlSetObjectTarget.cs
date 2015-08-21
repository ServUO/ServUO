using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Commands;
using Server.Commands.Generic;

namespace Server.Gumps
{
	public class XmlSetObjectTarget : Target
	{
		private PropertyInfo m_Property;
		private Mobile m_Mobile;
		private object m_Object;
#if (NEWTIMERS)
		private Stack<StackEntry> m_Stack;
#else
		private Stack m_Stack;
#endif
		private Type m_Type;
		private int m_Page;
		private ArrayList m_List;

#if (NEWTIMERS)
		public XmlSetObjectTarget( PropertyInfo prop, Mobile mobile, object o, Stack<StackEntry> stack, Type type, int page, ArrayList list ) : base( -1, false, TargetFlags.None )
#else
		public XmlSetObjectTarget( PropertyInfo prop, Mobile mobile, object o, Stack stack, Type type, int page, ArrayList list ) : base( -1, false, TargetFlags.None )
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

		protected override void OnTarget( Mobile from, object targeted )
		{
			try
			{
				if ( m_Type == typeof( Type ) )
					targeted = targeted.GetType();
				else if ( (m_Type == typeof( BaseAddon ) || m_Type.IsAssignableFrom( typeof( BaseAddon ) )) && targeted is AddonComponent )
					targeted = ((AddonComponent)targeted).Addon;

				if ( m_Type.IsAssignableFrom( targeted.GetType() ) )
				{
					CommandLogging.LogChangeProperty( m_Mobile, m_Object, m_Property.Name, targeted.ToString() );
					m_Property.SetValue( m_Object, targeted, null );
				}
				else
				{
					m_Mobile.SendMessage( "That cannot be assigned to a property of type : {0}", m_Type.Name );
				}
			}
			catch
			{
				m_Mobile.SendMessage( "An exception was caught. The property may not have changed." );
			}
		}

		protected override void OnTargetFinish( Mobile from )
		{
			if ( m_Type == typeof( Type ) )
				from.SendGump( new XmlPropertiesGump( m_Mobile, m_Object, m_Stack, m_List, m_Page ) );
			else
				from.SendGump( new XmlSetObjectGump( m_Property, m_Mobile, m_Object, m_Stack, m_Type, m_Page, m_List ) );
		}
	}
}
