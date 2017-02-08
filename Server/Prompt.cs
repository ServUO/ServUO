#region Header
// **********
// ServUO - Prompt.cs
// **********
#endregion

using System;

namespace Server.Prompts
{
	public abstract class Prompt
	{
        private IEntity m_Sender;
        private String m_MessageArgs;

        public IEntity Sender
        {
            get { return m_Sender; }
        }

        public String MessageArgs
        {
            get { return m_MessageArgs; }
        }

        public virtual int MessageCliloc
        {
            get { return 1042971; } // ~1_NOTHING~
        }

        public virtual int MessageHue
        {
            get { return 0; }
        }

        public virtual int TypeId
        {
            get { return GetType().FullName.GetHashCode(); }
        }

        public Prompt()
            : this(null)
        {
        }

        public Prompt(IEntity sender)
            : this(sender, String.Empty)
        {
        }

        public Prompt(IEntity sender, String args)
        {
            m_Sender = sender;
            m_MessageArgs = args;
        }

        public virtual void OnCancel(Mobile from)
        {
        }

        public virtual void OnResponse(Mobile from, string text)
        {
        }
    }
}