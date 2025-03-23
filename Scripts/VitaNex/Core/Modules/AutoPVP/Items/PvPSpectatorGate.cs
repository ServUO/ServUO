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
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPSpectatorGate : FloorTile<PlayerMobile>
	{
		[CommandProperty(AutoPvP.Access)]
		public PvPBattle Battle { get; set; }

		public override bool ForceShowProperties => true;

		public PvPSpectatorGate(PvPBattle battle)
		{
			Battle = battle;

			Name = "PvP Battle Gate";
			ItemID = 19343;
			Hue = 51;
			Visible = true;
			Movable = false;
		}

		public PvPSpectatorGate(Serial serial)
			: base(serial)
		{ }

		public override void OnAosSingleClick(Mobile from)
		{
			OnSingleClick(from);
		}

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			if (Battle == null)
			{
				return;
			}

			LabelTo(from, "Battle: {0}", Battle.Name);
			LabelTo(from, "Status: {0} ({1})", Battle.State, Battle.GetStateTimeLeft().ToSimpleString("h:m:s"));
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (Battle != null)
			{
				list.Add(
					"Battle: {0}\nStatus: {1} ({2})",
					Battle.Name,
					Battle.State,
					Battle.GetStateTimeLeft().ToSimpleString("h:m:s"));
			}
		}

		public override bool OnMoveOver(PlayerMobile mob)
		{
			if (!base.OnMoveOver(mob) || mob == null || Battle == null || Battle.Deleted || !Battle.SpectateAllowed || Battle.IsInternal)
			{
				return false;
			}

			Timer.DelayCall(
				TimeSpan.FromSeconds(1),
				() =>
				{
					if (mob.X == X && mob.Y == Y)
					{
						new ConfirmDialogGump(mob)
						{
							Title = "Join as Spectator?",
							Html = "Join " + Battle.Name +
								   " as a spectator.\nYou will be teleported to a safe area where you can watch the battle.\nClick OK to join!",
							AcceptHandler = b => Battle.AddSpectator(mob, true)
						}.Send();
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
