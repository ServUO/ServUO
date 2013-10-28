#region Header
// **********
// ServUO - OpenBackpackEntry.cs
// **********
#endregion

namespace Server.ContextMenus
{
	public class OpenBackpackEntry : ContextMenuEntry
	{
		private readonly Mobile m_Mobile;

		public OpenBackpackEntry(Mobile m)
			: base(6145)
		{
			m_Mobile = m;
		}

		public override void OnClick()
		{
			m_Mobile.Use(m_Mobile.Backpack);
		}
	}
}