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

#if ServUO58
#define ServUOX
#endif

#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Network;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public enum PvPBattleWarning
	{
		Starting,
		Ending,
		WBStarting,
		WBEnding
	}

	public abstract partial class PvPBattle
	{
		public virtual IEnumerable<PlayerMobile> GetLocalBroadcastList()
		{
			if (BattleRegion != null)
			{
#if ServUOX
				foreach (var pm in BattleRegion.AllPlayers.OfType<PlayerMobile>().Where(IsOnline))
#else
				foreach (var pm in BattleRegion.GetMobiles().OfType<PlayerMobile>().Where(IsOnline))
#endif
				{
					yield return pm;
				}
			}

			if (SpectateRegion != null)
			{
#if ServUOX
				foreach (var pm in SpectateRegion.AllPlayers.OfType<PlayerMobile>().Where(IsOnline))
#else
				foreach (var pm in SpectateRegion.GetMobiles().OfType<PlayerMobile>().Where(IsOnline))
#endif
				{
					yield return pm;
				}
			}
		}

		public virtual IEnumerable<PlayerMobile> GetWorldBroadcastList()
		{
			return NetState.Instances.Where(state => state != null)
						   .Select(state => state.Mobile as PlayerMobile)
						   .Where(pm => pm != null && !pm.Deleted)
						   .Where(pm => IsOnline(pm) && AutoPvP.EnsureProfile(pm).IsSubscribed(this));
		}

		public virtual void LocalBroadcast(string message, params object[] args)
		{
			var text = String.Format(message, args);

			if (String.IsNullOrWhiteSpace(text))
			{
				return;
			}

			if (Options.Broadcasts.Local.Mode == PvPBattleLocalBroadcastMode.Disabled)
			{
				return;
			}

			AutoPvP.InvokeBattleLocalBroadcast(this, text);

			foreach (var pm in GetLocalBroadcastList())
			{
				pm.SendMessage(IsParticipant(pm, out var team) ? team.Color : Options.Broadcasts.Local.MessageHue, text);
			}
		}

		public virtual void WorldBroadcast(string message, params object[] args)
		{
			var text = String.Format(message, args);

			if (String.IsNullOrWhiteSpace(text))
			{
				return;
			}

			if (Options.Broadcasts.World.Mode == PvPBattleWorldBroadcastMode.Disabled)
			{
				return;
			}

			AutoPvP.InvokeBattleWorldBroadcast(this, text);

			switch (Options.Broadcasts.World.Mode)
			{
				case PvPBattleWorldBroadcastMode.Notify:
				{
					foreach (var pm in GetWorldBroadcastList())
					{
						pm.SendNotification(text, true, 0.5, 10.0);
					}
				}
				break;
				case PvPBattleWorldBroadcastMode.Broadcast:
				{
					var p = new AsciiMessage(Server.Serial.MinusOne, -1, MessageType.Regular, Options.Broadcasts.World.MessageHue, 3, "System", text);

					p.Acquire();

					foreach (var pm in GetWorldBroadcastList())
					{
						pm.Send(p);
					}

					p.Release();

					NetState.FlushAll();
				}
				break;
				case PvPBattleWorldBroadcastMode.TownCrier:
				{
					foreach (var tc in TownCrier.Instances)
					{
						tc.PublicOverheadMessage(MessageType.Yell, Options.Broadcasts.World.MessageHue, true, String.Format(message, args));
					}
				}
				break;
			}
		}

		protected virtual void BroadcastStateHandler()
		{
			if (Hidden)
			{
				return;
			}

			var state = State;
			var timeLeft = GetStateTimeLeft(DateTime.UtcNow).Add(TimeSpan.FromSeconds(1.0));

			if (timeLeft <= TimeSpan.Zero)
			{
				return;
			}

			switch (state)
			{
				case PvPBattleState.Ended:
					BroadcastOpenMessage(timeLeft);
					break;
				case PvPBattleState.Preparing:
					BroadcastStartMessage(timeLeft);
					break;
				case PvPBattleState.Running:
					BroadcastEndMessage(timeLeft);
					break;
			}
		}

		protected virtual void BroadcastOpenMessage(TimeSpan timeLeft)
		{
			if (timeLeft.Minutes > 5 || timeLeft.Minutes == 0 || timeLeft.Seconds != 0)
			{
				return;
			}

			var msg = String.Format("{0} {1}", timeLeft.Minutes, timeLeft.Minutes != 1 ? "minutes" : "minute");

			if (String.IsNullOrWhiteSpace(msg))
			{
				return;
			}

			if (Options.Broadcasts.Local.OpenNotify)
			{
				LocalBroadcast("{0} will open in {1}!", Name, msg);
			}

			if (Options.Broadcasts.World.OpenNotify)
			{
				var cmd = String.Empty;

				if (QueueAllowed)
				{
					cmd = AutoPvP.CMOptions.Advanced.Commands.BattlesCommand;
					cmd = String.Format("Use {0}{1} to join!", CommandSystem.Prefix, cmd);
				}

				WorldBroadcast("{0} will open in {1}! {2}", Name, msg, cmd);
			}
		}

		protected virtual void BroadcastStartMessage(TimeSpan timeLeft)
		{
			if ((timeLeft.Minutes == 0 && timeLeft.Seconds > 10) || timeLeft.Minutes > 5)
			{
				return;
			}

			var msg = String.Empty;

			if (timeLeft.Minutes > 0)
			{
				if (timeLeft.Seconds == 0)
				{
					msg = String.Format("{0} {1}", timeLeft.Minutes, timeLeft.Minutes != 1 ? "minutes" : "minute");
				}
			}
			else if (timeLeft.Seconds > 0)
			{
				msg = String.Format("{0} {1}", timeLeft.Seconds, timeLeft.Seconds != 1 ? "seconds" : "second");
			}

			if (String.IsNullOrWhiteSpace(msg))
			{
				return;
			}

			if (Options.Broadcasts.Local.StartNotify)
			{
				LocalBroadcast("{0} will start in {1}!", Name, msg);
			}

			if (Options.Broadcasts.World.StartNotify && timeLeft.Minutes > 0)
			{
				var cmd = String.Empty;

				if (QueueAllowed)
				{
					cmd = AutoPvP.CMOptions.Advanced.Commands.BattlesCommand;
					cmd = String.Format("Use {0}{1} to join!", CommandSystem.Prefix, cmd);
				}

				WorldBroadcast("{0} will start in {1}! {2}", Name, msg, cmd);
			}
		}

		protected virtual void BroadcastEndMessage(TimeSpan timeLeft)
		{
			if ((timeLeft.Minutes == 0 && timeLeft.Seconds > 10) || timeLeft.Minutes > 5)
			{
				return;
			}

			var msg = String.Empty;

			if (timeLeft.Minutes > 0)
			{
				if (timeLeft.Seconds == 0)
				{
					msg = String.Format("{0} {1}", timeLeft.Minutes, timeLeft.Minutes != 1 ? "minutes" : "minute");
				}
			}
			else if (timeLeft.Seconds > 0)
			{
				msg = String.Format("{0} {1}", timeLeft.Seconds, timeLeft.Seconds != 1 ? "seconds" : "second");
			}

			if (String.IsNullOrWhiteSpace(msg))
			{
				return;
			}

			if (Options.Broadcasts.Local.EndNotify)
			{
				LocalBroadcast("{0} will end in {1}!", Name, msg);
			}

			if (Options.Broadcasts.World.EndNotify && timeLeft.Minutes > 0)
			{
				WorldBroadcast("{0} will end in {1}", Name, msg);
			}
		}
	}
}