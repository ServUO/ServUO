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

using Server;
using Server.Engines.Craft;
using Server.Items;

using VitaNex.Items;
#endregion

namespace VitaNex.SuperCrafts
{
	public sealed class Pyrotechnics : SuperCraftSystem
	{
		public static Type PowderType = ScriptCompiler.FindTypeByName("BlackPowder") ?? typeof(SulfurousAsh);

		public override SkillName MainSkill => SkillName.Alchemy;

		public override TextDefinition GumpTitle => "<BASEFONT COLOR=#FFFFFF><CENTER>PYROTECHNICS MENU</CENTER>";

		public Pyrotechnics()
			: base(0, 0, 3)
		{
			MarkOption = true;
		}

        //#if ServUO
        //#else
        //		public override int CanCraft(Mobile m, BaseTool tool, Type itemType)
        //#endif
        public override int CanCraft(Mobile m, ITool tool, Type itemType)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
			{
				return 1044038; // You have worn out your tool!
			}

			if (tool is Item o && !BaseTool.CheckAccessible(o, m))
			{
				return 1044263; // The tool must be on your person to use.
			}

			return base.CanCraft(m, tool, itemType);
		}

		public override bool RetainsColorFrom(CraftItem item, Type type)
		{
			if (item != null && type != null)
			{
				if (item.ItemType.IsEqualOrChildOf<BaseFirework>() && type.IsEqualOrChildOf<BaseFireworkStar>())
				{
					return true;
				}

				if (item.ItemType.IsEqualOrChildOf<BaseFireworkStar>() && type.IsEqualOrChildOf<BaseIngot>())
				{
					return true;
				}
			}

			return false;
		}

		public override void InitCraftList()
		{
			InitComponents();
			InitRockets();

			SetSubRes(typeof(IronIngot), 1044022);

			AddSubRes(typeof(IronIngot), 1044022, 00.0, 1044036, 1044267);
			AddSubRes(typeof(DullCopperIngot), 1044023, 65.0, 1044036, 1044268);
			AddSubRes(typeof(ShadowIronIngot), 1044024, 70.0, 1044036, 1044268);
			AddSubRes(typeof(CopperIngot), 1044025, 75.0, 1044036, 1044268);
			AddSubRes(typeof(BronzeIngot), 1044026, 80.0, 1044036, 1044268);
			AddSubRes(typeof(GoldIngot), 1044027, 85.0, 1044036, 1044268);
			AddSubRes(typeof(AgapiteIngot), 1044028, 90.0, 1044036, 1044268);
			AddSubRes(typeof(VeriteIngot), 1044029, 95.0, 1044036, 1044268);
			AddSubRes(typeof(ValoriteIngot), 1044030, 97.0, 1044036, 1044268);

			var type = ScriptCompiler.FindTypeByName("MithrilIngot");

			if (type != null && type.IsChildOf<BaseIngot>())
			{
				AddSubRes(type, "MITHRIL", 99.0, 1044268);
			}

			SetSubRes2(typeof(FireworkStarIron), "IRON STARS");

			AddSubRes2(typeof(FireworkStarIron), "IRON STARS", 00.0, "You cannot work with this unusual star.");
			AddSubRes2(typeof(FireworkStarDullCopper), "DULL COPPER STARS", 65.0, "You cannot work with this unusual star.");
			AddSubRes2(typeof(FireworkStarShadowIron), "SHADOW IRON STARS", 70.0, "You cannot work with this unusual star.");
			AddSubRes2(typeof(FireworkStarCopper), "COPPER STARS", 75.0, "You cannot work with this unusual star.");
			AddSubRes2(typeof(FireworkStarBronze), "BRONZE STARS", 80.0, "You cannot work with this unusual star.");
			AddSubRes2(typeof(FireworkStarGold), "GOLD STARS", 85.0, "You cannot work with this unusual star.");
			AddSubRes2(typeof(FireworkStarAgapite), "AGAPITE STARS", 90.0, "You cannot work with this unusual star.");
			AddSubRes2(typeof(FireworkStarVerite), "VERITE STARS", 95.0, "You cannot work with this unusual star.");
			AddSubRes2(typeof(FireworkStarValorite), "VALORITE STARS", 99.0, "You cannot work with this unusual star.");
		}

