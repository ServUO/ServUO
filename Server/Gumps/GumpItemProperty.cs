using Server.Network;

namespace Server.Gumps
{
	public class GumpItemProperty : GumpEntry
	{
		private int m_Serial;

		public GumpItemProperty(int serial)
		{
			m_Serial = serial;
		}

		public int Serial
		{
			get => m_Serial;
			set => Delta(ref m_Serial, value);
		}

		public override string Compile()
		{
			return $"{{ itemproperty {m_Serial} }}";
		}

		private static readonly byte[] m_LayoutName = Gump.StringToBuffer("itemproperty");

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(m_LayoutName);
			disp.AppendLayout(m_Serial);
		}
	}
}
