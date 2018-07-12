#region Header
// **********
// ServUO - CraftItem.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;

using Server.Commands;
using Server.Engines.Plants;
using Server.Factions;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Quests;
#endregion

namespace Server.Engines.Craft
{
	public enum ConsumeType
	{
		All,
		Half,
		None
	}

	public interface ICraftable
	{
		int OnCraft(
			int quality,
			bool makersMark,
			Mobile from,
			CraftSystem craftSystem,
			Type typeRes,
            ITool tool,
			CraftItem craftItem,
			int resHue);
	}

	public class CraftItem
	{
        /// <summary>
        /// this delegate will handle all crafting functions, 
        /// such as resource check, actual crafting, etc. 
        /// For use for abnormal crafting, ie combine cloth, etc.
        /// </summary>
        public Action<Mobile, CraftItem, ITool> TryCraft { get; set; }

        /// <summary>
        /// this func will create complex items that may require args, or other
        /// things to create that Activator may not be able to accomidate.
        /// </summary>
        public Func<Mobile, CraftItem, ITool, Item> CreateItem { get; set; }

		private readonly CraftResCol m_arCraftRes;
		private readonly CraftSkillCol m_arCraftSkill;
		private readonly Type m_Type;
        public double MinSkillOffset { get; set; }

		private readonly string m_GroupNameString;
		private readonly int m_GroupNameNumber;

		private readonly string m_NameString;
		private readonly int m_NameNumber;

		private BeverageType m_RequiredBeverage;

		private bool m_NeedHeat;
		private bool m_NeedOven;
		private bool m_NeedMill;

		private bool m_UseSubRes2;

		private bool m_ForceNonExceptional;

		public bool ForceNonExceptional { get { return m_ForceNonExceptional; } set { m_ForceNonExceptional = value; } }

		public Expansion RequiredExpansion { get; set; }
        public ThemePack RequiredThemePack { get; set; }

        #region SA
        public bool RequiresBasketWeaving { get; set; }
        public bool RequiresResTarget { get; set; }
        public bool RequiresMechanicalLife { get; set; }
        #endregion

        #region TOL
        private object m_Data;
        private int m_DisplayID;

        public object Data
        {
            get { return m_Data; }
            set { m_Data = value; }
        }

        public int DisplayID
        {
            get { return m_DisplayID; }
            set { m_DisplayID = value; }
        }
        #endregion

        public Func<Mobile, ConsumeType, int> ConsumeResCallback { get; set; }

        private Recipe m_Recipe;

		public Recipe Recipe { get { return m_Recipe; } }

		public void AddRecipe(int id, CraftSystem system)
		{
			if (m_Recipe != null)
			{
				Console.WriteLine(
					"Warning: Attempted add of recipe #{0} to the crafting of {1} in CraftSystem {2}.", id, m_Type.Name, system);
				return;
			}

			m_Recipe = new Recipe(id, system, this);
		}

		private static readonly Dictionary<Type, int> _itemIds = new Dictionary<Type, int>();

		public static int ItemIDOf(Type type)
		{
			int itemId;

			if (!_itemIds.TryGetValue(type, out itemId))
			{
				if (type == typeof(FactionExplosionTrap))
				{
					itemId = 14034;
				}
				else if (type == typeof(FactionGasTrap))
				{
					itemId = 4523;
				}
				else if (type == typeof(FactionSawTrap))
				{
					itemId = 4359;
				}
				else if (type == typeof(FactionSpikeTrap))
				{
					itemId = 4517;
				}
					#region Mondain's Legacy
				else if (type == typeof(ArcaneBookshelfSouthDeed))
				{
					itemId = 0x2DEF;
				}
				else if (type == typeof(ArcaneBookshelfEastDeed))
				{
					itemId = 0x2DF0;
				}
				else if (type == typeof(OrnateElvenChestSouthDeed))
				{
					itemId = 0x2DE9;
				}
				else if (type == typeof(OrnateElvenChestEastDeed))
				{
					itemId = 0x2DEA;
				}
				else if (type == typeof(ElvenWashBasinSouthDeed) ||
					type == typeof(ElvenWashBasinSouthAddonWithDrawer))
				{
					itemId = 0x2D0B;
				}
				else if (type == typeof(ElvenWashBasinEastDeed) ||
					type == typeof(ElvenWashBasinEastAddonWithDrawer))
				{
					itemId = 0x2D0C;
				}
				else if (type == typeof(ElvenDresserSouthDeed))
				{
					itemId = 0x2D09;
				}
				else if (type == typeof(ElvenDresserEastDeed))
				{
					itemId = 0x2D0A;
				}
				#endregion

				if (itemId == 0)
				{
					var attrs = type.GetCustomAttributes(typeof(CraftItemIDAttribute), false);

					if (attrs.Length > 0)
					{
						CraftItemIDAttribute craftItemID = (CraftItemIDAttribute)attrs[0];
						itemId = craftItemID.ItemID;
					}
				}

				if (itemId == 0)
				{
					Item item = null;

					try
					{
						item = Activator.CreateInstance(type) as Item;
					}
					catch
					{ }

					if (item != null)
					{
						itemId = item.ItemID;
						item.Delete();
					}
				}

				_itemIds[type] = itemId;
			}

			return itemId;
		}

		public CraftItem(Type type, TextDefinition groupName, TextDefinition name)
		{
			m_arCraftRes = new CraftResCol();
			m_arCraftSkill = new CraftSkillCol();

			m_Type = type;

			m_GroupNameString = groupName;
			m_NameString = name;

			m_GroupNameNumber = groupName;
			m_NameNumber = name;

			m_RequiredBeverage = BeverageType.Water;
		}

		public BeverageType RequiredBeverage { get { return m_RequiredBeverage; } set { m_RequiredBeverage = value; } }

		public void AddRes(Type type, TextDefinition name, int amount)
		{
			AddRes(type, name, amount, "");
		}

		public void AddRes(Type type, TextDefinition name, int amount, TextDefinition message)
		{
			CraftRes craftRes = new CraftRes(type, name, amount, message);
			m_arCraftRes.Add(craftRes);
		}

		public void AddSkill(SkillName skillToMake, double minSkill, double maxSkill)
		{
			CraftSkill craftSkill = new CraftSkill(skillToMake, minSkill, maxSkill);
			m_arCraftSkill.Add(craftSkill);
		}

		public int Mana { get; set; }
		public int Hits { get; set; }
		public int Stam { get; set; }
		public bool UseSubRes2 { get { return m_UseSubRes2; } set { m_UseSubRes2 = value; } }
		public bool UseAllRes { get; set; }
		public bool ForceTypeRes { get; set; }
		public bool NeedHeat { get { return m_NeedHeat; } set { m_NeedHeat = value; } }
		public bool NeedOven { get { return m_NeedOven; } set { m_NeedOven = value; } }
		public bool NeedMill { get { return m_NeedMill; } set { m_NeedMill = value; } }
		public Type ItemType { get { return m_Type; } }
		public int ItemHue { get; set; }
		public string GroupNameString { get { return m_GroupNameString; } }
		public int GroupNameNumber { get { return m_GroupNameNumber; } }
		public string NameString { get { return m_NameString; } }
		public int NameNumber { get { return m_NameNumber; } }
		public CraftResCol Resources { get { return m_arCraftRes; } }
		public CraftSkillCol Skills { get { return m_arCraftSkill; } }

