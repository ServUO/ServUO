namespace Server.Prompts
{
	public abstract class Prompt
	{
		private readonly IEntity m_Sender;
		private readonly string m_MessageArgs;

		public IEntity Sender => m_Sender;

		public string MessageArgs => m_MessageArgs;

		public virtual int MessageCliloc => 1042971;

		public virtual int MessageHue => 0;

		public virtual int TypeId => GetType().FullName.GetHashCode();

		public Prompt()
			: this(null)
		{
		}

		public Prompt(IEntity sender)
			: this(sender, string.Empty)
		{
		}

		public Prompt(IEntity sender, string args)
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