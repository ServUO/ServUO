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
using System.Drawing;
using System.Text;

using Server;
using Server.Mobiles;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPBattleMissions : PropertyObject
	{
		[CommandProperty(AutoPvP.Access)]
		public bool Enabled { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleObjectives Team { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleObjectives Player { get; set; }

		public PvPBattleMissions()
		{
			Enabled = false;

			Team = new PvPBattleObjectives();
			Player = new PvPBattleObjectives();
		}

		public PvPBattleMissions(GenericReader reader)
			: base(reader)
		{ }

		public virtual double ComputeScore(PvPTeam team)
		{
			if (!Enabled || Team.IsEmpty)
			{
				return 0;
			}

			return Team.ComputeScore(team);
		}

		public virtual double ComputeScore(PvPBattle battle, PlayerMobile player)
		{
			if (!Enabled || Player.IsEmpty)
			{
				return 0;
			}

			return Player.ComputeScore(battle, player);
		}

		public virtual bool Completed(PvPTeam team)
		{
			if (!Enabled || Team.IsEmpty)
			{
				return false;
			}

			return Team.Completed(team);
		}

		public virtual bool Completed(PvPBattle battle, PlayerMobile player)
		{
			if (!Enabled || Player.IsEmpty)
			{
				return false;
			}

			return Player.Completed(battle, player);
		}

		public virtual string GetStatus(PvPTeam team)
		{
			if (!Enabled || Team.IsEmpty)
			{
				return String.Empty;
			}

			return Team.GetStatus(team);
		}

		public virtual string GetStatus(PvPBattle battle, PlayerMobile player)
		{
			if (!Enabled || Player.IsEmpty)
			{
				return String.Empty;
			}

			return Player.GetStatus(battle, player);
		}

		public virtual void GetHtmlString(StringBuilder html)
		{
			if (!Enabled)
			{
				return;
			}

			var idx = html.Length;
			var len = html.Length;

			if (!Team.IsEmpty)
			{
				Team.GetHtmlString(html);

				if (len < html.Length)
				{
					html.Insert(len, "Team Objectives\n".WrapUOHtmlColor(Color.LawnGreen, false));
				}
			}

			len = html.Length;

			if (!Player.IsEmpty)
			{
				Player.GetHtmlString(html);

				if (len < html.Length)
				{
					html.Insert(len, "Player Objectives\n".WrapUOHtmlColor(Color.LawnGreen, false));
				}
			}

			if (idx < html.Length)
			{
				html.Insert(idx, "Missions\n".WrapUOHtmlBig().WrapUOHtmlColor(Color.PaleGoldenrod, false));
				html.Append(String.Empty.WrapUOHtmlColor(Color.White, false));
			}
		}

		public virtual string ToHtmlString()
		{
			var html = new StringBuilder();

			GetHtmlString(html);

			return html.ToString();
		}

		public override string ToString()
		{
			return "Battle Missions";
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.Write(Enabled);

			writer.WriteBlock(w => w.WriteType(Team, (w1, t) => Team.Serialize(w1)));
			writer.WriteBlock(w => w.WriteType(Player, (w1, t) => Player.Serialize(w1)));
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Enabled = reader.ReadBool();

			Team = reader.ReadBlock(r => r.ReadTypeCreate<PvPBattleObjectives>(r)) ?? new PvPBattleObjectives();
			Player = reader.ReadBlock(r => r.ReadTypeCreate<PvPBattleObjectives>(r)) ?? new PvPBattleObjectives();
		}
	}
}