		public bool ConsumeAttributes(Mobile from, ref object message, bool consume)
		{
			bool consumMana = false;
			bool consumHits = false;
			bool consumStam = false;

			if (Hits > 0 && from.Hits < Hits)
			{
				message = "You lack the required hit points to make that.";
				return false;
			}
			else
			{
				consumHits = consume;
			}

			if (Mana > 0)
			{
                if (from.Backpack != null && m_System is DefInscription)
                {
                    Item item = from.Backpack.FindItemByType(typeof(ChronicleOfTheGargoyleQueen1));

                    if (item != null && item is ChronicleOfTheGargoyleQueen1 && ((ChronicleOfTheGargoyleQueen1)item).Charges > 0)
                    {
                        if (consume)
                            ((ChronicleOfTheGargoyleQueen1)item).Charges--;
                        return true;
                    }
                }

                if (ManaPhasingOrb.IsInManaPhase(from))
                {
                    if (consume)
                        ManaPhasingOrb.RemoveFromTable(from);
                    return true;
                }

                if (from.Mana < Mana)
                {
                    message = "You lack the required mana to make that.";
                    return false;
                }
				else
				{
					consumMana = consume;
				}
			}


			if (Stam > 0 && from.Stam < Stam)
			{
				message = "You lack the required stamina to make that.";
				return false;
			}
			else
			{
				consumStam = consume;
			}

			if (consumMana)
			{
				from.Mana -= Mana;
			}

			if (consumHits)
			{
				from.Hits -= Hits;
			}

			if (consumStam)
			{
				from.Stam -= Stam;
			}

			return true;
		}

		#region Tables
		private static readonly int[] m_HeatSources = new[]
		{
			0x461, 0x48E, // Sandstone oven/fireplace
			0x92B, 0x96C, // Stone oven/fireplace
			0xDE3, 0xDE9, // Campfire
			0xFAC, 0xFAC, // Firepit
			0x184A, 0x184C, // Heating stand (left)
			0x184E, 0x1850, // Heating stand (right)
			0x398C, 0x399F, // Fire field
			0x2DDB, 0x2DDC, //Elven stove
			0x19AA, 0x19BB, // Veteran Reward Brazier
			0x197A, 0x19A9, // Large Forge 
			0x0FB1, 0x0FB1, // Small Forge
			0x2DD8, 0x2DD8 // Elven Forge
		};

		private static readonly int[] m_Ovens = new[]
		{
			0x461, 0x46F, // Sandstone oven
			0x92B, 0x93F, // Stone oven
			0x2DDB, 0x2DDC //Elven stove
		};

		private static readonly int[] m_Mills = new[]
		{
			0x1920, 0x1921, 0x1922, 0x1923, 0x1924, 0x1295, 0x1926, 0x1928, 0x192C, 0x192D, 0x192E, 0x129F, 0x1930, 0x1931,
			0x1932, 0x1934
		};

		private static readonly Type[][] m_TypesTable = new[]
		{
			new[] {typeof(Board), typeof(Log)}, 
            new[] {typeof(HeartwoodBoard), typeof(HeartwoodLog)},
			new[] {typeof(BloodwoodBoard), typeof(BloodwoodLog)}, 
            new[] {typeof(FrostwoodBoard), typeof(FrostwoodLog)},
			new[] {typeof(OakBoard), typeof(OakLog)}, 
            new[] {typeof(AshBoard), typeof(AshLog)},
			new[] {typeof(YewBoard), typeof(YewLog)}, 
            new[] {typeof(Leather), typeof(Hides)},
			new[] {typeof(SpinedLeather), typeof(SpinedHides)}, 
            new[] {typeof(HornedLeather), typeof(HornedHides)},
			new[] {typeof(BarbedLeather), typeof(BarbedHides)}, 
            new[] {typeof(BlankMap), typeof(BlankScroll)},
			new[] {typeof(Cloth), typeof(UncutCloth), typeof(AbyssalCloth)},
            new[] {typeof(CheeseWheel), typeof(CheeseWedge)},
			new[] {typeof(Pumpkin), typeof(SmallPumpkin)}, 
            new[] {typeof(WoodenBowlOfPeas), typeof(PewterBowlOfPeas)},
            new[] { typeof( CrystallineFragments ), typeof( BrokenCrystals ), typeof( ShatteredCrystals ), typeof( ScatteredCrystals ), typeof( CrushedCrystals ), typeof( JaggedCrystals ), typeof( AncientPotteryFragments ) },
            new[] { typeof( MedusaDarkScales ), typeof( MedusaLightScales ), typeof( RedScales ), typeof( BlueScales ), typeof( BlackScales ), typeof( YellowScales ), typeof( GreenScales ), typeof( WhiteScales ) }
		};

		private static readonly Type[] m_ColoredItemTable = new[]
		{
			#region Mondain's Legacy
			typeof(BaseContainer), typeof(ParrotPerchAddonDeed),
			#endregion

			typeof(BaseWeapon), typeof(BaseArmor), typeof(BaseClothing), typeof(BaseJewel), typeof(DragonBardingDeed),
			typeof(BaseAddonDeed), typeof(BaseAddon),

            #region Stygian Abyss
            typeof(PlantPigment), typeof(SoftenedReeds), typeof(DryReeds), typeof(PlantClippings),
            typeof(MedusaLightScales), typeof(MedusaDarkScales)
            #endregion
		};

        private static readonly Type[] m_ClothColoredItemTable = new[]
        {
            typeof( GozaMatSouthDeed ), typeof( GozaMatEastDeed ),
			typeof( SquareGozaMatSouthDeed ), typeof( SquareGozaMatEastDeed ),
			typeof( BrocadeGozaMatSouthDeed ), typeof( BrocadeGozaMatEastDeed ),
			typeof( BrocadeSquareGozaMatSouthDeed ), typeof( BrocadeSquareGozaMatEastDeed ),
            typeof( Tessen )
        };

		private static readonly Type[] m_ColoredResourceTable = new[]
		{
			#region Mondain's Legacy
			typeof(Board), typeof(Log),
			#endregion
			typeof(BaseIngot), typeof(BaseOre), typeof(BaseLeather), typeof(BaseHides), typeof(AbyssalCloth), typeof(UncutCloth), typeof(Cloth),
			typeof(BaseGranite), typeof(BaseScales), typeof(PlantClippings), typeof(DryReeds), typeof(SoftenedReeds),
			typeof(PlantPigment), typeof(BaseContainer)
		};

		private static readonly Type[] m_MarkableTable = new[]
		{
			#region Mondain's Legacy
			typeof(BlueDiamondRing), typeof(BrilliantAmberBracelet), typeof(DarkSapphireBracelet), typeof(EcruCitrineRing),
			typeof(FireRubyBracelet), typeof(PerfectEmeraldRing), typeof(TurqouiseRing), typeof(WhitePearlBracelet),
			typeof(BaseContainer), typeof(CraftableFurniture),
			#endregion

			typeof(BaseArmor), typeof(BaseWeapon), typeof(BaseClothing), typeof(BaseInstrument), typeof(BaseTool),
			typeof(BaseHarvestTool), typeof(BaseQuiver), typeof(DragonBardingDeed), typeof(Fukiya), typeof(FukiyaDarts),
			typeof(Shuriken), typeof(Spellbook), typeof(Runebook), typeof(ShortMusicStandLeft), typeof(ShortMusicStandRight),
            typeof(TallMusicStandLeft), typeof(TallMusicStandRight), typeof(EasleNorth), typeof(EasleEast), typeof(EasleSouth),
            typeof(RedHangingLantern), typeof(WhiteHangingLantern), typeof(BambooScreen), typeof(ShojiScreen),
			typeof(FishingPole), typeof(Stool), typeof(FootStool), typeof(WoodenBench), typeof(WoodenThrone), typeof(Throne),
			typeof(BambooChair), typeof(WoodenChair), typeof(FancyWoodenChairCushion), typeof(WoodenChairCushion),
			typeof(Nightstand), typeof(LargeTable), typeof(WritingTable), typeof(YewWoodTable), typeof(PlainLowTable),
			typeof(ElegantLowTable), typeof(DressformFront), typeof(DressformSide), typeof(BasePlayerBB), typeof(BaseContainer), typeof(BarrelStaves),
			typeof(BarrelLid), typeof(Clippers), typeof(Scissors),

            typeof(KeyRing), typeof(Key), typeof(Globe), typeof(Spyglass), typeof(Lantern), typeof(Candelabra), typeof(Scales), typeof(BroadcastCrystal), typeof(TerMurStyleCandelabra),
            typeof(BaseUtensil), typeof(BaseBeverage)
		};

