#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Engines.Craft;
using Server.Items;
#endregion

namespace VitaNex.SuperCrafts
{
	public sealed class CraftInfo
	{
		public static ResourceInfo[] DefResources = { new ResourceInfo(typeof(Gold), "Gold Coin", 60000) };

		public CraftItem CraftItem { get; private set; }

		public Type TypeOf { get; private set; }
		public TextDefinition Group { get; private set; }
		public TextDefinition Name { get; private set; }
		public SkillName Skill { get; private set; }
		public double MinSkill { get; private set; }
		public double MaxSkill { get; private set; }
		public ResourceInfo[] Resources { get; private set; }

		private readonly Action<CraftItem> _OnAdded;

		public CraftInfo(
			Type t,
			TextDefinition group,
			TextDefinition name,
			SkillName skill,
			double skillReq,
			IEnumerable<ResourceInfo> resources,
			Action<CraftItem> onAdded = null)
			: this(t, group, name, skill, skillReq, skillReq, resources, onAdded)
		{ }

		public CraftInfo(
			Type t,
			TextDefinition group,
			TextDefinition name,
			SkillName skill,
			double minSkill,
			double maxSkill,
			IEnumerable<ResourceInfo> resources,
			Action<CraftItem> onAdded = null)
		{
			TypeOf = t;
			Group = group.IsNullOrWhiteSpace() ? new TextDefinition("Misc") : group;
			Name = name.IsNullOrWhiteSpace() ? new TextDefinition("Unknown") : name;
			Skill = skill;
			MinSkill = Math.Max(0.0, Math.Min(minSkill, maxSkill));
			MaxSkill = Math.Max(MinSkill, Math.Max(minSkill, maxSkill));

			if (resources != null)
			{
				Resources = resources.Where(r => r.TypeOf != null && r.TypeOf.IsEqualOrChildOf<Item>()).ToArray();
			}

			if (Resources.Length == 0)
			{
				Resources = DefResources.Dupe(r => new ResourceInfo(r.TypeOf, r.Name, r.Amount));
			}

			_OnAdded = onAdded;
		}

		public void OnAdded(CraftItem item)
		{
			if (item == null)
			{
				return;
			}

			CraftItem = item;

			if (_OnAdded != null)
			{
				_OnAdded(CraftItem);
			}
		}
	}
}