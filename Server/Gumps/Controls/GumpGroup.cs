#region Header
// **********
// ServUO - GumpGroup.cs
// **********
#endregion

#region References
using System;

using Server.Network;
#endregion

namespace Server.Gumps
{
	public class GumpGroup : GumpEntry
	{
		private static readonly byte[] m_LayoutName = Gump.StringToBuffer("group");
		private int m_Group;

		public GumpGroup(int group)
		{
			m_Group = group;
		}

		public int Group { get { return m_Group; } set { Delta(ref m_Group, value); } }

		public override string Compile()
		{
			return String.Format("{{ group {0} }}", m_Group);
		}

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(m_LayoutName);
			disp.AppendLayout(m_Group);
		}
	}
}