		private static readonly Dictionary<Type, Type> m_ResourceConversionTable = new Dictionary<Type, Type>()
		{
			{ typeof(Board), typeof(Log) },
			{ typeof(HeartwoodBoard), typeof(HeartwoodLog) },
			{ typeof(BloodwoodBoard), typeof(BloodwoodLog) },
			{ typeof(FrostwoodBoard), typeof(FrostwoodLog) },
			{ typeof(OakBoard), typeof(OakLog) },
			{ typeof(AshBoard), typeof(AshLog) },
			{ typeof(YewBoard), typeof(YewLog) },
			{ typeof(Leather), typeof(Hides) },
			{ typeof(SpinedLeather), typeof(SpinedHides) },
			{ typeof(HornedLeather), typeof(HornedHides) },
			{ typeof(BarbedLeather), typeof(BarbedHides) },
		};

		private static Type[] m_NeverColorTable = new[] {typeof(OrcHelm)};
		#endregion

		public bool IsMarkable(Type type)
		{
			if (m_ForceNonExceptional) //Don't even display the stuff for marking if it can't ever be exceptional.
			{
				return false;
			}

			for (int i = 0; i < m_MarkableTable.Length; ++i)
			{
				if (type == m_MarkableTable[i] || type.IsSubclassOf(m_MarkableTable[i]))
				{
					return true;
				}
			}

			return false;
		}

		public static bool RetainsColor(Type type)
		{
			bool inItemTable = false;

			for (int i = 0; !inItemTable && i < m_ColoredItemTable.Length; ++i)
			{
				inItemTable = (type == m_ColoredItemTable[i] || type.IsSubclassOf(m_ColoredItemTable[i]));
			}

			return inItemTable;
		}

		public bool RetainsColorFrom(CraftSystem system, Type type)
		{
			if (system.RetainsColorFrom(this, type))
			{
				return true;
			}

			bool inItemTable = false, inResourceTable = false;

			for (int i = 0; !inItemTable && i < m_ColoredItemTable.Length; ++i)
			{
				inItemTable = (m_Type == m_ColoredItemTable[i] || m_Type.IsSubclassOf(m_ColoredItemTable[i]));
			}

			for (int i = 0; inItemTable && !inResourceTable && i < m_ColoredResourceTable.Length; ++i)
			{
				inResourceTable = (type == m_ColoredResourceTable[i] || type.IsSubclassOf(m_ColoredResourceTable[i]));
			}

			return (inItemTable && inResourceTable);
		}

        public bool RetainsColorFromCloth(Item item)
        {
            Type t = item.GetType();

            foreach (var type in m_ClothColoredItemTable)
            {
                if (type == t)
                    return true;
            }

            return false;
        }

