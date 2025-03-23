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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Server;
using Server.Gumps;
using Server.Network;

using VitaNex.Network;
#endregion

namespace VitaNex.SuperGumps
{
	public abstract partial class SuperGump
	{
		private static readonly object _GlobalLock = new object();
		private static readonly object _InstanceLock = new object();

		public static Dictionary<int, SuperGump> GlobalInstances { get; private set; }
		public static Dictionary<Mobile, List<SuperGump>> Instances { get; private set; }

		public static event Action<Mobile, Gump> CoreGumpSend;

		static SuperGump()
		{
			GlobalInstances = new Dictionary<int, SuperGump>(0x400);
			Instances = new Dictionary<Mobile, List<SuperGump>>(0x100);
		}

		[CallPriority(Int32.MaxValue)]
		public static void Configure()
		{
			EventSink.Logout += OnLogoutImpl;
			EventSink.Disconnected += OnDisconnectedImpl;
			EventSink.Speech += OnSpeechImpl;
			EventSink.Movement += OnMovementImpl;

			NetState.GumpCap = 1024;

			VitaNexCore.OnInitialized += () =>
			{
				OutgoingPacketOverrides.Register(0xB0, OnEncode0xB0_0xDD);
				OutgoingPacketOverrides.Register(0xDD, OnEncode0xB0_0xDD);
			};
		}

		public static void OnLogoutImpl(LogoutEventArgs e)
		{
			var user = e.Mobile;

			if (user == null)
			{
				return;
			}

			lock (_InstanceLock)
			{
				if (!Instances.ContainsKey(user))
				{
					return;
				}
			}

			VitaNexCore.TryCatch(
				() =>
				{
					foreach (var g in EnumerateInstances<SuperGump>(user, true))
					{
						g.Close(true);
					}
				},
				x => x.ToConsole(true));
		}

		public static void OnDisconnectedImpl(DisconnectedEventArgs e)
		{
			var user = e.Mobile;

			if (user == null)
			{
				return;
			}

			lock (_InstanceLock)
			{
				if (!Instances.ContainsKey(user))
				{
					return;
				}
			}

			VitaNexCore.TryCatch(
				() =>
				{
					foreach (var g in EnumerateInstances<SuperGump>(user, true))
					{
						g.Close(true);
					}
				},
				x => x.ToConsole(true));
		}

		public static void OnSpeechImpl(SpeechEventArgs e)
		{
			var user = e.Mobile;

			if (user == null)
			{
				return;
			}

			lock (_InstanceLock)
			{
				if (!Instances.ContainsKey(user))
				{
					return;
				}
			}

			VitaNexCore.TryCatch(
				() =>
				{
					foreach (var g in EnumerateInstances<SuperGump>(user, true))
					{
						g.OnSpeech(e);
					}
				},
				x => x.ToConsole(true));
		}

		public static void OnMovementImpl(MovementEventArgs e)
		{
			var user = e.Mobile;

			if (user == null)
			{
				return;
			}

			lock (_InstanceLock)
			{
				if (!Instances.ContainsKey(user))
				{
					return;
				}
			}

			VitaNexCore.TryCatch(
				() =>
				{
					foreach (var g in EnumerateInstances<SuperGump>(user, true))
					{
						g.OnMovement(e);
					}
				},
				x => x.ToConsole(true));
		}

		public static int RefreshInstances<TGump>(Mobile user)
			where TGump : SuperGump
		{
			return RefreshInstances<TGump>(user, false);
		}

		public static int RefreshInstances<TGump>(Mobile user, bool inherited)
			where TGump : SuperGump
		{
			return EnumerateInstances<TGump>(user, inherited)
				.Count(
					g =>
					{
						if ((g.IsOpen || g.Hidden) && !g.IsDisposed)
						{
							g.Refresh(true);
							return true;
						}

						return false;
					});
		}

		public static int RefreshInstances(Mobile user, Type type)
		{
			return RefreshInstances(user, type, false);
		}

		public static int RefreshInstances(Mobile user, Type type, bool inherited)
		{
			return EnumerateInstances(user, type, inherited)
				.Count(
					g =>
					{
						if ((g.IsOpen || g.Hidden) && !g.IsDisposed)
						{
							g.Refresh(true);
							return true;
						}

						return false;
					});
		}

