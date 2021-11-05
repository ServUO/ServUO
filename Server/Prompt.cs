using System;

using Server.Gumps;
using Server.Network;

namespace Server.Prompts
{
	public abstract class Prompt
	{
		private static int m_Serials;

		public string MessageArgs { get; }

		public virtual int MessageCliloc => 1042971;

		public virtual int MessageHue => 0;

		public int Serial { get; }

		public virtual int TypeId => GetType().FullName.GetHashCode();

		protected Prompt()
		{
			do
			{
				Serial = ++m_Serials;
			}
			while (Serial == 0);
		}

		public Prompt(string args)
			: this()
		{
			MessageArgs = args ?? String.Empty;
		}

		public virtual void OnCancel(Mobile from)
		{
		}

		public virtual void OnResponse(Mobile from, string text)
		{
		}

		public void SendTo(Mobile m)
		{
			if (m == null)
				return;

			var ns = m.NetState;

			if (ns == null)
				return;

			if (ns.IsEnhancedClient)
			{
				m.SendGump(new PromptGump(this, m));
				return;
			}

			if (MessageCliloc > 0 && (MessageCliloc != 1042971 || !String.IsNullOrEmpty(MessageArgs)))
			{
				m.SendLocalizedMessage(MessageCliloc, MessageArgs, MessageHue);
			}

			m.Send(new UnicodePrompt(this));
		}
	}

	public class PromptGump : Gump
	{
		public Mobile User { get; }

		public PromptGump(Prompt prompt, Mobile to)
			: base(0, 0)
		{
			User = to;
			Serial = to.Serial;

			Intern("TEXTENTRY", false);
			Intern(Serial.ToString(), false);
			Intern(to.Serial.Value.ToString(), false);
			Intern(prompt.TypeId.ToString(), false);
			Intern(prompt.MessageCliloc.ToString(), false); // TODO: Is there a way to include args here?
			Intern("1", false); // 0 = Ascii response, 1 = Unicode Response

			AddBackground(50, 50, 540, 350, 0xA28);

			AddPage(0);

			AddHtmlLocalized(264, 80, 200, 24, 1062524, false, false);
			AddHtmlLocalized(120, 108, 420, 48, 1062638, false, false);
			AddBackground(100, 148, 440, 200, 0xDAC);
			AddTextEntryIntern(120, 168, 400, 200, 0x0, 44, 0);
			AddButton(175, 355, 0x81A, 0x81B, 1, GumpButtonType.Reply, 0);
			AddButton(405, 355, 0x819, 0x818, 0, GumpButtonType.Reply, 0);
		}

		public override int GetTypeID()
		{
			return 0x2AE;
		}
	}
}