		public bool Find(Mobile from, int[] itemIDs)
		{
			Map map = from.Map;

			if (map == null)
			{
				return false;
			}

			IPooledEnumerable eable = map.GetItemsInRange(from.Location, 2);

			foreach (Item item in eable)
			{
				if ((item.Z + 16) > from.Z && (from.Z + 16) > item.Z && Find(item.ItemID, itemIDs))
				{
					eable.Free();
					return true;
				}
			}

			eable.Free();

			for (int x = -2; x <= 2; ++x)
			{
				for (int y = -2; y <= 2; ++y)
				{
					int vx = from.X + x;
					int vy = from.Y + y;

					var tiles = map.Tiles.GetStaticTiles(vx, vy, true);

					for (int i = 0; i < tiles.Length; ++i)
					{
						int z = tiles[i].Z;
						int id = tiles[i].ID;

						if ((z + 16) > from.Z && (from.Z + 16) > z && Find(id, itemIDs))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		public bool Find(int itemID, int[] itemIDs)
		{
			bool contains = false;

			for (int i = 0; !contains && i < itemIDs.Length; i += 2)
			{
				contains = (itemID >= itemIDs[i] && itemID <= itemIDs[i + 1]);
			}

			return contains;
		}

		public bool IsQuantityType(Type[][] types)
		{
			for (int i = 0; i < types.Length; ++i)
			{
				var check = types[i];

				for (int j = 0; j < check.Length; ++j)
				{
					if (typeof(IHasQuantity).IsAssignableFrom(check[j]))
					{
						return true;
					}
				}
			}

			return false;
		}

        #region SA
        public bool IsPlantHueType(Type[][] types)
        {
            for (int i = 0; i < types.Length; ++i)
            {
                Type[] check = types[i];

                for (int j = 0; j < check.Length; ++j)
                {
                    if (typeof(IPlantHue).IsAssignableFrom(check[j]))
                        return true;
                    else if (typeof(IPigmentHue).IsAssignableFrom(check[j]))
                        return true;
                }
            }

            return false;
        }

        public int ConsumeQuantityByPlantHue(Mobile from, CraftSystem craftSystem, Container cont, Type[][] types, int[] amounts)
        {
            if (types.Length != amounts.Length)
                throw new ArgumentException();

            CraftContext context = craftSystem.GetContext(from);

            if (context == null)
                return 0;

            Item[][] items = new Item[types.Length][];
            int[] totals = new int[types.Length];

            for (int i = 0; i < types.Length; ++i)
            {
                items[i] = cont.FindItemsByType(types[i], true);

                for (int j = 0; j < items[i].Length; ++j)
                {
                    IPlantHue plantHue = items[i][j] as IPlantHue;
                    IPigmentHue pigmentHue = items[i][j] as IPigmentHue;

                    if (plantHue != null && plantHue.PlantHue != context.RequiredPlantHue)
                        continue;
                    else if (pigmentHue != null && pigmentHue.PigmentHue != context.RequiredPigmentHue)
                        continue;

                    totals[i] += items[i][j].Amount;
                }

                if (totals[i] < amounts[i])
                    return i;
            }

            for (int i = 0; i < types.Length; ++i)
            {
                int need = amounts[i];

                for (int j = 0; j < items[i].Length; ++j)
                {
                    Item item = items[i][j];
                    IPlantHue ph = item as IPlantHue;
                    IPigmentHue pigh = item as IPigmentHue;

                    int theirAmount = item.Amount;

                    if (ph != null && ph.PlantHue != context.RequiredPlantHue)
                        continue;
                    else if (pigh != null && pigh.PigmentHue != context.RequiredPigmentHue)
                        continue;

                    if (theirAmount < need)
                    {
                        OnResourceConsumed(item, theirAmount);

                        item.Delete();
                        need -= theirAmount;
                    }
                    else
                    {
                        OnResourceConsumed(item, need);

                        item.Consume(need);
                        break;
                    }
                }
            }

            return -1;
        }
        #endregion

		public int ConsumeQuantity(Container cont, Type[][] types, int[] amounts)
		{
			if (types.Length != amounts.Length)
			{
				throw new ArgumentException();
			}

			var items = new Item[types.Length][];
			var totals = new int[types.Length];

			for (int i = 0; i < types.Length; ++i)
			{
				items[i] = cont.FindItemsByType(types[i], true);

				for (int j = 0; j < items[i].Length; ++j)
				{
					IHasQuantity hq = items[i][j] as IHasQuantity;

					if (hq == null)
					{
						totals[i] += items[i][j].Amount;
					}
					else
					{
						if (hq is BaseBeverage && ((BaseBeverage)hq).Content != m_RequiredBeverage)
						{
							continue;
						}

						totals[i] += hq.Quantity;
					}
				}

				if (totals[i] < amounts[i])
				{
					return i;
				}
			}

			for (int i = 0; i < types.Length; ++i)
			{
				int need = amounts[i];

				for (int j = 0; j < items[i].Length; ++j)
				{
					Item item = items[i][j];
					IHasQuantity hq = item as IHasQuantity;

					if (hq == null)
					{
						int theirAmount = item.Amount;

						if (theirAmount < need)
						{
							item.Delete();
							need -= theirAmount;
						}
						else
						{
							item.Consume(need);
							break;
						}
					}
					else
					{
						if (hq is BaseBeverage && ((BaseBeverage)hq).Content != m_RequiredBeverage)
						{
							continue;
						}

						int theirAmount = hq.Quantity;

						if (theirAmount < need)
						{
							hq.Quantity -= theirAmount;
							need -= theirAmount;
						}
						else
						{
							hq.Quantity -= need;
							break;
						}
					}
				}
			}

			return -1;
		}

		public int GetQuantity(Container cont, Type[] types)
		{
			var items = cont.FindItemsByType(types, true);

			int amount = 0;

			for (int i = 0; i < items.Length; ++i)
			{
				IHasQuantity hq = items[i] as IHasQuantity;

				if (hq == null)
				{
					amount += items[i].Amount;
				}
				else
				{
					if (hq is BaseBeverage && ((BaseBeverage)hq).Content != m_RequiredBeverage)
					{
						continue;
					}

					amount += hq.Quantity;
				}
			}

			return amount;
		}

        #region SA
        public int GetPlantHueAmount(Mobile from, CraftSystem craftSystem, Container cont, Type[] types)
        {
            Item[] items = cont.FindItemsByType(types, true);
            CraftContext context = craftSystem.GetContext(from);

            int amount = 0;

            for (int i = 0; i < items.Length; ++i)
            {
                IPlantHue ph = items[i] as IPlantHue;
                IPigmentHue pigh = items[i] as IPigmentHue;

                if (context == null || (ph != null && ph.PlantHue != context.RequiredPlantHue))
                    continue;
                else if (context == null || (pigh != null && pigh.PigmentHue != context.RequiredPigmentHue))
                    continue;

                amount += items[i].Amount;
            }

            return amount;
        }
        #endregion

		public bool ConsumeRes(
			Mobile from,
			Type typeRes,
			CraftSystem craftSystem,
			ref int resHue,
			ref int maxAmount,
			ConsumeType consumeType,
			ref object message)
		{
			return ConsumeRes(from, typeRes, craftSystem, ref resHue, ref maxAmount, consumeType, ref message, false);
		}

		public bool ConsumeRes(
			Mobile from,
			Type typeRes,
			CraftSystem craftSystem,
			ref int resHue,
			ref int maxAmount,
			ConsumeType consumeType,
			ref object message,
			bool isFailure)
		{
			Container ourPack = from.Backpack;

			if (ourPack == null)
			{
				return false;
			}

            if (ConsumeResCallback != null)
            {
                int resMessage = ConsumeResCallback(from, consumeType);

                if (resMessage > 0)
                {
                    message = resMessage;
                    return false;
                }
            }

			if (m_NeedHeat && !Find(from, m_HeatSources))
			{
				message = 1044487; // You must be near a fire source to cook.
				return false;
			}

			if (m_NeedOven && !Find(from, m_Ovens))
			{
				message = 1044493; // You must be near an oven to bake that.
				return false;
			}

			if (m_NeedMill && !Find(from, m_Mills))
			{
				message = 1044491; // You must be near a flour mill to do that.
				return false;
			}

			var types = new Type[m_arCraftRes.Count][];
			var amounts = new int[m_arCraftRes.Count];

			maxAmount = int.MaxValue;

			CraftSubResCol resCol = (m_UseSubRes2 ? craftSystem.CraftSubRes2 : craftSystem.CraftSubRes);
            MasterCraftsmanTalisman talisman = null;

			for (int i = 0; i < types.Length; ++i)
			{
				CraftRes craftRes = m_arCraftRes.GetAt(i);
				Type baseType = craftRes.ItemType;

				if (typeRes != null && ForceTypeRes)
				{
					Type outType;
					if (m_ResourceConversionTable.TryGetValue(typeRes, out outType))
						baseType = outType;
				}

				// Resource Mutation
				if ((baseType == resCol.ResType) && (typeRes != null))
				{
					baseType = typeRes;

					CraftSubRes subResource = resCol.SearchFor(baseType);

					if (subResource != null && from.Skills[craftSystem.MainSkill].Base < subResource.RequiredSkill)
					{
						message = subResource.Message;
						return false;
					}
				}
				// ******************

				for (int j = 0; types[i] == null && j < m_TypesTable.Length; ++j)
				{
					if (m_TypesTable[j][0] == baseType)
					{
						types[i] = m_TypesTable[j];
					}
				}

				if (types[i] == null)
				{
					types[i] = new[] {baseType};
				}

				amounts[i] = craftRes.Amount;

				// For stackable items that can ben crafted more than one at a time
				if (UseAllRes)
				{
					int tempAmount = ourPack.GetAmount(types[i]);
					tempAmount /= amounts[i];
					if (tempAmount < maxAmount)
					{
						maxAmount = tempAmount;

						if (maxAmount == 0)
						{
							CraftRes res = m_arCraftRes.GetAt(i);

							if (res.MessageNumber > 0)
							{
								message = res.MessageNumber;
							}
							else if (!String.IsNullOrEmpty(res.MessageString))
							{
								message = res.MessageString;
							}
							else
							{
								message = 502925; // You don't have the resources required to make that item.
							}

							return false;
						}
					}
				}
				// ****************************

                if (isFailure && (talisman != null || !craftSystem.ConsumeOnFailure(from, types[i][0], this, ref talisman)))
				{
					amounts[i] = 0;
				}
			}

            if (talisman != null)
            {
                talisman.Charges--;
            }

			// We adjust the amount of each resource to consume the max posible
			if (UseAllRes)
			{
				for (int i = 0; i < amounts.Length; ++i)
				{
					amounts[i] *= maxAmount;
				}
			}
			else
			{
				maxAmount = -1;
			}

			Item consumeExtra = null;

			if (m_NameNumber == 1041267)
			{
				// Runebooks are a special case, they need a blank recall rune
				var runes = ourPack.FindItemsByType<RecallRune>();

				for (int i = 0; i < runes.Count; ++i)
				{
					RecallRune rune = runes[i];

					if (rune != null && !rune.Marked)
					{
						consumeExtra = rune;
						break;
					}
				}

				if (consumeExtra == null)
				{
					message = 1044253; // You don't have the components needed to make that.
					return false;
				}
			}

			int index = 0;

			// Consume ALL
			if (consumeType == ConsumeType.All)
			{
				m_ResHue = 0;
				m_ResAmount = 0;
				m_System = craftSystem;

				if (IsQuantityType(types))
				{
					index = ConsumeQuantity(ourPack, types, amounts);
				}
                else if (IsPlantHueType(types))
                {
                    index = ConsumeQuantityByPlantHue(from, craftSystem, ourPack, types, amounts);
                }
                else
                {
                    index = ourPack.ConsumeTotalGrouped(types, amounts, true, OnResourceConsumed, CheckHueGrouping);
                }

				resHue = m_ResHue;
			}
				// Consume Half ( for use all resource craft type )
			else if (consumeType == ConsumeType.Half)
			{
				for (int i = 0; i < amounts.Length; i++)
				{
					amounts[i] /= 2;

					if (amounts[i] < 1)
					{
						amounts[i] = 1;
					}
				}

				m_ResHue = 0;
				m_ResAmount = 0;
				m_System = craftSystem;

				if (IsQuantityType(types))
				{
					index = ConsumeQuantity(ourPack, types, amounts);
				}
                else if (IsPlantHueType(types))
                {
                    index = ConsumeQuantityByPlantHue(from, craftSystem, ourPack, types, amounts);
                }
                else
                {
                    index = ourPack.ConsumeTotalGrouped(types, amounts, true, OnResourceConsumed, CheckHueGrouping);
                }

				resHue = m_ResHue;
			}
			else
				// ConstumeType.None ( it's basicaly used to know if the crafter has enough resource before starting the process )
			{
				index = -1;

				if (IsQuantityType(types))
				{
					for (int i = 0; i < types.Length; i++)
					{
						if (GetQuantity(ourPack, types[i]) < amounts[i])
						{
							index = i;
							break;
						}
					}
				}
                else if (IsPlantHueType(types))
                {
                    CraftContext c = craftSystem.GetContext(from);

                    for (int i = 0; i < types.Length; i++)
                    {
                        if (GetPlantHueAmount(from, craftSystem, ourPack, types[i]) < amounts[i])
                        {
                            index = i;
                            break;
                        }
                    }
                }
				else
				{
					for (int i = 0; i < types.Length; i++)
					{
						if (ourPack.GetBestGroupAmount(types[i], true, CheckHueGrouping) < amounts[i])
						{
							index = i;
							break;
						}
					}
				}
			}

			if (index == -1)
			{
				if (consumeType != ConsumeType.None)
				{
					if (consumeExtra != null)
					{
						consumeExtra.Delete();
					}
				}

				return true;
			}
			else
			{
				CraftRes res = m_arCraftRes.GetAt(index);

				if (res.MessageNumber > 0)
				{
					message = res.MessageNumber;
				}
				else if (res.MessageString != null && res.MessageString != String.Empty)
				{
					message = res.MessageString;
				}
				else
				{
					message = 502925; // You don't have the resources required to make that item.
				}

				return false;
			}
		}

		private int m_ResHue;
        private int m_ClothHue;
		private int m_ResAmount;
		private CraftSystem m_System;

		#region Plant Pigments
		private PlantHue m_PlantHue = PlantHue.None;
		private PlantPigmentHue m_PlantPigmentHue = PlantPigmentHue.None;
		#endregion

		private void OnResourceConsumed(Item item, int amount)
		{
			#region Plant Pigments
            if (item is IPlantHue)
            {
                m_PlantHue = ((IPlantHue)item).PlantHue;
            }
            else if (item is IPigmentHue)
            {
                m_PlantPigmentHue = ((IPigmentHue)item).PigmentHue;
            }
			#endregion

            if (!RetainsColorFrom(m_System, item.GetType()))
			{
				return;
			}

            if (item is Cloth || item is UncutCloth || item is AbyssalCloth)
            {
                m_ClothHue = item.Hue;
            }

            if (amount >= m_ResAmount)
			{
				m_ResHue = item.Hue;
				m_ResAmount = amount;
			}
		}

		private int CheckHueGrouping(Item a, Item b)
		{
			return b.Hue.CompareTo(a.Hue);
		}

		public double GetExceptionalChance(CraftSystem system, double chance, Mobile from)
		{
			if (m_ForceNonExceptional)
			{
				return 0.0;
			}

			double bonus = 0.0;

			if (from.Talisman is BaseTalisman)
			{
				BaseTalisman talisman = (BaseTalisman)from.Talisman;

				if (talisman.CheckSkill(system))
				{
					bonus = talisman.ExceptionalBonus / 100.0;
				}
			}

            MasterChefsApron apron = from.FindItemOnLayer(Layer.MiddleTorso) as MasterChefsApron;

            if (apron != null)
            {
                bonus += apron.Bonus / 100.0;
            }

            if (WoodworkersBench.HasBonus(from, system.MainSkill))
            {
                bonus += .3;
            }

			switch (system.ECA)
			{
				default:
				case CraftECA.ChanceMinusSixty:
					chance -= 0.6;
					break;
				case CraftECA.FiftyPercentChanceMinusTenPercent:
					chance = chance * 0.5 - 0.1;
					break;
				case CraftECA.ChanceMinusSixtyToFourtyFive:
					{
						double offset = 0.60 - ((from.Skills[system.MainSkill].Value - 95.0) * 0.03);

						if (offset < 0.45)
						{
							offset = 0.45;
						}
						else if (offset > 0.60)
						{
							offset = 0.60;
						}

						chance -= offset;
						break;
					}
			}

			if (chance > 0)
			{
				return chance + bonus;
			}

			return chance;
		}

		public bool CheckSkills(
			Mobile from, Type typeRes, CraftSystem craftSystem, ref int quality, ref bool allRequiredSkills)
		{
			return CheckSkills(from, typeRes, craftSystem, ref quality, ref allRequiredSkills, true);
		}

		public bool CheckSkills(
			Mobile from, Type typeRes, CraftSystem craftSystem, ref int quality, ref bool allRequiredSkills, bool gainSkills)
		{
			double chance = GetSuccessChance(from, typeRes, craftSystem, gainSkills, ref allRequiredSkills);

			if (GetExceptionalChance(craftSystem, chance, from) > Utility.RandomDouble())
			{
				quality = 2;
			}

			return (chance > Utility.RandomDouble());
		}

		public double GetSuccessChance(
			Mobile from, Type typeRes, CraftSystem craftSystem, bool gainSkills, ref bool allRequiredSkills)
		{
			double minMainSkill = 0.0;
			double maxMainSkill = 0.0;
			double valMainSkill = 0.0;

			allRequiredSkills = true;

			for (int i = 0; i < m_arCraftSkill.Count; i++)
			{
				CraftSkill craftSkill = m_arCraftSkill.GetAt(i);

				double minSkill = craftSkill.MinSkill - MinSkillOffset;
				double maxSkill = craftSkill.MaxSkill;
				double valSkill = from.Skills[craftSkill.SkillToMake].Value;

				if (valSkill < minSkill)
				{
					allRequiredSkills = false;
				}

				if (craftSkill.SkillToMake == craftSystem.MainSkill)
				{
					minMainSkill = minSkill;
					maxMainSkill = maxSkill;
					valMainSkill = valSkill;
				}

				if (gainSkills) // This is a passive check. Success chance is entirely dependant on the main skill
				{
					from.CheckSkill(craftSkill.SkillToMake, minSkill, maxSkill);
				}
			}

			double chance;

			if (allRequiredSkills)
			{
				chance = craftSystem.GetChanceAtMin(this) +
						 ((valMainSkill - minMainSkill) / (maxMainSkill - minMainSkill) * (1.0 - craftSystem.GetChanceAtMin(this)));
			}
			else
			{
				chance = 0.0;
			}

			if (allRequiredSkills && from.Talisman is BaseTalisman)
			{
				BaseTalisman talisman = (BaseTalisman)from.Talisman;

				if (talisman.CheckSkill(craftSystem))
				{
					chance += talisman.SuccessBonus / 100.0;
				}
			}

            if (WoodworkersBench.HasBonus(from, craftSystem.MainSkill))
            {
                chance += .5;
            }

			if (allRequiredSkills && valMainSkill == maxMainSkill)
			{
				chance = 1.0;
			}

			return chance;
		}

        public void Craft(Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool)
		{
			if (from.BeginAction(typeof(CraftSystem)))
			{
				if (RequiredExpansion == Expansion.None ||
					(from.NetState != null && from.NetState.SupportsExpansion(RequiredExpansion)))
				{
					bool allRequiredSkills = true;
					double chance = GetSuccessChance(from, typeRes, craftSystem, false, ref allRequiredSkills);

					if (allRequiredSkills && chance >= 0.0)
					{
                        if (Recipe == null || !(from is PlayerMobile) || ((PlayerMobile)from).HasRecipe(Recipe))
                        {
                            if (!RequiresBasketWeaving || (from is PlayerMobile && ((PlayerMobile)from).BasketWeaving))
                            {
                                if (!RequiresMechanicalLife || (from is PlayerMobile && ((PlayerMobile)from).MechanicalLife))
                                {
                                    int badCraft = craftSystem.CanCraft(from, tool, m_Type);

                                    if (badCraft <= 0)
                                    {
                                        if (RequiresResTarget && NeedsResTarget(from, craftSystem))
                                        {
                                            from.Target = new ChooseResTarget(from, this, craftSystem, typeRes, tool);
                                            from.SendMessage("Choose the resource you would like to use.");
                                            return;
                                        }

                                        int resHue = 0;
                                        int maxAmount = 0;
                                        object message = null;

                                        if (ConsumeRes(from, typeRes, craftSystem, ref resHue, ref maxAmount, ConsumeType.None, ref message))
                                        {
                                            message = null;

                                            if (ConsumeAttributes(from, ref message, false))
                                            {
                                                CraftContext context = craftSystem.GetContext(from);

                                                if (context != null)
                                                {
                                                    context.OnMade(this);
                                                }

                                                int iMin = craftSystem.MinCraftEffect;
                                                int iMax = (craftSystem.MaxCraftEffect - iMin) + 1;
                                                int iRandom = Utility.Random(iMax);
                                                iRandom += iMin + 1;
                                                new InternalTimer(from, craftSystem, this, typeRes, tool, iRandom).Start();
                                                return;
                                            }
                                            else
                                            {
                                                from.EndAction(typeof(CraftSystem));
                                                from.SendGump(new CraftGump(from, craftSystem, tool, message));
                                            }
                                        }
                                        else
                                        {
                                            from.EndAction(typeof(CraftSystem));
                                            from.SendGump(new CraftGump(from, craftSystem, tool, message));
                                        }
                                    }
                                    else
                                    {
                                        from.EndAction(typeof(CraftSystem));
                                        from.SendGump(new CraftGump(from, craftSystem, tool, badCraft));
                                    }
                                }
                                else
                                {
                                    from.EndAction(typeof(CraftSystem));
                                    from.SendGump(new CraftGump(from, craftSystem, tool, 1113034)); // You haven't read the Mechanical Life Manual. Talking to Sutek might help!
                                }
                            }
                            else
                            {
                                from.EndAction(typeof(CraftSystem));
                                from.SendGump(new CraftGump(from, craftSystem, tool, 1112253)); // You haven't learned basket weaving. Perhaps studying a book would help!
                            }
                        }
                        else
                        {
                            from.EndAction(typeof(CraftSystem));
                            from.SendGump(new CraftGump(from, craftSystem, tool, 1072847)); // You must learn that recipe from a scroll.
                        }
					}
					else
					{
						from.EndAction(typeof(CraftSystem));
						from.SendGump(new CraftGump(from, craftSystem, tool, 1044153));
						// You don't have the required skills to attempt this item.
					}
				}
				else
				{
					from.EndAction(typeof(CraftSystem));
					from.SendGump(new CraftGump(from, craftSystem, tool, RequiredExpansionMessage(RequiredExpansion)));
						//The {0} expansion is required to attempt this item.
				}
			}
			else
			{
				from.SendLocalizedMessage(500119); // You must wait to perform another action
			}

            AutoCraftTimer.EndTimer(from);
		}

		private object RequiredExpansionMessage(Expansion expansion)
		{
			switch (expansion)
			{
                case Expansion.SE:
                    return 1063307; // The "Samurai Empire" expansion is required to attempt this item.
                case Expansion.ML:
                    return 1072650; // The "Mondain's Legacy" expansion is required to attempt this item.
                case Expansion.SA:
                    return 1094731; // You must have the Stygian Abyss expansion pack to use this feature.
                case Expansion.HS:
                    return 1116295; // You must have the High Seas booster pack to use this feature
                case Expansion.TOL:
                    return 1155875; // You must have the Time of Legends expansion to use this feature.
                default:
                    return String.Format("The \"{0}\" expansion is required to attempt this item.", ExpansionInfo.GetInfo(expansion).Name);
			}
		}

        public void CompleteCraft(
			int quality,
			bool makersMark,
			Mobile from,
			CraftSystem craftSystem,
			Type typeRes,
			ITool tool,
			CustomCraft customCraft)
		{
			int badCraft = craftSystem.CanCraft(from, tool, m_Type);

			if (badCraft > 0)
			{
				if (tool != null && !tool.Deleted && tool.UsesRemaining > 0)
				{
					from.SendGump(new CraftGump(from, craftSystem, tool, badCraft));
				}
				else
				{
					from.SendLocalizedMessage(badCraft);
				}

                AutoCraftTimer.EndTimer(from);

				return;
			}

			int checkResHue = 0, checkMaxAmount = 0;
			object checkMessage = null;

			// Not enough resource to craft it
			if (!ConsumeRes(from, typeRes, craftSystem, ref checkResHue, ref checkMaxAmount, ConsumeType.None, ref checkMessage))
			{
				if (tool != null && !tool.Deleted && tool.UsesRemaining > 0)
				{
					from.SendGump(new CraftGump(from, craftSystem, tool, checkMessage));
				}
				else if (checkMessage is int && (int)checkMessage > 0)
				{
					from.SendLocalizedMessage((int)checkMessage);
				}
				else if (checkMessage is string)
				{
					from.SendMessage((string)checkMessage);
				}

                AutoCraftTimer.EndTimer(from);

				return;
			}
			else if (!ConsumeAttributes(from, ref checkMessage, false))
			{
				if (tool != null && !tool.Deleted && tool.UsesRemaining > 0)
				{
					from.SendGump(new CraftGump(from, craftSystem, tool, checkMessage));
				}
				else if (checkMessage is int && (int)checkMessage > 0)
				{
					from.SendLocalizedMessage((int)checkMessage);
				}
				else if (checkMessage is string)
				{
					from.SendMessage((string)checkMessage);
				}

                AutoCraftTimer.EndTimer(from);

				return;
			}

			bool toolBroken = false;

			int ignored = 1;
			int endquality = 1;

			bool allRequiredSkills = true;

			if (CheckSkills(from, typeRes, craftSystem, ref ignored, ref allRequiredSkills))
			{
				// Resource
				int resHue = 0;
				int maxAmount = 0;

				object message = null;

				// Not enough resource to craft it
				if (!ConsumeRes(from, typeRes, craftSystem, ref resHue, ref maxAmount, ConsumeType.All, ref message))
				{
					if (tool != null && !tool.Deleted && tool.UsesRemaining > 0)
					{
						from.SendGump(new CraftGump(from, craftSystem, tool, message));
					}
					else if (message is int && (int)message > 0)
					{
						from.SendLocalizedMessage((int)message);
					}
					else if (message is string)
					{
						from.SendMessage((string)message);
					}

                    AutoCraftTimer.EndTimer(from);

					return;
				}
				else if (!ConsumeAttributes(from, ref message, true))
				{
					if (tool != null && !tool.Deleted && tool.UsesRemaining > 0)
					{
						from.SendGump(new CraftGump(from, craftSystem, tool, message));
					}
					else if (message is int && (int)message > 0)
					{
						from.SendLocalizedMessage((int)message);
					}
					else if (message is string)
					{
						from.SendMessage((string)message);
					}

                    AutoCraftTimer.EndTimer(from);

					return;
				}

				if (craftSystem is DefBlacksmithy)
				{
					AncientSmithyHammer hammer = from.FindItemOnLayer(Layer.OneHanded) as AncientSmithyHammer;
					if (hammer != null && hammer != tool)
					{
						#region Mondain's Legacy
						if (hammer is HammerOfHephaestus)
						{
							if (hammer.UsesRemaining > 0)
							{
								hammer.UsesRemaining--;
							}

							if (hammer.UsesRemaining < 1)
							{
								from.PlaceInBackpack(hammer);
							}
						}
						else
						{
							hammer.UsesRemaining--;

							if (hammer.UsesRemaining < 1)
							{
								hammer.Delete();
							}
						}
						#endregion
					}
				}

				int num = 0;

				Item item;
				if (customCraft != null)
				{
					item = customCraft.CompleteCraft(out num);
				}
				else if (!Core.SA && typeof(MapItem).IsAssignableFrom(ItemType) && from.Map != Map.Trammel && from.Map != Map.Felucca)
				{
					item = new IndecipherableMap();
					from.SendLocalizedMessage(1070800); // The map you create becomes mysteriously indecipherable.
				}
                else if (CreateItem != null)
                {
                    item = CreateItem(from, this, tool);
                }
                else
                {
                    item = Activator.CreateInstance(ItemType) as Item;
                }

				if (item != null)
				{
					#region Mondain's Legacy
					if (item is Board)
					{
						Type resourceType = typeRes;

						if (resourceType == null)
						{
							resourceType = Resources.GetAt(0).ItemType;
						}

						CraftResource thisResource = CraftResources.GetFromType(resourceType);

						switch (thisResource)
						{
							case CraftResource.OakWood:
								item = new OakBoard();
								break;
							case CraftResource.AshWood:
								item = new AshBoard();
								break;
							case CraftResource.YewWood:
								item = new YewBoard();
								break;
							case CraftResource.Heartwood:
								item = new HeartwoodBoard();
								break;
							case CraftResource.Bloodwood:
								item = new BloodwoodBoard();
								break;
							case CraftResource.Frostwood:
								item = new FrostwoodBoard();
								break;
							default:
								item = new Board();
								break;
						}
					}
					#endregion

                    #region High Seas
                    if (Core.HS && item is MapItem)
                        ((MapItem)item).Facet = from.Map;
                    #endregion

                    CraftContext context = craftSystem.GetContext(from);
                    int originalHue = item.Hue;

					if (item is ICraftable)
					{
						endquality = ((ICraftable)item).OnCraft(quality, makersMark, from, craftSystem, typeRes, tool, this, resHue);
					}
					else if (item is Food)
					{
						((Food)item).PlayerConstructed = true;
					}
					else if (item.Hue == 0)
					{
						item.Hue = resHue;
					}

                    if (item.Hue == 0 && RetainsColorFromCloth(item) && m_ClothHue != 0)
                    {
                        item.Hue = m_ClothHue;
                    }

                    // This takes into account for natural hues, ie plant hues
                    if (item.Hue != originalHue && context.DoNotColor)
                    {
                        item.Hue = originalHue;
                    }

					if (maxAmount > 0)
					{
						if (!item.Stackable && item is IUsesRemaining)
						{
							((IUsesRemaining)item).UsesRemaining *= maxAmount;
						}
						else
						{
							item.Amount = maxAmount;
						}
					}

					#region Plant Pigments
                    if (m_PlantHue != PlantHue.None)
                    {
                        if (item is IPlantHue)
                            ((IPlantHue)item).PlantHue = m_PlantHue;
                        else if (item is IPigmentHue)
                            ((IPigmentHue)item).PigmentHue = PlantPigmentHueInfo.HueFromPlantHue(m_PlantHue);
                    }
                    else if (m_PlantPigmentHue != PlantPigmentHue.None && item is IPigmentHue)
                    {
                        ((IPigmentHue)item).PigmentHue = m_PlantPigmentHue;
                    }

                    if (context.QuestOption == CraftQuestOption.QuestItem)
                    {
                        PlayerMobile px = from as PlayerMobile;

                        if (!QuestHelper.CheckItem(px, item))
                            from.SendLocalizedMessage(1072355, null, 0x23); // That item does not match any of your quest criteria	
                    }

                    context.RequiredPigmentHue = PlantPigmentHue.None;
                    context.RequiredPlantHue = PlantHue.None;

                    m_PlantHue = PlantHue.None;
                    m_PlantPigmentHue = PlantPigmentHue.None;
					#endregion

                    if (tool is Item && ((Item)tool).Parent is Container)
                    {
                        Container cntnr = (Container)((Item)tool).Parent;

                        if (!cntnr.TryDropItem(from, item, false))
                        {
                            if(cntnr != from.Backpack)
                                from.AddToBackpack(item);
                            else
                                item.MoveToWorld(from.Location, from.Map);
                        }
                    }
                    else
                    {
                        from.AddToBackpack(item);
                    }

                    if (tool is Item) // sanity check
                    {
                        EventSink.InvokeCraftSuccess(new CraftSuccessEventArgs(from, item, (Item)tool));
                    }

					if (from.IsStaff())
					{
						CommandLogging.WriteLine(
							from, "Crafting {0} with craft system {1}", CommandLogging.Format(item), craftSystem.GetType().Name);
					}

                    AutoCraftTimer.OnSuccessfulCraft(from);
					//from.PlaySound( 0x57 );
				}

                tool.UsesRemaining--;

                #region Mondain's Legacy
                if (tool is HammerOfHephaestus)
                {
                    if (tool.UsesRemaining < 1)
                    {
                        tool.UsesRemaining = 0;
                    }
                }
                #endregion
                else
                {
                    if (tool.UsesRemaining < 1 && tool.BreakOnDepletion)
                    {
                        toolBroken = true;
                    }

                    if (toolBroken)
                    {
                        tool.Delete();
                    }
                }

				if (num == 0)
				{
					num = craftSystem.PlayEndingEffect(from, false, true, toolBroken, endquality, makersMark, this);
				}

				bool queryFactionImbue = false;
				int availableSilver = 0;
				FactionItemDefinition def = null;
				Faction faction = null;

				if (item is IFactionItem)
				{
					def = FactionItemDefinition.Identify(item);

					if (def != null)
					{
						faction = Faction.Find(from);

						if (faction != null)
						{
							Town town = Town.FromRegion(from.Region);

							if (town != null && town.Owner == faction)
							{
								Container pack = from.Backpack;

								if (pack != null)
								{
									availableSilver = pack.GetAmount(typeof(Silver));

									if (availableSilver >= def.SilverCost)
									{
										queryFactionImbue = Faction.IsNearType(from, def.VendorType, 12);
									}
								}
							}
						}
					}
				}

				// TODO: Scroll imbuing

				if (queryFactionImbue)
				{
					from.SendGump(new FactionImbueGump(quality, item, from, craftSystem, tool, num, availableSilver, faction, def));
				}
				else if (tool != null && !tool.Deleted && tool.UsesRemaining > 0)
				{
					from.SendGump(new CraftGump(from, craftSystem, tool, num));
				}
				else if (num > 0)
				{
					from.SendLocalizedMessage(num);
				}
			}
			else if (!allRequiredSkills)
			{
				if (tool != null && !tool.Deleted && tool.UsesRemaining > 0)
				{
					from.SendGump(new CraftGump(from, craftSystem, tool, 1044153));
				}
				else
				{
					from.SendLocalizedMessage(1044153); // You don't have the required skills to attempt this item.
				}

                AutoCraftTimer.EndTimer(from);
			}
			else
			{
				ConsumeType consumeType = (UseAllRes ? ConsumeType.Half : ConsumeType.All);
				int resHue = 0;
				int maxAmount = 0;

				object message = null;

				// Not enough resource to craft it
				if (!ConsumeRes(from, typeRes, craftSystem, ref resHue, ref maxAmount, consumeType, ref message, true))
				{
					if (tool != null && !tool.Deleted && tool.UsesRemaining > 0)
					{
						from.SendGump(new CraftGump(from, craftSystem, tool, message));
					}
					else if (message is int && (int)message > 0)
					{
						from.SendLocalizedMessage((int)message);
					}
					else if (message is string)
					{
						from.SendMessage((string)message);
					}

                    AutoCraftTimer.EndTimer(from);

					return;
				}

				tool.UsesRemaining--;

                if (tool.UsesRemaining < 1 && tool.BreakOnDepletion)
				{
					toolBroken = true;
				}

				if (toolBroken)
				{
					tool.Delete();
				}

				// SkillCheck failed.
				int num = craftSystem.PlayEndingEffect(from, true, true, toolBroken, endquality, false, this);

				if (tool != null && !tool.Deleted && tool.UsesRemaining > 0)
				{
					from.SendGump(new CraftGump(from, craftSystem, tool, num));
				}
				else if (num > 0)
				{
					from.SendLocalizedMessage(num);
				}
			}
		}

		private class InternalTimer : Timer
		{
			private readonly Mobile m_From;
			private int m_iCount;
			private readonly int m_iCountMax;
			private readonly CraftItem m_CraftItem;
			private readonly CraftSystem m_CraftSystem;
			private readonly Type m_TypeRes;
			private readonly ITool m_Tool;
            private bool m_AutoCraft;

			public InternalTimer(
				Mobile from, CraftSystem craftSystem, CraftItem craftItem, Type typeRes, ITool tool, int iCountMax)
				: base(TimeSpan.Zero, TimeSpan.FromSeconds(craftSystem.Delay), iCountMax)
			{
				m_From = from;
				m_CraftItem = craftItem;
				m_iCount = 0;
				m_iCountMax = iCountMax;
				m_CraftSystem = craftSystem;
				m_TypeRes = typeRes;
				m_Tool = tool;
                m_AutoCraft = AutoCraftTimer.HasTimer(from);
			}

			protected override void OnTick()
			{
				m_iCount++;

				m_From.DisruptiveAction();

				if (m_iCount < m_iCountMax)
				{
					m_CraftSystem.PlayCraftEffect(m_From);
				}
				else
				{
					m_From.EndAction(typeof(CraftSystem));

					int badCraft = m_CraftSystem.CanCraft(m_From, m_Tool, m_CraftItem.m_Type);

					if (badCraft > 0)
					{
						if (m_Tool != null && !m_Tool.Deleted && m_Tool.UsesRemaining > 0)
						{
							m_From.SendGump(new CraftGump(m_From, m_CraftSystem, m_Tool, badCraft));
						}
						else
						{
							m_From.SendLocalizedMessage(badCraft);
						}

                        AutoCraftTimer.EndTimer(m_From);

						return;
					}

					int quality = 1;
					bool allRequiredSkills = true;

					m_CraftItem.CheckSkills(m_From, m_TypeRes, m_CraftSystem, ref quality, ref allRequiredSkills, false);

					CraftContext context = m_CraftSystem.GetContext(m_From);

					if (context == null)
					{
						return;
					}

					if (typeof(CustomCraft).IsAssignableFrom(m_CraftItem.ItemType))
					{
						CustomCraft cc = null;

						try
						{
							cc =
								Activator.CreateInstance(
									m_CraftItem.ItemType, new object[] {m_From, m_CraftItem, m_CraftSystem, m_TypeRes, m_Tool, quality}) as
								CustomCraft;
						}
						catch
						{ }

						if (cc != null)
						{
							cc.EndCraftAction();
						}

						return;
					}

					bool makersMark = false;

					if (quality == 2 && m_From.Skills[m_CraftSystem.MainSkill].Base >= 100.0)
					{
						makersMark = m_CraftItem.IsMarkable(m_CraftItem.ItemType);
					}

                    if (makersMark && context.MarkOption == CraftMarkOption.PromptForMark && !m_AutoCraft)
					{
						m_From.SendGump(new QueryMakersMarkGump(quality, m_From, m_CraftItem, m_CraftSystem, m_TypeRes, m_Tool));
					}
					else
					{
						if (context.MarkOption == CraftMarkOption.DoNotMark)
						{
							makersMark = false;
						}

						m_CraftItem.CompleteCraft(quality, makersMark, m_From, m_CraftSystem, m_TypeRes, m_Tool, null);
					}
				}
			}
		}

        #region SA
        public static void RemoveResTarget(Mobile from)
        {
            if (m_HasTarget.Contains(from))
                m_HasTarget.Remove(from);
        }

        public static void AddResTarget(Mobile from)
        {
            if (!m_HasTarget.Contains(from))
                m_HasTarget.Add(from);
        }

        public static bool HasResTarget(Mobile from)
        {
            return m_HasTarget.Contains(from);
        }

        private static List<Mobile> m_HasTarget = new List<Mobile>();

        public bool NeedsResTarget(Mobile from, CraftSystem craftSystem)
        {
            CraftContext context = craftSystem.GetContext(from);

            if (context == null || HasResTarget(from))
                return false;

            Type[][] types = new Type[m_arCraftRes.Count][];
            Container pack = from.Backpack;
            PlantHue hue = PlantHue.None;
            PlantPigmentHue phue = PlantPigmentHue.None;

            for (int i = 0; i < types.Length; ++i)
            {
                CraftRes craftRes = m_arCraftRes.GetAt(i);
                Type type = craftRes.ItemType;

                if (pack != null)
                {
                    Item[] items = pack.FindItemsByType(type);

                    if (items != null && items.Length > 0 && items[0] is IPlantHue)
                        hue = ((IPlantHue)items[0]).PlantHue;
                    else if (items != null && items.Length > 0 && items[0] is IPigmentHue)
                        phue = ((IPigmentHue)items[0]).PigmentHue;

                    foreach (Item item in items)
                    {
                        if (item is IPlantHue && ((IPlantHue)item).PlantHue != hue)
                            return true;
                        else if (item is IPigmentHue && ((IPigmentHue)item).PigmentHue != phue)
                            return true;
                    }

                    if (hue != PlantHue.None)
                        context.RequiredPlantHue = hue;
                    else if (phue != PlantPigmentHue.None)
                        context.RequiredPigmentHue = phue;
                    
                }
            }

            return false;
        }

        public class ChooseResTarget : Server.Targeting.Target
        {
            private CraftItem m_CraftItem;
            private CraftSystem m_CraftSystem;
            private Type m_TypeRes;
            private ITool m_Tool;

            public ChooseResTarget(Mobile from, CraftItem craftitem, CraftSystem craftSystem, Type typeRes, ITool tool)
                : base(-1, false, Server.Targeting.TargetFlags.None)
            {
                m_CraftItem = craftitem;
                m_CraftSystem = craftSystem;
                m_TypeRes = typeRes;
                m_Tool = tool;

                CraftItem.AddResTarget(from);
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                CraftContext context = m_CraftSystem.GetContext(from);

                if (context != null && targeted is IPlantHue)
                    context.RequiredPlantHue = ((IPlantHue)targeted).PlantHue;
                else if (context != null && targeted is IPigmentHue)
                    context.RequiredPigmentHue = ((IPigmentHue)targeted).PigmentHue;

                from.EndAction(typeof(CraftSystem));
                m_CraftItem.Craft(from, m_CraftSystem, m_TypeRes, m_Tool);
            }

            protected override void OnTargetCancel(Mobile from, Server.Targeting.TargetCancelType cancelType)
            {
                from.EndAction(typeof(CraftSystem));
                from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, null));
            }

            protected override void OnTargetFinish(Mobile from)
            {
                CraftItem.RemoveResTarget(from);
            }
        }
        #endregion
	}
}
