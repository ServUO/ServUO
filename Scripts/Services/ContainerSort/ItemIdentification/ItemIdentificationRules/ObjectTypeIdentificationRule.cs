namespace Server.Services.ContainerSort.ItemIdentification.ItemIdentificationRules
{
	public class ObjectTypeIdentificationRule<T> : IItemIdentificationRule
		where T : Item
	{
		public bool DoesItemQualifyForSortFilter(Item item)
		{
			return item is T;
		}
	}
}