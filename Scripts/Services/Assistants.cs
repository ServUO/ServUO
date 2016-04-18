#region References
using System;
using System.Collections.Generic;

using Server.Gumps;
using Server.Network;
#endregion

namespace Server.Misc
{
	public static class Assistants
	{
		[Flags]
		public enum Features : ulong
		{
			None = 0,

			FilterWeather = 1 << 0, // Weather Filter
			FilterLight = 1 << 1, // Light Filter
			SmartTarget = 1 << 2, // Smart Last Target
			RangedTarget = 1 << 3, // Range Check Last Target
			AutoOpenDoors = 1 << 4, // Automatically Open Doors
			DequipOnCast = 1 << 5, // Unequip Weapon on spell cast
			AutoPotionEquip = 1 << 6, // Un/Re-equip weapon on potion use
			PoisonedChecks = 1 << 7, // Block heal If poisoned/Macro If Poisoned condition/Heal or Cure self
			LoopedMacros = 1 << 8, // Disallow Looping macros, For loops, and macros that call other macros
			UseOnceAgent = 1 << 9, // The use once agent
			RestockAgent = 1 << 10, // The restock agent
			SellAgent = 1 << 11, // The sell agent
			BuyAgent = 1 << 12, // The buy agent
			PotionHotkeys = 1 << 13, // All potion hotkeys
			RandomTargets = 1 << 14, // All random target hotkeys (not target next, last target, target self)
			ClosestTargets = 1 << 15, // All closest target hotkeys
			OverheadHealth = 1 << 16, // Health and Mana/Stam messages shown over player's heads

			// AssistUO Only
			AutolootAgent = 1 << 17, // The autoloot agent
			BoneCutterAgent = 1 << 18, // The bone cutter agent
			JScriptMacros = 1 << 19, // Javascript macro engine
			AutoRemount = 1 << 20, // Auto remount after dismount

			All = UInt64.MaxValue // Every feature possible
		}

		public class Settings
		{
			/// <summary>
			///     Enable assistant negotiator?
			/// </summary>
			public static bool Enabled { get; set; }

			/// <summary>
			///     When true, this will cause anyone who does not negotiate.
			///     (include those not running allowed assistants at all) to be disconnected from the server.
			/// </summary>
			public static bool KickOnFailure { get; set; }

			public static Features DisallowedFeatures { get; private set; }

			/// <summary>
			///     How long to wait for a handshake response before showing warning and disconnecting.
			/// </summary>
			public static TimeSpan HandshakeTimeout { get; set; }

			/// <summary>
			///     How long to show warning message before they are disconnected.
			/// </summary>
			public static TimeSpan DisconnectDelay { get; set; }

			public static string WarningMessage { get; set; }

			static Settings()
			{
				Enabled = false;
				KickOnFailure = true;

				DisallowedFeatures = Features.None;

				HandshakeTimeout = TimeSpan.FromSeconds(30.0);
				DisconnectDelay = TimeSpan.FromSeconds(15.0);

				WarningMessage =
					"The server was unable to negotiate features with your assistant. You must download and run an updated version of <A HREF='http://www.runuo.com/products/assistuo'>AssistUO</A> or <A HREF='http://www.runuo.com/products/razor'>Razor</A>.<BR><BR>Make sure you've checked the option <B>Negotiate features with server</B>, once you have this box checked you may log in and play normally.<BR><BR>You will be disconnected shortly.";
			}

			public static void Configure()
			{
				//DisallowFeature( Features.FilterLight );
			}

			public static void DisallowFeature(Features feature)
			{
				SetDisallowed(feature, true);
			}

			public static void AllowFeature(Features feature)
			{
				SetDisallowed(feature, false);
			}

			public static void SetDisallowed(Features feature, bool value)
			{
				if (value)
				{
					DisallowedFeatures |= feature;
				}
				else
				{
					DisallowedFeatures &= ~feature;
				}
			}
		}

		public class Negotiator
		{
			private static readonly Dictionary<Mobile, Timer> _Dictionary = new Dictionary<Mobile, Timer>();

			public static void Initialize()
			{
				if (!Settings.Enabled)
				{
					return;
				}

				EventSink.Login += e =>
				{
					Mobile m = e.Mobile;

					if (m == null || m.NetState == null || !m.NetState.Running)
					{
						return;
					}

					m.Send(new BeginHandshake());

					if (_Dictionary.ContainsKey(m))
					{
						Timer t = _Dictionary[m];

						if (t != null && t.Running)
						{
							t.Stop();
						}
					}

					_Dictionary[m] = Timer.DelayCall(Settings.HandshakeTimeout, OnHandshakeTimeout, m);
				};

				ProtocolExtensions.Register(
					0xFF,
					true,
					(state, pvSrc) =>
					{
						pvSrc.Trace(state);

						if (state == null || state.Mobile == null || !state.Running)
						{
							return;
						}

						Mobile m = state.Mobile;

						if (!_Dictionary.ContainsKey(m))
						{
							return;
						}

						Timer t = _Dictionary[m];

						if (t != null)
						{
							t.Stop();
						}

						_Dictionary.Remove(m);
					});
			}

			private static void OnHandshakeResponse(NetState state, PacketReader pvSrc)
			{
				pvSrc.Trace(state);

				if (state == null || state.Mobile == null || !state.Running)
				{
					return;
				}

				Mobile m = state.Mobile;

				if (!_Dictionary.ContainsKey(m))
				{
					return;
				}

				Timer t = _Dictionary[m];

				if (t != null)
				{
					t.Stop();
				}

				_Dictionary.Remove(m);
			}

			private static void OnHandshakeTimeout(object state)
			{
				Mobile m = state as Mobile;

				if (m == null)
				{
					return;
				}

				Timer t;

				if (_Dictionary.TryGetValue(m, out t) && t != null)
				{
					t.Stop();
				}

				_Dictionary.Remove(m);

				if (!Settings.KickOnFailure)
				{
					Console.WriteLine("Player '{0}' failed to negotiate features.", m);
				}
				else if (m.NetState != null && m.NetState.Running)
				{
					m.SendGump(new WarningGump(1060635, 30720, Settings.WarningMessage, 0xFFC000, 420, 250, null, null));

					if (m.AccessLevel <= AccessLevel.Player)
					{
						_Dictionary[m] = Timer.DelayCall(Settings.DisconnectDelay, OnForceDisconnect, m);
					}
				}
			}

			private static void OnForceDisconnect(object state)
			{
				if (!(state is Mobile))
				{
					return;
				}

				Mobile m = (Mobile)state;

				if (m.NetState != null && m.NetState.Running)
				{
					m.NetState.Dispose();
				}

				_Dictionary.Remove(m);

				Console.WriteLine("Player {0} kicked (Failed assistant handshake)", m);
			}

			private sealed class BeginHandshake : ProtocolExtension
			{
				public BeginHandshake()
					: base(0xFE, 8)
				{
					m_Stream.Write((uint)((ulong)Settings.DisallowedFeatures >> 32));
					m_Stream.Write((uint)((ulong)Settings.DisallowedFeatures & 0xFFFFFFFF));
				}
			}
		}
	}
}