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

using Server;
#endregion

namespace VitaNex.Modules.Games
{
	public interface IGame
	{
		Type EngineType { get; }

		IconDefinition Icon { get; }

		string Name { get; }
		string Desc { get; }
		string Help { get; }

		bool Enabled { get; set; }

		IEnumerable<IGameEngine> Sessions { get; }

		int SessionCount { get; }

		IGameEngine this[Mobile user] { get; set; }

		GameStatistics Statistics { get; }

		void Enable();
		void Disable();

		bool Validate(Mobile user);
		bool Open(Mobile user);
		void Close(Mobile user);
		void Reset(Mobile user);

		void Log(string context, double value, bool offset);
		void LogIncrease(string context, double value);
		void LogDecrease(string context, double value);

		void Serialize(GenericWriter writer);
		void Deserialize(GenericReader reader);
	}
}