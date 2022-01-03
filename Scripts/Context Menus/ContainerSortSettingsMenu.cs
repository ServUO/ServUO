using Server.Items;
using Server.Services.ContainerSort;

namespace Server.ContextMenus
{
    internal class ContainerSortSettingsMenu : ContextMenuEntry
	{
		private readonly Item m_Item;

		public ContainerSortSettingsMenu(Item item)
			: base(3001018)
		{
			m_Item = item;
		}

		public override void OnClick()
		{
			if (m_Item is Container && Owner.From.OwnsContainer(m_Item as Container))
			{
				((Container)m_Item).DisplaySortSettingsTo(Owner.From);
			}
		}
	}
}