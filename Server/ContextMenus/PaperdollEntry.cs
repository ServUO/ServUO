#region Header
// **********
// ServUO - PaperdollEntry.cs
// **********
#endregion

namespace Server.ContextMenus
{
	public class PaperdollEntry : ContextMenuEntry
	{
		private readonly Mobile m_Mobile;

		public PaperdollEntry(Mobile m)
			: base(6123, 18)
		{
			m_Mobile = m;
		}

		public override void OnClick()
		{
			if (m_Mobile.CanPaperdollBeOpenedBy(Owner.From))
			{
				m_Mobile.DisplayPaperdollTo(Owner.From);
			}
		}
	}
}