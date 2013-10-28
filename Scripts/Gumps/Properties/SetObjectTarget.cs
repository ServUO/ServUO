using System;
using System.Collections;
using System.Reflection;
using Server.Commands;
using Server.Items;
using Server.Targeting;

namespace Server.Gumps
{
    public class SetObjectTarget : Target
    {
        private readonly PropertyInfo m_Property;
        private readonly Mobile m_Mobile;
        private readonly object m_Object;
        private readonly Stack m_Stack;
        private readonly Type m_Type;
        private readonly int m_Page;
        private readonly ArrayList m_List;
        public SetObjectTarget(PropertyInfo prop, Mobile mobile, object o, Stack stack, Type type, int page, ArrayList list)
            : base(-1, false, TargetFlags.None)
        {
            this.m_Property = prop;
            this.m_Mobile = mobile;
            this.m_Object = o;
            this.m_Stack = stack;
            this.m_Type = type;
            this.m_Page = page;
            this.m_List = list;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            try
            {
                if (this.m_Type == typeof(Type))
                    targeted = targeted.GetType();
                else if ((this.m_Type == typeof(BaseAddon) || this.m_Type.IsAssignableFrom(typeof(BaseAddon))) && targeted is AddonComponent)
                    targeted = ((AddonComponent)targeted).Addon;

                if (this.m_Type.IsAssignableFrom(targeted.GetType()))
                {
                    CommandLogging.LogChangeProperty(this.m_Mobile, this.m_Object, this.m_Property.Name, targeted.ToString());
                    this.m_Property.SetValue(this.m_Object, targeted, null);
                    PropertiesGump.OnValueChanged(this.m_Object, this.m_Property, this.m_Stack);
                }
                else
                {
                    this.m_Mobile.SendMessage("That cannot be assigned to a property of type : {0}", this.m_Type.Name);
                }
            }
            catch
            {
                this.m_Mobile.SendMessage("An exception was caught. The property may not have changed.");
            }
        }

        protected override void OnTargetFinish(Mobile from)
        {
            if (this.m_Type == typeof(Type))
                from.SendGump(new PropertiesGump(this.m_Mobile, this.m_Object, this.m_Stack, this.m_List, this.m_Page));
            else
                from.SendGump(new SetObjectGump(this.m_Property, this.m_Mobile, this.m_Object, this.m_Stack, this.m_Type, this.m_Page, this.m_List));
        }
    }
}