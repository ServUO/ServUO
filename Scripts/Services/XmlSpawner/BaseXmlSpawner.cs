using Server.Commands;
using Server.Items;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Server.Mobiles
{
    public class BaseXmlSpawner
    {
        [Flags]
        public enum KeywordFlags
        {
            HoldSpawn = 0x01,
            HoldSequence = 0x02,
            Serialize = 0x04,
            Defrag = 0x08
        }

        public class TypeInfo
        {
            public List<PropertyInfo> plist = new List<PropertyInfo>(); // hold propertyinfo list
            public Type t;
        }

        private static readonly Type typeofTimeSpan = typeof(TimeSpan);
        private static readonly Type typeofParsable = typeof(ParsableAttribute);
        private static readonly Type typeofCustomEnum = typeof(CustomEnumAttribute);

        private static bool IsParsable(Type t)
        {
            return (t == typeofTimeSpan || t.IsDefined(typeofParsable, false));
        }

        private static readonly Type[] m_ParseTypes = { typeof(string) };
        private static readonly object[] m_ParseParams = new object[1];

        private static object Parse(object o, Type t, string value)
        {
            MethodInfo method = t.GetMethod("Parse", m_ParseTypes);

            m_ParseParams[0] = value;

            return method.Invoke(o, m_ParseParams);
        }

        private static readonly Type[] m_NumericTypes =
        {
            typeof( byte ), typeof( sbyte ),
            typeof( short ), typeof( ushort ),
            typeof( int ), typeof( uint ),
            typeof( long ), typeof( ulong ), typeof( Serial )
        };

        public static bool IsNumeric(Type t)
        {
            return (Array.IndexOf(m_NumericTypes, t) >= 0);
        }

        private static readonly Type typeofType = typeof(Type);

        private static bool IsType(Type t)
        {
            return (t == typeofType);
        }

        private static readonly Type typeofChar = typeof(char);

        private static bool IsChar(Type t)
        {
            return (t == typeofChar);
        }

        private static readonly Type typeofString = typeof(string);

        private static bool IsString(Type t)
        {
            return (t == typeofString);
        }

        private static bool IsEnum(Type t)
        {
            return t.IsEnum;
        }

        private static bool IsCustomEnum(Type t)
        {
            return t.IsDefined(typeofCustomEnum, false);
        }

        private enum typeKeyword
        {
            SET,
            GOTO,
            COMMAND,
            SPAWN,
            DESPAWN
        }

        private enum typemodKeyword
        {
            // Preparing for removal. 
        }

        private enum valueKeyword
        {
            PLAYERSINRANGE,
            RANDNAME
        }

        private enum valuemodKeyword
        {
            INC,
            MOB,
            TRIGMOB,
            PLAYERSINRANGE
        }
        #region Static variable declarations
        // name of mobile used to issue commands via the COMMAND keyword.  The accesslevel of the mobile will determine
        // the accesslevel of commands that can be issued.
        // if this is null, then COMMANDS can only be issued when triggered by players of the appropriate accesslevel
        private static readonly string CommandMobileName = null;

        private static readonly Dictionary<string, typeKeyword> typeKeywordHash = new Dictionary<string, typeKeyword>();
        private static readonly Dictionary<string, typemodKeyword> typemodKeywordHash = new Dictionary<string, typemodKeyword>();
        private static readonly Dictionary<string, valueKeyword> valueKeywordHash = new Dictionary<string, valueKeyword>();
        private static readonly Dictionary<string, valuemodKeyword> valuemodKeywordHash = new Dictionary<string, valuemodKeyword>();

        private static readonly char[] slashdelim = { '/' };
        private static readonly char[] commadelim = { ',' };
        private static readonly char[] semicolondelim = { ';' };
        private static readonly char[] literalend = { '§' };
        #endregion

        #region Keywords
        public static bool IsValueKeyword(string str)
        {
            if (string.IsNullOrEmpty(str) || !char.IsUpper(str[0])) return false;
            return valueKeywordHash.ContainsKey(str);
        }

        public static bool IsValuemodKeyword(string str)
        {
            if (string.IsNullOrEmpty(str) || !char.IsUpper(str[0])) return false;
            return valuemodKeywordHash.ContainsKey(str);
        }

        public static bool IsTypeKeyword(string typeName)
        {
            if (string.IsNullOrEmpty(typeName) || !char.IsUpper(typeName[0])) return false;
            return (typeKeywordHash.ContainsKey(typeName));
        }

        public static bool IsTypeOrItemKeyword(string typeName)
        {
            if (string.IsNullOrEmpty(typeName) || !char.IsUpper(typeName[0])) return false;
            return (typeKeywordHash.ContainsKey(typeName));
        }

        public static void RemoveKeyword(string name)
        {
            if (name == null) return;

            name = name.Trim().ToUpper();

            typeKeywordHash.Remove(name);

            typemodKeywordHash.Remove(name);

            valueKeywordHash.Remove(name);

            valuemodKeywordHash.Remove(name);
        }
        #endregion

        #region KeywordTag

        public class KeywordTag
        {
            public KeywordFlags Flags;
            public int Type;
            private Timer m_Timer;
            public DateTime m_End;
            public DateTime m_TimeoutEnd;
            public TimeSpan m_Delay;
            public TimeSpan m_Timeout;
            private XmlSpawner m_Spawner;
            public string m_Condition;
            public int m_Goto;
            public bool Deleted = false;
            public int Serial = -1;
            public Mobile m_TrigMob;
            public string Typename;

            public KeywordTag(string typename, XmlSpawner spawner)
                : this(typename, spawner, -1)
            {
            }

            public KeywordTag(string typename, XmlSpawner spawner, int type)
                : this(typename, spawner, type, TimeSpan.Zero, TimeSpan.Zero, null, -1)
            {
            }

            public KeywordTag(string typename, XmlSpawner spawner, int type, TimeSpan delay, TimeSpan timeout, string condition, int gotogroup)
            {
                Type = type;
                m_Delay = delay;
                m_Timeout = timeout;
                m_TimeoutEnd = DateTime.UtcNow + timeout;
                m_Spawner = spawner;
                m_Condition = condition;
                m_Goto = gotogroup;

                Typename = typename;
                // add the tag to the list
                if (spawner != null && !spawner.Deleted)
                {
                    m_TrigMob = spawner.TriggerMob;
                    if (spawner.m_KeywordTagList == null)
                    {
                        spawner.m_KeywordTagList = new List<KeywordTag>();
                    }
                    // calculate the serial index of the new tag by adding one to the last one if there is one, otherwise just reset to 0
                    if (spawner.m_KeywordTagList.Count > 0)
                        Serial = spawner.m_KeywordTagList[spawner.m_KeywordTagList.Count - 1].Serial + 1;
                    else
                        Serial = 0;

                    spawner.m_KeywordTagList.Add(this);

                    switch (type)
                    {
                        case 0: // WAIT timer type
                                // start up the timer
                            DoTimer(delay, m_Delay, condition, gotogroup);
                            Flags |= KeywordFlags.HoldSpawn;
                            Flags |= KeywordFlags.Serialize;

                            break;
                        case 1: // GUMP type

                            break;
                        case 2: // GOTO type
                            Flags |= KeywordFlags.HoldSequence;
                            Flags |= KeywordFlags.Serialize;

                            break;
                        default:
                            // dont do anything for other types
                            Flags |= KeywordFlags.Defrag;
                            break;
                    }
                }
            }

            public void Delete()
            {
                // and stop all timers
                if (m_Timer != null && Type == 0)
                {
                    m_Timer.Stop();
                }

                Deleted = true;

                // and remove it from the list
                RemoveFromTagList(m_Spawner, this);

            }

            private void DoTimer(TimeSpan delay, TimeSpan repeatdelay, string condition, int gotogroup)
            {
                m_End = DateTime.UtcNow + delay;

                if (m_Timer != null)
                    m_Timer.Stop();

                m_Timer = new KeywordTimer(m_Spawner, this, delay, repeatdelay, condition, gotogroup);
                m_Timer.Start();
            }

            public void Serialize(GenericWriter writer)
            {
                writer.Write(1); // version
                                 // Version 1
                writer.Write((int)Flags);
                // Version 0
                writer.Write(m_Spawner);
                writer.Write(Type);
                writer.Write(Serial);
                if (Type == 0)
                {
                    // save any timer information
                    writer.Write(m_End - DateTime.UtcNow);
                    writer.Write(m_Delay);
                    writer.Write(m_Condition);
                    writer.Write(m_Goto);
                    writer.Write(m_TimeoutEnd - DateTime.UtcNow);
                    writer.Write(m_Timeout);
                    writer.Write(m_TrigMob);
                }
            }
            public void Deserialize(GenericReader reader)
            {

                int version = reader.ReadInt();
                switch (version)
                {
                    case 1:
                        Flags = (KeywordFlags)reader.ReadInt();
                        goto case 0;
                    case 0:
                        m_Spawner = (XmlSpawner)reader.ReadItem();
                        Type = reader.ReadInt();
                        Serial = reader.ReadInt();
                        if (Type == 0)
                        {
                            // get any timer info
                            TimeSpan delay = reader.ReadTimeSpan();
                            m_Delay = reader.ReadTimeSpan();
                            m_Condition = reader.ReadString();
                            m_Goto = reader.ReadInt();

                            TimeSpan timeoutdelay = reader.ReadTimeSpan();
                            m_TimeoutEnd = DateTime.UtcNow + timeoutdelay;
                            m_Timeout = reader.ReadTimeSpan();
                            m_TrigMob = reader.ReadMobile();

                            DoTimer(delay, m_Delay, m_Condition, m_Goto);
                        }
                        break;
                }
            }

            // added the timer that begins on spawning tmp keywords
            private class KeywordTimer : Timer
            {
                private readonly KeywordTag m_Tag;
                private readonly XmlSpawner m_Spawner;
                private readonly string m_Condition;
                private readonly int m_Goto;
                private TimeSpan m_Repeatdelay;

                public KeywordTimer(XmlSpawner spawner, KeywordTag tag, TimeSpan delay, TimeSpan repeatdelay, string condition, int gotogroup)
                    : base(delay)
                {
                    Priority = TimerPriority.OneSecond;
                    m_Tag = tag;
                    m_Spawner = spawner;
                    m_Condition = condition;
                    m_Goto = gotogroup;
                    m_Repeatdelay = repeatdelay;
                }

                protected override void OnTick()
                {
                    // if a condition is available then test it
                    if (!string.IsNullOrEmpty(m_Condition) && m_Spawner != null && m_Spawner.Running)
                    {
                        // if the test is valid then terminate the timer
                        string status_str;

                        if (TestItemProperty(m_Spawner, m_Spawner, m_Condition, out status_str))
                        {
                            // spawn the designated subgroup if specified
                            if (m_Goto >= 0 && m_Spawner != null && !m_Spawner.Deleted)
                            {
                                // set the trigmob to the mob that originally triggered the wait keyword
                                if (m_Tag != null)
                                    m_Spawner.TriggerMob = m_Tag.m_TrigMob;

                                // spawn the subgroup
                                m_Spawner.SpawnSubGroup(m_Goto, 0);
                            }

                            // get rid of the temporary tag
                            if (m_Tag != null && !m_Tag.Deleted)
                            {
                                m_Tag.Delete();
                            }

                        }
                        else
                        {
                            // otherwise restart it and keep on holding
                            if (m_Tag != null && !m_Tag.Deleted)
                            {
                                // check the timeout if applicable
                                if (m_Tag.m_Timeout > TimeSpan.Zero && m_Tag.m_TimeoutEnd < DateTime.UtcNow)
                                {
                                    // release the hold on spawning and delete the tag
                                    m_Tag.Delete();
                                }
                                else
                                {
                                    m_Tag.DoTimer(m_Repeatdelay, m_Repeatdelay, m_Condition, m_Goto);
                                }
                            }
                        }
                    }
                    else
                    {
                        // and terminate the timer
                        if (m_Tag != null && !m_Tag.Deleted)
                        {
                            m_Tag.Delete();
                        }
                    }
                }
            }
        }

        public static string TagInfo(KeywordTag tag)
        {
            if (tag != null)
                return (string.Format("{0} : type={1} cond={2} go={3} del={4} end={5}", tag.Typename, tag.Type, tag.m_Condition, tag.m_Goto, tag.m_Delay, tag.m_End));

            return null;
        }

        public static void RemoveFromTagList(XmlSpawner spawner, KeywordTag tag)
        {
            for (int i = 0; i < spawner.m_KeywordTagList.Count; i++)
            {
                if (tag == spawner.m_KeywordTagList[i])
                {
                    spawner.m_KeywordTagList.RemoveAt(i);
                    break;
                }
            }
        }

        public static KeywordTag GetFromTagList(XmlSpawner spawner, int serial)
        {
            for (int i = 0; i < spawner.m_KeywordTagList.Count; i++)
            {
                if (serial == spawner.m_KeywordTagList[i].Serial)
                {
                    return spawner.m_KeywordTagList[i];
                }
            }
            return (null);
        }

        #endregion

        #region Property parsing methods
        private static string InternalGetValue(object o, PropertyInfo p, int index)
        {
            Type type = p.PropertyType;
            object value = null;

            if (type.IsPrimitive)
            {
                value = p.GetValue(o, null);
            }
            else if ((type.GetInterface("IList") != null) && index >= 0)
            {
                try
                {
                    object arrayvalue = p.GetValue(o, null);
                    value = ((IList<object>)arrayvalue)[index];
                }
                catch { }
            }
            else
            {
                value = p.GetValue(o, null);
            }

            string toString;

            if (value == null)
                toString = "(-null-)";
            else if (IsNumeric(type))
                toString = string.Format("{0} (0x{0:X})", value);
            else if (IsChar(type))
                toString = string.Format("'{0}' ({1} [0x{1:X}])", value, (int)value);
            else if (IsString(type))
                toString = string.Format("\"{0}\"", value);
            else
                toString = value.ToString();

            return string.Format("{0} = {1}", p.Name, toString);
        }

        public static bool IsItem(Type type)
        {
            return (type != null && (type == typeof(Item) || type.IsSubclassOf(typeof(Item))));
        }

        public static bool IsMobile(Type type)
        {
            return (type != null && (type == typeof(Mobile) || type.IsSubclassOf(typeof(Mobile))));
        }

        public static string ConstructFromString(PropertyInfo p, Type type, object obj, string value, ref object constructed)
        {
            object toSet;

            if (value == "(-null-)" && !type.IsValueType)
                value = null;

            if (IsEnum(type))
            {
                try
                {
                    toSet = Enum.Parse(type, value, true);
                }
                catch
                {
                    return "That is not a valid enumeration member.";
                }
            }
            else if (IsCustomEnum(type))
            {
                try
                {
                    MethodInfo info = p.PropertyType.GetMethod("Parse", new[] { typeof(string) });
                    if (info != null)
                        toSet = info.Invoke(null, new object[] { value });
                    else if (p.PropertyType == typeof(Enum) || p.PropertyType.IsSubclassOf(typeof(Enum)))
                        toSet = Enum.Parse(p.PropertyType, value, false);
                    else
                        toSet = null;

                    if (toSet == null)
                        return "That is not a valid custom enumeration member.";
                }
                catch
                {
                    return "That is not a valid custom enumeration member.";
                }
            }
            else if (IsType(type))
            {
                try
                {
                    toSet = ScriptCompiler.FindTypeByName(value);

                    if (toSet == null)
                        return "No type with that name was found.";
                }
                catch
                {
                    return "No type with that name was found.";
                }
            }
            else if (IsParsable(type))
            {
                try
                {
                    toSet = Parse(obj, type, value);
                }
                catch
                {
                    return "That is not properly formatted.";
                }
            }
            else if (value == null)
            {
                toSet = null;
            }
            else if (value.StartsWith("0x") && IsNumeric(type))
            {
                try
                {
                    toSet = Convert.ChangeType(Convert.ToUInt64(value.Substring(2), 16), type);
                }
                catch
                {
                    return "That is not properly formatted. not convertible.";
                }
            }
            else if (value.StartsWith("0x") && (IsItem(type) || IsMobile(type)))
            {
                try
                {
                    // parse out the mobile or item name from the value string
                    int ispace = value.IndexOf(' ');
                    string valstr = value.Substring(2);
                    if (ispace > 0)
                    {
                        valstr = value.Substring(2, ispace - 2);
                    }

                    toSet = World.FindEntity(Convert.ToInt32(valstr, 16));

                    // now check to make sure the object returned is consistent with the type
                    if (!((toSet is Mobile && IsMobile(type)) || (toSet is Item && IsItem(type))))
                    {
                        return "Item/Mobile type mismatch. cannot assign.";
                    }
                }
                catch
                {
                    return "That is not properly formatted. not convertible.";
                }
            }
            else if ((type.GetInterface("IList") != null))
            {
                try
                {
                    object arrayvalue = p.GetValue(obj, null);

                    object po = ((IList<object>)arrayvalue)[0];

                    Type atype = po.GetType();

                    toSet = Parse(obj, atype, value);
                }
                catch
                {
                    return "That is not properly formatted.";
                }
            }
            else
            {
                try
                {
                    toSet = Convert.ChangeType(value, type);
                }
                catch
                {
                    return "That is not properly formatted.";
                }
            }

            constructed = toSet;

            return null;
        }

        public static string InternalSetValue(Mobile from, object o, PropertyInfo p, string value, bool shouldLog, int index)
        {
            object toSet = null;
            Type ptype = p.PropertyType;

            string result = ConstructFromString(p, p.PropertyType, o, value, ref toSet);

            if (result != null)
                return result;

            try
            {
                if (shouldLog)
                    CommandLogging.LogChangeProperty(from, o, p.Name, value);

                if (ptype.IsPrimitive)
                {
                    p.SetValue(o, toSet, null);
                }
                else if ((ptype.GetInterface("IList") != null) && index >= 0)
                {
                    try
                    {
                        object arrayvalue = p.GetValue(o, null);
                        ((IList<object>)arrayvalue)[index] = toSet;
                    }
                    catch { }
                }
                else
                {
                    p.SetValue(o, toSet, null);
                }

                return "Property has been set.";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return "An exception was caught, the property may not be set.";
            }
        }

        // set property values with support for nested attributes
        public static string SetPropertyValue(XmlSpawner spawner, object o, string name, string value)
        {
            if (o == null)
            {
                return "Null object";
            }

            Type type = o.GetType();

            PropertyInfo[] props = type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

            // parse the strings of the form property.attribute into two parts
            // first get the property
            string[] arglist = ParseString(name, 2, ".");

            string propname = arglist[0];

            // do a bit of parsing to handle array references
            string[] arraystring = propname.Split('[');
            int index = 0;
            if (arraystring.Length > 1)
            {
                // parse the property name from the indexing
                propname = arraystring[0];

                // then parse to get the index value
                string[] arrayvalue = arraystring[1].Split(']');

                if (arrayvalue.Length > 0)
                {
                    int.TryParse(arraystring[0], out index);
                }
            }

            if (arglist.Length == 2)
            {
                PropertyInfo plookup = LookupPropertyInfo(spawner, type, propname);

                object po;
                if (plookup != null)
                {
                    po = plookup.GetValue(o, null);

                    // now set the nested attribute using the new property list
                    return (SetPropertyValue(spawner, po, arglist[1], value));
                }

                // is a nested property with attributes so first get the property
                foreach (PropertyInfo p in props)
                {
                    if (Insensitive.Equals(p.Name, propname))
                    {
                        po = p.GetValue(o, null);

                        // now set the nested attribute using the new property list
                        return (SetPropertyValue(spawner, po, arglist[1], value));
                    }
                }
            }
            else
            {
                // its just a simple single property

                PropertyInfo plookup = LookupPropertyInfo(spawner, type, propname);

                if (plookup != null)
                {
                    if (!plookup.CanWrite)
                        return "Property is read only.";

                    string returnvalue = InternalSetValue(null, o, plookup, value, false, index);

                    return returnvalue;
                }
                // note, looping through all of the props turns out to be a significant performance bottleneck
                // good place for optimization

                foreach (PropertyInfo p in props)
                {
                    if (Insensitive.Equals(p.Name, propname))
                    {
                        if (!p.CanWrite)
                            return "Property is read only.";

                        string returnvalue = InternalSetValue(null, o, p, value, false, index);

                        return returnvalue;

                    }
                }
            }

            return "Property not found.";
        }

        public static string SetPropertyObject(XmlSpawner spawner, object o, string name, object value)
        {
            if (o == null)
            {
                return "Null object";
            }

            Type type = o.GetType();

            PropertyInfo[] props = type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

            // parse the strings of the form property.attribute into two parts
            // first get the property
            string[] arglist = ParseString(name, 2, ".");

            if (arglist.Length == 2)
            {
                // is a nested property with attributes so first get the property

                // use the lookup table for optimization if possible
                PropertyInfo plookup = LookupPropertyInfo(spawner, type, arglist[0]);

                object po;
                if (plookup != null)
                {
                    po = plookup.GetValue(o, null);

                    // now set the nested attribute using the new property list
                    return (SetPropertyObject(spawner, po, arglist[1], value));
                }

                foreach (PropertyInfo p in props)
                {
                    if (Insensitive.Equals(p.Name, arglist[0]))
                    {
                        po = p.GetValue(o, null);

                        // now set the nested attribute using the new property list
                        return (SetPropertyObject(spawner, po, arglist[1], value));

                    }
                }
            }
            else
            {
                // its just a simple single property

                // use the lookup table for optimization if possible
                PropertyInfo plookup = LookupPropertyInfo(spawner, type, name);

                if (plookup != null)
                {
                    if (!plookup.CanWrite)
                        return "Property is read only.";

                    if (plookup.PropertyType == typeof(Mobile))
                    {
                        plookup.SetValue(o, value, null);

                        return "Property has been set.";
                    }

                    return "Property is not of type Mobile.";
                }

                foreach (PropertyInfo p in props)
                {
                    if (Insensitive.Equals(p.Name, name))
                    {

                        if (!p.CanWrite)
                            return "Property is read only.";

                        if (p.PropertyType == typeof(Mobile))
                        {
                            p.SetValue(o, value, null);

                            return "Property has been set.";
                        }

                        return "Property is not of type Mobile.";
                    }
                }
            }

            return "Property not found.";
        }

        public static string GetPropertyValue(XmlSpawner spawner, object o, string name, out Type ptype)
        {
            ptype = null;
            if (o == null || name == null) return null;

            Type type = o.GetType();
            object po = null;

            PropertyInfo[] props;
            try
            {
                props = type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
            }
            catch
            {
                Console.WriteLine("GetProperties error with type {0}", type);
                return null;
            }

            // parse the strings of the form property.attribute into two parts
            // first get the property
            string[] arglist = ParseString(name, 2, ".");
            string propname = arglist[0];
            // parse up to 4 comma separated args for special keyword properties
            string[] keywordargs = ParseString(propname, 4, ",");

            if (keywordargs[0] == "SERIAL")
            {
                try
                {
                    if (o is Mobile)
                    {
                        ptype = ((Mobile)o).Serial.GetType();

                        return string.Format("Serial = {0}", ((Mobile)o).Serial);
                    }

                    if (o is Item)
                    {
                        ptype = ((Item)o).Serial.GetType();

                        return string.Format("Serial = {0}", ((Item)o).Serial);
                    }

                    return "Object is not item/mobile";
                }

                catch { return "Serial not found."; }
            }

            if (keywordargs[0] == "TYPE")
            {
                ptype = typeof(Type);

                return string.Format("Type = {0}", o.GetType().Name);

            }

            // do a bit of parsing to handle array references
            string[] arraystring = arglist[0].Split('[');
            int index = -1;
            if (arraystring.Length > 1)
            {
                // parse the property name from the indexing
                propname = arraystring[0];

                // then parse to get the index value
                string[] arrayvalue = arraystring[1].Split(']');

                if (arrayvalue.Length > 0)
                {
                    if (!int.TryParse(arrayvalue[0], out index))
                        index = -1;
                }
            }

            if (arglist.Length == 2)
            {
                // use the lookup table for optimization if possible
                PropertyInfo plookup = LookupPropertyInfo(spawner, type, propname);

                if (plookup != null)
                {
                    if (!plookup.CanRead)
                        return "Property is write only.";

                    ptype = plookup.PropertyType;
                    if (ptype.IsPrimitive)
                    {
                        po = plookup.GetValue(o, null);
                    }
                    else if ((ptype.GetInterface("IList") != null) && index >= 0)
                    {
                        try
                        {
                            object arrayvalue = plookup.GetValue(o, null);
                            po = ((IList<object>)arrayvalue)[index];
                        }
                        catch { }
                    }
                    else
                    {
                        po = plookup.GetValue(o, null);
                    }
                    // now set the nested attribute using the new property list
                    return (GetPropertyValue(spawner, po, arglist[1], out ptype));
                }

                // is a nested property with attributes so first get the property
                foreach (PropertyInfo p in props)
                {
                    //if ( Insensitive.Equals( p.Name, arglist[0] ) )
                    if (Insensitive.Equals(p.Name, propname))
                    {
                        if (!p.CanRead)
                            return "Property is write only.";

                        ptype = p.PropertyType;
                        if (ptype.IsPrimitive)
                        {
                            po = p.GetValue(o, null);
                        }
                        else if ((ptype.GetInterface("IList") != null) && index >= 0)
                        {
                            try
                            {
                                object arrayvalue = p.GetValue(o, null);
                                po = ((IList<object>)arrayvalue)[index];
                            }
                            catch { }
                        }
                        else
                        {
                            po = p.GetValue(o, null);
                        }
                        // now set the nested attribute using the new property list
                        return (GetPropertyValue(spawner, po, arglist[1], out ptype));
                    }
                }
            }
            else
            {
                // use the lookup table for optimization if possible
                PropertyInfo plookup = LookupPropertyInfo(spawner, type, propname);

                if (plookup != null)
                {
                    if (!plookup.CanRead)
                        return "Property is write only.";

                    ptype = plookup.PropertyType;

                    return InternalGetValue(o, plookup, index);
                }

                // its just a simple single property
                foreach (PropertyInfo p in props)
                {
                    //if ( Insensitive.Equals( p.Name, name ) )
                    if (Insensitive.Equals(p.Name, propname))
                    {
                        if (!p.CanRead)
                            return "Property is write only.";

                        ptype = p.PropertyType;

                        return InternalGetValue(o, p, index);
                    }
                }
            }

            return "Property not found.";
        }

        // added in arg parsing to handle object property setting
        public static bool ApplyObjectStringProperties(XmlSpawner spawner, string str, object o, Mobile trigmob, object refobject, out string status_str)
        {
            status_str = null;

            if (str == null || str.Length <= 0 || o == null) return false;

            // object strings will be of the form "object/modifier" where the modifier string is of the form "propname/value/propname/value/..."
            // some keywords do not have value arguments so the modifier could take the form "propname/propname/value/..."
            // this is handled by parsing into both forms

            // make sure the string is properly terminated to assure proper parsing of any final keywords
            bool terminated = false;
            str = str.Trim();

            if (str[str.Length - 1] != '/')
            {
                str += "/";
                terminated = true;
            }

            var arglist = ParseSlashArgs(str, 2);

            string remainder = null;

            // place the modifier section of the string in remainder
            if (arglist.Length > 1)
                remainder = arglist[1];

            bool no_error = true;

            // process the modifier string if there is anything
            while (arglist.Length > 1)
            {
                // place into arglist the parsed modifier up to this point
                // arglist[0] will contain the propname
                // arglist[1] will contain the value
                // arglist[2] will contain the reset of the modifier
                arglist = ParseSlashArgs(remainder, 3);

                // singlearglist will contain the propname and the remainder
                // for those keywords that do not have value args
                string[] singlearglist = ParseSlashArgs(remainder, 2);

                if (arglist.Length > 1)
                {
                    // handle value keywords that may take comma args

                    // itemarglist[1] will contain arg2/arg3/arg4>/arg5
                    // additemstr should have the full list of args <arg2/arg3/arg4>/arg5 if they are there.  In the case of /arg1/ADD/arg2
                    // it will just have arg2
                    string[] groupedarglist = ParseString(arglist[1], 2, "[");
                    string groupargstring = null;
                    if (groupedarglist.Length > 1)
                    {
                        // take that argument list that should like like arg2/ag3/arg4>/arg5
                        // need to find the matching ">"

                        string[] groupargs = ParseToMatchingParen(groupedarglist[1], '[', ']');

                        // and get the first part of the string without the >  so itemargs[0] should be arg2/ag3/arg4
                        groupargstring = groupargs[0];
                    }

                    // need to handle comma args that may be grouped with the () such as the (ATTACHMENT,args) arg

                    //string[] value_keywordargs = ParseString(groupedarglist[0],10,",");
                    string[] value_keywordargs = groupedarglist[0].Trim().Split(',');
                    if (!string.IsNullOrEmpty(groupargstring))
                    {

                        if (value_keywordargs != null && value_keywordargs.Length > 0)
                            value_keywordargs[value_keywordargs.Length - 1] = groupargstring;
                    }

                    // this quick optimization can determine whether this is a regular prop/value assignment
                    // since most prop modification strings will use regular propnames and not keywords, it makes sense to check for that first
                    if (value_keywordargs[0].Length > 0 && !char.IsUpper(value_keywordargs[0][0]) &&
                        arglist[0].Length > 0 && !char.IsUpper(arglist[0][0]))
                    {
                        // all of this code is also included in the keyword candidate tests
                        // this is because regular props can also be entered with uppercase so the lowercase test is not definitive
                        {
                            // check for the literal char
                            if (singlearglist[1] != null && singlearglist[1].Length > 0 && singlearglist[1][0] == '@')
                            {
                                //support for literal terminator
                                singlearglist = ParseLiteralTerminator(singlearglist[1]);
                                string lstr = singlearglist[0];
                                if (terminated && lstr[lstr.Length - 1] == '/')
                                    lstr = lstr.Remove(lstr.Length - 1, 1);

                                string result = SetPropertyValue(spawner, o, arglist[0], lstr.Remove(0, 1));

                                // see if it was successful
                                if (result != "Property has been set.")
                                {
                                    status_str = arglist[0] + " : " + result;
                                    no_error = false;
                                }
                                if (singlearglist.Length > 1 && singlearglist[1] != null)
                                {
                                    remainder = singlearglist[1];
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                string result = SetPropertyValue(spawner, o, arglist[0], arglist[1]);

                                // see if it was successful
                                if (result != "Property has been set.")
                                {
                                    status_str = arglist[0] + " : " + result;
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                        }
                    }
                    else
                    {
                        if (IsValuemodKeyword(value_keywordargs[0]))
                        {
                            valuemodKeyword kw = valuemodKeywordHash[value_keywordargs[0]];

                            if (kw == valuemodKeyword.INC)
                            {
                                // increment the property value by the amount.  Use the format propname/INC,min,max/ or propname/INC,value
                                if (value_keywordargs.Length > 1)
                                {
                                    // get a random number
                                    string incvalue = "0";
                                    if (value_keywordargs.Length > 2)
                                    {
                                        int min, max;
                                        if (int.TryParse(value_keywordargs[1], out min) && int.TryParse(value_keywordargs[2], out max))
                                        {
                                            incvalue = string.Format("{0}", Utility.RandomMinMax(min, max));
                                        }
                                        else { status_str = "Invalid INC args : " + arglist[1]; no_error = false; }
                                    }
                                    else
                                    {
                                        incvalue = value_keywordargs[1];
                                    }
                                    // get the current property value
                                    Type ptype;
                                    string tmpvalue = GetPropertyValue(spawner, o, arglist[0], out ptype);


                                    // see if it was successful
                                    if (ptype == null)
                                    {
                                        status_str = string.Format("Cant find {0}", arglist[0]);
                                        no_error = false;
                                    }
                                    else
                                    {
                                        string currentvalue = "0";
                                        try
                                        {
                                            string[] arglist2 = ParseString(tmpvalue, 2, "=");
                                            string[] arglist3 = ParseString(arglist2[1], 2, " ");
                                            currentvalue = arglist3[0].Trim();
                                        }
                                        catch { }
                                        string tmpstr = currentvalue;

                                        // should use the actual ptype info to do the addition.  Maybe later.
                                        double d0, d1;
                                        if (double.TryParse(currentvalue, NumberStyles.Any, CultureInfo.InvariantCulture, out d0) && double.TryParse(incvalue, NumberStyles.Any, CultureInfo.InvariantCulture, out d1))
                                        {
                                            tmpstr = ((int)(d0 + d1)).ToString();
                                        }
                                        else
                                        { status_str = "Invalid INC args : " + arglist[1]; no_error = false; }

                                        // set the property value using the incremented value
                                        string result = SetPropertyValue(spawner, o, arglist[0], tmpstr);
                                        // see if it was successful
                                        if (result != "Property has been set.")
                                        {
                                            status_str = arglist[0] + " : " + result;
                                            no_error = false;
                                        }
                                    }
                                }
                                else
                                {
                                    status_str = "Invalid INC args : " + arglist[1];
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.MOB)
                            {
                                // lookup the mob id based on the name. format is /MOB,name[,type]/
                                if (value_keywordargs.Length > 1)
                                {
                                    string typestr = null;
                                    if (value_keywordargs.Length > 2)
                                    {
                                        typestr = value_keywordargs[2];
                                    }
                                    // lookup the name
                                    Mobile mob_id = null;
                                    try
                                    {
                                        mob_id = FindMobileByName(spawner, value_keywordargs[1], typestr); // the format of this will be 0xvalue "name"
                                    }
                                    catch { status_str = "Invalid MOB args : " + arglist[1]; no_error = false; }
                                    // set the property value using this format (M) id name

                                    string result = SetPropertyObject(spawner, o, arglist[0], mob_id);

                                    // see if it was successful
                                    if (result != "Property has been set.")
                                    {
                                        status_str = arglist[0] + " : " + result;
                                        no_error = false;
                                    }
                                }
                                else
                                {
                                    no_error = false;
                                }

                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.TRIGMOB)
                            {
                                string result = SetPropertyObject(spawner, o, arglist[0], trigmob);
                                // see if it was successful
                                if (result != "Property has been set.")
                                {
                                    status_str = arglist[0] + " : " + result;
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.PLAYERSINRANGE)
                            {
                                // syntax is PLAYERSINRANGE,range
                                int nplayers = 0;
                                int range = 0;
                                // get the number of players in range
                                if (value_keywordargs.Length > 1)
                                {
                                    int.TryParse(value_keywordargs[1], out range);
                                }

                                // count nearby players
                                if (refobject is Item)
                                {
                                    IPooledEnumerable ie = ((Item)refobject).GetMobilesInRange(range);
                                    foreach (Mobile p in ie)
                                    {
                                        if (p.Player && p.AccessLevel == AccessLevel.Player) nplayers++;
                                    }
                                    ie.Free();
                                }
                                else if (refobject is Mobile)
                                {
                                    IPooledEnumerable ie = ((Mobile)refobject).GetMobilesInRange(range);
                                    foreach (Mobile p in ie)
                                    {
                                        if (p.Player && p.AccessLevel == AccessLevel.Player) nplayers++;
                                    }
                                    ie.Free();
                                }

                                string result = SetPropertyValue(spawner, o, arglist[0], nplayers.ToString());

                                // see if it was successful
                                if (result != "Property has been set.")
                                {
                                    status_str = arglist[0] + " : " + result;
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                        }
                        else
                        {
                            // check for the literal char
                            if (singlearglist[1] != null && singlearglist[1].Length > 0 && singlearglist[1][0] == '@')
                            {
                                //support for literal terminator
                                singlearglist = ParseLiteralTerminator(singlearglist[1]);
                                string lstr = singlearglist[0];
                                if (terminated && lstr[lstr.Length - 1] == '/')
                                    lstr = lstr.Remove(lstr.Length - 1, 1);

                                string result = SetPropertyValue(spawner, o, arglist[0], lstr.Remove(0, 1));
                                // see if it was successful
                                if (result != "Property has been set.")
                                {
                                    status_str = arglist[0] + " : " + result;
                                    no_error = false;
                                }
                                if (singlearglist.Length > 1 && singlearglist[1] != null)
                                {
                                    remainder = singlearglist[1];
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                string result = SetPropertyValue(spawner, o, arglist[0], arglist[1]);
                                // see if it was successful
                                if (result != "Property has been set.")
                                {
                                    status_str = arglist[0] + " : " + result;
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                        }
                    }
                }
            }
            return (no_error);
        }
        #endregion

        #region Property testing
        public static bool TestMobProperty(XmlSpawner spawner, Mobile mobile, string testString, out string status_str)
        {
            status_str = null;
            // now make sure the mobile itself is there
            if (mobile == null || mobile.Deleted)
            {
                return false;
            }

            bool testreturn = CheckPropertyString(spawner, mobile, testString, out status_str);

            return testreturn;
        }

        public static bool TestItemProperty(XmlSpawner spawner, Item ObjectPropertyItem, string testString, out string status_str)
        {
            // now make sure the item itself is there
            if (ObjectPropertyItem == null || ObjectPropertyItem.Deleted)
            {
                status_str = "Trigger Object not found";
                return false;
            }

            bool testreturn = CheckPropertyString(spawner, ObjectPropertyItem, testString, out status_str);

            return testreturn;
        }

        public static PropertyInfo LookupPropertyInfo(XmlSpawner spawner, Type type, string propname)
        {
            if (spawner == null || type == null || propname == null) return null;

            // look up the info in the current list

            if (spawner.PropertyInfoList == null) spawner.PropertyInfoList = new List<TypeInfo>();

            PropertyInfo pinfo = null;
            TypeInfo tinfo = null;

            foreach (TypeInfo to in spawner.PropertyInfoList)
            {
                // check the type
                if (to.t == type)
                {
                    // found it
                    tinfo = to;

                    // now search the property list
                    foreach (PropertyInfo p in to.plist)
                    {
                        if (Insensitive.Equals(p.Name, propname))
                        {
                            pinfo = p;
                        }
                    }
                }
            }

            // did we find the property?
            if (pinfo != null)
            {
                return pinfo;
            }
            // if it cant be found, then do the full search and add it to the list

            PropertyInfo[] props = type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo p in props)
            {
                if (Insensitive.Equals(p.Name, propname))
                {
                    // did we find the type at least?
                    if (tinfo == null)
                    {
                        // if not then add the type to the list
                        tinfo = new TypeInfo
                        {
                            t = type
                        };

                        spawner.PropertyInfoList.Add(tinfo);
                    }

                    // and add the property to the tinfo property list
                    tinfo.plist.Add(p);
                    return p;
                }
            }

            return null;
        }

        public static string ParseForKeywords(XmlSpawner spawner, object o, string valstr, bool literal, out Type ptype)
        {
            ptype = null;

            if (valstr == null || valstr.Length <= 0) return null;

            string str = valstr.Trim();

            // look for keywords
            // need to handle the case of nested arglists like arg,arg,<arg,arg>
            // handle value keywords that may take comma args

            // itemarglist[1] will contain arg2/arg3/arg4>/arg5
            // additemstr should have the full list of args <arg2/arg3/arg4>/arg5 if they are there.  In the case of /arg1/ADD/arg2
            // it will just have arg2
            string[] groupedarglist = ParseString(str, 2, "[");
            string groupargstring = null;
            if (groupedarglist.Length > 1)
            {
                // take that argument list that should like like arg2/ag3/arg4>/arg5
                // need to find the matching ">"

                string[] groupargs = ParseToMatchingParen(groupedarglist[1], '[', ']');

                // and get the first part of the string without the >  so itemargs[0] should be arg2/ag3/arg4
                groupargstring = groupargs[0];
            }

            // need to handle comma args that may be grouped with the () such as the (ATTACHMENT,args) arg
            string[] arglist = groupedarglist[0].Trim().Split(',');

            if (!string.IsNullOrEmpty(groupargstring) && arglist.Length > 0)
            {
                arglist[arglist.Length - 1] = groupargstring;
            }


            string pname = arglist[0].Trim();
            char startc = str[0];

            // first see whether it is a standard numeric value
            if ((startc == '.') || (startc == '-') || (startc == '+') || (startc >= '0' && startc <= '9'))
            {
                // determine the type
                ptype = str.IndexOf(".") >= 0 ? typeof(double) : typeof(int);

                return str;
            }

            if (startc == '"' || startc == '(')
            {
                ptype = typeof(string);
                return str;
            }

            if (startc == '#')
            {
                ptype = typeof(string);
                return str.Substring(1);
            }
            // or a bool

            if ((str.ToLower()) == "true" || (str.ToLower() == "false"))
            {
                ptype = typeof(bool);
                return str;
            }
            // then look for a keyword

            if (IsValueKeyword(pname))
            {
                valueKeyword kw = valueKeywordHash[pname];

                if ((kw == valueKeyword.PLAYERSINRANGE) && arglist.Length > 1)
                {
                    // syntax is PLAYERSINRANGE,range

                    ptype = typeof(int);

                    int nplayers = 0;
                    int range;
                    // get the number of players in range
                    int.TryParse(arglist[1], out range);

                    // count nearby players
                    if (spawner?.SpawnRegion != null && range < 0)
                    {
                        foreach (Mobile p in spawner.SpawnRegion.GetPlayers())
                        {
                            if (p.AccessLevel <= spawner.TriggerAccessLevel) nplayers++;
                        }
                    }
                    else if (o is Item)
                    {
                        IPooledEnumerable ie = ((Item)o).GetMobilesInRange(range);
                        foreach (Mobile p in ie)
                        {
                            if (p.Player && p.AccessLevel == AccessLevel.Player) nplayers++;
                        }
                        ie.Free();
                    }
                    else if (o is Mobile)
                    {
                        IPooledEnumerable ie = ((Mobile)o).GetMobilesInRange(range);
                        foreach (Mobile p in ie)
                        {
                            if (p.Player && p.AccessLevel == AccessLevel.Player) nplayers++;
                        }
                        ie.Free();
                    }

                    return nplayers.ToString();
                }
                if ((kw == valueKeyword.RANDNAME) && arglist.Length > 1)
                {
                    // syntax is RANDNAME,nametype
                    return NameList.RandomName(arglist[1]);
                }

                // an invalid keyword format will be passed as literal
                return str;
            }

            if (literal)
            {
                ptype = typeof(string);
                return str;
            }

            // otherwise treat it as a property name
            string result = GetPropertyValue(spawner, o, pname, out ptype);

            return ParseGetValue(result, ptype);
        }

        public static string ParseGetValue(string str, Type ptype)
        {
            // the results of getPropertyValue takes the form
            // propname = value
            // or
            // propname = value (hexvalue)

            if (str == null)
                return null;

            // find the separator
            string[] arglist = str.Split("=".ToCharArray(), 2);

            if (arglist.Length > 1)
            {
                if (IsNumeric(ptype))
                {
                    // parse the value portion and get rid of the possible (hexvalue) portion of the string
                    string[] arglist2 = arglist[1].Trim().Split(" ".ToCharArray(), 2);

                    return arglist2[0];
                }

                // for everything else
                // pass on as is
                return arglist[1].Trim();
            }

            return null;
        }

        public static bool CheckPropertyString(XmlSpawner spawner, object o, string testString, out string status_str)
        {
            status_str = null;

            if (o == null)
                return false;

            if (string.IsNullOrEmpty(testString))
            {
                status_str = "Null property test string";
                return false;
            }
            // parse the property test string for and(&)/or(|) operators
            string[] arglist = ParseString(testString, 2, "&|");
            if (arglist.Length < 2)
            {
                bool returnval = CheckSingleProperty(spawner, o, testString, out status_str);

                // simple conditional test with no and/or operators
                return returnval;
            }

            // test each half independently and combine the results
            bool first = CheckSingleProperty(spawner, o, arglist[0], out status_str);

            // this will recursively parse the property test string with implicit nesting for multiple logical tests of the
            // form A * B * C * D    being grouped as A * (B * (C * D))
            bool second = CheckPropertyString(spawner, o, arglist[1], out status_str);

            int andposition = testString.IndexOf("&");
            int orposition = testString.IndexOf("|");

            // combine them based upon the operator
            if ((andposition > 0 && orposition <= 0) || (andposition > 0 && andposition < orposition))
            {
                // and operator
                return (first && second);
            }

            if ((orposition > 0 && andposition <= 0) || (orposition > 0 && orposition < andposition))
            {
                // or operator
                return (first || second);
            }

            // should never get here
            return false;
        }

        public static bool CheckSingleProperty(XmlSpawner spawner, object o, string testString, out string status_str)
        {
            status_str = null;

            if (o == null || testString == null || testString.Length == 0) return false;

            //get the prop name and test value
            // format will be prop=prop, or prop>prop, prop<prop, prop!prop
            // also support the 'not' operator ~ at the beginning of a test, like ~prop=prop
            testString = testString.Trim();

            bool invertreturn = false;

            if (testString.Length > 0 && testString[0] == '~')
            {
                invertreturn = true;
                testString = testString.Substring(1, testString.Length - 1);
            }

            string[] arglist = ParseString(testString, 2, "=><!");
            if (arglist.Length < 2)
            {
                status_str = "invalid property string : " + testString;
                return false;
            }
            bool hasequal = false;
            bool hasnotequals = false;
            bool hasgreaterthan = false;
            bool haslessthan = false;

            if (testString.IndexOf("=") > 0)
            {
                hasequal = true;
            }
            else
                if (testString.IndexOf("!") > 0)
            {
                hasnotequals = true;
            }
            else
                    if (testString.IndexOf(">") > 0)
            {
                hasgreaterthan = true;
            }
            else
                        if (testString.IndexOf("<") > 0)
            {
                haslessthan = true;
            }

            // does it have a valid operator?
            if (!hasequal && !hasgreaterthan && !haslessthan && !hasnotequals)
                return false;

            Type ptype1;
            Type ptype2;

            string value1 = ParseForKeywords(spawner, o, arglist[0].Trim(), false, out ptype1);

            // see if it was successful
            if (ptype1 == null)
            {
                status_str = arglist[0] + " : " + value1;

                return invertreturn;
                //return false;
            }

            string value2 = ParseForKeywords(spawner, o, arglist[1].Trim(), false, out ptype2);

            // see if it was successful
            if (ptype2 == null)
            {
                status_str = arglist[1] + " : " + value2;

                return invertreturn;
                //return false;
            }

            // look for hex numeric specifications
            int base1 = 10;
            int base2 = 10;
            if (IsNumeric(ptype1) && !string.IsNullOrEmpty(value1) && value1.StartsWith("0x"))
            {
                base1 = 16;
            }

            if (IsNumeric(ptype2) && !string.IsNullOrEmpty(value2) && value2.StartsWith("0x"))
            {
                base2 = 16;
            }

            // and do the type dependent comparisons
            if (ptype2 == typeof(TimeSpan) || ptype1 == typeof(TimeSpan))
            {
                if (hasequal)
                {
                    TimeSpan ts1, ts2;
                    if (TimeSpan.TryParse(value1, out ts1) && TimeSpan.TryParse(value2, out ts2))
                    {
                        if (ts1 == ts2) return !invertreturn;
                    }
                    else
                    {
                        status_str = "invalid timespan comparison : {0}" + testString;
                    }
                }
                else if (hasnotequals)
                {
                    TimeSpan ts1, ts2;
                    if (TimeSpan.TryParse(value1, out ts1) && TimeSpan.TryParse(value2, out ts2))
                    {
                        if (ts1 != ts2) return !invertreturn;
                    }
                    else
                    {
                        status_str = "invalid timespan comparison : {0}" + testString;
                    }
                }
                else if (hasgreaterthan)
                {
                    TimeSpan ts1, ts2;
                    if (TimeSpan.TryParse(value1, out ts1) && TimeSpan.TryParse(value2, out ts2))
                    {
                        if (ts1 > ts2) return !invertreturn;
                    }
                    else
                    {
                        status_str = "invalid timespan comparison : {0}" + testString;
                    }
                }
                else
                {
                    TimeSpan ts1, ts2;
                    if (TimeSpan.TryParse(value1, out ts1) && TimeSpan.TryParse(value2, out ts2))
                    {
                        if (ts1 < ts2) return !invertreturn;
                    }
                    else
                    {
                        status_str = "invalid timespan comparison : {0}" + testString;
                    }
                }
            }
            else
                // and do the type dependent comparisons
                if (ptype2 == typeof(DateTime) || ptype1 == typeof(DateTime))
            {
                if (hasequal)
                {
                    DateTime dt1, dt2;
                    if (DateTime.TryParse(value1, out dt1) && DateTime.TryParse(value2, out dt2))
                    {
                        if (dt1 == dt2) return !invertreturn;
                    }
                    else
                    {
                        status_str = "invalid DateTime comparison : {0}" + testString;
                    }
                }
                else if (hasnotequals)
                {
                    DateTime dt1, dt2;
                    if (DateTime.TryParse(value1, out dt1) && DateTime.TryParse(value2, out dt2))
                    {
                        if (dt1 != dt2) return !invertreturn;
                    }
                    else
                    {
                        status_str = "invalid DateTime comparison : {0}" + testString;
                    }
                }
                else if (hasgreaterthan)
                {
                    DateTime dt1, dt2;
                    if (DateTime.TryParse(value1, out dt1) && DateTime.TryParse(value2, out dt2))
                    {
                        if (dt1 > dt2) return !invertreturn;
                    }
                    else
                    {
                        status_str = "invalid DateTime comparison : {0}" + testString;
                    }
                }
                else
                {
                    DateTime dt1, dt2;
                    if (DateTime.TryParse(value1, out dt1) && DateTime.TryParse(value2, out dt2))
                    {
                        if (dt1 < dt2) return !invertreturn;
                    }
                    else
                    {
                        status_str = "invalid DateTime comparison : {0}" + testString;
                    }
                }
            }
            else if (IsNumeric(ptype2) && IsNumeric(ptype1))
            {
                if (hasequal)
                {
                    try
                    {
                        if (Convert.ToInt64(value1, base1) == Convert.ToInt64(value2, base2)) return !invertreturn;
                    }
                    catch
                    {
                        status_str = "invalid int comparison : {0}" + testString;
                    }
                }
                else if (hasnotequals)
                {
                    try
                    {
                        if (Convert.ToInt64(value1, base1) != Convert.ToInt64(value2, base2)) return !invertreturn;
                    }
                    catch
                    {
                        status_str = "invalid int comparison : {0}" + testString;
                    }
                }
                else if (hasgreaterthan)
                {
                    try
                    {
                        if (Convert.ToInt64(value1, base1) > Convert.ToInt64(value2, base2)) return !invertreturn;
                    }
                    catch { status_str = "invalid int comparison : {0}" + testString; }
                }
                else
                {
                    try
                    {
                        if (Convert.ToInt64(value1, base1) < Convert.ToInt64(value2, base2)) return !invertreturn;
                    }
                    catch { status_str = "invalid int comparison : {0}" + testString; }
                }
            }
            else if ((ptype2 == typeof(double)) && IsNumeric(ptype1))
            {
                if (hasequal)
                {
                    try
                    {
                        if (Convert.ToInt64(value1, base1) == double.Parse(value2)) return !invertreturn;
                    }
                    catch
                    {
                        status_str = "invalid int comparison : {0}" + testString;
                    }
                }
                else if (hasnotequals)
                {
                    try
                    {
                        if (Convert.ToInt64(value1, base1) != double.Parse(value2)) return !invertreturn;
                    }
                    catch
                    {
                        status_str = "invalid int comparison : {0}" + testString;
                    }
                }
                else if (hasgreaterthan)
                {
                    try
                    {
                        if (Convert.ToInt64(value1, base1) > double.Parse(value2)) return !invertreturn;
                    }
                    catch { status_str = "invalid int comparison : {0}" + testString; }
                }
                else
                {
                    try
                    {
                        if (Convert.ToInt64(value1, base1) < double.Parse(value2)) return !invertreturn;
                    }
                    catch { status_str = "invalid int comparison : {0}" + testString; }
                }
            }
            else if ((ptype1 == typeof(double)) && IsNumeric(ptype2))
            {
                if (hasequal)
                {
                    try
                    {
                        if (double.Parse(value1) == Convert.ToInt64(value2, base2)) return !invertreturn;
                    }
                    catch
                    {
                        status_str = "invalid int comparison : {0}" + testString;
                    }
                }
                else if (hasnotequals)
                {
                    try
                    {
                        if (double.Parse(value1) != Convert.ToInt64(value2, base2)) return !invertreturn;
                    }
                    catch
                    {
                        status_str = "invalid int comparison : {0}" + testString;
                    }
                }
                else if (hasgreaterthan)
                {
                    try
                    {
                        if (double.Parse(value1) > Convert.ToInt64(value2, base2)) return !invertreturn;
                    }
                    catch { status_str = "invalid int comparison : {0}" + testString; }
                }
                else
                {
                    try
                    {
                        if (double.Parse(value1) < Convert.ToInt64(value2, base2)) return !invertreturn;
                    }
                    catch { status_str = "invalid int comparison : {0}" + testString; }
                }
            }
            else if ((ptype1 == typeof(double)) && (ptype2 == typeof(double)))
            {
                double val1;
                double val2;
                if (hasequal)
                {
                    if (double.TryParse(value1, NumberStyles.Any, CultureInfo.InvariantCulture, out val1) && double.TryParse(value2, NumberStyles.Any, CultureInfo.InvariantCulture, out val2))
                    {
                        if (val1 == val2)
                            return !invertreturn;
                    }
                    else
                    {
                        status_str = "invalid int comparison : {0}" + testString;
                    }
                }
                else if (hasnotequals)
                {
                    if (double.TryParse(value1, NumberStyles.Any, CultureInfo.InvariantCulture, out val1) && double.TryParse(value2, NumberStyles.Any, CultureInfo.InvariantCulture, out val2))
                    {
                        if (val1 != val2)
                            return !invertreturn;
                    }
                    else
                    {
                        status_str = "invalid int comparison : {0}" + testString;
                    }
                }
                else if (hasgreaterthan)
                {
                    if (double.TryParse(value1, NumberStyles.Any, CultureInfo.InvariantCulture, out val1) && double.TryParse(value2, NumberStyles.Any, CultureInfo.InvariantCulture, out val2))
                    {
                        if (val1 > val2)
                            return !invertreturn;
                    }
                    else { status_str = "invalid int comparison : {0}" + testString; }
                }
                else
                {
                    if (double.TryParse(value1, NumberStyles.Any, CultureInfo.InvariantCulture, out val1) && double.TryParse(value2, NumberStyles.Any, CultureInfo.InvariantCulture, out val2))
                    {
                        if (val1 < val2)
                            return !invertreturn;
                    }
                    else { status_str = "invalid int comparison : {0}" + testString; }
                }
            }
            else if (ptype2 == typeof(bool) && ptype1 == typeof(bool))
            {
                bool val1, val2;
                if (hasequal)
                {
                    if (bool.TryParse(value1, out val1) && bool.TryParse(value2, out val2))
                    {
                        if (val1 == val2) return !invertreturn;
                    }
                    else { status_str = "invalid bool comparison : {0}" + testString; }
                }
                else if (hasnotequals)
                {
                    if (bool.TryParse(value1, out val1) && bool.TryParse(value2, out val2))
                    {
                        if (val1 != val2) return !invertreturn;
                    }
                    else { status_str = "invalid bool comparison : {0}" + testString; }
                }
            }
            else if (ptype2 == typeof(double) || ptype2 == typeof(double))
            {
                double val1;
                double val2;
                if (hasequal)
                {
                    if (double.TryParse(value1, NumberStyles.Any, CultureInfo.InvariantCulture, out val1) && double.TryParse(value2, NumberStyles.Any, CultureInfo.InvariantCulture, out val2))
                    {
                        if (val1 == val2) return !invertreturn;
                    }
                    else { status_str = "invalid double comparison : {0}" + testString; }
                }
                else if (hasnotequals)
                {
                    if (double.TryParse(value1, NumberStyles.Any, CultureInfo.InvariantCulture, out val1) && double.TryParse(value2, NumberStyles.Any, CultureInfo.InvariantCulture, out val2))
                    {
                        if (val1 != val2) return !invertreturn;
                    }
                    else { status_str = "invalid double comparison : {0}" + testString; }
                }
                else if (hasgreaterthan)
                {
                    if (double.TryParse(value1, NumberStyles.Any, CultureInfo.InvariantCulture, out val1) && double.TryParse(value2, NumberStyles.Any, CultureInfo.InvariantCulture, out val2))
                    {
                        if (val1 > val2) return !invertreturn;
                    }
                    else { status_str = "invalid double comparison : {0}" + testString; }
                }
                else
                {
                    if (double.TryParse(value1, NumberStyles.Any, CultureInfo.InvariantCulture, out val1) && double.TryParse(value2, NumberStyles.Any, CultureInfo.InvariantCulture, out val2))
                    {
                        if (val1 < val2) return !invertreturn;
                    }
                    else { status_str = "invalid double comparison : {0}" + testString; }
                }
            }
            else
            {
                // by default just do a string comparison
                if (hasequal)
                {
                    if (value1 == value2) return !invertreturn;
                }
                else
                    if (hasnotequals)
                {
                    if (value1 != value2) return !invertreturn;
                }
            }
            return invertreturn;
        }
        #endregion

        #region Search object methods
        public static Item FindItemByName(XmlSpawner fromspawner, string name, string typestr)
        {
            if (name == null) return null;

            int count = 0;

            Item founditem = FindInRecentItemSearchList(fromspawner, name, typestr);

            if (founditem != null)
            {
                return founditem;
            }

            Type targettype = null;
            if (typestr != null)
            {
                targettype = SpawnerType.GetType(typestr);
            }

            // search through all items in the world and find the first one with a matching name
            foreach (Item item in World.Items.Values)
            {
                Type itemtype = item.GetType();

                if (!item.Deleted && (name.Length == 0 || string.Compare(item.Name, name, true) == 0))
                {

                    if (typestr == null ||
                        targettype != null && (itemtype.Equals(targettype) || itemtype.IsSubclassOf(targettype)))
                    {
                        founditem = item;
                        count++;
                        // added the break in to return the first match instead of forcing uniqueness (overrides the count test)
                        break;
                    }
                }
            }

            if (count == 1) // if a unique item is found then success
            {
                // add this to the recent search list
                AddToRecentItemSearchList(fromspawner, founditem);

                return (founditem);
            }

            return null;
        }

        public static Mobile FindMobileByName(XmlSpawner fromspawner, string name, string typestr)
        {
            if (name == null) return null;

            int count = 0;

            Mobile foundmobile = FindInRecentMobileSearchList(fromspawner, name, typestr);

            if (foundmobile != null) return foundmobile;

            Type targettype = null;
            if (typestr != null)
            {
                targettype = SpawnerType.GetType(typestr);
            }

            // search through all mobiles in the world and find one with a matching name
            foreach (Mobile mobile in World.Mobiles.Values)
            {
                Type mobtype = mobile.GetType();
                if (!mobile.Deleted && ((name.Length == 0 || string.Compare(mobile.Name, name, true) == 0)) && (typestr == null ||
                    targettype != null && (mobtype.Equals(targettype) || mobtype.IsSubclassOf(targettype))))
                {
                    foundmobile = mobile;
                    count++;
                    // added the break in to return the first match instead of forcing uniqueness (overrides the count test)
                    break;
                }
            }

            // if a unique item is found then success
            if (count == 1)
            {
                // add this to the recent search list
                AddToRecentMobileSearchList(fromspawner, foundmobile);

                return (foundmobile);
            }

            return null;
        }

        public static XmlSpawner FindSpawnerByName(XmlSpawner fromspawner, string name)
        {
            if (name == null)
                return null;

            if (name.StartsWith("0x"))
            {
                int serial = -1;
                try
                {
                    serial = Convert.ToInt32(name, 16);
                }
                catch { }
                return World.FindEntity(serial) as XmlSpawner;
            }

            // do a quick search through the recent search list to see if it is there
            XmlSpawner foundspawner = FindInRecentSpawnerSearchList(fromspawner, name);

            if (foundspawner != null) return foundspawner;

            int count = 0;

            // search through all xmlspawners in the world and find one with a matching name
            foreach (Item item in World.Items.Values)
            {
                if (item is XmlSpawner)
                {
                    XmlSpawner spawner = (XmlSpawner)item;
                    if (!spawner.Deleted && (string.Compare(spawner.Name, name, true) == 0))
                    {
                        foundspawner = spawner;

                        count++;
                        // added the break in to return the first match instead of forcing uniqueness (overrides the count test)
                        break;
                    }
                }
            }

            // if a unique item is found then success
            if (count == 1)
            {
                // add this to the recent search list
                AddToRecentSpawnerSearchList(fromspawner, foundspawner);

                return (foundspawner);
            }

            return null;
        }

        public static void AddToRecentSpawnerSearchList(XmlSpawner spawner, XmlSpawner target)
        {
            if (spawner == null || target == null) return;

            if (spawner.RecentSpawnerSearchList == null)
            {
                spawner.RecentSpawnerSearchList = new List<XmlSpawner>();
            }
            spawner.RecentSpawnerSearchList.Add(target);

            // check the length and truncate if it gets too long
            if (spawner.RecentSpawnerSearchList.Count > 100)
            {
                spawner.RecentSpawnerSearchList.RemoveAt(0);
            }
        }

        public static XmlSpawner FindInRecentSpawnerSearchList(XmlSpawner spawner, string name)
        {
            if (spawner == null || name == null || spawner.RecentSpawnerSearchList == null) return null;

            List<XmlSpawner> deletelist = null;
            XmlSpawner foundspawner = null;

            foreach (XmlSpawner s in spawner.RecentSpawnerSearchList)
            {
                if (s.Deleted)
                {
                    // clean it up
                    if (deletelist == null)
                        deletelist = new List<XmlSpawner>();
                    deletelist.Add(s);
                }
                else
                    if (string.Compare(s.Name, name, true) == 0)
                {
                    foundspawner = s;
                    break;
                }
            }

            if (deletelist != null)
            {
                foreach (XmlSpawner i in deletelist)
                    spawner.RecentSpawnerSearchList.Remove(i);
            }

            return (foundspawner);
        }

        public static void AddToRecentItemSearchList(XmlSpawner spawner, Item target)
        {
            if (spawner == null || target == null) return;

            if (spawner.RecentItemSearchList == null)
            {
                spawner.RecentItemSearchList = new List<Item>();
            }

            spawner.RecentItemSearchList.Add(target);

            // check the length and truncate if it gets too long
            if (spawner.RecentItemSearchList.Count > 100)
            {
                spawner.RecentItemSearchList.RemoveAt(0);
            }
        }

        public static Item FindInRecentItemSearchList(XmlSpawner spawner, string name, string typestr)
        {
            if (spawner == null || name == null || spawner.RecentItemSearchList == null) return null;

            List<Item> deletelist = null;
            Item founditem = null;

            Type targettype = null;
            if (typestr != null)
            {
                targettype = SpawnerType.GetType(typestr);
            }

            foreach (Item item in spawner.RecentItemSearchList)
            {
                if (item.Deleted)
                {
                    // clean it up
                    if (deletelist == null)
                        deletelist = new List<Item>();
                    deletelist.Add(item);
                }
                else
                    if (name.Length == 0 || string.Compare(item.Name, name, true) == 0)
                {
                    if (typestr == null ||
                        targettype != null && (item.GetType().Equals(targettype) || item.GetType().IsSubclassOf(targettype)))
                    {
                        founditem = item;
                        break;
                    }
                }
            }

            if (deletelist != null)
            {
                foreach (Item i in deletelist)
                    spawner.RecentItemSearchList.Remove(i);
            }

            return (founditem);
        }

        public static void AddToRecentMobileSearchList(XmlSpawner spawner, Mobile target)
        {
            if (spawner == null || target == null) return;

            if (spawner.RecentMobileSearchList == null)
            {
                spawner.RecentMobileSearchList = new List<Mobile>();
            }

            spawner.RecentMobileSearchList.Add(target);

            // check the length and truncate if it gets too long
            if (spawner.RecentMobileSearchList.Count > 100)
            {
                spawner.RecentMobileSearchList.RemoveAt(0);
            }
        }

        public static Mobile FindInRecentMobileSearchList(XmlSpawner spawner, string name, string typestr)
        {
            if (spawner == null || name == null || spawner.RecentMobileSearchList == null) return null;

            List<Mobile> deletelist = null;
            Mobile foundmobile = null;

            Type targettype = null;
            if (typestr != null)
            {
                targettype = SpawnerType.GetType(typestr);
            }

            foreach (Mobile m in spawner.RecentMobileSearchList)
            {
                if (m.Deleted)
                {
                    // clean it up
                    if (deletelist == null)
                        deletelist = new List<Mobile>();
                    deletelist.Add(m);
                }
                else
                    if (name.Length == 0 || string.Compare(m.Name, name, true) == 0)
                {

                    if (typestr == null ||
                        targettype != null && (m.GetType().Equals(targettype) || m.GetType().IsSubclassOf(targettype)))
                    {
                        foundmobile = m;
                        break;
                    }
                }
            }

            if (deletelist != null)
            {
                foreach (Mobile i in deletelist)
                    spawner.RecentMobileSearchList.Remove(i);
            }

            return (foundmobile);
        }
        #endregion

        #region String parsing methods
        public static string ApplySubstitution(XmlSpawner spawner, object o, string typeName)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // go through the string looking for instances of {keyword}
            string remaining = typeName;

            while (!string.IsNullOrEmpty(remaining))
            {

                int startindex = remaining.IndexOf('{');

                if (startindex == -1 || startindex + 1 >= remaining.Length)
                {
                    // if there are no more delimiters then append the remainder and finish
                    sb.Append(remaining);
                    break;
                }

                // might be a substitution, check for keywords
                int endindex = remaining.Substring(startindex + 1).IndexOf("}");

                // if the ending delimiter cannot be found then just append and finish
                if (endindex == -1)
                {
                    sb.Append(remaining);
                    break;
                }

                // get the string up to the delimiter
                string firstpart = remaining.Substring(0, startindex);
                sb.Append(firstpart);

                string keypart = remaining.Substring(startindex + 1, endindex);

                // try to evaluate and then substitute the arg
                Type ptype;

                string value = ParseForKeywords(spawner, o, keypart.Trim(), true, out ptype);

                // trim off the " from strings
                if (value != null)
                {
                    value = value.Trim('"');
                }

                // replace the parsed value for the keyword
                sb.Append(value);

                // continue processing the rest of the string
                if (endindex + startindex + 2 >= remaining.Length) break;

                remaining = remaining.Substring(endindex + startindex + 2, remaining.Length - endindex - startindex - 2);
            }
            return sb.ToString();
        }

        public static string ParseObjectType(string str)
        {
            string[] arglist = ParseSlashArgs(str, 2);
            if (arglist != null && arglist.Length > 0)
            {
                // parse out any arguments of the form typename,arg,arg,..
                string[] typeargs = ParseCommaArgs(arglist[0], 2);
                if (typeargs.Length > 1)
                {
                    return (typeargs[0]);
                }
                return arglist[0];
            }

            return null;
        }

        public static string[] ParseObjectArgs(string str)
        {
            string[] arglist = ParseSlashArgs(str, 2);
            if (arglist.Length > 0)
            {
                string itemtypestring = arglist[0];
                // parse out any arguments of the form typename,arg,arg,..
                // find the first arg if it is there
                string[] typeargs = null;
                int argstart = 0;
                if (!string.IsNullOrEmpty(itemtypestring))
                    argstart = itemtypestring.IndexOf(",") + 1;

                if (argstart > 1 && argstart < itemtypestring.Length)
                {
                    typeargs = ParseCommaArgs(itemtypestring.Substring(argstart), 15);
                }
                return (typeargs);

            }

            return null;
        }

        // take a string of the form str-opendelim-str-closedelim-str-closedelimstr
        public static string[] ParseToMatchingParen(string str, char opendelim, char closedelim)
        {
            int nopen = 1;
            int nclose = 0;
            int splitpoint = str.Length;
            for (int i = 0; i < str.Length; i++)
            {
                // walk through the string until a matching close delimstr is found
                if (str[i] == opendelim) nopen++;
                if (str[i] == closedelim) nclose++;

                if (nopen == nclose)
                {
                    splitpoint = i;
                    break;
                }
            }

            string[] args = new string[2];

            // allow missing closing delimiters at the end of the line, basically just treat eol as a closing delim

            args[0] = str.Substring(0, splitpoint);
            args[1] = "";
            if (splitpoint + 1 < str.Length)
            {
                args[1] = str.Substring(splitpoint + 1, str.Length - splitpoint - 1);
            }

            return args;
        }

        public static string[] ParseString(string str, int nitems, string delimstr)
        {
            if (str == null || delimstr == null) return null;

            char[] delims = delimstr.ToCharArray();
            str = str.Trim();
            string[] args = str.Split(delims, nitems);

            return args;
        }

        public static string[] ParseSlashArgs(string str, int nitems)
        {
            if (str == null) return null;
            str = str.Trim();


            string[] args;
            // this supports strings that may have special html formatting in them that use the /
            if (str.IndexOf("</") >= 0 || str.IndexOf("/>") >= 0)
            {
                // or use indexof to do it with more context control
                List<string> tmparray = new List<string>();
                // find the next slash char
                int index = 0;
                int preindex = 0;
                int searchindex = 0;
                int length = str.Length;
                while (index >= 0 && searchindex < length && tmparray.Count < nitems - 1)
                {
                    index = str.IndexOf('/', searchindex);

                    if (index >= 0)
                    {
                        // check the char before it and after it to ignore </ and />
                        if ((index > 0 && str[index - 1] == '<') || (index < length - 1 && str[index + 1] == '>'))
                        {
                            // skip it
                            searchindex = index + 1;
                        }
                        else
                        {
                            // split it
                            tmparray.Add(str.Substring(preindex, index - preindex));

                            preindex = index + 1;
                            searchindex = preindex;
                        }
                    }

                }

                // is there still room for more args?
                if (tmparray.Count <= nitems - 1 && preindex < length)
                {
                    // searched past the end and didnt find anything
                    tmparray.Add(str.Substring(preindex, length - preindex));
                }

                // turn tmparray into a string[]

                args = new string[tmparray.Count];
                tmparray.CopyTo(args);
            }
            else
            {
                // just use split to do it with no context control
                args = str.Split(slashdelim, nitems);

            }

            return args;
        }

        public static string[] ParseCommaArgs(string str, int nitems)
        {
            if (str == null) return null;
            str = str.Trim();

            string[] args = str.Split(commadelim, nitems);
            return args;
        }

        public static string[] ParseLiteralTerminator(string str)
        {
            if (str == null) return null;
            str = str.Trim();

            string[] args = str.Split(literalend, 2);
            return args;
        }

        public static string[] ParseSemicolonArgs(string str, int nitems)
        {
            if (str == null) return null;
            str = str.Trim();

            string[] args = str.Split(semicolondelim, nitems);
            return args;
        }

        public static string[] SplitString(string str, string separator)
        {
            if (str == null || separator == null) return null;

            int lastindex = 0;
            List<string> strargs = new List<string>();
            while (true)
            {
                // go through the string and find the first instance of the separator
                var index = str.IndexOf(separator);
                if (index < 0)
                {
                    // no separator so its the end of the string
                    strargs.Add(str);
                    break;
                }

                string arg = str.Substring(lastindex, index);

                strargs.Add(arg);

                str = str.Substring(index + separator.Length, str.Length - (index + separator.Length));
            }

            // now make the string args
            string[] args = new string[strargs.Count];
            for (int i = 0; i < strargs.Count; i++)
            {
                args[i] = strargs[i];
            }

            return args;
        }

        #endregion

        #region Spawn methods

        public static void AddSpawnItem(XmlSpawner spawner, object invoker, XmlSpawner.SpawnObject theSpawn, Item item, Point3D location, Map map, Mobile trigmob, bool requiresurface,
            string propertyString, out string status_str)
        {
            AddSpawnItem(spawner, invoker, theSpawn, item, location, map, trigmob, requiresurface, null, propertyString, false, out status_str);
        }

        public static void AddSpawnItem(XmlSpawner spawner, XmlSpawner.SpawnObject theSpawn, Item item, Point3D location, Map map, Mobile trigmob, bool requiresurface,
            List<XmlSpawner.SpawnPositionInfo> spawnpositioning, string propertyString, bool smartspawn, out string status_str)
        {
            AddSpawnItem(spawner, spawner, theSpawn, item, location, map, trigmob, requiresurface, spawnpositioning, propertyString, smartspawn, out status_str);
        }

        public static void AddSpawnItem(XmlSpawner spawner, object invoker, XmlSpawner.SpawnObject theSpawn, Item item, Point3D location, Map map, Mobile trigmob, bool requiresurface,
            List<XmlSpawner.SpawnPositionInfo> spawnpositioning, string propertyString, bool smartspawn, out string status_str)
        {
            status_str = null;
            if (item == null || theSpawn == null) return;

            // add the item to the spawned list
            theSpawn.SpawnedObjects.Add(item);

            item.Spawner = spawner;

            if (spawner != null)
            {
                // this is being called by a spawner so use spawner information for placement
                if (!spawner.Deleted)
                {
                    // set the item amount
                    if (spawner.StackAmount > 1 && item.Stackable)
                    {
                        item.Amount = spawner.StackAmount;
                    }
                    // if this is in any container such as a pack then add to the container.
                    if (spawner.Parent is Container)
                    {
                        Container c = (Container)spawner.Parent;

                        Point3D loc = spawner.Location;

                        if (!smartspawn)
                        {
                            item.OnBeforeSpawn(loc, map);
                        }

                        item.Location = loc;

                        // check to see whether we drop or add the item based on the spawnrange
                        // this will distribute multiple items around the spawn point, and allow precise
                        // placement of single spawns at the spawn point
                        if (spawner.SpawnRange > 0)
                            c.DropItem(item);
                        else
                            c.AddItem(item);

                    }
                    else
                    {
                        // if the spawn entry is in a subgroup and has a packrange, then get the packcoord

                        Point3D packcoord = Point3D.Zero;
                        if (theSpawn.PackRange >= 0 && theSpawn.SubGroup > 0)
                        {
                            packcoord = spawner.GetPackCoord(theSpawn.SubGroup);
                        }
                        Point3D loc = spawner.GetSpawnPosition(requiresurface, theSpawn.PackRange, packcoord, spawnpositioning);

                        if (!smartspawn)
                        {
                            item.OnBeforeSpawn(loc, map);
                        }

                        // standard placement for all items in the world
                        item.MoveToWorld(loc, map);
                    }
                }
                else
                {
                    // if the spawner has already been deleted then delete the item since it cannot be cleaned up by spawner deletion any longer
                    item.Delete();
                    return;
                }
            }
            else
            {
                if (!smartspawn)
                {
                    item.OnBeforeSpawn(location, map);
                }
                // use the location and map info passed in
                // this allows AddSpawnItem to be called by objects other than spawners as long as they pass in a valid SpawnObject
                item.MoveToWorld(location, map);
            }

            // clear the taken flag on all newly spawned items
            ItemFlags.SetTaken(item, false);

            if (!smartspawn)
            {
                item.OnAfterSpawn();
            }

            // apply the parsed arguments from the typestring using setcommand
            // be sure to do this after setting map and location so that errors dont place the mob on the internal map
            ApplyObjectStringProperties(spawner, propertyString, item, trigmob, spawner, out status_str);
        }

        public static bool SpawnTypeKeyword(object invoker, XmlSpawner.SpawnObject TheSpawn, string typeName, string substitutedtypeName,
            Mobile triggermob, Map map, out string status_str)
        {
            return SpawnTypeKeyword(invoker, TheSpawn, typeName, substitutedtypeName,
                triggermob, map, out status_str, 0);
        }

        public static bool SpawnTypeKeyword(object invoker, XmlSpawner.SpawnObject TheSpawn, string typeName, string substitutedtypeName, Mobile triggermob, Map map, out string status_str, byte loops)
        {
            status_str = null;

            if (typeName == null || TheSpawn == null || substitutedtypeName == null) return false;

            XmlSpawner spawner = invoker as XmlSpawner;

            // check for any special keywords that might appear in the type such as SET, GIVE, or TAKE
            #region typeKeyword
            if (IsTypeKeyword(typeName))
            {
                typeKeyword kw = typeKeywordHash[typeName];

                switch (kw)
                {
                    case typeKeyword.SET:
                        {
                            // the syntax is SET/prop/value/prop2/value...
                            // check for the SET,itemname or serialno[,itemtype]/prop/value form is used
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            string[] keywordargs = ParseString(arglist[0], 3, ",");

                            if (keywordargs.Length > 1)
                            {
                                string typestr = null;
                                if (keywordargs.Length > 2)
                                {
                                    typestr = keywordargs[2];
                                }

                                // is the itemname a serialno?
                                object setitem = null;
                                if (keywordargs[1].StartsWith("0x"))
                                {
                                    int serial = -1;
                                    try
                                    {
                                        serial = Convert.ToInt32(keywordargs[1], 16);
                                    }
                                    catch { }
                                    if (serial >= 0)
                                        setitem = World.FindEntity(serial);
                                }
                                else
                                {
                                    // just look it up by name
                                    setitem = FindItemByName(spawner, keywordargs[1], typestr);
                                }

                                if (setitem == null)
                                {
                                    status_str = "cant find unique item :" + keywordargs[1];
                                    return false;
                                }

                                ApplyObjectStringProperties(spawner, substitutedtypeName, setitem, triggermob, invoker, out status_str);
                            }
                            else if (spawner != null)
                            {
                                ApplyObjectStringProperties(spawner, substitutedtypeName, spawner.SetItem, triggermob, invoker, out status_str);
                            }


                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.DESPAWN:
                        {
                            // the syntax is DESPAWN[,spawnername],subgroup

                            // first find the spawner and group
                            int subgroup = -1;
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            XmlSpawner targetspawner = spawner;
                            if (arglist.Length > 0)
                            {
                                string[] keywordargs = ParseString(arglist[0], 3, ",");
                                if (keywordargs.Length < 2)
                                {
                                    status_str = "missing subgroup in DESPAWN";
                                    return false;
                                }

                                string subgroupstr = keywordargs[1];
                                string spawnerstr = null;
                                if (keywordargs.Length > 2)
                                {
                                    spawnerstr = keywordargs[1];
                                    subgroupstr = keywordargs[2];
                                }
                                if (spawnerstr != null)
                                {
                                    targetspawner = FindSpawnerByName(spawner, spawnerstr);
                                }
                                if (!int.TryParse(subgroupstr, out subgroup))
                                    subgroup = -1;
                            }
                            if (subgroup == -1)
                            {
                                status_str = "invalid subgroup in DESPAWN";
                                return false;
                            }

                            if (targetspawner != null)
                            {
                                targetspawner.ClearSubgroup(subgroup);
                            }
                            else
                            {
                                status_str = "invalid spawner in DESPAWN";
                                return false;
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.SPAWN:
                        {
                            // the syntax is SPAWN[,spawnername],subgroup

                            // first find the spawner and group
                            int subgroup = -1;
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            XmlSpawner targetspawner = spawner;
                            if (arglist.Length > 0)
                            {
                                string[] keywordargs = ParseString(arglist[0], 3, ",");
                                if (keywordargs.Length < 2)
                                {
                                    status_str = "missing subgroup in SPAWN";
                                    return false;
                                }

                                string subgroupstr = keywordargs[1];
                                string spawnerstr = null;
                                if (keywordargs.Length > 2)
                                {
                                    spawnerstr = keywordargs[1];
                                    subgroupstr = keywordargs[2];
                                }
                                if (spawnerstr != null)
                                {
                                    targetspawner = FindSpawnerByName(spawner, spawnerstr);
                                }
                                if (!int.TryParse(subgroupstr, out subgroup))
                                    subgroup = -1;
                            }
                            if (subgroup == -1)
                            {
                                status_str = "invalid subgroup in SPAWN";
                                return false;
                            }

                            if (targetspawner != null)
                            {
                                if (spawner != targetspawner)
                                {
                                    // allow spawning of other spawners to be forced and ignore the normal loop protection
                                    if (loops >= XmlSpawner.MaxLoops) //preventing looping from spawner to spawner, via recursive linked method calls
                                    {
                                        status_str = "recursive looping stop in SPAWN";
                                        return false;
                                    }
                                    targetspawner.SpawnSubGroup(subgroup, false, true, (byte)(loops + 1));
                                }
                                else
                                {
                                    if (loops >= XmlSpawner.MaxLoops)
                                    {
                                        status_str = "recursive looping stop in SPAWN";
                                        return false;
                                    }
                                    targetspawner.SpawnSubGroup(subgroup, (byte)(loops + 1));
                                }
                            }
                            else
                            {
                                status_str = "invalid spawner in SPAWN";
                                return false;
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.GOTO:
                        {
                            // the syntax is GOTO/subgroup
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            int group = -1;
                            if (arglist.Length < 2)
                            {
                                status_str = "insufficient args to GOTO";
                            }
                            else
                            {
                                if (!int.TryParse(arglist[1], out group))
                                {
                                    status_str = "invalid subgroup arg to GOTO";
                                    group = -1;
                                }
                            }
                            if (status_str != null)
                            {
                                return false;
                            }

                            // move the sequence to the specified subgroup
                            if (group >= 0 && spawner != null && !spawner.Deleted)
                            {
                                // note, this will activate sequential spawning if it wasnt already set
                                spawner.SequentialSpawn = group;

                                // and suppress sequential advancement so that the specified group is the next to spawn
                                spawner.HoldSequence = true;
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner, 2));

                            break;
                        }
                    case typeKeyword.COMMAND:
                        {
                            // the syntax is COMMAND/commandstring
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            if (arglist.Length > 0)
                            {
                                // mod to use a dummy char to issue commands
                                if (CommandMobileName != null)
                                {
                                    Mobile dummy = FindMobileByName(spawner, CommandMobileName, "Mobile");
                                    if (dummy != null)
                                    {
                                        CommandSystem.Handle(dummy, string.Format("{0}{1}", CommandSystem.Prefix, arglist[1]));
                                    }
                                }
                                else
                                    if (triggermob != null && !triggermob.Deleted)
                                {
                                    CommandSystem.Handle(triggermob, string.Format("{0}{1}", CommandSystem.Prefix, arglist[1]));
                                }
                            }
                            else
                            {
                                status_str = "insufficient args to COMMAND";
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    default:
                        {
                            status_str = "unrecognized keyword";
                            // should never get here
                            break;
                        }
                }
                // indicate successful keyword spawn
                return true;
            }
            #endregion

            // should never get here
            status_str = "unrecognized keyword";
            return false;
        }
        #endregion

        public static List<Item> GetItems(Region r)
        {
            List<Item> list = new List<Item>();
            if (r == null) return list;

            Sector[] sectors = r.Sectors;

            if (sectors != null)
            {
                for (int i = 0; i < sectors.Length; i++)
                {
                    Sector sector = sectors[i];

                    foreach (Item item in sector.Items)
                    {
                        if (Region.Find(item.Location, item.Map).IsPartOf(r))
                            list.Add(item);
                    }
                }
            }

            return list;
        }
    }
}
