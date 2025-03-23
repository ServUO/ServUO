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
using Server.Targeting;

using VitaNex.Network;
#endregion

namespace VitaNex.Items
{
	public interface IBaseThrowable
	{
		string Token { get; set; }
		string Usage { get; set; }

		bool AllowCombat { get; set; }
		bool AllowDeadUser { get; set; }
		bool ClearHands { get; set; }
		bool Consumable { get; set; }
		bool DismountUser { get; set; }

		int EffectHue { get; set; }
		int EffectID { get; set; }
		int EffectSpeed { get; set; }
		EffectRender EffectRender { get; set; }

		int ThrowSound { get; set; }
		int ImpactSound { get; set; }

		SkillName RequiredSkill { get; set; }
		double RequiredSkillValue { get; set; }

		TargetFlags TargetFlags { get; set; }
		DateTime ThrownLast { get; set; }
		TimeSpan ThrowRecovery { get; set; }
		int ThrowRange { get; set; }
	}
}