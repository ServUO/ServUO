namespace Server.Services.ContainerSort.ItemIdentification.ItemIdentificationRules
{
	public interface IItemIdentificationRule
	{
		bool DoesItemQualifyForSortFilter(Item item);
	}
}