		private void InitComponents()
		{
			AddCraft<FireworkStarCustom>(
				"Components",
				"Firework Star",
				0.0,
				100.0,
				new[]
				{
					new ResourceInfo(typeof(IronIngot), 1044036, 2), new ResourceInfo(PowderType, PowderType.Name.SpaceWords(), 1),
					new ResourceInfo(typeof(BlackPearl), "Black Pearl", 1)
				},
				c => c.NeedHeat = true);

			AddCraft<FireworkFuse>(
				"Components",
				"Firework Fuse",
				0.0,
				100.0,
				new[] { new ResourceInfo(typeof(Wool), "Wool", 2), new ResourceInfo(PowderType, PowderType.Name.SpaceWords(), 1) });
		}

		private void InitRockets()
		{
			AddCraft<BottleRocket>(
				"Rockets",
				"Bottle Rocket",
				0.0,
				20.0,
				new[]
				{
					new ResourceInfo(typeof(FireworkStarIron), "Firework Star", 1),
					new ResourceInfo(typeof(FireworkFuse), "Firework Fuse", 1)
				},
				c => c.UseSubRes2 = true);

			AddCraft<SkyShieldRocket>(
				"Rockets",
				"Sky Shield",
				10.0,
				30.0,
				new[]
				{
					new ResourceInfo(typeof(FireworkStarIron), "Firework Star", 2),
					new ResourceInfo(typeof(FireworkFuse), "Firework Fuse", 1)
				},
				c => c.UseSubRes2 = true);

			AddCraft<LittleBoyRocket>(
				"Rockets",
				"Little Boy",
				20.0,
				40.0,
				new[]
				{
					new ResourceInfo(typeof(FireworkStarIron), "Firework Star", 3),
					new ResourceInfo(typeof(FireworkFuse), "Firework Fuse", 2)
				},
				c => c.UseSubRes2 = true);

			AddCraft<MoonShineRocket>(
				"Rockets",
				"Moon Shine",
				40.0,
				60.0,
				new[]
				{
					new ResourceInfo(typeof(FireworkStarIron), "Firework Star", 5),
					new ResourceInfo(typeof(FireworkFuse), "Firework Fuse", 2)
				},
				c => c.UseSubRes2 = true);

			AddCraft<PenetratorRocket>(
				"Rockets",
				"The Penetrator",
				60.0,
				80.0,
				new[]
				{
					new ResourceInfo(typeof(FireworkStarIron), "Firework Star", 7),
					new ResourceInfo(typeof(FireworkFuse), "Firework Fuse", 3)
				},
				c => c.UseSubRes2 = true);

			AddCraft<BigBettyRocket>(
				"Rockets",
				"Big Betty",
				80.0,
				100.0,
				new[]
				{
					new ResourceInfo(typeof(FireworkStarIron), "Firework Star", 9),
					new ResourceInfo(typeof(FireworkFuse), "Firework Fuse", 3)
				},
				c => c.UseSubRes2 = true);

			AddCraft<BlockBusterRocket>(
				"Rockets",
				"Block Buster",
				100.0,
				120.0,
				new[]
				{
					new ResourceInfo(typeof(FireworkStarIron), "Firework Star", 11),
					new ResourceInfo(typeof(FireworkFuse), "Firework Fuse", 4)
				},
				c => c.UseSubRes2 = true);
		}

		public override double GetChanceAtMin(CraftItem item)
		{
			return 0.10;
		}

		public override void PlayCraftEffect(Mobile m)
		{
			if (m != null)
			{
				m.PlaySound(85);
			}
		}

		public override int PlayEndingEffect(
			Mobile m,
			bool failed,
			bool lostMaterial,
			bool toolBroken,
			int quality,
			bool makersMark,
			CraftItem item)
		{
			if (toolBroken)
			{
				m.SendLocalizedMessage(1044038); // You have worn out your tool
			}

			if (failed)
			{
				if (lostMaterial)
				{
					return 1044043; // You failed to create the item, and some of your materials are lost.
				}

				return 1044157; // You failed to create the item, but no materials were lost.
			}

			return 1044154; // You create the item.
		}
	}
}
