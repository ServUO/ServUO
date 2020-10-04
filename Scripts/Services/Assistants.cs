#region References
using Server.Gumps;
using Server.Network;
using System;
using System.Collections.Generic;
#endregion References

namespace Server.Misc
{
    public static class Assistants
    {
        [Flags]
        public enum Features : ulong
        {
            None = 0,

            FilterWeather = 1ul << 0, // Weather Filter
            FilterLight = 1ul << 1, // Light Filter
            SmartTarget = 1ul << 2, // Smart Last Target
            RangedTarget = 1ul << 3, // Range Check Last Target
            AutoOpenDoors = 1ul << 4, // Automatically Open Doors
            DequipOnCast = 1ul << 5, // Unequip Weapon on spell cast
            AutoPotionEquip = 1ul << 6, // Un/Re-equip weapon on potion use
            PoisonedChecks = 1ul << 7, // Block heal If poisoned/Macro If Poisoned condition/Heal or Cure self
            LoopedMacros = 1ul << 8, // Disallow Looping macros, For loops, and macros that call other macros
            UseOnceAgent = 1ul << 9, // The use once agent
            RestockAgent = 1ul << 10, // The restock agent
            SellAgent = 1ul << 11, // The sell agent
            BuyAgent = 1ul << 12, // The buy agent
            PotionHotkeys = 1ul << 13, // All potion hotkeys
            RandomTargets = 1ul << 14, // All random target hotkeys (not target next, last target, target self)
            ClosestTargets = 1ul << 15, // All closest target hotkeys
            OverheadHealth = 1ul << 16, // Health and Mana/Stam messages shown over player's heads

            // AssistUO Only
            AutolootAgent = 1ul << 17, // The autoloot agent

            BoneCutterAgent = 1ul << 18, // The bone cutter agent
            JScriptMacros = 1ul << 19, // Javascript macro engine
            AutoRemount = 1ul << 20, // Auto remount after dismount

            All = ~None // Every feature possible
        }

        public class Settings
        {
            /// <summary>
            ///     Enable assistant negotiator?
            /// </summary>
            public static bool Enabled { get; set; } = false;

            /// <summary>
            ///     When true, this will cause anyone who does not negotiate.
            ///     (include those not running allowed assistants at all) to be disconnected from the server.
            /// </summary>
            public static bool KickOnFailure { get; set; } = true;

            public static Features DisallowedFeatures { get; private set; } = Features.None;

            /// <summary>
            ///     How long to wait for a handshake response before showing warning and disconnecting.
            /// </summary>
            public static TimeSpan HandshakeTimeout { get; set; } = TimeSpan.FromSeconds(30.0);

            /// <summary>
            ///     How long to show warning message before they are disconnected.
            /// </summary>
            public static TimeSpan DisconnectDelay { get; set; } = TimeSpan.FromSeconds(15.0);

            public static string WarningMessage { get; set; } = "The server was unable to negotiate features with your assistant. You must download and run an updated version of <A HREF='http://www.runuo.com/products/assistuo'>AssistUO</A> or <A HREF='http://www.runuo.com/products/razor'>Razor</A>.<BR><BR>Make sure you've checked the option <B>Negotiate features with server</B>, once you have this box checked you may log in and play normally.<BR><BR>You will be disconnected shortly.";

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
                    DisallowedFeatures |= feature;
                else
                    DisallowedFeatures &= ~feature;
            }
        }

        public class Negotiator
        {
            private static readonly Dictionary<Mobile, Timer> _Handshakes = new Dictionary<Mobile, Timer>();

            public static void Initialize()
            {
                if (!Settings.Enabled) return;
                EventSink.Login += OnLogin;

                ProtocolExtensions.Register(0xFF, true, OnResponse);
            }

            private static void OnLogin(LoginEventArgs e)
            {
                var m = e.Mobile;

                if (m == null || m.NetState == null || !m.NetState.Running || m.NetState.IsEnhancedClient)
                    return;

                m.Send(new BeginHandshake());

                if (_Handshakes.TryGetValue(m, out var t))
                    t?.Stop();

                _Handshakes[m] = Timer.DelayCall(Settings.HandshakeTimeout, OnTimeout, m);
            }

            private static void OnResponse(NetState state, PacketReader pvSrc)
            {
                if (state == null || state.Mobile == null || !state.Running) return;

                Mobile m = state.Mobile;

                if (!_Handshakes.TryGetValue(m, out var t))
                    return;

                t?.Stop();

                _Handshakes.Remove(m);
            }

            private static void OnTimeout(Mobile m)
            {
                if (m == null || !_Handshakes.TryGetValue(m, out var t)) return;

                t?.Stop();

                _Handshakes.Remove(m);

                if (Settings.KickOnFailure)
                {
                    if (m.NetState == null || !m.NetState.Running)
                        return;

                    m.SendGump(new WarningGump(1060635, 30720, Settings.WarningMessage, 0xFFC000, 420, 250, null, null));

                    if (m.AccessLevel <= AccessLevel.Player)
                        _Handshakes[m] = Timer.DelayCall(Settings.DisconnectDelay, OnForceDisconnect, m);
                }
                else
                    Console.WriteLine($"Player '{m}' failed to negotiate features.");
            }

            private static void OnForceDisconnect(Mobile m)
            {
                if (m == null) return;

                m.NetState?.Dispose();

                _Handshakes.Remove(m);

                Console.WriteLine($"Player {m} kicked (Failed assistant handshake)");
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
