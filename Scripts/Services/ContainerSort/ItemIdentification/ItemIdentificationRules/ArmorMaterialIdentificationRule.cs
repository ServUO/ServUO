using Server.Items;

namespace Server.Services.ContainerSort.ItemIdentification.ItemIdentificationRules
{
	public class ArmorMaterialIdentificationRule : IItemIdentificationRule
	{
		private readonly ArmorMaterialType MaterialType;

		public ArmorMaterialIdentificationRule(ArmorMaterialType materialType)
		{
			MaterialType = materialType;
		}

		public bool DoesItemQualifyForSortFilter(Item item)
		{
			return item is BaseArmor baseArmor && baseArmor.MaterialType == MaterialType;
		}
	}
}