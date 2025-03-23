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

namespace VitaNex.Modules.TrashCollection
{
	public class GeneralTrashHandler : BaseTrashHandler
	{
		public GeneralTrashHandler()
			: this(true)
		{ }

		public GeneralTrashHandler(
			bool enabled,
			TrashPriority priority = TrashPriority.Lowest,
			IEnumerable<Type> accepts = null,
			IEnumerable<Type> ignores = null)
			: base(enabled, priority, accepts, ignores)
		{
			IgnoreBlessed = true;
			IgnoreInsured = true;
		}

		public GeneralTrashHandler(GenericReader reader)
			: base(reader)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(1);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			if (version > 0)
			{
				return;
			}

			IgnoreBlessed = true;
			IgnoreInsured = true;
		}
	}
}