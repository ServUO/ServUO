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

using VitaNex.Items;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPTeamGate : FloorTile<PlayerMobile>
	{
		[CommandProperty(AutoPvP.Access)]
		public PvPTeam Team { get; set; }

		public override bool ForceShowProperties => true;

		public PvPTeamGate(PvPTeam team)
		{
			Team = team;

			Name = "PvP Team Gate";
			Hue = Team.Color;
			ItemID = 19343;
			Visible = true;
			Movable = false;
		}

		public PvPTeamGate(Serial serial)
			: base(serial)
		{ }

		public override void OnAosSingleClick(Mobile from)
		{
			OnSingleClick(from);
		}

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			if (Team == null || Team.Battle == null)
			{
				return;
			}

			LabelTo(from, "Battle: {0}", Team.Battle.Name);
			LabelTo(from, "Team: {0}", Team.Name);
			LabelTo(from, "Status: {0} ({1})", Team.Battle.State, Team.Battle.GetStateTimeLeft().ToSimpleString("h:m:s"));
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (Team != null && Team.Battle != null)
			{
				list.Add(
					"Battle: {0}\nTeam: {1}\nStatus: {2} ({3})",
					Team.Battle.Name,
					Team.Name,
					Team.Battle.State,
					Team.Battle.GetStateTimeLeft().ToSimpleString("h:m:s"));
			}
		}

		public override bool OnMoveOver(PlayerMobile mob)
		{
			if (mob == null || !base.OnMoveOver(mob))
			{
				return false;
			}

			if (Team == null || Team.Deleted || Team.Battle == null || Team.Battle.Deleted || !Team.Battle.QueueAllowed || Team.Battle.IsInternal)
			{
				return false;
			}

			Timer.DelayCall(
				TimeSpan.FromSeconds(1),
				() =>
				{
					if (mob.X == X && mob.Y == Y)
					{
						Team.Battle.Enqueue(mob, Team);
					}
				});

			return true;
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
