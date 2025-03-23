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
#endregion

namespace VitaNex.Modules.AutoPvP
{
	[PropertyObject]
	public sealed class PvPScenario
	{
		private readonly Type _TypeOf;
		private string _Info;

		private string _Name;

		public PvPScenario(PvPBattle battle)
		{
			_TypeOf = battle.GetType();
			_Name = battle.Name;
			_Info = battle.ToHtmlString(preview: true).Replace("(Internal)", String.Empty);
		}

		public Type TypeOf => _TypeOf;

		public string Name
		{
			get => _Name;
			set
			{
				if (!String.IsNullOrEmpty(value))
				{
					_Name = value;
				}
			}
		}

		public string Info
		{
			get => _Info;
			set
			{
				if (!String.IsNullOrEmpty(value))
				{
					_Info = value;
				}
			}
		}

		public PvPBattle CreateBattle()
		{
			var battle = Activator.CreateInstance(_TypeOf) as PvPBattle;

			foreach (var profile in AutoPvP.Profiles.Values)
			{
				if (!profile.IsSubscribed(battle))
				{
					profile.Subscribe(battle);
				}
			}

			return battle;
		}

		public override string ToString()
		{
			return String.Format("{0}: {1}", _Name, _TypeOf);
		}

		public string ToHtmlString(Mobile viewer = null, bool big = true)
		{
			return big ? String.Format("<big>{0}</big>", _Info) : _Info;
		}

		public static implicit operator PvPScenario(PvPBattle battle)
		{
			return new PvPScenario(battle);
		}
	}
}