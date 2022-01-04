using Server.Items;

namespace Server.Services.ContainerSort.ItemIdentification.ItemIdentificationRules
{
	public class ItemPowerIdentificationRule : IItemIdentificationRule
	{
		private readonly ItemPower ItemPower;

		public ItemPowerIdentificationRule(ItemPower itemPower)
		{
			ItemPower = itemPower;
		}

		public bool DoesItemQualifyForSortFilter(Item item)
		{
			return item is ICombatEquipment combatEquipment && combatEquipment.ItemPower == ItemPower;
		}
	}
}