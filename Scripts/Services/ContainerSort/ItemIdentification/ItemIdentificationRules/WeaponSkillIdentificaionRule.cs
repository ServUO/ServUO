using Server.Items;

namespace Server.Services.ContainerSort.ItemIdentification.ItemIdentificationRules
{
	public class WeaponSkillIdentificaionRule : IItemIdentificationRule
	{
		private readonly SkillName SkillName;

		public WeaponSkillIdentificaionRule(SkillName skillName)
		{
			SkillName = skillName;
		}
		public bool DoesItemQualifyForSortFilter(Item item)
		{
			return item is BaseWeapon baseWeapon && baseWeapon.Skill == SkillName;
		}
	}
}