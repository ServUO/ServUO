using Server.Gumps;
using System.Collections;

namespace Server.Commands.Generic
{
    public enum ObjectTypes
    {
        Both,
        Items,
        Mobiles,
        All
    }

    public abstract class BaseCommand
    {
        private readonly ArrayList m_Responses;
        private readonly ArrayList m_Failures;
        private string[] m_Commands;
        private AccessLevel m_AccessLevel;
        private CommandSupport m_Implementors;
        private ObjectTypes m_ObjectTypes;
        private bool m_ListOptimized;
        private string m_Usage;
        private string m_Description;
        public BaseCommand()
        {
            m_Responses = new ArrayList();
            m_Failures = new ArrayList();
        }

        public bool ListOptimized
        {
            get
            {
                return m_ListOptimized;
            }
            set
            {
                m_ListOptimized = value;
            }
        }
        public string[] Commands
        {
            get
            {
                return m_Commands;
            }
            set
            {
                m_Commands = value;
            }
        }
        public string Usage
        {
            get
            {
                return m_Usage;
            }
            set
            {
                m_Usage = value;
            }
        }
        public string Description
        {
            get
            {
                return m_Description;
            }
            set
            {
                m_Description = value;
            }
        }
        public AccessLevel AccessLevel
        {
            get
            {
                return m_AccessLevel;
            }
            set
            {
                m_AccessLevel = value;
            }
        }
        public ObjectTypes ObjectTypes
        {
            get
            {
                return m_ObjectTypes;
            }
            set
            {
                m_ObjectTypes = value;
            }
        }
        public CommandSupport Supports
        {
            get
            {
                return m_Implementors;
            }
            set
            {
                m_Implementors = value;
            }
        }
        public static bool IsAccessible(Mobile from, object obj)
        {
            if (from.AccessLevel >= AccessLevel.Administrator || obj == null)
                return true;

            Mobile mob;

            if (obj is Mobile)
                mob = (Mobile)obj;
            else if (obj is Item)
                mob = ((Item)obj).RootParent as Mobile;
            else
                mob = null;

            if (mob == null || mob == from || from.AccessLevel > mob.AccessLevel)
                return true;

            return false;
        }

        public virtual void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            for (int i = 0; i < list.Count; ++i)
                Execute(e, list[i]);
        }

        public virtual void Execute(CommandEventArgs e, object obj)
        {
        }

        public virtual bool ValidateArgs(BaseCommandImplementor impl, CommandEventArgs e)
        {
            return true;
        }

        public void AddResponse(string message)
        {
            for (int i = 0; i < m_Responses.Count; ++i)
            {
                MessageEntry entry = (MessageEntry)m_Responses[i];

                if (entry.m_Message == message)
                {
                    ++entry.m_Count;
                    return;
                }
            }

            if (m_Responses.Count == 10)
                return;

            m_Responses.Add(new MessageEntry(message));
        }

        public void AddResponse(Gump gump)
        {
            m_Responses.Add(gump);
        }

        public void LogFailure(string message)
        {
            for (int i = 0; i < m_Failures.Count; ++i)
            {
                MessageEntry entry = (MessageEntry)m_Failures[i];

                if (entry.m_Message == message)
                {
                    ++entry.m_Count;
                    return;
                }
            }

            if (m_Failures.Count == 10)
                return;

            m_Failures.Add(new MessageEntry(message));
        }

        public void Flush(Mobile from, bool flushToLog)
        {
            if (m_Responses.Count > 0)
            {
                for (int i = 0; i < m_Responses.Count; ++i)
                {
                    object obj = m_Responses[i];

                    if (obj is MessageEntry)
                    {
                        from.SendMessage(((MessageEntry)obj).ToString());

                        if (flushToLog)
                            CommandLogging.WriteLine(from, ((MessageEntry)obj).ToString());
                    }
                    else if (obj is Gump)
                    {
                        from.SendGump((Gump)obj);
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_Failures.Count; ++i)
                    from.SendMessage(((MessageEntry)m_Failures[i]).ToString());
            }

            m_Responses.Clear();
            m_Failures.Clear();
        }

        private class MessageEntry
        {
            public readonly string m_Message;
            public int m_Count;
            public MessageEntry(string message)
            {
                m_Message = message;
                m_Count = 1;
            }

            public override string ToString()
            {
                if (m_Count > 1)
                    return string.Format("{0} ({1})", m_Message, m_Count);

                return m_Message;
            }
        }
    }
}