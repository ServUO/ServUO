using Server.Network;

namespace Server.Gumps
{
	public class GumpGroup : GumpEntry
	{
		private int m_Group;

		public GumpGroup(int group)
		{
			m_Group = group;
		}

		public int Group
		{
			get => m_Group;
			set => Delta(ref m_Group, value);
		}

		public override string Compile()
		{
			return $"{{ group {m_Group} }}";
		}

		private static readonly byte[] m_LayoutName = Gump.StringToBuffer("group");

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(m_LayoutName);
			disp.AppendLayout(m_Group);
		}
	}
}