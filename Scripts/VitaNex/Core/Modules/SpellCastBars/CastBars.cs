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
using System.Drawing;

using Server;
using Server.Mobiles;
using Server.Spells;

using VitaNex.IO;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.CastBars
{
	public static partial class SpellCastBars
	{
		public const AccessLevel Access = AccessLevel.Administrator;

		private static readonly PollTimer _InternalTimer;

		private static readonly Queue<PlayerMobile> _CastBarQueue;

		public static CastBarsOptions CMOptions { get; private set; }

		public static Dictionary<PlayerMobile, SpellCastBar> Instances { get; private set; }

		public static BinaryDataStore<PlayerMobile, CastBarState> States { get; private set; }

		public static event Action<CastBarRequestEventArgs> OnCastBarRequest;

		private static void OnSpellRequest(CastSpellRequestEventArgs e)
		{
			if (!CMOptions.ModuleEnabled)
			{
				return;
			}

			var user = e.Mobile as PlayerMobile;

			if (user == null)
			{
				return;
			}

			var o = EnsureState(user);

			if (o != null && o.Enabled)
			{
				if (CMOptions.ModuleDebug)
				{
					CMOptions.ToConsole("{0} casting {1} ({2})", user, e.SpellID, user.Spell);
				}

				if (user.Spell != null && SpellRegistry.GetRegistryNumber(user.Spell) == e.SpellID)
				{
					SendCastBarGump(user);
				}
				else
				{
					_CastBarQueue.Enqueue(user);
				}
			}
		}

		private static void PollCastBarQueue()
		{
			if (!CMOptions.ModuleEnabled)
			{
				_CastBarQueue.Clear();

				return;
			}

			while (_CastBarQueue.Count > 0)
			{
				SendCastBarGump(_CastBarQueue.Dequeue());
			}
		}

		public static void SendCastBarGump(PlayerMobile user)
		{
			if (!CMOptions.ModuleEnabled || user == null || !user.IsOnline())
			{
				return;
			}

			var o = EnsureState(user);

			if (o == null || !o.Enabled)
			{
				return;
			}

			var e = new CastBarRequestEventArgs(user, o.Offset);

			if (OnCastBarRequest != null)
			{
				OnCastBarRequest(e);
			}

			if (e.Gump == null)
			{
				CastBarRequestHandler(e);
			}

			if (e.Gump != null)
			{
				e.Gump.Refresh(true);
			}
		}

		private static void CastBarRequestHandler(CastBarRequestEventArgs e)
		{
			if (!CMOptions.ModuleEnabled || e.User == null || !e.User.IsOnline() || e.Gump != null)
			{
				return;
			}

			if (!Instances.TryGetValue(e.User, out var cb) || cb.IsDisposed)
			{
				Instances[e.User] = cb = new SpellCastBar(e.User, e.Location.X, e.Location.Y);
			}
			else
			{
				cb.X = e.Location.X;
				cb.Y = e.Location.Y;
			}

			cb.Preview = false;

			e.Gump = cb;

			if (CMOptions.ModuleDebug)
			{
				CMOptions.ToConsole(
					"Request: {0} casting {1}, using {2} ({3}) at {4}",
					e.User,
					e.User.Spell,
					cb,
					cb.Preview ? "Prv" : "Std",
					e.Location);
			}
		}

		public static void HandleToggleCommand(PlayerMobile user)
		{
			if (!CMOptions.ModuleEnabled || user == null || !user.IsOnline())
			{
				return;
			}

			var t = !GetToggle(user);

			SetToggle(user, t);

			user.SendMessage(!t ? 0x22 : 0x55, "Cast-Bar has been {0}.", !t ? "disabled" : "enabled");
		}

		public static void HandlePositionCommand(PlayerMobile user)
		{
			if (!CMOptions.ModuleEnabled || user == null || !user.IsOnline())
			{
				return;
			}

			var e = new CastBarRequestEventArgs(user, GetOffset(user));

			if (OnCastBarRequest != null)
			{
				OnCastBarRequest(e);
			}

			if (e.Gump == null)
			{
				CastBarRequestHandler(e);
			}

			if (e.Gump == null)
			{
				return;
			}

			e.Gump.Preview = true;

			new OffsetSelectorGump(
				user,
				e.Gump.Refresh(true),
				e.Location,
				(ui, oldValue) =>
				{
					SetOffset(user, ui.Value);

					ui.User.SendMessage(0x55, "Cast-Bar position set to X({0:#,0}), Y({1:#,0}).", ui.Value.X, ui.Value.Y);

					e.Gump.X = ui.Value.X;
					e.Gump.Y = ui.Value.Y;

					e.Gump.Refresh(true);
				}).Send();
		}

		public static bool GetToggle(PlayerMobile user)
		{
			var o = EnsureState(user);

			if (o != null)
			{
				return o.Enabled;
			}

			return CastBarState.DefEnabled;
		}

		public static void SetToggle(PlayerMobile user, bool toggle)
		{
			var o = EnsureState(user);

			if (o != null)
			{
				o.Enabled = toggle;
			}
		}

		public static Point GetOffset(PlayerMobile user)
		{
			var o = EnsureState(user);

			if (o != null)
			{
				return o.Offset;
			}

			return CastBarState.DefOffset;
		}

		public static void SetOffset(PlayerMobile user, Point loc)
		{
			var o = EnsureState(user);

			if (o != null)
			{
				o.Offset = loc;
			}
		}

		public static CastBarState EnsureState(PlayerMobile user)
		{
			if (user == null)
			{
				return null;
			}

			var o = States.GetValue(user);

			if (user.Deleted)
			{
				States.Remove(user);

				return null;
			}

			if (o == null)
			{
				States[user] = o = new CastBarState();
			}

			return o;
		}
	}
}