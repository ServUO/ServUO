#region References
using Server.Engines.Craft;
using Server.Items;
using System;
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

        public static void ApplyMods(Item item)
        {
            if (item != null)
            {
                RunicReforging.GenerateRandomItem(item, 0, 10, 850);
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
            double ran = Utility.RandomDouble();

            if (ran <= 0.0001)
            {
                return new RunicFletcherTool(CraftResource.Heartwood, 15);
            }
            else if (ran <= 0.0005)
            {
                return new RunicFletcherTool(CraftResource.YewWood, 25);
            }
            else if (ran <= 0.0025)
            {
                return new RunicFletcherTool(CraftResource.AshWood, 35);
            }
            else if (ran <= 0.005)
            {
                return new RunicFletcherTool(CraftResource.OakWood, 45);
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
            return GetRecipe(new int[] { 501, 502, 503, 504, 505, 550, 551, 552 });
        }

        public static Item Armor()
        {
            BaseArmor item = Loot.RandomArmor(false, true);
            ApplyMods(item);

            return item;
        }

        public static Item SmithRecipe()
        {
            return GetRecipe(new int[] { 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 350, 351, 352, 353, 354 });
        }

        public static Item Weapon()
        {
            BaseWeapon item = Loot.RandomWeapon(false, true);
            ApplyMods(item);

            return item;
        }

        public static Item TinkerRecipe()
        {
            RecipeScroll recipe = null;

            if (0.01 > Utility.RandomDouble())
            {
                recipe = new RecipeScroll(Utility.RandomList(450, 451, 452, 453));
            }

            return recipe;
        }

        public static Item AlchemyRecipe()
        {
            return GetRecipe(new int[] { 400, 401, 402 }); ;
        }

        public static Item CarpentryRecipe()
        {
            int[] array = new int[24];

            for (int i = 0; i <= 20; i++)
            {
                array[i] = 100 + i;
            }

            array[21] = 150;
            array[22] = 151;
            array[23] = 152;

            return GetRecipe(array);
        }

        public static Item CarpenterRunic()
        {
            double ran = Utility.RandomDouble();

            if (ran <= 0.0001)
            {
                return new RunicDovetailSaw(CraftResource.Heartwood, 15);
            }
            else if (ran <= 0.0005)
            {
                return new RunicDovetailSaw(CraftResource.YewWood, 25);
            }
            else if (ran <= 0.0025)
            {
                return new RunicDovetailSaw(CraftResource.AshWood, 35);
            }
            else if (ran <= 0.005)
            {
                return new RunicDovetailSaw(CraftResource.OakWood, 45);
            }

            return null;
        }

        public static Item RandomFurniture()
        {
            if (0.005 >= Utility.RandomDouble())
            {
                return Loot.Construct(ElvishFurniture);
            }

            return null;
        }

        public static Type[] ElvishFurniture =
        {
            typeof(WarriorStatueSouthDeed),
            typeof(WarriorStatueEastDeed),
            typeof(SquirrelStatueSouthDeed),
            typeof(SquirrelStatueEastDeed),
            typeof(ElvenDresserSouthDeed),
            typeof(ElvenDresserEastDeed),
            typeof(TallElvenBedSouthDeed),
            typeof(TallElvenBedEastDeed),
            typeof(StoneAnvilSouthDeed),
            typeof(StoneAnvilEastDeed),
            typeof(OrnateElvenChestEastDeed)
        };

        public static Item CookRecipe()
        {
            return GetRecipe(Enum.GetValues(typeof(CookRecipes)));
        }

        public static RecipeScroll GetRecipe(Array list)
        {
            int[] recipes = new int[list.Length];

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

            double ran = Utility.RandomDouble();

            if (mid == -1 && ran <= 0.33) // only lesser recipes in list
            {
                return new RecipeScroll(recipes[Utility.Random(list.Length)]);
            }
            else if (mid == 0 && ran <= 0.01) // only greater recipes in list
            {
                return new RecipeScroll(recipes[Utility.Random(list.Length)]);
            }
            else
            {
                if (ran <= 0.01)
                {
                    return new RecipeScroll(recipes[Utility.RandomMinMax(mid, list.Length - 1)]);
                }
                else if (ran <= 0.33)
                {
                    return new RecipeScroll(recipes[Utility.Random(mid)]);
                }
            }

            return null;
        }

        public virtual void GiveReward()
        { }
    }
}
