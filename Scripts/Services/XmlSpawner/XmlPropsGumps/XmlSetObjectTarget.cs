using Server.Commands;
using Server.Items;
using Server.Targeting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Server.Gumps
{
    public class XmlSetObjectTarget : Target
    {
        private readonly PropertyInfo m_Property;
        private readonly Mobile m_Mobile;
        private readonly object m_Object;
#if (NEWTIMERS)
        private readonly Stack<PropertiesGump.StackEntry> m_Stack;
#else
		private Stack m_Stack;
#endif
        private readonly Type m_Type;
        private readonly int m_Page;
        private readonly ArrayList m_List;

#if (NEWTIMERS)
        public XmlSetObjectTarget(PropertyInfo prop, Mobile mobile, object o, Stack<PropertiesGump.StackEntry> stack, Type type, int page, ArrayList list) : base(-1, false, TargetFlags.None)
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

        protected override void OnTarget(Mobile from, object targeted)
        {
            try
            {
                if (m_Type == typeof(Type))
                    targeted = targeted.GetType();
                else if ((m_Type == typeof(BaseAddon) || m_Type.IsAssignableFrom(typeof(BaseAddon))) && targeted is AddonComponent component)
                    targeted = component.Addon;

                if (m_Type.IsInstanceOfType(targeted))
                {
                    CommandLogging.LogChangeProperty(m_Mobile, m_Object, m_Property.Name, targeted.ToString());
                    m_Property.SetValue(m_Object, targeted, null);
                }
                else
                {
                    m_Mobile.SendMessage("That cannot be assigned to a property of type : {0}", m_Type.Name);
                }
            }
            catch
            {
                m_Mobile.SendMessage("An exception was caught. The property may not have changed.");
            }
        }

        protected override void OnTargetFinish(Mobile from)
        {
            if (m_Type == typeof(Type))
                from.SendGump(new XmlPropertiesGump(m_Mobile, m_Object, m_Stack, m_List, m_Page));
            else
                from.SendGump(new XmlSetObjectGump(m_Property, m_Mobile, m_Object, m_Stack, m_Type, m_Page, m_List));
        }
    }
}
