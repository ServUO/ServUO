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
using Server;

using VitaNex.IO;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class AutoPvPStatistics : PropertyObject
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual DataStoreStatus BattlesStatus => AutoPvP.Profiles.Status;

		[CommandProperty(AutoPvP.Access)]
		public virtual DataStoreStatus ProfilesStatus => AutoPvP.Battles.Status;

		[CommandProperty(AutoPvP.Access)]
		public virtual int Battles => AutoPvP.Battles.Count;

		[CommandProperty(AutoPvP.Access)]
		public virtual int BattlesInternal => AutoPvP.CountBattles(PvPBattleState.Internal);

		[CommandProperty(AutoPvP.Access)]
		public virtual int BattlesEnded => AutoPvP.CountBattles(PvPBattleState.Ended);

		[CommandProperty(AutoPvP.Access)]
		public virtual int BattlesPreparing => AutoPvP.CountBattles(PvPBattleState.Preparing);

		[CommandProperty(AutoPvP.Access)]
		public virtual int BattlesQueueing => AutoPvP.CountBattles(PvPBattleState.Queueing);

		[CommandProperty(AutoPvP.Access)]
		public virtual int BattlesRunning => AutoPvP.CountBattles(PvPBattleState.Running);

		[CommandProperty(AutoPvP.Access)]
		public virtual int Participants => AutoPvP.TotalParticipants();

		[CommandProperty(AutoPvP.Access)]
		public virtual int Spectators => AutoPvP.TotalSpectators();

		[CommandProperty(AutoPvP.Access)]
		public virtual int Queueing => AutoPvP.TotalQueued();

		[CommandProperty(AutoPvP.Access)]
		public virtual int Profiles => AutoPvP.Profiles.Count;

		[CommandProperty(AutoPvP.Access)]
		public virtual int Scenarios => AutoPvP.Scenarios.Length;

		public AutoPvPStatistics()
		{ }

		public AutoPvPStatistics(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "View Statistics";
		}

		public override void Clear()
		{ }

		public override void Reset()
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
					break;
			}
		}
	}
}