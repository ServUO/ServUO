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
using System.Globalization;
using System.Linq;
using System.Text;

using Server;
using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPBattleCommandState
	{
		public virtual PvPBattle Battle { get; protected set; }
		public virtual PlayerMobile Mobile { get; protected set; }
		public virtual string Command { get; protected set; }
		public virtual string Speech { get; protected set; }
		public virtual string[] Args { get; protected set; }

		public PvPBattleCommandState(PvPBattle battle, PlayerMobile from, string command, string[] args)
		{
			Battle = battle;
			Mobile = from;
			Command = command;
			Args = args;
			Speech = String.Join(" ", args);
		}
	}

	public class PvPBattleCommandInfo
	{
		public virtual string Command { get; protected set; }
		public virtual string Usage { get; set; }
		public virtual string Description { get; set; }
		public virtual AccessLevel Access { get; set; }
		public virtual Func<PvPBattleCommandState, bool> Handler { get; set; }

		public PvPBattleCommandInfo(
			string command,
			string desc,
			string usage,
			AccessLevel access,
			Func<PvPBattleCommandState, bool> handler)
		{
			Command = command;
			Description = desc;
			Usage = usage;
			Access = access;
			Handler = handler;
		}
	}

	public abstract partial class PvPBattle
	{
		private char _SubCommandPrefix = '@';

		[CommandProperty(AutoPvP.Access)]
		public virtual char SubCommandPrefix
		{
			get
			{
				if (!Char.IsSymbol(_SubCommandPrefix))
				{
					_SubCommandPrefix = '@';
				}

				return _SubCommandPrefix;
			}
			set
			{
				if (!Char.IsSymbol(value))
				{
					value = '@';
				}

				_SubCommandPrefix = value;
			}
		}

		public Dictionary<string, PvPBattleCommandInfo> SubCommandHandlers { get; private set; }

		protected virtual void RegisterSubCommands()
		{
			RegisterSubCommand(
				"help",
				state =>
				{
					if (state == null || state.Mobile == null || state.Mobile.Deleted)
					{
						return false;
					}

					foreach (var ci in SubCommandHandlers.Keys.Select(cmd => SubCommandHandlers[cmd])
														 .Where(ci => state.Mobile.AccessLevel >= ci.Access))
					{
						state.Mobile.SendMessage("{0}{1} {2}", SubCommandPrefix, ci.Command, ci.Usage);
					}

					return true;
				},
				"Displays a list of available commands for this battle.",
				"[?]",
				AccessLevel.Player);

			RegisterSubCommandAlias("help", "commands");

			RegisterSubCommand(
				"battle",
				state =>
				{
					if (state == null || state.Mobile == null || state.Mobile.Deleted)
					{
						return false;
					}

					LocalBroadcast("[{0}]: {1}", state.Mobile.RawName, state.Speech);
					return true;
				},
				"Broadcasts a message to all battle participants and spectators.",
				"<message>",
				AccessLevel.Player);

			RegisterSubCommandAlias("battle", "b");

			RegisterSubCommand(
				"team",
				state =>
				{
					if (state == null || state.Mobile == null || state.Mobile.Deleted)
					{
						return false;
					}

					if (!IsParticipant(state.Mobile, out var team))
					{
						state.Mobile.SendMessage("You must be a participant to use that command.");
						return true;
					}

					if (team == null || team.Deleted)
					{
						state.Mobile.SendMessage("You are not a member of a team.");
						return true;
					}

					team.Broadcast("[{0}]: {1}", state.Mobile.RawName, state.Speech);
					return true;
				},
				"Sends a message to your team.",
				"<message>",
				AccessLevel.Player);

			RegisterSubCommandAlias("team", "t");

			RegisterSubCommand(
				"join",
				state =>
				{
					if (state == null || state.Mobile == null || state.Mobile.Deleted)
					{
						return false;
					}

					if (IsParticipant(state.Mobile))
					{
						state.Mobile.SendMessage("You are already participating in this battle.");
						return true;
					}

					if (IsQueued(state.Mobile))
					{
						state.Mobile.SendMessage("You are already queued for this battle.");
						return true;
					}

					Enqueue(state.Mobile);
					return true;
				},
				"Joins the queue for the battle.",
				"",
				AccessLevel.Player);

			RegisterSubCommandAlias("join", "j");

			RegisterSubCommand(
				"leave",
				state =>
				{
					if (state == null || state.Mobile == null || state.Mobile.Deleted)
					{
						return false;
					}

					Quit(state.Mobile, true);
					return true;
				},
				"Removes you from the battle.",
				"",
				AccessLevel.Player);

			RegisterSubCommandAlias("leave", "quit");

			RegisterSubCommand(
				"time",
				state =>
				{
					if (state == null || state.Mobile == null || state.Mobile.Deleted)
					{
						return false;
					}

					var timeLeft = GetStateTimeLeft(DateTime.UtcNow);

					if (timeLeft > TimeSpan.Zero)
					{
						var time = timeLeft.ToSimpleString(@"h\:m\:s");
						var nextState = "";

						switch (State)
						{
							case PvPBattleState.Queueing:
								nextState = "Preparation";
								break;
							case PvPBattleState.Preparing:
								nextState = "Start";
								break;
							case PvPBattleState.Running:
								nextState = "End";
								break;
							case PvPBattleState.Ended:
								nextState = "Open";
								break;
						}

						state.Mobile.SendMessage("Battle is currently {0}. Time left until {1}: {2}", State.ToString(), nextState, time);
					}
					else
					{
						state.Mobile.SendMessage("Time unavailable.");
					}

					return true;
				},
				"Displays the amount of time left until the next battle state.",
				"",
				AccessLevel.Player);

			RegisterSubCommand(
				"menu",
				state =>
				{
					if (state == null || state.Mobile == null || state.Mobile.Deleted)
					{
						return false;
					}

					new PvPBattleUI(state.Mobile, battle: this).Send();
					return true;
				},
				"Opens the interface for this battle.",
				"",
				AccessLevel.Player);

			RegisterSubCommand(
				"config",
				state =>
				{
					if (state == null || state.Mobile == null || state.Mobile.Deleted)
					{
						return false;
					}

					state.Mobile.SendGump(new PropertiesGump(state.Mobile, this));
					return true;
				},
				"Opens the configuration for this battle.");

			RegisterSubCommand(
				"scores",
				state =>
				{
					if (state == null || state.Mobile == null || state.Mobile.Deleted)
					{
						return false;
					}

					var stats = new StringBuilder();

					ForEachTeam(
						t =>
						{
							stats.AppendLine("Team: {0}", t.Name);
							stats.AppendLine();

							t.GetHtmlStatistics(state.Mobile, stats);

							stats.AppendLine();
						});

					new NoticeDialogGump(state.Mobile)
					{
						Width = 600,
						Height = 400,
						Title = "Battle Statistics",
						Html = stats.ToString()
					}.Send();

					return true;
				},
				"Display the statistics for all teams.");

			RegisterSubCommand(
				"hidden",
				state =>
				{
					if (state == null || state.Mobile == null || state.Mobile.Deleted)
					{
						return false;
					}

					Hidden = !Hidden;
					return true;
				},
				"Toggle the battle visiblility.");

			RegisterSubCommand(
				"state",
				state =>
				{
					if (state == null || state.Mobile == null || state.Mobile.Deleted)
					{
						return false;
					}

					var oldState = State;

					if (!Validate())
					{
						State = PvPBattleState.Internal;
						state.Mobile.SendMessage("This battle has failed validation and has been internalized.");
					}
					else
					{
						Hidden = false;

						switch (State)
						{
							case PvPBattleState.Internal:
								State = PvPBattleState.Queueing;
								break;
							case PvPBattleState.Queueing:
								State = PvPBattleState.Preparing;
								break;
							case PvPBattleState.Preparing:
								State = PvPBattleState.Running;
								break;
							case PvPBattleState.Running:
								State = PvPBattleState.Ended;
								break;
							case PvPBattleState.Ended:
								State = PvPBattleState.Queueing;
								break;
						}
					}

					if (State != oldState)
					{
						state.Mobile.SendMessage("The state of the battle has been changed to {0}", State.ToString());
						return true;
					}

					return false;
				},
				"Toggles the state of the current battle.");
		}

		public void RegisterSubCommand(
			string command,
			Func<PvPBattleCommandState, bool> handler,
			string desc = "",
			string usage = "",
			AccessLevel access = AutoPvP.Access)
		{
			if (!String.IsNullOrWhiteSpace(command) && handler != null)
			{
				RegisterSubCommand(new PvPBattleCommandInfo(command, desc, usage, access, handler));
			}
		}

		public void RegisterSubCommand(PvPBattleCommandInfo info)
		{
			if (info != null)
			{
				SubCommandHandlers[info.Command] = info;
			}
		}

		public void RegisterSubCommandAlias(string command, params string[] alias)
		{
			if (!IsCommand(command))
			{
				return;
			}

			var info = SubCommandHandlers[command];

			foreach (var cmd in alias)
			{
				RegisterSubCommand(cmd, info.Handler, info.Description, info.Usage, info.Access);
			}
		}

		public bool HandleSubCommand(PlayerMobile pm, string speech)
		{
			if (pm == null || pm.Deleted || String.IsNullOrWhiteSpace(speech))
			{
				return false;
			}

			speech = speech.Trim();

			if (!speech.StartsWith(SubCommandPrefix.ToString(CultureInfo.InvariantCulture)))
			{
				return false;
			}

			var command = String.Empty;
			var args = new string[0];

			speech = speech.TrimStart(SubCommandPrefix);

			var split = speech.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			if (split.Length > 0)
			{
				command = split[0];
				args = new string[split.Length - 1];

				for (var i = 0; i < args.Length; i++)
				{
					args[i] = split[i + 1];
				}
			}

			if (!IsCommand(command))
			{
				pm.SendMessage("Command not found.");
				return true;
			}

			var info = SubCommandHandlers[command];

			if (pm.AccessLevel < info.Access)
			{
				pm.SendMessage("You do not have access to that command.");
				return true;
			}

			if (args.Length > 0 && (args[0] == "?" || Insensitive.Equals(args[0], "help")))
			{
				pm.SendMessage("Usage: {0}{1} {2}", SubCommandPrefix, info.Command, info.Usage);
				pm.SendMessage(info.Description);
				return true;
			}

			var state = new PvPBattleCommandState(this, pm, command, args);

			if (info.Handler.Invoke(state))
			{
				OnCommand(state);
				return true;
			}

			pm.SendMessage("Usage: {0}{1} {2}", SubCommandPrefix, info.Command, info.Usage);
			pm.SendMessage(info.Description);
			return true;
		}

		public bool IsCommand(string command)
		{
			return !String.IsNullOrWhiteSpace(command) && SubCommandHandlers.ContainsKey(command);
		}

		protected virtual void OnCommand(PvPBattleCommandState state)
		{ }
	}
}