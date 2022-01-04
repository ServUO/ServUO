using System.Collections.Generic;

namespace Server.Services.ContainerSort
{
	public class SortItemGroups
	{
		public static Dictionary<ItemSortType, List<ItemSortType>> Groups = new Dictionary<ItemSortType, List<ItemSortType>>
		{
			{ 
				ItemSortType.AllOre, 
				new List<ItemSortType> { ItemSortType.IronOre, ItemSortType.DullCopperOre, ItemSortType.ShadowIronOre,
										 ItemSortType.CopperOre, ItemSortType.BronzeOre, ItemSortType.GoldOre,
										 ItemSortType.AgapiteOre, ItemSortType.VeriteOre, ItemSortType.ValoriteOre
				                       } 
			},
			{
				ItemSortType.AllIgnots,
				new List<ItemSortType> { ItemSortType.IronIngot, ItemSortType.DullCopperIngot, ItemSortType.ShadowIronIngot,
										 ItemSortType.CopperIngot, ItemSortType.BronzeIngot, ItemSortType.GoldIngot,
										 ItemSortType.AgapiteIngot, ItemSortType.VeriteIngot, ItemSortType.ValoriteIngot 
				                       }
			},
			{
				ItemSortType.AllGems,
				new List<ItemSortType> { ItemSortType.Amber, ItemSortType.Citrine, ItemSortType.Ruby,
										 ItemSortType.Tourmaline, ItemSortType.Amethyst, ItemSortType.Emerald,
										 ItemSortType.Sapphire, ItemSortType.StarSapphire, ItemSortType.Diamond,
										 ItemSortType.PrismaticAmber, ItemSortType.BrilliantAmber, ItemSortType.BlueDiamond,
										 ItemSortType.DarkSapphire, ItemSortType.EcruCitrine, ItemSortType.FireRuby,
										 ItemSortType.PerfectEmerald, ItemSortType.Turquoise, ItemSortType.WhitePearl,
										 ItemSortType.ArcaneGem
				                       }
			},
			{
				ItemSortType.AllMageryReagents,
				new List<ItemSortType> { ItemSortType.BlackPearl, ItemSortType.BloodMoss, ItemSortType.Garlic,
                                         ItemSortType.Ginseng, ItemSortType.MandrakeRoot, ItemSortType.Nightshade,
					                     ItemSortType.SpidersSilk, ItemSortType.SulfurousAsh
									   }
			},
			{
				ItemSortType.AllNecromancyReagents,
				new List<ItemSortType> { ItemSortType.GraveDust, ItemSortType.NoxCrystal, ItemSortType.DaemonBlood,
		                                 ItemSortType.Batwing, ItemSortType.PigIron
									   }
			},
			{
				ItemSortType.AllMysticismReagents,
				new List<ItemSortType> { ItemSortType.Bone, ItemSortType.DragonsBlood, ItemSortType.DaemonBone,
                                         ItemSortType.FertileDirt
									   }
			},
			{
				ItemSortType.AllWeapons,
				new List<ItemSortType> { ItemSortType.WeaponSkillSwords, ItemSortType.WeaponSkillMaces, ItemSortType.WeaponSkillFencing,
										 ItemSortType.WeaponSkillRanged, ItemSortType.WeaponSkillThrowing
									   }
			},
			{
				ItemSortType.AllArmorMaterials,
				new List<ItemSortType> { ItemSortType.ClothArmor, ItemSortType.LeatherArmor, ItemSortType.StuddedArmor,
										 ItemSortType.BoneArmor, ItemSortType.SpinedArmor, ItemSortType.HornedArmor,
										 ItemSortType.BarbedArmor, ItemSortType.RingmailArmor, ItemSortType.ChainmailArmor,
										 ItemSortType.PlateArmor, ItemSortType.DragonArmor, ItemSortType.WoodArmor,
										 ItemSortType.StoneArmor
									   }
			},
			{
				ItemSortType.AllArmorSlots,
				new List<ItemSortType> { ItemSortType.EaringArmorPosition, ItemSortType.FingerArmorPosition, ItemSortType.WristArmorPosition,
										 ItemSortType.NeckArmorPosition, ItemSortType.HeadArmorPosition, ItemSortType.RobeArmorPosition,
										 ItemSortType.ChestArmorPosition, ItemSortType.ArmsArmorPosition, ItemSortType.HandsArmorPosition,
										 ItemSortType.LegsArmorPosition, ItemSortType.FeetArmorPosition, ItemSortType.BackArmorPosition,
										 ItemSortType.ShieldArmorPosition
									   }
			},
			{
				ItemSortType.AllWoodTypes,
				new List<ItemSortType> { ItemSortType.RegularWood, ItemSortType.OakWood, ItemSortType.AshWood,
										 ItemSortType.YewWood, ItemSortType.Heartwood, ItemSortType.Bloodwood,
										 ItemSortType.Frostwood
									   }
			},
			{
				ItemSortType.AllBoardTypes,
				new List<ItemSortType> { ItemSortType.RegularBoard, ItemSortType.OakBoard, ItemSortType.AshBoard, 
										 ItemSortType.YewBoard, ItemSortType.HeartwoodBoard, ItemSortType.BloodwoodBoard, 
										 ItemSortType.FrostwoodBoard,
									   }
			}
		};
	}
}