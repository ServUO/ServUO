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

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class TournamentTeam : PvPTeam
	{
		public TournamentTeam(
			PvPBattle battle,
			string name = "Incognito",
			int minCapacity = 10,
			int maxCapacity = 50,
			int color = 12)
			: base(battle, name, minCapacity, maxCapacity, color)
		{
			RespawnOnStart = false;
			RespawnOnDeath = false;
			KickOnDeath = true;
		}

		public TournamentTeam(PvPBattle battle, GenericReader reader)
			: base(battle, reader)
		{ }

		public override void AddMember(PlayerMobile pm, bool teleport)
		{
			if (IsMember(pm))
			{
				return;
			}

			Members[pm] = DateTime.UtcNow;

			if (teleport)
			{
				Battle.TeleportToHomeBase(this, pm);
			}

			OnMemberAdded(pm);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
}