using System;
using System.Collections;
using System.Reflection;
using Server.Commands;
using Server.Network;

namespace Server.Gumps
{
    public class XmlSetCustomEnumGump : XmlSetListOptionGump
    {
        private readonly string[] m_Names;
        public XmlSetCustomEnumGump(PropertyInfo prop, Mobile mobile, object o, Stack stack, int propspage, ArrayList list, string[] names)
            : base(prop, mobile, o, stack, propspage, list, names, null)
        {
            this.m_Names = names;
        }

        public override void OnResponse(NetState sender, RelayInfo relayInfo)
        {
            int index = relayInfo.ButtonID - 1;

            if (index >= 0 && index < this.m_Names.Length)
            {
                try
                {
                    MethodInfo info = this.m_Property.PropertyType.GetMethod("Parse", new Type[] { typeof(string) });

                    CommandLogging.LogChangeProperty(this.m_Mobile, this.m_Object, this.m_Property.Name, this.m_Names[index]);

                    if (info != null)
                        this.m_Property.SetValue(this.m_Object, info.Invoke(null, new object[] { this.m_Names[index] }), null);
                    else if (this.m_Property.PropertyType == typeof(Enum) || this.m_Property.PropertyType.IsSubclassOf(typeof(Enum)))
                        this.m_Property.SetValue(this.m_Object, Enum.Parse(this.m_Property.PropertyType, this.m_Names[index], false), null);
                }
                catch
                {
                    this.m_Mobile.SendMessage("An exception was caught. The property may not have changed.");
                }
            }

            this.m_Mobile.SendGump(new XmlPropertiesGump(this.m_Mobile, this.m_Object, this.m_Stack, this.m_List, this.m_Page));
        }
    }
}