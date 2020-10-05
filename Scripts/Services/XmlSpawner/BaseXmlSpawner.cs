using Server.Accounting;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Spells;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Server.Mobiles
{
    public delegate void XmlGumpCallback(Mobile from, object invoker, string response);

    public class BaseXmlSpawner
    {
        #region Initialization
        private static readonly List<ProtectedProperty> ProtectedPropertiesList = new List<ProtectedProperty>();

        private class ProtectedProperty
        {
            public Type ObjectType;
            public string Name;

            public ProtectedProperty(Type type, string name)
            {
                ObjectType = type;
                Name = name;
            }
        }

        public static bool IsProtected(Type type, string property)
        {
            if (type == null || property == null) return false;

            // search through the protected list for a matching entry
            foreach (ProtectedProperty p in ProtectedPropertiesList)
            {
                if ((p.ObjectType == type || type.IsSubclassOf(p.ObjectType)) && (property.ToLower() == p.Name.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        public static void Initialize()
        {
            ProtectedPropertiesList.Add(new ProtectedProperty(typeof(Mobile), "accesslevel"));
            ProtectedPropertiesList.Add(new ProtectedProperty(typeof(Item), "stafflevel"));
        }
        #endregion

        #region Type support
        [Flags]
        public enum KeywordFlags
        {
            HoldSpawn = 0x01,
            HoldSequence = 0x02,
            Serialize = 0x04,
            Defrag = 0x08,

        }

        public class TypeInfo
        {
            public List<PropertyInfo> plist = new List<PropertyInfo>();      // hold propertyinfo list
            public Type t;
        }

        private static readonly Type typeofTimeSpan = typeof(TimeSpan);
        private static readonly Type typeofParsable = typeof(ParsableAttribute);
        private static readonly Type typeofCustomEnum = typeof(CustomEnumAttribute);

        private static bool IsParsable(Type t)
        {
            return (t == typeofTimeSpan || t.IsDefined(typeofParsable, false));
        }

        private static readonly Type[] m_ParseTypes = new Type[] { typeof(string) };
        private static readonly object[] m_ParseParams = new object[1];

        private static object Parse(object o, Type t, string value)
        {
            MethodInfo method = t.GetMethod("Parse", m_ParseTypes);

            m_ParseParams[0] = value;

            return method.Invoke(o, m_ParseParams);
        }

        private static readonly Type[] m_NumericTypes = new Type[]
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

        public static string ParsedType(Type type)
        {
            if (type == null) return null;

            string s = type.ToString();

            if (s == null) return null;

            string[] args = s.Split(Type.Delimiter);

            if (args != null && args.Length > 0)
            {
                return args[args.Length - 1];
            }
            else
            {
                return null;
            }
        }

        public static bool CheckType(object o, string typename)
        {
            if (typename == null || o == null) return false;

            // test the type
            Type objecttype = o.GetType();
            Type targettype = SpawnerType.GetType(typename);
            if (objecttype != null && targettype != null && (objecttype.Equals(targettype) || objecttype.IsSubclassOf(targettype)))
            {
                return true;

            }

            return false;

        }

        private enum typeKeyword
        {
            SET,
            SETVAR,
            SETONTRIGMOB,
            SETONCARRIED,
            SETONSPAWN,
            SETONSPAWNENTRY,
            SETONNEARBY,
            SETONPETS,
            SETONMOB,
            SETONTHIS,
            SETONPARENT,
            SETACCOUNTTAG,
            FOREACH,
            GIVE,
            TAKEGIVE,
            GUMP,
            BROWSER,
            SENDMSG,
            SENDASCIIMSG,
            MUSIC,
            RESURRECT,
            POISON,
            DAMAGE,
            EFFECT,
            MEFFECT,
            SOUND,
            WAITUNTIL,
            WHILE,
            IF,
            GOTO,
            CAST,
            BCAST,
            BSOUND,
            COMMAND,
            SPAWN,
            DESPAWN
        }

        private enum typemodKeyword
        {
            MUSIC,
            POISON,
            DAMAGE,
            EFFECT,
            BOLTEFFECT,
            MEFFECT,
            PEFFECT,
            SOUND,
            SAY,
            SPEECH,
            ANIMATE,
            OFFSET,
            ADD,
            MSG,
            ASCIIMSG,
            SENDMSG,
            SENDASCIIMSG,
            BCAST,
            EQUIP,
            UNEQUIP,
            DELETE,
            KILL,
            FACETO,
            FLASH,
            PRIVMSG
        }

        private enum valueKeyword
        {
            GET,
            GETONMOB,
            GETONCARRIED,
            GETONTRIGMOB,
            GETONNEARBY,
            GETONSPAWN,
            GETONPARENT,
            GETONTHIS,
            GETFROMFILE,
            GETACCOUNTTAG,
            AMOUNTCARRIED,
            RND,
            RNDBOOL,
            RNDLIST,
            RNDSTRLIST,
            TRIGSKILL,
            PLAYERSINRANGE,
            MY,
            RANDNAME
        }

        private enum valuemodKeyword
        {
            GET,
            GETONMOB,
            GETONCARRIED,
            GETONTRIGMOB,
            GETONNEARBY,
            GETONSPAWN,
            GETONPARENT,
            GETONTHIS,
            GETFROMFILE,
            GETACCOUNTTAG,
            AMOUNTCARRIED,
            RND,
            RNDBOOL,
            RNDLIST,
            RNDSTRLIST,
            MUL,
            INC,
            MOB,
            TRIGMOB,
            MY,
            TRIGSKILL,
            PLAYERSINRANGE,
            RANDNAME
        }
        #endregion

        #region Static variable declarations
        // name of mobile used to issue commands via the COMMAND keyword.  The accesslevel of the mobile will determine
        // the accesslevel of commands that can be issued.
        // if this is null, then COMMANDS can only be issued when triggered by players of the appropriate accesslevel
        private static readonly string CommandMobileName = null;

        private static readonly Dictionary<string, typeKeyword> typeKeywordHash = new Dictionary<string, typeKeyword>();
        private static readonly Dictionary<string, typemodKeyword> typemodKeywordHash = new Dictionary<string, typemodKeyword>();
        private static readonly Dictionary<string, valueKeyword> valueKeywordHash = new Dictionary<string, valueKeyword>();
        private static readonly Dictionary<string, valuemodKeyword> valuemodKeywordHash = new Dictionary<string, valuemodKeyword>();

        private static readonly char[] slashdelim = new char[1] { '/' };
        private static readonly char[] commadelim = new char[1] { ',' };
        private static readonly char[] spacedelim = new char[1] { ' ' };
        private static readonly char[] semicolondelim = new char[1] { ';' };
        private static readonly char[] literalend = new char[1] { '§' };
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

        public static bool IsTypemodKeyword(string typeName)
        {
            if (string.IsNullOrEmpty(typeName) || !char.IsUpper(typeName[0])) return false;
            return (typemodKeywordHash.ContainsKey(typeName));
        }

        public static bool IsTypeOrItemKeyword(string typeName)
        {
            if (string.IsNullOrEmpty(typeName) || !char.IsUpper(typeName[0])) return false;
            return (typeKeywordHash.ContainsKey(typeName));
        }

        private static void AddTypeKeyword(string name)
        {
            typeKeywordHash.Add(name, (typeKeyword)Enum.Parse(typeof(typeKeyword), name));
        }

        private static void AddTypemodKeyword(string name)
        {
            typemodKeywordHash.Add(name, (typemodKeyword)Enum.Parse(typeof(typemodKeyword), name));
        }

        private static void AddValueKeyword(string name)
        {
            valueKeywordHash.Add(name, (valueKeyword)Enum.Parse(typeof(valueKeyword), name));
        }

        private static void AddValuemodKeyword(string name)
        {
            valuemodKeywordHash.Add(name, (valuemodKeyword)Enum.Parse(typeof(valuemodKeyword), name));
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

        public static void Configure()
        {
            // set up the keyword hash tables
            // already set at startup time
            /*typeKeywordHash = new Hashtable();
			typemodKeywordHash = new Hashtable();
			valueKeywordHash = new Hashtable();
			valuemodKeywordHash = new Hashtable();*/

            // Type keywords
            // spawned as primary objects
            AddTypeKeyword("SET");
            AddTypeKeyword("SETVAR");
            AddTypeKeyword("SETONTRIGMOB");
            AddTypeKeyword("SETONCARRIED");
            AddTypeKeyword("SETONNEARBY");
            AddTypeKeyword("SETONPETS");
            AddTypeKeyword("SETONSPAWN");
            AddTypeKeyword("SETONSPAWNENTRY");
            AddTypeKeyword("SETONMOB");
            AddTypeKeyword("SETONTHIS");
            AddTypeKeyword("SETONPARENT");
            AddTypeKeyword("SETACCOUNTTAG");
            AddTypeKeyword("FOREACH");
            AddTypeKeyword("GIVE");
            AddTypeKeyword("TAKEGIVE");
            AddTypeKeyword("GUMP");
            AddTypeKeyword("BROWSER");
            AddTypeKeyword("SENDMSG");
            AddTypeKeyword("SENDASCIIMSG");
            AddTypeKeyword("RESURRECT");
            AddTypeKeyword("DAMAGE");
            AddTypeKeyword("EFFECT");
            AddTypeKeyword("MEFFECT");
            AddTypeKeyword("WAITUNTIL");
            AddTypeKeyword("WHILE");
            AddTypeKeyword("IF");
            AddTypeKeyword("GOTO");
            AddTypeKeyword("CAST");
            AddTypeKeyword("BCAST");
            AddTypeKeyword("COMMAND");
            AddTypeKeyword("SPAWN");
            AddTypeKeyword("DESPAWN");

            // Typemod keywords
            // used in place of properties as modifiers of the primary object type
            AddTypemodKeyword("DAMAGE");
            AddTypemodKeyword("EFFECT");
            AddTypemodKeyword("BOLTEFFECT");
            AddTypemodKeyword("MEFFECT");
            AddTypemodKeyword("PEFFECT");
            AddTypemodKeyword("SAY");
            AddTypemodKeyword("SPEECH");
            AddTypemodKeyword("ANIMATE");
            AddTypemodKeyword("OFFSET");
            AddTypemodKeyword("MSG");
            AddTypemodKeyword("ASCIIMSG");
            AddTypemodKeyword("SENDMSG");
            AddTypemodKeyword("SENDASCIIMSG");
            AddTypemodKeyword("BCAST");
            AddTypemodKeyword("ADD");
            AddTypemodKeyword("EQUIP");
            AddTypemodKeyword("UNEQUIP");
            AddTypemodKeyword("DELETE");
            AddTypemodKeyword("KILL");
            AddTypemodKeyword("FACETO");
            AddTypemodKeyword("FLASH");
            AddTypemodKeyword("PRIVMSG");

            // Value keywords
            // used in property tests
            AddValueKeyword("RND");
            AddValueKeyword("RNDBOOL");
            AddValueKeyword("RNDLIST");
            AddValueKeyword("RNDSTRLIST");
            AddValueKeyword("MY");
            AddValueKeyword("GET");
            AddValueKeyword("GETONMOB");
            AddValueKeyword("GETONCARRIED");
            AddValueKeyword("AMOUNTCARRIED");
            AddValueKeyword("GETONTRIGMOB");
            AddValueKeyword("GETONNEARBY");
            AddValueKeyword("GETONSPAWN");
            AddValueKeyword("GETONPARENT");
            AddValueKeyword("GETONTHIS");
            AddValueKeyword("GETFROMFILE");
            AddValueKeyword("GETACCOUNTTAG");
            AddValueKeyword("RANDNAME");
            AddValueKeyword("TRIGSKILL");
            AddValueKeyword("PLAYERSINRANGE");

            // Valuemod keywords
            // used as values in property assignments
            AddValuemodKeyword("RND");
            AddValuemodKeyword("RNDBOOL");
            AddValuemodKeyword("RNDLIST");
            AddValuemodKeyword("RNDSTRLIST");
            AddValuemodKeyword("MY");
            AddValuemodKeyword("GET");
            AddValuemodKeyword("GETONMOB");
            AddValuemodKeyword("GETONCARRIED");
            AddValuemodKeyword("AMOUNTCARRIED");
            AddValuemodKeyword("GETONTRIGMOB");
            AddValuemodKeyword("GETONNEARBY");
            AddValuemodKeyword("GETONSPAWN");
            AddValuemodKeyword("GETONPARENT");
            AddValuemodKeyword("GETONTHIS");
            AddValuemodKeyword("GETFROMFILE");
            AddValuemodKeyword("GETACCOUNTTAG");
            AddValuemodKeyword("MUL");
            AddValuemodKeyword("INC");
            AddValuemodKeyword("MOB");
            AddValuemodKeyword("TRIGMOB");
            AddValuemodKeyword("RANDNAME");
            AddValuemodKeyword("TRIGSKILL");
            AddValuemodKeyword("PLAYERSINRANGE");
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

            public KeywordTag(string typename, XmlSpawner spawner, TimeSpan delay, string condition, int gotogroup)
                : this(typename, spawner, 0, delay, TimeSpan.Zero, condition, gotogroup)
            {
            }

            public KeywordTag(string typename, XmlSpawner spawner, TimeSpan delay, TimeSpan timeout, string condition, int gotogroup)
                : this(typename, spawner, 0, delay, timeout, condition, gotogroup)
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
                    if (m_Condition != null && m_Condition.Length > 0 && m_Spawner != null && m_Spawner.Running)
                    {
                        // if the test is valid then terminate the timer
                        string status_str;
                        Mobile trigmob = null;

                        if (m_Spawner != null && !m_Spawner.Deleted)
                        {
                            trigmob = m_Spawner.TriggerMob;
                        }

                        if (TestItemProperty(m_Spawner, m_Spawner, m_Condition, trigmob, out status_str))
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
            else
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
                    MethodInfo info = p.PropertyType.GetMethod("Parse", new Type[] { typeof(string) });
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

            string[] keywordargs = ParseString(propname, 4, ",");

            // check for special keywords
            if (keywordargs[0] == "SKILL")
            {
                if (keywordargs.Length < 2)
                {
                    return "Invalid SKILL format";
                }
                SkillName skillname;

                if (TryParse(keywordargs[1], true, out skillname))
                {
                    if (o is Mobile)
                    {
                        Skill skill = ((Mobile)o).Skills[skillname];
                        double d;
                        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                        {
                            skill.Base = d;
                            return "Property has been set.";
                        }
                        else
                            return "Invalid double number";
                    }
                    else
                        return "Object is not mobile";
                }
                else
                    return "Invalid SKILL reference.";
            }
            else if (keywordargs[0] == "STEALABLE")
            {
                if (o is Item)
                {
                    bool b;
                    if (bool.TryParse(value, out b))
                    {
                        ItemFlags.SetStealable((Item)o, b);
                        return "Property has been set.";
                    }
                    else
                        return "Invalid Stealable assignment.";
                }
                else
                    return "Object is not an item";
            }

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


                Type ptype;
                object po;
                if (plookup != null)
                {
                    if (IsProtected(type, propname))
                        return "Property is protected.";

                    ptype = plookup.PropertyType;
                    po = plookup.GetValue(o, null);

                    // now set the nested attribute using the new property list
                    return (SetPropertyValue(spawner, po, arglist[1], value));
                }
                else
                {
                    // is a nested property with attributes so first get the property
                    foreach (PropertyInfo p in props)
                    {
                        if (Insensitive.Equals(p.Name, propname))
                        {
                            if (IsProtected(type, propname))
                                return "Property is protected.";

                            ptype = p.PropertyType;

                            po = p.GetValue(o, null);

                            // now set the nested attribute using the new property list
                            return (SetPropertyValue(spawner, po, arglist[1], value));
                        }
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

                    if (IsProtected(type, propname))
                        return "Property is protected.";

                    string returnvalue = InternalSetValue(null, o, plookup, value, false, index);

                    return returnvalue;
                }
                else
                {
                    // note, looping through all of the props turns out to be a significant performance bottleneck
                    // good place for optimization

                    foreach (PropertyInfo p in props)
                    {
                        if (Insensitive.Equals(p.Name, propname))
                        {

                            if (!p.CanWrite)
                                return "Property is read only.";

                            if (IsProtected(type, propname))
                                return "Property is protected.";

                            string returnvalue = InternalSetValue(null, o, p, value, false, index);

                            return returnvalue;

                        }
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


                Type ptype;
                object po;
                if (plookup != null)
                {
                    if (IsProtected(type, arglist[0]))
                        return "Property is protected.";

                    ptype = plookup.PropertyType;

                    po = plookup.GetValue(o, null);

                    // now set the nested attribute using the new property list
                    return (SetPropertyObject(spawner, po, arglist[1], value));
                }
                else
                {

                    foreach (PropertyInfo p in props)
                    {
                        if (Insensitive.Equals(p.Name, arglist[0]))
                        {
                            if (IsProtected(type, arglist[0]))
                                return "Property is protected.";

                            ptype = p.PropertyType;

                            po = p.GetValue(o, null);

                            // now set the nested attribute using the new property list
                            return (SetPropertyObject(spawner, po, arglist[1], value));

                        }
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

                    if (IsProtected(type, name))
                        return "Property is protected.";

                    if (plookup.PropertyType == typeof(Mobile))
                    {
                        plookup.SetValue(o, value, null);

                        return "Property has been set.";
                    }
                    else
                    {
                        return "Property is not of type Mobile.";
                    }
                }
                else
                {
                    foreach (PropertyInfo p in props)
                    {
                        if (Insensitive.Equals(p.Name, name))
                        {

                            if (!p.CanWrite)
                                return "Property is read only.";

                            if (IsProtected(type, name))
                                return "Property is protected.";

                            if (p.PropertyType == typeof(Mobile))
                            {
                                p.SetValue(o, value, null);

                                return "Property has been set.";
                            }
                            else
                            {
                                return "Property is not of type Mobile.";
                            }
                        }
                    }
                }
            }

            return "Property not found.";
        }

        public static string GetBasicPropertyValue(object o, string propname, out Type ptype)
        {
            ptype = null;

            if (o == null || propname == null) return null;

            Type type = o.GetType();

            PropertyInfo[] props = type.GetProperties();

            foreach (PropertyInfo p in props)
            {

                if (Insensitive.Equals(p.Name, propname))
                {
                    if (!p.CanRead)
                        return null;

                    ptype = p.PropertyType;

                    string value = InternalGetValue(o, p, -1);

                    return ParseGetValue(value, ptype);
                }
            }
            return null;
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

            if (keywordargs[0] == "SKILL")
            {
                // syntax is SKILL,skillname
                if (keywordargs.Length < 2)
                {
                    return "Invalid SKILL format";
                }
                SkillName skillname;

                if (TryParse(keywordargs[1], true, out skillname))
                {
                    if (o is Mobile)
                    {
                        Skill skill = ((Mobile)o).Skills[skillname];
                        ptype = skill.Value.GetType();

                        return string.Format("{0} = {1}", skillname, skill.Value);
                    }
                    else
                        return "Object is not mobile";
                }
                else
                { return "Skill not found."; }
            }
            else if (keywordargs[0] == "SERIAL")
            {
                bool found;
                try
                {
                    if (o is Mobile)
                    {
                        ptype = ((Mobile)o).Serial.GetType();

                        return string.Format("Serial = {0}", ((Mobile)o).Serial);
                    }
                    else
                        if (o is Item)
                    {
                        ptype = ((Item)o).Serial.GetType();

                        return string.Format("Serial = {0}", ((Item)o).Serial);
                    }
                    else
                        return "Object is not item/mobile";
                }
                catch { found = false; }

                if (!found)
                    return "Serial not found.";
            }
            else if (keywordargs[0] == "TYPE")
            {
                ptype = typeof(Type);

                return string.Format("Type = {0}", o.GetType().Name);

            }
            else if (keywordargs[0] == "STEALABLE")
            {
                bool found;
                try
                {

                    if (o is Item)
                    {
                        ptype = typeof(bool);
                        return string.Format("Stealable = {0}", ItemFlags.GetStealable((Item)o));
                    }
                    else
                        return "Object is not an item";
                }
                catch { found = false; }

                if (!found)
                    return "Stealable flag not found.";
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
                else
                {
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
                else
                {
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
            }

            return "Property not found.";
        }

        public static string ApplyToProperty(XmlSpawner spawner, object getobject, object setobject, string getpropertystring, string setpropertystring)
        {
            Type ptype;

            string getvalue = GetPropertyValue(spawner, getobject, getpropertystring, out ptype);

            if (getvalue == null)
            {
                return "Null object or property";
            }

            if (ptype == null)
            {
                return getvalue;
            }

            string value2 = ParseGetValue(getvalue, ptype);

            if (value2 != null)
            {
                // set the property value using returned get value as the the value
                string result = SetPropertyValue(spawner, setobject, setpropertystring, value2);

                // see if it was successful
                if (result != "Property has been set.")
                {
                    return setpropertystring + " : " + result;
                }
            }

            return null;
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

            string[] arglist;

            arglist = ParseSlashArgs(str, 2);

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
                    if (groupargstring != null && groupargstring.Length > 0)
                    {

                        if (value_keywordargs != null && value_keywordargs.Length > 0)
                            value_keywordargs[value_keywordargs.Length - 1] = groupargstring;
                    }

                    // handle propname keywords that may take comma args
                    //string[] keywordargs = ParseString(arglist[0],10,",");
                    string[] keywordargs = arglist[0].Trim().Split(',');


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

                            if (kw == valuemodKeyword.RND)
                            {
                                // generate a random number and use it as the property value.  Use the format /RND,min,max/
                                if (value_keywordargs.Length > 2)
                                {
                                    // get a random number
                                    string randvalue = "0";
                                    int min, max;
                                    if (int.TryParse(value_keywordargs[1], out min) && int.TryParse(value_keywordargs[2], out max))
                                    {
                                        randvalue = string.Format("{0}", Utility.RandomMinMax(min, max));
                                    }
                                    else
                                    {
                                        status_str = "Invalid RND args : " + arglist[1]; no_error = false;
                                    }
                                    // set the property value using the random number as the value
                                    string result = SetPropertyValue(spawner, o, arglist[0], randvalue);
                                    // see if it was successful
                                    if (result != "Property has been set.")
                                    {
                                        status_str = arglist[0] + " : " + result;
                                        no_error = false;
                                    }
                                }
                                else
                                {
                                    status_str = "Invalid RND args : " + arglist[1];
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.RNDBOOL)
                            {
                                // generate a random bool and use it as the property value.  Use the format /RNDBOOL/

                                string randvalue = Utility.RandomBool().ToString();

                                // set the property value using the random number as the value
                                string result = SetPropertyValue(spawner, o, arglist[0], randvalue);
                                // see if it was successful
                                if (result != "Property has been set.")
                                {
                                    status_str = arglist[0] + " : " + result;
                                    no_error = false;
                                }

                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.RNDLIST || kw == valuemodKeyword.RNDSTRLIST)
                            {
                                // generate a random number and use it as the property value.  Use the format /RNDLIST,val1,val2,.../
                                if (value_keywordargs.Length > 1)
                                {
                                    // compute a random index into the arglist

                                    int randindex = Utility.Random(1, value_keywordargs.Length - 1);

                                    // assign the list entry as the value

                                    string randvalue = value_keywordargs[randindex];

                                    // set the property value 
                                    string result = SetPropertyValue(spawner, o, arglist[0], randvalue);

                                    // see if it was successful
                                    if (result != "Property has been set.")
                                    {
                                        status_str = arglist[0] + " : " + result;
                                        no_error = false;
                                    }
                                }
                                else
                                {
                                    status_str = "Invalid " + arglist[0] + " args : " + arglist[1];
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.MY)
                            {
                                // syntax is MY,property
                                // note this will be an arg to some property
                                if (value_keywordargs.Length > 1)
                                {
                                    string resultstr = ApplyToProperty(spawner, o, o, value_keywordargs[1], arglist[0]);
                                    if (resultstr != null)
                                    {
                                        status_str = "MY error: " + resultstr;
                                        no_error = false;
                                    }

                                }
                                else
                                {
                                    status_str = "Invalid MY args : " + arglist[1];
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.GET)
                            {
                                // syntax is GET,[itemname -OR- SETITEM][,itemtype],property
                                // or GET,itemname[,itemtype],<ATTACHMENT,type,name,property>
                                // note this will be an arg to some property
                                if (value_keywordargs.Length > 2)
                                {
                                    string propname = value_keywordargs[2];
                                    string typestr = null;

                                    if (value_keywordargs.Length > 3)
                                    {
                                        propname = value_keywordargs[3];
                                        typestr = value_keywordargs[2];
                                    }
                                    // get the current property value
                                    //Type ptype;
                                    // get target item
                                    // is the itemname a serialno?
                                    object testitem = null;
                                    if (value_keywordargs[1].StartsWith("0x"))
                                    {
                                        int serial;
                                        if (!int.TryParse(value_keywordargs[1].Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out serial))
                                            serial = -1;
                                        if (serial >= 0)
                                            testitem = World.FindEntity(serial);
                                    }
                                    else if (value_keywordargs[1] == "SETITEM" && spawner != null && !spawner.Deleted && spawner.SetItem != null)
                                    {
                                        testitem = spawner.SetItem;
                                    }
                                    else
                                    {
                                        testitem = FindItemByName(spawner, value_keywordargs[1], typestr);
                                    }

                                    string resultstr = ApplyToProperty(spawner, testitem, o, propname, arglist[0]);

                                    if (resultstr != null)
                                    {
                                        status_str = "GET error: " + resultstr;
                                        no_error = false;
                                    }

                                }
                                else if (spawner != null && value_keywordargs.Length > 0)
                                {
                                    string propname = value_keywordargs[0];
                                    string resultstr = ApplyToProperty(spawner, spawner.SetItem, o, propname, arglist[0]);

                                    if (resultstr != null)
                                    {
                                        status_str = "GET error: " + resultstr;
                                        no_error = false;
                                    }
                                }
                                else
                                {
                                    status_str = "Invalid GET args : " + arglist[1];
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.GETONMOB)
                            {
                                // syntax is GETONMOB,mobname[,mobtype],property
                                // or GETONMOB,mobname[,mobtype],<ATTACHMENT,type,name,property>
                                // note this will be an arg to some property
                                if (value_keywordargs.Length > 2)
                                {
                                    //if(trigmob != null && !trigmob.Deleted){
                                    // get the current property value
                                    //Type ptype;
                                    // get target item

                                    string propname = value_keywordargs[2];
                                    string typestr = null;
                                    if (value_keywordargs.Length > 3)
                                    {
                                        propname = value_keywordargs[3];
                                        typestr = value_keywordargs[2];
                                    }

                                    Mobile testmobile = FindMobileByName(spawner, value_keywordargs[1], typestr);
                                    string resultstr = ApplyToProperty(spawner, testmobile, o, propname, arglist[0]);
                                    if (resultstr != null)
                                    {
                                        status_str = "GETONMOB error: " + resultstr;
                                        no_error = false;
                                    }
                                }
                                else
                                {
                                    status_str = "Invalid GETONMOB args : " + arglist[1];
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.GETONCARRIED)
                            {
                                // syntax is GETONCARRIED,itemname[,itemtype][,equippedonly],property
                                // or GETONCARRIED,itemname[,itemtype][,equippedonly],<ATTACHMENT,type,name,property>
                                // note this will be an arg to some property
                                if (value_keywordargs.Length > 2)
                                {
                                    bool equippedonly = false;

                                    if (trigmob != null && !trigmob.Deleted)
                                    {
                                        string itemname = value_keywordargs[1];
                                        string propname = value_keywordargs[2];
                                        string typestr = null;
                                        if (value_keywordargs.Length > 3)
                                        {
                                            propname = value_keywordargs[3];
                                            typestr = value_keywordargs[2];
                                        }
                                        if (value_keywordargs.Length > 4)
                                        {
                                            propname = value_keywordargs[4];
                                            if (value_keywordargs[3].ToLower() == "equippedonly")
                                            {
                                                equippedonly = true;
                                            }
                                            else
                                            {
                                                if (!bool.TryParse(value_keywordargs[3], out equippedonly))
                                                {
                                                    status_str = "GETONCARRIED error parsing equippedonly";
                                                    no_error = false;
                                                }
                                            }
                                        }
                                        // get the current property value
                                        //Type ptype;
                                        // get target item
                                        Item testitem = SearchMobileForItem(trigmob, ParseObjectType(itemname), typestr, false, equippedonly);

                                        string resultstr = ApplyToProperty(spawner, testitem, o, propname, arglist[0]);

                                        if (resultstr != null)
                                        {
                                            status_str = "GETONCARRIED error: " + resultstr;
                                            no_error = false;
                                        }

                                    }
                                    else
                                    {
                                        no_error = false;
                                    }
                                }
                                else
                                {
                                    status_str = "Invalid GETONCARRIED args : " + arglist[1];
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.GETONTRIGMOB)
                            {
                                // syntax is GETONTRIGMOB,property
                                // or  GETONTRIGMOB,<ATTACHMENT,type,name,property>
                                // note this will be an arg to some property
                                if (value_keywordargs.Length > 1)
                                {
                                    if (trigmob != null && !trigmob.Deleted)
                                    {
                                        string resultstr = ApplyToProperty(spawner, trigmob, o, value_keywordargs[1], arglist[0]);
                                        if (resultstr != null)
                                        {
                                            status_str = "GETONTRIGMOB error: " + resultstr;
                                            no_error = false;
                                        }
                                    }
                                    else
                                    {
                                        no_error = false;
                                    }
                                }
                                else
                                {
                                    status_str = "Invalid GETONTRIGMOB args : " + arglist[1];
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.GETONNEARBY)
                            {
                                // syntax is GETONNEARBY,range,name[,type][,searchcontainers],property
                                // or GETONNEARBY,range,name[,type][,searchcontainers],[ATTACHMENT,type,name,property]
                                // note this will be an arg to some property
                                if (value_keywordargs.Length > 3)
                                {
                                    string targetname = value_keywordargs[2];
                                    string propname = value_keywordargs[3];
                                    string typestr = null;
                                    bool searchcontainers = false;
                                    int range;
                                    if (!int.TryParse(value_keywordargs[1], out range))
                                        range = -1;

                                    if (range < 0)
                                    {
                                        status_str = "invalid range in GETONNEARBY";
                                        no_error = false;
                                    }

                                    if (value_keywordargs.Length > 4)
                                    {
                                        typestr = value_keywordargs[3];
                                        propname = value_keywordargs[4];
                                    }

                                    if (value_keywordargs.Length > 5)
                                    {
                                        if (!bool.TryParse(value_keywordargs[3], out searchcontainers))
                                        {
                                            status_str = "invalid searchcontainer bool in GETONNEARBY";
                                            no_error = false;
                                        }
                                        typestr = value_keywordargs[4];
                                        propname = value_keywordargs[5];
                                    }

                                    Type targettype = null;
                                    if (typestr != null)
                                    {
                                        targettype = SpawnerType.GetType(typestr);
                                    }

                                    if (no_error)
                                    {
                                        // get all of the nearby objects
                                        object relativeto = spawner;
                                        List<object> nearbylist = GetNearbyObjects(relativeto, targetname, targettype, typestr, range, searchcontainers, null);

                                        string resultstr = null;

                                        // apply the properties from the first valid thing on the list
                                        foreach (object nearbyobj in nearbylist)
                                        {
                                            resultstr = ApplyToProperty(spawner, nearbyobj, o, propname, arglist[0]);
                                            if (resultstr == null)
                                                break;
                                        }
                                        if (resultstr != null)
                                        {
                                            status_str = "GETONNEARBY error: " + resultstr;
                                            no_error = false;
                                        }
                                    }
                                }
                                else
                                {
                                    status_str = "Invalid GETONNEARBY args : " + arglist[1];
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.GETONPARENT)
                            {
                                // syntax is GETONPARENT,property
                                // or  GETONPARENT,<ATTACHMENT,type,name,property>
                                // note this will be an arg to some property
                                if (value_keywordargs.Length > 1)
                                {
                                    object parent = null;
                                    if (refobject is Item)
                                    {
                                        parent = ((Item)refobject).Parent;
                                    }

                                    if (parent != null)
                                    {
                                        string resultstr = ApplyToProperty(spawner, parent, o, value_keywordargs[1], arglist[0]);
                                        if (resultstr != null)
                                        {
                                            status_str = "GETONPARENT error: " + resultstr;
                                            no_error = false;
                                        }
                                    }
                                    else
                                    {
                                        no_error = false;
                                    }
                                }
                                else
                                {
                                    status_str = "Invalid GETONPARENT args : " + arglist[1];
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.GETONTHIS)
                            {
                                // syntax is GETONTHIS,property
                                // or  GETONTHIS,<ATTACHMENT,type,name,property>
                                // note this will be an arg to some property
                                if (value_keywordargs.Length > 1)
                                {
                                    if (refobject != null)
                                    {
                                        string resultstr = ApplyToProperty(spawner, refobject, o, value_keywordargs[1], arglist[0]);
                                        if (resultstr != null)
                                        {
                                            status_str = "GETONTHIS error: " + resultstr;
                                            no_error = false;
                                        }
                                    }
                                    else
                                    {
                                        no_error = false;
                                    }
                                }
                                else
                                {
                                    status_str = "Invalid GETONTHIS args : " + arglist[1];
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.GETONSPAWN)
                            {
                                // syntax is GETONSPAWN[,spawername],subgroup,property
                                // or GETONSPAWN[,spawername],subgroup,<ATTACHMENT,type,name,property>
                                // note this will be an arg to some property

                                if (value_keywordargs.Length > 2)
                                {
                                    string subgroupstr = value_keywordargs[1];
                                    string propstr = value_keywordargs[2];
                                    string spawnerstr = null;
                                    if (value_keywordargs.Length > 3)
                                    {
                                        spawnerstr = value_keywordargs[1];
                                        subgroupstr = value_keywordargs[2];
                                        propstr = value_keywordargs[3];
                                    }
                                    // get the current property value
                                    //Type ptype;
                                    // get target object
                                    int subgroup;
                                    if (!int.TryParse(subgroupstr, out subgroup))
                                        subgroup = -1;
                                    if (subgroup == -1)
                                    {
                                        status_str = "Invalid subgroup in GETONSPAWN";
                                        no_error = false;
                                    }
                                    else
                                    {
                                        if (spawnerstr != null)
                                        {
                                            spawner = FindSpawnerByName(spawner, spawnerstr);
                                            if (spawner == null)
                                            {
                                                status_str = "Invalid spawnername in GETONSPAWN";
                                                no_error = false;
                                            }
                                        }
                                        object targetobj = XmlSpawner.GetSpawned(spawner, subgroup);
                                        if (targetobj != null)
                                        {
                                            string resultstr = ApplyToProperty(spawner, targetobj, o, propstr, arglist[0]);
                                            if (resultstr != null)
                                            {
                                                status_str = "GETONSPAWN error: " + resultstr;
                                                no_error = false;
                                            }
                                        }
                                        else
                                        {
                                            no_error = false;
                                        }
                                    }
                                }
                                else
                                {
                                    status_str = "Invalid GETONSPAWN args : " + arglist[1];
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.GETFROMFILE)
                            {
                                // syntax is GETFROMFILE,filename
                                if (value_keywordargs.Length > 1)
                                {

                                    string filename = value_keywordargs[1];
                                    string filestring = null;

                                    // read in the string from the file
                                    if (File.Exists(filename) == true)
                                    {
                                        try
                                        {
                                            // Create an instance of StreamReader to read from a file.
                                            // The using statement also closes the StreamReader.
                                            using (StreamReader sr = new StreamReader(filename))
                                            {
                                                string line;
                                                // Read and display lines from the file until the end of
                                                // the file is reached.
                                                while ((line = sr.ReadLine()) != null)
                                                {
                                                    filestring += line;
                                                }

                                                sr.Close();
                                            }
                                        }
                                        catch
                                        {
                                            status_str = "GETFROMFILE error: " + filename;
                                            no_error = false;
                                        }
                                        // set the property value 
                                        string result = SetPropertyValue(spawner, o, arglist[0], filestring);

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
                                    status_str = "Invalid GETFROMFILE args : " + arglist[1];
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.GETACCOUNTTAG)
                            {
                                // syntax is GETACCOUNTTAG,tagname

                                if (value_keywordargs.Length > 1)
                                {

                                    string tagname = value_keywordargs[1];
                                    string tagvalue = null;

                                    // get the value of the account tag from the triggering mob
                                    if (trigmob != null && !trigmob.Deleted)
                                    {
                                        Account acct = trigmob.Account as Account;
                                        if (acct != null)
                                        {
                                            tagvalue = acct.GetTag(tagname);
                                        }
                                    }
                                    else
                                    {
                                        no_error = false;
                                    }

                                    if (tagvalue != null)
                                    {
                                        // set the property value 
                                        string result = SetPropertyValue(spawner, o, arglist[0], tagvalue);

                                        // see if it was successful
                                        if (result != "Property has been set.")
                                        {
                                            status_str = arglist[0] + " : " + result;
                                            no_error = false;
                                        }
                                    }
                                    else
                                    {
                                        status_str = "Invalid GETACCOUNTTAG tagname : " + tagname;
                                        no_error = false;
                                    }
                                }
                                else
                                {
                                    status_str = "Invalid GETACCOUNTTAG args : " + arglist[1];
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.MUL)
                            {
                                // increment the property value by the amount.  Use the format propname/MUL,min,max/ or propname/MUL,value
                                if (value_keywordargs.Length > 1)
                                {
                                    // get a random number
                                    string incvalue = "0";
                                    if (value_keywordargs.Length > 2)
                                    {
                                        double d0, d1;

                                        if (double.TryParse(value_keywordargs[1], NumberStyles.Any, CultureInfo.InvariantCulture, out d0) && double.TryParse(value_keywordargs[2], NumberStyles.Any, CultureInfo.InvariantCulture, out d1))
                                        {
                                            incvalue = string.Format("{0}", Utility.RandomMinMax((int)(10000 * d0), (int)(10000 * d1)) / 10000.0);
                                        }
                                        else { status_str = "Invalid MUL args : " + arglist[1]; no_error = false; }
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
                                        // should use the actual ptype info to do the multiplication.  Maybe later.
                                        double d0, d1;
                                        if (double.TryParse(currentvalue, NumberStyles.Any, CultureInfo.InvariantCulture, out d0) && double.TryParse(incvalue, NumberStyles.Any, CultureInfo.InvariantCulture, out d1))
                                        {
                                            tmpstr = ((int)(d0 * d1)).ToString();
                                        }
                                        else { status_str = "Invalid MUL args : " + arglist[1]; no_error = false; }

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
                                    status_str = "Invalid MUL args : " + arglist[1];
                                    no_error = false;
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.INC)
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
                            else if (kw == valuemodKeyword.AMOUNTCARRIED)
                            {
                                // syntax is AMOUNTCARRIED,itemtype[[,banksearch],itemname]
                                int amount = 0;

                                if (value_keywordargs.Length > 1)
                                {
                                    string typestr = value_keywordargs[1], namestr = "*";
                                    bool banksearch = false;
                                    if (value_keywordargs.Length > 2)
                                    {
                                        if (!bool.TryParse(value_keywordargs[2], out banksearch))
                                        {
                                            status_str = "Invalid AMOUNTCARRIED banksearch boolean : " + arglist[1];
                                            no_error = false;
                                        }
                                        else if (value_keywordargs.Length > 3)
                                        {
                                            namestr = value_keywordargs[3];
                                        }
                                    }

                                    // get the list of items being carried of the specified type
                                    Type targetType = SpawnerType.GetType(typestr);

                                    if (targetType != null && trigmob != null && trigmob.Backpack != null)
                                    {
                                        Item[] items = trigmob.Backpack.FindItemsByType(targetType, true);

                                        for (int i = 0; i < items.Length; ++i)
                                        {
                                            if (CheckNameMatch(namestr, items[i].Name))
                                                amount += items[i].Amount;
                                        }
                                        if (banksearch && trigmob.BankBox != null)
                                        {
                                            items = trigmob.BankBox.FindItemsByType(targetType, true);
                                            for (int i = 0; i < items.Length; ++i)
                                            {
                                                if (CheckNameMatch(namestr, items[i].Name))
                                                    amount += items[i].Amount;
                                            }
                                        }
                                    }

                                    string result = SetPropertyValue(spawner, o, arglist[0], amount.ToString());

                                    // see if it was successful
                                    if (result != "Property has been set.")
                                    {
                                        status_str = arglist[0] + " : " + result;
                                        no_error = false;
                                    }
                                }
                                else
                                {
                                    status_str = "Invalid AMOUNTCARRIED args : " + arglist[1];
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
                            else if (kw == valuemodKeyword.TRIGSKILL)
                            {
                                if (value_keywordargs.Length > 1)
                                {
                                    if (spawner != null && spawner.TriggerSkill != null)
                                    {
                                        string skillstr = null;
                                        // syntax is TRIGSKILL,name|value|cap|base
                                        if (value_keywordargs[1].ToLower() == "name")
                                        {
                                            skillstr = spawner.TriggerSkill.Name;
                                        }
                                        else
                                            if (value_keywordargs[1].ToLower() == "value")
                                        {
                                            skillstr = spawner.TriggerSkill.Value.ToString();
                                        }
                                        else
                                                if (value_keywordargs[1].ToLower() == "cap")
                                        {
                                            skillstr = spawner.TriggerSkill.Cap.ToString();
                                        }
                                        else
                                                    if (value_keywordargs[1].ToLower() == "base")
                                        {
                                            skillstr = spawner.TriggerSkill.Base.ToString();
                                        }

                                        string result = SetPropertyValue(spawner, o, arglist[0], skillstr);
                                        // see if it was successful
                                        if (result != "Property has been set.")
                                        {
                                            status_str = arglist[0] + " : " + result;
                                            no_error = false;
                                        }
                                    }
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == valuemodKeyword.RANDNAME)
                            {
                                if (value_keywordargs.Length > 1)
                                {
                                    string result = SetPropertyValue(spawner, o, arglist[0], NameList.RandomName(value_keywordargs[1]));
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
                        }
                        else if (IsTypemodKeyword(keywordargs[0]))
                        {
                            typemodKeyword kw = typemodKeywordHash[keywordargs[0]];
                            //
                            //  EFFECT keyword
                            //
                            if (kw == typemodKeyword.EFFECT)
                            {
                                int effect = -1;
                                int duration = 1;
                                // syntax is EFFECT,itemid,duration,[x,y,z]
                                // try to get the effect argument
                                if (keywordargs.Length < 2)
                                {
                                    status_str = "Missing effect number";
                                    no_error = false;
                                }
                                else
                                {
                                    if (!int.TryParse(keywordargs[1], out effect))
                                    { status_str = "Improper effect number format"; no_error = false; }
                                }
                                if (keywordargs.Length > 2)
                                {
                                    if (!int.TryParse(keywordargs[2], out duration))
                                    { status_str = "Improper effect duration format"; no_error = false; }
                                }
                                // by default just use the spawn location
                                Point3D eloc;
                                Map emap = Map.Internal;
                                if (o is Mobile)
                                {
                                    eloc = ((Mobile)o).Location;
                                    emap = ((Mobile)o).Map;
                                }
                                else if (o is Item)
                                {
                                    eloc = ((Item)o).Location;
                                    emap = ((Item)o).Map;
                                }
                                else
                                {
                                    // should never get here
                                    eloc = new Point3D(0, 0, 0);
                                }

                                if (keywordargs.Length > 3)
                                {
                                    // is this applied to the trig mob or to a location?
                                    if (keywordargs.Length > 5)
                                    {
                                        int y = 0;
                                        int z = 0;
                                        int x;
                                        if (!int.TryParse(keywordargs[3], out x) || !int.TryParse(keywordargs[4], out y) || !int.TryParse(keywordargs[5], out z))
                                        { status_str = "Improper effect location format"; }
                                        eloc = new Point3D(x, y, z);
                                    }
                                }
                                if (effect >= 0 && emap != Map.Internal)
                                {
                                    Effects.SendLocationEffect(eloc, emap, effect, duration);
                                }
                                if (arglist.Length < 2) break;
                                remainder = singlearglist[1];
                            }
                            //
                            //  BOLTEFFECT keyword
                            //
                            else if (kw == typemodKeyword.BOLTEFFECT)
                            {
                                int sound = 0x29;
                                int hue = 0;
                                // syntax is BOLTEFFECT[,sound[,hue]]


                                // try to get the effect argument
                                if (keywordargs.Length > 1)
                                {
                                    if (!int.TryParse(keywordargs[1], out sound))
                                    { status_str = "Improper sound id"; no_error = false; }
                                }
                                if (keywordargs.Length > 2)
                                {
                                    if (!int.TryParse(keywordargs[2], out hue))
                                    { status_str = "Improper hue"; no_error = false; }
                                }
                                if (o is IEntity)
                                {
                                    SendBoltEffect((IEntity)o, sound, hue);
                                }

                                if (arglist.Length < 2) break;
                                remainder = singlearglist[1];
                            }
                            //
                            //  MEFFECT keyword
                            //
                            else if (kw == typemodKeyword.MEFFECT)
                            {
                                int effect = -1;
                                int duration = 0;
                                int speed = 1;
                                Point3D eloc1 = new Point3D(0, 0, 0);
                                Point3D eloc2 = new Point3D(0, 0, 0);
                                Map emap = Map.Internal;
                                bool hasloc = false;
                                // syntax is MEFFECT,itemid[,speed][,x,y,z][,x2,y2,z2]

                                // try to get the effect argument
                                if (keywordargs.Length < 2)
                                {
                                    status_str = "Missing effect number";
                                    no_error = false;
                                }
                                else
                                {
                                    if (!int.TryParse(keywordargs[1], out effect))
                                    { status_str = "Improper effect number format"; no_error = false; }
                                }
                                if (keywordargs.Length > 2)
                                {
                                    if (!int.TryParse(keywordargs[2], out speed))
                                    { status_str = "Improper effect speed format"; no_error = false; }
                                }

                                // by default just use the spawn location

                                if (o is Mobile)
                                {
                                    eloc2 = ((Mobile)o).Location;
                                    emap = ((Mobile)o).Map;
                                }
                                else if (o is Item)
                                {
                                    eloc2 = ((Item)o).Location;
                                    emap = ((Item)o).Map;
                                }

                                if (keywordargs.Length > 8)
                                {
                                    int y = 0;
                                    int z = 0;

                                    int x;
                                    if (!int.TryParse(keywordargs[3], out x) || !int.TryParse(keywordargs[4], out y) || !int.TryParse(keywordargs[5], out z))
                                    { status_str = "Improper effect location format"; }
                                    eloc1 = new Point3D(x, y, z);

                                    x = y = z = 0;
                                    if (!int.TryParse(keywordargs[6], out x) || !int.TryParse(keywordargs[7], out y) || !int.TryParse(keywordargs[8], out z))
                                    { status_str = "Improper effect location format"; }
                                    eloc2 = new Point3D(x, y, z);
                                    hasloc = true;
                                }
                                else
                                    if (keywordargs.Length > 5)
                                {
                                    int y = 0;
                                    int z = 0;
                                    int x;
                                    if (!int.TryParse(keywordargs[3], out x) || !int.TryParse(keywordargs[4], out y) || !int.TryParse(keywordargs[5], out z))
                                    { status_str = "Improper effect location format"; }
                                    eloc1 = new Point3D(x, y, z);
                                    hasloc = true;
                                }

                                if (effect >= 0 && hasloc && emap != Map.Internal)
                                {
                                    Effects.SendPacket(eloc1, emap, new HuedEffect(EffectType.Moving, -1, -1, effect, eloc1, eloc2, speed, duration, false, false, 0, 0));
                                }
                                else
                                    if (effect >= 0 && refobject is IEntity && o is IEntity)
                                {
                                    Effects.SendMovingEffect((IEntity)refobject, (IEntity)o, effect, speed, duration, false, false);
                                }
                                if (arglist.Length < 2) break;
                                remainder = singlearglist[1];
                            }
                            //
                            //  PEFFECT keyword
                            //
                            else if (kw == typemodKeyword.PEFFECT)
                            {
                                int effect = -1;
                                int duration = 1;
                                // syntax is PEFFECT,itemid,duration,[x,y,z]
                                // try to get the effect argument
                                if (keywordargs.Length < 2)
                                {
                                    status_str = "Missing effect number";
                                    no_error = false;
                                }
                                else
                                {
                                    if (!int.TryParse(keywordargs[1], out effect))
                                    { status_str = "Improper effect number format"; no_error = false; }
                                }
                                if (keywordargs.Length > 2)
                                {
                                    if (!int.TryParse(keywordargs[2], out duration))
                                    { status_str = "Improper effect duration format"; no_error = false; }
                                }
                                // by default just use the spawn location
                                Point3D eloc;
                                Map emap = Map.Internal;
                                if (o is Mobile)
                                {
                                    eloc = ((Mobile)o).Location;
                                    emap = ((Mobile)o).Map;
                                }
                                else if (o is Item)
                                {
                                    eloc = ((Item)o).Location;
                                    emap = ((Item)o).Map;
                                }
                                else
                                {
                                    // should never get here
                                    eloc = new Point3D(0, 0, 0);
                                }

                                if (keywordargs.Length > 3)
                                {
                                    // is this applied to the trig mob or to a location?
                                    if (keywordargs.Length > 5)
                                    {
                                        int y = 0;
                                        int z = 0;
                                        int x;
                                        if (!int.TryParse(keywordargs[3], out x) || !int.TryParse(keywordargs[4], out y) || !int.TryParse(keywordargs[5], out z))
                                        { status_str = "Improper effect location format"; }
                                        eloc = new Point3D(x, y, z);
                                    }
                                }
                                if (effect >= 0 && emap != Map.Internal)
                                {
                                    Effects.SendLocationEffect(eloc, emap, effect, duration);
                                }
                                if (arglist.Length < 2) break;
                                remainder = singlearglist[1];
                            }
                            //
                            //  Damage keyword
                            //
                            else if (kw == typemodKeyword.DAMAGE)
                            {
                                // the syntax is DAMAGE,damage,phys,fire,cold,pois,energy[,range][,playeronly]
                                ApplyDamageToPlayers(arglist[0], o as Mobile, o, out status_str);

                                //ApplyDamageToPlayers(arglist[0], trigmob, refobject, out status_str);
                                if (status_str != null)
                                {
                                    no_error = false;
                                }
                                if (arglist.Length < 2) break;
                                remainder = singlearglist[1];
                            }
                            else if (kw == typemodKeyword.ADD)
                            {
                                no_error = AddItemToTarget(spawner, o, keywordargs, arglist, trigmob, refobject, false, out remainder, out status_str);
                            }
                            else if (kw == typemodKeyword.EQUIP)
                            {
                                no_error = AddItemToTarget(spawner, o, keywordargs, arglist, trigmob, refobject, true, out remainder, out status_str);
                            }
                            else if (kw == typemodKeyword.DELETE)
                            {
                                if (o is Item)
                                {
                                    ((Item)o).Delete();
                                }
                                else if (o is Mobile)
                                {
                                    if (!((Mobile)o).Player)
                                    {
                                        ((Mobile)o).Delete();
                                    }
                                }

                                if (arglist.Length < 2) break;
                                remainder = singlearglist[1];

                            }
                            else if (kw == typemodKeyword.KILL)
                            {
                                if (o is Mobile)
                                {
                                    ((Mobile)o).Kill();
                                }

                                if (arglist.Length < 2) break;
                                remainder = singlearglist[1];
                            }
                            else if (kw == typemodKeyword.UNEQUIP)
                            {
                                // syntax is UNEQUIP,layer[,delete]

                                Layer layer = Layer.Invalid;
                                bool remove = false;

                                if (keywordargs.Length > 1)
                                {
                                    if (!TryParse(keywordargs[1], true, out layer))
                                    { status_str = "Invalid layer"; }
                                }

                                if (keywordargs.Length > 2)
                                {
                                    if (keywordargs[2] == "delete")
                                    {
                                        remove = true;
                                    }
                                    else bool.TryParse(keywordargs[2], out remove);
                                }

                                if (o is Mobile && layer != Layer.Invalid)
                                {
                                    Mobile m = (Mobile)o;
                                    // go through all of the items on the mobile
                                    List<Item> packlist = m.Items;

                                    for (int i = 0; i < packlist.Count; ++i)
                                    {
                                        Item item = packlist[i];

                                        //  check the layer
                                        // if it matches then unequip it
                                        if (item.Layer == layer)
                                        {
                                            if (remove)
                                            {
                                                item.Delete();
                                            }
                                            else
                                            {
                                                m.AddToBackpack(item);
                                            }
                                        }
                                    }
                                }

                                if (status_str != null)
                                {
                                    no_error = false;
                                }
                                if (arglist.Length < 2) break;
                                remainder = singlearglist[1];

                            }
                            else if (kw == typemodKeyword.MSG)
                            {
                                // syntax is MSG[,probability][,hue]

                                // if the object is a mobile then display a msg over the mob or item
                                double drop_probability = 1;
                                int hue = 0x3b2;
                                if (keywordargs.Length > 1)
                                {
                                    if (!double.TryParse(keywordargs[1], NumberStyles.Any, CultureInfo.InvariantCulture, out drop_probability))
                                    { status_str = "Invalid msg probability : " + arglist[1]; no_error = false; }
                                }
                                if (keywordargs.Length > 2)
                                {
                                    if (!int.TryParse(keywordargs[2], out hue))
                                    { status_str = "Invalid MSG hue : " + arglist[1]; no_error = false; }
                                }

                                if (hue < 0) hue = 0;

                                if (o is Mobile || o is Item)
                                {
                                    string msgstr = arglist[1];

                                    // test the drop probability
                                    if (Utility.RandomDouble() < drop_probability)
                                    {
                                        if (o is Mobile)
                                            ((Mobile)o).PublicOverheadMessage(MessageType.Regular, hue, false, msgstr);
                                        else
                                            if (o is Item)
                                            ((Item)o).PublicOverheadMessage(MessageType.Regular, hue, false, msgstr);

                                    }
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == typemodKeyword.ASCIIMSG)
                            {
                                // syntax is ASCIIMSG[,probability][,hue][,font]

                                // if the object is a mobile then display a msg over the mob or item
                                double drop_probability = 1;
                                int hue = 0x3b2;
                                int font = 3;
                                if (keywordargs.Length > 1)
                                {
                                    if (!double.TryParse(keywordargs[1], NumberStyles.Any, CultureInfo.InvariantCulture, out drop_probability))
                                    { status_str = "Invalid msg probability : " + arglist[1]; no_error = false; }
                                }
                                if (keywordargs.Length > 2)
                                {
                                    if (!int.TryParse(keywordargs[2], out hue))
                                    { status_str = "Invalid MSG hue : " + arglist[1]; no_error = false; }
                                }
                                if (keywordargs.Length > 3)
                                {
                                    if (!int.TryParse(keywordargs[3], out font))
                                    { status_str = "Invalid MSG font : " + arglist[1]; no_error = false; }
                                }
                                if (hue < 0) hue = 0;
                                if (font < 0) font = 0;

                                if (o is Mobile || o is Item)
                                {
                                    string msgstr = arglist[1];

                                    // test the drop probability
                                    if (Utility.RandomDouble() < drop_probability)
                                    {
                                        if (o is Mobile)
                                            PublicOverheadMobileMessage((Mobile)o, MessageType.Regular, hue, font, msgstr, true);
                                        else
                                            if (o is Item)
                                            PublicOverheadItemMessage((Item)o, MessageType.Regular, hue, font, msgstr);
                                    }
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == typemodKeyword.SENDMSG)
                            {
                                // syntax is SENDMSG[,probability][,hue]/msg

                                // if the object is a mobile then display a msg over the mob or item
                                double drop_probability = 1;
                                int hue = 0x3b2;
                                int font = 3;
                                if (keywordargs.Length > 1)
                                {
                                    if (!double.TryParse(keywordargs[1], NumberStyles.Any, CultureInfo.InvariantCulture, out drop_probability))
                                    { status_str = "Invalid msg probability : " + arglist[1]; no_error = false; }

                                }
                                if (keywordargs.Length > 2)
                                {
                                    if (!int.TryParse(keywordargs[2], out hue))
                                    { status_str = "Invalid SENDMSG hue : " + arglist[1]; no_error = false; }

                                }

                                if (hue < 0) hue = 0;

                                if (o is Mobile)
                                {
                                    string msgstr = arglist[1];

                                    // test the drop probability
                                    if (Utility.RandomDouble() < drop_probability)
                                    {
                                        ((Mobile)o).Send(new UnicodeMessage(Serial.MinusOne, -1, MessageType.Regular, hue, font, "ENU", "System", msgstr));
                                    }
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == typemodKeyword.SENDASCIIMSG)
                            {
                                // syntax is SENDASCIIMSG[,probability][,hue][,font]

                                // if the object is a mobile then display a msg over the mob or item
                                double drop_probability = 1;
                                int hue = 0x3b2;
                                int font = 3;
                                if (keywordargs.Length > 1)
                                {
                                    if (!double.TryParse(keywordargs[1], NumberStyles.Any, CultureInfo.InvariantCulture, out drop_probability))
                                    { status_str = "Invalid msg probability : " + arglist[1]; no_error = false; }
                                }
                                if (keywordargs.Length > 2)
                                {
                                    if (!int.TryParse(keywordargs[2], out hue))
                                    { status_str = "Invalid MSG hue : " + arglist[1]; no_error = false; }
                                }
                                if (keywordargs.Length > 3)
                                {
                                    if (!int.TryParse(keywordargs[3], out font))
                                    { status_str = "Invalid MSG font : " + arglist[1]; no_error = false; }
                                }
                                if (hue < 0) hue = 0;
                                if (font < 0) font = 0;

                                if (o is Mobile)
                                {
                                    string msgstr = arglist[1];

                                    // test the drop probability
                                    if (Utility.RandomDouble() < drop_probability)
                                    {
                                        ((Mobile)o).Send(new AsciiMessage(Serial.MinusOne, -1, MessageType.Regular, hue, font, "System", msgstr));
                                    }
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == typemodKeyword.SAY)
                            {
                                // if the object is a mobile then display a msg over the mob
                                double drop_probability = 1;
                                if (keywordargs.Length > 1)
                                {
                                    if (!double.TryParse(keywordargs[1], NumberStyles.Any, CultureInfo.InvariantCulture, out drop_probability))
                                    { status_str = "Invalid say probability : " + arglist[1]; no_error = false; }
                                }
                                if (o is Mobile)
                                {
                                    string msgstr = arglist[1];

                                    // test the drop probability
                                    if (Utility.RandomDouble() < drop_probability)
                                    {
                                        ((Mobile)o).Say(msgstr);
                                    }
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];

                            }
                            else if (kw == typemodKeyword.SPEECH)
                            {
                                // syntax is SPEECH[,probability][,keywordnumber]
                                // if the object is a mobile then have it speak with optional keyword arg
                                double drop_probability = 1;
                                if (keywordargs.Length > 1)
                                {
                                    if (!double.TryParse(keywordargs[1], NumberStyles.Any, CultureInfo.InvariantCulture, out drop_probability))
                                    { status_str = "Invalid speech probability : " + arglist[1]; no_error = false; }
                                }
                                int keyword_number = -1;
                                if (keywordargs.Length > 2)
                                {
                                    if (!int.TryParse(keywordargs[2], out keyword_number))
                                    { status_str = "Invalid keyword number : " + arglist[1]; no_error = false; }
                                }
                                if (o is Mobile)
                                {
                                    string msgstr = arglist[1];

                                    // test the drop probability
                                    if (Utility.RandomDouble() < drop_probability)
                                    {
                                        int[] keywordarray = new int[] { };
                                        if (keyword_number >= 0)
                                        {
                                            keywordarray = new int[] { keyword_number };
                                        }
                                        ((Mobile)o).DoSpeech(msgstr, keywordarray, MessageType.Regular, 0x3B2);
                                    }
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];

                            }
                            else if (kw == typemodKeyword.OFFSET)
                            {
                                // syntax is OFFSET,x,y[,z]
                                // shift the location of the object by the specified amount

                                int xoffset = 0;
                                int yoffset = 0;
                                int zoffset = 0;

                                if (keywordargs.Length > 2)
                                {
                                    if (!int.TryParse(keywordargs[1], out xoffset) || !int.TryParse(keywordargs[2], out yoffset))
                                    { status_str = "Invalid xy offset : " + arglist[1]; no_error = false; }
                                }

                                if (keywordargs.Length > 3)
                                {
                                    if (!int.TryParse(keywordargs[3], out zoffset))
                                    { status_str = "Invalid zoffset : " + arglist[1]; no_error = false; }
                                }

                                if (o is Mobile)
                                {
                                    Point3D loc = ((Mobile)o).Location;
                                    ((Mobile)o).Location = new Point3D(loc.X + xoffset, loc.Y + yoffset, loc.Z + zoffset);
                                }
                                else if (o is Item)
                                {
                                    Point3D loc = ((Item)o).Location;
                                    ((Item)o).Location = new Point3D(loc.X + xoffset, loc.Y + yoffset, loc.Z + zoffset);
                                }

                                if (arglist.Length < 2) break;
                                remainder = singlearglist[1];

                            }
                            else if (kw == typemodKeyword.ANIMATE)
                            {
                                // syntax is ANIMATE,action[,framecount][,repeatcount][,forward true/false][,repeat true/false][delay]
                                // Animate( int action, int frameCount, int repeatCount, bool forward, bool repeat, int delay )
                                int action = -1;
                                int framecount = 7;
                                int repeatcount = 1;
                                bool forward = true;
                                bool repeat = false;
                                int delay = 0;
                                if (keywordargs.Length > 1)
                                {
                                    if (!int.TryParse(keywordargs[1], out action))
                                    { status_str = "Invalid action : " + arglist[1]; no_error = false; action = -1; }
                                }

                                if (keywordargs.Length > 2)
                                {
                                    if (!int.TryParse(keywordargs[2], out framecount))
                                    { status_str = "Invalid framecount : " + arglist[1]; no_error = false; framecount = 7; }
                                }

                                if (keywordargs.Length > 3)
                                {
                                    if (!int.TryParse(keywordargs[3], out repeatcount))
                                    { status_str = "Invalid repeatcount : " + arglist[1]; no_error = false; repeatcount = 1; }
                                }

                                if (keywordargs.Length > 4)
                                {
                                    if (!bool.TryParse(keywordargs[4], out forward))
                                    { status_str = "Invalid forward : " + arglist[1]; no_error = false; forward = true; }
                                }

                                if (keywordargs.Length > 5)
                                {
                                    if (!bool.TryParse(keywordargs[5], out repeat))
                                    { status_str = "Invalid repeat : " + arglist[1]; no_error = false; }
                                }

                                if (keywordargs.Length > 6)
                                {
                                    if (!int.TryParse(keywordargs[6], out delay))
                                    { status_str = "Invalid delay : " + arglist[1]; no_error = false; }
                                }

                                if (o is Mobile && action >= 0)
                                {
                                    ((Mobile)o).Animate(action, framecount, repeatcount, forward, repeat, delay);
                                }

                                if (arglist.Length < 2) break;
                                remainder = singlearglist[1];
                            }
                            else if (kw == typemodKeyword.FACETO)
                            {
                                // the mobile will turn to the direction coordinates
                                // syntax is FACETO,x,y
                                if (o is Mobile)
                                {
                                    Mobile m = (Mobile)o;
                                    int dx = m.X, dy = m.Y;
                                    if (keywordargs.Length > 2)
                                    {
                                        int.TryParse(keywordargs[1], out dx);
                                        int.TryParse(keywordargs[2], out dy);
                                    }
                                    m.Direction = m.GetDirectionTo(dx, dy);
                                }

                                if (arglist.Length < 2) break;
                                remainder = singlearglist[1];
                            }
                            else if (kw == typemodKeyword.FLASH)
                            {
                                //the mobile, only player, will get a flash effect
                                //syntax is FLASH,int (from 1 to 5)
                                // 1 is fade to black
                                // 2 is fade to white
                                // 3 is light flash
                                // 4 is light to black flash
                                // 5 is black flash
                                if (o is PlayerMobile)
                                {
                                    PlayerMobile m = (PlayerMobile)o;
                                    if (m.NetState != null && m.NetState.Running)
                                    {
                                        short flash;
                                        if (short.TryParse(keywordargs[1], out flash) && flash > 0 && flash < 6)
                                        {
                                            ScreenEffect se = new ScreenEffect((ScreenEffectType)(flash - 1));
                                            if (se != null)
                                            {
                                                Packet.Acquire(se);
                                                m.Send(se);

                                            }
                                            Packet.Release(se);
                                        }
                                    }
                                }

                                if (arglist.Length < 2) break;
                                remainder = singlearglist[1];
                            }
                            else if (kw == typemodKeyword.PRIVMSG)
                            {
                                // syntax is PRIVMSG[,probability][,hue]

                                // if the object is a mobile actively connected display the message
                                double drop_probability = 1;
                                int hue = 0x3b2;
                                if (keywordargs.Length > 1)
                                {
                                    if (!double.TryParse(keywordargs[1], NumberStyles.Any, CultureInfo.InvariantCulture, out drop_probability))
                                    { status_str = "Invalid msg probability : " + arglist[1]; no_error = false; }
                                }
                                if (keywordargs.Length > 2)
                                {
                                    if (!int.TryParse(keywordargs[2], out hue))
                                    { status_str = "Invalid MSG hue : " + arglist[1]; no_error = false; }
                                }

                                if (hue < 0) hue = 0;

                                if (o is Mobile)
                                {
                                    string msgstr = arglist[1];

                                    // test the drop probability
                                    if (Utility.RandomDouble() < drop_probability)
                                    {
                                        Mobile mob = (Mobile)o;
                                        mob.PrivateOverheadMessage(MessageType.Regular, hue, false, msgstr, mob.NetState);
                                    }
                                }
                                if (arglist.Length < 3) break;
                                remainder = arglist[2];
                            }
                            else if (kw == typemodKeyword.BCAST)
                            {
                                // syntax is BCAST[,hue][,font]/message

                                int hue = 0x482;
                                int font = -1; // default unicode messages

                                if (keywordargs.Length > 1)
                                {
                                    if (!int.TryParse(keywordargs[1], out hue))
                                    { status_str = "Invalid hue : " + arglist[1]; no_error = false; hue = 0x482; }
                                }

                                if (keywordargs.Length > 2)
                                {
                                    if (!int.TryParse(keywordargs[2], out font))
                                    { status_str = "Invalid font : " + arglist[1]; no_error = false; font = -1; }
                                }

                                if (font >= 0)
                                {
                                    // broadcast an ascii message to all players
                                    BroadcastAsciiMessage(AccessLevel.Player, hue, font, arglist[1]);
                                }
                                else
                                {
                                    // broadcast a message to all players
                                    CommandHandlers.BroadcastMessage(AccessLevel.Player, hue, arglist[1]);
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
        public static bool TestMobProperty(XmlSpawner spawner, Mobile mobile, string testString, Mobile trigmob, out string status_str)
        {
            status_str = null;
            // now make sure the mobile itself is there
            if (mobile == null || mobile.Deleted)
            {
                return false;
            }

            bool testreturn = CheckPropertyString(spawner, mobile, testString, trigmob, out status_str);

            return testreturn;
        }

        public static bool TestItemProperty(XmlSpawner spawner, Item ObjectPropertyItem, string testString, Mobile trigmob, out string status_str)
        {
            // now make sure the item itself is there
            if (ObjectPropertyItem == null || ObjectPropertyItem.Deleted)
            {
                status_str = "Trigger Object not found";
                return false;
            }

            bool testreturn = CheckPropertyString(spawner, ObjectPropertyItem, testString, trigmob, out status_str);

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
            else
            {
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
            }

            return null;
        }

        public static string ParseForKeywords(XmlSpawner spawner, object o, string valstr, Mobile trigmob, bool literal, out Type ptype)
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
            if (groupargstring != null && groupargstring.Length > 0)
            {
                if (arglist != null && arglist.Length > 0)
                    arglist[arglist.Length - 1] = groupargstring;
            }


            string pname = arglist[0].Trim();
            char startc = str[0];

            // first see whether it is a standard numeric value
            if ((startc == '.') || (startc == '-') || (startc == '+') || (startc >= '0' && startc <= '9'))
            {
                // determine the type
                if (str.IndexOf(".") >= 0)
                {
                    ptype = typeof(double);
                }
                else
                {
                    ptype = typeof(int);
                }
                return str;
            }
            else if (startc == '"' || startc == '(')
            {
                ptype = typeof(string);
                return str;
            }
            else if (startc == '#')
            {
                ptype = typeof(string);
                return str.Substring(1);
            }
            // or a bool
            else if ((str.ToLower()) == "true" || (str.ToLower() == "false"))
            {
                ptype = typeof(bool);
                return str;
            }
            // then look for a keyword
            else if (IsValueKeyword(pname))
            {
                valueKeyword kw = valueKeywordHash[pname];

                //if(pname == "GETONMOB" && arglist.Length > 2)
                if ((kw == valueKeyword.GETONMOB) && arglist.Length > 2)
                {
                    // syntax is GETONMOB,mobname[,mobtype],property

                    string propname = arglist[2];
                    string typestr = null;

                    if (arglist.Length > 3)
                    {
                        typestr = arglist[2];
                        propname = arglist[3];
                    }

                    Mobile testmobile = FindMobileByName(spawner, arglist[1], typestr);

                    string getvalue = GetPropertyValue(spawner, testmobile, propname, out ptype);

                    return ParseGetValue(getvalue, ptype);
                }
                else if ((kw == valueKeyword.GET) && arglist.Length > 2)
                {
                    // syntax is GET,[itemname -OR- SETITEM][,itemtype],property

                    string propname = arglist[2];
                    string typestr = null;

                    if (arglist.Length > 3)
                    {
                        typestr = arglist[2];
                        propname = arglist[3];
                    }

                    // is the itemname a serialno?
                    object testitem = null;
                    if (arglist[1].StartsWith("0x"))
                    {
                        int serial;
                        if (!int.TryParse(arglist[1].Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out serial))
                            serial = -1;

                        if (serial >= 0)
                            testitem = World.FindEntity(serial);
                    }
                    else if (arglist[1] == "SETITEM" && spawner != null && !spawner.Deleted && spawner.SetItem != null)
                    {
                        testitem = spawner.SetItem;
                    }
                    else
                    {
                        testitem = FindItemByName(spawner, arglist[1], typestr);
                    }

                    string getvalue = GetPropertyValue(spawner, testitem, propname, out ptype);

                    return ParseGetValue(getvalue, ptype);
                }
                else if ((kw == valueKeyword.GETONCARRIED) && arglist.Length > 2)
                {
                    // syntax is GETONCARRIED,itemname[,itemtype][,equippedonly],property

                    string propname = arglist[2];
                    string typestr = null;
                    bool equippedonly = false;

                    // if the itemtype arg has been specified then check it
                    if (arglist.Length > 3)
                    {
                        propname = arglist[3];
                        typestr = arglist[2];
                    }
                    if (arglist.Length > 4)
                    {
                        propname = arglist[4];
                        if (arglist[3].ToLower() == "equippedonly")
                        {
                            equippedonly = true;
                        }
                        else
                        {
                            bool.TryParse(arglist[3], out equippedonly);
                        }
                    }

                    Item testitem = SearchMobileForItem(trigmob, ParseObjectType(arglist[1]), typestr, false, equippedonly);

                    string getvalue = GetPropertyValue(spawner, testitem, propname, out ptype);

                    return ParseGetValue(getvalue, ptype);
                }
                else if ((kw == valueKeyword.GETONNEARBY) && arglist.Length > 3)
                {
                    // syntax is GETONNEARBY,range,name[,type][,searchcontainers],property
                    // or GETONNEARBY,range,name[,type][,searchcontainers],[ATTACHMENT,type,name,property]

                    string targetname = arglist[2];
                    string propname = arglist[3];
                    string typestr = null;
                    bool searchcontainers = false;
                    int range;
                    if (!int.TryParse(arglist[1], out range))
                        range = -1;

                    if (arglist.Length > 4)
                    {
                        typestr = arglist[3];
                        propname = arglist[4];
                    }

                    if (arglist.Length > 5)
                    {
                        bool.TryParse(arglist[4], out searchcontainers);
                        propname = arglist[5];
                    }

                    Type targettype = null;
                    if (typestr != null)
                    {
                        targettype = SpawnerType.GetType(typestr);
                    }

                    if (range >= 0)
                    {
                        // get all of the nearby objects
                        object relativeto = spawner;

                        List<object> nearbylist = GetNearbyObjects(relativeto, targetname, targettype, typestr, range, searchcontainers, null);

                        // apply the properties from the first valid thing on the list
                        foreach (object nearbyobj in nearbylist)
                        {
                            string getvalue = GetPropertyValue(spawner, nearbyobj, propname, out ptype);
                            return ParseGetValue(getvalue, ptype);
                        }
                    }
                    else
                        return null;
                }
                else if ((kw == valueKeyword.GETONTRIGMOB) && arglist.Length > 1)
                {
                    // syntax is GETONTRIGMOB,property
                    string getvalue = GetPropertyValue(spawner, trigmob, arglist[1], out ptype);

                    return ParseGetValue(getvalue, ptype);
                }
                else if ((kw == valueKeyword.GETONPARENT) && arglist.Length > 1)
                {
                    // syntax is GETONPARENT,property

                    string getvalue = null;

                    if (o is Item)
                    {
                        getvalue = GetPropertyValue(spawner, ((Item)o).Parent, arglist[1], out ptype);
                    }

                    return ParseGetValue(getvalue, ptype);
                }
                else if ((kw == valueKeyword.GETONTHIS) && arglist.Length > 1)
                {
                    // syntax is GETONTHIS,property

                    string getvalue = GetPropertyValue(spawner, o, arglist[1], out ptype);

                    return ParseGetValue(getvalue, ptype);
                }
                else if ((kw == valueKeyword.GETONSPAWN) && arglist.Length > 2)
                {
                    // syntax is GETONSPAWN[,spawnername],subgroup,property
                    // get the target from the spawn list
                    string subgroupstr = arglist[1];
                    string propstr = arglist[2];
                    string spawnerstr = null;

                    if (arglist.Length > 3)
                    {
                        spawnerstr = arglist[1];
                        subgroupstr = arglist[2];
                        propstr = arglist[3];
                    }

                    int subgroup;
                    if (!int.TryParse(subgroupstr, out subgroup))
                        subgroup = -1;

                    if (subgroup == -1) return null;

                    if (spawnerstr != null)
                    {
                        spawner = FindSpawnerByName(spawner, spawnerstr);
                    }

                    // check for the special COUNT property keyword
                    if (propstr == "COUNT")
                    {
                        ptype = typeof(int);

                        // get all of the currently active spawns with the specified subgroup
                        List<object> so = XmlSpawner.GetSpawnedList(spawner, subgroup);

                        if (so == null) return "0";

                        // and return the count
                        return so.Count.ToString();
                    }
                    else
                    {
                        object targetobj = XmlSpawner.GetSpawned(spawner, subgroup);
                        if (targetobj == null) return null;

                        string getvalue = GetPropertyValue(spawner, targetobj, propstr, out ptype);

                        return ParseGetValue(getvalue, ptype);
                    }
                }
                else if ((kw == valueKeyword.GETFROMFILE) && arglist.Length > 1)
                {
                    // syntax is GETFROMFILE,filename
                    ptype = typeof(string);

                    string filename = arglist[1];
                    string filestring = null;

                    // read in the string from the file
                    if (File.Exists(filename) == true)
                    {
                        try
                        {
                            // Create an instance of StreamReader to read from a file.
                            // The using statement also closes the StreamReader.
                            using (StreamReader sr = new StreamReader(filename))
                            {
                                string line;
                                // Read and display lines from the file until the end of
                                // the file is reached.
                                while ((line = sr.ReadLine()) != null)
                                {
                                    filestring += line;
                                }
                                sr.Close();
                            }
                        }
                        catch { }
                    }

                    return filestring;
                }
                else if ((kw == valueKeyword.GETACCOUNTTAG) && arglist.Length > 1)
                {
                    // syntax is GETACCOUNTTAG,tagname
                    ptype = typeof(string);

                    string tagname = arglist[1];
                    string tagvalue = null;

                    // get the value of the account tag from the triggering mob
                    if (trigmob != null && !trigmob.Deleted)
                    {
                        Account acct = trigmob.Account as Account;
                        if (acct != null)
                        {
                            tagvalue = '"' + acct.GetTag(tagname) + '"';
                        }
                    }
                    return tagvalue;
                }
                else if ((kw == valueKeyword.RND) && arglist.Length > 2)
                {
                    // syntax is RND,min,max
                    string randvalue = "0";
                    ptype = typeof(int);
                    int min, max;
                    if (int.TryParse(arglist[1], out min) && int.TryParse(arglist[2], out max))
                        randvalue = string.Format("{0}", Utility.RandomMinMax(min, max));
                    // return the random number as the value
                    return randvalue;
                }
                else if ((kw == valueKeyword.RNDBOOL))
                {
                    // syntax is RNDBOOL

                    ptype = typeof(bool);

                    // return the random number as the value
                    return Utility.RandomBool().ToString();
                }
                else if ((kw == valueKeyword.RNDLIST) && arglist.Length > 1)
                {
                    // syntax is RNDLIST,val1,val2,...

                    ptype = typeof(int);

                    // compute a random index into the arglist

                    int randindex = Utility.Random(1, arglist.Length - 1);

                    // return the list entry as the value

                    return arglist[randindex];

                }
                else if ((kw == valueKeyword.RNDSTRLIST) && arglist.Length > 1)
                {
                    // syntax is RNDSTRLIST,val1,val2,...
                    ptype = typeof(string);

                    // compute a random index into the arglist

                    int randindex = Utility.Random(1, arglist.Length - 1);
                    if (trigmob != null)
                    {
                        if (arglist[randindex].Contains("$TRIGNAME"))
                            arglist[randindex] = arglist[randindex].Replace("$TRIGNAME", trigmob.Name);
                    }

                    // return the list entry as the value

                    return arglist[randindex];
                }
                else if ((kw == valueKeyword.AMOUNTCARRIED) && arglist.Length > 1)
                {
                    // syntax is AMOUNTCARRIED,itemtype[,banksearch[,itemname]]

                    ptype = typeof(int);

                    int amount = 0;

                    string typestr = arglist[1];
                    if (typestr != null)
                    {
                        string namestr = "*";
                        bool banksearch = false;
                        if (arglist.Length > 2)
                        {
                            if (!bool.TryParse(arglist[2], out banksearch) && arglist[2].ToLowerInvariant() != "banksearch")
                            {
                                return null;
                            }
                            else if (arglist.Length > 3)
                            {
                                namestr = arglist[3];
                            }
                        }

                        // get the list of items being carried of the specified type
                        Type targetType = SpawnerType.GetType(typestr);

                        if (targetType != null && trigmob != null && trigmob.Backpack != null)
                        {
                            Item[] items = trigmob.Backpack.FindItemsByType(targetType, true);

                            for (int i = 0; i < items.Length; ++i)
                            {
                                if (CheckNameMatch(namestr, items[i].Name))
                                    amount += items[i].Amount;
                            }
                            if (banksearch && trigmob.BankBox != null)
                            {
                                items = trigmob.BankBox.FindItemsByType(targetType, true);
                                for (int i = 0; i < items.Length; ++i)
                                {
                                    if (CheckNameMatch(namestr, items[i].Name))
                                        amount += items[i].Amount;
                                }
                            }
                        }
                    }

                    return amount.ToString();
                }
                else if ((kw == valueKeyword.PLAYERSINRANGE) && arglist.Length > 1)
                {
                    // syntax is PLAYERSINRANGE,range

                    ptype = typeof(int);

                    int nplayers = 0;
                    int range;
                    // get the number of players in range
                    int.TryParse(arglist[1], out range);

                    // count nearby players
                    if (spawner != null && spawner.SpawnRegion != null && range < 0)
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
                else if ((kw == valueKeyword.TRIGSKILL) && arglist.Length > 1)
                {
                    if (spawner != null && spawner.TriggerSkill != null)
                    {
                        // syntax is TRIGSKILL,name|value|cap|base
                        if (arglist[1].ToLower() == "name")
                        {
                            ptype = typeof(string);
                            return spawner.TriggerSkill.Name;
                        }
                        else if (arglist[1].ToLower() == "value")
                        {
                            ptype = typeof(double);
                            return spawner.TriggerSkill.Value.ToString();
                        }
                        else if (arglist[1].ToLower() == "cap")
                        {
                            ptype = typeof(double);
                            return spawner.TriggerSkill.Cap.ToString();
                        }
                        else if (arglist[1].ToLower() == "base")
                        {
                            ptype = typeof(double);
                            return spawner.TriggerSkill.Base.ToString();
                        }
                    }

                    return null;
                }
                if ((kw == valueKeyword.RANDNAME) && arglist.Length > 1)
                {
                    // syntax is RANDNAME,nametype
                    return NameList.RandomName(arglist[1]);
                }
                else
                {
                    // an invalid keyword format will be passed as literal
                    return str;
                }
            }
            else if (literal)
            {
                ptype = typeof(string);
                return str;
            }
            else
            {
                // otherwise treat it as a property name
                string result = GetPropertyValue(spawner, o, pname, out ptype);

                return ParseGetValue(result, ptype);
            }
        }

        public static string ParseGetValue(string str, Type ptype)
        {
            // the results of getPropertyValue takes the form
            // propname = value
            // or
            // propname = value (hexvalue)

            if (str == null) return null;

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
                else
                {
                    // for everything else
                    // pass on as is
                    return arglist[1].Trim();
                }
            }
            else
            {
                return null;
            }
        }

        public static bool CheckSubstitutedPropertyString(XmlSpawner spawner, object o, string testString, Mobile trigmob, out string status_str)
        {
            string substitutedtest = ApplySubstitution(spawner, o, trigmob, testString);

            return CheckPropertyString(spawner, o, substitutedtest, trigmob, out status_str);
        }

        public static bool CheckPropertyString(XmlSpawner spawner, object o, string testString, Mobile trigmob, out string status_str)
        {
            status_str = null;

            if (o == null) return false;

            if (testString == null || testString.Length < 1)
            {
                status_str = "Null property test string";
                return false;
            }
            // parse the property test string for and(&)/or(|) operators
            string[] arglist = ParseString(testString, 2, "&|");
            if (arglist.Length < 2)
            {
                bool returnval = CheckSingleProperty(spawner, o, testString, trigmob, out status_str);

                // simple conditional test with no and/or operators
                return returnval;
            }
            else
            {
                // test each half independently and combine the results
                bool first = CheckSingleProperty(spawner, o, arglist[0], trigmob, out status_str);

                // this will recursively parse the property test string with implicit nesting for multiple logical tests of the
                // form A * B * C * D    being grouped as A * (B * (C * D))
                bool second = CheckPropertyString(spawner, o, arglist[1], trigmob, out status_str);

                int andposition = testString.IndexOf("&");
                int orposition = testString.IndexOf("|");

                // combine them based upon the operator
                if ((andposition > 0 && orposition <= 0) || (andposition > 0 && andposition < orposition))
                {
                    // and operator
                    return (first && second);
                }
                else
                    if ((orposition > 0 && andposition <= 0) || (orposition > 0 && orposition < andposition))
                {
                    // or operator
                    return (first || second);
                }
                else
                {
                    // should never get here
                    return false;
                }
            }
        }

        public static bool CheckSingleProperty(XmlSpawner spawner, object o, string testString, Mobile trigmob, out string status_str)
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

            string value1 = ParseForKeywords(spawner, o, arglist[0].Trim(), trigmob, false, out ptype1);

            // see if it was successful
            if (ptype1 == null)
            {
                status_str = arglist[0] + " : " + value1;

                return invertreturn;
                //return false;
            }

            string value2 = ParseForKeywords(spawner, o, arglist[1].Trim(), trigmob, false, out ptype2);

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
                else if (haslessthan)
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
                else if (haslessthan)
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
                else if (haslessthan)
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
                else if (haslessthan)
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
                else if (haslessthan)
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
                else if (haslessthan)
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
                else if (haslessthan)
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

        private static bool CheckNameMatch(string targetname, string name)
        {
            // a "*" targetname will match anything
            // a null or empty targetname will match a null name
            // otherwise the strings must match
            return (targetname == "*") || (name == targetname) || (targetname != null && targetname.Length == 0 && name == null);
        }

        private static void GetItemsIn(Item source, string targetname, Type targettype, string typestr, ref List<object> nearbylist, string proptest)
        {
            string status_str;
            if (source != null && source.Items != null && nearbylist != null)
            {
                foreach (Item i in source.Items)
                {
                    // check the type and name

                    Type itemtype = i.GetType();

                    if (!i.Deleted && CheckNameMatch(targetname, i.Name) && (typestr == null ||
                        (itemtype != null && targettype != null && (itemtype.Equals(targettype) || itemtype.IsSubclassOf(targettype)))))
                    {
                        if (proptest == null || CheckPropertyString(null, i, proptest, null, out status_str))
                            nearbylist.Add(i);
                    }

                    if (i is Container)
                    {
                        GetItemsIn(i, targetname, targettype, typestr, ref nearbylist, proptest);
                    }
                }

            }
        }

        private static List<object> GetNearbyObjects(object invoker, string targetname, Type targettype, string typestr, int range, bool searchcontainers, string proptest)
        {
            IPooledEnumerable itemlist = null;
            IPooledEnumerable mobilelist = null;
            List<object> nearbylist = new List<object>();
            string status_str;

            // get nearby items
            if (targettype == null || targettype == typeof(Item) || targettype.IsSubclassOf(typeof(Item)))
            {
                if (invoker is Item)
                {
                    itemlist = ((Item)invoker).GetItemsInRange(range);
                }
                else
                    if (invoker is Mobile)
                {
                    itemlist = ((Mobile)invoker).GetItemsInRange(range);
                }


                if (itemlist != null)
                {
                    foreach (Item i in itemlist)
                    {
                        // check the type and name

                        Type itemtype = i.GetType();

                        if (searchcontainers)
                        {
                            if (i is Container)
                                GetItemsIn(i, targetname, targettype, typestr, ref nearbylist, proptest);
                        }
                        else
                            if (!i.Deleted && CheckNameMatch(targetname, i.Name) && (typestr == null ||
                                (itemtype != null && targettype != null && (itemtype.Equals(targettype) || itemtype.IsSubclassOf(targettype)))))
                        {
                            if (proptest == null || CheckPropertyString(null, i, proptest, null, out status_str))
                                nearbylist.Add(i);
                        }

                    }
                    itemlist.Free();
                }
            }

            // get nearby mobiles
            if (targettype == null || targettype == typeof(Mobile) || targettype.IsSubclassOf(typeof(Mobile)))
            {
                if (invoker is Item)
                {
                    mobilelist = ((Item)invoker).GetMobilesInRange(range);
                }
                else
                    if (invoker is Mobile)
                {
                    mobilelist = ((Mobile)invoker).GetMobilesInRange(range);
                }

                if (mobilelist != null)
                {
                    foreach (Mobile m in mobilelist)
                    {

                        // check the type and name
                        Type mobtype = m.GetType();

                        if (!m.Deleted && CheckNameMatch(targetname, m.Name) && (typestr == null ||
                            (mobtype != null && targettype != null && (mobtype.Equals(targettype) || mobtype.IsSubclassOf(targettype)))))
                        {
                            if (proptest == null || CheckPropertyString(null, m, proptest, null, out status_str))
                                nearbylist.Add(m);
                        }
                    }
                    mobilelist.Free();
                }
            }
            return nearbylist;
        }

        public static Item SearchMobileForItem(Mobile m, string targetName, string typeStr, bool searchbank)
        {
            return SearchMobileForItem(m, targetName, typeStr, searchbank, false);
        }


        public static Item SearchMobileForItem(Mobile m, string targetName, string typeStr, bool searchbank, bool equippedonly)
        {
            if (m != null && !m.Deleted)
            {
                // go through all of the items in the pack
                List<Item> packlist = m.Items;

                for (int i = 0; i < packlist.Count; ++i)
                {
                    Item item = packlist[i];

                    // dont search bank boxes
                    if (item is BankBox && !searchbank && !equippedonly) continue;

                    // recursively search containers
                    if (item != null && !item.Deleted)
                    {
                        if (item is Container && !equippedonly)
                        {
                            Item itemTarget = SearchPackForItem((Container)item, targetName, typeStr);

                            if (itemTarget != null) return itemTarget;
                        }
                        // test the item name against the trigger string
                        // if a typestring has been specified then check against that as well
                        if (CheckNameMatch(targetName, item.Name))
                        {
                            if ((typeStr == null || CheckType(item, typeStr)))
                            {
                                //found it
                                return item;
                            }
                        }
                    }
                }
                // now check any item that might be held
                Item held = m.Holding;

                if (held != null && !held.Deleted && !equippedonly)
                {
                    if (held is Container)
                    {
                        Item itemTarget = SearchPackForItem((Container)held, targetName, typeStr);

                        if (itemTarget != null) return itemTarget;
                    }
                    // test the item name against the trigger string
                    if (CheckNameMatch(targetName, held.Name))
                    {
                        if (typeStr == null || CheckType(held, typeStr))
                        {
                            //found it
                            return held;
                        }
                    }
                }
            }
            return null;
        }

        public static List<Item> SearchMobileForItems(Mobile m, string targetName, string typeStr, bool searchbank, bool equippedonly)
        {
            List<Item> itemlist = new List<Item>();
            if (m != null && !m.Deleted)
            {
                // go through all of the items in the pack
                List<Item> packlist = m.Items;

                for (int i = 0; i < packlist.Count; ++i)
                {
                    Item item = packlist[i];

                    // dont search bank boxes
                    if (item is BankBox && (!searchbank || equippedonly)) continue;

                    // recursively search containers
                    if (item != null && !item.Deleted)
                    {
                        if (item is Container && !equippedonly)
                        {
                            itemlist.AddRange(SearchPackForItems((Container)item, targetName, typeStr));
                        }
                        // test the item name against the trigger string
                        // if a typestring has been specified then check against that as well
                        else if (CheckNameMatch(targetName, item.Name))
                        {
                            if ((typeStr == null || CheckType(item, typeStr)))
                            {
                                //found it
                                itemlist.Add(item);
                            }
                        }
                    }
                }
                // now check any item that might be held
                Item held = m.Holding;

                if (held != null && !held.Deleted && !equippedonly)
                {
                    if (held is Container)
                    {
                        itemlist.AddRange(SearchPackForItems((Container)held, targetName, typeStr));
                    }
                    // test the item name against the trigger string
                    else if (CheckNameMatch(targetName, held.Name))
                    {
                        if (typeStr == null || CheckType(held, typeStr))
                        {
                            //found it
                            itemlist.Add(held);
                        }
                    }
                }
            }
            return itemlist;
        }

        public static Item SearchPackForItemType(Container pack, string targetName)
        {
            if (pack != null && !pack.Deleted && targetName != null && targetName.Length > 0)
            {
                Type targetType = SpawnerType.GetType(targetName);

                // go through all of the items in the pack
                List<Item> packlist = pack.Items;

                for (int i = 0; i < packlist.Count; ++i)
                {
                    Item item = packlist[i];

                    if (item != null && !item.Deleted)
                    {
                        if (item is Container)
                        {
                            Item itemTarget = SearchPackForItemType((Container)item, targetName);

                            if (itemTarget != null) return itemTarget;
                        }
                        // test the item name against the trigger string
                        if (item.GetType() == targetType)
                        {
                            //found it
                            return item;
                        }
                    }
                }
            }
            return null;
        }

        public static Item SearchMobileForItemType(Mobile m, string targetName, bool searchbank)
        {
            if (m != null && !m.Deleted && targetName != null && targetName.Length > 0)
            {
                Type targetType = SpawnerType.GetType(targetName);

                // go through all of the items in the pack
                List<Item> packlist = m.Items;

                for (int i = 0; i < packlist.Count; ++i)
                {
                    Item item = packlist[i];

                    // dont search bank boxes
                    if (item is BankBox && !searchbank) continue;

                    // recursively search containers
                    if (item != null && !item.Deleted)
                    {
                        if (item is Container)
                        {
                            Item itemTarget = SearchPackForItemType((Container)item, targetName);

                            if (itemTarget != null) return itemTarget;
                        }
                        // test the item type against the trigger string
                        if ((item.GetType() == targetType))
                        {
                            //found it
                            return item;
                        }
                    }
                }
                // now check any item that might be held
                Item held = m.Holding;

                if (held != null && !held.Deleted)
                {
                    if (held is Container)
                    {
                        Item itemTarget = SearchPackForItemType((Container)held, targetName);

                        if (itemTarget != null) return itemTarget;
                    }
                    // test the item name against the trigger string
                    if (held.GetType() == targetType)
                    {
                        //found it
                        return held;
                    }
                }
            }
            return null;
        }

        public static Item SearchPackForItem(Container pack, string targetName, string typestr)
        {
            if (pack != null && !pack.Deleted)
            {
                Type targettype = null;

                if (typestr != null)
                {
                    targettype = SpawnerType.GetType(typestr);
                }

                // go through all of the items in the pack
                List<Item> packlist = pack.Items;

                for (int i = 0; i < packlist.Count; ++i)
                {
                    Item item = packlist[i];

                    if (item != null && !item.Deleted)
                    {

                        if (item is Container)
                        {
                            Item itemTarget = SearchPackForItem((Container)item, targetName, typestr);

                            if (itemTarget != null) return itemTarget;
                        }
                        // test the item name against the trigger string
                        if (CheckNameMatch(targetName, item.Name))
                        {
                            if (targettype == null || (item.GetType().Equals(targettype) || item.GetType().IsSubclassOf(targettype)))
                            {
                                //found it
                                return item;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static List<Item> SearchPackForItems(Container pack, string targetName, string typestr)
        {
            List<Item> itemlist = new List<Item>();
            if (pack != null && !pack.Deleted)
            {
                Type targettype = null;

                if (typestr != null)
                {
                    targettype = SpawnerType.GetType(typestr);
                }

                // go through all of the items in the pack
                List<Item> packlist = pack.Items;

                for (int i = 0; i < packlist.Count; ++i)
                {
                    Item item = packlist[i];

                    if (item != null && !item.Deleted)
                    {

                        if (item is Container)
                        {
                            itemlist.AddRange(SearchPackForItems((Container)item, targetName, typestr));
                        }
                        // test the item name against the trigger string since it's not a container
                        else if (CheckNameMatch(targetName, item.Name))
                        {
                            if (targettype == null || (item.GetType().Equals(targettype) || item.GetType().IsSubclassOf(targettype)))
                            {
                                //found it
                                itemlist.Add(item);
                            }
                        }
                    }
                }
            }
            return itemlist;
        }

        public static List<Item> SearchPackListForItemType(Container pack, string targetName, List<Item> itemlist)
        {
            if (pack != null && !pack.Deleted && targetName != null && targetName.Length > 0)
            {
                Type targetType = SpawnerType.GetType(targetName);

                if (targetType == null) return null;

                // go through all of the items in the pack
                List<Item> packlist = pack.Items;

                for (int i = 0; i < packlist.Count; ++i)
                {
                    Item item = packlist[i];
                    if (item != null && !item.Deleted && item is Container)
                    {
                        itemlist = SearchPackListForItemType((Container)item, targetName, itemlist);
                    }
                    // test the item name against the trigger string
                    if (item != null && !item.Deleted && (item.GetType().IsSubclassOf(targetType) || item.GetType().Equals(targetType)))
                    {
                        //found it
                        itemlist.Add(item);
                    }
                }
            }
            return itemlist;
        }

        public static List<Item> FindItemListByType(Mobile m, string targetName, bool searchbank)
        {
            List<Item> itemlist = new List<Item>();

            if (m != null && !m.Deleted && targetName != null && targetName.Length > 0)
            {
                Type targetType = SpawnerType.GetType(targetName);

                // go through all of the items on the mobile
                List<Item> packlist = m.Items;

                for (int i = 0; i < packlist.Count; ++i)
                {
                    Item item = packlist[i];

                    // dont search bank boxes
                    if (item is BankBox && !searchbank) continue;

                    // recursively search containers
                    if (item != null && !item.Deleted)
                    {
                        if (item is Container)
                        {
                            itemlist = SearchPackListForItemType((Container)item, targetName, itemlist);
                        }
                        // test the item name against the trigger string
                        if (item.GetType().IsSubclassOf(targetType) || item.GetType().Equals(targetType))
                        {
                            //found it
                            // add the item to the list
                            itemlist.Add(item);
                        }
                    }
                }
                // now check any item that might be held
                Item held = m.Holding;

                if (held != null && !held.Deleted)
                {
                    if (held is Container)
                    {
                        itemlist = SearchPackListForItemType((Container)held, targetName, itemlist);
                    }
                    // test the item name against the trigger string
                    if (held.GetType().IsSubclassOf(targetType) || held.GetType().Equals(targetType))
                    {
                        //found it
                        // add the item to the list
                        itemlist.Add(held);
                    }
                }
            }
            return itemlist;
        }

        public static bool CheckForNotCarried(Mobile m, string objectivestr)
        {
            if (m == null || objectivestr == null) return true;

            // parse the objective string that might be of the form 'obj &| obj &| obj ...'
            string[] arglist = ParseString(objectivestr, 2, "&|");
            if (arglist.Length < 2)
            {
                // simple test with no and/or operators
                return SingleCheckForNotCarried(m, objectivestr);
            }
            else
            {
                // test each half independently and combine the results
                bool first = SingleCheckForNotCarried(m, arglist[0]);

                // this will recursively parse the property test string with implicit nesting for multiple logical tests of the
                // form A * B * C * D    being grouped as A * (B * (C * D))
                bool second = CheckForNotCarried(m, arglist[1]);

                int andposition = objectivestr.IndexOf("&");
                int orposition = objectivestr.IndexOf("|");

                // for the & operator
                // notrigger if
                // notcarrying A | notcarrying B
                // people will actually think of it as  not(carrying A | carrying B)
                // which is
                // notrigger if
                // notcarrying A && notcarrying B
                // similarly for the & operator

                // combine them based upon the operator
                if ((andposition > 0 && orposition <= 0) || (andposition > 0 && andposition < orposition))
                {
                    // and operator (see explanation above)
                    return (first || second);
                }
                else if ((orposition > 0 && andposition <= 0) || (orposition > 0 && orposition < andposition))
                {
                    // or operator (see explanation above)
                    return (first && second);
                }
                else
                {
                    // should never get here
                    return false;
                }
            }
        }

        public static bool SingleCheckForNotCarried(Mobile m, string objectivestr)
        {
            if (m == null || objectivestr == null) return true;

            bool has_no_such_item = true;

            // check to see whether there is an objective specification as well.  The format is name[,type][,EQUIPPED][,objective,objective,...]
            string[] objstr = ParseString(objectivestr, 8, ",");
            string itemname = objstr[0];

            bool equippedonly = false;
            string typestr = null;
            int objoffset = 1;
            // is there a type specification?

            while (objoffset < objstr.Length)
            {
                if (objstr[objoffset] != null && objstr[objoffset].Length > 0)
                {

                    char startc = objstr[objoffset][0];

                    if (startc >= '0' && startc <= '9')
                    {
                        // this is the start of the numeric objective specifications
                        break;
                    }
                    else
                        if (objstr[objoffset] == "EQUIPPED")
                    {
                        equippedonly = true;
                    }
                    else
                    {
                        // treat as a type specification if it does not begin with a numeric char
                        // and is not the EQUIPPED keyword
                        typestr = objstr[objoffset];
                    }
                }
                objoffset++;
            }

            // look for the item
            Item testitem = SearchMobileForItem(m, itemname, typestr, false, equippedonly);

            // found the item
            if (testitem != null)
            {
                // is the equippedonly flag set?  If so then see if the item is equipped
                if ((equippedonly && testitem.Parent == m) || !equippedonly)
                    has_no_such_item = false;

            }
            return has_no_such_item;
        }

        public static bool CheckForCarried(Mobile m, string objectivestr)
        {
            if (m == null || objectivestr == null) return true;

            // parse the objective string that might be of the form 'obj &| obj &| obj ...'
            string[] arglist = ParseString(objectivestr, 2, "&|");
            if (arglist.Length < 2)
            {
                // simple test with no and/or operators
                return SingleCheckForCarried(m, objectivestr);
            }
            else
            {
                // test each half independently and combine the results
                bool first = SingleCheckForCarried(m, arglist[0]);

                // this will recursively parse the property test string with implicit nesting for multiple logical tests of the
                // form A * B * C * D    being grouped as A * (B * (C * D))
                bool second = CheckForCarried(m, arglist[1]);

                int andposition = objectivestr.IndexOf("&");
                int orposition = objectivestr.IndexOf("|");

                // combine them based upon the operator
                if ((andposition > 0 && orposition <= 0) || (andposition > 0 && andposition < orposition))
                {
                    // and operator
                    return (first && second);
                }
                else if ((orposition > 0 && andposition <= 0) || (orposition > 0 && orposition < andposition))
                {
                    // or operator
                    return (first || second);
                }
                else
                {
                    // should never get here
                    return false;
                }
            }
        }

        public static bool SingleCheckForCarried(Mobile m, string objectivestr)
        {
            if (m == null || objectivestr == null) return false;

            bool has_valid_item = false;

            // check to see whether there is an objective specification as well.  The format is name[,type][,EQUIPPED][,objective,objective,...]
            string[] objstr = ParseString(objectivestr, 8, ",");

            string itemname = objstr[0];

            bool equippedonly = false;
            string typestr = null;
            int objoffset = 1;
            // is there a type specification?

            while (objoffset < objstr.Length)
            {
                if (objstr[objoffset] != null && objstr[objoffset].Length > 0)
                {

                    char startc = objstr[objoffset][0];

                    if (startc >= '0' && startc <= '9')
                    {
                        // this is the start of the numeric objective specifications
                        break;
                    }
                    else if (objstr[objoffset].ToLowerInvariant() == "equipped")
                    {
                        equippedonly = true;
                    }
                    else
                    {
                        // treat as a type specification if it does not begin with a numeric char
                        // and is not the EQUIPPED keyword
                        typestr = objstr[objoffset];
                    }
                }
                objoffset++;
            }

            Item testitem = SearchMobileForItem(m, itemname, typestr, false, equippedonly);

            // found the item
            if (testitem != null)
            {
                // is the equippedonly flag set?  If so then see if the item is equipped
                if ((equippedonly && testitem.Parent == m) || !equippedonly)
                    has_valid_item = true;

            }
            return has_valid_item;
        }

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
                        (itemtype != null && targettype != null && (itemtype.Equals(targettype) || itemtype.IsSubclassOf(targettype))))
                    {
                        founditem = item;
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
                AddToRecentItemSearchList(fromspawner, founditem);

                return (founditem);
            }
            else
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
                    (mobtype != null && targettype != null && (mobtype.Equals(targettype) || mobtype.IsSubclassOf(targettype)))))
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
            else
                return null;
        }

        public static XmlSpawner FindSpawnerByName(XmlSpawner fromspawner, string name)
        {
            if (name == null) return null;

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
            else
            {
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
                else
                    return null;
            }
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
                        (item.GetType() != null && targettype != null && (item.GetType().Equals(targettype) || item.GetType().IsSubclassOf(targettype))))
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
                        (m.GetType() != null && targettype != null && (m.GetType().Equals(targettype) || m.GetType().IsSubclassOf(targettype))))
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

        #region Add object methods

        public static bool AddItemToTarget(XmlSpawner spawner, object o, string[] keywordargs, string[] arglist, Mobile trigmob,
            object refobject, bool equip, out string remainder, out string status_str)
        {
            remainder = "";
            status_str = null;

            if (o == null || keywordargs == null || arglist == null) return false;

            // if the object is a mobile then add the item in the next arg to its pack.  Use the format /ADD,drop_probability/itemtype/
            // or /ADD,drop_probability/<itemtype/propname1/value1/propname2/value2/...>/
            double drop_probability = 1;

            if (keywordargs.Length > 1)
            {
                bool converterror = false;
                try { drop_probability = Convert.ToDouble(keywordargs[1], CultureInfo.InvariantCulture); }
                catch { status_str = "Invalid drop probability : " + arglist[1]; converterror = true; }

                if (converterror) return false;
            }

            Mobile m = null;
            if (o is Mobile || (o is Container))
            {
                Container pack = null;

                if (o is Mobile)
                {
                    m = (Mobile)o;

                    if (!m.Deleted)
                    {
                        pack = m.Backpack;

                        // auto add a pack if the mob doesnt have one
                        if (pack == null)
                        {
                            pack = new Backpack();
                            m.AddItem(pack);
                        }
                    }
                }
                else
                {
                    pack = o as Container;

                }

                if (pack != null)
                {
                    // handle the nested item property specification using <>
                    string itemargstring = null;

                    // itemtypestr will be the actual item type to be created.  In the simple form  it will be arglist[1]
                    // for the string /arg1/ADD/arg2/arg3/arg4/arg5 arglist [0] will contain ADD , arglist[1] will be arg2,
                    // and arglist[2] will be arg3/arg4/arg5
                    // if nested property specs
                    // arglist[1] will be <arg2 and arglist[2] will be arg3/ar4>/arg5
                    // the drop probability will be in probargs
                    string[] probargs = ParseCommaArgs(arglist[0], 2);

                    string itemtypestr = arglist[1];

                    // get the argument list after the < if any , note for the string /arg1/ADD/<arg2/arg3/arg4>/arg5 arglist [0] will contain ADD
                    // arglist[1] will be <arg2 and arglist[2] will be arg3/ar4>/arg5
                    // but note arglist[1] could also be <arg2>
                    // remainder will have ADD/<arg2/arg3/arg4>/arg5
                    //
                    // also need to deal with nested cases of ADD/<args/ADD/<args>>  and ADD/<args/ADD/<args>/ADD<args>>
                    string additemstr = arglist[1];

                    if (arglist.Length > 2)
                        additemstr = arglist[1] + "/" + arglist[2];

                    // check to see if the first char is a "<"
                    if (additemstr.IndexOf("<") == 0)
                    {
                        // itemarglist[1] will contain arg2/arg3/arg4>/arg5
                        // additemstr should have the full list of args <arg2/arg3/arg4>/arg5 if they are there.  In the case of /arg1/ADD/arg2
                        // it will just have arg2
                        string[] itemarglist = ParseString(additemstr, 2, "<");

                        // take that argument list that should like like arg2/ag3/arg4>/arg5
                        // need to find the matching ">"
                        //string[] itemargs = ParseString(itemarglist[1],2,">");

                        string[] itemargs = ParseToMatchingParen(itemarglist[1], '<', '>');

                        // and get the first part of the string without the >  so itemargs[0] should be arg2/ag3/arg4
                        itemargstring = itemargs[0];

                        // and itemargs[1] should be the remainder
                        if (itemargs.Length > 1)
                        {
                            // but have to get rid of any trailing / that might be after the >
                            string[] trailstr = ParseSlashArgs(itemargs[1], 2);
                            if (trailstr.Length > 1)
                                remainder = trailstr[1];
                            else
                                remainder = itemargs[1];
                        }
                        else
                            remainder = "";

                        // get the type info by pulling out the first arg in itemargstring
                        string[] tempitemarg = ParseSlashArgs(itemargstring, 2);

                        // and get the type info from it
                        itemtypestr = tempitemarg[0];

                    }
                    else
                    {
                        // otherwise its just a regular case with arglist[2] containing the rest of the arguments
                        if (arglist.Length > 2)
                            remainder = arglist[2];
                        else
                            remainder = "";
                    }

                    // test the drop probability
                    if (Utility.RandomDouble() >= drop_probability) return true;

                    // is it a valid item specification?
                    string baseitemtype = ParseObjectType(itemtypestr);

                    Type type = SpawnerType.GetType(ParseObjectType(itemtypestr));

                    // if so then create it
                    if (type != null)
                    {
                        object newo = XmlSpawner.CreateObject(type, itemtypestr);
                        if (newo is Item)
                        {
                            Item item = newo as Item;

                            if (equip && m != null)
                            {
                                if (!m.EquipItem(item)) pack.DropItem(item);
                            }
                            else
                                pack.DropItem(item);

                            // could call applyobjectstringproperties on a nested propertylist here to set item attributes
                            if (itemargstring != null)
                            {
                                ApplyObjectStringProperties(spawner, itemargstring, item, trigmob, refobject, out status_str);
                            }
                        }
                        else
                        {
                            status_str = "Invalid ADD. No such item : " + itemtypestr;

                            if (newo is BaseCreature)
                                ((BaseCreature)newo).Delete();

                            return false;
                        }

                    }
                    else
                    {
                        status_str = "Invalid ADD. No such item : " + itemtypestr;
                        return false;
                    }
                }
                else
                {
                    status_str = "Invalid ADD. mobile has no pack.";

                    if (arglist.Length < 3) return false;

                    remainder = arglist[2];
                    return false;
                }
            }
            else
            {
                status_str = "Invalid ADD. must be mobile or container.";

                if (arglist.Length < 3) return false;

                remainder = arglist[2];
                return false;
            }
            return true;
        }

        #endregion

        #region String parsing methods

        public static string ApplySubstitution(XmlSpawner spawner, object o, Mobile trigmob, string typeName)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // go through the string looking for instances of {keyword}
            string remaining = typeName;

            while (remaining != null && remaining.Length > 0)
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

                string value = ParseForKeywords(spawner, o, keypart.Trim(), trigmob, true, out ptype);

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
            else
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
                if (itemtypestring != null && itemtypestring.Length > 0)
                    argstart = itemtypestring.IndexOf(",") + 1;

                if (argstart > 1 && argstart < itemtypestring.Length)
                {
                    typeargs = ParseCommaArgs(itemtypestring.Substring(argstart), 15);
                }
                return (typeargs);

            }
            else
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

        public static string[] ParseSpaceArgs(string str, int nitems)
        {
            if (str == null) return null;
            str = str.Trim();

            string[] args = str.Split(spacedelim, nitems);
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
            int index = 0;
            List<string> strargs = new List<string>();
            while (index >= 0)
            {
                // go through the string and find the first instance of the separator
                index = str.IndexOf(separator);
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

        #region Keyword support methods

        public static void BroadcastAsciiMessage(AccessLevel ac, int hue, int font, string message)
        {
            foreach (NetState state in NetState.Instances)
            {
                Mobile m = state.Mobile;

                if (m != null && m.AccessLevel >= ac)
                    //m.SendMessage(hue, message);
                    m.Send(new AsciiMessage(Serial.MinusOne, -1, MessageType.Regular, hue, font, "System", message));
            }
        }

        public static void PublicOverheadMobileMessage(Mobile mob, MessageType type, int hue, int font, string text, bool noLineOfSight)
        {
            if (mob != null && mob.Map != null)
            {
                Packet p = null;

                IPooledEnumerable eable = mob.Map.GetClientsInRange(mob.Location);

                foreach (NetState state in eable)
                {
                    if (state.Mobile.CanSee(mob) && (noLineOfSight || state.Mobile.InLOS(mob)))
                    {
                        if (p == null)
                        {
                            p = new AsciiMessage(mob.Serial, mob.Body, type, hue, font, mob.Name, text);

                            p.Acquire();

                        }

                        state.Send(p);
                    }
                }

                Packet.Release(p);

                eable.Free();
            }
        }

        public static void PublicOverheadItemMessage(Item item, MessageType type, int hue, int font, string text)
        {
            if (item != null && item.Map != null)
            {
                Packet p = null;
                Point3D worldLoc = item.GetWorldLocation();

                IPooledEnumerable eable = item.Map.GetClientsInRange(worldLoc, item.GetMaxUpdateRange());

                foreach (NetState state in eable)
                {
                    Mobile m = state.Mobile;

                    if (m.CanSee(item) && m.InRange(worldLoc, item.GetUpdateRange(m)))
                    {
                        if (p == null)
                        {
                            p = new AsciiMessage(item.Serial, item.ItemID, type, hue, font, item.Name, text);

                            p.Acquire();
                        }

                        state.Send(p);
                    }
                }

                Packet.Release(p);

                eable.Free();
            }
        }

        public static void ResurrectPlayers(string arglist, Mobile triggermob, object refobject, out string status_str)
        {
            status_str = null;
            Item refitem = null;
            Mobile refmob = null;
            if (refobject is Item)
            {
                refitem = (Item)refobject;
            }
            else
                if (refobject is Mobile)
            {
                refmob = (Mobile)refobject;
            }
            // syntax is RESURRECT[,range][,PETS]
            // look for the range arg
            string[] str = ParseString(arglist, 3, ",");
            int range = 0;
            bool petres = false;

            if (str.Length > 1)
            {

                if (!int.TryParse(str[1], out range))
                    status_str = "bad range arg in RESURRECT";
            }
            if (str.Length > 2)
            {
                // get the range arg
                if (str[2].ToLower() == "pets")
                    petres = true;
                else bool.TryParse(str[2], out petres);
            }

            try
            {
                if ((range > 0) || (triggermob != null && !triggermob.Deleted))
                {
                    if (range > 0)
                    {
                        IPooledEnumerable rangelist = null;
                        if (refitem != null && !refitem.Deleted)
                        {
                            rangelist = refitem.GetMobilesInRange(range);
                        }
                        else if (refmob != null && !refmob.Deleted)
                        {
                            rangelist = refmob.GetMobilesInRange(range);
                        }

                        if (rangelist != null)
                        {
                            foreach (Mobile p in rangelist)
                            {
                                if (!petres && p is PlayerMobile && p.Body.IsGhost)
                                {
                                    p.Resurrect();
                                }
                                else if (petres && p is BaseCreature && ((BaseCreature)p).ControlMaster == triggermob && ((BaseCreature)p).Controlled && ((BaseCreature)p).IsDeadPet)
                                {
                                    ((BaseCreature)p).ResurrectPet();
                                }
                            }
                            rangelist.Free();
                        }
                    }
                    else
                    {
                        // just send it to the mob who triggered
                        if (triggermob.Body.IsGhost)
                            triggermob.Resurrect();

                    }
                }
            }
            catch { }
        }

        public static void ApplyDamageToPlayers(string arglist, Mobile triggermob, object refobject, out string status_str)
        {
            status_str = null;
            Item refitem = null;
            Mobile refmob = null;
            if (refobject is Item)
            {
                refitem = (Item)refobject;
            }
            else if (refobject is Mobile)
            {
                refmob = (Mobile)refobject;
            }
            // look for the other args. Syntax is DAMAGE,damage,phys,fire,cold,pois,energy[,range][,playeronly]
            string[] str = ParseString(arglist, 9, ",");
            bool playeronly = false;
            int range = -1;
            int damage = 0, phys = 0, fire = 0, cold = 0, pois = 0, ener = 0;
            if (str.Length < 3)
            {
                // we consider all the damage as physical 
                if (str.Length > 1 && int.TryParse(str[1], out damage))
                    phys = 100;
                else
                    status_str = "bad damage arg in DAMAGE";
            }
            else if (str.Length < 7)
            {
                status_str = "missing damage args in DAMAGE";
            }
            else if (!int.TryParse(str[1], out damage) |
                !int.TryParse(str[2], out phys) |
                !int.TryParse(str[3], out fire) |
                !int.TryParse(str[4], out cold) |
                !int.TryParse(str[5], out pois) |
                !int.TryParse(str[6], out ener))
            {
                status_str = "bad damage args in DAMAGE";
            }
            if (str.Length > 7)
            {
                // get the range arg
                if (!int.TryParse(str[7], out range))
                {
                    range = -1;
                    status_str = "bad range arg in DAMAGE";
                }
            }
            if (str.Length > 8)
            {
                if (str[8].ToLower() == "playeronly")
                    playeronly = true;
                else
                    bool.TryParse(str[8], out playeronly);
            }
            try
            {
                if (range >= 0 || (triggermob != null && !triggermob.Deleted))
                {
                    // apply the poison to all players within range if range is > 0
                    if (range >= 0)
                    {
                        IPooledEnumerable rangelist = null;
                        if (refitem != null && !refitem.Deleted)
                        {
                            rangelist = refitem.GetMobilesInRange(range);
                        }
                        else if (refmob != null && !refmob.Deleted)
                        {
                            rangelist = refmob.GetMobilesInRange(range);
                        }

                        if (rangelist != null)
                        {
                            foreach (Mobile p in rangelist)
                            {

                                if (p is PlayerMobile || !playeronly)
                                    AOS.Damage(p, damage, phys, fire, cold, pois, ener);
                            }
                            rangelist.Free();
                        }
                    }
                    else
                    {
                        // just apply it to the mob who triggered
                        AOS.Damage(triggermob, damage, phys, fire, cold, pois, ener);
                    }
                }
            }
            catch { }
        }

        public static void SendBoltEffect(IEntity e, int sound, int hue)
        {
            Map map = e.Map;

            if (map == null)
                return;

            if (e is Item)
                ((Item)e).ProcessDelta();
            else if (e is Mobile)
                ((Mobile)e).ProcessDelta();

            Packet preEffect = null, boltEffect = null, playSound = null;

            IPooledEnumerable eable = map.GetClientsInRange(e.Location);

            foreach (NetState state in eable)
            {
                if (Effects.SendParticlesTo(state))
                {
                    if (preEffect == null)
                        preEffect = Packet.Acquire(new TargetParticleEffect(e, 0, 10, 5, 0, 0, 5031, 3, 0));

                    state.Send(preEffect);
                }

                if (boltEffect == null)
                    boltEffect = Packet.Acquire(new BoltEffect(e, hue));

                state.Send(boltEffect);

                if (sound > 0)
                {
                    if (playSound == null)
                        playSound = Packet.Acquire(new PlaySound(sound, e));

                    state.Send(playSound);
                }
            }

            Packet.Release(preEffect);
            Packet.Release(boltEffect);
            Packet.Release(playSound);

            eable.Free();
        }

        public static void ExecuteActions(Mobile mob, object attachedto, string actions)
        {
            if (actions == null || actions.Length <= 0) return;
            // execute any action associated with it
            // allow for multiple action strings on a single line separated by a semicolon

            string[] args = actions.Split(';');

            for (int j = 0; j < args.Length; j++)
            {
                ExecuteAction(mob, attachedto, args[j]);
            }

        }

        public static void ExecuteAction(Mobile trigmob, object attachedto, string action)
        {
            Point3D loc = Point3D.Zero;
            Map map = null;
            if (attachedto is IEntity)
            {
                loc = ((IEntity)attachedto).Location;
                map = ((IEntity)attachedto).Map;
            }

            if (action == null || action.Length <= 0 || attachedto == null || map == null) return;
            XmlSpawner.SpawnObject TheSpawn = new XmlSpawner.SpawnObject(null, 0)
            {
                TypeName = action
            };
            string substitutedtypeName = ApplySubstitution(null, attachedto, trigmob, action);
            string typeName = ParseObjectType(substitutedtypeName);


            string status_str;
            if (IsTypeOrItemKeyword(typeName))
            {
                SpawnTypeKeyword(attachedto, TheSpawn, typeName, substitutedtypeName, true, trigmob, loc, map, out status_str);
            }
            else
            {
                // its a regular type descriptor so find out what it is
                Type type = SpawnerType.GetType(typeName);
                try
                {
                    string[] arglist = ParseString(substitutedtypeName, 3, "/");
                    object o = XmlSpawner.CreateObject(type, arglist[0]);

                    if (o == null)
                    {
                        status_str = "invalid type specification: " + arglist[0];
                    }
                    else
                        if (o is Mobile)
                    {
                        Mobile m = (Mobile)o;
                        if (m is BaseCreature)
                        {
                            BaseCreature c = (BaseCreature)m;
                            c.Home = loc; // Spawners location is the home point
                        }

                        m.Location = loc;
                        m.Map = map;

                        ApplyObjectStringProperties(null, substitutedtypeName, m, trigmob, attachedto, out status_str);
                    }
                    else
                            if (o is Item)
                    {
                        Item item = (Item)o;
                        AddSpawnItem(null, attachedto, TheSpawn, item, loc, map, trigmob, false, substitutedtypeName, out status_str);
                    }
                }
                catch { }
            }
        }
        #endregion

        #region Spawn methods


        public static void AddSpawnItem(XmlSpawner spawner, XmlSpawner.SpawnObject theSpawn, Item item, Point3D location, Map map, Mobile trigmob, bool requiresurface,
            string propertyString, out string status_str)
        {
            AddSpawnItem(spawner, spawner, theSpawn, item, location, map, trigmob, requiresurface, null, propertyString, false, out status_str);
        }

        public static void AddSpawnItem(XmlSpawner spawner, object invoker, XmlSpawner.SpawnObject theSpawn, Item item, Point3D location, Map map, Mobile trigmob, bool requiresurface,
            string propertyString, out string status_str)
        {
            AddSpawnItem(spawner, invoker, theSpawn, item, location, map, trigmob, requiresurface, null, propertyString, false, out status_str);
        }

        public static void AddSpawnItem(XmlSpawner spawner, XmlSpawner.SpawnObject theSpawn, Item item, Point3D location, Map map, Mobile trigmob, bool requiresurface,
            string propertyString, bool smartspawn, out string status_str)
        {
            AddSpawnItem(spawner, spawner, theSpawn, item, location, map, trigmob, requiresurface, null, propertyString, smartspawn, out status_str);
        }

        public static void AddSpawnItem(XmlSpawner spawner, XmlSpawner.SpawnObject theSpawn, Item item, Point3D location, Map map, Mobile trigmob, bool requiresurface,
            List<XmlSpawner.SpawnPositionInfo> spawnpositioning, string propertyString, out string status_str)
        {
            AddSpawnItem(spawner, spawner, theSpawn, item, location, map, trigmob, requiresurface, spawnpositioning, propertyString, false, out status_str);
        }

        public static void AddSpawnItem(XmlSpawner spawner, object invoker, XmlSpawner.SpawnObject theSpawn, Item item, Point3D location, Map map, Mobile trigmob, bool requiresurface,
            List<XmlSpawner.SpawnPositionInfo> spawnpositioning, string propertyString, out string status_str)
        {
            AddSpawnItem(spawner, invoker, theSpawn, item, location, map, trigmob, requiresurface, spawnpositioning, propertyString, false, out status_str);
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

        public static bool SpawnTypeKeyword(object invoker, XmlSpawner.SpawnObject TheSpawn, string typeName, string substitutedtypeName, bool requiresurface,
            Mobile triggermob, Point3D location, Map map, out string status_str)
        {
            return SpawnTypeKeyword(invoker, TheSpawn, typeName, substitutedtypeName, requiresurface, null,
                triggermob, location, map, null, out status_str, 0);
        }

        public static bool SpawnTypeKeyword(object invoker, XmlSpawner.SpawnObject TheSpawn, string typeName, string substitutedtypeName, bool requiresurface,
            Mobile triggermob, Point3D location, Map map, XmlGumpCallback gumpcallback, out string status_str, byte loops)
        {
            return SpawnTypeKeyword(invoker, TheSpawn, typeName, substitutedtypeName, requiresurface, null,
                triggermob, location, map, gumpcallback, out status_str, loops);
        }

        public static bool SpawnTypeKeyword(object invoker, XmlSpawner.SpawnObject TheSpawn, string typeName, string substitutedtypeName, bool requiresurface,
            List<XmlSpawner.SpawnPositionInfo> spawnpositioning, Mobile triggermob, Point3D location, Map map, out string status_str, byte loops)
        {
            return SpawnTypeKeyword(invoker, TheSpawn, typeName, substitutedtypeName, requiresurface, spawnpositioning,
                triggermob, location, map, null, out status_str, loops);
        }

        public static bool SpawnTypeKeyword(object invoker, XmlSpawner.SpawnObject TheSpawn, string typeName, string substitutedtypeName, bool requiresurface,
            List<XmlSpawner.SpawnPositionInfo> spawnpositioning, Mobile triggermob, Point3D location, Map map, XmlGumpCallback gumpcallback, out string status_str, byte loops)
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
                                else
                                {
                                    ApplyObjectStringProperties(spawner, substitutedtypeName, setitem, triggermob, invoker, out status_str);
                                }
                            }
                            else if (spawner != null)
                            {
                                ApplyObjectStringProperties(spawner, substitutedtypeName, spawner.SetItem, triggermob, invoker, out status_str);
                            }


                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.SETONMOB:
                        {
                            // the syntax is SETONMOB,mobname[,mobtype]/prop/value/prop2/value...
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            Mobile mob = null;
                            if (arglist.Length > 0)
                            {
                                string[] keywordargs = ParseString(arglist[0], 3, ",");
                                if (keywordargs.Length > 1)
                                {
                                    string typestr = null;
                                    if (keywordargs.Length > 2)
                                    {
                                        typestr = keywordargs[2];
                                    }

                                    mob = FindMobileByName(spawner, keywordargs[1], typestr);

                                    if (mob == null)
                                    {
                                        status_str = string.Format("named mob '{0}' not found", keywordargs[1]);
                                    }
                                }
                                else
                                {
                                    status_str = "missing mob name in SETONMOB";
                                }
                            }
                            if (mob != null)
                            {
                                ApplyObjectStringProperties(spawner, substitutedtypeName, mob, triggermob, invoker, out status_str);
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.SETONTHIS:
                        {
                            // the syntax is SETONTHIS[,proptest=value]/prop/value/prop2/value2/prop3/value3/..
                            //string [] arglist = ParseString(substitutedtypeName,3,"/");

                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            string proptest = null;

                            if (arglist.Length > 0)
                            {
                                string[] objstr = ParseString(arglist[0], 2, ",");
                                if (objstr.Length > 1)
                                {
                                    proptest = objstr[1];
                                }
                            }
                            else
                            {
                                status_str = "missing args to SETONTHIS";
                                return false;
                            }

                            if (invoker != null && (proptest == null || CheckPropertyString(null, invoker, proptest, null, out status_str)))
                            {
                                ApplyObjectStringProperties(spawner, substitutedtypeName, invoker, triggermob, invoker, out status_str);
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.SETONTRIGMOB:
                        {
                            // the syntax is SETONTRIGMOB/prop/value/prop2/value...
                            ApplyObjectStringProperties(spawner, substitutedtypeName, triggermob, triggermob, invoker, out status_str);
                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.SETACCOUNTTAG:
                        {
                            // the syntax is SETACCOUNTTAG,tagname/value
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 2);

                            if (arglist.Length > 1)
                            {
                                string[] objstr = ParseString(arglist[0], 2, ",");

                                if (objstr.Length < 2)
                                {
                                    status_str = "missing tagname in SETACCOUNTTAG";
                                    return false;
                                }

                                string tagname = objstr[1];
                                string tagval = arglist[1];

                                // set the tag value
                                // get the value of the account tag from the triggering mob
                                if (triggermob != null && !triggermob.Deleted)
                                {
                                    Account acct = triggermob.Account as Account;
                                    if (acct != null)
                                    {
                                        acct.SetTag(tagname, tagval);
                                    }
                                }
                            }
                            else
                            {
                                status_str = "no value assigned to SETACCOUNTTAG";
                                return false;
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.FOREACH:
                        {
                            // the syntax is FOREACH,objecttype[,name][,range]/action
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 2);
                            if (arglist.Length > 1)
                            {
                                string[] objstr = ParseString(arglist[0], 4, ",");
                                if (objstr.Length < 2)
                                {
                                    status_str = "missing objecttype in FOREACH";
                                    return false;
                                }
                                Type objecttype = SpawnerType.GetType(objstr[1]);
                                if (objecttype != null)
                                {
                                    int range = -1;
                                    string objectname = "*";
                                    if (objstr.Length > 2)
                                    {
                                        objectname = objstr[2];
                                        if (objstr.Length > 3 && !int.TryParse(objstr[3], out range))
                                            range = -1;
                                    }
                                    if ((spawner == null || spawner.Deleted) && range < 0)
                                    {
                                        status_str = "invalid range outside of spawner in FOREACH";
                                        return false;
                                    }

                                    string remaining = arglist[1];
                                    if (typeof(Mobile).IsAssignableFrom(objecttype))
                                    {
                                        List<Mobile> mobs = new List<Mobile>();
                                        if (spawner != null)
                                        {
                                            if (spawner.SpawnRegion != null && spawner.HasRegionPoints(spawner.SpawnRegion))
                                            {
                                                foreach (Mobile m in spawner.SpawnRegion.GetMobiles())
                                                {
                                                    if (objecttype.IsAssignableFrom(m.GetType()) && CheckNameMatch(objectname, m.Name))
                                                        mobs.Add(m);
                                                }
                                            }
                                            else
                                            {
                                                foreach (Mobile m in map.GetMobilesInBounds(spawner.SpawnerBounds))
                                                {
                                                    if (objecttype.IsAssignableFrom(m.GetType()) && CheckNameMatch(objectname, m.Name))
                                                    {
                                                        mobs.Add(m);
                                                    }
                                                }
                                            }
                                        }
                                        else if (invoker != null && invoker is IEntity)
                                        {
                                            IPooledEnumerable eable = map.GetMobilesInRange(((IEntity)invoker).Location, range);
                                            foreach (Mobile m in eable)
                                            {
                                                if (objecttype.IsAssignableFrom(m.GetType()) && CheckNameMatch(objectname, m.Name))
                                                    mobs.Add(m);
                                            }
                                            eable.Free();
                                        }
                                        for (int x = mobs.Count - 1; x >= 0; --x)
                                        {
                                            if (mobs[x].AccessLevel < AccessLevel.Counselor)
                                                ApplyObjectStringProperties(spawner, substitutedtypeName, mobs[x], mobs[x], invoker, out status_str);
                                        }
                                    }
                                    else if (typeof(Item).IsAssignableFrom(objecttype))
                                    {
                                        List<Item> items = new List<Item>();
                                        if (spawner != null)
                                        {
                                            if (spawner.SpawnRegion != null && spawner.HasRegionPoints(spawner.SpawnRegion))
                                            {
                                                foreach (Item i in GetItems(spawner.SpawnRegion))
                                                {
                                                    if (objecttype.IsAssignableFrom(i.GetType()) && CheckNameMatch(objectname, i.Name))
                                                        items.Add(i);
                                                }
                                            }
                                            else
                                            {
                                                foreach (Item i in map.GetItemsInBounds(spawner.SpawnerBounds))
                                                {
                                                    if (objecttype.IsAssignableFrom(i.GetType()) && CheckNameMatch(objectname, i.Name))
                                                        items.Add(i);
                                                }
                                            }
                                        }
                                        else if (invoker != null && invoker is IEntity)
                                        {
                                            foreach (Item i in map.GetItemsInRange(((IEntity)invoker).Location, range))
                                            {
                                                if (objecttype.IsAssignableFrom(i.GetType()) && CheckNameMatch(objectname, i.Name))
                                                    items.Add(i);
                                            }
                                        }
                                        for (int x = items.Count - 1; x >= 0; --x)
                                            ApplyObjectStringProperties(spawner, substitutedtypeName, items[x], triggermob, invoker, out status_str);
                                    }
                                    else
                                    {
                                        status_str = "invalid TYPE specified in FOREACH";
                                        return false;
                                    }
                                }
                                else
                                {
                                    status_str = "invalid TYPE specified in FOREACH";
                                    return false;
                                }
                            }
                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.SETONNEARBY:
                        {
                            // the syntax is SETONNEARBY,range,name[,type][,searchcontainers][,proptest]/prop/value/prop/value...

                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            string typestr = null;
                            string proptest = null;
                            bool searchcontainers = false;

                            string targetname;
                            int range;
                            if (arglist.Length > 0)
                            {
                                string[] objstr = ParseString(arglist[0], 6, ",");
                                if (objstr.Length < 3)
                                {
                                    status_str = "missing range or name in SETONNEARBY";
                                    return false;
                                }

                                if (!int.TryParse(objstr[1], out range))
                                    range = -1;

                                if (range < 0)
                                {
                                    status_str = "invalid range in SETONNEARBY";
                                    return false;
                                }

                                targetname = objstr[2];

                                if (objstr.Length > 3)
                                {
                                    typestr = objstr[3];
                                }

                                if (objstr.Length > 4)
                                {
                                    bool.TryParse(objstr[4], out searchcontainers);
                                }

                                if (objstr.Length > 5)
                                {
                                    proptest = objstr[5];
                                }
                            }
                            else
                            {
                                status_str = "missing args to SETONNEARBY";
                                return false;
                            }

                            Type targettype = null;
                            if (typestr != null)
                            {
                                targettype = SpawnerType.GetType(typestr);
                            }
                            List<object> nearbylist = GetNearbyObjects(invoker, targetname, targettype, typestr, range, searchcontainers, proptest);

                            // apply the properties to everything on the list
                            foreach (object nearbyobj in nearbylist)
                            {
                                ApplyObjectStringProperties(spawner, substitutedtypeName, nearbyobj, triggermob, invoker, out status_str);
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.SETONPETS:
                        {
                            // the syntax is SETONPETS,range[,name]/prop/value/prop/value...

                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            string typestr = "BaseCreature";
                            string targetname = "*";
                            bool searchcontainers = false;

                            int range;
                            if (arglist.Length > 0)
                            {
                                string[] objstr = ParseString(arglist[0], 3, ",");
                                if (objstr.Length < 2)
                                {
                                    status_str = "missing range in SETONPETS";
                                    return false;
                                }

                                if (!int.TryParse(objstr[1], out range))
                                    range = -1;

                                if (range < 0)
                                {
                                    status_str = "invalid range in SETONPETS";
                                    return false;
                                }
                                if (objstr.Length > 2)
                                    targetname = objstr[2];
                            }
                            else
                            {
                                status_str = "missing args to SETONPETS";
                                return false;
                            }

                            Type targettype = null;

                            if (typestr != null)
                            {
                                targettype = SpawnerType.GetType(typestr);
                            }

                            // get all of the nearby pets
                            List<object> nearbylist = GetNearbyObjects(triggermob, targetname, targettype, typestr, range, searchcontainers, null);

                            // apply the properties to everything on the list
                            foreach (object nearbyobj in nearbylist)
                            {
                                // is this a pet of the triggering mob
                                BaseCreature pet = nearbyobj as BaseCreature;

                                if (pet != null && pet.Controlled && pet.ControlMaster == triggermob)
                                {
                                    ApplyObjectStringProperties(spawner, substitutedtypeName, nearbyobj, triggermob, invoker, out status_str);
                                }
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }

                    case typeKeyword.SETONCARRIED:
                        {
                            // the syntax is SETONCARRIED,itemname[,itemtype][,equippedonly]/prop/value/prop2/value...
                            // or SETONCARRIED,itemname[,itemtype]/prop/value

                            // first find the carried item
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            string typestr = null;
                            bool equippedonly = false;

                            string itemname;
                            if (arglist.Length > 0)
                            {
                                string[] objstr = ParseString(arglist[0], 4, ",");
                                if (objstr.Length < 2)
                                {
                                    status_str = "missing itemname in SETONCARRIED";
                                    return false;
                                }

                                itemname = objstr[1];

                                if (objstr.Length > 2)
                                {
                                    typestr = objstr[2];
                                }

                                if (objstr.Length > 3)
                                {
                                    if (objstr[3].ToLower() == "equippedonly")
                                    {
                                        equippedonly = true;
                                    }
                                    else
                                    {
                                        bool.TryParse(objstr[3], out equippedonly);
                                    }
                                }
                            }
                            else
                            {
                                status_str = "missing args to SETONCARRIED";
                                return false;
                            }

                            Item testitem = SearchMobileForItem(triggermob, ParseObjectType(itemname), typestr, false, equippedonly);

                            ApplyObjectStringProperties(spawner, substitutedtypeName, testitem, triggermob, invoker, out status_str);

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.SETONSPAWN:
                        {
                            // the syntax is SETONSPAWN[,spawnername],subgroup/prop/value/prop2/value...
                            // or SETONSPAWN[,spawnername],subgroup/prop/value

                            // first find the spawn
                            int subgroup = -1;
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            XmlSpawner targetspawner = spawner;
                            if (arglist.Length > 0)
                            {
                                string[] keywordargs = ParseString(arglist[0], 3, ",");
                                if (keywordargs.Length < 2)
                                {
                                    status_str = "missing subgroup in SETONSPAWN";
                                    return false;
                                }
                                else
                                {
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
                            }
                            if (subgroup == -1)
                            {
                                status_str = "invalid subgroup in SETONSPAWN";
                                return false;
                            }

                            List<object> spawnedlist = XmlSpawner.GetSpawnedList(targetspawner, subgroup);
                            if (spawnedlist == null) return true;
                            foreach (object targetobj in spawnedlist)
                            {
                                if (targetobj == null) return true;

                                // dont apply it to keyword tags
                                if (targetobj is KeywordTag) continue;

                                // set the properties on the target object
                                ApplyObjectStringProperties(spawner, substitutedtypeName, targetobj, triggermob, spawner, out status_str);
                            }
                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.SETONSPAWNENTRY:
                        {
                            // the syntax is SETONSPAWNENTRY[,spawnername],entrystring/prop/value/prop2/value...

                            // find the spawn entry
                            string entrystring = null;
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            XmlSpawner targetspawner = spawner;

                            if (arglist.Length > 0)
                            {
                                string[] keywordargs = ParseString(arglist[0], 3, ",");
                                if (keywordargs.Length < 2)
                                {
                                    status_str = "missing entrystring in SETONSPAWNENTRY";
                                    return false;
                                }
                                else
                                {
                                    entrystring = keywordargs[1];
                                    string spawnerstr = null;
                                    if (keywordargs.Length > 2)
                                    {
                                        spawnerstr = keywordargs[1];
                                        entrystring = keywordargs[2];
                                    }
                                    if (spawnerstr != null)
                                    {
                                        targetspawner = FindSpawnerByName(spawner, spawnerstr);
                                    }
                                }
                            }
                            if (entrystring == null || entrystring.Length == 0)
                            {
                                status_str = "invalid entrystring in SETONSPAWNENTRY";
                                return false;
                            }

                            int entryindex = -1;
                            // is the entrystring a number?
                            if (entrystring[0] >= '0' && entrystring[0] <= '9')
                            {
                                if (!int.TryParse(entrystring, out entryindex))
                                    entryindex = -1;
                            }

                            if (targetspawner == null || targetspawner.SpawnObjects == null) return true;

                            for (int i = 0; i < targetspawner.SpawnObjects.Length; i++)
                            {
                                XmlSpawner.SpawnObject targetobj = targetspawner.SpawnObjects[i];

                                // is this references by entrystring or entryindex?
                                if ((entryindex == i)
                                || (entryindex == -1 && targetobj != null && targetobj.TypeName != null && targetobj.TypeName.IndexOf(entrystring) >= 0))
                                {
                                    // set the properties on the spawn entry object
                                    ApplyObjectStringProperties(spawner, substitutedtypeName, targetobj, triggermob, spawner, out status_str);
                                }
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.SETONPARENT:
                        {
                            // the syntax is SETONPARENT/prop/value/prop2/value...
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);

                            if (invoker != null && (invoker is Item))
                            {
                                ApplyObjectStringProperties(spawner, substitutedtypeName, ((Item)invoker).Parent, triggermob, invoker, out status_str);
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.TAKEGIVE:
                        {
                            // syntax TAKEGIVE[,quantity[,true*,[type]]]/itemnametotake/GIVE/itemtypetogive *search in banca
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 5);
                            string targetName;
                            string typestr = null;
                            string[] givelist;
                            if (arglist.Length < 4)
                            {
                                status_str = "invalid TAKEGIVE specification";
                                return false;
                            }
                            else
                            {
                                givelist = new string[arglist.Length - 2];
                                targetName = arglist[1];
                                Array.Copy(arglist, 2, givelist, 0, arglist.Length - 2);
                            }
                            string[] keywordargs = ParseString(arglist[0], 4, ",");
                            string[] givekeywordargs = ParseString(givelist[0], 2, ",");
                            int quantity = 0;
                            bool banksearch = false;
                            bool success = false;
                            List<Item> toRemove = new List<Item>();
                            if (keywordargs.Length > 1)
                            {
                                if (!int.TryParse(keywordargs[1], out quantity))
                                {
                                    status_str = "Invalid TAKE quantity : " + arglist[1];
                                    return false;
                                }
                            }
                            if (keywordargs.Length > 2)
                            {
                                if (!bool.TryParse(keywordargs[2], out banksearch))
                                {
                                    status_str = "Invalid TAKE bankflag : " + arglist[1];
                                    return false;
                                }
                            }
                            if (keywordargs.Length > 3)
                            {
                                typestr = keywordargs[3];
                            }
                            // search the trigger mob for the named item
                            Item itemTarget = SearchMobileForItem(triggermob, targetName, typestr, banksearch);

                            // found the item so get rid of it
                            if (itemTarget != null)
                            {
                                // if a quantity was specified and the item is stackable, then try to take the quantity
                                if (quantity > 0 && itemTarget.Stackable)
                                {
                                    List<Item> itemlist = SearchMobileForItems(triggermob, targetName, typestr, banksearch, false);
                                    itemlist.Reverse();
                                    int totaltaken = 0;
                                    int totake = quantity;
                                    foreach (Item it in itemlist)
                                    {
                                        int remaining = it.Amount - quantity;
                                        if (remaining <= 0)
                                        {
                                            int taken = it.Amount;
                                            totaltaken += taken;

                                            toRemove.Add(it);
                                            quantity -= taken;
                                        }
                                        else
                                        {
                                            totaltaken += quantity;
                                            it.Amount = remaining;
                                            break;
                                        }
                                    }

                                    if (totaltaken >= totake)
                                    {
                                        for (int i = toRemove.Count - 1; i >= 0; --i)
                                            toRemove[i].Delete();
                                        success = true;
                                    }
                                }
                                else//non stackable, we have to find them all
                                {
                                    if (quantity <= 1)
                                    {
                                        toRemove.Add(itemTarget);
                                    }
                                    else
                                    {
                                        List<Item> itemlist = SearchMobileForItems(triggermob, targetName, typestr, banksearch, false);
                                        itemlist.Reverse();

                                        for (int i = itemlist.Count - 1, totake = quantity; i >= 0 && totake > 0; --i, --totake)
                                        {
                                            toRemove.Add(itemlist[i]);
                                        }
                                    }
                                    if (toRemove.Count >= quantity)
                                    {
                                        for (int i = toRemove.Count - 1; i >= 0; --i)
                                            toRemove[i].Delete();
                                        success = true;
                                    }
                                }

                                string remainder;
                                if (success)
                                    AddItemToTarget(spawner, triggermob, givekeywordargs, givelist, triggermob, invoker, false, out remainder, out status_str);
                            }
                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));
                            break;
                        }
                    case typeKeyword.GIVE:
                        {
                            //syntax is GIVE[,probability (0.01=1% 1=100%)]/itemtypetogive
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);

                            string remainder;
                            if (arglist.Length > 1)
                            {
                                // check for any special keywords such as the additem option or the subproperty specification
                                // note this will be an arg to some property
                                string[] keywordargs = ParseString(arglist[0], 2, ",");
                                AddItemToTarget(spawner, triggermob, keywordargs, arglist, triggermob, invoker, false, out remainder, out status_str);

                            }
                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.GUMP:
                        {
                            // the syntax is GUMP,title,type/string
                            // can alternatively accept a gump constructor name
                            // GUMP,title,type,constructorname/string
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 2);
                            string gumpText;
                            if (arglist.Length > 1)
                            {
                                gumpText = arglist[1];
                            }
                            else
                            {
                                status_str = "invalid GUMP specification";
                                return false;
                            }

                            string[] gumpkeywordargs = ParseString(arglist[0], 4, ",");
                            string gumpTitle;
                            int gumpNumber;
                            if (gumpkeywordargs.Length > 2)
                            {
                                gumpTitle = gumpkeywordargs[1];
                                if (!int.TryParse(gumpkeywordargs[2], out gumpNumber))
                                {
                                    status_str = "Invalid GUMP args";
                                    return false;
                                }
                            }
                            else
                            {
                                status_str = "invalid GUMP specification";
                                return false;
                            }
                            string gumptypestr = "XmlSimpleGump"; // default gump constructor

                            if (gumpkeywordargs.Length > 3)
                            {
                                // get the gump constructor type
                                gumptypestr = gumpkeywordargs[3].Trim();
                            }
                            Type type = SpawnerType.GetType(gumptypestr); ;

                            if (type == null)
                            {
                                status_str = "invalid GUMP constructor : " + gumptypestr;
                                return false;
                            }

                            // prepare the keyword tag for the gump
                            KeywordTag newtag = new KeywordTag(substitutedtypeName, spawner, 1);
                            if (triggermob != null && !triggermob.Deleted && (triggermob is PlayerMobile))
                            {
                                object[] gumpargs = new object[7];
                                gumpargs[0] = invoker;
                                gumpargs[1] = gumpText;
                                gumpargs[2] = gumpTitle;
                                gumpargs[3] = gumpNumber;
                                gumpargs[4] = newtag;
                                gumpargs[5] = triggermob;
                                gumpargs[6] = gumpcallback;

                                object newgump;
                                //spawner.TriggerMob.SendGump( new XmlSimpleGump(this, gumpText,gumpTitle, gumpType ));
                                try
                                {
                                    newgump = Activator.CreateInstance(type, gumpargs);
                                }
                                catch { status_str = "Error in creating gump type : " + gumptypestr; newtag.Delete(); return false; }
                                if (newgump != null)
                                {
                                    if (newgump is Gump)
                                    {
                                        triggermob.SendGump((Gump)newgump);
                                    }
                                    else if (newgump is Item)
                                    {
                                        ((Item)newgump).Delete();
                                        status_str = gumptypestr + " is not a Gump type";
                                        newtag.Delete();
                                        return false;
                                    }
                                    else if (newgump is Mobile)
                                    {
                                        ((Mobile)newgump).Delete();
                                        status_str = gumptypestr + " is not a Gump type";
                                        newtag.Delete();
                                        return false;
                                    }
                                    else
                                    {
                                        status_str = gumptypestr + " is not a Gump type";
                                        newtag.Delete();
                                        return false;
                                    }
                                }
                            }
                            TheSpawn.SpawnedObjects.Add(newtag);

                            break;
                        }
                    case typeKeyword.BROWSER:
                        {
                            // the syntax is BROWSER/url
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 2);
                            string url;

                            if (arglist.Length > 1)
                            {
                                if (arglist[1] != null && arglist[1].Length > 0 && arglist[1][0] == '@')
                                {
                                    url = arglist[1].Substring(1);
                                }
                                else
                                    url = arglist[1];
                            }
                            else
                            {
                                status_str = "invalid BROWSER specification";
                                return false;
                            }

                            if (triggermob != null && !triggermob.Deleted && (triggermob is PlayerMobile))
                            {
                                triggermob.LaunchBrowser(url);
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.SENDMSG:
                        {
                            // the syntax is SENDMSG[,hue]/string
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            // check for literal
                            string msgText;
                            int hue = 0x3B2;
                            int font = 3;

                            if (arglist.Length > 0)
                            {
                                string[] keywordargs = ParseString(arglist[0], 3, ",");
                                if (keywordargs.Length > 1)
                                {
                                    if (!int.TryParse(keywordargs[1], out hue))
                                    {
                                        hue = 0x3B2;
                                        status_str = "invalid hue arg to SENDMSG";
                                    }
                                }
                                if (keywordargs.Length > 2)
                                {
                                    if (!int.TryParse(keywordargs[2], out font))
                                    {
                                        font = 3;
                                        status_str = "invalid font arg to SENDASCIIMSG";
                                    }
                                }
                            }
                            if (arglist.Length > 1)
                            {
                                if (arglist[1] != null && arglist[1].Length > 0 && arglist[1][0] == '@')
                                {
                                    arglist = ParseSlashArgs(substitutedtypeName, 2);
                                    msgText = arglist[1].Substring(1);
                                }
                                else
                                    msgText = arglist[1];
                            }
                            else
                            {
                                status_str = "invalid SENDMSG specification";
                                return false;
                            }
                            if (triggermob != null && !triggermob.Deleted && (triggermob is PlayerMobile))
                            {
                                //triggermob.SendMessage(msgText);
                                triggermob.Send(new UnicodeMessage(Serial.MinusOne, -1, MessageType.Regular, hue, font, "ENU", "System", msgText));
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.SENDASCIIMSG:
                        {
                            // the syntax is SENDASCIIMSG[,hue][,font#]/string
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            // check for literal
                            string msgText;
                            int hue = 0x3B2;
                            int font = 3;

                            if (arglist.Length > 0)
                            {
                                string[] keywordargs = ParseString(arglist[0], 3, ",");
                                if (keywordargs.Length > 1)
                                {
                                    if (!int.TryParse(keywordargs[1], out hue))
                                    {
                                        hue = 0x3B2;
                                        status_str = "invalid hue arg to SENDASCIIMSG";
                                    }
                                }
                                if (keywordargs.Length > 2)
                                {
                                    if (!int.TryParse(keywordargs[2], out font))
                                    {
                                        font = 3;
                                        status_str = "invalid font arg to SENDASCIIMSG";
                                    }
                                }
                            }
                            if (arglist.Length > 1)
                            {
                                if (arglist[1] != null && arglist[1].Length > 0 && arglist[1][0] == '@')
                                {
                                    arglist = ParseSlashArgs(substitutedtypeName, 2);
                                    msgText = arglist[1].Substring(1);
                                }
                                else
                                    msgText = arglist[1];
                            }
                            else
                            {
                                status_str = "invalid SENDASCIIMSG specification";
                                return false;
                            }
                            if (triggermob != null && !triggermob.Deleted && (triggermob is PlayerMobile))
                            {
                                triggermob.Send(new AsciiMessage(Serial.MinusOne, -1, MessageType.Regular, hue, font, "System", msgText));
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.WAITUNTIL:
                        {
                            // the syntax is WAITUNTIL[,delay][,timeout][/condition][/thendogroup]
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 4);
                            double delay = 0;
                            double timeout = 0;
                            string condition = null;
                            int gotogroup = -1;
                            if (arglist.Length > 0)
                            {
                                string[] keywordargs = ParseString(arglist[0], 3, ",");
                                if (keywordargs.Length > 1)
                                {
                                    if (!double.TryParse(keywordargs[1], NumberStyles.Any, CultureInfo.InvariantCulture, out delay))
                                        status_str = "invalid delay arg to WAITUNTIL";
                                }
                                if (keywordargs.Length > 2)
                                {
                                    if (!double.TryParse(keywordargs[2], NumberStyles.Any, CultureInfo.InvariantCulture, out timeout))
                                        status_str = "invalid timeout arg to WAITUNTIL";
                                }
                            }
                            if (arglist.Length > 1)
                            {
                                condition = arglist[1];
                            }
                            if (arglist.Length > 2)
                            {
                                if (!int.TryParse(arglist[2], out gotogroup))
                                {
                                    status_str = "invalid goto arg to WAITUNTIL";
                                    gotogroup = -1;
                                }
                            }
                            if (status_str != null)
                            {
                                return false;
                            }
                            // suppress sequential advancement
                            //spawner.HoldSequence = true;

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner, TimeSpan.FromMinutes(delay), TimeSpan.FromMinutes(timeout), condition, gotogroup));

                            break;
                        }
                    case typeKeyword.WHILE:
                        {
                            // the syntax is WHILE/condition/dogroup
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 4);
                            string condition = null;
                            int gotogroup = -1;
                            if (arglist.Length < 3)
                            {
                                status_str = "insufficient args to WHILE";
                            }
                            else
                            {
                                condition = arglist[1];
                                if (!int.TryParse(arglist[2], out gotogroup))
                                {
                                    status_str = "invalid dogroup arg to WHILE";
                                    gotogroup = -1;
                                }
                            }
                            if (status_str != null)
                            {
                                return false;
                            }
                            // test the condition
                            if (TestItemProperty(spawner, spawner, condition, triggermob, out status_str))
                            {
                                // try to spawn the dogroup
                                if (spawner != null && !spawner.Deleted)
                                {
                                    if (gotogroup >= 0)
                                    {
                                        if (loops >= XmlSpawner.MaxLoops)
                                        {
                                            status_str = "recursive looping stop in WHILE";
                                            return false;
                                        }
                                        // spawn the subgroup
                                        spawner.SpawnSubGroup(gotogroup, (byte)(loops + 1));
                                        // advance the sequence to that group
                                        //spawner.SequentialSpawn = gotogroup;
                                    }
                                    // and suppress sequential advancement
                                    spawner.HoldSequence = true;

                                }

                            }
                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.IF:
                        {
                            // the syntax is IF/condition/thengroup [/elsegroup]
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 5);
                            string condition = null;
                            int thengroup = -1;
                            int elsegroup = -1;
                            if (arglist.Length < 3)
                            {
                                status_str = "insufficient args to IF";
                            }
                            else
                            {
                                condition = arglist[1];
                                if (!int.TryParse(arglist[2], out thengroup))
                                {
                                    status_str = "invalid thengroup arg to IF";
                                    thengroup = -1;
                                }
                            }
                            if (arglist.Length > 3)
                            {
                                if (!int.TryParse(arglist[3], out elsegroup))
                                {
                                    status_str = "invalid elsegroup arg to IF";
                                    elsegroup = -1;
                                }
                            }

                            if (status_str != null)
                            {
                                return false;
                            }

                            // test the condition
                            if (TestItemProperty(spawner, spawner, condition, triggermob, out status_str))
                            {
                                // try to spawn the thengroup
                                if (thengroup >= 0 && spawner != null && !spawner.Deleted)
                                {
                                    // spawn the subgroup
                                    if (loops >= XmlSpawner.MaxLoops)
                                    {
                                        status_str = "recursive looping stop in IF";
                                        return false;
                                    }
                                    spawner.SpawnSubGroup(thengroup, (byte)(loops + 1));
                                    // advance the sequence to that group
                                }
                            }
                            else
                            {
                                // try to spawn the elsegroup
                                if (elsegroup >= 0 && spawner != null && !spawner.Deleted)
                                {
                                    // spawn the subgroup
                                    if (loops >= XmlSpawner.MaxLoops)
                                    {
                                        status_str = "recursive looping stop in IF";
                                        return false;
                                    }
                                    spawner.SpawnSubGroup(elsegroup, (byte)(loops + 1));
                                    // advance the sequence to that group
                                }
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
                                else
                                {
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
                                else
                                {
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
                    //
                    //  MEFFECT keyword
                    //
                    case typeKeyword.MEFFECT:
                        {
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            if (arglist.Length > 0)
                            {
                                string[] keywordargs = ParseString(arglist[0], 9, ",");

                                if (keywordargs.Length < 9)
                                {
                                    status_str = "Missing args";
                                }
                                else
                                {
                                    int effect;
                                    int duration = 0;
                                    int speed;
                                    Point3D eloc1 = new Point3D(0, 0, 0);
                                    Point3D eloc2 = new Point3D(0, 0, 0);
                                    Map emap = Map.Internal;

                                    // syntax is MEFFECT,itemid,speed,x,y,z,x2,y2,z2

                                    // try to get the effect argument

                                    if (!int.TryParse(keywordargs[1], out effect))
                                    {
                                        status_str = "Improper effect number format";
                                        effect = -1;
                                    }

                                    if (!int.TryParse(keywordargs[2], out speed))
                                    {
                                        status_str = "Improper effect speed format";
                                        speed = 1;
                                    }

                                    int y = 0, z = 0;
                                    int x;
                                    if (!int.TryParse(keywordargs[3], out x) || !int.TryParse(keywordargs[4], out y) || !int.TryParse(keywordargs[5], out z))
                                        status_str = "Improper effect location format";
                                    eloc1 = new Point3D(x, y, z);

                                    if (!int.TryParse(keywordargs[6], out x) || !int.TryParse(keywordargs[7], out y) || !int.TryParse(keywordargs[8], out z))
                                        status_str = "Improper effect location format";
                                    eloc2 = new Point3D(x, y, z);


                                    if (effect >= 0 && emap != Map.Internal)
                                    {
                                        Effects.SendPacket(eloc1, emap, new HuedEffect(EffectType.Moving, -1, -1, effect, eloc1, eloc2, speed, duration, false, false, 0, 0));
                                    }
                                }
                                if (status_str != null)
                                {
                                    return false;
                                }
                            }
                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));
                            break;
                        }
                    case typeKeyword.EFFECT:
                        {
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            if (spawner == null || spawner.Deleted) return false;
                            if (arglist.Length > 0)
                            {
                                string[] keywordargs = ParseString(arglist[0], 6, ",");
                                int effect = -1;
                                int duration = 1;
                                // syntax is EFFECT,itemid,duration[,x,y,z] or EFFECT,itemid,duration[,trigmob]
                                // try to get the effect argument
                                // some interesting effects are explosion(14013,15), sparkle(14155,15), explosion2(14000,13)
                                if (keywordargs.Length < 3)
                                {
                                    status_str = "Missing effect number and duration";
                                }
                                else
                                {
                                    if (!int.TryParse(keywordargs[1], out effect))
                                    {
                                        status_str = "Improper effect number format";
                                        effect = -1;
                                    }
                                    if (!int.TryParse(keywordargs[2], out duration))
                                    {
                                        status_str = "Improper effect duration format";
                                        duration = 1;
                                    }
                                }
                                // by default just use the spawner location
                                Point3D eloc = spawner.Location;
                                Map emap = spawner.Map;
                                if (keywordargs.Length > 3)
                                {
                                    // is this applied to the trig mob or to a location?
                                    if (keywordargs.Length > 5)
                                    {
                                        int x, y, z;
                                        if (!int.TryParse(keywordargs[3], out x) || !int.TryParse(keywordargs[4], out y) || !int.TryParse(keywordargs[5], out z))
                                        {
                                            status_str = "Improper effect location format";
                                            x = spawner.Location.X;
                                            y = spawner.Location.Y;
                                            z = spawner.Location.Z;
                                        }
                                        eloc = new Point3D(x, y, z);
                                    }
                                }
                                if (status_str != null)
                                {
                                    return false;
                                }
                                if (effect >= 0)
                                {
                                    Effects.SendLocationEffect(eloc, emap, effect, duration);
                                }
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.DAMAGE:
                        {
                            // the syntax is DAMAGE,damage,phys,fire,cold,pois,energy[,range][,playeronly]
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            if (arglist.Length > 0)
                            {
                                ApplyDamageToPlayers(arglist[0], triggermob, invoker, out status_str);
                                if (status_str != null)
                                {
                                    return false;
                                }
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.RESURRECT:
                        {
                            // the syntax is RESURRECT[,range][,PETS]
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            if (arglist.Length > 0)
                            {
                                ResurrectPlayers(arglist[0], triggermob, invoker, out status_str);
                                if (status_str != null)
                                {
                                    return false;
                                }
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));

                            break;
                        }
                    case typeKeyword.CAST:
                        {
                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);
                            // Syntax is CAST,spellnumber[,arg] or CAST,spellname[,arg]
                            string[] keywordargs = ParseString(arglist[0], 3, ",");
                            int spellnumber;
                            bool hasnumber;
                            // try it as spellnumber
                            if (keywordargs.Length > 1)
                            {
                                hasnumber = int.TryParse(keywordargs[1], out spellnumber);
                            }
                            else
                            {
                                status_str = "invalid CAST specification";
                                // note that returning true means that Spawn will assume that it worked and will not try to recast
                                return true;
                            }
                            // call this with the 3 argument version that includes the bodytype arg
                            int keywordarg2 = 0;
                            if (keywordargs.Length > 2)
                            {
                                int.TryParse(keywordargs[2], out keywordarg2);
                            }


                            // the trigger mob will cast the spells

                            Mobile caster = triggermob;
                            if (caster == null)
                            {
                                // note that returning true means that Spawn will assume that it worked and will not try to recast
                                return true;
                            }

                            // make the placeholder wand to avoid reagent and mana use
                            BaseWand cwand = new ClumsyWand
                            {
                                Parent = caster
                            };


                            Spell spell;
                            if (hasnumber)
                            {
                                spell = SpellRegistry.NewSpell(spellnumber, caster, cwand);
                            }
                            else
                            {
                                spell = SpellRegistry.NewSpell(keywordargs[1], caster, cwand);
                            }
                            if (spell != null)
                            {
                                bool casterror = false;
                                try
                                {
                                    // deal with the 3 types of spells, mob targeted, location targeted, and self targeted
                                    // dont go through all of the warm up stuff, get right to the casting
                                    spell.State = SpellState.Sequencing;

                                    Type spelltype = spell.GetType();
                                    // deal with any special cases here
                                    if (spelltype == typeof(Spells.Seventh.PolymorphSpell))
                                    {
                                        if (keywordarg2 == 0)
                                        {
                                            // this is invalid so dont cast
                                            throw (new ArgumentNullException());
                                        }
                                        object[] polyargs = new object[3];
                                        polyargs[0] = caster;
                                        polyargs[1] = cwand;
                                        polyargs[2] = keywordarg2;
                                        spell = (Spell)Activator.CreateInstance(spelltype, polyargs);

                                        if (spell == null)
                                        {
                                            throw (new ArgumentNullException());
                                        }
                                        spell.State = SpellState.Sequencing;
                                    }
                                    MethodInfo spelltargetmethod = null;

                                    // get the targeting method from the spell
                                    // note, the precedence is important as the target call should override oncast if it is present
                                    if (spelltype != null && (spelltargetmethod = spelltype.GetMethod("Target")) != null)
                                    {

                                    }
                                    // if it doesnt have it then check for self targeted types
                                    else if (spelltype != null && (spelltargetmethod = spelltype.GetMethod("OnCast")) != null)
                                    {

                                    }
                                    else
                                    {
                                        throw (new ArgumentNullException());
                                    }
                                    // Get the parameters for the target method.
                                    ParameterInfo[] spelltargetparms = spelltargetmethod.GetParameters();
                                    // target will have one parm
                                    // selftarg will have none
                                    object[] targetargs = null;
                                    // check the parameters
                                    if (spelltargetparms != null && spelltargetparms.Length > 0)
                                    {
                                        if (spelltargetparms[0].ParameterType == typeof(Mobile))
                                        {
                                            // set the target parameter
                                            targetargs = new object[1];
                                            targetargs[0] = triggermob;
                                        }
                                        else if (spelltargetparms[0].ParameterType == typeof(IPoint3D))
                                        {
                                            // set the target parameter
                                            targetargs = new object[1];
                                            // pick a random point around the caster
                                            int range = keywordarg2;
                                            if (range == 0) range = 1;
                                            int randx = Utility.RandomMinMax(-range, range);
                                            int randy = Utility.RandomMinMax(-range, range);
                                            if (randx == 0 && randy == 0) randx = 1;
                                            targetargs[0] = new Point3D(triggermob.Location.X + randx,
                                                triggermob.Location.Y + randy,
                                                triggermob.Location.Z);
                                        }
                                        else
                                        {
                                            // dont handle any other types of args
                                            throw (new ArgumentNullException());
                                        }
                                    }
                                    // set the spell on the caster
                                    caster.Spell = spell;
                                    // invoke the spell method with the appropriate args
                                    spelltargetmethod.Invoke(spell, targetargs);

                                    // get rid of the placeholder wand
                                    if (cwand != null && !cwand.Deleted)
                                        cwand.Delete();
                                }
                                catch
                                {
                                    status_str = "bad spell call : " + spell.Name;
                                    casterror = true;
                                    // get rid of the placeholder wand
                                    if (cwand != null && !cwand.Deleted)
                                        cwand.Delete();
                                }

                                if (casterror) return true;
                            }
                            else
                            {
                                status_str = "spell invalid or disabled : " + keywordargs[1];

                                //return true;
                            }

                            TheSpawn.SpawnedObjects.Add(new KeywordTag(substitutedtypeName, spawner));
                            // note that returning true means that Spawn assume that it worked and will not try to recast
                            break;
                        }
                    case typeKeyword.BCAST:
                        {
                            // syntax is BCAST[,hue][,font]/message

                            string[] arglist = ParseSlashArgs(substitutedtypeName, 3);

                            int hue = 0x482;
                            int font = -1;

                            if (arglist.Length > 0)
                            {
                                string[] keywordargs = ParseString(arglist[0], 3, ",");
                                if (keywordargs.Length > 1)
                                {
                                    if (!int.TryParse(keywordargs[1], out hue))
                                    {
                                        status_str = "invalid hue arg to BCAST";
                                        hue = 0x482;
                                    }
                                }
                                if (keywordargs.Length > 2)
                                {
                                    if (!int.TryParse(keywordargs[2], out font))
                                    {
                                        status_str = "invalid font arg to BCAST";
                                        font = -1;
                                    }
                                }
                            }

                            if (arglist.Length > 1)
                            {
                                string msg = arglist[1];
                                if (arglist[1] != null && arglist[1].Length > 0 && arglist[1][0] == '@')
                                {
                                    arglist = ParseSlashArgs(substitutedtypeName, 2);
                                    msg = arglist[1].Substring(1);
                                }
                                if (font >= 0)
                                {
                                    // broadcast an ascii message to all players
                                    BroadcastAsciiMessage(AccessLevel.Player, hue, font, msg);
                                }
                                else
                                {
                                    // standard unicode message format
                                    CommandHandlers.BroadcastMessage(AccessLevel.Player, hue, msg);
                                }

                            }
                            else
                            {
                                status_str = "missing msg arg in BCAST";
                                return false;
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
            else
            {
                // should never get here
                status_str = "unrecognized keyword";
                return false;
            }
        }

        #endregion

        #region Specials by Fwiffo
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

        /// <summary>
        /// Converts the string representation of the name value of one or more enumerated constants to an equivalent enumerated object. A parameter specifies whether the operation is case-sensitive (default: false). The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="tocheck"> The string representation of the enumeration name or underlying value to convert.</param>
        /// <param name="result">result: if the method returns true, it's a TEnum whose value is represented by value. Otherwise uninitialized parameter.</param>
        /// <returns> true if the value parameter was converted successfully; otherwise, false. </returns>
        /// <exception cref="ArgumentException"> TEnum is not an enumeration type. </exception>
        public static bool TryParse<TEnum>(string tocheck, out TEnum result) where TEnum : struct, IConvertible
        {
            return TryParse(tocheck, false, out result);
        }

        /// <summary>
        /// Converts the string representation of the name value of one or more enumerated constants to an equivalent enumerated object. A parameter specifies whether the operation is case-sensitive. The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="tocheck"> The string representation of the enumeration name or underlying value to convert.</param>
        /// <param name="ignorecase"> If this parameter is set to true it will ignore case of string tocheck, otherwise it will check case. </param>
        /// <param name="result">result: if the method returns true, it's a TEnum whose value is represented by value. Otherwise uninitialized parameter.</param>
        /// <returns> true if the value parameter was converted successfully; otherwise, false. </returns>
        /// <exception cref="ArgumentException"> TEnum is not an enumeration type. </exception>
        public static bool TryParse<TEnum>(string tocheck, bool ignorecase, out TEnum result) where TEnum : struct, IConvertible
        {
            bool boolean = (tocheck == null ? false : Enum.IsDefined(typeof(TEnum), tocheck));
            result = (boolean ? (TEnum)Enum.Parse(typeof(TEnum), tocheck) : default(TEnum));
            return boolean;
        }
        #endregion
    }
}
