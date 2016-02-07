using System;
using System.Collections;
using Server.Gumps;

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
            this.m_Responses = new ArrayList();
            this.m_Failures = new ArrayList();
        }

        public bool ListOptimized
        {
            get
            {
                return this.m_ListOptimized;
            }
            set
            {
                this.m_ListOptimized = value;
            }
        }
        public string[] Commands
        {
            get
            {
                return this.m_Commands;
            }
            set
            {
                this.m_Commands = value;
            }
        }
        public string Usage
        {
            get
            {
                return this.m_Usage;
            }
            set
            {
                this.m_Usage = value;
            }
        }
        public string Description
        {
            get
            {
                return this.m_Description;
            }
            set
            {
                this.m_Description = value;
            }
        }
        public AccessLevel AccessLevel
        {
            get
            {
                return this.m_AccessLevel;
            }
            set
            {
                this.m_AccessLevel = value;
            }
        }
        public ObjectTypes ObjectTypes
        {
            get
            {
                return this.m_ObjectTypes;
            }
            set
            {
                this.m_ObjectTypes = value;
            }
        }
        public CommandSupport Supports
        {
            get
            {
                return this.m_Implementors;
            }
            set
            {
                this.m_Implementors = value;
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
                this.Execute(e, list[i]);
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
            for (int i = 0; i < this.m_Responses.Count; ++i)
            {
                MessageEntry entry = (MessageEntry)this.m_Responses[i];

                if (entry.m_Message == message)
                {
                    ++entry.m_Count;
                    return;
                }
            }

            if (this.m_Responses.Count == 10)
                return;

            this.m_Responses.Add(new MessageEntry(message));
        }

        public void AddResponse(Gump gump)
        {
            this.m_Responses.Add(gump);
        }

        public void LogFailure(string message)
        {
            for (int i = 0; i < this.m_Failures.Count; ++i)
            {
                MessageEntry entry = (MessageEntry)this.m_Failures[i];

                if (entry.m_Message == message)
                {
                    ++entry.m_Count;
                    return;
                }
            }

            if (this.m_Failures.Count == 10)
                return;

            this.m_Failures.Add(new MessageEntry(message));
        }

        public void Flush(Mobile from, bool flushToLog)
        {
            if (this.m_Responses.Count > 0)
            {
                for (int i = 0; i < this.m_Responses.Count; ++i)
                {
                    object obj = this.m_Responses[i];

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
                for (int i = 0; i < this.m_Failures.Count; ++i)
                    from.SendMessage(((MessageEntry)this.m_Failures[i]).ToString());
            }

            this.m_Responses.Clear();
            this.m_Failures.Clear();
        }

        private class MessageEntry
        {
            public readonly string m_Message;
            public int m_Count;
            public MessageEntry(string message)
            {
                this.m_Message = message;
                this.m_Count = 1;
            }

            public override string ToString()
            {
                if (this.m_Count > 1)
                    return String.Format("{0} ({1})", this.m_Message, this.m_Count);

                return this.m_Message;
            }
        }
    }
}