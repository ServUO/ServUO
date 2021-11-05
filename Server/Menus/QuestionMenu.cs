#region References
using Server.Network;
#endregion

namespace Server.Menus.Questions
{
	public class QuestionMenu : IMenu
	{
		private static int m_NextSerial;

		private readonly string[] m_Answers;

		private readonly int m_Serial;

		int IMenu.Serial => m_Serial;

		int IMenu.EntryLength => m_Answers.Length;

		public int GumpID { get; set; }

		public string Question { get; set; }

		public string[] Answers => m_Answers;

		public QuestionMenu(string question, string[] answers)
		{
			//GumpID = 83;

			Question = question;
			m_Answers = answers;

			do
			{
				m_Serial = ++m_NextSerial;
				m_Serial &= 0x7FFFFFFF;
			}
			while (m_Serial == 0);
		}

		public virtual void OnCancel(NetState state)
		{ }

		public virtual void OnResponse(NetState state, int index)
		{ }

		public void SendTo(NetState state)
		{
			state.AddMenu(this);
			state.Send(new DisplayQuestionMenu(this));
		}
	}
}