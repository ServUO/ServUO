#region Header
// **********
// ServUO - Prompt.cs
// **********
#endregion

namespace Server.Prompts
{
	public abstract class Prompt
	{
		private readonly int m_Serial;
		private static int m_Serials;

		public int Serial { get { return m_Serial; } }

		protected Prompt()
		{
			do
			{
				m_Serial = ++m_Serials;
			}
			while (m_Serial == 0);
		}

		public virtual void OnCancel(Mobile from)
		{ }

		public virtual void OnResponse(Mobile from, string text)
		{ }
	}
}