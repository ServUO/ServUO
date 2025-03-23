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
using Server.Mobiles;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public delegate T BattleNotorietyHandler<out T>(PlayerMobile x, PlayerMobile y);

	public static class BattleNotoriety
	{
		public const int Bubble = -1;

		public static void Enable()
		{
			NotoUtility.RegisterNameHandler(MobileNotoriety, Int32.MaxValue - 100);
			NotoUtility.RegisterBeneficialHandler(AllowBeneficial, Int32.MaxValue - 100);
			NotoUtility.RegisterHarmfulHandler(AllowHarmful, Int32.MaxValue - 100);
		}

		public static void Disable()
		{
			NotoUtility.UnregisterNameHandler(MobileNotoriety);
			NotoUtility.UnregisterBeneficialHandler(AllowBeneficial);
			NotoUtility.UnregisterHarmfulHandler(AllowHarmful);
		}

		public static bool AllowBeneficial(Mobile a, Mobile b, out bool handled)
		{
			handled = false;

			if (!AutoPvP.CMOptions.ModuleEnabled)
			{
				return false;
			}

			if (NotoUtility.Resolve(a, b, out PlayerMobile x, out PlayerMobile y))
			{
				var battle = AutoPvP.FindBattle(y);

				if (battle != null && !battle.Deleted)
				{
					var allow = battle.AllowBeneficial(a, b, out handled);

					if (handled)
					{
						return allow;
					}
				}
			}

			return false;
		}

		public static bool AllowHarmful(Mobile a, Mobile b, out bool handled)
		{
			handled = false;

			if (!AutoPvP.CMOptions.ModuleEnabled)
			{
				return false;
			}

			if (NotoUtility.Resolve(a, b, out PlayerMobile x, out PlayerMobile y))
			{
				var battle = AutoPvP.FindBattle(y);

				if (battle != null && !battle.Deleted)
				{
					var allow = battle.AllowHarmful(a, b, out handled);

					if (handled)
					{
						return allow;
					}
				}
			}

			return false;
		}

		public static int MobileNotoriety(Mobile a, Mobile b, out bool handled)
		{
			handled = false;

			if (!AutoPvP.CMOptions.ModuleEnabled)
			{
				return Bubble;
			}

			if (NotoUtility.Resolve(a, b, out PlayerMobile x, out PlayerMobile y))
			{
				var battle = AutoPvP.FindBattle(y);

				if (battle != null && !battle.Deleted)
				{
					var result = battle.NotorietyHandler(a, b, out handled);

					if (handled)
					{
						return result;
					}
				}
			}

			return Bubble;
		}
	}
}