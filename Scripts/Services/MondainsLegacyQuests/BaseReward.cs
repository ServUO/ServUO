#region Header
// **********
// ServUO - BaseReward.cs
// **********
#endregion

#region References
using System;

using Server.Engines.Craft;
using Server.Items;
#endregion

namespace Server.Engines.Quests
{
	public class BaseReward
	{
		private static readonly int[] m_SatchelHues = new[]
		{0x1C, 0x37, 0x71, 0x3A, 0x62, 0x44, 0x59, 0x13, 0x21, 0x3, 0xD, 0x3F,}; // TODO update

		private static readonly int[] m_RewardBagHues = new[]
		{
			//	from,	to,
			0x385, 0x3E9, 0x4B0, 0x4E6, 0x514, 0x54A, 0x578, 0x5AE, 0x5DC, 0x612, 0x640, 0x676, 0x6A5, 0x6DA, 0x708, 0x774
		};

		public BaseReward(object name)
			: this(null, 1, name)
		{ }

		public BaseReward(Type type, object name)
			: this(type, 1, name)
		{ }

		public BaseReward(Type type, int amount, object name)
		{
			Type = type;
			Amount = amount;
			Name = name;
		}

		public BaseQuest Quest { get; set; }

		public Type Type { get; set; }

		public int Amount { get; set; }

		public object Name { get; set; }

		public static int SatchelHue()
		{
			return m_SatchelHues[Utility.Random(m_SatchelHues.Length)];
		}

		public static int RewardBagHue()
		{
			if (Utility.RandomDouble() < 0.005)
			{
				return 0;
			}

			int row = Utility.Random(m_RewardBagHues.Length / 2) * 2;

			return Utility.RandomMinMax(m_RewardBagHues[row], m_RewardBagHues[row + 1]);
		}

		public static int StrongboxHue()
		{
			return Utility.RandomMinMax(0x898, 0x8B0);
		}

		private static void ApplyMods(Item item)
		{
			if (item != null)
			{
				if (Core.SA && Mobiles.RandomItemGenerator.Enabled)
				{
					RunicReforging.GenerateRandomItem(item, 0, 0, 700);
				}
				else
				{
					int attributeCount = Utility.RandomMinMax(1, 5);
					if(item is BaseJewel)
						BaseRunicTool.ApplyAttributesTo((BaseJewel)item, false, 0, attributeCount, 10, 100);
					else if (item is BaseWeapon)
						BaseRunicTool.ApplyAttributesTo((BaseWeapon)item, false, 0, attributeCount, 10, 100);
					else if (item is BaseRanged)
						BaseRunicTool.ApplyAttributesTo((BaseRanged)item, false, 0, attributeCount, 10, 100);
					else if (item is BaseArmor)
						BaseRunicTool.ApplyAttributesTo((BaseArmor)item, false, 0, attributeCount, 10, 100);
				}
			}
		}

		public static Item Jewlery()
		{
			BaseJewel item = Loot.RandomJewelry();
			ApplyMods(item);

			return item;
		}

		public static Item FletcherRecipe()
		{
			return GetRecipe(Enum.GetValues(typeof(BowRecipes)));
		}

		public static Item FletcherRunic()
		{
			switch (Utility.Random(4))
			{
				case 0:
					return new RunicFletcherTool(CraftResource.OakWood, 45);
				case 1:
					return new RunicFletcherTool(CraftResource.AshWood, 35);
				case 2:
					return new RunicFletcherTool(CraftResource.YewWood, 25);
				case 3:
					return new RunicFletcherTool(CraftResource.Heartwood, 15);
			}

			return null;
		}

		public static Item RangedWeapon()
		{
			BaseWeapon item = Loot.RandomRangedWeapon(false, true);
			ApplyMods(item);

			return item;
		}

		public static Item TailorRecipe()
		{
			return GetRecipe(Enum.GetValues(typeof(TailorRecipe)));
		}

		public static Item Armor()
		{
			BaseArmor item = Loot.RandomArmor(false, true);
			ApplyMods(item);

			return item;
		}

		public static Item SmithRecipe()
		{
			return GetRecipe(Enum.GetValues(typeof(SmithRecipes)));
		}

		public static Item Weapon()
		{
			BaseWeapon item = Loot.RandomWeapon(false, true);
			ApplyMods(item);

			return item;
		}

		public static Item TinkerRecipe()
		{
			return GetRecipe(Enum.GetValues(typeof(TinkerRecipes)));
		}

		public static Item CarpRecipe()
		{
			return GetRecipe(Enum.GetValues(typeof(CarpRecipes)));
		}

		public static Item CarpRunic()
		{
			switch (Utility.Random(4))
			{
				case 0:
					return new RunicDovetailSaw(CraftResource.OakWood, 45);
				case 1:
					return new RunicDovetailSaw(CraftResource.AshWood, 35);
				case 2:
					return new RunicDovetailSaw(CraftResource.YewWood, 25);
				case 3:
					return new RunicDovetailSaw(CraftResource.Heartwood, 15);
			}

			return null;
		}

		public static Item CookRecipe()
		{
			return GetRecipe(Enum.GetValues(typeof(CookRecipes)));
		}

		public static RecipeScroll GetRecipe(Array list)
		{
			var recipes = new int[list.Length];

			int index = 0;
			int mid = -1;

			foreach (int i in list)
			{
				int val = i - (i / 100) * 100;

				if (val >= 50 && mid == -1)
				{
					mid = index;
				}

				recipes[index] = i;
				index += 1;
			}

			if (list.Length == 0) // empty list
			{
				return null;
			}
			
			if (mid == -1) // only lesser recipes in list
			{
				return new RecipeScroll(recipes[Utility.Random(list.Length)]);
			}
			
			if (mid == 0) // only greater recipes in list
			{
				if (Utility.RandomDouble() < 0.01)
				{
					return new RecipeScroll(recipes[Utility.Random(list.Length)]);
				}
			}
			else
			{
				if (Utility.RandomDouble() < 0.01)
				{
					return new RecipeScroll(recipes[Utility.RandomMinMax(mid, list.Length - 1)]);
				}

				return new RecipeScroll(recipes[Utility.Random(mid)]);
			}

			return null;
		}

		public virtual void GiveReward()
		{ }
	}
}