		public static int CloseInstances<TGump>(Mobile user)
			where TGump : SuperGump
		{
			return CloseInstances<TGump>(user, false);
		}

		public static int CloseInstances<TGump>(Mobile user, bool inherited)
			where TGump : SuperGump
		{
			return EnumerateInstances<TGump>(user, inherited)
				.Count(
					g =>
					{
						if ((g.IsOpen || g.Hidden) && !g.IsDisposed)
						{
							g.Close(true);
							return true;
						}

						return false;
					});
		}

		public static int CloseInstances(Mobile user, Type type)
		{
			return CloseInstances(user, type, false);
		}

		public static int CloseInstances(Mobile user, Type type, bool inherited)
		{
			return EnumerateInstances(user, type, inherited)
				.Count(
					g =>
					{
						if ((g.IsOpen || g.Hidden) && !g.IsDisposed)
						{
							g.Close(true);
							return true;
						}

						return false;
					});
		}

		public static int CountInstances<TGump>(Mobile user)
			where TGump : SuperGump
		{
			return CountInstances<TGump>(user, false);
		}

		public static int CountInstances<TGump>(Mobile user, bool inherited)
			where TGump : SuperGump
		{
			return EnumerateInstances<TGump>(user, inherited).Count();
		}

		public static int CountInstances(Mobile user, Type type)
		{
			return CountInstances(user, type, false);
		}

		public static int CountInstances(Mobile user, Type type, bool inherited)
		{
			return EnumerateInstances(user, type, inherited).Count();
		}

		public static bool HasInstance<TGump>(Mobile user)
			where TGump : SuperGump
		{
			return HasInstance<TGump>(user, false);
		}

		public static bool HasInstance<TGump>(Mobile user, bool inherited)
			where TGump : SuperGump
		{
			return EnumerateInstances<TGump>(user, inherited).Any();
		}

		public static bool HasInstance(Mobile user, Type type)
		{
			return HasInstance(user, type, false);
		}

		public static bool HasInstance(Mobile user, Type type, bool inherited)
		{
			return EnumerateInstances(user, type, inherited).Any();
		}

		public static TGump GetInstance<TGump>(Mobile user)
			where TGump : SuperGump
		{
			return GetInstance<TGump>(user, false);
		}

		public static TGump GetInstance<TGump>(Mobile user, bool inherited)
			where TGump : SuperGump
		{
			return EnumerateInstances<TGump>(user, inherited).FirstOrDefault();
		}

		public static SuperGump GetInstance(Mobile user, Type type)
		{
			return GetInstance(user, type, false);
		}

		public static SuperGump GetInstance(Mobile user, Type type, bool inherited)
		{
			return EnumerateInstances(user, type, inherited).FirstOrDefault();
		}

		public static TGump[] GetInstances<TGump>(Mobile user)
			where TGump : SuperGump
		{
			return GetInstances<TGump>(user, false);
		}

		public static TGump[] GetInstances<TGump>(Mobile user, bool inherited)
			where TGump : SuperGump
		{
			return EnumerateInstances<TGump>(user, inherited).ToArray();
		}

		public static SuperGump[] GetInstances(Mobile user, Type type)
		{
			return GetInstances(user, type, false);
		}

		public static SuperGump[] GetInstances(Mobile user, Type type, bool inherited)
		{
			return EnumerateInstances(user, type, inherited).ToArray();
		}

		public static IEnumerable<TGump> EnumerateInstances<TGump>(Mobile user)
			where TGump : SuperGump
		{
			return EnumerateInstances<TGump>(user, false);
		}

		public static IEnumerable<TGump> EnumerateInstances<TGump>(Mobile user, bool inherited)
			where TGump : SuperGump
		{
			if (user == null)
			{
				yield break;
			}

			List<SuperGump> list;

			lock (_InstanceLock)
			{
				list = Instances.GetValue(user);

				if (list == null || list.Count == 0)
				{
					Instances.Remove(user);
					yield break;
				}
			}

			lock (((ICollection)list).SyncRoot)
			{
				var index = list.Count;

				while (--index >= 0)
				{
					if (!list.InBounds(index))
					{
						continue;
					}

					var gump = list[index];

					if (gump != null && gump.TypeEquals<TGump>(inherited))
					{
						yield return (TGump)gump;
					}
				}
			}
		}

		public static IEnumerable<SuperGump> EnumerateInstances(Mobile user, Type type)
		{
			return EnumerateInstances(user, type, false);
		}

		public static IEnumerable<SuperGump> EnumerateInstances(Mobile user, Type type, bool inherited)
		{
			if (user == null)
			{
				yield break;
			}

			List<SuperGump> list;

			lock (_InstanceLock)
			{
				list = Instances.GetValue(user);

				if (list == null || list.Count == 0)
				{
					Instances.Remove(user);
					yield break;
				}
			}

			lock (((ICollection)list).SyncRoot)
			{
				var index = list.Count;

				while (--index >= 0)
				{
					if (!list.InBounds(index))
					{
						continue;
					}

					var gump = list[index];

					if (gump != null && gump.TypeEquals(type, inherited))
					{
						yield return gump;
					}
				}
			}
		}

		private static void OnEncode0xB0_0xDD(NetState state, PacketReader reader, ref byte[] buffer, ref int length)
		{
			if (state == null || reader == null || buffer == null || length < 0)
			{
				return;
			}

			var pos = reader.Seek(0, SeekOrigin.Current);
			reader.Seek(3, SeekOrigin.Begin);
			var serial = reader.ReadInt32();
			reader.Seek(pos, SeekOrigin.Begin);

			if (serial <= 0)
			{
				return;
			}

			var cg = state.Gumps.FirstOrDefault(o => o != null && o.Serial == serial);

			if (cg == null || cg is SuperGump)
			{
				var g = cg as SuperGump;

				if (g == null)
				{
					lock (_GlobalLock)
					{
						g = GlobalInstances.GetValue(serial);
					}
				}

				if (g != null && !g.Compiled)
				{
					g.Refresh(true);
				}

				return;
			}

			if (CoreGumpSend != null && state.Mobile != null)
			{
				Timer.DelayCall(m => CoreGumpSend(m, cg), state.Mobile);
			}
		}

		protected virtual void RegisterInstance()
		{
			if (User == null || User.Deleted || IsDisposed)
			{
				return;
			}

			lock (_GlobalLock)
			{
				GlobalInstances[Serial] = this;
			}

			List<SuperGump> list;

			lock (_InstanceLock)
			{
				list = Instances.GetValue(User);

				if (list == null)
				{
					Instances[User] = list = new List<SuperGump>(0x10);
				}
			}

			var added = false;

			lock (((ICollection)list).SyncRoot)
			{
				if (!list.Contains(this))
				{
					list.Add(this);

					added = true;
				}
			}

			if (added)
			{
				OnInstanceRegistered();
			}
		}

		protected virtual void UnregisterInstance()
		{
			lock (_GlobalLock)
			{
				GlobalInstances.Remove(Serial);
			}

			var user = User;

			if (user == null)
			{
				lock (_InstanceLock)
				{
					user = Instances.FirstOrDefault(kv => kv.Value.Contains(this)).Key;
				}
			}

			if (user == null)
			{
				return;
			}

			List<SuperGump> list;

			lock (_InstanceLock)
			{
				list = Instances.GetValue(User);

				if (list == null || list.Count == 0)
				{
					Instances.Remove(user);
					return;
				}
			}

			var removed = false;

			lock (((ICollection)list).SyncRoot)
			{
				if (list.Remove(this))
				{
					list.TrimExcess();

					removed = true;
				}

				if (list.Count == 0)
				{
					lock (_InstanceLock)
					{
						Instances.Remove(user);
					}
				}
			}

			if (removed)
			{
				OnInstanceUnregistered();
			}
		}

		protected virtual void OnInstanceRegistered()
		{
			//Console.WriteLine("{0} Registered to {1}", this, User);
		}

		protected virtual void OnInstanceUnregistered()
		{
			//Console.WriteLine("{0} Unregistered from {1}", this, User);
		}
	}
}